using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIController : MonoBehaviour
{
  [SerializeField] TMP_Text text_01;
  [SerializeField] TMP_Text text_02;
  [SerializeField] TMP_Text text_03;
  [SerializeField] Image background;

  void Awake()
  {
    text_01.enabled = false;
    text_02.enabled = false;
    text_03.enabled = false;
  }

  // Start is called before the first frame update
  void Start()
  {
    StartCoroutine("TitlesCoroutine");
  }

  IEnumerator TitlesCoroutine()
  {
    yield return new WaitForSeconds(1.0f);

    // At the beginning was Egeo
    text_01.DOFade(0.0f, 0.0f);
    background.DOFade(0.0f, 0.0f);

    text_01.enabled = true;
    background.enabled = true;

    background.DOFade(1.0f, 1.0f);

    yield return new WaitForSeconds(2.0f);
    text_01.DOFade(1.0f, 1.0f);

    EgeoController.Instance.ShowEgeo();

    yield return new WaitForSeconds(3.0f);
    text_01.DOFade(0.0f, 1.0f);

    yield return new WaitForSeconds(0.5f);
    background.DOFade(0.0f, 1.0f);

    yield return new WaitForSeconds(1f);
    text_01.enabled = false;
    background.enabled = false;

    yield return new WaitForSeconds(6.0f);
    // At the beginning were the Planets
    text_02.DOFade(0.0f, 0.0f);
    background.DOFade(0.0f, 0.0f);

    text_02.enabled = true;
    background.enabled = true;

    background.DOFade(1.0f, 1.0f);

    yield return new WaitForSeconds(1.5f);
    text_02.DOFade(1.0f, 1.0f);

    yield return new WaitForSeconds(3.0f);
    text_02.DOFade(0.0f, 1.0f);

    yield return new WaitForSeconds(1.5f);
    background.DOFade(0.0f, 1.0f);

    PlanetSpawnerController.Instance.FirstSpawn();

    yield return new WaitForSeconds(1f);
    text_02.enabled = false;
    background.enabled = false;

    yield return new WaitForSeconds(6.0f);
    // but, how the stars appeared?
    text_03.DOFade(0.0f, 0.0f);
    background.DOFade(0.0f, 0.0f);

    text_03.enabled = true;
    background.enabled = true;

    background.DOFade(1.0f, 1.0f);

    yield return new WaitForSeconds(1.5f);
    text_03.DOFade(1.0f, 1.0f);

    yield return new WaitForSeconds(3.0f);
    text_03.DOFade(0.0f, 1.0f);

    yield return new WaitForSeconds(1.5f);
    background.DOFade(0.0f, 1.0f);

    yield return new WaitForSeconds(1f);
    text_03.enabled = false;
    background.enabled = false;
  }

}
