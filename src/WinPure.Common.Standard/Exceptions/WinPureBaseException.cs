namespace WinPure.Common.Exceptions
{
    public class WinPureBaseException : Exception
    {
        public WinPureBaseException(string message) : base(message)
        {

        }

        public WinPureBaseException(string message, Exception exception) : base(message, exception)
        {

        }
    }
}