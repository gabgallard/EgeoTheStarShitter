using System;
using System.Collections.Generic;
using System.Linq;
using ABXY.Layers.Editor.Graph_Variable_Editors;
using ABXY.Layers.Runtime.Graph_Variable_Values;

namespace ABXY.Layers.Editor
{
    public static class VariableInspectorUtility
    {
        //private static Dictionary<string, System.Type> typeName2EditorType = new Dictionary<string, System.Type>();
        //private static Dictionary<string, System.Type> typeName2EditorTypeIncNonReferenceable = new Dictionary<string, System.Type>();
        private static Dictionary<string, TypePrettynamePair> typeName2PlayerEditorType = new Dictionary<string, TypePrettynamePair>();
        //private static List<string> playerEditorPrettyNames = new List<string>();

        private static Dictionary<string, TypePrettynamePair> typeName2InputNodeEditorType = new Dictionary<string, TypePrettynamePair>();
        //private static List<string> inputNodeEditorPrettyNames = new List<string>();

        private static Dictionary<string, TypePrettynamePair> typeName2CombineEditorType = new Dictionary<string, TypePrettynamePair>();
        //private static List<string> combinePrettyNames = new List<string>();

        private static Dictionary<string, TypePrettynamePair> typeName2SplitEditorType = new Dictionary<string, TypePrettynamePair>();
        //private static List<string> splitPrettyNames = new List<string>();

        private static Dictionary<string, TypePrettynamePair> typeName2OtherType = new Dictionary<string, TypePrettynamePair>();
        //private static List<string> otherTypePrettyNames = new List<string>();

        //private static Dictionary<string, string> fullnameToPrettyNames = new Dictionary<string, string>();
        //private static List<string> prettyNames = new List<string>();

        public struct TypePrettynamePair
        {
            public System.Type type { get; private set; }
            public string prettyName { get; private set; }

            public TypePrettynamePair(Type type, string prettyName)
            {
                this.type = type ?? throw new ArgumentNullException(nameof(type));
                this.prettyName = prettyName ?? throw new ArgumentNullException(nameof(prettyName));
            }
        }

        [System.Flags]
        public enum EditorFilter
        {
            Player = 1 << 0,
            InputNode = 1 << 1,
            Combineable = 1 << 2,
            Splittable = 1 << 3,
            Other = 1 << 4,
            All = ~(-1 << 4)
        }

        public static List<string> GetManagedTypes(EditorFilter filter)
        {
            LoadEditors();
            return FilterTypenames(filter).Keys.ToList();

        }


        public static List<System.Type> GetManagedEditors(EditorFilter filter)
        {
            LoadEditors();

            return FilterTypenames(filter).Values.Select(x => x.type).ToList();
        }


        public static System.Type GetEditorType(string targetTypeName, EditorFilter filter)
        {
            LoadEditors();
            Dictionary<string, TypePrettynamePair> editorDictionary = FilterTypenames(filter);

            TypePrettynamePair editorType = new TypePrettynamePair();
            editorDictionary.TryGetValue(targetTypeName, out editorType);


            return editorType.type;
        }

        /// <summary>
        /// optimization!
        /// </summary>
        private static Dictionary<EditorFilter, Dictionary<string, TypePrettynamePair>> filterCache = new Dictionary<EditorFilter, Dictionary<string, TypePrettynamePair>>();

        private static Dictionary<string, TypePrettynamePair> FilterTypenames(EditorFilter filter)
        {
            if (filterCache.ContainsKey(filter))
                return filterCache[filter];

            Dictionary<string, TypePrettynamePair> results = new Dictionary<string, TypePrettynamePair>();
            if (filter.HasFlag(EditorFilter.Player))
            {
                ConcatenateDictionaries(results, typeName2PlayerEditorType);
            }

            if (filter.HasFlag(EditorFilter.InputNode))
            {
                ConcatenateDictionaries(results, typeName2InputNodeEditorType);
            }

            if (filter.HasFlag(EditorFilter.Combineable))
            {
                ConcatenateDictionaries(results, typeName2CombineEditorType);
            }

            if (filter.HasFlag(EditorFilter.Splittable))
            {
                ConcatenateDictionaries(results, typeName2SplitEditorType);
            }

            if (filter.HasFlag(EditorFilter.Other))
            {
                ConcatenateDictionaries(results, typeName2OtherType);
            }
            filterCache.Add(filter, results);

            return results;
        }

        private static void ConcatenateDictionaries(Dictionary<string, TypePrettynamePair> to, Dictionary<string, TypePrettynamePair> from)
        {
            foreach (KeyValuePair<string, TypePrettynamePair> kv in from)
            {
                if (!to.ContainsKey(kv.Key))
                    to.Add(kv.Key, kv.Value);
            }
        }

        /// <summary>
        /// optimization!
        /// </summary>
        private static Dictionary<EditorFilter, List<string>> prettyNameQueryCache = new Dictionary<EditorFilter, List<string>>();

        private static List<string> FilterPrettyNames(EditorFilter filter)
        {
            if (prettyNameQueryCache.ContainsKey(filter))
            {
                return prettyNameQueryCache[filter];
            }
            else
            {

                List<string> results = new List<string>();
                Dictionary<string, TypePrettynamePair> inspectors = FilterTypenames(filter);
                results = inspectors.Select(x => x.Value.prettyName).ToList();
                prettyNameQueryCache.Add(filter, results);
                return results;
            }


        }


        private static bool loaded = false;


        private static void LoadEditors()
        {
            if (loaded) // Then already loaded
                return;

            loaded = true;

            foreach (System.Reflection.Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (System.Type type in assembly.GetTypes())
                {
                    if (type.BaseType == typeof(GraphVariableEditor))
                    {

                        GraphVariableEditor instance = (GraphVariableEditor)System.Activator.CreateInstance(type);

                        List<System.Type> interfaces = new List<System.Type>(type.GetInterfaces());

                        string prettyName = instance.GetPrettyTypeName();

                        bool includedInOtherCategories = false;
                        if (interfaces.Contains(typeof(PlayerInspector)))
                        {
                            typeName2PlayerEditorType.Add(instance.handlesType.FullName, new TypePrettynamePair(type, prettyName));
                            includedInOtherCategories = true;
                        }

                        if (interfaces.Contains(typeof(InputNodeInspector)))
                        {

                            typeName2InputNodeEditorType.Add(instance.handlesType.FullName, new TypePrettynamePair(type, prettyName));
                            includedInOtherCategories = true;
                        }

                        if (interfaces.Contains(typeof(CombinableInspector)))
                        {

                            GraphVariableValue value = ValueUtility.GetVariableValue(instance.handlesType.FullName, ValueUtility.ValueFilter.All);
                            bool hasValue = value is CombinableValue;
                            if (hasValue)
                            {
                                typeName2CombineEditorType.Add(instance.handlesType.FullName, new TypePrettynamePair(type, prettyName));
                                includedInOtherCategories = true;
                            }
                        }

                        if (interfaces.Contains(typeof(SplittableInspector)))
                        {

                            GraphVariableValue value = ValueUtility.GetVariableValue(instance.handlesType.FullName, ValueUtility.ValueFilter.All);
                            bool hasValue = value is SplittableValue;
                            if (hasValue)
                            {
                                typeName2SplitEditorType.Add(instance.handlesType.FullName, new TypePrettynamePair(type, prettyName));
                                includedInOtherCategories = true;
                            }
                        }

                        if (!includedInOtherCategories)
                        {
                            typeName2OtherType.Add(instance.handlesType.FullName, new TypePrettynamePair(type, prettyName));
                        }

                    }
                }
            }
        }



        public static string GetPrettyName(string typeName)
        {
            LoadEditors();
            TypePrettynamePair result = new TypePrettynamePair();
            FilterTypenames(EditorFilter.All).TryGetValue(typeName, out result);
            return result.prettyName;
        }

        public static List<string> GetPrettyNames(EditorFilter filter)
        {
            return FilterPrettyNames(filter);
        }

    }
}
