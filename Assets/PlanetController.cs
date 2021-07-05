using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetController : MonoBehaviour
{
    [SerializeField] Transform anchor;
    [SerializeField] GameObject body;
    [SerializeField] float restLength;
    [SerializeField] float k;
    [SerializeField] Vector3 gravity;

    Rigidbody2D bodyRigidBody;

    // Start is called before the first frame update
    void Start()
    {
      bodyRigidBody = body.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
      Vector3 forceDirection = (body.transform.position - anchor.position).normalized;
      float distance = Vector3.Distance(body.transform.position, anchor.position);
      float distanceFromRestLength = distance - restLength;

      Vector3 force = forceDirection * -1 * k * distanceFromRestLength;

      bodyRigidBody.AddForce(new Vector2(force.x, force.y));
    }
}
