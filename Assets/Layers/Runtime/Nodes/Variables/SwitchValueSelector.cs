using ABXY.Layers.Runtime.Graph_Variable_Values;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ABXY.Layers.Runtime.Nodes
{
	[CreateNodeMenu("Variables/Switch Value Selector")]
	public class SwitchValueSelector : FlowNode
	{

		[SerializeField]
		private string comparisonType = typeof(float).FullName;

		[SerializeField]
		private string comparisonArrayType = typeof(float).FullName;

		[SerializeField]
		private string elementType = typeof(AudioClip).FullName;

		[SerializeField]
		private string elementArrayType = typeof(AudioClip).FullName;

		[SerializeField]
		List<SwitchElement> switchElements = new List<SwitchElement>();


        // Use this for initialization
        protected override void Init()
		{
			base.Init();

		}

		// Return the correct value of an output port when requested
		public override object GetValue(NodePort port)
		{
            object selector = GetInputValue("Input");
            foreach(SwitchElement switchElement in switchElements)
            {
                object comparisonValue = ValueUtility.GetVariableValue(switchElement.comparisonValue.typeName, ValueUtility.ValueFilter.All).GetDefaultValue(switchElement.comparisonValue);
                comparisonValue = GetInputValue(switchElement.comparisonValue.variableID, comparisonValue);

                if (Compare(selector, comparisonValue))
                {
                    object value = ValueUtility.GetVariableValue(switchElement.value.typeName, ValueUtility.ValueFilter.All).GetDefaultValue(switchElement.value);
                    return GetInputValue(switchElement.value.variableID, value);
                }
            }
            return null;
		}

        private bool Compare(object value1, object value2)
        {
            if (value1 == null || value2 == null)
                return false;
            GraphVariableValue valueComparer = ValueUtility.GetVariableValue(value1.GetType().FullName, ValueUtility.ValueFilter.All);
            
            return valueComparer.CompareValues(Logic.Comparison.comparisonOperators.Equal, value1, value2);
            
        }



        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
			return new List<GraphEvent.EventParameterDef>();
        }

		[System.Serializable]
		public class SwitchElement
        {

			[SerializeField]
			public GraphVariable comparisonValue = new GraphVariable();

			[SerializeField]
			public GraphVariable value = new GraphVariable();

        }

        protected override string GetHelpFileResourcePath()
        {
            return "Nodes/Variables/Switch-Value-Selector";
        }
    }
}