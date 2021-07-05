using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BublingMaterialInitializer : MonoBehaviour
{
  void Awake()
  {
    Renderer renderer = gameObject.GetComponent<Renderer>();
    renderer.material = new Material(renderer.material);
    renderer.material.SetFloat("Seed", Random.Range(0f, 100f));
  }
}
