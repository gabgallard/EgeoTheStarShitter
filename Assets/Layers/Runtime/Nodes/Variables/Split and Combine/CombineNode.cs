using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Graph_Variable_Values;
using UnityEngine;

namespace ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine
{
    [Node.CreateNodeMenu("Variables/Combine")]
    public class CombineNode : CombineSplitBase
    {

        [SerializeField]
        private string typeName = "";


        private static Dictionary<string, GraphVariableValue> _typeName2Value = new Dictionary<string, GraphVariableValue>();
        private static Dictionary<string, GraphVariableValue> typeName2Value
        {
            get
            {
                if (_typeName2Value.Count == 0)
                    LoadGraphVarValues();
                return _typeName2Value;
            }
        }

        private Dictionary<string, object> defaultValues = new Dictionary<string, object>();
        [SerializeField]
        private List<string> defaultValuesKeys = new List<string>();
        [SerializeField]
        private List<string> defaultValuesValues = new List<string>();
        [SerializeField]
        private List<string> defaultValuestypes = new List<string>();

        [SerializeField]
        public List<GraphVariable> parameters = new List<GraphVariable>();
        private static void LoadGraphVarValues()
        {
            if (_typeName2Value.Count != 0) // Then already loaded
                return;

            _typeName2Value = ValueUtility.GetVariableValues(ValueUtility.ValueFilter.All);
        }

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

        }

        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            GraphVariableValue value = null;
            if (typeName2Value.TryGetValue(typeName, out value))
            {
                if (value is CombineValuePlayEvent)
                    (value as CombineValuePlayEvent).PlayAtDSPTime(this, calledBy, time, data, nodesCalledThisFrame);
            }
        }

        public override void Stop(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            base.Stop(calledBy, time, data, nodesCalledThisFrame);
            StopAllCoroutines();
        }


        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {

            GraphVariableValue value = null;
            if (typeName2Value.TryGetValue(typeName, out value))
            {
                return (value as CombinableValue).GetCombineValue(port, this);
            }
            return null;
        }


        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            GraphVariableValue value = null;
            if (typeName2Value.TryGetValue(typeName, out value))
            {
                if (value is CombineNodeParamSource)
                {
                    visitedNodes.Add(this);
                    return (value as CombineNodeParamSource).GetOutGoingEventParametersOnPort(this, port, visitedNodes);
                }
            }

            return new List<GraphEvent.EventParameterDef>();
        }

    

        public T GetDefaultValueTyped<T>(string valueName, T defaultValue)
        {
            return (T)GetDefaultValue(valueName, defaultValue);
        }

        public object GetDefaultValue(string valueName, object defaultValue)
        {
            object foundValue = null;
            defaultValues.TryGetValue(valueName, out foundValue);
            if (foundValue != null)
                defaultValue = foundValue;
            return defaultValue;
        }

        public void SetDefaultValue(string valueName, object value)
        {
            if (defaultValues.ContainsKey(valueName))
                defaultValues[valueName] = value;
        }

        public void ClearDefaults()
        {
            defaultValues.Clear();
            defaultValuesKeys.Clear();
            defaultValuesValues.Clear();
            defaultValuestypes.Clear();
        }

        public void BuildDefaults()
        {
            foreach (NodePort port in DynamicPorts) {
                if (typeof(Object).IsAssignableFrom(port.ValueType))
                {
                    if (defaultValues.ContainsKey(port.fieldName))
                        defaultValues[port.fieldName] = null;
                    else
                        defaultValues.Add(port.fieldName, null);
                }
                else
                {
                    if (defaultValues.ContainsKey(port.fieldName))
                        defaultValues[port.fieldName] = System.Activator.CreateInstance(port.ValueType);
                    else
                        defaultValues.Add(port.fieldName, System.Activator.CreateInstance(port.ValueType));
                }
            }
        }

        protected override void OnBeforeSerializeOverride()
        {
            defaultValuesKeys.Clear();
            defaultValuesValues.Clear();
            defaultValuestypes.Clear();
            foreach (KeyValuePair<string, object> objects in defaultValues)
            {
                if (objects.Value != null)
                {
                    defaultValuesKeys.Add(objects.Key);
                    defaultValuestypes.Add(objects.Value.GetType().FullName);

                    string serializedValue = "";
                    GraphVariableValue valueConverter = null;

                    if (typeName2Value.TryGetValue(objects.Value.GetType().FullName, out valueConverter))
                    {
                        serializedValue = valueConverter.Serialize(objects.Value);
                    }
                    else
                    {
                        serializedValue = JsonUtility.ToJson(objects.Value);
                    }

                    defaultValuesValues.Add(serializedValue);
                }
            }
        }

        protected override void OnAfterDeserializeOverride()
        {
            defaultValues.Clear();
            for (int index = 0; index < defaultValuesKeys.Count; index++)
            {
                string typeName = defaultValuestypes[index];

                object deserializedObject = null;
                GraphVariableValue valueConverter = null;
                if (typeName2Value.TryGetValue(typeName, out valueConverter))
                    deserializedObject = valueConverter.Deserialize(defaultValuesValues[index]);
                else
                    deserializedObject = JsonUtility.FromJson(defaultValuesValues[index], ReflectionUtils.FindType(defaultValuestypes[index]));

                defaultValues.Add(defaultValuesKeys[index], deserializedObject);
            }
            defaultValuesKeys.Clear();
            defaultValuesValues.Clear();
            defaultValuestypes.Clear();
        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Variables/Combine";
        }

    }
}