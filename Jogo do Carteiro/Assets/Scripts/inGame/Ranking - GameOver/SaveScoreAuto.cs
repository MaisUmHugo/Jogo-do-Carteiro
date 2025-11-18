using UnityEngine;
using TMPro;

public class SaveScoreAuto : MonoBehaviour
{
    public TMP_InputField nomeInput;
    public GameObject painelRanking;
    public GameObject painelSalvar;
    public ExibirRanking exibirRanking;

    private int scoreParaSalvar;

    public void Iniciar(int score)
    {
        scoreParaSalvar = score;
        nomeInput.text = "";
        nomeInput.onEndEdit.AddListener(SalvarAutomatico);
        nomeInput.ActivateInputField();
    }

    private void SalvarAutomatico(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            nome = "Player";

        LeaderboardManager.instance.AdicionarEntrada(nome, scoreParaSalvar);

        // Fecha painel de nome
        painelSalvar.SetActive(false);

        // Abre ranking
        painelRanking.SetActive(true);
        exibirRanking.AtualizarRanking();

        nomeInput.onEndEdit.RemoveListener(SalvarAutomatico);
    }
}
