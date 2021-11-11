using System.Collections.Generic;
using ABXY.Layers.Runtime.Graph_Variable_Values;
using UnityEngine;

namespace ABXY.Layers.Runtime
{
    [System.Serializable]
    public class GraphVariableBase: ISerializationCallbackReceiver
    {
        [SerializeField]
        public string name;




        [SerializeField]
        public string variableID = System.Guid.Empty.ToString();

        public enum ExposureTypes { AsInput = 1 << 0, AsOutput = 1 << 1, DoNotExpose = 1 << 2 }

        [SerializeField]
        public ExposureTypes expose = ExposureTypes.DoNotExpose;

        [SerializeField]
        public string typeName;

        [SerializeField]
        public bool expanded = false;


        /// <summary>
        /// Used to control synchronization between input component variables and their original values in the graph
        /// </summary>
        [SerializeField]
        public bool synchronizeWithGraphVariable = true;


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

        public object objectValue;
        [SerializeField]
        protected string serializedObjectValue;

        public UnityEngine.Object unityObjectValue;

        public object defaultObjectValue;
        [SerializeField]
        protected string defaultSerializedObjectValue;

        public UnityEngine.Object defaultUnityObjectValue;

        public GraphVariableBase()
        {
        }

        public GraphVariableBase(string name, string typeName, object value)
        {
            this.name = name;
            this.typeName = typeName;
            SetValue(value);
        }

        public GraphVariableBase(string name, string typeName)
        {
            this.name = name;
            this.typeName = typeName;
        }

        public GraphVariableBase(System.Guid variableID)
        {
            this.variableID = variableID.ToString();
        }

        private static void LoadGraphVarValues()
        {
            if (_typeName2Value.Count != 0) // Then already loaded
                return;

            foreach (System.Reflection.Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (System.Type type in assembly.GetTypes())
                {
                    if (type.BaseType == typeof(GraphVariableValue))
                    {
                        GraphVariableValue instance = (GraphVariableValue)System.Activator.CreateInstance(type);
                        _typeName2Value.Add(instance.handlesType.FullName, instance);
                    }
                }
            }
        }


        public object Value()
        {
            GraphVariableValue valueGetter = null;
            if (typeName2Value.TryGetValue(typeName, out valueGetter))
                return valueGetter.GetValue((GraphVariableBase)this);
            return null;
        }

        public void SetValue(object obj)
        {
            GraphVariableValue valueSetter = null;
            if (typeName2Value.TryGetValue(typeName, out valueSetter))
                valueSetter.SetValue(this, obj);
            synchronizeWithGraphVariable = false;
        }

        public object DefaultValue()
        {
            GraphVariableValue valueGetter = null;
            if (typeName2Value.TryGetValue(typeName, out valueGetter))
                return valueGetter.GetDefaultValue((GraphVariableBase)this);
            return null;
        }

        public void SetDefaultValue(object obj)
        {
            GraphVariableValue valueSetter = null;
            if (typeName2Value.TryGetValue(typeName, out valueSetter))
                valueSetter.SetDefaultValue((GraphVariable)this, obj);
            synchronizeWithGraphVariable = true;
        }

        public System.Type GetVariableType()
        {
            return ReflectionUtils.FindType(typeName);
        }


        public virtual void ResetToDefaultValue()
        {
            objectValue = defaultObjectValue;
            unityObjectValue = defaultUnityObjectValue;
            serializedObjectValue = defaultSerializedObjectValue;
            synchronizeWithGraphVariable = true;
        }

        public GraphVariableBase Copy()
        {
            GraphVariableBase var = new GraphVariableBase();
            var.typeName = typeName;

            var.serializedObjectValue = serializedObjectValue;
            var.objectValue = objectValue;
            var.unityObjectValue = unityObjectValue;

            var.defaultSerializedObjectValue = defaultSerializedObjectValue;
            var.defaultObjectValue = defaultObjectValue;
            var.defaultUnityObjectValue = defaultUnityObjectValue;

            var.variableID = variableID;
            var.name = name;
            var.expose = expose;
            //var.arrayType = arrayType;
            //var.arrayElements = arrayElements;
            return var;
        }

        public void OnBeforeSerialize()
        {
            if (typeName == null)
                return;

            GraphVariableValue valueSerializer = null;
           
            typeName2Value.TryGetValue(typeName, out valueSerializer);


            if (!string.IsNullOrEmpty(typeName))
            {
                if (valueSerializer != null)
                {
                    serializedObjectValue = valueSerializer.Serialize(objectValue);
                    defaultSerializedObjectValue = valueSerializer.Serialize(defaultObjectValue);
                }
                else
                {
                    serializedObjectValue = "";
                    defaultSerializedObjectValue = "";
                }
            }
            
            
        }

        public void OnAfterDeserialize()
        {
            GraphVariableValue valueDeSerializer = null;
            typeName2Value.TryGetValue(typeName, out valueDeSerializer);

            if (!string.IsNullOrEmpty(serializedObjectValue))
            {
                if (valueDeSerializer != null)
                    objectValue = valueDeSerializer.Deserialize(serializedObjectValue);
                else
                    objectValue = null;
            }

            if (!string.IsNullOrEmpty(defaultSerializedObjectValue))
            {
                if (valueDeSerializer != null)
                    defaultObjectValue = valueDeSerializer.Deserialize(defaultSerializedObjectValue);
                else
                    defaultObjectValue = null;
            }


        }
    }
}
