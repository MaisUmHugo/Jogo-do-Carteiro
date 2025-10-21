using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    [Header("Configurações da Cutscene")]
    [SerializeField] private float duracao; 
    [SerializeField] private string CenaJogo = "CenaJogo"; 

    private bool podePular = true;

    void Start()
    {
        // Simula uma cutscene com tempo (placeholder)
        Invoke(nameof(CarregarCenaJogo), duracao);
    }

    void Update()
    {
        // Permite pular com tecla ou botão (ex: espaço, Enter, A do controle)
        if (podePular && (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Submit")))
        {
            CancelInvoke();
            CarregarCenaJogo();
        }
    }

    void CarregarCenaJogo()
    {
        SceneManager.LoadScene(CenaJogo);
    }
}
