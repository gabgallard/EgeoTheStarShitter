using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class PlanetBodyController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
  [SerializeField] float shakingDuration;
  [SerializeField] float shakingSpeed;
  [SerializeField] float shakingAmplitude;
  [SerializeField] float slurpDuration;
  [SerializeField] float bornDuration;
  [SerializeField] GameObject planet;
  [SerializeField] string bodyType;
  [SerializeField] float blowForce = 10;

  Vector3 cursorOffset;
  Material material;
  Joint2D springJoint;
  float shakingStartedAt;
  bool shaking = false;
  bool underForces = false;
  bool dragging = false;
  Rigidbody2D theRigidbody;

  PointerEventData pointerEventData;

  Transform egeoMouthInside;

  void Awake()
  {
    springJoint = GetComponent<SpringJoint2D>();
    theRigidbody = GetComponent<Rigidbody2D>();

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
        if(bodyType == "planet")
          StartSlurping();
        else
          BlowAway();
      }
    }

    if(!underForces && dragging)
      transform.position = MouseCursor2D() + cursorOffset;
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

    material.SetFloat("ShakingSpeed", shakingSpeedLerp);
    material.SetFloat("ShakingAmplitude", shakingAmplitudeLerp);
  }


  void OnTriggerEnter2D(Collider2D other)
  {
    if(other.CompareTag("Mouth"))
    {
      StartShaking();
    }

    if(other.CompareTag("MouthInside") && underForces)
    {
      StopSlurping();
    }
  }

  void OnTriggerExit2D(Collider2D other)
  {
    if(other.CompareTag("Mouth"))
    {
      StopShaking();
    }
  }

  // Drag and Drop :: INI
  public void OnPointerDown(PointerEventData eventData)
  {
    if(!underForces && !EgeoController.Instance.UniverseFinished)
    {
      cursorOffset = transform.position - MouseCursor2D();
      StopSpringJoint();
      dragging = true;
    }
  }

  public void OnPointerUp(PointerEventData eventData)
  {
    if(!underForces)
    {
      StartSpringJoint();
      dragging = false;
    }
  }

  Vector3 MouseCursor2D()
  {
    Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    cursorPosition.z = 0;
    return cursorPosition;
  }

  void StopDragging()
  {
    dragging = false;
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
    underForces = true;

    transform.DOScale(0.4f, slurpDuration / 10);
    transform.DOMove(egeoMouthInside.position, slurpDuration);

    EgeoController.Instance.StartEating();
  }

  void StopSlurping()
  {
    EgeoController.Instance.StopEating();
    EgeoController.Instance.AddPlanetToStomach();
    PlanetSpawnerController.Instance.RemovePlanet(planet);
    Destroy(planet, 1.0f);
  }

  void StartBorning()
  {
    transform.localScale = new Vector3(0f, 0f, transform.localScale.z);
    transform.DOScale(1.0f, bornDuration).OnComplete(StopBorning);
  }

  void StopBorning()
  {
  }

  float RandomDeviation(float number) {
    return Random.Range(number - (number / 2), number + (number / 2));
  }

  void BlowAway()
  {
    StartCoroutine("BlowAwayCoroutine");
    EgeoController.Instance.Blow(this);
  }

  IEnumerator BlowAwayCoroutine()
  {
    yield return new WaitForSeconds(0.5f);
    Vector3 direction = (egeoMouthInside.transform.position - transform.position).normalized;
    theRigidbody.AddForce(direction * -1 * blowForce, ForceMode2D.Impulse);
    float temporalMass = 10.0f;
    float previousMass = theRigidbody.mass;
    theRigidbody.drag = temporalMass;
    underForces = true;
    StopSpringJoint();
    StopDragging();

    yield return new WaitForSeconds(0.4f);
    StartSpringJoint();
    underForces = false;

    yield return new WaitForSeconds(0.5f);
    theRigidbody.drag = previousMass;
  }
}
