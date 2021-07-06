using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetBodyController : MonoBehaviour
{
  [SerializeField] float shakingSeconds;
  [SerializeField] float shakingSpeed;
  [SerializeField] float shakingAmplitude;

  Material material;
  Joint2D springJoint;

  void Awake()
  {
    springJoint = GetComponent<SpringJoint2D>();
  }

  void Start()
  {
    material = GetComponent<Renderer>().material;
    // StartShaking();
  }

  void StartShaking()
  {
    material.SetFloat("ShakingSpeed", shakingSpeed);
    material.SetFloat("ShakingAmplitude", shakingAmplitude);
  }

  void StopShaking()
  {
    material.SetFloat("ShakingSpeed", 0);
    material.SetFloat("ShakingAmplitude", 0);
  }

  void Update()
  {
  }

  void OnTriggerEnter2D(Collider2D other)
  {
    if(other.CompareTag("Mouth"))
    {
      Debug.Log("Collision with Mouth :: Start");
      StartShaking();
    }
  }

  void OnTriggerExit2D(Collider2D other)
  {
    if(other.CompareTag("Mouth"))
    {
      Debug.Log("Collision with Mouth :: Stop");
      StopShaking();
    }
  }

  void OnMouseDown()
  {
    StopSpringJoint();
  }

  void OnMouseUp()
  {
    StartSpringJoint();
  }

  void StartSpringJoint()
  {

    springJoint.enabled = true;
  }

  void StopSpringJoint()
  {
    springJoint.enabled = false;
  }
}
