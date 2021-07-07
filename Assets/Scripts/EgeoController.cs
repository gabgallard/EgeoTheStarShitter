using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EgeoController : MonoBehaviour
{
  public static EgeoController Instance;

  int numOfPlanetsEaten = 0;
  int numOfPlanetsOnStomach = 0;

  void Awake()
  {
    Instance = this;
  }

  void ShitStar()
  {
    StarSpawnerController.Instance.SpawnStar();
    numOfPlanetsOnStomach = 0;
  }

  public void AddPlanetToStomach()
  {
    numOfPlanetsOnStomach ++;
    numOfPlanetsEaten ++;

    if(numOfPlanetsOnStomach >= 3) {
      ShitStar();
    }
  }
}
