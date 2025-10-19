using UnityEngine;
using System.Collections;
using System.Reflection;

public class Malabarista : Entregavel
{
    public GameObject bola;
    [Header("Configuração do malabarista")]
    public float velocidade;
    public float tempoAtivoEntrega;
    public float intervaloPiscar;
    public float distanciaEntrega;
    public float IntervaloTiro;
    public float DistanciaTiro;

    // Sprite renderer para fazer o efeito de piscar
    private SpriteRenderer sr;
    public Color corNormal = Color.white; // cor padrão
    public Color corAtivo = Color.red;    // cor quando está ativo para receber entrega
    private bool coroutineIniciada = false;
    private Mov jogador;
    private bool recebeu, podereceber;
    private bool podeatirar;
    private bool ComecouCoroutineAtirar;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        //sr = GetComponentInChildren<SpriteRenderer>();
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

        if (!ComecouCoroutineAtirar && Mathf.Abs(transform.position.x - jogador.transform.position.x) <= DistanciaTiro)
        {
            podeatirar = true;
            ComecouCoroutineAtirar = true;
            StartCoroutine(atirar());
        }

            // Se está em range de entrega, pode esperar pela caixa
            if (!coroutineIniciada && Mathf.Abs(transform.position.x - jogador.transform.position.x) <= distanciaEntrega)
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
        if (collision.CompareTag("Player"))
        {
            FalharEntrega();
        }
    }
    public override void ReceberEntrega()
    {
        base.ReceberEntrega();
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false; // desliga colisão
        }
        recebeu = true;
        StartCoroutine(DelayTransparente());

    }
    private IEnumerator ProntoparaEntrega()
    {
        yield return new WaitForSeconds(1.5f);
        podereceber = true;
        ativoParaEntrega = true;
        Debug.Log("Malabarista proximo, entregue agora!");

        // espera a janela de tempo para aceitar a entrega
        yield return new WaitForSeconds(tempoAtivoEntrega);

        if (ativoParaEntrega && !recebeu)
        {
            // não recebeu a entrega → falha
            PerderCombo();
            sr.color = corNormal;
        }
    }
    private IEnumerator DelayTransparente()
    {
        yield return new WaitForSeconds(3.5f);
    }
    private IEnumerator atirar()
    {
        if (podeatirar)
        {
            Debug.Log("Tentou Atirar");
            Vector3 posSpawn = transform.position + transform.right * -0.5f;
            Instantiate(bola, posSpawn, Quaternion.identity);
            podeatirar = false;
            yield return new WaitForSeconds(IntervaloTiro);
            podeatirar = true;
            StartCoroutine(atirar());
        }
    }
}
