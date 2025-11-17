namespace ArgParse.Attributes
{
    public class CmdParameterAttribute : CmdAttribute
    {
        public object Default { get; set; }
        public bool Multiple { get; set; }

    }
}
