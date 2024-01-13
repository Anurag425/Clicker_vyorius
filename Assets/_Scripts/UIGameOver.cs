using UnityEngine;
using TMPro;
using Unity.Netcode;

public class UIGameOver : MonoBehaviour
{
    private Canvas _gameOverCanvas;
    public TextMeshProUGUI winnerText;
    public static string winnerName;

    private void Start()
    {
        _gameOverCanvas = GetComponent<Canvas>();
    }

    private void OnEnable()
    {
        PlayerController.GameOverEvent += GameOver;
    }

    private void OnDisable()
    {
        PlayerController.GameOverEvent  -= GameOver;
    }

    private void GameOver()
    {
        winnerText.text = winnerName;
        _gameOverCanvas.enabled = true;
    }

}
