using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ABXY.Layers.Runtime.Nodes.Math_Operations
{
	[CreateNodeMenu("Math operations/Remap")]
	public class RemapNode : FlowNode
	{
		[SerializeField, Input(ShowBackingValue.Never, ConnectionType.Override, TypeConstraint.Inherited)]
		private float inputValue;

		[SerializeField, Output(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.Inherited)]
		private float outputValue;

		[SerializeField, Input(ShowBackingValue.Unconnected, ConnectionType.Override, TypeConstraint.Inherited)]
		private float inputMin = 0;

		[SerializeField, Input(ShowBackingValue.Unconnected, ConnectionType.Override, TypeConstraint.Inherited)]
		private float inputMax = 1;

		[SerializeField, Input(ShowBackingValue.Unconnected, ConnectionType.Override, TypeConstraint.Inherited)]
		private float outputMin = 0;

		[SerializeField, Input(ShowBackingValue.Unconnected, ConnectionType.Override, TypeConstraint.Inherited)]
		private float outputMax = 1;

		// Use this for initialization
		protected override void Init()
		{
			base.Init();

		}

		// Return the correct value of an output port when requested
		public override object GetValue(NodePort port)
		{
			float iValue = GetInputValue<float>("inputValue", 0f);
			float iMin = GetInputValue<float>("inputMin", inputMin);
			float iMax = GetInputValue<float>("inputMax", inputMax);
			float oMin = GetInputValue<float>("outputMin", outputMin);
			float oMax = GetInputValue<float>("outputMax", outputMax);
			float t = Mathf.InverseLerp(iMin, iMax, iValue);
			return Mathf.Lerp( oMin, oMax, t);
		}

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            throw new System.NotImplementedException();
        }

		protected override string GetHelpFileResourcePath()
		{
			return "Nodes/Math-Operations/Remap";
		}
	}
}