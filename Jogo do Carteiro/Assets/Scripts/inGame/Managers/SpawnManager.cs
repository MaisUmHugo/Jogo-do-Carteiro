using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    public static SpawnerManager instance;

    public enum TipoSpawn
    {
        EmLane,
        Livre,
        Fixo
    }

    [System.Serializable]
    public class ConfiguracaoSpawn
    {
        public string nome;
        public GameObject prefab;
        public string tagAssociada;
        public TipoSpawn tipoSpawn = TipoSpawn.EmLane;
        public Transform[] pontosFixos; // usado se tipo = Fixo
    }

    [Header("Configurações de Spawn")]
    public List<ConfiguracaoSpawn> configuracoes = new List<ConfiguracaoSpawn>();
    private float posicaoForaCameraX; // agora só a variável

    [Header("Controle Dinâmico")]
    public float intervaloSpawn;
    public float multiplicadorVelocidade;
    public bool spawnAtivo = true;

    private Dictionary<string, ConfiguracaoSpawn> dicionarioConfig = new Dictionary<string, ConfiguracaoSpawn>();
    private float proximoSpawn;

    private float intervaloPadrao;
    private float velocidadePadrao;

    private List<string> tagsPermitidas = new List<string>();

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        posicaoForaCameraX = Camera.main.ViewportToWorldPoint(new Vector3(1.2f, 0, 0)).x;

        foreach (var c in configuracoes)
        {
            if (!dicionarioConfig.ContainsKey(c.tagAssociada))
                dicionarioConfig.Add(c.tagAssociada, c);
        }
        intervaloPadrao = intervaloSpawn;
        velocidadePadrao = multiplicadorVelocidade;
    }

    void Update()
    {
        if (!spawnAtivo) return;

        if (Time.time >= proximoSpawn)
        {
            List<ConfiguracaoSpawn> candidatos = new List<ConfiguracaoSpawn>();

            foreach (var config in configuracoes)
            {
                if (PodeSpawnar(config.tagAssociada))
                    candidatos.Add(config);
            }
            if (candidatos.Count > 0)
            {
                int idx = Random.Range(0, configuracoes.Count);
                SpawnPorTag(candidatos[idx].tagAssociada);
            }
            proximoSpawn = Time.time + intervaloSpawn;
        }
    }

    public void AtivarSpawn() => spawnAtivo = true;
    public void DesativarSpawn() => spawnAtivo = false;

    public void DefinirIntervalo(float intervalo) => intervaloSpawn = intervalo;
    public void DefinirVelocidade(float mult) => multiplicadorVelocidade = mult;

    public void ResetarConfig()
    {
        intervaloSpawn = intervaloPadrao;
        multiplicadorVelocidade = velocidadePadrao;
        spawnAtivo = true;
        tagsPermitidas.Clear();
    }

    public void DefinirTagsPermitidas(List<string> tags)
    {
        tagsPermitidas = tags;
    }

    public bool PodeSpawnar(string tag)
    {
        return tagsPermitidas.Count == 0 || tagsPermitidas.Contains(tag);
    }
    public GameObject SpawnPorTag(string tag)
    {
        if (!dicionarioConfig.ContainsKey(tag))
        {
            Debug.LogWarning($"[SpawnerManager] Nenhum prefab configurado para tag: {tag}");
            return null;
        }

        if (!PodeSpawnar(tag))
        {
            Debug.Log($"[SpawnerManager] Spawn de {tag} desabilitado nesta horda.");
            return null;
        }

        ConfiguracaoSpawn config = dicionarioConfig[tag];
        Vector3 posicaoSpawn = Vector3.zero;

        switch (config.tipoSpawn)
        {
            case TipoSpawn.EmLane:
                int idx = Random.Range(0, LanesController.instance.linhas.Length);
                float y = LanesController.instance.PosicaoY((LanesController.Linhas)idx);
                posicaoSpawn = new Vector3(posicaoForaCameraX, y, 0f);
                break;

            case TipoSpawn.Livre:
                posicaoSpawn = new Vector3(
                    posicaoForaCameraX,
                    Random.Range(-3f, 3f),
                    0f);  
                    break;

            case TipoSpawn.Fixo:
                if (config.pontosFixos != null && config.pontosFixos.Length > 0)
                {
                    int idxFixo = Random.Range(0, config.pontosFixos.Length);
                    posicaoSpawn = config.pontosFixos[idxFixo].position;
                }
                else
                {
                    Debug.LogWarning($"Nenhum ponto fixo configurado para {tag}");
                    return null;
                }
                break;
        }

        GameObject go = Instantiate(config.prefab, posicaoSpawn, Quaternion.identity);
        AjustarVelocidade(go);

        return go;
    }

    public IEnumerator SpawnMultiplo(string tag, int quantidade, float intervalo)
    {
        for (int i = 0; i < quantidade; i++)
        {
            SpawnPorTag(tag);
            yield return new WaitForSeconds(intervalo);
        }
    }

    private void AjustarVelocidade(GameObject inimigo)
    {
        var componentes = inimigo.GetComponents<MonoBehaviour>();
        foreach (var c in componentes)
        {
            var campo = c.GetType().GetField("velocidade");
            if (campo != null && campo.FieldType == typeof(float))
            {
                float vOriginal = (float)campo.GetValue(c);
                campo.SetValue(c, vOriginal * multiplicadorVelocidade);
            }
        }
    }
}
