namespace RocketUI.Design
{
    public class RocketDesignerHostOptions
    {
        public virtual string File { get; set; }


        public virtual string[] AssemblySearchPaths { get; set; }
        public virtual string   AssemblyName        { get; set; }
        public virtual string   GuiRendererType     { get; set; }
        public virtual string   ProjectPath     { get; set; }
    }
}