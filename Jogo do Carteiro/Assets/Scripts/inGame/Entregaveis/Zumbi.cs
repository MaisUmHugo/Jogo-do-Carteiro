using UnityEngine;
using System.Collections;
public class Zumbi : Entregavel
{
    [Header("Configuração do Zumbi")]
    public float velocidadeCaminhada;
    public float velocidadeCorrida;
    public float velocidadeTrocaLane;
    public float distanciaCorrida;   // quando começa a correr
    public float distanciaColisao;   // quando escorrega e cai no player

    private bool correndo = false;
    private bool caiu = false;
    private bool recebeuEntrega = false;
    private float yTravado;
    private SpriteRenderer sr;
    private Mov jogador;
    public EntregavelPisca entregavelPisca;
    //public Color corNormal = Color.white;
    //public Color corAtivo = Color.red;

    private Animator anim;

   /* public Sprite[] spritesRecebeu;
    public float tempoPorFrame = 0.35f;
    private bool estaRecebendo;
   */
    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            jogador = playerObj.GetComponent<Mov>();

        // começa em uma lane aleatória
        Vector3 pos = transform.position;
        pos.y = LanesController.instance.PosicaoY((LanesController.Linhas)Random.Range(0, 4));
        transform.position = pos;
    }

    private void Update()
    {
        if (jogador == null) return;

        if (caiu)
        {
            // Estado 3: caído - só desliza até sair da tela
            MoverParaEsquerda(velocidadeCaminhada);
        }
        else if (recebeuEntrega)
        {
            // Estado 4: recebeu entrega - vai embora
            MoverParaEsquerda(velocidadeCaminhada);
        }
        else if (!correndo)
        {
            anim.SetBool("Andar", true);

            // Estado 1: caminhando
            transform.position += Vector3.left * velocidadeCaminhada * Time.deltaTime;

            // Se chegou perto o suficiente - começa corrida
            if (Mathf.Abs(transform.position.x - jogador.transform.position.x) <= distanciaCorrida)
            {
                IniciarCorrida();
            }
        }
        else
        {
            // Estado 2: correndo atrás do player
            float yAlvo = LanesController.instance.PosicaoY(jogador.linhaAtual);

            float novoY = Mathf.MoveTowards(
                transform.position.y,
                yAlvo,
                velocidadeTrocaLane * Time.deltaTime
            );

            transform.position = new Vector3(
                transform.position.x - velocidadeCorrida * Time.deltaTime,
                novoY,
                transform.position.z);


            // Se chegou colado no player - escorrega/cai e causa dano
            if (Vector3.Distance(transform.position, jogador.transform.position) <= distanciaColisao)
            {
                CairECausarDano();
            }
        }

        // saiu da tela
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewPos.x < -0.1f)
        {
            Destroy(gameObject);
        }
    }

    private void MoverParaEsquerda(float velocidade)
    {
        transform.position = new Vector3(
            transform.position.x + Vector3.left.x * velocidade * Time.deltaTime,
            yTravado,
            transform.position.z);
    }

    private void IniciarCorrida()
    {
        correndo = true;
        anim.SetBool("Correr", true);
        anim.SetBool("Andar", false);
        ativoParaEntrega = true;
        //anim.SetTrigger("PodeReceber");
        entregavelPisca?.IniciarPiscar();
        Debug.Log("Zumbi começou a correr! Pode entregar agora.");
    }

    private void CairECausarDano()
    {
        if (caiu) return;

        anim.SetBool("Caiu", true);
        caiu = true;
        correndo = false;
        ativoParaEntrega = false; // encerra janela de entrega

        entregavelPisca?.PararPiscar();

        yTravado = transform.position.y;
        


        //sr.color = new Color(corNormal.r, corNormal.g, corNormal.b, 0.5f);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        StartCoroutine(DelayCair()); //causa dano no player
        Debug.Log("Zumbi escorregou e bateu no player!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (correndo && ativoParaEntrega && collision.CompareTag("Caixa"))
        {
            ReceberEntrega();
        }
    }

    public override void ReceberEntrega()
    {
        if (!ativoParaEntrega) return;

        base.ReceberEntrega();
        anim.SetBool("RecebeuEntrega", true);
        ativoParaEntrega = false;
        correndo = false;
        recebeuEntrega = true;
        yTravado = transform.position.y;

        entregavelPisca?.PararPiscar();

        StartCoroutine(DelayTransparente());

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Debug.Log("Zumbi recebeu a entrega e desistiu do player.");
    }

    private IEnumerator DelayTransparente()
    {
        yield return new WaitForSeconds(1.5f);
        anim.SetBool("Transparente", true);
    }
    private IEnumerator DelayCair()
    {
        yield return new WaitForSeconds(1.5f);
        FalharEntrega();//causa dano no player
    }
}
