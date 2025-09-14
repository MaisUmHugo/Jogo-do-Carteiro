using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    private INPUTS inputs;

    [Header("Painéis")]
    [SerializeField] private GameObject painelMenuInicial;
    [SerializeField] private GameObject painelOpcoes;
    [SerializeField] private GameObject painelConfirmacao;
    [SerializeField] private GameObject fundoCinza;

    [SerializeField] private string CenaJogar;

    private System.Action acaoConfirmada;

    private void Awake()
    {
        inputs = new INPUTS();

        // ESC fecha opções se estiver nelas
        inputs.UI.Cancel.performed += ctx =>
        {
            if (painelOpcoes.activeSelf) FecharOpcoes();
            else if (painelConfirmacao.activeSelf) BotaoConfirmarNao();
        };
    }

    private void OnEnable()
    {
        inputs.Enable();
        inputs.Gameplay.Disable(); // garante que controles de gameplay não fiquem ativos
        inputs.UI.Enable();
    }

    private void OnDisable()
    {
        inputs.Disable();
    }

    private void Start()
    {
        if (painelConfirmacao != null)
            painelConfirmacao.SetActive(false);
    }

    public void Jogar()
    {
        SceneManager.LoadScene(CenaJogar); 
    }

    public void AbrirOpcoes()
    {
        painelMenuInicial.SetActive(false);
        fundoCinza.SetActive(true);
        painelOpcoes.SetActive(true);
    }

    public void FecharOpcoes()
    {
        painelOpcoes.SetActive(false);
        fundoCinza.SetActive(false);
        painelMenuInicial.SetActive(true);
    }

    public void SairJogo()
    {
        MostrarConfirmacao(() =>
        {
            Debug.Log("Saindo do jogo...");
            Application.Quit();
        });
    }

    public void BotaoCreditos()
    {
        SceneManager.LoadScene("Creditos"); 
    }

    private void MostrarConfirmacao(System.Action acao)
    {
        painelMenuInicial.SetActive(false);
        fundoCinza.SetActive(true);
        painelConfirmacao.SetActive(true);

        acaoConfirmada = acao;
    }

    public void BotaoConfirmarSim()
    {
        acaoConfirmada?.Invoke();
        acaoConfirmada = null;
    }

    public void BotaoConfirmarNao()
    {
        painelConfirmacao.SetActive(false);
        fundoCinza.SetActive(false);
        painelMenuInicial.SetActive(true);

        acaoConfirmada = null;
    }
}
