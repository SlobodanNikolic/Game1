using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;
    public AudioMixerGroup explosionsGroup;
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup musicGroup;
    public bool soundOn = true;
    public bool musicOn = true;
    public float lowPitchRange = .95f;
    public float highPitchRange = 1.05f;

    public AudioClip bgMusic;
    public AudioClip dropTileSfx;
    public AudioClip[] explosionSfx;
    public AudioClip levelUpSfx;
    public AudioClip gameOverSfx;
    public AudioClip specialTileLineSfx;
	public AudioClip specialTileShuffleSfx;
    public AudioClip wildCardLineSfx;
    public AudioClip wildCardShuffleSfx;
    public AudioClip scoreCountSfx;
    public AudioClip clickSfx;
	public AudioClip unavailableSfx;
	public AudioClip clearBoardSfx;
	public AudioClip erasersSfx;
	public AudioClip multiSfx;
	public AudioClip[] fireworksSfx;

    [HideInInspector] public AudioSource[] sfxSource;
    [HideInInspector] public AudioSource musicSource;
    private int sfxIndex;
    private const int NUM_OF_SFX = 70;

    private static int expIndex = 0;
    private static float lastExpTime = 0;
    private static int  expInc= 1;


    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.outputAudioMixerGroup = musicGroup;
        musicSource.loop = true;

        expIndex = 0;
        lastExpTime = 0f;
        expInc = 1;

        sfxIndex = 0;
        sfxSource = new AudioSource[NUM_OF_SFX];
        for (int i = 0; i < NUM_OF_SFX; i++)
        {
            sfxSource[i] = gameObject.AddComponent<AudioSource>();
            sfxSource[i].playOnAwake = false;
            sfxSource[i].loop = false;
        }
    }

    public void PlayClickSound()
    {
        PlaySound(SoundManager.instance.clickSfx, -1, true);
    }

	public void PlayFireworksSound()
	{
		PlaySound(fireworksSfx[Random.Range(0,fireworksSfx.Length-1)]);
	}


    public void PlaySound(AudioClip clip, float duration=-1f, bool randomizePitch = false, bool explosion=false)
    {
        if (soundOn)
        {
            sfxSource[sfxIndex].clip = clip;

            if (randomizePitch)
                sfxSource[sfxIndex].pitch = Random.Range(lowPitchRange, highPitchRange);
            else
                sfxSource[sfxIndex].pitch = 1f;

            sfxSource[sfxIndex].volume = 1f;
            if (explosion)
            {
                sfxSource[sfxIndex].outputAudioMixerGroup = explosionsGroup;
                NormalizeSfxVolume();
            }
            else
                sfxSource[sfxIndex].outputAudioMixerGroup = sfxGroup;
            
            sfxSource[sfxIndex].Play();
            if (duration > 0f)
            {
                StartCoroutine(FadeOut(sfxSource[sfxIndex], 0f,duration));
            }
            sfxIndex = (sfxIndex + 1) % NUM_OF_SFX;

        }
    }

	public void PlaySoundWithPitch(AudioClip clip, float pitch)
	{
		if (soundOn)
		{
			sfxSource[sfxIndex].clip = clip;
			sfxSource[sfxIndex].pitch = pitch;
			sfxSource[sfxIndex].volume = 1f;
			sfxSource[sfxIndex].outputAudioMixerGroup = sfxGroup;
			sfxSource[sfxIndex].Play();
			sfxIndex = (sfxIndex + 1) % NUM_OF_SFX;
		}
	}

    public void PlayMusic(AudioClip clip=null)
    {
        if (musicOn)
        {
            if (clip == null)
                clip = bgMusic;

            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PauseUnPauseMusic()
    {
        if (!musicOn)
            musicSource.Pause();
        else
            PlayMusic();
    }

    public void PlayNextExplosion()
    {
        if (Time.time - lastExpTime > 1f)
        {
            expInc = 1;
            expIndex = Random.Range(0,4);
        }
        else
        {
            expIndex+=expInc;
            if (expIndex >= explosionSfx.Length)
            {
                expInc = -1;
                expIndex = explosionSfx.Length-2;
            }
            if (expIndex < 0)
            {
                expInc = 1;
                expIndex = 1;
            }

        }



        PlaySound(explosionSfx[expIndex],-1,false,true);
        lastExpTime = Time.time;
    }

    public static IEnumerator FadeOut(AudioSource source, float newVolume=0f, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        int oldPriority = source.priority;
        source.priority = 255;
        while (source.volume > newVolume)
        {
            source.volume -= 0.005f;
            yield return null;
            if (source.priority == 0)
                yield break;
        }
        source.volume = newVolume;
        source.priority = oldPriority;
    }

    public static IEnumerator FadeIn(AudioSource source, float newVolume=1f, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        int oldPriority = source.priority;
        source.priority = 0;
        
        while (source.volume < newVolume)
        {
            source.volume += 0.005f;
            yield return null;
            if (source.priority == 255)
                yield break;
        }
        source.volume = newVolume;
        source.priority = oldPriority;
    }


private void NormalizeSfxVolume(int length=10,float factor=0.5f)
    {
        sfxSource[sfxIndex].volume = 1f;
        for (int i = 1; i <= length; i++)
        {
            if (sfxSource[(NUM_OF_SFX + sfxIndex - i) % NUM_OF_SFX].outputAudioMixerGroup==sfxSource[sfxIndex].outputAudioMixerGroup)
                sfxSource[(NUM_OF_SFX+sfxIndex - i) % NUM_OF_SFX].volume*= factor;
        }
    }
}


    