namespace ArgParse.Exceptions
{
    public class RequiredAttributeException : ArgumentException
    {
        public RequiredAttributeException(string message) : base(message)
        {
        }
    }
}
