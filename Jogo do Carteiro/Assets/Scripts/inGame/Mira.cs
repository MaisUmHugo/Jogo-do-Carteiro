using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class Mira : MonoBehaviour
{
    private Camera cam;
    private INPUTS inputs;

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
    }
}
