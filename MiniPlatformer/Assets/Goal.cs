using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    [SerializeField] private GameObject levelFinishedGameObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Win();
        }
    }

    private void Win()
    {
        Time.timeScale = 0f;
        float time = Time.timeSinceLevelLoad;
        LevelFinishedUI levelFinishedUI = levelFinishedGameObject.GetComponent<LevelFinishedUI>();
        levelFinishedGameObject.SetActive(true);
        levelFinishedUI.timeText.text = $"Time: {time.ToString("0.00")}";
        if (PlayerPrefs.GetFloat($"{SceneManager.GetActiveScene().name}speed") == 0 || PlayerPrefs.GetFloat($"{SceneManager.GetActiveScene().name}speed") > time)
        {
            PlayerPrefs.SetFloat($"{SceneManager.GetActiveScene().name}speed", time);
            levelFinishedUI.newHighscore.SetActive(true);
        }
    }
}
