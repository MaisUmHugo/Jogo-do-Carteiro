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
        // Dps adicionar tags (ex: "Inimigo", "Entregavel")
        // if (collision.gameObject.CompareTag("Inimigo")) 

        Destroy(gameObject); 
    }
}
