using UnityEngine;
using UnityEngine.InputSystem;

public class CaixaTiro : MonoBehaviour
{
    [Header("Configuração do Tiro")]
    public GameObject prefabCaixa; // Prefab da caixa
    public Transform pontoDisparo; // De onde a caixa sai (pode ser a posição do jogador)
    public float velocidadeCaixa;

    private INPUTS inputs;
    private Camera cam;

    private void Awake()
    {
        inputs = new INPUTS();
        cam = Camera.main;
    }

    private void OnEnable()
    {
        inputs.Gameplay.Enable();
        inputs.Gameplay.Shoot.performed += Atirar; // "Shoot" = ação no Input Actions
    }

    private void OnDisable()
    {
        inputs.Gameplay.Shoot.performed -= Atirar;
        inputs.Gameplay.Disable();
    }

    private void Atirar(InputAction.CallbackContext ctx)
    {
        // Garante que temos prefab e mira na cena
        if (prefabCaixa == null) return;

        // Pega posição da mira
        Vector3 posMira = Object.FindFirstObjectByType<Mira>().transform.position;

        // Direção da caixa (da origem até a mira)
        Vector3 direcao = (posMira - pontoDisparo.position).normalized;

        // Instancia a caixa
        GameObject novaCaixa = Instantiate(prefabCaixa, pontoDisparo.position, Quaternion.identity);

        // Adiciona movimento
        Rigidbody2D rb = novaCaixa.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direcao * velocidadeCaixa;
        }
    }
}
