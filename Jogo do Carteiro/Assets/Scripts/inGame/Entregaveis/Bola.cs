using Unity.VisualScripting;
using UnityEngine;

public class Bola : Entregavel
{
    public float velocidade;

    void Update()
    {
        transform.position += Vector3.left * velocidade * Time.deltaTime;
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
