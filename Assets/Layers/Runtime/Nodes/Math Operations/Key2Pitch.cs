using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ABXY.Layers.Runtime.Nodes.Math_Operations
{
    [Node.CreateNodeMenu(null), DeprecatedNode("This node has been updated. Create a new Key2Pitch node to get the new version.")]
    public class Key2Pitch : FlowNode
    {
#pragma warning disable CS0414
        [SerializeField, Input(ShowBackingValue.Unconnected, ConnectionType.Override, TypeConstraint.Inherited)]
        private int keyNumber = 0;

        [SerializeField, Output(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.Inherited)]
        private float pitch = 0f;


        [SerializeField, Input(ShowBackingValue.Unconnected, ConnectionType.Override, TypeConstraint.Inherited)]
        private float referencePitch = 440f;
#pragma warning restore CS0414
        public enum ReferenceNotes { C4,C5,A4}

        [SerializeField]
        public ReferenceNotes referenceNote = ReferenceNotes.C4;

        
        public enum PitchTypes { HZ, RelativeMultiplier }

        [SerializeField]
        private PitchTypes outputPitchType = PitchTypes.HZ;

        public override object GetValue(NodePort port)
        {
            int referenceNoteNumber = 0;
            if (referenceNote == ReferenceNotes.C4)
                referenceNoteNumber = 60;
            else if (referenceNote == ReferenceNotes.C5)
                referenceNoteNumber = 72;
            else
                referenceNoteNumber = 69;


            int keyNumberValue = GetInputValue<int>("keyNumber", keyNumber);

            float referencePitchValue = GetInputValue<float>("referencePitch", referencePitch);

            if (outputPitchType == PitchTypes.RelativeMultiplier)
                referencePitchValue = 1f;

            int c4Key = keyNumberValue - referenceNoteNumber;
            float pitch = referencePitchValue * Mathf.Pow(2, c4Key / 12f);

            return pitch;
        }

        protected override List<GraphEvent.EventParameterDef> GetOutGoingEventParametersOnPortInternal(NodePort port, List<Node> visitedNodes)
        {
            return new List<GraphEvent.EventParameterDef>();
        }
    }
}