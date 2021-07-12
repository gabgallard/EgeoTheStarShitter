using UnityEngine;
using UnityEngine.UI;

public class BublingMaterialInitializerForImage : MonoBehaviour
{
  void Awake()
  {
    Debug.Log("BublingMaterialInitializerForImage.Awake()");
    Image image = gameObject.GetComponent<Image>();
    image.material = new Material(image.material);
    image.material.SetFloat("Seed", Random.Range(0f, 100f));
  }
}
