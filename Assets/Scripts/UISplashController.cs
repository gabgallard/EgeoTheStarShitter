using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class UISplashController : MonoBehaviour, IPointerClickHandler
{
  Animator animator;
    AudioSource startSFX;

  void Awake()
  {
    animator = GetComponent<Animator>();
  }

  void Start()
  {
    Invoke("ShowClickToStart", 3.0f);
        startSFX = GetComponent<AudioSource>();
  }

  public void OnPointerClick(PointerEventData eventData)
  {
    SceneManager.LoadScene("Game");
        startSFX.Play();
  }


  void ShowClickToStart()
  {
    animator.SetTrigger("ClickToStart");
  }
}
