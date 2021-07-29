using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace ABXY.Layers.Editor
{
    public static class SerializedPropertyUtils
    {
        /// <summary>
        /// This function is taken from https://github.com/lordofduct/spacepuppy-unity-framework. Its use is permitted under the following license:
        /// 
        /// Copyright (c) 2015, Dylan Engelman, Jupiter Lighthouse Studio
        /// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files(the "Software"), 
        /// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
        /// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
        /// 
        /// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
        /// 
        /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
        /// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
        /// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static object GetPropertyObject(SerializedProperty property)
        {
            if (property == null) return null;

            string originalPath = property.propertyPath;
            object obj = property.serializedObject.targetObject;


            //Added caching to speed up this function. It was a pretty bad bottleneck
            CachedProperty cachedProperty;
            lock (propertyCache)
            {
                if (propertyCache.TryGetValue(property.serializedObject.targetObject.GetInstanceID()+ originalPath, out cachedProperty))
                {
                    CachedProperty currentProperty = cachedProperty;

                    object currentObject = obj;

                    while (currentProperty.IsValid)
                    {
                        if (currentProperty.isArrayElement)
                            currentObject = GetValue_Imp(currentObject, currentProperty.elementName, currentProperty.index, ref currentProperty);
                        else
                            currentObject = GetValue_Imp(currentObject, currentProperty.elementName, ref currentProperty);

                        currentProperty = currentProperty.subProperties.FirstOrDefault();
                    }

                    obj = currentObject;

                }
                else
                {
                    string path = originalPath.Replace(".Array.data[", "[");
                    var elements = path.Split('.');

                    CachedProperty rootPropertyCache = new CachedProperty();
                    CachedProperty currentPropertyCache = new CachedProperty();

                    foreach (var element in elements)
                    {
                        CachedProperty newCachedProperty;
                        if (element.Contains("["))
                        {
                            var elementName = element.Substring(0, element.IndexOf("["));
                            var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                            newCachedProperty = new CachedProperty(elementName, index);
                            obj = GetValue_Imp(obj, elementName, index, ref newCachedProperty);

                        }
                        else
                        {
                            newCachedProperty = new CachedProperty(element);
                            obj = GetValue_Imp(obj, element, ref newCachedProperty);


                        }

                        if (!rootPropertyCache.IsValid)
                        {
                            rootPropertyCache = newCachedProperty;
                            currentPropertyCache = newCachedProperty;
                        }
                        else
                        {
                            currentPropertyCache.subProperties.Add(newCachedProperty);
                            currentPropertyCache = newCachedProperty;
                        }
                    }
                    propertyCache.Add(property.serializedObject.targetObject.GetInstanceID() + originalPath, rootPropertyCache);
                }
            }
            return obj;
        }

        private static Dictionary<string, CachedProperty> propertyCache = new Dictionary<string, CachedProperty>();

        private struct CachedProperty
        {
            public string elementName;
            public int index;
            public bool isArrayElement { get { return index >= 0; } }

            public List<CachedProperty> subProperties;

            public bool IsValid { get { return !string.IsNullOrEmpty(elementName); } }

            public FieldInfo cachedFieldInfo;

            public PropertyInfo cachedPropertyInfo;

            public bool IsMemberCached { get { return cachedFieldInfo != null || cachedPropertyInfo != null; } }

            public CachedProperty(string elementName, int index) : this()
            {
                this.elementName = elementName ?? throw new System.ArgumentNullException(nameof(elementName));
                this.index = index;
                subProperties = new List<CachedProperty>();
            }

            public CachedProperty(string elementName) : this()
            {
                this.elementName = elementName ?? throw new System.ArgumentNullException(nameof(elementName));
                this.index = -400;
                subProperties = new List<CachedProperty>();
            }
        }

        /// <summary>
        /// This function is taken from https://github.com/lordofduct/spacepuppy-unity-framework. Its use is permitted under the following license:
        /// 
        /// Copyright (c) 2015, Dylan Engelman, Jupiter Lighthouse Studio
        /// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files(the "Software"), 
        /// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
        /// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
        /// 
        /// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
        /// 
        /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
        /// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
        /// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static object GetValue_Imp(object source, string name, ref CachedProperty cachedProp)
        {
            if (source == null)
                return null;

            if (cachedProp.cachedFieldInfo != null)
                return cachedProp.cachedFieldInfo.GetValue(source);

            if (cachedProp.cachedPropertyInfo != null)
                return cachedProp.cachedPropertyInfo.GetValue(source);

            var type = source.GetType();

            while (type != null)
            {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                {
                    cachedProp.cachedFieldInfo = f;
                    return f.GetValue(source);
                }

                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                {
                    cachedProp.cachedPropertyInfo = p;
                    return p.GetValue(source, null);
                }

                type = type.BaseType;
            }
            return null;
        }

        /// <summary>
        /// This function is taken from https://github.com/lordofduct/spacepuppy-unity-framework. Its use is permitted under the following license:
        /// 
        /// Copyright (c) 2015, Dylan Engelman, Jupiter Lighthouse Studio
        /// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files(the "Software"), 
        /// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
        /// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
        /// 
        /// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
        /// 
        /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
        /// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
        /// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="name"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static object GetValue_Imp(object source, string name, int index, ref CachedProperty cachedProp)
        {
            var enumerable = GetValue_Imp(source, name, ref cachedProp) as System.Collections.IEnumerable;
            if (enumerable == null) return null;
            var enm = enumerable.GetEnumerator();
            //while (index-- >= 0)
            //    enm.MoveNext();
            //return enm.Current;

            for (int i = 0; i <= index; i++)
            {
                if (!enm.MoveNext()) return null;
            }
            return enm.Current;
        }
    }
}
