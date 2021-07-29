using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ABXY.Layers.Runtime
{
    //[AddComponentMenu("Layers/Audio Controllers/Collision")]
    /*
    public class CollisionAudioController : AudioController
    {
        List<Sessions> sessions = new List<Sessions>();

        [SerializeField]
        private string onCollisionStartEventID = "";

        [SerializeField]
        private string onCollisionStartParameterID = "";

        [SerializeField]
        private string onCollisionEndEventID = "";
        [SerializeField]
        private string onCollisionEndParameterID = "";

        [SerializeField]
        private string whileCollidingEventID = "";
        [SerializeField]
        private string whileCollidingParameterID = "";


        private void OnCollisionEnter(Collision collision)
        {
            if (sessions.Find(x=>x.otherCollider == collision.collider) == null)
            {
                SoundGraphPlayer player = GetPlayer();
                if (player != null)
                    sessions.Add(new Sessions(collision.collider, player));

                Dictionary<string, object> parameters = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(onCollisionStartParameterID))
                    parameters.Add(onCollisionStartParameterID, collision);

                if (!string.IsNullOrEmpty(onCollisionStartEventID))
                    player.runtimeGraphCopy?.CallEventByID(onCollisionStartEventID, AudioSettings.dspTime, parameters,0);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            

            Sessions session = sessions.Find(x => x.otherCollider == collision.collider);
            if (session == null)
                return;


            sessions.Remove(session);

            Dictionary<string, object> parameters = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(onCollisionEndParameterID))
                parameters.Add(onCollisionEndParameterID, collision);

            if (!string.IsNullOrEmpty(onCollisionEndEventID))
                session.player.runtimeGraphCopy?.CallEventByID(onCollisionEndEventID, AudioSettings.dspTime, parameters,0);

            FinishWithPlayer(session.player);
            
        }

        private void OnCollisionStay(Collision collision)
        {
            Sessions session = sessions.Find(x => x.otherCollider == collision.collider);
            if (session == null)
                return;


            Dictionary<string, object> parameters = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(whileCollidingParameterID))
                parameters.Add(whileCollidingParameterID, collision);

            if (!string.IsNullOrEmpty(whileCollidingEventID))
                session.player.runtimeGraphCopy?.CallEventByID(whileCollidingEventID, AudioSettings.dspTime, parameters,0);
        }

        private class Sessions
        {
            public Collider otherCollider = null;
            public SoundGraphPlayer player;

            public Sessions(Collider otherCollider, SoundGraphPlayer player)
            {
                this.otherCollider = otherCollider;
                this.player = player;
            }
        }
    }*/
}