using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ABXY.Layers.Runtime.Nodes.Playback
{
    [System.Serializable]
    public class State
    {
        public string stateName;
        public SoundGraph subGraph;

        private SoundGraph _subGraphRuntime;
        public SoundGraph subGraphRuntime
        {
            get
            {
                if (_subGraphRuntime == null || subGraph == null || subGraph.graphID != _subGraphRuntime.graphID)
                    MakeRuntimeCopy();
                return _subGraphRuntime;
            }
            private set
            {
                _subGraphRuntime = value;
            }
        }

        public string guid = System.Guid.NewGuid().ToString();
        public string startGraphEventID;
        public string volumeVariableID;
        public List<Transition> transitions = new List<Transition>();
        public bool expanded = false;
        public float volume = 0f;

        public void MakeRuntimeCopy()
        {
            if (subGraph != null)
                _subGraphRuntime = (SoundGraph)(Application.isPlaying ? subGraph.RuntimeCopy() : subGraph.Copy());
            else
                _subGraphRuntime = null;
        }
    }
}