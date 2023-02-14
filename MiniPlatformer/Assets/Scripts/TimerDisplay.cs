using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;

    private void Update()
    {
        timerText.text = $"TIME: {Time.timeSinceLevelLoad.ToString("0.00")} SECONDS";
    }
}
