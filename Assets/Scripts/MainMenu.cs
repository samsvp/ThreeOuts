using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenu;
    [SerializeField]
    private GameObject tutorialMenu;

    string tutorialPath;
    
    private void Start()
    {
        print(Application.persistentDataPath);
        tutorialPath = Application.persistentDataPath + "/tutorialDone.txt";
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void NewGame()
    {
        Save.SaveGame(0);
        PlayGame(true);
    }

    public void PlayGame(bool checkTutorial)
    {
        if (!checkTutorial) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        if (System.IO.File.Exists(tutorialPath))
        {
            string text = System.IO.File.ReadAllText(tutorialPath);
            if(Int32.Parse(text) == 1) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            mainMenu.SetActive(false);
            tutorialMenu.SetActive(true);
        }
    }

    public void PlayTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }

}