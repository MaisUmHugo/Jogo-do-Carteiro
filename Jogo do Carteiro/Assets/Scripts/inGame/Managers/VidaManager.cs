using UnityEngine;
using System;
using System.Collections;

public class VidaManager : MonoBehaviour
{
    public static VidaManager instance;

    [Header("Configuração de Vidas")]
    public int vidasIniciais = 3;
    [HideInInspector] public int vidasAtuais;

    public event Action<int> OnVidaMudou; // evento para HUD
    public event Action OnGameOver;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        ResetarVidas();
    }

    public void ResetarVidas()
    {
        vidasAtuais = vidasIniciais;
        OnVidaMudou?.Invoke(vidasAtuais);
    }

    public void PerderVida()
    {
        vidasAtuais--;
        anim.SetBool("Damage", true);
        OnVidaMudou?.Invoke(vidasAtuais);
        DelayVida();
        if (vidasAtuais <= 0)
        {
            Debug.Log("GAME OVER!");
            OnGameOver?.Invoke();
        }
    }

    public void GanharVida()
    {
        vidasAtuais++;
        OnVidaMudou?.Invoke(vidasAtuais);
    }
    private IEnumerator DelayVida()
    {
        yield return new WaitForSeconds(1.5f);
        anim.SetBool("Damage", false);
    }
}
