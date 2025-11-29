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
    public float duracaoFrame;
    public float intervaloMin = 8f;
    public float intervaloMax = 14f;

    private bool animando = false;

    [Header("Movimento")]
    public float velocidadeMovimento = 2f; 
    public Transform pontoInicial;         
    public Transform pontoFinal;           


    private List<Sprite[]> animacoesRestantes = new List<Sprite[]>();

    void Start()
    {
        spriteRenderer.enabled = false;

        animacoesRestantes.Add(animacaoA);
        animacoesRestantes.Add(animacaoB);
        animacoesRestantes.Add(animacaoC);

        StartCoroutine(ControlarAnimacoes());
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.T))
        {
            // Se ainda há animações e não está animando, inicia uma agora
            if (!animando && animacoesRestantes.Count > 0)
            {
                int indice = Random.Range(0, animacoesRestantes.Count);
                Sprite[] escolhida = animacoesRestantes[indice];
                animacoesRestantes.RemoveAt(indice);

                StartCoroutine(TocarAnimacao(escolhida));
            }
            else if (animacoesRestantes.Count == 0)
            {
                Debug.Log("Foi todas as animações ai.");
                StartCoroutine(ReiniciarAnimacoes());
            }
        }
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
        StartCoroutine(ReiniciarAnimacoes());
    }
    IEnumerator TocarAnimacao(Sprite[] frames)
    {
        animando = true;
        spriteRenderer.enabled = true;

        // COMEÇA FORA DA CÂMERA
        transform.position = pontoInicial.position;

        for (int i = 0; i < frames.Length; i++)
        {
            spriteRenderer.sprite = frames[i];

            // MOVE EM X enquanto anima
            float tempo = 0f;
            while (tempo < duracaoFrame)
            {
                tempo += Time.deltaTime;
                transform.position += Vector3.right * velocidadeMovimento * Time.deltaTime;

                if (transform.position.x >= pontoFinal.position.x)
                    break;

                yield return null;
            }
        }

        spriteRenderer.enabled = false;
        animando = false;
    }

    IEnumerator ReiniciarAnimacoes()
      {
          animacoesRestantes.Add(animacaoA);
          animacoesRestantes.Add(animacaoB);
          animacoesRestantes.Add(animacaoC);

          StartCoroutine(ControlarAnimacoes());
          yield break;
      }
}