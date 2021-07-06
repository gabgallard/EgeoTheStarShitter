using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetSpawnerController : MonoBehaviour
{
  public static PlanetSpawnerController Instance;

  RandomPointInCollider randomPointInCollider;

  Collider2D theCollider;
  [SerializeField] int numOfPlanetsAtStart;
  [SerializeField] int maxNumOfPlanets;
  [SerializeField] GameObject planetPrefab;
  [SerializeField] float betweenSpawnsDuration;
  [SerializeField] int numOfPlanetsOnSpawn;

  List<GameObject> planets;

  float nextSpawnAt;

  void Awake()
  {
    Instance = this;
    theCollider = GetComponent<Collider2D>();
    randomPointInCollider = new RandomPointInCollider(theCollider);
    planets = new List<GameObject>();
  }

  void Update()
  {
    if(Time.time > nextSpawnAt && planets.Count < maxNumOfPlanets)
      SpawnPlanet(Random.Range(1, numOfPlanetsOnSpawn));
  }

  void SetNextSpawnAt()
  {
    nextSpawnAt = Time.time + betweenSpawnsDuration + Random.Range(betweenSpawnsDuration - (betweenSpawnsDuration / 2f), betweenSpawnsDuration + (betweenSpawnsDuration / 2f));
  }


  // Start is called before the first frame update
  void Start()
  {
    SpawnPlanet(numOfPlanetsAtStart);
  }

  void SpawnPlanet(int numPlanets = 1)
  {
    theCollider.enabled = true;
    for (int i = 0; i < numPlanets; i++)
    {
      Vector3 position = randomPointInCollider.RandomPoint();
      GameObject planet = Instantiate(planetPrefab, position, Quaternion.identity);
      planets.Add(planet);
      Debug.Log("AtBorn: " + planet.GetInstanceID());
    }
    theCollider.enabled = false;

    SetNextSpawnAt();
  }

  public void RemovePlanet(GameObject planet)
  {
    Debug.Log("PlanetSpawnerController.RemovePlanet()");
    Debug.Log("AtRemove: " + planet.GetInstanceID());
    Debug.Log("Index at: " + planets.IndexOf(planet));
    Debug.Log("Before: " + planets.Count);
    planets.Remove(planet);
    Debug.Log("After: " + planets.Count);
  }
}
