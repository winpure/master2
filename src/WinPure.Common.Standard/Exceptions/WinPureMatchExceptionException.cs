using WinPure.Common.Properties;

namespace WinPure.Common.Exceptions
{
    public class WinPureEntityResolutionException : WinPureBaseException
    {
        public WinPureEntityResolutionException(string message) : base(message)
        {

        }
    }

    public class WinPureAPIWrongConditionException : WinPureBaseException
    {
        public WinPureAPIWrongConditionException(string tableName, string fieldName, string dataType, string searchType)
            : base(string.Format(Resources.API_EXCEPTION_WRONG_FIELD_TYPE, tableName, fieldName, searchType, dataType))
        {
        }
    }

    public class WinPureAPIWrongParametersException : WinPureBaseException
    {
        public WinPureAPIWrongParametersException() : base(Resources.API_EXCEPTION_PARAMETER_NOT_NULL)
        {

        }
    }

    public class WinPureAPINoTableException : WinPureBaseException
    {
        public WinPureAPINoTableException(string tableName) : base(string.Format(Resources.API_EXCEPTION_NO_TABLE_IN_THE_DATA, tableName))
        {

        }
    }

    public class WinPureAPINoFieldException : WinPureBaseException
    {
        public WinPureAPINoFieldException(string tableName, string fieldName)
            : base(string.Format(Resources.API_EXCEPTION_TABLE_NOT_CONTAINS_FIELD, tableName, fieldName))
        {

        }
    }
}