using System.Collections.Generic;
using System.Text;

namespace ABXY.Layers.Runtime
{
    public static class ReflectionUtils 
    {
        private static Dictionary<string, System.Type> typeName2Type = new Dictionary<string, System.Type>();

        public static System.Type FindType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                return null;

            string newTypeName = typeName.Replace("[]", "");
            bool isArray = newTypeName.Length != typeName.Length;

            System.Type foundType = null;
            if(!typeName2Type.TryGetValue(newTypeName, out foundType))
            {

                foundType = System.Type.GetType(newTypeName);

                if (foundType == null)
                { // then need to search
                    foreach (System.Reflection.Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                    {
                        foreach (System.Type type in assembly.GetTypes())
                        {
                            if (type.FullName == newTypeName)
                            {
                                foundType = type;
                                break;
                            }
                        }
                        if (foundType != null)
                            break;
                    }
                }
                typeName2Type.Add(newTypeName, foundType);
            }

            if (isArray)
                foundType = foundType.MakeArrayType();

            return foundType;
        }

        //Adapted from https://stackoverflow.com/questions/1120198/most-efficient-way-to-remove-special-characters-from-string
        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if (((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_') && c != ' ')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

    }

}
