using UnityEngine;

public class LinhasController : MonoBehaviour
{
    public enum Linhas
    {
        L1, L2, L3, L4
    }

    [Header("Linhas")]
    public Transform[] linhas; // array de linhas no inspector

    public static LinhasController instance { get; private set; }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public float PosicaoX(Linhas linha)
    {
        return linhas[(int)linha].position.x;
    }

    public float PosicaoY(Linhas linha)
    {
        return linhas[(int)linha].position.y;
    }

    // Atalho para pegar direto a posição completa da linha
    public Vector3 Posicao(Linhas linha)
    {
        return linhas[(int)linha].position;
    }
}
