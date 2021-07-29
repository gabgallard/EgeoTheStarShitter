using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.FlowTypes;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ABXY.Layers.Runtime.Nodes.Signal_Sources
{
	[CreateNodeMenu("Signal sources/Raise Event On Condition")]
	public class RaiseEventOnConditionNode : FlowNode
	{

		[SerializeField, Input(ShowBackingValue.Never, ConnectionType.Override, TypeConstraint.Inherited)]
		private bool condition;

		[SerializeField, Output(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.Strict)]
		private LayersEvent OnTrue;

		private bool lastValue = false;


		// Use this for initialization
		protected override void Init()
		{
			base.Init();
		}

		public override void NodeUpdate()
		{
			bool newValue = GetInputValue<bool>("condition", false);
			if (newValue && newValue != lastValue)
			{
				CallFunctionOnOutputNodes("OnTrue", AudioSettings.dspTime,0);
			}
			lastValue = newValue;
		}

		// Return the correct value of an output port when requested
		public override object GetValue(NodePort port)
		{

			return null; // Replace this
		}

		protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
		{
			return new List<GraphEvent.EventParameterDef>();
		}

		protected override string GetHelpFileResourcePath()
		{
			return "Nodes/Signal-Sources/Raise-Event-On-Condition";
		}
	}
}