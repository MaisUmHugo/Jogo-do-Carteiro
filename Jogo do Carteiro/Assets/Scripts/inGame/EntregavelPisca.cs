using UnityEngine;
using System.Collections;

public class EntregavelPisca : MonoBehaviour
{
    private SpriteRenderer sr;
    private Coroutine rotina;

    [Header("Configurações - Piscar Ativo (pode receber)")]
    public float intervaloAtivo = 0.3f;
    public int quantidadePiscadasAtivo = 3;
    public Color corPiscarAtivo = new Color(1f, 1f, 1f, 0.6f);

    [Header("Configurações - Piscar Recebendo")]
    public float intervaloRecebendo = 0.1f;
    public int quantidadePiscadasRecebendo = 2;
    public Color corPiscarRecebendo = new Color(1f, 1f, 1f, 0.3f);

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    public void PiscarAtivo()
    {
        if (rotina != null) StopCoroutine(rotina);
        rotina = StartCoroutine(Piscar(corPiscarAtivo, intervaloAtivo, quantidadePiscadasAtivo));
    }

    public void PiscarRecebendo()
    {
        if (rotina != null) StopCoroutine(rotina);
        rotina = StartCoroutine(Piscar(corPiscarRecebendo, intervaloRecebendo, quantidadePiscadasRecebendo));
    }

    private IEnumerator Piscar(Color corPiscar, float intervalo, int quantidade)
    {
        if (sr == null) yield break;
        Color corOriginal = sr.material.color;

        for (int i = 0; i < quantidade; i++)
        {
            sr.material.color = corPiscar;
            yield return new WaitForSeconds(intervalo);
            sr.material.color = corOriginal;
            yield return new WaitForSeconds(intervalo);
        }

        sr.material.color = corOriginal;
        rotina = null;
    }

    public void PararPiscar()
    {
        if (rotina != null)
        {
            StopCoroutine(rotina);
            rotina = null;
        }

        if (sr != null)
            sr.material.color = Color.white;
    }
}
