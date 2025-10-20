using Unity.VisualScripting;
using UnityEngine;

public class Bola : Entregavel
{
    public float velocidade;

    void Update()
    {
        transform.position += Vector3.left * velocidade * Time.deltaTime;
       
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewPos.x < -0.1f)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            FalharEntrega();
            Destroy(gameObject);
        }
    }
}
