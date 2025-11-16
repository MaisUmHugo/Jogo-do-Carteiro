using UnityEngine;

public class Bola : MonoBehaviour
{
    [Header("Arco")]
    public float tempoDoArco = 0.5f;  // tempo total até cair
    public float alturaExtra = 1.5f;  // quanto pode ultrapassar verticalmente/altura

    private Vector3 pontoInicial;
    private Vector3 pontoFinal;
    private float tempoAtual;

    private float alturaPico;   // altura máxima do arco
    private bool caiuLane;

    [Header("Velocidade")]
    public float velocidadeFinal = 7f; // movimento na lane

    public void IniciarArco(Vector3 destino)
    {
        pontoInicial = transform.position;
        pontoFinal = destino;

        // A altura máxima será entre o inicial e final  extra
        alturaPico = Mathf.Max(pontoInicial.y, pontoFinal.y) + alturaExtra;

        tempoAtual = 0f;
        caiuLane = false;
    }

    void Update()
    {
        if (!caiuLane)
        {
            tempoAtual += Time.deltaTime / tempoDoArco;
            float t = Mathf.Clamp01(tempoAtual);

            // acerta horizontalmente
            float x = Mathf.Lerp(pontoInicial.x, pontoFinal.x, t);

            // acerta verticalmente (parábola)
            float y = Mathf.Lerp(pontoInicial.y, pontoFinal.y, t)
                   + AlturaParabolica(t);

            transform.position = new Vector3(x, y, 0);

            if (t >= 1f)
                caiuLane = true;

            return;
        }

        // Movimento reto depois de cair
        transform.position += Vector3.left * velocidadeFinal * Time.deltaTime;

        // Sai da tela - vai destruir
        Vector3 view = Camera.main.WorldToViewportPoint(transform.position);
        if (view.x < -0.1f)
            Destroy(gameObject);
    }

    // Faz a curva sem depender de distância
    float AlturaParabolica(float t)
    {
        // curva bonitinha 
        return 4 * alturaExtra * t * (1 - t);
    }
}
