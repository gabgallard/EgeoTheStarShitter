using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
  public static UIController Instance;
  Animator animator;

  [SerializeField] GameObject infoPage;

  void Awake()
  {
    animator = GetComponent<Animator>();
    Instance = this;
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

  void IntroFinished()
  {
  }

  public void ToggleInfoPage()
  {
    infoPage.SetActive(!infoPage.activeSelf);
    infoPage.transform.Find("Scroll").GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
  }

  public void ShowWonderfulUniverseMessage()
  {
    animator.SetTrigger("Wonderful");
  }
}
