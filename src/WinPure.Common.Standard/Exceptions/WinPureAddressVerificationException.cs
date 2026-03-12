namespace WinPure.Common.Exceptions
{
    public class WinPureAddressVerificationException : WinPureBaseException
    {
        public WinPureAddressVerificationException(string message) : base(message)
        {

        }

        public WinPureAddressVerificationException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}