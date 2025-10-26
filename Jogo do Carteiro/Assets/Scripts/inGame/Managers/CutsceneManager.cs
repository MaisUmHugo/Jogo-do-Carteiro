using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CutsceneManager : MonoBehaviour
{
    [Header("Configurações da Cutscene")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string cenaJogo = "CenaJogo";
    [SerializeField] private bool podePular = true;

    void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Prepare();
            videoPlayer.prepareCompleted += (vp) =>
            {
                // Agora o áudio e o vídeo estão prontos
                vp.Play();
            };
            videoPlayer.loopPointReached += OnVideoEnd;
        }
        else
        {
            Debug.LogWarning("Nenhum VideoPlayer atribuído — usando fallback temporizador.");
            Invoke(nameof(CarregarCenaJogo), 5f);
        }
    }

    void Update()
    {
        // Permite pular cutscene
        if (podePular && (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Submit")))
        {
            CarregarCenaJogo();
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        CarregarCenaJogo();
    }

    private void CarregarCenaJogo()
    {
        SceneManager.LoadScene(cenaJogo);
    }
}
