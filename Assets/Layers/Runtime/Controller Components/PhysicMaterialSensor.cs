using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ABXY.Layers.Runtime
{
    [AddComponentMenu("Layers/Audio Controllers/Physic Material Sensor")]
    public class PhysicMaterialSensor : AudioController
    {


        [SerializeField]
        private Vector3 raycastStartPosition = Vector3.zero;

        [SerializeField]
        private Vector3 raycastDirection = Vector3.down;

        [SerializeField]
        private float raycastDistance = 1f;

        [SerializeField]
        private Space raycastSpace = Space.Self;

        public enum TagStyles { Inclusive, Exclusive, AllTags}

        [SerializeField]
        private TagStyles tagStyle = TagStyles.AllTags;

        [SerializeField]
        private List<string> tagList = new List<string>();

        [SerializeField]
        private string physicMaterialPropertyID = "";

        public PhysicMaterial targetPhysicMaterial { get; private set; }

        private Vector3 calculatedRaycastDirection
        {
            get
            {
                if (raycastSpace == Space.Self)
                    return transform.TransformDirection(raycastDirection).normalized;
                else
                    return raycastDirection.normalized ;
            }
        }

        private void OnEnable()
        {

        }

        private void OnDisable()
        {

        }

        private void Update()
        {
            if (player != null)
            {
                RaycastHit[] hits = Physics.RaycastAll(transform.TransformPoint(raycastStartPosition)
                    , calculatedRaycastDirection, raycastDistance);

                float closestDistance = float.MaxValue;
                Collider selectedCollider = null;

                foreach(RaycastHit hit in hits)
                {
                    if (CheckTags(hit) && closestDistance > hit.distance)
                    {
                        closestDistance = hit.distance;
                        selectedCollider = hit.collider;
                    }
                }

                targetPhysicMaterial = selectedCollider != null ? selectedCollider.sharedMaterial : null;
                player?.runtimeGraphCopy?.SetVariableByID(physicMaterialPropertyID, targetPhysicMaterial);
            }
        }

        private bool CheckTags(RaycastHit hit)
        {
            switch (tagStyle)
            {
                case TagStyles.Inclusive:
                    return tagList.Contains(hit.collider.tag);
                case TagStyles.Exclusive:
                    return !tagList.Contains(hit.collider.tag);
            }
            return true;
        }

        private void OnDrawGizmos()
        {
            Color gizmoColor = Gizmos.color;
            Gizmos.color = Color.magenta;
            Vector3 startPosition = transform.TransformPoint(raycastStartPosition);
            Gizmos.DrawSphere(startPosition, 0.025f);
            Gizmos.DrawLine(startPosition, startPosition
                + (calculatedRaycastDirection * raycastDistance));
            Gizmos.color = gizmoColor;
        }
    }
}