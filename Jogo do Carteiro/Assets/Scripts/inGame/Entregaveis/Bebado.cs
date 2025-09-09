using System.Collections;
using UnityEngine;

public class Bebado : Entregavel
{
    [Header("Configuração do Bêbado")]
    public float velocidade;
    public float trocaLaneIntervalo;
    public float velocidadeTrocaLane;
    

    [Header("Entrega")]
    public float tempoAtivoEntrega = 3.5f;
    public float intervaloPiscar = 0.15f;

    private Mov jogador;
    private float tempoUltimaTroca;
    private LanesController.Linhas minhaLane;  // lane atual
    private SpriteRenderer sr;

    private Coroutine piscarRoutine;
    public Color corNormal = Color.white;
    public Color corPiscar = Color.red;   

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            jogador = playerObj.GetComponent<Mov>();

        sr = GetComponent<SpriteRenderer>();

        // começa em uma lane aleatória
        int laneIndex = Random.Range(0, 4);
        minhaLane = (LanesController.Linhas)laneIndex;
        Vector3 pos = transform.position;
        pos.y = LanesController.instance.PosicaoY(minhaLane);
        transform.position = pos;

        tempoUltimaTroca = Time.time;
    }

    private void Update()
    {
        if (jogador == null) return;

        transform.position += Vector3.left * velocidade * Time.deltaTime;

        // fica alternando entre as lanes
        if (Time.time >= tempoUltimaTroca + trocaLaneIntervalo)
        {
            TrocarLaneAleatoria();
            tempoUltimaTroca = Time.time;
        }

        // move até a nova lane
        float novoY = Mathf.MoveTowards(
            transform.position.y,
            LanesController.instance.PosicaoY(minhaLane),
            velocidadeTrocaLane * Time.deltaTime
        );
        transform.position = new Vector3(transform.position.x, novoY, transform.position.z);

        // verifica se pode abrir janela de entrega
        if (!ativoParaEntrega && PodeReceberEntrega())
        {
            AtivarEntrega();
        }

        // se saiu da tela perde o combo e falha
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewPos.x < -0.1f)
        {
            FalharEntrega();
            Destroy(gameObject);
        }
    }

    private void TrocarLaneAleatoria()
    {
        int novaLane;
        do
        {
            novaLane = Random.Range(0, 4); // pega qualquer lane
        } while (novaLane == (int)minhaLane); // garante que não repita a mesma

        minhaLane = (LanesController.Linhas)novaLane;
    }
    private IEnumerator JanelaEntrega()
    {
        ativoParaEntrega = true;
        float tempo = 0f;

        while (tempo < tempoAtivoEntrega)
        {
            sr.color = sr.color == corNormal ? corPiscar : corNormal;
            yield return new WaitForSeconds(intervaloPiscar);
            tempo += intervaloPiscar;

            if (!ativoParaEntrega) // caso seja interrompido no meio
            {
                sr.color = corNormal;
                yield break;
            }
        }

        ativoParaEntrega = false;
        sr.color = corNormal;
        piscarRoutine = null;
    }


    private bool PodeReceberEntrega()
    {
        
        bool mesmaLane = Mathf.Abs(jogador.transform.position.y - transform.position.y) < 0.1f;
       
        bool atras = jogador.transform.position.x < transform.position.x;

        return mesmaLane && atras;
    }


    public override void ReceberEntrega()
    {
        if (!ativoParaEntrega) return;

        if (PodeReceberEntrega())
        {
            ativoParaEntrega = false;
            sr.color = corNormal;

            if (piscarRoutine != null)
            {
                StopCoroutine(piscarRoutine);
                piscarRoutine = null;
            }

            base.ReceberEntrega();
            Debug.Log("Parabéns! Entregou.");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("ENTREGA DE LADO JUMENTO! ...entrega de lado, frente não dá.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // dano direto se colidir com o jogador
            FalharEntrega();
            Destroy(gameObject);
        }
    }

    public void AtivarEntrega()
    {
        if (piscarRoutine != null)
            StopCoroutine(piscarRoutine);

        piscarRoutine = StartCoroutine(JanelaEntrega());
    }
}
