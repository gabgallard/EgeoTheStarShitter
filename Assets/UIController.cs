using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIController : MonoBehaviour
{
  Animator animator;

  [SerializeField] GameObject infoPage;

  void Awake()
  {
    animator = GetComponent<Animator>();
  }

  // Start is called before the first frame update
  void Start()
  {
    animator.SetTrigger("Intro");
  }

  void StartSky()
  {
    Debug.Log("StartSky()");
    PlanetSpawnerController.Instance.FirstSpawn();
  }

  public void ToggleInfoPage()
  {
    infoPage.SetActive(!infoPage.activeSelf);
  }
}
