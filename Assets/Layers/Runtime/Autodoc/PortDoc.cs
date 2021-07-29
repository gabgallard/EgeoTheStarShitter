namespace ABXY.Layers.Runtime.Autodoc
{
    public class PortDoc : System.Attribute
    {
        public string description { get; private set; }

        public PortDoc(string description)
        {
            this.description = description;
        }
    }
}
