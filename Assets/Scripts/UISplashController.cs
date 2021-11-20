using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UISplashController : MonoBehaviour, IPointerClickHandler
{
  Animator animator;

  void Awake()
  {
    animator = GetComponent<Animator>();
  }

  void Start()
  {
    Invoke("ShowClickToStart", 3.0f);
  }

  public void OnPointerClick(PointerEventData eventData)
  {
    FMODUnity.RuntimeManager.PlayOneShot("event:/ClickMenu");
        SceneManager.LoadScene("Game");
  }


  void ShowClickToStart()
  {
    animator.SetTrigger("ClickToStart");
  }
}
