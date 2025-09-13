using System.Collections;
using UnityEngine;

public class Louco : Entregavel
{
    [Header("Configuração do Louco")]
    public float velocidadeVertical = 5f;
    public float velocidadeHorizontal = 3f;
    public float tempoAtivoEntrega = 1.2f;
    public float intervaloPiscar = 0.15f;

    private SpriteRenderer sr;
    private bool atravessando = true;
    private bool terminouTravessia = false;
    private float yDestino;
    private float yTravado;

    private bool entregaJaAtivada = false; // evita ativar 2x
    private bool entregaRecebida = false;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        // Decide direção: sobe ou desce
        bool vindoDeBaixo = Random.value > 0.5f;

        float yTopo = LanesController.instance.PosicaoY(LanesController.Linhas.L1);
        float yFundo = LanesController.instance.PosicaoY(LanesController.Linhas.L4);

        float offset = 4f; // quanto além da lane ele vai parar (fora da rua)

        // Define destino acima ou abaixo das lanes
        yDestino = vindoDeBaixo ? (yTopo + offset) : (yFundo - offset);
    }


    private void Update()
    {
        if (atravessando)
        {
            // Move no eixo Y até o outro lado
            transform.position = Vector3.MoveTowards(
                transform.position,
                new Vector3(transform.position.x, yDestino, transform.position.z),
                velocidadeVertical * Time.deltaTime
            );

            // ---- Ativação da entrega (ignora a primeira lane extrema) ----
            float yL2 = LanesController.instance.PosicaoY(LanesController.Linhas.L2);
            float yL3 = LanesController.instance.PosicaoY(LanesController.Linhas.L3);

            if (!entregaJaAtivada)
            {
                // só ativa se passar em L2 ou L3
                if (Mathf.Abs(transform.position.y - yL2) < 0.1f ||
                    Mathf.Abs(transform.position.y - yL3) < 0.1f)
                {
                    entregaJaAtivada = true;
                    StartCoroutine(JanelaEntrega());
                }
            }

            // ---- Chegou no outro lado (após cruzar TODAS lanes) ----
            if ((yDestino > 0 && transform.position.y >= yDestino - 0.05f) ||
                (yDestino < 0 && transform.position.y <= yDestino + 0.05f))
            {
                atravessando = false;
                terminouTravessia = true;

                if (!entregaRecebida)
                {
                    PerderCombo();
                    Debug.Log("Louco caiu de cara no chão!");
                }
                else
                {
                    Debug.Log("Louco terminou feliz no outro lado!");
                }

                yTravado = transform.position.y; // trava Y só aqui
            }
        }
        else if (terminouTravessia)
        {
            // Depois da travessia, anda no X até sair da tela
            transform.position = new Vector3(
                transform.position.x + Vector3.left.x * velocidadeHorizontal * Time.deltaTime,
                yTravado,
                transform.position.z
            );

            // Quando sair da câmera → destruir
            Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
            if (viewPos.x < -0.1f) Destroy(gameObject);
        }
    }


    private IEnumerator JanelaEntrega()
    {
        ativoParaEntrega = true;
        float tempo = 0f;

        while (tempo < tempoAtivoEntrega)
        {
            sr.color = sr.color == Color.white ? Color.red : Color.white;
            yield return new WaitForSeconds(intervaloPiscar);
            tempo += intervaloPiscar;
        }

        ativoParaEntrega = false;
        sr.color = Color.white;
    }

    public override void ReceberEntrega()
    {
        if (!ativoParaEntrega) return;

        ativoParaEntrega = false;
        entregaRecebida = true;
        sr.color = Color.white;

        base.ReceberEntrega();
        Debug.Log("📦 Louco recebeu a entrega!");
    }
}
