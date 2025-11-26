using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParallaxRandomSpawner : MonoBehaviour
{
    [Header("Renderer para mostrar os sprites")]
    public SpriteRenderer spriteRenderer;

    [Header("Animações (listas de sprites)")]
    public Sprite[] animacaoA;
    public Sprite[] animacaoB;
    public Sprite[] animacaoC;

    [Header("Configurações")]
    public float duracaoFrame = 0.07f;
    public float intervaloMin = 8f;
    public float intervaloMax = 14f;

    private bool animando = false;

    private List<Sprite[]> animacoesRestantes = new List<Sprite[]>();

    void Start()
    {
        spriteRenderer.enabled = false;

        animacoesRestantes.Add(animacaoA);
        animacoesRestantes.Add(animacaoB);
        animacoesRestantes.Add(animacaoC);

        StartCoroutine(ControlarAnimacoes());
    }

    IEnumerator ControlarAnimacoes()
    {
        while (animacoesRestantes.Count > 0)
        {
            float espera = Random.Range(intervaloMin, intervaloMax);
            yield return new WaitForSeconds(espera);

            if (!animando)
            {
                // sorteia um índice entre as animações restantes
                int indice = Random.Range(0, animacoesRestantes.Count);
                // pega a animação escolhida
                Sprite[] escolhida = animacoesRestantes[indice];
                animacoesRestantes.RemoveAt(indice);
                yield return StartCoroutine(TocarAnimacao(escolhida));
            }
        }

        // reiniciar se quiser
        // StartCoroutine(ReiniciarAnimacoes());  

        IEnumerator TocarAnimacao(Sprite[] frames)
        {
            animando = true;
            spriteRenderer.enabled = true;

            for (int i = 0; i < frames.Length; i++)
            {
                spriteRenderer.sprite = frames[i];
                yield return new WaitForSeconds(duracaoFrame);
            }

            spriteRenderer.enabled = false;
            animando = false;
        }

        // reiniciar quando acabar todas
        IEnumerator ReiniciarAnimacoes()
        {
            animacoesRestantes.Add(animacaoA);
            animacoesRestantes.Add(animacaoB);
            animacoesRestantes.Add(animacaoC);

            StartCoroutine(ControlarAnimacoes());
            yield break;
        }
    }
}