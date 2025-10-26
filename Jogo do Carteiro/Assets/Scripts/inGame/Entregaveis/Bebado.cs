using System.Collections;
using UnityEngine;

public class Bebado : Entregavel
{
    public Transform Exclamacao;
    [Header("Configuração do Bêbado")]
    public float velocidade;
    public float trocaLaneIntervalo;
    public float velocidadeTrocaLane;

    [Header("Entrega")]
    public float tempoAtivoEntrega;
    public float intervaloPiscar;
    public float distanciaEntrega; // distancia minima pra pode entrega
    public float tempoexclamacao;

    private Mov jogador;
    private float tempoUltimaTroca;
    private LanesController.Linhas minhaLane;  // lane atual
    private SpriteRenderer sr;

    //private bool jaPiscou = false;

    // Travar o y do bebado quando recebe a entrega,
    // para dar a sensação que ele parou quando recebe a entrega
    private float yTravado;

    private Animator anim;
    public EntregavelPisca entregavelPisca;
    public PontuacaoPopup popupPontuacao;

    private bool parado = false; // quando recebe entrega para de mexe

    private void Start()
    {
        if (Exclamacao == null) Exclamacao = transform;
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
                    entregavelPisca?.PiscarAtivo();
                    StartCoroutine(exclamacao());
                }
            }
            else if (ativoParaEntrega && !PodeReceberEntrega())
            {
                ativoParaEntrega = false;
                entregavelPisca.PararPiscar();
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
        // precisa estar em outra lane
        bool outraLane = jogador.linhaAtual != minhaLane;

        // precisa ta perto o suficiente
        float distancia = Mathf.Abs(transform.position.x - jogador.transform.position.x);
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
            // Calcula pontuação com bônus
            int multiplicador = ComboManager.instance.GetMultiplicador();
            int total = 100 * multiplicador;

            popupPontuacao?.MostrarPontuacao(total);
            entregavelPisca?.PiscarRecebendo();
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = false; // desliga colisão
            }

            Debug.Log("Pabéns, se entregou fih");

            StartCoroutine(PararPiscar());
            // Agora ele vai sumir depois de um tempo

            //Destroy(gameObject);
        }
        else
        {
            Debug.Log("Tem que ser de lado JUMENTO!");
        }
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

    private IEnumerator PararPiscar()
    {
        yield return new WaitForSeconds(1.5f);
        entregavelPisca?.PararPiscar();
    }
    private IEnumerator exclamacao()
    {
        yield return new WaitForSeconds(0.1f);
        //exclamacao
        GameObject prefab = Resources.Load<GameObject>("PontoExclamacao");
        if (prefab != null)
        {
            Debug.Log("prefab não é nulo");
            GameObject instancia = Instantiate(prefab, Exclamacao.position, Quaternion.identity);
            instancia.transform.SetParent(gameObject.transform, worldPositionStays: true);
            float tempo = 0;
            while (tempo < tempoexclamacao)
            {
                tempo += Time.deltaTime;
                yield return null;
            }
            Destroy(instancia);
            tempo = 0;
        }
    }
}
