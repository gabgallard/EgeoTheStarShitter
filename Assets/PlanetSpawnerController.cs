using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetSpawnerController : MonoBehaviour
{
  RandomPointInCollider randomPointInCollider;

  Collider2D theCollider;
  [SerializeField] int numOfPlanets;
  [SerializeField] GameObject planetPrefab;

  void Awake()
  {
    theCollider = GetComponent<Collider2D>();
    randomPointInCollider = new RandomPointInCollider(theCollider);
  }

  // Start is called before the first frame update
  void Start()
  {
    for (int i = 0; i < numOfPlanets; i++)
    {
      Vector3 position = randomPointInCollider.RandomPoint();
      GameObject planet = Instantiate(planetPrefab, position, Quaternion.identity);
    }

    theCollider.enabled = false;
  }
}
