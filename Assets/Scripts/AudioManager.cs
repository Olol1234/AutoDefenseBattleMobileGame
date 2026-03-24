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

    [Header("Stage Win/Lose Music")]
    [SerializeField] private AudioSource stageResultMusicSource;
    public AudioClip stageWinMusic;
    public AudioClip stageLoseMusic;

    [Header("Looping Sources")]
    [SerializeField] private AudioSource laserLoopSource;

    [Header("Music Sources")]
    [SerializeField] private AudioSource musicSource;
    public AudioClip ambientMenuMusic;

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

    public void PlayMenuMusic()
    {
        if (musicSource.isPlaying && musicSource.clip == ambientMenuMusic) return;

        musicSource.clip = ambientMenuMusic;
        musicSource.loop = true; // Essential for that 7-min loop
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
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

    public void PlayStageWin()
    {
        stageResultMusicSource.clip = stageWinMusic;
        stageResultMusicSource.loop = false;
        stageResultMusicSource.Play();
    }

    public void PlayStageLose()
    {
        stageResultMusicSource.clip = stageLoseMusic;
        stageResultMusicSource.loop = false;
        stageResultMusicSource.Play();
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