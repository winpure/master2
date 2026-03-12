using WinPure.Common.Exceptions;

namespace WinPure.Matching.Models;

/// <summary>
/// Describe correspondence between fields in different tables when two or more tables could be joined.
/// </summary>
[Serializable]
public class FieldMapping
{
    private string _fieldName;

    /// <summary>
    /// Default constructor
    /// </summary>
    public FieldMapping()
    {
        FieldMap = new List<MatchField>();
    }

    /// <summary>
    /// Displayed field name. If not defined, then return field name from a first table. 
    /// </summary>
    public string FieldName
    {
        get
        {
            if (string.IsNullOrEmpty(_fieldName) && FieldMap.Any())
            {
                _fieldName = FieldMap.First().ColumnName;
            }
            return _fieldName;
        }
        set => _fieldName = value ?? throw new WinPureArgumentException($"Null value {nameof(value)}");
    }

    /// <summary>
    /// Map of fields from different tables, which should be joined
    /// </summary>
    public List<MatchField> FieldMap { get; }

    /// <summary>
    /// Represent system data type of given field
    /// </summary>
    public Type FieldType
    {
        get
        {
            var tp = FieldMap.Select(x => x.ColumnDataType).Distinct();
            var sTp = tp.FirstOrDefault();
            return tp.Count() > 1 || string.IsNullOrEmpty(sTp) ? typeof(string) : Type.GetType(sTp);
        }
    }
}