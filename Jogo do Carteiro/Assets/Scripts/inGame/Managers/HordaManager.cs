using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HordaManager : MonoBehaviour
{
    [Header("Referência ao Spawner")]
    public SpawnerManager spawnerManager;

    private int NumeroHorda, N_Entregas;
    [Header("Controle Hordas")]
    public int E_Necessarias;
    public TextMeshProUGUI TextoHorda;
    public float aumentovelocidade;
    public float ReduzirIntervalo;

    private bool HordaMudou, Objetivo;
    public static HordaManager instance;
    [System.Serializable]
    public class TagsPorHorda
    {
        public int horda;
        public List<string> tagsPermitidas;
    }

    [Header("Configuração de Inimigos por Horda")]
    public List<TagsPorHorda> tagsPorHorda = new List<TagsPorHorda>();

    private void Start()
    {
        NumeroHorda = 1;
        AtualizarInimigosPermitidos();
    }
    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        verificarhorda();
        TextoHorda.text = "Horda: " + NumeroHorda;
    }
    private void Mudarcondicao()
    {
        if (HordaMudou)
        {
            // velocidade dos inimigos da horda
            float novoMultiplicador = spawnerManager.multiplicadorVelocidade + aumentovelocidade;
            spawnerManager.DefinirVelocidade(novoMultiplicador);
            // intervalo de spawn deles
            float novoIntervalo = spawnerManager.intervaloSpawn - ReduzirIntervalo;
            spawnerManager.DefinirIntervalo(novoIntervalo);
            HordaMudou = false;
        }
    }
    private void verificarhorda()
    {
        if (N_Entregas >= E_Necessarias)
        {
            Objetivo = true;
        }
        if (Objetivo)
        {
            AlterarHorda();
        }
    }
    private void AlterarHorda()
    {
        NumeroHorda = NumeroHorda + 1;
        HordaMudou = true;
        Objetivo = false;
        N_Entregas = 0;
        Mudarcondicao();
        AtualizarInimigosPermitidos();
    }
    public void AumentarEntrega()
    {
        N_Entregas++;
        Debug.Log(N_Entregas);
    }
    private void AtualizarInimigosPermitidos()
    {
        foreach (var config in tagsPorHorda)
        {
            if (config.horda == NumeroHorda)
            {
                spawnerManager.DefinirTagsPermitidas(config.tagsPermitidas);
                Debug.Log($"[HordaManager] Tags permitidas na horda {NumeroHorda}: {string.Join(", ", config.tagsPermitidas)}");
                return;
            }
        }

        spawnerManager.DefinirTagsPermitidas(new List<string>());
    }
    public void DefinirTagsPermitidas(List<string> tags)
    {
        spawnerManager.tagsPermitidas = tags;
        Debug.Log($"[SpawnerManager] Tags permitidas atualizadas: {string.Join(", ", tags)}");
    }
}
