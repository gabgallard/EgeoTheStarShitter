namespace ABXY.Layers.Editor.Curve_Editor.Wrappers
{
    public class TickStyleWrapper
    {
        public static System.Type TickStyleType { get; private set; }
        object instance;

        public SkinnedColorWrapper tickColor
        {
            get
            {
                return new SkinnedColorWrapper( TickStyleType.GetField("tickColor").GetValue(instance));
            }
            set
            {
                TickStyleType.GetField("tickColor").SetValue(instance, value.GetWrappedObject());
            }
        }
        public SkinnedColorWrapper labelColor
        {
            get
            {
                return new SkinnedColorWrapper(TickStyleType.GetField("labelColor").GetValue(instance));
            }
            set
            {
                TickStyleType.GetField("labelColor").SetValue(instance, value.GetWrappedObject());
            }
        }
        public int distMin
        {
            get
            {
                return (int)TickStyleType.GetField("distMin").GetValue(instance);
            }
            set
            {
                TickStyleType.GetField("distMin").SetValue(instance, value);
            }
        }
        public int distFull
        {
            get
            {
                return (int)TickStyleType.GetField("distFull").GetValue(instance);
            }
            set
            {
                TickStyleType.GetField("distFull").SetValue(instance, value);
            }
        }
        public int distLabel
        {
            get
            {
                return (int)TickStyleType.GetField("distLabel").GetValue(instance);
            }
            set
            {
                TickStyleType.GetField("distLabel").SetValue(instance, value);
            }
        }

        public bool stubs
        {
            get
            {
                return (bool)TickStyleType.GetField("stubs").GetValue(instance);
            }
            set
            {
                TickStyleType.GetField("stubs").SetValue(instance, value);
            }
        }
        public bool centerLabel
        {
            get
            {
                return (bool)TickStyleType.GetField("centerLabel").GetValue(instance);
            }
            set
            {
                TickStyleType.GetField("centerLabel").SetValue(instance, value);
            }
        }
        public string unit
        {
            get
            {
                return (string)TickStyleType.GetField("unit").GetValue(instance);
            }
            set
            {
                TickStyleType.GetField("unit").SetValue(instance, value);
            }
        }

        public TickStyleWrapper()
        {
            TickStyleType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.TickStyle");
            instance = System.Activator.CreateInstance(TickStyleType);

        }

        public TickStyleWrapper(object instance)
        {
            TickStyleType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.TickStyle");
            this.instance = instance;

        }

        public object GetWrappedObject()
        {
            return instance;
        }
    }
}
