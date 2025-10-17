using Unity.VisualScripting;
using UnityEngine;

public class Bola : MonoBehaviour
{
    public float velocidade;

    void Update()
    {
        transform.position += Vector3.left * velocidade * Time.deltaTime;
    }
}
