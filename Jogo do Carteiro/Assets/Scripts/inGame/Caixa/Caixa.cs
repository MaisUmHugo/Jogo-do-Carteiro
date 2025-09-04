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
        if (collision.gameObject.layer == LayerMask.NameToLayer("Entregavel"))
        {
            ScoreManager.instance.AdicionarPontos(100);
            ComboManager.instance.AumentarCombo();
            Debug.Log("Entrega realizada!");
            // dps colocar para chamar o inimigo/cliente para reagir
        }
        else
        {
            Debug.Log("Caixa perdida...");
            ComboManager.instance.ResetarCombo();
        }

        Destroy(gameObject); 
    }
}
