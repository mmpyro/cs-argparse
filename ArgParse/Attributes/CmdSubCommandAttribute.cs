namespace ArgParse.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CmdSubCommandAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
