using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("--------------- Audio source ---------------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("--------------- Audio clip ---------------")]
    public AudioClip backgroundMenu;
    public AudioClip backgroundGame;
    public AudioClip newGame;
    public AudioClip endGame;
    public AudioClip islandMiss;
    public AudioClip islandSuccess;
    public AudioClip timeTick;
    public AudioClip buttonClick;


    public static AudioManager instance;
    private float musicTime;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        /*musicSource.clip = backgroundMenu;
        musicSource.Play();*/

        if (!musicSource.isPlaying)
        {
            musicSource.clip = backgroundMenu;
            musicSource.time = musicTime; // Resume from last position
            musicSource.Play();
        }
    }

    public void PlayBackground(AudioClip clip)
    {
        if (musicSource.clip == clip)
            return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    private void OnApplicationQuit()
    {
        musicTime = musicSource.time; // Save the current time when quitting
    }
}
