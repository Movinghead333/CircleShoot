using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UICanvasUIC : MonoBehaviour
{
    public GameObject LevelFinishedMenu;
    public GameObject ResetGameMenu;
    public GameObject GameFinishedMenu;

    // Start is called before the first frame update
    void Start()
    {
        LevelManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void OnDestroy()
    {
        LevelManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void GameManager_OnGameStateChanged(GameState newState)
    {
        LevelFinishedMenu.SetActive(false);
        ResetGameMenu.SetActive(false);
        //GameFinishedMenu.SetActive(false);

        switch (newState)
        {
            case GameState.Setup:
                break;
            case GameState.Playing:
                break;
            case GameState.LevelFinished:
                LevelFinishedMenu.SetActive(true);
                break;
            case GameState.Dead:
                ResetGameMenu.SetActive(true);
                break;
            case GameState.GoToNextLevel:
                break;
            case GameState.GameCompleted:
                Debug.Log("Setting game completed panel active");
                GameFinishedMenu.SetActive(true);
                break;
            case GameState.ExitGame:
                break;
            case GameState.RestartGame:
                break;
                ;
        }
    }
}
