namespace ABXY.Layers.Runtime.Autodoc
{
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple =false)]
    public class NodeDoc : System.Attribute
    {
        public string description;

        public NodeDoc(string description)
        {
            this.description = description;
        }
    }
}
