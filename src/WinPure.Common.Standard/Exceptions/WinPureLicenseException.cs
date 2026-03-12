namespace WinPure.Common.Exceptions
{
    public class WinPureLicenseException : WinPureBaseException
    {
        public WinPureLicenseException() : base(Properties.Resources.API_EXCEPTION_LICENSE)
        {

        }
        public WinPureLicenseException(string message) : base(message)
        {

        }
    }
}