namespace ABXY.Layers.Editor.Curve_Editor.Wrappers
{
    public static class MathUtilsWrapper
    {
        private static System.Type EditorGUILayoutType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.MathUtils");


        public static float DiscardLeastSignificantDecimal(float v)
        {
            return (float)EditorGUILayoutType.GetMethod("DiscardLeastSignificantDecimal", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).Invoke(null, new object[] { v });
        }

        public static double DiscardLeastSignificantDecimal(double v)
        {
            return (double)EditorGUILayoutType.GetMethod("DiscardLeastSignificantDecimal", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).Invoke(null, new object[] { v });
        }
    }
}
