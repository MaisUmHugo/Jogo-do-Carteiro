using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUIListener : MonoBehaviour
{
    [Header("Painéis")]
    public GameObject painelGameOver;
    public GameObject painelRanking;

    [Header("Referências do Game Over")]
    public TextMeshProUGUI textoPontuacao;
    public GameObject grupoNome;
    public TMP_InputField campoNome;
    public Button botaoConfirmar;
    public GameObject grupoBotoesSemRecorde;

    private int ultimaPontuacao;

    private void OnEnable()
    {
        VidaManager.instance.OnGameOver += AbrirGameOver;
    }

    private void OnDisable()
    {
        VidaManager.instance.OnGameOver -= AbrirGameOver;
    }

    private void AbrirGameOver()
    {
        // PEGAR A PONTUAÇÃO DO SCOREMANAGER
        ultimaPontuacao = ScoreManager.instance.pontuacaoAtual;

        painelGameOver.SetActive(true);
        painelRanking.SetActive(false);

        textoPontuacao.text = "Pontuação: " + ultimaPontuacao;

        // ENTROU NO TOP 10?
        if (LeaderboardManager.instance.Top10(ultimaPontuacao))
        {
            grupoNome.SetActive(true);
            grupoBotoesSemRecorde.SetActive(false);

            botaoConfirmar.onClick.RemoveAllListeners();
            botaoConfirmar.onClick.AddListener(SalvarRecorde);
        }
        else
        {
            grupoNome.SetActive(false);
            grupoBotoesSemRecorde.SetActive(true);
        }
    }

    private void SalvarRecorde()
    {
        string nome = string.IsNullOrWhiteSpace(campoNome.text) ? "Player" : campoNome.text;

        LeaderboardManager.instance.AdicionarEntrada(nome, ultimaPontuacao);

        painelGameOver.SetActive(false);
        painelRanking.SetActive(true);
    }
}
