using UnityEngine;

public class Zumbi : Entregavel
{
    [Header("Configuração do Zumbi")]
    public float velocidade; 
    public float velocidadeTrocaLane; 
    public float distanciaAtaque; 
    public float tempoAtivoEntrega; 

    private Transform alvo; // referência ao jogador (pegamos pela tag)
    private bool atacando = false; // flag para controlar se já está atacando

    // Sprite renderer para fazer o efeito de piscar
    private SpriteRenderer sr;
    public Color corNormal = Color.white; // cor padrão
    public Color corAtivo = Color.red;    // cor quando está ativo para receber entrega

    private Mov jogador;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // procura o objeto que tem a tag "Player"
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            jogador = playerObj.GetComponent<Mov>();
        }
        else
        {
            Debug.LogWarning("Player não encontrado! Verifique se o objeto do jogador tem a tag 'Player'.");
        }

        // trava o zumbi em uma lane inicial aleatória
        Vector3 pos = transform.position;
        pos.y = LanesController.instance.PosicaoY((LanesController.Linhas)Random.Range(0, 4));
        transform.position = pos;
    }

    private void Update()
    {
        if (jogador == null || atacando) return;

        
        // Zumbi anda na direção do player apenas no eixo X
        Vector3 direcao = (jogador.transform.position - transform.position).normalized;
        transform.position += new Vector3(direcao.x, 0, 0) * velocidade * Time.deltaTime;

        
        // Faz o zumbi perseguir a lane do jogador
        Vector3 posAlvoLane = LanesController.instance.Posicao(jogador.linhaAtual);

        float novoY = Mathf.MoveTowards(
            transform.position.y,
            posAlvoLane.y,
            velocidadeTrocaLane * Time.deltaTime 
        );

        transform.position = new Vector3(transform.position.x, novoY, transform.position.z);

        // Verifica se chegou na distância de ataque
        if (Vector3.Distance(transform.position, jogador.transform.position) <= distanciaAtaque)
        {
            StartCoroutine(Ataque());
        }
    }

    private System.Collections.IEnumerator Ataque()
    {
        atacando = true;
        ativoParaEntrega = true;
        sr.color = corAtivo; // piscar (feedback visual)
        Debug.Log("Zumbi atacando, entregue agora!");

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

    public override void ReceberEntrega()
    {
        sr.color = corNormal;
        base.ReceberEntrega();
        // Lógica de reação específica do zumbi pode ser colocada aqui (animação, som, etc.)
        Destroy(gameObject);
    }
}
