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
    public float distanciaEntrega = 50f; // distancia minima pra pode entrega

    private Mov jogador;
    private float tempoUltimaTroca;
    private LanesController.Linhas minhaLane;  // lane atual
    private SpriteRenderer sr;

    private Coroutine piscarRoutine;
    public Color corNormal = Color.white;
    public Color corPiscar = Color.red;
    //private bool jaPiscou = false;

    // Travar o y do bebado quando recebe a entrega,
    // para dar a sensação que ele parou quando recebe a entrega
    private float yTravado;

    private Animator anim;


    private bool parado = false; // quando recebe entrega para de mexe

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            jogador = playerObj.GetComponent<Mov>();

        sr = GetComponent<SpriteRenderer>();
        //sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();

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

        if (!parado)
        {
            // movimento normal (anda no X e troca de lane)
            transform.position += Vector3.left * velocidade * Time.deltaTime;

            if (Time.time >= tempoUltimaTroca + trocaLaneIntervalo)
            {
                TrocarLaneAleatoria();
                tempoUltimaTroca = Time.time;
            }

            float novoY = Mathf.MoveTowards(
                transform.position.y,
                LanesController.instance.PosicaoY(minhaLane),
                velocidadeTrocaLane * Time.deltaTime
            );

            transform.position = new Vector3(transform.position.x, novoY, transform.position.z);

            // verifica se pode receber entrega (só pisca uma vez)
            if (!ativoParaEntrega)
            {
                if (PodeReceberEntrega())
                {
                    ativoParaEntrega = true;
                    //jaPiscou = true;
                    piscarRoutine = StartCoroutine(PiscarEnquantoAtivo());
                }
            }
            else if (ativoParaEntrega && !PodeReceberEntrega())
            {
                ativoParaEntrega = false;

                if (piscarRoutine != null)
                {
                    StopCoroutine(piscarRoutine);
                    piscarRoutine = null;
                }

                sr.color = corNormal;
            }
        }
        else
        {
            // quando parado, só anda no eixo X, Y fica travado
            transform.position = new Vector3(
                transform.position.x + Vector3.left.x * velocidade * Time.deltaTime,
                yTravado,
                transform.position.z
            );
        }

        // saiu da tela
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewPos.x < -0.1f)
        {
            if (!parado) // saiu sem receber -> falha
            {
                Debug.Log("Saiu da tela sem entrega, sobrou nada para o beta");
                PerderCombo();
            }
            else
            {
                Debug.Log("Saiu da tela após entrega, farmou aura");
            }

            Destroy(gameObject);
        }
    }


    private void TrocarLaneAleatoria()
    {
        int novaLane;
        do
        {
            novaLane = Random.Range(0, 4);
        } while (novaLane == (int)minhaLane);

        minhaLane = (LanesController.Linhas)novaLane;
    }

    private bool PodeReceberEntrega()
    {
        anim.SetTrigger("PodeReceber");
        // precisa estar em outra lane
        bool outraLane = jogador.linhaAtual != minhaLane;

        // precisa ta perto o suficiente
        float distancia = Vector3.Distance(transform.position, jogador.transform.position);
        bool perto = distancia <= distanciaEntrega;

        /*if (outraLane && perto)
            Debug.Log($"Pode receber entrega! Distância: {distancia}"); */

        return outraLane && perto;
    }


    public override void ReceberEntrega()
    {
        if (!ativoParaEntrega) return;

       

        if (PodeReceberEntrega())
        {
            anim.SetTrigger("RecebeuEntrega");
            base.ReceberEntrega();
            ativoParaEntrega = false;
            parado = true; // para de andar
            yTravado = transform.position.y; 

            if (piscarRoutine != null)
            {
                StopCoroutine(piscarRoutine);
                sr.color = corNormal;
            }
            Color cor = sr.color;
            cor.a = 0.5f; // meio transparente
            sr.color = cor;

            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = false; // desliga colisão
            }

            Debug.Log("Pabéns, se entregou fih");

            StartCoroutine(DelayTransparente());

            // Agora ele vai sumir depois de um tempo

            //Destroy(gameObject);
        }
        else
        {
            Debug.Log("Tem que ser de lado JUMENTO!");
        }
    }

    private IEnumerator DelayTransparente()
    {
        yield return new WaitForSeconds(1.5f);
        anim.SetTrigger("Transparente");
    }
    private IEnumerator PiscarEnquantoAtivo()
    {
        int quantidadePiscos = 5; // muda aqui se quiser mais ou menos piscadas
        for (int i = 0; i < quantidadePiscos; i++)
        {
            sr.color = corPiscar;
            yield return new WaitForSeconds(intervaloPiscar);
            sr.color = corNormal;
            yield return new WaitForSeconds(intervaloPiscar);
        }

        sr.color = corNormal; // garante cor normal no final
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            FalharEntrega();
        }
        else if (collision.CompareTag("Caixa"))
        {
            Vector3 dir = collision.transform.position - transform.position;

            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                ReceberEntrega();
            }
            else
            {
                // lado errado → cancela entrega
                Debug.Log("JOGA NO LADO JUMENTO!");
                //ComboManager.instance.ResetarCombo();
                Destroy(collision.gameObject); // cancela antes da Caixa entregar
            }
        }
    }
}
