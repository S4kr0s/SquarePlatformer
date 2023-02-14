using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class LevelSelectIcon : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;

    [SerializeField] private float[] devTimes { 
        get{
            return new float[] { 
                2.88f, 2.91f, 2.92f, 1.31f, 1.61f, 
                2.31f, 2.80f, 9.87f, 8.71f, 11.81f,
                4.95f, 12.50f, 3.50f, 12.48f, 8.37f, 2.22f
            };
        } 
    } 

    void Start()
    {
        float time = PlayerPrefs.GetFloat($"{gameObject.name}speed");

        if (time == 0 && gameObject.name != "Level00")
            gameObject.SetActive(false);

        timeText.text = time.ToString("0.00");

        int index = int.Parse(gameObject.name.Replace("Level", ""));

        float timeOnPreviousLevel = PlayerPrefs.GetFloat($"Level{(index-1).ToString("00")}speed");
        if (timeOnPreviousLevel != 0 && gameObject.name != "Level00")
            gameObject.SetActive(true);

        if (time != 0 && time <= devTimes[index])
        {
            Image image = GetComponent<Image>();
            image.color = new Color(1f, 0.816f, 0f);
        }
    }
}
