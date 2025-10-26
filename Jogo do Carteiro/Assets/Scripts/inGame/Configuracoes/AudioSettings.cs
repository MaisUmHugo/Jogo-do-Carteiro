using UnityEngine;
using UnityEngine.Audio;

public static class AudioSettings
{
    public static void AplicarVolumesIniciais(AudioMixer mixer)
    {
        AplicarVolume(mixer, "masterParameter", PlayerPrefs.GetFloat("masterParameter", 1f));
        AplicarVolume(mixer, "bgmParameter", PlayerPrefs.GetFloat("bgmParameter", 1f));
        AplicarVolume(mixer, "sfxParameter", PlayerPrefs.GetFloat("sfxParameter", 1f));
        AplicarVolume(mixer, "cutsceneParameter", PlayerPrefs.GetFloat("cutsceneParameter", 1f));
    }

    public static void AplicarVolume(AudioMixer mixer, string parametro, float valor)
    {
        float db = Mathf.Log10(Mathf.Clamp(valor, 0.001f, 1f)) * 20f;
        mixer.SetFloat(parametro, db);

        PlayerPrefs.SetFloat(parametro, valor);
        PlayerPrefs.Save();
    }
}
