using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Text textHigh;
    public int highScore;
    void Start()
    {
        if (!PlayerPrefs.HasKey("highscore"))
        {
            PlayerPrefs.SetInt("highscore", 0);
            PlayerPrefs.Save();
        }
        textHigh.text = PlayerPrefs.GetInt("highscore").ToString();
    }
    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
