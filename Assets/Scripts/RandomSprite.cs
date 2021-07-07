using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSprite : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;

    void Awake()
    {
      Sprite sprite = sprites[Random.Range(0, sprites.Length)];
      GetComponent<SpriteRenderer>().sprite = sprite;
    }
}
