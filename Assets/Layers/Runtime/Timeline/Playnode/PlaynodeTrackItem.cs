using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.FlowTypes;
using ABXY.Layers.Runtime.Nodes.Playback;
using UnityEngine;

namespace ABXY.Layers.Runtime.Timeline.Playnode
{
    [System.Serializable]
    public class PlaynodeTrackItem : TimeLineRowDataItem
    {
        [SerializeField]
        public string trackLabel = "Track";

        [SerializeField]
        public float volume = 1f;

        [SerializeField]
        public float stereoPan = 0f;


        [SerializeField]
        public string audioOutNodeName = "";

        [SerializeField]
        public string audioOutSendName = "";

        [SerializeField]
        public string midiOutNodeName = "";

        [SerializeField]
        public string volumeInNodeName = "";

        [SerializeField]
        public string panInNodeName = "";

        [SerializeField]
        public bool exposed = false;

        public PlaynodeTrackItem(PlayNode node)
        {
            NodePort audioOutNode = node.AddDynamicOutput(typeof(AudioFlow), Node.ConnectionType.Multiple, Node.TypeConstraint.Strict);
            this.audioOutNodeName = audioOutNode.fieldName;

            NodePort midiOut = node.AddDynamicOutput(typeof(LayersEvent), Node.ConnectionType.Multiple, Node.TypeConstraint.Strict);
            this.midiOutNodeName = midiOut.fieldName;

            NodePort volumeIn = node.AddDynamicInput(typeof(float), Node.ConnectionType.Override, Node.TypeConstraint.Inherited);
            this.volumeInNodeName = volumeIn.fieldName;

            NodePort panIn = node.AddDynamicInput(typeof(float), Node.ConnectionType.Override, Node.TypeConstraint.Inherited);
            this.panInNodeName = panIn.fieldName;
        }

        public void ClearConnections(PlayNode node)
        {
            node.GetOutputPort(audioOutNodeName)?.ClearConnections();
            node.GetOutputPort(midiOutNodeName)?.ClearConnections();
            node.GetOutputPort(volumeInNodeName)?.ClearConnections();
            node.GetOutputPort(panInNodeName)?.ClearConnections();
        }
    }
}
