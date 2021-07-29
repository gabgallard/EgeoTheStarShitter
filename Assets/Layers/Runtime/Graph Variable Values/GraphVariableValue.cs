using System;
using System.Collections.Generic;
using ABXY.Layers.Runtime.ThirdParty.XNode.Scripts;
using ABXY.Layers.Runtime.Nodes;
using ABXY.Layers.Runtime.Nodes.Logic;
using ABXY.Layers.Runtime.Nodes.Variables.Split_and_Combine;
using UnityEngine;

namespace ABXY.Layers.Runtime.Graph_Variable_Values
{
    public abstract class GraphVariableValue
    {
        public abstract System.Type handlesType { get; }

        public abstract object GetValue(GraphVariableBase graphVariable);

        public abstract void SetValue(GraphVariableBase graphVariable, object value);

        public abstract object GetDefaultValue(GraphVariableBase graphVariable);

        public abstract void SetDefaultValue(GraphVariableBase graphVariable, object value);

        public abstract object GetValueOnInitialization();

        public abstract string GetValueInitializationString();

        public static readonly List<string> bannedNames = new List<string>(new string[] {
            "Awake",
            "FixedUpdate",
            "LateUpdate",
            "OnAnimatorIK",
            "OnAnimatorMove",
            "OnApplicationFocus",
            "OnApplicationPause",
            "OnApplicationQuit",
            "OnAudioFilterRead",
            "OnBecameInvisible",
            "OnBecameVisible",
            "OnCollisionEnter",
            "OnCollisionEnter2D",
            "OnCollisionExit",
            "OnCollisionExit2D",
            "OnCollisionStay",
            "OnCollisionStay2D",
            "OnConnectedToServer",
            "OnControllerColliderHit",
            "OnDestroy",
            "OnDisable",
            "OnDisconnectedFromServer",
            "OnDrawGizmos",
            "OnDrawGizmosSelected",
            "OnEnable",
            "OnFailedToConnect",
            "OnFailedToConnectToMasterServer",
            "OnGUI",
            "OnJointBreak",
            "OnJointBreak2D",
            "OnMasterServerEvent",
            "OnMouseDown",
            "OnMouseDrag",
            "OnMouseEnter",
            "OnMouseExit",
            "OnMouseOver",
            "OnMouseUp",
            "OnMouseUpAsButton",
            "OnNetworkInstantiate",
            "OnParticleCollision",
            "OnParticleSystemStopped",
            "OnParticleTrigger",
            "OnParticleUpdateJobScheduled",
            "OnPlayerConnected",
            "OnPlayerDisconnected",
            "OnPostRender",
            "OnPreCull",
            "OnPreRender",
            "OnRenderImage",
            "OnRenderObject",
            "OnSerializeNetworkView",
            "OnServerInitialized",
            "OnTransformChildrenChanged",
            "OnTransformParentChanged",
            "OnTriggerEnter",
            "OnTriggerEnter2D",
            "OnTriggerExit",
            "OnTriggerExit2D",
            "OnTriggerStay",
            "OnTriggerStay2D",
            "OnValidate",
            "OnWillRenderObject",
            "Reset",
            "Start",
            "Update",
            "BroadcastMessage",
            "CompareTag",
            "GetComponent",
            "GetComponentInChildren",
            "GetComponentInParent",
            "GetComponents",
            "GetComponentsInChildren",
            "GetComponentsInParent",
            "SendMessage",
            "SendMessageUpwards",
            "TryGetComponent",
            "GetInstanceID",
            "ToString",
            "Destroy",
            "DestroyImmediate",
            "DontDestroyOnLoad",
            "FindObjectOfType",
            "FindObjectsOfType",
            "Instantiate"

        });

        public static bool IsValidVariableName(string variableName)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(variableName, "^[a-zA-z][a-zA-Z0-9_ ]+");
        }
    

        public virtual string Serialize(object objectValue)
        {
            return "";
        }

        public virtual object Deserialize(string serializedObjectValue)
        {
            return null;
        }

        public abstract bool CompareValues(Comparison.comparisonOperators comparator, object a, object b);


        protected bool IsNumericType(object o)
        {
            if (o == null)
                return false;
            return IsNumericType(o.GetType());
        }

        protected bool IsNumericType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsNumericType()
        {
            return IsNumericType(handlesType);
        }


        /// <summary>
        /// For example, Layers Events and Audio Outs
        /// </summary>
        /// <returns></returns>
        public virtual bool IsNonreferenceableType()
        {
            return false;
        }

        public virtual Color GetDefaultColor()
        {

            Color col = GetTypeColor(handlesType.FullName);



            return col;
        }

        public virtual Color GetDefaultColorPro()
        {

            return GetDefaultColor();
        }

        public static Color GetTypeColor(string typeName)
        {
        
#if UNITY_5_4_OR_NEWER
            UnityEngine.Random.State oldState = UnityEngine.Random.state;
            UnityEngine.Random.InitState(typeName.GetHashCode());
#else
        int oldSeed = UnityEngine.Random.seed;
        UnityEngine.Random.seed = typeName.GetHashCode();
#endif
            Color col = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
#if UNITY_5_4_OR_NEWER
            UnityEngine.Random.state = oldState;
#else
        UnityEngine.Random.seed = oldSeed;
#endif
            return col;
        }
    }
}
