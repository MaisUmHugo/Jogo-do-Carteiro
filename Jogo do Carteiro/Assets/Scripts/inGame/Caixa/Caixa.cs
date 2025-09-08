using UnityEngine;

public class Caixa : MonoBehaviour
{
    [Header("Configuração")]
    public float tempoMaximo = 5f; // some depois de 5s se não colidir

    private void Start()
    {
        Destroy(gameObject, tempoMaximo);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Entregavel>(out var entregavel))
        {
            entregavel.ReceberEntrega();
        }
        else
        {
            Debug.Log("Caixa perdida...");
            ComboManager.instance.ResetarCombo();
        }

        Destroy(gameObject);
    }
}
