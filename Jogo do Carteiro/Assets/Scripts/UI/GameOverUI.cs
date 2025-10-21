using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [Header("Painéis")]
    [SerializeField] private GameObject fundoCinza;
    [SerializeField] private GameObject painelGameOver;
    [SerializeField] private GameObject painelConfirmacao;

    private System.Action acaoConfirmada;

    private void Start()
    {
        // Painéis começam desligados
        fundoCinza.SetActive(false);
        painelGameOver.SetActive(false);
        painelConfirmacao.SetActive(false);

        // Se inscreve no evento do VidaManager
        VidaManager.instance.OnGameOver += MostrarGameOver;
    }

    private void OnDestroy()
    {
        if (VidaManager.instance != null)
            VidaManager.instance.OnGameOver -= MostrarGameOver;
    }

    private void MostrarGameOver()
    {
        Time.timeScale = 0f;
        AudioManager.instance.TocarMusicaGameOver();

        fundoCinza.SetActive(true);
        painelGameOver.SetActive(true);
        painelConfirmacao.SetActive(false);

        Debug.Log("Game Over ativado!");
    }

    // --- Botões principais ---
    public void BotaoReiniciar()
    {
        MostrarConfirmacao(() =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }

    public void BotaoMenuPrincipal()
    {
        MostrarConfirmacao(() =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MenuPrincipal");
        });
    }

   /* public void BotaoSair()
    {
        MostrarConfirmacao(() =>
        {
            Application.Quit();
        });
    }
   */

    // --- Confirmação ---
    private void MostrarConfirmacao(System.Action acao)
    {
        painelGameOver.SetActive(false);
        painelConfirmacao.SetActive(true);

        acaoConfirmada = acao;
    }

    public void BotaoConfirmarSim()
    {
        acaoConfirmada?.Invoke();
        acaoConfirmada = null;
    }

    public void BotaoConfirmarNao()
    {
        painelConfirmacao.SetActive(false);
        painelGameOver.SetActive(true);

        acaoConfirmada = null;
    }
}
