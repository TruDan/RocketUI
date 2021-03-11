namespace RocketUI
{
    public class TextureResource : IResource
    {
        public string Namespace { get; }
        public string Key       { get; }
        public string Source    { get; set; }
    }
}