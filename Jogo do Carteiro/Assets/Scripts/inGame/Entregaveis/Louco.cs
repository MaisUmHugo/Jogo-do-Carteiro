using System.Collections;
using UnityEngine;

public class Louco : Entregavel
{
    [Header("Configuração do Louco")]
    public float velocidadeVertical = 5f;   // velocidade no eixo Y
    public float velocidadeFrente = 2f;     // velocidade no eixo X (frente = esquerda)
    public float velocidadeSaida = 3f;      // velocidade depois que atravessou
    public float tempoAtivoEntrega = 1.2f;
    public float intervaloPiscar = 0.15f;

    private SpriteRenderer sr;
    private bool atravessando = true;
    private bool terminouTravessia = false;
    private float yDestino;
    private float yTravado;

    private bool entregaJaAtivada = false;
    private bool entregaRecebida = false;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        // Decide direção: sobe ou desce
        bool vindoDeBaixo = Random.value > 0.5f;

        float yTopo = LanesController.instance.PosicaoY(LanesController.Linhas.L1);
        float yFundo = LanesController.instance.PosicaoY(LanesController.Linhas.L4);

        float offset = 4f; // para sair da "rua"

        yDestino = vindoDeBaixo ? (yTopo + offset) : (yFundo - offset);
    }

    private void Update()
    {
        if (atravessando)
        {
            // --- Movimento diagonal (X negativo + Y até destino) ---
            transform.position += new Vector3(-velocidadeFrente * Time.deltaTime, 0f, 0f);

            float novoY = Mathf.MoveTowards(
                transform.position.y,
                yDestino,
                velocidadeVertical * Time.deltaTime
            );

            transform.position = new Vector3(transform.position.x, novoY, transform.position.z);

            // ---- Ativação da entrega (ignora L1 e L4) ----
            float yL2 = LanesController.instance.PosicaoY(LanesController.Linhas.L2);
            float yL3 = LanesController.instance.PosicaoY(LanesController.Linhas.L3);

            if (!entregaJaAtivada)
            {
                if (Mathf.Abs(transform.position.y - yL2) < 0.1f ||
                    Mathf.Abs(transform.position.y - yL3) < 0.1f)
                {
                    entregaJaAtivada = true;
                    StartCoroutine(JanelaEntrega());
                }
            }

            // ---- Chegou no outro lado (passou todas as lanes) ----
            if ((yDestino > 0 && transform.position.y >= yDestino - 0.05f) ||
                (yDestino < 0 && transform.position.y <= yDestino + 0.05f))
            {
                atravessando = false;
                terminouTravessia = true;

                if (!entregaRecebida)
                {
                    PerderCombo();
                    Debug.Log("Louco caiu sem receber entrega, fazueli");
                }
                else
                {
                    Debug.Log("Louco terminou, yeah");
                }

                yTravado = transform.position.y; // trava Y
            }
        }
        else if (terminouTravessia)
        {
            // Depois da travessia, anda reto em X (esquerda) até sair da tela
            transform.position = new Vector3(
                transform.position.x - velocidadeSaida * Time.deltaTime,
                yTravado,
                transform.position.z
            );

            // Destrói quando sumir da tela
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
        Debug.Log("Louco recebeu a entrega, kilegal");
    }
}
