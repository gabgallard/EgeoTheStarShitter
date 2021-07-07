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
  [SerializeField] float force;

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

  void Update()
  {
      if (Input.GetKeyDown("space"))
      {
          SpawnStar();
      }
  }

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
    bodyRigidbody.AddForce((target.position - body.position) * force, ForceMode2D.Impulse);

    yield return new WaitForSeconds(3.0f);

    float previousDrag = bodyRigidbody.drag;
    bodyRigidbody.drag = 10;
    bodySpringJoint.enabled = true;

    yield return new WaitForSeconds(1.0f);
    bodyRigidbody.drag = previousDrag;
  }

}
