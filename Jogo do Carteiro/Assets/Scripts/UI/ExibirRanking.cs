using UnityEngine;
using TMPro;

public class ExibirRanking : MonoBehaviour
{
    // Cada um com 5
    public TextMeshProUGUI[] grupoEsquerda; 
    public TextMeshProUGUI[] grupoDireita;  

    private void OnEnable()
    {
        AtualizarRanking();
    }

    public void AtualizarRanking()
    {
        var lista = LeaderboardManager.instance.ObterRanking();

        // garante no mínimo 10 entradas (preenche com vazios)
        while (lista.Count < 10)
            lista.Add(new EntradaRanking("---", 0));

        // posições 1 a 5
        for (int i = 0; i < 5; i++)
        {
            grupoEsquerda[i].text = $"{i + 1}. {lista[i].nome} - {lista[i].pontuacao}";
        }

        // posições 6 a 10
        for (int i = 5; i < 10; i++)
        {
            grupoDireita[i - 5].text = $"{i + 1}. {lista[i].nome} - {lista[i].pontuacao}";
        }
    }
}
