using System.Collections.Generic;
using ABXY.Layers.Runtime.Sound_graph_players;
using UnityEngine;

namespace ABXY.Layers.Runtime
{
    [ExecuteInEditMode, System.Obsolete]
    public class SymphonyPlayerPool<T> : PlayerBase where T:SoundGraphPlayer
    {
    

        private static List<T> checkedInPlayers = new List<T>();

        protected override void Awake()
        {
            if (soundGraph == null)
                LoadGraph();

            playOnAwake = false;

            SyncVariables();
            SyncEvents();
            SyncStartingEventParameters();



            if (Application.isPlaying)
            {
                for (int index = 0; index < soundGraph.graphInput.poolSize; index++)
                {
                    checkedInPlayers.Add(NewPlayer());
                }
                if (dontDestroyOnLoad)
                {
                    this.transform.parent = null;
                    DontDestroyOnLoad(this.gameObject);
                }
            }
        }

        private T NewPlayer()
        {
            if (soundGraph == null)
                return null;
            GameObject newGo = new GameObject();
            newGo.transform.parent = this.transform;
            newGo.transform.localPosition = Vector3.zero;
            newGo.name = "[Inactive]" + soundGraph.name;
            T player = newGo.AddComponent<T>();
            player.soundGraph = soundGraph;
            return player;
        }

        public T Checkout()
        {
            T player = null;
            if (soundGraph == null)
            {
                Debug.LogError("No soundgraph loaded on " + this.name);
                return null;
            }

            if (checkedInPlayers.Count == 0)
                player = NewPlayer();
            else
            {
                player = checkedInPlayers[0];
                checkedInPlayers.RemoveAt(0);
            }
            player.name = "[Active]" + soundGraph.name;
            return player;
        }

        public void Checkin(T player)
        {
            if (player == null)
                return;

            if (checkedInPlayers.Contains(player))
            {
                Debug.LogWarning("Player was returned twice to " + this.name);
            }
            else
            {
                player.ClearEventListeners();
                player.ResetVariables();
                player.name = "[Inactive]" + soundGraph.name;
                checkedInPlayers.Add(player);
            }
        }

        internal override SoundGraphPlayer GetSoundGraphPlayer()
        {
            return Checkout();
        }

        internal override void FinishedWithSoundGraphPlayer(SoundGraphPlayer player)
        {
            Checkin(player as T);
        }
    }
}
