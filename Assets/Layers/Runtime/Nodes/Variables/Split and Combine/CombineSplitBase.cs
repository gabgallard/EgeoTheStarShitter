using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine
{
    public abstract class CombineSplitBase : FlowNode, ISerializationCallbackReceiver
    {
        public Dictionary<string, string> arbitraryStrings = new Dictionary<string, string>();
        [SerializeField]
        private List<string> arbitraryStringsKeys = new List<string>();
        [SerializeField]
        private List<string> arbitraryStringsValues = new List<string>();

        public Dictionary<string, object> arbitraryData = new Dictionary<string, object>();

        public void OnBeforeSerialize()
        {
            arbitraryStringsKeys.Clear();
            arbitraryStringsValues.Clear();
            foreach (KeyValuePair<string, string> kvs in arbitraryStrings)
            {
                arbitraryStringsKeys.Add(kvs.Key);
                arbitraryStringsValues.Add(kvs.Value);
            }
            OnBeforeSerializeOverride();
        }

        protected virtual void OnBeforeSerializeOverride()
        {

        }

        public void OnAfterDeserialize()
        {
            arbitraryStrings.Clear();
            for (int index = 0; index < arbitraryStringsKeys.Count; index++)
                arbitraryStrings.Add(arbitraryStringsKeys[index], arbitraryStringsValues[index]);
            OnAfterDeserializeOverride();
        }

        protected virtual void OnAfterDeserializeOverride()
        {

        }

        public override void PlayAtDSPTime(NodePort calledBy, double time, Dictionary<string, object> data, int nodesCalledThisFrame)
        {
            base.PlayAtDSPTime(calledBy, time, data, nodesCalledThisFrame);
        }
    }
}