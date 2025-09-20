using UnityEngine;

public class Pessoa_janela : Entregavel
{
    [Header("Configuração da Janela")]
    public float velocidade;
    public float tempoAtivoEntrega;
    public float intervaloPiscar;
    public float distanciaEntrega;
    public Vector3 offset;

    private SpriteRenderer sr;
    public Color corNormal = Color.blue; // cor padrão
    public Color corAtivo = Color.red;    // cor quando está ativo para receber entrega
    private bool coroutineIniciada = false;
    private Mov jogador;
    private bool recebeu, podereceber;
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
        }
        else
        {
            Debug.LogWarning("Player não encontrado! Verifique se o objeto do jogador tem a tag 'Player'.");
        }
        Vector3 pos = transform.position;
        pos.y = LanesController.instance.PosicaoY((LanesController.Linhas.L1));
        transform.position = pos + offset;
    }
    private void Update()
    {
        if (recebeu)
        {
            transform.position += Vector3.left * velocidade * Time.deltaTime;
            return;
        }
        Vector3 direcao = (jogador.transform.position - transform.position).normalized;
        transform.position += new Vector3(direcao.x, 0, 0) * velocidade * Time.deltaTime;

        if (!coroutineIniciada && Vector3.Distance(transform.position, jogador.transform.position) <= distanciaEntrega)
        {
            coroutineIniciada = true;
            StartCoroutine(ProntoparaEntrega());
        }
        // saiu da tela
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewPos.x < -0.1f)
        {
            if (!recebeu) // saiu sem receber -> falha
            {
                PerderCombo();
                Debug.Log($"{gameObject.name} saiu da tela sem entrega!");
            }
            else
            {
                Debug.Log($"{gameObject.name} saiu da tela após entrega.");
            }

            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Caixa") && podereceber)
        {
            ReceberEntrega();
        }
    }
    public override void ReceberEntrega()
    {
        sr.color = corNormal;
        base.ReceberEntrega();
        Color cor = sr.color;
        cor.a = 0.5f; // meio transparente
        sr.color = cor;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false; // desliga colisão
        }
        recebeu = true;
    }
    private System.Collections.IEnumerator ProntoparaEntrega()
    {
        podereceber = true;
        ativoParaEntrega = true;
        sr.color = corAtivo; // piscar (feedback visual)
        Debug.Log("Janela proxima, entregue agora!");

        // espera a janela de tempo para aceitar a entrega
        yield return new WaitForSeconds(tempoAtivoEntrega);

        if (ativoParaEntrega && !recebeu)
        {
            // não recebeu a entrega → falha
            PerderCombo();
            sr.color = corNormal;
            podereceber = false;
        }
    }
}
