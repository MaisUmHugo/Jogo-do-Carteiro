using UnityEngine;
using UnityEngine.SceneManagement;

public class ComoJogar : MonoBehaviour
{
    public GameObject HudCanvas;
    private void Awake()
    {
        HudCanvas.SetActive(false);
        Time.timeScale = 0f;
    }
    public void Comecar()
    {
        HudCanvas.SetActive(true);
        this.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}
