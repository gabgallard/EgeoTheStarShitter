using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

  float actualLipMovementAmplitude;
  bool eating = false;
  bool closing = false;

  void Start()
  {
    StopEating();
  }

  void Update()
  {
    if(!eating && !closing)
      MouthClosedAnimation();
  }

  void MouthClosedAnimation()
  {
    float movement = actualLipMovementAmplitude * Mathf.PerlinNoise(Time.time * lipMovementSpeed, 0.0f);
    movement = movement - (actualLipMovementAmplitude / 2.0f);

    Vector3 position = lipUpClosed.position;
    position.y = position.y + movement;
    lipUpTarget.transform.position = position;

    position = lipDownClosed.position;
    position.y = position.y - movement;
    lipDownTarget.transform.position = position;
  }

  public void StartSmeling()
  {
    DOTween.To(() => actualLipMovementAmplitude, x => actualLipMovementAmplitude = x, lipMovementAmplitudeWhenSmeling, 1);
  }

  public void StopSmeling()
  {
    DOTween.To(() => actualLipMovementAmplitude, x => actualLipMovementAmplitude = x, lipMovementAmplitude, 1);
  }

  public void StartEating()
  {
    eating = true;
    lipUpTarget.DOMove(lipUpOpened.position, 0.5f);
    lipDownTarget.DOMove(lipDownOpened.position, 0.5f);
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
}
