using UnityEngine;

public class Caixa : MonoBehaviour
{
    [Header("Configuração")]
    public float tempoMaximo = 5f; // some depois de 5s se não colidir

    private void Start()
    {
        Destroy(gameObject, tempoMaximo);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // não interage com o player
        if (collision.CompareTag("Player"))
            return;

        if (collision.TryGetComponent<Entregavel>(out var entregavel))
        {
            entregavel.ReceberEntrega();
        }
        else
        {
            Debug.Log(" Caixa perdida...");
            ComboManager.instance.ResetarCombo();
        }

        Destroy(gameObject);
    }
}
