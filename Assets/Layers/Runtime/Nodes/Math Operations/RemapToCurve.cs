using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ABXY.Layers.Runtime.Nodes.Math_Operations
{
	[CreateNodeMenu("Math operations/Remap to Curve")]
	public class RemapToCurve : FlowNode
	{
		[SerializeField, Input (ShowBackingValue.Unconnected, ConnectionType.Override, TypeConstraint.Inherited)]
		private float input = 0f;

#pragma warning disable CS0414
		[SerializeField, Output(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.Inherited)]
		private float output = 0f;
#pragma warning restore CS0414

		[SerializeField]
		AnimationCurve curve = new AnimationCurve(new Keyframe(0f,0f), new Keyframe(1f,1f));

		// Use this for initialization
		protected override void Init()
		{
			base.Init();

		}

		// Return the correct value of an output port when requested
		public override object GetValue(NodePort port)
		{
			return curve.Evaluate(GetInputValue<float>("input", input));
		}

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
			return new List<GraphEvent.EventParameterDef>();
        }

		protected override string GetHelpFileResourcePath()
		{
			return "Nodes/Math-Operations/Remap-to-Curve";
		}
	}
}