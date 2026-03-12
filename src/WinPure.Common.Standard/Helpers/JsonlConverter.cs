using System.Collections.Generic;
using System.Data;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WinPure.Common.Helpers;

public class JsonlConversionResult
{
    public DataTable Table { get; set; }
    // Document schema is stored as a serialized JSONL schema string.
    public string DocumentSchema { get; set; }
}

public class JsonFieldSchema
{
    public string Name { get; set; }
    // FullPath is kept for compatibility but will be equal to Name (no prefixes)
    public string FullPath { get; set; }
    // Field type: "primitive", "primitiveArray", "object", "objectArray"
    public string FieldType { get; set; }
    // If the field is an object or an array of objects, its child fields.
    public List<JsonFieldSchema> Children { get; set; } = new List<JsonFieldSchema>();
}

public class JsonDocumentSchema
{
    public List<JsonFieldSchema> Fields { get; set; } = new List<JsonFieldSchema>();
}

public class JsonlConverter
{
    public static JsonlConversionResult ConvertJsonlToDataTable(string filePath)
    {
        var table = new DataTable();
        var docSchema = new JsonDocumentSchema();
        var jsonObjects = new List<JObject>();

        foreach (var line in File.ReadLines(filePath))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var obj = JObject.Parse(line);
            jsonObjects.Add(obj);
            UpdateDocumentSchema(obj, docSchema);
        }

        CreateTableColumnsFromDocumentSchema(docSchema, table);

        foreach (var obj in jsonObjects)
        {
            var row = table.NewRow();
            FillDataRowFromJObject(obj, row);
            table.Rows.Add(row);
        }

        var docSchemaStr = JsonConvert.SerializeObject(docSchema);
        return new JsonlConversionResult { Table = table, DocumentSchema = docSchemaStr };
    }

    public static Dictionary<long, string> ConvertDataTableToJsonl(JsonlConversionResult conversionResult)
    {
        var table = conversionResult.Table;
        var docSchema = JsonConvert.DeserializeObject<JsonDocumentSchema>(conversionResult.DocumentSchema);
        var result = new Dictionary<long, string>();

        foreach (DataRow row in table.Rows)
        {
            var key = Convert.ToInt64(row[WinPureColumnNamesHelper.WPCOLUMN_PRIMARY_KEY]);
            var outputObj = new JObject();
            // Add mandatory field WinPurePrimK as the first property.
            if (row.Table.Columns.Contains("WinPurePrimK"))
            {
                outputObj.Add("WinPurePrimK", Convert.ToInt64(row["WinPurePrimK"]));
            }
            // Add mandatory field WinPurePrimK as the first property.
            if (row.Table.Columns.Contains("OriginalRecordId"))
            {
                outputObj.Add("OriginalRecordId", row["OriginalRecordId"].ToString());
            }
            // Reconstruct the remaining object using the schema.
            var bodyObj = ReconstructObjectFromSchema(docSchema, row);

            if (bodyObj.ContainsKey("WinPurePrimK"))
            {
                bodyObj.Remove("WinPurePrimK");
            }

            if (bodyObj.ContainsKey("OriginalRecordId"))
            {
                bodyObj.Remove("OriginalRecordId");
            }

            foreach (var prop in bodyObj.Properties())
            {
                // Only add non-empty properties.
                if (prop.Value.Type == JTokenType.String)
                {
                    if (!IsEmpty(prop.Value.ToString()))
                        outputObj.Add(prop.Name, prop.Value);
                }
                else if (prop.Value.Type == JTokenType.Array)
                {
                    var arr = (JArray)prop.Value;
                    if (arr.HasValues)
                        outputObj.Add(prop.Name, prop.Value);
                }
                else if (prop.Value.Type == JTokenType.Object)
                {
                    var objVal = (JObject)prop.Value;
                    if (objVal.HasValues)
                        outputObj.Add(prop.Name, prop.Value);
                }
                else
                {
                    outputObj.Add(prop.Name, prop.Value);
                }
            }
            result.Add(key, outputObj.ToString(Formatting.None));
        }
        
        return result;
    }

    private static void UpdateDocumentSchema(JObject token, JsonDocumentSchema schema)
    {
        foreach (var property in token.Properties())
        {
            var fieldName = property.Name; // no prefix
            var existingField = schema.Fields.FirstOrDefault(f => f.Name == fieldName);
            if (existingField == null)
            {
                var newField = new JsonFieldSchema { Name = fieldName, FullPath = fieldName };
                if (property.Value is JObject obj)
                {
                    newField.FieldType = "object";
                    UpdateDocumentSchema(obj, new JsonDocumentSchema { Fields = newField.Children });
                }
                else if (property.Value is JArray array)
                {
                    if (array.HasValues && array.First is JObject)
                    {
                        newField.FieldType = "objectArray";
                        foreach (var element in array)
                        {
                            if (element is JObject elementObj)
                            {
                                UpdateDocumentSchema(elementObj, new JsonDocumentSchema { Fields = newField.Children });
                            }
                        }
                    }
                    else
                    {
                        newField.FieldType = "primitiveArray";
                    }
                }
                else
                {
                    newField.FieldType = "primitive";
                }
                schema.Fields.Add(newField);
            }
            else
            {
                // Field already exists: merge new keys if the value is a complex type.
                if (property.Value is JObject obj && (existingField.FieldType == "object" || existingField.FieldType == "objectArray"))
                {
                    UpdateDocumentSchema(obj, new JsonDocumentSchema { Fields = existingField.Children });
                }
                else if (property.Value is JArray array && array.HasValues && array.First is JObject)
                {
                    if (existingField.FieldType == "objectArray")
                    {
                        foreach (var element in array)
                        {
                            if (element is JObject elementObj)
                            {
                                UpdateDocumentSchema(elementObj, new JsonDocumentSchema { Fields = existingField.Children });
                            }
                        }
                    }
                }
            }
        }
    }

    private static void CreateTableColumnsFromDocumentSchema(JsonDocumentSchema schema, DataTable table)
    {
        foreach (var field in schema.Fields)
        {
            if (field.FieldType == "primitive" || field.FieldType == "primitiveArray")
            {
                if (!table.Columns.Contains(field.Name))
                {
                    table.Columns.Add(field.Name, typeof(string));
                }
            }
            else if (field.FieldType == "object" || field.FieldType == "objectArray")
            {
                // Do not add a column for the complex object itself.
                // Instead, recursively add columns for its child fields.
                CreateTableColumnsFromDocumentSchema(new JsonDocumentSchema { Fields = field.Children }, table);
            }
        }
    }

    private static void FillDataRowFromJObject(JObject obj, DataRow row)
    {
        foreach (var property in obj.Properties())
        {
            var key = property.Name; // no prefix
            var value = property.Value;

            if (value is JArray array)
            {
                if (array.HasValues && array.First is JObject)
                {
                    // For arrays of objects, flatten their nested properties.
                    var nestedValues = new Dictionary<string, List<string>>();
                    foreach (var token in array)
                    {
                        if (token is JObject nestedObj)
                            FillNestedValuesForArray(nestedObj, nestedValues);
                    }
                    // Assign collected values to corresponding columns.
                    foreach (var kvp in nestedValues)
                    {
                        if (row.Table.Columns.Contains(kvp.Key))
                        {
                            row[kvp.Key] = string.Join(Environment.NewLine, kvp.Value);
                        }
                    }
                }
                else
                {
                    // For arrays of primitives, join values using a newline as separator.
                    if (row.Table.Columns.Contains(key))
                    {
                        var values = new List<string>();
                        foreach (var item in array)
                        {
                            values.Add(item.ToString());
                        }
                        row[key] = string.Join(Environment.NewLine, values);
                    }
                }
            }
            else if (value is JObject nested)
            {
                // Process nested objects recursively.
                FillDataRowFromJObject(nested, row);
            }
            else
            {
                if (row.Table.Columns.Contains(key))
                {
                    row[key] = value.ToString();
                }
            }
        }
    }

    private static void FillNestedValuesForArray(JObject obj, Dictionary<string, List<string>> nestedValues)
    {
        foreach (var property in obj.Properties())
        {
            string key = property.Name; // no prefix for nested fields
            var value = property.Value;
            if (value is JObject nested)
            {
                FillNestedValuesForArray(nested, nestedValues);
            }
            else if (value is JArray innerArray)
            {
                if (innerArray.HasValues && innerArray.First is JObject)
                {
                    foreach (var token in innerArray)
                    {
                        if (token is JObject innerObj)
                            FillNestedValuesForArray(innerObj, nestedValues);
                    }
                }
                else
                {
                    string joined = string.Join(Environment.NewLine, innerArray.Select(v => v.ToString()));
                    if (!nestedValues.ContainsKey(key))
                        nestedValues[key] = new List<string>();
                    nestedValues[key].Add(joined);
                }
            }
            else
            {
                if (!nestedValues.ContainsKey(key))
                    nestedValues[key] = new List<string>();
                nestedValues[key].Add(value.ToString());
            }
        }
    }

    private static bool IsEmpty(string val)
    {
        return string.IsNullOrWhiteSpace(val);
    }

    private static JObject ReconstructObjectFromSchema(JsonDocumentSchema schema, DataRow row)
    {
        var obj = new JObject();
        foreach (var field in schema.Fields)
        {
            if (field.FieldType == "primitive")
            {
                if (row.Table.Columns.Contains(field.Name))
                {
                    var val = row[field.Name].ToString();
                    if (!IsEmpty(val))
                    {
                        obj[field.Name] = val.Contains(Environment.NewLine)
                            ? (JToken)new JArray(val.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Where(s => !string.IsNullOrWhiteSpace(s)))
                            : val;
                    }
                }
            }
            else if (field.FieldType == "primitiveArray")
            {
                if (row.Table.Columns.Contains(field.Name))
                {
                    var val = row[field.Name].ToString();
                    if (!IsEmpty(val))
                    {
                        var parts = val.Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                                       .Where(s => !string.IsNullOrWhiteSpace(s))
                                       .ToArray();
                        if (parts.Any())
                            obj[field.Name] = new JArray(parts);
                    }
                }
            }
            else if (field.FieldType == "object")
            {
                // Reconstruct the nested object from its child fields.
                var childObj = new JObject();
                foreach (var child in field.Children)
                {
                    if (row.Table.Columns.Contains(child.Name))
                    {
                        var val = row[child.Name].ToString();
                        if (!IsEmpty(val))
                        {
                            childObj[child.Name] = val.Contains(Environment.NewLine)
                                ? (JToken)new JArray(val.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Where(s => !string.IsNullOrWhiteSpace(s)))
                                : val;
                        }
                    }
                }
                if (childObj.HasValues)
                    obj[field.Name] = childObj;
            }
            else if (field.FieldType == "objectArray")
            {
                // Reconstruct an array of objects from flattened child fields.
                var maxCount = 0;
                var childValuesDict = new Dictionary<string, string[]>();
                foreach (var child in field.Children)
                {
                    if (row.Table.Columns.Contains(child.Name))
                    {
                        var val = row[child.Name].ToString();
                        if (!IsEmpty(val))
                        {
                            var arr = val.Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                                         .Where(s => !string.IsNullOrWhiteSpace(s))
                                         .ToArray();
                            childValuesDict[child.Name] = arr;
                            if (arr.Length > maxCount)
                                maxCount = arr.Length;
                        }
                    }
                }

                if (maxCount > 0)
                {
                    var arrObj = new JArray();
                    for (var i = 0; i < maxCount; i++)
                    {
                        var itemObj = new JObject();
                        foreach (var kvp in childValuesDict)
                        {
                            var childVal = (i < kvp.Value.Length) ? kvp.Value[i] : "";
                            if (!IsEmpty(childVal))
                                itemObj[kvp.Key] = childVal;
                        }
                        if (itemObj.HasValues)
                            arrObj.Add(itemObj);
                    }
                    if (arrObj.HasValues)
                        obj[field.Name] = arrObj;
                }
            }
        }
        return obj;
    }
}