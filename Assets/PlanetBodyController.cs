using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlanetBodyController : MonoBehaviour
{
  [SerializeField] float shakingSeconds;
  [SerializeField] float shakingSpeed;
  [SerializeField] float shakingAmplitude;
  [SerializeField] float slurpSeconds;

  Vector3 cursorOffset;
  Material material;
  Joint2D springJoint;
  float shakingStartedAt;
  bool shaking = false;
  bool slurping = false;

  Transform egeoMouthInside;

  void Awake()
  {
    springJoint = GetComponent<SpringJoint2D>();
    egeoMouthInside = EgeoController.Instance.MouthInside;
  }

  void Start()
  {
    material = GetComponent<Renderer>().material;
    // StartShaking();
  }

  void Update()
  {
    if(shaking)
    {
      UpdateShakingValues();
      if(Time.time - shakingStartedAt >= shakingSeconds)
      {
        StopShaking();
        StartSlurping();
      }
    }

    if(slurping)
    {
      if(transform.position == egeoMouthInside.position) {
        StopSlurping();
      }
    }
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
    // Debug.Log("Draggable.OnMouseDown()");
    cursorOffset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    StopSpringJoint();
  }

  void OnMouseDrag()
  {
    // Debug.Log("Draggable.OnMouseDrag()");
    if(!slurping)
      transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + cursorOffset;
  }

  void OnMouseUp()
  {
    if(!slurping)
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

  void StartSlurping()
  {
    slurping = true;

    transform.DOScale(0.4f, slurpSeconds / 10).SetEase(Ease.InBounce);
    transform.DOMove(egeoMouthInside.position, slurpSeconds);
  }

  void StopSlurping()
  {
    Debug.Log("StopSlurping");
  }
}
