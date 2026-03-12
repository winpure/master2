namespace WinPure.Common.Exceptions
{
    public class WinPurePrimaryKeyNotFoundException : WinPureBaseException
    {
        public WinPurePrimaryKeyNotFoundException() : base("WinPure primary key not found")
        {

        }
    }
}