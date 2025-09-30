using UnityEngine;
using UnityEngine.InputSystem;

public class CaixaTiro : MonoBehaviour
{
    [Header("Configuração do Tiro")]
    public GameObject prefabCaixa; // Prefab da caixa
    public Transform pontoLancamento; // De onde a caixa sai (pode ser a posição do jogador)
    public float velocidadeCaixa;
    public float delayLancamento;

    private INPUTS inputs;
    private Camera cam;
    private Mira mira;
    private float tempoUltimoLancamento;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        inputs = new INPUTS();
        cam = Camera.main;
        mira = FindFirstObjectByType<Mira>();
        if (pontoLancamento == null) pontoLancamento = transform;
    }

    private void OnEnable()
    {
        inputs.Gameplay.Enable();
        inputs.Gameplay.Shoot.performed += Atirar; // ação no Input Actions
    }

    private void OnDisable()
    {
        inputs.Gameplay.Shoot.performed -= Atirar;
        inputs.Gameplay.Disable();
    }

    private void Atirar(InputAction.CallbackContext ctx)
    {
        //Cooldown
        if (Time.time < tempoUltimoLancamento + delayLancamento) return;

        // Garante que temos prefab e mira na cena
        if (prefabCaixa == null) return;

        anim.SetTrigger("Throw");

        // Pega posição da mira
        Vector3 posMira = mira.transform.position;

        // Direção da caixa (da origem até a mira)
        Vector3 direcao = (posMira - pontoLancamento.position).normalized;

        // Instancia a caixa
        GameObject novaCaixa = Instantiate(prefabCaixa, pontoLancamento.position, Quaternion.identity);

        

        // Adiciona movimento
        Rigidbody2D rb = novaCaixa.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direcao * velocidadeCaixa;
        }
        //armazena o tempo do último lançamento
        tempoUltimoLancamento = Time.time;
    }
}
