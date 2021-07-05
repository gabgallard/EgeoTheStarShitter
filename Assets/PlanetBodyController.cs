using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetBodyController : MonoBehaviour
{
  [SerializeField] float shakingSeconds;
  [SerializeField] float shakingAmount;
  [SerializeField] float shakingSpeed;

  void Shaking()
  {
    transform.position =
      new Vector3(
        transform.position.x + Mathf.Sin(Time.time * shakingSpeed) * shakingAmount,
        transform.position.y + Mathf.Sin(Time.time * shakingSpeed) * shakingAmount,
        transform.position.z
      );
  }

  void Update()
  {
    Shaking();
  }
}
