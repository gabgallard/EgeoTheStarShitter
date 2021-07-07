using UnityEngine;

public class MaterialDuplicator : MonoBehaviour
{
  void Awake()
  {
    Renderer renderer = gameObject.GetComponent<Renderer>();
    renderer.material = new Material(renderer.material);
  }
}
