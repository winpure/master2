using System.Collections.Generic;

namespace WinPure.Common.Models
{
    [Serializable]
    internal class ImportedDataInfo
    {
        public ImportedDataInfo()
        {
            Fields = new List<DataField>();
        }

        public string TableName { get; set; }
        public string FileName { get; set; }
        public string DisplayName { get; set; }
        public int RowCount { get; set; }
        public bool IsStatisticCalculated { get; set; }
        public bool IsSelected { get; set; }
        public bool IsResolutionSelected { get; set; }
        public bool IsErMainTable { get; set; }
        public bool IsUndoAvailable { get; set; }
        public string ImportParameters { get; set; }
        public ExternalSourceTypes SourceType { get; set; }

        public List<DataField> Fields { get; set; }
        public string AdditionalInfo { get; set; }
    }
}