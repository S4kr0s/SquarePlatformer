﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    public void LoadLevel(string name)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(name);
    }
}
