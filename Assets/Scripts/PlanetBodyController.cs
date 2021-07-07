using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlanetBodyController : MonoBehaviour
{
  [SerializeField] float shakingDuration;
  [SerializeField] float shakingSpeed;
  [SerializeField] float shakingAmplitude;
  [SerializeField] float slurpDuration;
  [SerializeField] float bornDuration;
  [SerializeField] GameObject planet;

  Vector3 cursorOffset;
  Material material;
  Joint2D springJoint;
  float shakingStartedAt;
  bool shaking = false;
  bool slurping = false;
  bool borning = false;

  Transform egeoMouthInside;

  void Awake()
  {
    springJoint = GetComponent<SpringJoint2D>();

    shakingDuration = RandomDeviation(shakingDuration);
    shakingSpeed = RandomDeviation(shakingSpeed);
    shakingAmplitude = RandomDeviation(shakingAmplitude);
    slurpDuration = RandomDeviation(slurpDuration);
    bornDuration = RandomDeviation(bornDuration);
  }

  void Start()
  {
    egeoMouthInside = EgeoController.Instance.MouthInside;
    material = GetComponent<Renderer>().material;
    StartBorning();
  }

  void Update()
  {
    if(shaking)
    {
      UpdateShakingValues();
      if(Time.time - shakingStartedAt >= shakingDuration)
      {
        StopShaking();
        StartSlurping();
      }
    }
  }

  void StartShaking()
  {
    shakingStartedAt = Time.time;
    shaking = true;

    UpdateShakingValues();
    EgeoController.Instance.StartSmeling();
  }

  void StopShaking()
  {
    material.SetFloat("ShakingSpeed", 0);
    material.SetFloat("ShakingAmplitude", 0);

    shaking = false;
    EgeoController.Instance.StopSmeling();
  }

  void UpdateShakingValues()
  {
    float secondsShaking = Time.time - shakingStartedAt;

    float lerpInterpolationValue = secondsShaking / shakingDuration;
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

    if(other.CompareTag("MouthInside") && slurping)
    {
      StopSlurping();
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
    if(!slurping)
    {
      cursorOffset = transform.position - MouseCursor2D();
      StopSpringJoint();
    }
  }

  void OnMouseDrag()
  {
    // Debug.Log("Draggable.OnMouseDrag()");
    if(!slurping)
      transform.position = MouseCursor2D() + cursorOffset;
  }

  Vector3 MouseCursor2D()
  {
    Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    cursorPosition.z = 0;
    return cursorPosition;
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

    transform.DOScale(0.4f, slurpDuration / 10);
    transform.DOMove(egeoMouthInside.position, slurpDuration);

    EgeoController.Instance.StartEating();
  }

  void StopSlurping()
  {
    Debug.Log("StopSlurping");
    EgeoController.Instance.StopEating();
    EgeoController.Instance.AddPlanetToStomach();
    PlanetSpawnerController.Instance.RemovePlanet(planet);
    Destroy(planet, 1.0f);
  }

  void StartBorning()
  {
    borning = true;
    transform.localScale = new Vector3(0f, 0f, transform.localScale.z);
    transform.DOScale(1.0f, bornDuration).OnComplete(StopBorning);
  }

  void StopBorning()
  {
    borning = false;
  }

  float RandomDeviation(float number) {
    return Random.Range(number - (number / 2), number + (number / 2));
  }
}
