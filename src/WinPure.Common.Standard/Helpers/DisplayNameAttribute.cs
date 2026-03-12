namespace WinPure.Common.Helpers
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DisplayNameAttribute : Attribute
    {
        public DisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }

        public string DisplayName { get; private set; }
    }
}