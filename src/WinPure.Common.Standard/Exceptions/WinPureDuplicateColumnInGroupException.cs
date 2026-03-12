using WinPure.Common.Properties;

namespace WinPure.Common.Exceptions
{
    public class WinPureDuplicateColumnInGroupException : WinPureBaseException
    {
        public WinPureDuplicateColumnInGroupException() : base(Resources.API_EXCEPTION_DUPLICATE_FIELD_IN_GROUP)
        {

        }
    }
}