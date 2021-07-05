using UnityEngine;

public class Draggable : MonoBehaviour
{
    Vector3 offset;

    void OnMouseDown()
    {
      offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void OnMouseDrag()
    {
      transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
    }
}
