using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StartSpawnerController : MonoBehaviour
{
  public static StartSpawnerController Instance;
  RandomPointInCollider randomPointInCollider;

  [SerializeField] Collider2D skyCollider;
  [SerializeField] GameObject starPrefab;
  [SerializeField] Transform target;

  void Awake()
  {
    Instance = this;
    randomPointInCollider = new RandomPointInCollider(skyCollider);
  }

  [ContextMenu("SpawnStar")]
  public void SpawnStar()
  {
    skyCollider.enabled = true;
    Vector3 position = randomPointInCollider.RandomPoint();
    skyCollider.enabled = false;
    GameObject star = Instantiate(starPrefab, position, Quaternion.identity);
    Transform body = star.transform.Find("Body");
    body.position = target.position;
    body.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, 0));
  }

}
