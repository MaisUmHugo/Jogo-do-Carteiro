using TMPro;
using UnityEngine;

public class HordaManager : SpawnerManager
{
    private int NumeroHorda, N_Entregas;
    [Header("Controle Hordas")]
    public int E_Necessarias;
    public TextMeshProUGUI TextoHorda; 
    
    private bool HordaMudou, Objetivo;
    public new static HordaManager instance;
    private void Start()
    {
        NumeroHorda = 1;
    }
    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        verificarhorda();
        TextoHorda.text = "Horda: " + NumeroHorda;
    }
    private void MudarVelocidade()
    {
        if (HordaMudou)
        {
            multiplicadorVelocidade = multiplicadorVelocidade + 0.05f;
            HordaMudou = false;
        }
    }
    private void verificarhorda()
    {
        if (N_Entregas >= E_Necessarias)
        {
            Objetivo = true;
        }
        if (Objetivo)
        {
            AlterarHorda();
        }
    }
    private void AlterarHorda()
    {
        NumeroHorda = NumeroHorda + 1;
        HordaMudou = true;
        Objetivo = false;
        N_Entregas = 0;
        MudarVelocidade();
    }
    public void AumentarEntrega()
    {
        N_Entregas++;
        Debug.Log(N_Entregas);
    }
}
