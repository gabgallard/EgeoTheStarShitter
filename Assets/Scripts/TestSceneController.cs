using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSceneController : MonoBehaviour
{
  void Start()
  {
    PlanetSpawnerController.Instance.FirstSpawn();
  }

  void Update()
  {

  }
}
