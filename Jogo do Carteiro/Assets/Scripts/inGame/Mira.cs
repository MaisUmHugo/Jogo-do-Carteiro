using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using UnityEngine.UI;

public class Mira : MonoBehaviour
{
    private Camera cam;
    private INPUTS inputs;

    [Header("Cooldown Visual")]
    public Image cooldownUI; // Imagem do círculo de cooldown
    [HideInInspector] public float cooldownProgresso; // Valor entre 0 e 1
    [HideInInspector] public bool emCooldown;
    void Awake()
    {
        cam = Camera.main;
        inputs = new INPUTS();
    }

    void OnEnable()
    {
        inputs.Gameplay.Enable();
    }

    void OnDisable()
    {
        inputs.Gameplay.Disable();
    }

    void Update()
    {
        // Lê a posição do mouse/tela pelo novo Input System
        Vector2 pointerPos = inputs.Gameplay.Aim.ReadValue<Vector2>();

        // Converte para mundo
        Vector3 worldPos = cam.ScreenToWorldPoint(pointerPos);
        worldPos.z = -5f;

        // Move a mira
        transform.position = worldPos;

        // Atualiza visual do cooldown
        if (cooldownUI != null)
        {
            cooldownUI.fillAmount = 1f - cooldownProgresso; // inverte, pra ir "descendo"
            cooldownUI.enabled = emCooldown; // só aparece enquanto está recarregando
        }
    }
}
