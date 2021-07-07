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


  [Header("Settings")]
  [SerializeField] float force = 2.0f;
  [SerializeField] float firstProjectionDuration = 3.0f;
  [SerializeField] float temporalMass = 10.0f;
  [SerializeField] float temporalMassDuration = 1.0f;

  void Awake()
  {
    Instance = this;
    randomPointInCollider = new RandomPointInCollider(skyCollider);
  }

  [ContextMenu("SpawnStar")]
  public void SpawnStar()
  {
    StartCoroutine("SpawnStarCoroutine");
  }

  // void Update()
  // {
  //     if (Input.GetKeyDown("space"))
  //     {
  //         SpawnStar();
  //     }
  // }

  IEnumerator SpawnStarCoroutine()
  {
    skyCollider.enabled = true;
    Vector3 position = randomPointInCollider.RandomPoint();
    skyCollider.enabled = false;
    GameObject star = Instantiate(starPrefab, position, Quaternion.identity);
    Transform body = star.transform.Find("Body");
    body.position = transform.position;
    SpringJoint2D bodySpringJoint = body.GetComponent<SpringJoint2D>();
    bodySpringJoint.enabled = false;
    Rigidbody2D bodyRigidbody = body.gameObject.GetComponent<Rigidbody2D>();
    bodyRigidbody.AddForce((target.position - body.position) * RandomDeviation(force), ForceMode2D.Impulse);

    yield return new WaitForSeconds(RandomDeviation(firstProjectionDuration));

    float previousDrag = bodyRigidbody.drag;
    bodyRigidbody.drag = RandomDeviation(temporalMass);
    bodySpringJoint.enabled = true;
    bodyRigidbody.AddForce(Vector2.up * RandomDeviation(force) * temporalMass * 3, ForceMode2D.Impulse);

    yield return new WaitForSeconds(RandomDeviation(temporalMassDuration));
    bodyRigidbody.drag = previousDrag;
  }

  float RandomDeviation(float number) {
    return Random.Range(number - (number / 2), number + (number / 2));
  }

}
