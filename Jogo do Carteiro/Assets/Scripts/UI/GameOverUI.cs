using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverController : MonoBehaviour
{
    [Header("Painéis")]
    [SerializeField] private GameObject fundoCinza;
    [SerializeField] private GameObject painelGameOver;
    [SerializeField] private GameObject painelConfirmacao;
    [SerializeField] private GameObject painelRanking;

    [Header("UI Nome")]
    [SerializeField] private TMP_InputField nomeInput;
    [SerializeField] private TextMeshProUGUI feedbackTexto;

    private System.Action acaoConfirmada;
    private bool pontuacaoSalva = false;

    private void Start()
    {
        fundoCinza.SetActive(false);
        painelGameOver.SetActive(false);
        painelConfirmacao.SetActive(false);
        painelRanking.SetActive(false);

        if (feedbackTexto != null)
            feedbackTexto.text = "";

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
        painelRanking.SetActive(false);

        pontuacaoSalva = false;
    }

    // ------------------------
    // SALVAR PONTUAÇÃO
    // ------------------------
    public void BotaoSalvarPontuacao()
    {
        if (pontuacaoSalva) return;

        string nome = nomeInput.text;

        if (string.IsNullOrWhiteSpace(nome))
            nome = "Jogador";

        int pontos = ScoreManager.instance.pontuacaoAtual;

        LeaderboardManager.instance.AdicionarEntrada(nome, pontos);

        pontuacaoSalva = true;

        // Troca de painel: vai para o ranking
        painelGameOver.SetActive(false);
        painelRanking.SetActive(true);
    }

    // ------------------------
    // BOTÕES DO RANKING
    // ------------------------
    public void RankingReiniciar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RankingMenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
    }

    // ------------------------
    // CONFIRMAÇÃO ORIGINAL (reiniciar/sair do Game Over)
    // ------------------------
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
