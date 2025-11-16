using UnityEngine;
using TMPro;

public class ExibirRanking : MonoBehaviour
{
    public TextMeshProUGUI rankingTexto;

    private void OnEnable()
    {
        AtualizarRanking();
    }

    public void AtualizarRanking()
    {
        var lista = LeaderboardManager.instance.ObterRanking();

        rankingTexto.text = "";

        int pos = 1;
        foreach (var entrada in lista)
        {
            rankingTexto.text += $"{pos}. {entrada.nome} - {entrada.pontuacao}\n";
            pos++;
        }
    }
}
