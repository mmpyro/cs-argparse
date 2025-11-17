namespace ArgParse.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class CmdAttribute : Attribute
    {
        protected bool required = true;

        public string Name { get; set; }
        public string Description { get; set; }
        public bool Required { get => required; set => required = value; }
    }
}
