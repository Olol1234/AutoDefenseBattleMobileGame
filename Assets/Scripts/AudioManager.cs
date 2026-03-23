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
    public AudioClip shotgunShootSound;
    // public AudioClip laserShootSound;

    [Header("Looping Sources")]
    [SerializeField] private AudioSource laserLoopSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            sfxSource.volume = savedVolume;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StopAllLoopingSounds()
    {
        if (laserLoopSource != null)
        {
            laserLoopSource.Stop();
        }
        // Add any other looping sources in the future
    }

    public void SetVolume(float volume)
    {
        sfxSource.volume = volume; 
        
        // Save it so the game remembers next time
        PlayerPrefs.SetFloat("MasterVolume", volume);
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

    public void PlayLaserLoop(bool shouldPlay)
    {
        if (shouldPlay)
        {
            if (!laserLoopSource.isPlaying) 
                laserLoopSource.Play();
        }
        else
        {
            laserLoopSource.Stop();
        }
    }

    public void PauseLaser(bool pause)
    {
        if (pause)
        {
            laserLoopSource.Pause();
        }
        else
        {
            if (!laserLoopSource.isPlaying)
                laserLoopSource.UnPause();
        }
    }

    public void PlayShotgunShoot()
    {
        sfxSource.pitch = Random.Range(0.9f, 1.1f); 
        sfxSource.PlayOneShot(shotgunShootSound);
    }

}