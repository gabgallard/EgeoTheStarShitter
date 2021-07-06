using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EgeoController : MonoBehaviour
{
  public static EgeoController Instance;
  [SerializeField] public Transform MouthInside;

  void Awake()
  {
    Instance = this;
  }

  void Start()
  {

  }

  void Update()
  {

  }
}
