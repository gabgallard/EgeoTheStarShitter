using UnityEngine;

public class Draggable : MonoBehaviour
{
    Vector3 offset;

    void OnMouseDown()
    {
      Debug.Log("Draggable.OnMouseDown()");
      offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void OnMouseDrag()
    {
      Debug.Log("Draggable.OnMouseDrag()");
      transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
    }
}
