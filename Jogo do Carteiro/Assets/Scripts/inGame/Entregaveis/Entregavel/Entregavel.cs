using UnityEngine;

public abstract class Entregavel : MonoBehaviour
{
    [Header("Configuração do Entregável")]
    public bool ativoParaEntrega = true; // booleana para alternar quando pode entregar ou n

    public virtual void ReceberEntrega()
    {
        if (!ativoParaEntrega) return;

        // Pontuação e combo
        ScoreManager.instance.AdicionarPontos(100);
        ComboManager.instance.AumentarCombo();


        Debug.Log($"{gameObject.name} recebeu a entrega!");
    }

    public virtual void FalharEntrega()
    {
        // Reset de combo e vida
        VidaManager.instance.PerderVida();
        ComboManager.instance.ResetarCombo();

        Debug.Log($"{gameObject.name} NÃO recebeu a entrega!");
    }

    public virtual void PerderCombo()
    {
        // Reset Combo
        ComboManager.instance.ResetarCombo();
    }
}
