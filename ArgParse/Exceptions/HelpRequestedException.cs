namespace ArgParse.Exceptions
{
    public class HelpRequestedException : Exception
    {
        public HelpRequestedException(string message) : base(message) { }
    }
}