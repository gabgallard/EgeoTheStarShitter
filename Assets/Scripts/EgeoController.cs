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
  [SerializeField] int numStarsForAWonderfulUniverse = 21;
  [SerializeField] int numStarsOnEachShit = 2;
  [SerializeField] float foodOnEachPlanet = 2;
  [SerializeField] float stomachFull = 6;

  [Header("Shitting settings")]
  [SerializeField] float bottomFullDuration = 3;
  [SerializeField] float shittingDuration = 1;
  [SerializeField] float afterShittingDuration = 2;

  float foodOnStomach = 0;
  int numOfStars = 0;

  public bool UniverseFinished = false;

  void Awake()
  {
    Instance = this;
    numStarsForAWonderfulUniverse = RandomDeviation(numStarsForAWonderfulUniverse);
  }

  void Update()
  {
    if(Input.GetKeyDown(KeyCode.S))
      StartShitting();
  }

  void ShitStar()
  {
    int numOfStarsToShit = RandomDeviation(numStarsOnEachShit);
    StarSpawnerController.Instance.SpawnStar(numOfStarsToShit);
    numOfStars += numOfStarsToShit;

    if(numOfStars >= numStarsForAWonderfulUniverse)
    {
      UIController.Instance.ShowWonderfulUniverseMessage();
      UniverseFinished = true;
    }
  }

  public void AddPlanetToStomach()
  {
    foodOnStomach += RandomDeviation(foodOnEachPlanet);

    if(foodOnStomach >= stomachFull) {
      StartShitting();
      foodOnStomach = 0;
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

  public void Blow(PlanetBodyController planetBodyController)
  {
    mouthController.StartBlowing();
  }

  float RandomDeviation(float number) {
    return Random.Range(number - (number / 2), number + (number / 2));
  }

  int RandomDeviation(int number) {
    return Random.Range(number - (number / 2), number + (number / 2));
  }
}
