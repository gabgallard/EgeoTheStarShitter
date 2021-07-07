using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EgeoController : MonoBehaviour
{
  public static EgeoController Instance;
  [SerializeField] public Transform MouthInside;
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
  int numOfPlanetsEaten = 0;
  int numOfPlanetsOnStomach = 0;


  void Awake()
  {
    Instance = this;
  }

  void Start()
  {
    StopEating();
  }

  void Update()
  {
    if(!eating)
      MouthClosedAnimation();
  }

  void MouthClosedAnimation()
  {
    float height = actualLipMovementAmplitude * Mathf.PerlinNoise(Time.time * lipMovementSpeed, 0.0f);
    Vector3 pos = lipUpClosed.position;
    pos.y = pos.y + (height - (actualLipMovementAmplitude / 2.0f));
    lipUpTarget.transform.position = pos;
  }

  public void StartSmeling()
  {
    actualLipMovementAmplitude = lipMovementAmplitudeWhenSmeling;
  }

  public void StopSmeling()
  {
    actualLipMovementAmplitude = lipMovementAmplitude;
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
    actualLipMovementAmplitude = lipMovementAmplitude;
    lipUpTarget.DOMove(lipUpClosed.position, 0.5f);
    lipDownTarget.DOMove(lipDownClosed.position, 0.5f);

    numOfPlanetsOnStomach ++;
    numOfPlanetsEaten ++;

    if(numOfPlanetsOnStomach >= 3) {
      ShitStar();
    }
  }

  void ShitStar()
  {
    StarSpawnerController.Instance.SpawnStar();
    numOfPlanetsOnStomach = 0;
  }
}
