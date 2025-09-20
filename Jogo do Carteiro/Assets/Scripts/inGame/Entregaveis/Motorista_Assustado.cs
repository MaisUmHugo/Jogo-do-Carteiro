using UnityEngine;

public class Motorista_Assustado : Entregavel
{
    [Header("Configuração da mão")]
    public float velocidade;
    public float tempoAtivoEntrega;
    public float intervaloPiscar;
    public float distanciaEntrega;

    private SpriteRenderer sr;
    public Color corNormal = Color.white; // cor padrão
    public Color corAtivo = Color.red;    // cor quando está ativo para receber entrega
    private bool coroutineIniciada = false;
    private Mov jogador;
    private bool recebeu, podereceber;

    private Animator anim;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        //sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
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
        pos.y = LanesController.instance.PosicaoY((LanesController.Linhas)Random.Range(0, 4));
        transform.position = pos;
    }
    private void Update()
    {
        if (recebeu)
        {
            // Já entregou → apenas vai embora para a esquerda
            transform.position += Vector3.left * velocidade * Time.deltaTime;
            return;
        }

        // Se está em range de entrega, pode esperar pela caixa
        if (!coroutineIniciada && Vector3.Distance(transform.position, jogador.transform.position) <= distanciaEntrega)
        {
            coroutineIniciada = true;
            StartCoroutine(ProntoparaEntrega());
        }

        // --- MOVIMENTO ---
        if (ativoParaEntrega && !recebeu)
        {
            // Continua andando para a esquerda mesmo que esteja esperando entrega
            transform.position += Vector3.left * velocidade * Time.deltaTime;
        }
        else
        {
            // Se ainda não está em range → segue em direção ao jogador
            Vector3 direcao = (jogador.transform.position - transform.position).normalized;
            transform.position += new Vector3(direcao.x, 0, 0) * velocidade * Time.deltaTime;
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
        anim.SetTrigger("ReceberEntrega");


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
        anim.SetTrigger("PodeEntregar");
       sr.color = corAtivo; // piscar (feedback visual)
        Debug.Log("MAssustado proximo, entregue agora!");

        // espera a janela de tempo para aceitar a entrega
        yield return new WaitForSeconds(tempoAtivoEntrega);

        if (ativoParaEntrega && !recebeu)
        {
            // não recebeu a entrega → falha
            PerderCombo();
            sr.color = corNormal;
        }
    }
}
