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
  float shakingStartedAt;
  bool shaking = false;

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
    shakingStartedAt = Time.time;
    shaking = true;

    UpdateShakingValues();
  }

  void StopShaking()
  {
    material.SetFloat("ShakingSpeed", 0);
    material.SetFloat("ShakingAmplitude", 0);

    shaking = false;
  }

  void UpdateShakingValues()
  {
    float secondsShaking = Time.time - shakingStartedAt;

    float lerpInterpolationValue = secondsShaking / shakingSeconds;
    float shakingSpeedLerp = Mathf.Lerp(0, shakingSpeed, lerpInterpolationValue);
    float shakingAmplitudeLerp = Mathf.Lerp(0, shakingAmplitude, lerpInterpolationValue);

    // Debug.Log("XXX: shakingSpeedLerp: " + shakingSpeedLerp);
    // Debug.Log("XXX: shakingAmplitudeLerp: " + shakingAmplitudeLerp);
    // Debug.Log("XXX: lerpInterpolationValue: " + lerpInterpolationValue);

    material.SetFloat("ShakingSpeed", shakingSpeedLerp);
    material.SetFloat("ShakingAmplitude", shakingAmplitudeLerp);
  }

  void Update()
  {
    if(shaking)
      UpdateShakingValues();
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
