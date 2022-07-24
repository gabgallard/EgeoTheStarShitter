using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class MouthController : MonoBehaviour
{
  [SerializeField] Transform lipUpTarget;
  [SerializeField] Transform lipDownTarget;
  [SerializeField] Transform lipUpClosed;
  [SerializeField] Transform lipDownClosed;
  [SerializeField] Transform lipUpOpened;
  [SerializeField] Transform lipDownOpened;

  [SerializeField] float lipMovementAmplitude;
  [SerializeField] float lipMovementSpeed;
  [SerializeField] float lipMovementAmplitudeWhenSmeling;

  [SerializeField] float actualLipMovementAmplitude;
  bool eating = false;
  bool closing = false;
  bool blowing = false;
  bool smelling = false;
  string mouthPhase = null;

  float previousMovementPerlinValue;

  public UnityEvent mouthClosingEvent;
  public UnityEvent mouthOpeningEvent;


  void Start()
  {
    StopEating();
  }

  void Update()
  {
    if(!eating && !closing && !blowing)
    {
      MouthClosedAnimation();

    }

    if(smelling)
      CheckLipsDirectionChange();
  }

  void MouthClosedAnimation()
  {
    float perlinValue = Mathf.PerlinNoise(Time.time * lipMovementSpeed, 0.0f);
    float movement = actualLipMovementAmplitude * perlinValue;
    movement = movement - (actualLipMovementAmplitude / 2.0f);

    Vector3 position = lipUpClosed.position;
    position.y = position.y + movement;
    lipUpTarget.transform.position = position;

    position = lipDownClosed.position;
    position.y = position.y - movement;
    lipDownTarget.transform.position = position;
  }

  void CheckLipsDirectionChange()
  {
    float actualPerlinValue = Mathf.PerlinNoise(Time.time * lipMovementSpeed, 0.0f);
    float actualPerlinValue_minus_1 = Mathf.PerlinNoise((Time.time - Time.deltaTime) * lipMovementSpeed, 0.0f);
    float actualPerlinValue_minus_2 = Mathf.PerlinNoise((Time.time - (Time.deltaTime * 2))* lipMovementSpeed, 0.0f);

    if(
        mouthPhase != "closing" &&
        (actualPerlinValue < actualPerlinValue_minus_1) &&
        (actualPerlinValue_minus_1 > actualPerlinValue_minus_2)
    )
    {
        // Debug.Log($"{actualPerlinValue}, ${actualPerlinValue_minus_1}, ${actualPerlinValue_minus_2} => Mouth closing");
        mouthPhase = "closing";
        mouthClosingEvent.Invoke();
    } else if(
        mouthPhase != "opening" &&
        (actualPerlinValue > actualPerlinValue_minus_1) &&
        (actualPerlinValue_minus_1 < actualPerlinValue_minus_2)
    )
    {
        // Debug.Log($"{actualPerlinValue}, ${actualPerlinValue_minus_1}, ${actualPerlinValue_minus_2} => Mouth opening");
        mouthPhase = "opening";
        mouthOpeningEvent.Invoke();
    }
  }

  public void StartSmeling()
  {
    DOTween.To(() => actualLipMovementAmplitude, x => actualLipMovementAmplitude = x, lipMovementAmplitudeWhenSmeling, 1);
    //Sound
    FMODUnity.RuntimeManager.PlayOneShot("event:/EgeoSmelling");
    smelling = true;
  }

  public void StopSmeling()
  {
    DOTween.To(() => actualLipMovementAmplitude, x => actualLipMovementAmplitude = x, lipMovementAmplitude, 1);
    smelling = false;
  }

  public void StartEating()
  {
    eating = true;
    lipUpTarget.DOMove(lipUpOpened.position, 0.5f);
    lipDownTarget.DOMove(lipDownOpened.position, 0.5f);
        //Sound
        FMODUnity.RuntimeManager.PlayOneShot("event:/EgeoSwalling");
    }

  public void StopEating()
  {
    eating = false;
    DOTween.To(() => actualLipMovementAmplitude, x => actualLipMovementAmplitude = x, lipMovementAmplitude, 1);
    StartClosing();
  }

  void StartClosing()
  {
    closing = true;
    lipUpTarget.DOMove(lipUpClosed.position, 0.5f);
    lipDownTarget.DOMove(lipDownClosed.position, 0.5f).OnComplete(StopClosing);
   }

  void StopClosing()
  {
    closing = false;
  }

  public void StartBlowing()
  {
    blowing = true;

        //Sound
        FMODUnity.RuntimeManager.PlayOneShot("event:/EgeoBlowing");
        //

    Sequence sequence = DOTween.Sequence();
    sequence.Append(lipUpTarget.DOMove(lipDownClosed.position, 0.5f));
    sequence.Append(lipUpTarget.DOMove(lipUpOpened.position, 0.1f));
    sequence.Append(lipUpTarget.DOMove(lipUpClosed.position, 0.1f));
    sequence.OnComplete(StopBlowing);
  }

  void StopBlowing()
  {
    StartClosing();
    blowing = false;
  }
}
