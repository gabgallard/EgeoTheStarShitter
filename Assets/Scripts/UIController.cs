using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    Animator animator;

    [SerializeField] GameObject infoPage;
    Camera theCamera;

    //FMOD stuff
    public FMODUnity.StudioEventEmitter loopSoundEmitter;
    public GameObject winStateParameterChanger;
    private FMOD.Studio.EventInstance sfxThunder;

    void Awake()
    {
        animator = GetComponent<Animator>();
        Instance = this;
        theCamera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        animator.SetTrigger("Intro");
    }

    void StartSky()
    {
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

    [ContextMenu("XXX")]
    public void ShowWonderfulUniverseMessage()
    {
        animator.SetTrigger("Wonderful");
    }

    void SkyWhiteFlash()
    {
        Sequence sequence = DOTween.Sequence();

        sfxThunder = FMODUnity.RuntimeManager.CreateInstance("event:/SfxThunder");
        sfxThunder.start();
        sfxThunder.release();

        winStateParameterChanger.SetActive(true);

        sequence.Append(theCamera.DOColor(new Color(0.9764706f, 0.9529412f, 0.9686275f), 0.10f));
        sequence.Append(theCamera.DOColor(new Color(0f, 0f, 0f), 0.10f));
        sequence.Append(theCamera.DOColor(new Color(0.9764706f, 0.9529412f, 0.9686275f), 0.20f));
        sequence.Append(theCamera.DOColor(new Color(0f, 0f, 0f), 0.50f));
    }

    //text sounds
    void PlaybackText1()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/CinText1");
    }
    void PlaybackText2()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/CinText2");
    }
    void PlaybackText3()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/CinText3");
    }
}


