using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EgeoController : MonoBehaviour
{
  public static EgeoController Instance;

  [SerializeField] public Transform MouthInside;
  [SerializeField] MouthController mouthController;
  [SerializeField] MouthController bottomController;

  [Header("Shitting settings")]
  [SerializeField] float bottomFullDuration = 3;
  [SerializeField] float shittingDuration = 1;
  [SerializeField] float afterShittingDuration = 2;

  int numOfPlanetsEaten = 0;
  int numOfPlanetsOnStomach = 0;

  void Awake()
  {
    Instance = this;
  }

  void ShitStar()
  {
    StarSpawnerController.Instance.SpawnStar();
  }

  public void AddPlanetToStomach()
  {
    numOfPlanetsOnStomach ++;
    numOfPlanetsEaten ++;

    if(numOfPlanetsOnStomach >= 3) {
      StartShitting();
      numOfPlanetsOnStomach = 0;
    }
  }

  public void StartShitting()
  {
    StartCoroutine("SpawnStarCoroutine");
  }

  public void StartEating() {
    mouthController.StartEating();
  }

  public void StopEating() {
    mouthController.StopEating();
  }

  public void StartSmeling() {
    mouthController.StartSmeling();
  }

  public void StopSmeling() {
    mouthController.StopSmeling();
  }

  IEnumerator SpawnStarCoroutine()
  {
    bottomController.StartSmeling();
    yield return new WaitForSeconds(RandomDeviation(bottomFullDuration));

    bottomController.StopSmeling();
    bottomController.StartEating();
    yield return new WaitForSeconds(RandomDeviation(shittingDuration));

    ShitStar();
    yield return new WaitForSeconds(RandomDeviation(afterShittingDuration));
    bottomController.StopEating();
  }

  float RandomDeviation(float number) {
    return Random.Range(number - (number / 2), number + (number / 2));
  }
}
