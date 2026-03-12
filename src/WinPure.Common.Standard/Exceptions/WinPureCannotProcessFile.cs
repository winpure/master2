namespace WinPure.Common.Exceptions
{
    public class WinPureCannotProcessFile : WinPureBaseException
    {
        public WinPureCannotProcessFile(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}