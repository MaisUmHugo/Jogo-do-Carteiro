using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Referências")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Músicas")]
    [SerializeField] private AudioClip musicaMenu;
    [SerializeField] private AudioClip musicaJogo;
    [SerializeField] private AudioClip musicaGameOver;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
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

        AudioSettings.AplicarVolumesIniciais(mixer);
    }

    // ---- MÚSICAS ----
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

    // ---- EFEITOS ----
    public void TocarSFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    // ---- VOLUME ----
    public void AjustarVolume(string parametro, float valor)
    {
        AudioSettings.AplicarVolume(mixer, parametro, valor);
    }

    public AudioMixer Mixer => mixer;
}
