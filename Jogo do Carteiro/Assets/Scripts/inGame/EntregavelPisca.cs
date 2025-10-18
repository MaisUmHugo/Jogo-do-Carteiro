using UnityEngine;
using System.Collections;

public class EntregavelPisca : MonoBehaviour
{
    private SpriteRenderer sr;
    private Coroutine rotina;

    [Header("Configurações de Piscar")]
    public float intervalo = 0.2f;
    public int quantidadePiscadas = 3;
    private Color corPiscar = new Color(1f, 0.2f, 0.2f, 0.8f);

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr == null)
            Debug.LogWarning($"{name}: Nenhum SpriteRenderer encontrado!");
        else
            Debug.Log($"{name}: SpriteRenderer encontrado -> {sr.gameObject.name}");
    }

    public void IniciarPiscar()
    {
        if (sr == null) return;

        // Se já estiver piscando, cancela a anterior
        if (rotina != null) StopCoroutine(rotina);
        rotina = StartCoroutine(Piscar());
    }

    private IEnumerator Piscar()
    {
        // guarda cor original
        Color corOriginal = sr.material.color;

        for (int i = 0; i < quantidadePiscadas; i++)
        {
            sr.material.color = corPiscar;
            yield return new WaitForSeconds(intervalo);
            sr.material.color = corOriginal;
            yield return new WaitForSeconds(intervalo);
        }

        // garante que volta ao normal no fim
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
