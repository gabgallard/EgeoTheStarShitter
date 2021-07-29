using System.Collections.Generic;
using UnityEngine;

namespace ABXY.Layers.Runtime
{
    [System.Serializable]
    public class GraphVariable :GraphVariableBase
    {
        public enum RetrievalTypes { DefaultValue, ActualValue}

        [SerializeField]
        public string arrayType;

        [SerializeField ]
        public List<GraphVariableBase> arrayElements = new List<GraphVariableBase>();


        [SerializeField]
        public List<GraphVariableBase> defaultArrayElements = new List<GraphVariableBase>();

        public GraphVariable()
        {
        }

        public GraphVariable(string name, string typeName, object value)
        {
            this.name = name;
            this.typeName = typeName;
            SetValue(value);
        }

        public GraphVariable(string name, string typeName)
        {
            this.name = name;
            this.typeName = typeName;
        }

        public GraphVariable(System.Guid variableID)
        {
            this.variableID = variableID.ToString();
        }

        
        //Copy all elements of the variable
        public GraphVariable FullCopy()
        {
            GraphVariable var = new GraphVariable();
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
            var.arrayType = arrayType;
            foreach (var element in arrayElements)
                var.arrayElements.Add(element.Copy());


            foreach (var element in defaultArrayElements)
                var.defaultArrayElements.Add(element.Copy());

            return var;
        }

        public override void ResetToDefaultValue()
        {
            base.ResetToDefaultValue();
            arrayElements.Clear();
            foreach (var defaultElement in defaultArrayElements)
                arrayElements.Add(defaultElement.Copy());
        }


        public static void CopyValue(GraphVariable from, GraphVariable to)
        {
            to.typeName = from.typeName;
            to.objectValue = from.objectValue;
            to.unityObjectValue = from.unityObjectValue;
            to.typeName = from.typeName;
            to.serializedObjectValue = from.serializedObjectValue;

            to.defaultObjectValue = from.defaultObjectValue;
            to.defaultUnityObjectValue = from.defaultUnityObjectValue;
            to.defaultSerializedObjectValue = from.defaultSerializedObjectValue;

            to.arrayElements.Clear();
            foreach (var arrayElement in from.arrayElements)
                to.arrayElements.Add(arrayElement.Copy());

            to.defaultArrayElements.Clear();
            foreach (var arrayElement in from.defaultArrayElements)
                to.defaultArrayElements.Add(arrayElement.Copy());

            //to.arrayType = (from as GraphVariable).arrayType;
            //to.arrayElements = (from as GraphVariable).arrayElements;
        }
        public GraphArrayBase ToGraphArray(RetrievalTypes retrievalType)
        {
            System.Type genericType = typeof(GraphArray<>).MakeGenericType(ReflectionUtils.FindType(arrayType));
            GraphArrayBase array = (GraphArrayBase)System.Activator.CreateInstance(genericType, this, retrievalType);
            return array;
        }
    }
}
