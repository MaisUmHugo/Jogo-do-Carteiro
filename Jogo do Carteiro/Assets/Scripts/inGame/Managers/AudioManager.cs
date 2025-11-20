using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Referências")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioSource bgmSource;
    public AudioSource cutsceneSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Músicas")]
    [SerializeField] private AudioClip musicaMenu;
    //[SerializeField] private AudioClip cutsceneAudio;
    [SerializeField] private AudioClip musicaJogo;
    [SerializeField] private AudioClip musicaGameOver;

    private void Awake()
    {

        if (instance != null && instance != this)
        {
            Debug.LogWarning("Outro AudioManager encontrado e destruído!");
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }

        if (cutsceneSource == null)
        {
            cutsceneSource = gameObject.AddComponent<AudioSource>();
            cutsceneSource.loop = false;
            cutsceneSource.playOnAwake = false;
        }

        var groups = mixer.FindMatchingGroups("Master");
        if (groups.Length > 0)
        {
            bgmSource.outputAudioMixerGroup = groups[0];
            sfxSource.outputAudioMixerGroup = groups[0];
            cutsceneSource.outputAudioMixerGroup = groups[0];
        }

        AudioSettings.AplicarVolumesIniciais(mixer);
    }

    // MÚSICAS 
    public void TocarMusica(AudioClip clip, bool loop = true)
    {
        if (clip == null || (bgmSource.clip == clip && bgmSource.isPlaying))
            return;

        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.time = 0f;
        bgmSource.Play();
    }

    public void TocarMusicaMenu() => TocarMusica(musicaMenu);
    public void TocarMusicaJogo() => TocarMusica(musicaJogo);
    public void TocarMusicaGameOver() => TocarMusica(musicaGameOver, false);

    public void PararMusica()
    {
        bgmSource.Stop();
    }

    // EFEITOS 
    public void TocarSFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    // VOLUME 
    public void AjustarVolume(string parametro, float valor)
    {
        AudioSettings.AplicarVolume(mixer, parametro, valor);
    }

    public AudioMixer Mixer => mixer;
}
