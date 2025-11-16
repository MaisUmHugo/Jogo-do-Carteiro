using UnityEngine;

public class ComoJogar : MonoBehaviour
{
    public GameObject HudCanvas;
    private void Awake()
    {
        HudCanvas.SetActive(false);
        Time.timeScale = 0f;
    }
}
