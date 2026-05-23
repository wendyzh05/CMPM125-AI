using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform player;
    public Transform startPoint;

    public GameObject winText;
    public GameObject caughtText;

    private CharacterController controller;

    void Start()
    {
        controller = player.GetComponent<CharacterController>();

        if (winText != null) winText.SetActive(false);
        if (caughtText != null) caughtText.SetActive(false);

        RespawnPlayer();
    }

    public void RespawnPlayer()
    {
        if (caughtText != null) caughtText.SetActive(true);

        if (controller != null) controller.enabled = false;

        player.position = startPoint.position;
        player.rotation = startPoint.rotation;

        if (controller != null) controller.enabled = true;

        Invoke(nameof(HideCaughtText), 1f);
    }

    void HideCaughtText()
    {
        if (caughtText != null) caughtText.SetActive(false);
    }

    public void WinGame()
    {
        if (winText != null) winText.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }
}
