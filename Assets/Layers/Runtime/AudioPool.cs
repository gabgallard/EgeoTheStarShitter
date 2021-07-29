using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ABXY.Layers.Runtime
{
    [ExecuteInEditMode]
    public class AudioPool : MonoBehaviour
    {
        private List<AudioSource> checkedInAudiosources = new List<AudioSource>();


        [SerializeField]
        private List<GameObject> allGos = new List<GameObject>();

        private static AudioPool _audioPoolInstance;
        public static AudioPool audioPoolInstance
        {
            get
            {
                if (_audioPoolInstance == null)
                {
                    _audioPoolInstance = FindObjectOfType<AudioPool>();
                    _audioPoolInstance?.SetFlags();
                }

                if (_audioPoolInstance == null)
                {

                    GameObject newGo = new GameObject();
                    newGo.name = "Audio Pool";

                    _audioPoolInstance = newGo.AddComponent<AudioPool>();
                    _audioPoolInstance.SetFlags();
                }

                return _audioPoolInstance;
            }
        }

        private void OnEnable()
        {
            SetFlags();

#if UNITY_EDITOR
            //UnityEditor.EditorApplication.playModeStateChanged -= OnPlayeNodeStateChange;
            //UnityEditor.EditorApplication.playModeStateChanged += OnPlayeNodeStateChange;
#endif
        }

#if UNITY_EDITOR
        private void OnPlayeNodeStateChange(UnityEditor.PlayModeStateChange state)
        {
            SetFlags();
        }
#endif

        private void Awake()
        {
            SetFlags();
        }

        private void Start()
        {
            if (Application.isPlaying)
                DontDestroyOnLoad(this.gameObject);
        }

        private void SetFlags()
        {
            //gameObject.hideFlags = HideFlags.DontSave ;


        }

        public AudioSource Checkout(string label)
        {
            AudioSource selection = checkedInAudiosources.FirstOrDefault();
            if (selection == null)
                selection = MakeAudiosource();
            else
                checkedInAudiosources.RemoveAt(0);
            selection.transform.SetAsLastSibling();
            selection.name = "[AudioSource][" + label + "]";
            return selection;
        }

        public AudioSource[] Checkout(int count, string label)
        {
            AudioSource[] audioSources = new AudioSource[count];
            for (int index = 0; index < count; index++)
                audioSources[index] = Checkout(label);
            return audioSources;
        }

        public void Return(AudioSource audioSource)
        {
            audioSource.Stop();
            audioSource.clip = null;
            audioSource.loop = false;
            audioSource.name = "[AudioSource][Unused]";
            audioSource.transform.SetAsFirstSibling();
            if (!checkedInAudiosources.Contains(audioSource))
                checkedInAudiosources.Add(audioSource);
        }

        public void Return(AudioSource[] audioSources)
        {
            foreach (AudioSource audiosource in audioSources)
                Return (audiosource);
        }

        private AudioSource MakeAudiosource()
        {
            GameObject go = new GameObject("[AudioSource][Unused]");
            allGos.Add(go);
            go.transform.SetParent(this.transform);
            go.transform.localPosition = Vector3.zero;
            AudioSource audioSource = go.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            return audioSource;
        }

        private void OnDestroy()
        {
            
            foreach (GameObject source in allGos)
            {
                if (Application.isPlaying)
                    Destroy(source);
                else
                    DestroyImmediate(source);
            }
        }
    }
}
