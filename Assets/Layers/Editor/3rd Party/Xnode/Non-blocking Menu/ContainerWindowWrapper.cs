using UnityEditor;

namespace ABXY.Layers.Editor.ThirdParty.Xnode
{
    public class ContainerWindowWrapper
    {
        private EditorWindow editorWindow;

        public ContainerWindowWrapper(EditorWindow editorWindow)
        {
            this.editorWindow = editorWindow;
        }

        public object GetWrappedWindow()
        {
            System.Type hostViewType = typeof(EditorWindow).Assembly.GetType("UnityEditor.HostView");

            object m_parent = typeof(EditorWindow).GetField("m_Parent", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(editorWindow);

            object window = hostViewType.GetProperty("window", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic).GetValue(m_parent);

            return window;
        }

        public static System.Type windowType
        {
            get
            {
                return typeof(EditorWindow).Assembly.GetType("UnityEditor.ContainerWindow");
            }
        }
    }
}
