using ABXY.Layers.Runtime.Sound_graph_players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ABXY.Layers.Runtime {
    public abstract class AudioController : MonoBehaviour
    {
        private SoundGraphPlayer _player;
        public SoundGraphPlayer player
        {
            get
            {
                if (_player == null)
                    _player = GetComponent<SoundGraphPlayer>();
                return _player;
            }
        }

    }
}