using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HordaManager : MonoBehaviour
{
    [Header("Referência ao Spawner")]
    public SpawnerManager spawnerManager;

    [Header("Barra de Progresso")]
    public Image barraProgresso; 

    private int NumeroHorda, N_Entregas;
    [Header("Controle Hordas")]
    public int E_Necessarias;
    public TextMeshProUGUI TextoHorda;
    public TextMeshProUGUI TextoEntrega;
    public float aumentovelocidade;
    public float ReduzirIntervalo;

    //private bool aguardandoInicio = false;

    [Header("Delay de Início das Hordas")]
    [Tooltip("Tempo (em segundos) antes da primeira horda começar após carregar a cena.")]
    public float delayInicial;
    [Tooltip("Tempo de espera entre o fim de uma horda e o início da próxima.")]
    public float delayEntreHordas;

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
        spawnerManager.DesativarSpawn();
        StartCoroutine(IniciarHordaComDelay(delayInicial));
    }
    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        verificarhorda();
        TextoHorda.text = "Horda: " + NumeroHorda;
        TextoEntrega.text = $"Entregas:{N_Entregas}/{E_Necessarias}" ;
        AtualizarBarraProgresso();
        if (Input.GetKeyDown(KeyCode.N))
        {
            AlterarHorda();
        }

    }

    private IEnumerator IniciarHordaComDelay(float delay)
    {
        //aguardandoInicio = true;
        spawnerManager.DesativarSpawn();

        yield return new WaitForSeconds(delay);

        AtualizarInimigosPermitidos(); 
        spawnerManager.AtivarSpawn();
        //aguardandoInicio = false;
    }

    private void AtualizarBarraProgresso()
    {
        if (barraProgresso != null)
        {
            // Calcula o valor do progresso como uma porcentagem
            float progresso = (float)N_Entregas / (float)E_Necessarias;
            barraProgresso.fillAmount = progresso;  // Atualiza a barra de progresso
        }
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

    private bool trocandoHorda = false;

    private void AlterarHorda()
    {
        if (trocandoHorda) return; // impede sobreposição
        trocandoHorda = true;

        NumeroHorda++;
        E_Necessarias++;
        HordaMudou = true;
        Objetivo = false;
        N_Entregas = 0;

        if (NumeroHorda >= 11)
        {
            Final();
            return;
        }

        Mudarcondicao();
        StartCoroutine(DelayProximaHorda());
        Debug.Log($"[HordaManager] Esperando {delayEntreHordas}s antes da próxima horda ({NumeroHorda})");
    }

    private IEnumerator DelayProximaHorda()
    {
        spawnerManager.DesativarSpawn();
        yield return new WaitForSeconds(delayEntreHordas);
        AtualizarInimigosPermitidos();
        spawnerManager.AtivarSpawn();
        trocandoHorda = false;
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
    private void Final()
    {
        SceneManager.LoadScene("CenaFimDemo");
    }
}
