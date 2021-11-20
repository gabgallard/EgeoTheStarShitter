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
  bool loading = true;

  private FMOD.Studio.EventInstance planetSpawnSound;

  void Awake()
  {
    Instance = this;
    theCollider = GetComponent<Collider2D>();
    randomPointInCollider = new RandomPointInCollider(theCollider);
    planets = new List<GameObject>();
  }

  void Update()
  {
    if(!loading && !EgeoController.Instance.UniverseFinished && Time.time > nextSpawnAt && planets.Count < maxNumOfPlanets)
      SpawnPlanet(Random.Range(1, numOfPlanetsOnSpawn));
  }

  void SetNextSpawnAt()
  {
    nextSpawnAt = Time.time + betweenSpawnsDuration + Random.Range(betweenSpawnsDuration - (betweenSpawnsDuration / 2f), betweenSpawnsDuration + (betweenSpawnsDuration / 2f));
  }


  public void FirstSpawn()
  {
    SpawnPlanet(numOfPlanetsAtStart);
    loading = false;
        //Sound
        planetSpawnSound = FMODUnity.RuntimeManager.CreateInstance("event:/SpawnPlanet");
        planetSpawnSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        planetSpawnSound.start();
        planetSpawnSound.release();
  }

  void SpawnPlanet(int numPlanets = 1)
  {
    theCollider.enabled = true;
    for (int i = 0; i < numPlanets; i++)
    {
      Vector3 position = randomPointInCollider.RandomPoint();
      GameObject planet = Instantiate(planetPrefab, position, Quaternion.identity);
      planets.Add(planet);

      //Sound
      FMODUnity.RuntimeManager.PlayOneShot("event:/SpawnPlanet");
    }
    theCollider.enabled = false;

    SetNextSpawnAt();
  }

  public void RemovePlanet(GameObject planet)
  {
    planets.Remove(planet);
  }
}
