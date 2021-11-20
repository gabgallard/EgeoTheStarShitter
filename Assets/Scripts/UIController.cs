using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    Animator animator;


    [SerializeField] GameObject infoPage;
    Camera theCamera;

    public FMODUnity.StudioEventEmitter loopSoundEmitter;
    public GameObject winStateParameterChanger;

    //FMOD events
    private FMOD.Studio.EventInstance sfxThunder;
    FMOD.Studio.EventInstance pause;

    private bool pauseState;

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
        //initialiting pause sound conditions
        pauseState = false;
    }

    private void Update()
    {

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

        //Fmod Pause Event
        //still not working well. Needs debugging

        if (!pauseState)
        {
            pause.start();
            pause.keyOff();
            pauseState = true;
        }
        else if (pauseState)
        {
            pause.release();
            pause.keyOff();
            pauseState = false;
        }
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

    //FMOD event
    void PlaybackBckgrSound()
    {
        //bckgrLoop.start();
        //FMODUnity.RuntimeManager.PlayOneShot("event:/BckgrLoop");
    }
}


