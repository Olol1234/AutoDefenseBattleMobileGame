using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("For tuning")]
    private float lastTimeShoot;
    [SerializeField] private float shootAudioCooldown = 0.05f;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip clickSound;
    public AudioClip cancelSound;

    [Header("Weapon Sounds")]
    public AudioClip playerShootSound;
    public AudioClip homingMissileSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayClick()
    {
        sfxSource.PlayOneShot(clickSound);
    }

    public void PlayCancel()
    {
        sfxSource.PlayOneShot(cancelSound);
    }

    public void PlayPlayerShoot()
    {
        if (Time.time - lastTimeShoot < shootAudioCooldown)
        {
            return;
        }
        lastTimeShoot = Time.time;
        sfxSource.pitch = Random.Range(0.95f, 1.05f);
        sfxSource.PlayOneShot(playerShootSound);
    }

    public void PlayHomingMissile()
    {
        sfxSource.pitch = Random.Range(0.95f, 1.05f);
        sfxSource.PlayOneShot(homingMissileSound);
    }
}