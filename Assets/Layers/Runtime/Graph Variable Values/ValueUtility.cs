using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ABXY.Layers.Runtime.Graph_Variable_Values
{
    public static class ValueUtility
    {
        

        private static Dictionary<string, GraphVariableValue> _nonreferenceable = new Dictionary<string, GraphVariableValue>();
        private static Dictionary<string, GraphVariableValue> nonreferenceable
        {
            get
            {
                if (_nonreferenceable.Count == 0)
                    LoadGraphVarValues();
                return _nonreferenceable;
            }
        }

        private static Dictionary<string, GraphVariableValue> _addable = new Dictionary<string, GraphVariableValue>();
        private static Dictionary<string, GraphVariableValue> addable
        {
            get
            {
                if (_addable.Count == 0)
                    LoadGraphVarValues();
                return _addable;
            }
        }

        private static Dictionary<string, GraphVariableValue> _subtractable = new Dictionary<string, GraphVariableValue>();
        private static Dictionary<string, GraphVariableValue> subtractable
        {
            get
            {
                if (_subtractable.Count == 0)
                    LoadGraphVarValues();
                return _subtractable;
            }
        }

        private static Dictionary<string, GraphVariableValue> _dividable = new Dictionary<string, GraphVariableValue>();
        private static Dictionary<string, GraphVariableValue> dividable
        {
            get
            {
                if (_dividable.Count == 0)
                    LoadGraphVarValues();
                return _dividable;
            }
        }

        private static Dictionary<string, GraphVariableValue> _secondaryDividable = new Dictionary<string, GraphVariableValue>();
        private static Dictionary<string, GraphVariableValue> secondaryDividable
        {
            get
            {
                if (_secondaryDividable.Count == 0)
                    LoadGraphVarValues();
                return _secondaryDividable;
            }
        }

        private static Dictionary<string, GraphVariableValue> _multipliable = new Dictionary<string, GraphVariableValue>();
        private static Dictionary<string, GraphVariableValue> multipliable
        {
            get
            {
                if (_multipliable.Count == 0)
                    LoadGraphVarValues();
                return _multipliable;
            }
        }

        private static Dictionary<string, GraphVariableValue> _secondaryMultipliable = new Dictionary<string, GraphVariableValue>();
        private static Dictionary<string, GraphVariableValue> secondaryMultipliable
        {
            get
            {
                if (_secondaryMultipliable.Count == 0)
                    LoadGraphVarValues();
                return _secondaryMultipliable;
            }
        }

        private static Dictionary<string, GraphVariableValue> _splittable = new Dictionary<string, GraphVariableValue>();
        private static Dictionary<string, GraphVariableValue> splittable
        {
            get
            {
                if (_splittable.Count == 0)
                    LoadGraphVarValues();
                return _splittable;
            }
        }

        private static Dictionary<string, GraphVariableValue> _combineable = new Dictionary<string, GraphVariableValue>();
        private static Dictionary<string, GraphVariableValue> combineable
        {
            get
            {
                if (_combineable.Count == 0)
                    LoadGraphVarValues();
                return _combineable;
            }
        }

        private static Dictionary<string, GraphVariableValue> _everythingElse = new Dictionary<string, GraphVariableValue>();
        private static Dictionary<string, GraphVariableValue> everythingElse
        {
            get
            {
                if (_everythingElse.Count == 0)
                    LoadGraphVarValues();
                return _everythingElse;
            }
        }

        [System.Flags]
        public enum ValueFilter {
            nonreferenceable = 1 << 0,
            addable = 1 << 1,
            subtractable = 1 << 2,
            dividable = 1 << 3,
            secondaryDividable = 1 << 4,
            multipliable = 1 << 5,
            secondaryMultipliable = 1 << 6,
            splittable = 1 << 7,
            combineable = 1 << 8,
            All = ~(-1 << 8)
        }

        private static Dictionary<string, GraphVariableValue> FilterKeyValues(ValueFilter filter)
        {
            Dictionary<string, GraphVariableValue> results = new Dictionary<string, GraphVariableValue>();
            if (filter.HasFlag(ValueFilter.nonreferenceable))
                ConcatenateDictionaries(results, nonreferenceable);
            if (filter.HasFlag(ValueFilter.addable))
                ConcatenateDictionaries(results, addable);
            if (filter.HasFlag(ValueFilter.subtractable))
                ConcatenateDictionaries(results, subtractable);
            if (filter.HasFlag(ValueFilter.dividable))
                ConcatenateDictionaries(results, dividable);
            if (filter.HasFlag(ValueFilter.secondaryDividable))
                ConcatenateDictionaries(results, secondaryDividable);
            if (filter.HasFlag(ValueFilter.multipliable))
                ConcatenateDictionaries(results, multipliable);
            if (filter.HasFlag(ValueFilter.secondaryMultipliable))
                ConcatenateDictionaries(results, secondaryMultipliable);
            if (filter.HasFlag(ValueFilter.splittable))
                ConcatenateDictionaries(results, splittable);
            if (filter.HasFlag(ValueFilter.combineable))
                ConcatenateDictionaries(results, combineable);
            if (filter.HasFlag(ValueFilter.All))
                ConcatenateDictionaries(results, everythingElse);
            return results;
        }


        private static void ConcatenateDictionaries(Dictionary<string, GraphVariableValue> to, Dictionary<string, GraphVariableValue> from)
        {
            foreach (KeyValuePair<string, GraphVariableValue> kv in from)
            {
                if (!to.ContainsKey(kv.Key))
                    to.Add(kv.Key, kv.Value);
            }
        }

        public static GraphVariableValue GetVariableValue(string typeName, ValueFilter filter)
        {
            Dictionary<string, GraphVariableValue> graphDictionary = FilterKeyValues(filter);
            GraphVariableValue value = null;
            graphDictionary.TryGetValue(typeName, out value);
            return value;
        }


        public static Dictionary<string, GraphVariableValue> GetVariableValues(ValueFilter filter)
        {
            return FilterKeyValues(filter);
        }


        public static List<string> GetManagedTypes(ValueFilter filter)
        {
            Dictionary<string, GraphVariableValue> graphDictionary = FilterKeyValues(filter);
       
            return graphDictionary.Keys.ToList();
        }

        private static bool loaded = false;
        private static void LoadGraphVarValues()
        {
            if (loaded) // Then already loaded
                return;

            loaded = true;

            foreach (System.Reflection.Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (System.Type type in assembly.GetTypes())
                {
                    if (type.BaseType == typeof(GraphVariableValue))
                    {
                        GraphVariableValue instance = (GraphVariableValue)System.Activator.CreateInstance(type);

                        bool addedToDictionary = false;
                        foreach (System.Type valInterface in type.GetInterfaces())
                        {

                            if (valInterface == typeof(AddableValue))
                            {
                                _addable.Add(instance.handlesType.FullName, instance);
                                addedToDictionary = true;
                            }
                            if (valInterface == typeof(SubtractableValue))
                            {
                                _subtractable.Add(instance.handlesType.FullName, instance);
                                addedToDictionary = true;
                            }
                            if (valInterface == typeof(DividableValue))
                            {
                                _dividable.Add(instance.handlesType.FullName, instance);
                                addedToDictionary = true;
                            }
                            if (valInterface == typeof(SecondaryDividableValue))
                            {
                                _secondaryDividable.Add(instance.handlesType.FullName, instance);
                                addedToDictionary = true;
                            }
                            if (valInterface == typeof(MultipliableValue))
                            {
                                _multipliable.Add(instance.handlesType.FullName, instance);
                                addedToDictionary = true;
                            }
                            if (valInterface == typeof(SecondaryMultipliableValue))
                            {
                                _secondaryMultipliable.Add(instance.handlesType.FullName, instance);
                                addedToDictionary = true;
                            }
                            if (valInterface == typeof(SplittableValue))
                            {
                                _splittable.Add(instance.handlesType.FullName, instance);
                                addedToDictionary = true;
                            }
                            if (valInterface == typeof(CombinableValue))
                            {
                                _combineable.Add(instance.handlesType.FullName, instance);
                                addedToDictionary = true;
                            }
                            
                        }

                        if (!addedToDictionary)
                            _everythingElse.Add(instance.handlesType.FullName, instance);




                        if (instance.IsNonreferenceableType())
                            nonreferenceable.Add(instance.handlesType.FullName, instance);
                        

                    }
                }
            }
        }
    }
}
