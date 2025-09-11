using UnityEngine;

public class Mao_Zumbi : Entregavel
{
    [Header("Configuração da mão")]
    public float velocidade;
    public float tempoAtivoEntrega;
    public float intervaloPiscar;
    public float distanciaEntrega;

    private Transform alvo; // referência ao jogador (pegamos pela tag)

    // Sprite renderer para fazer o efeito de piscar
    private SpriteRenderer sr;
    public Color corNormal = Color.white; // cor padrão
    public Color corAtivo = Color.red;    // cor quando está ativo para receber entrega
    private bool coroutineIniciada = false;
    private Mov jogador;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            jogador = playerObj.GetComponent<Mov>();
            Debug.Log("Jogador encontrado");
        }
        else
        {
            Debug.LogWarning("Player não encontrado! Verifique se o objeto do jogador tem a tag 'Player'.");
        }
        Vector3 pos = transform.position;
        pos.y = LanesController.instance.PosicaoY((LanesController.Linhas)Random.Range(0, 4));
        transform.position = pos;
    }
    private void Update()
    {
        Vector3 direcao = (jogador.transform.position - transform.position).normalized;
        transform.position += new Vector3(direcao.x, 0, 0) * velocidade * Time.deltaTime;
        if (!coroutineIniciada && Vector3.Distance(transform.position, jogador.transform.position) <= distanciaEntrega)
        {
            coroutineIniciada = true;
            StartCoroutine(ProntoparaEntrega());
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Caixa"))
        {
            ReceberEntrega();
        }
    }
    public override void ReceberEntrega()
    {
        sr.color = corNormal;
        base.ReceberEntrega();
        Destroy(gameObject);
    }
    private System.Collections.IEnumerator ProntoparaEntrega()
    {
        ativoParaEntrega = true;
        sr.color = corAtivo; // piscar (feedback visual)
        Debug.Log("Mão proxima, entregue agora!");

        // espera a janela de tempo para aceitar a entrega
        yield return new WaitForSeconds(tempoAtivoEntrega);

        if (ativoParaEntrega)
        {
            // não recebeu a entrega → falha
            FalharEntrega();
            sr.color = corNormal;
            Destroy(gameObject);
        }
    }
}
