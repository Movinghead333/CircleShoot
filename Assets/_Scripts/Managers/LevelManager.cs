using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : SingletonPersistent<LevelManager>
{
    public GameState GameState { get; private set; }

    public List<GameObject> AllEnemies = new List<GameObject>();

    public GameObject LevelFinishedMenu;
    public GameObject GameResetMenu;
    public GameObject GameFinishedMenu;

    public GameObject Player;
    private PlayerController _playerController;

    private string _lastLevelSceneName = "Level2";

    private void Start()
    {
        ChangeState(GameState.Playing);
    }

    public void ChangeState(GameState newState)
    {
        Debug.Log("Trasitioning from state " + GameState + " to state " + newState);

        // We cannot complete a level if we died before.
        if (GameState == GameState.Dead && newState == GameState.LevelFinished)
        {
            return;
        }

        // We cannot fail a level if we already completed it.
        if (GameState  == GameState.LevelFinished && newState == GameState.Dead)
        {
            return;
        }


        GameState = newState;

        switch (newState)
        {
            case GameState.Setup:
                AllEnemies.Clear();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                GameResetMenu.SetActive(false);
                ChangeState(GameState.Playing);
                break;
            case GameState.Playing:
                break;
            case GameState.LevelFinished:
                // Disable player controls when the level is won
                _playerController.enabled = false;
                LevelFinishedMenu.SetActive(true);
                break;
            case GameState.Dead:
                GameResetMenu.SetActive(true);
                break;
            case GameState.GoToNextLevel:
                LevelFinishedMenu.SetActive(false);

                AllEnemies.Clear();

                if (SceneManager.GetActiveScene().name == _lastLevelSceneName)
                {
                    ChangeState(GameState.GameCompleted);
                }
                else
                {
                    SceneManager.LoadScene("Level2");
                    ChangeState(GameState.Playing);
                }
                    
                break;
            case GameState.GameCompleted:
                GameFinishedMenu.SetActive(true);
                break;
            case GameState.ExitGame:
                GameFinishedMenu.SetActive(false);
                Debug.Log("Quitting game");
                Application.Quit();
                break;
            case GameState.RestartGame:
                Debug.Log("Restarting game");
                SceneManager.LoadScene("Level1");
                break;
;        }
    }

    public void RegisterPlayer(GameObject playerGameobject)
    {
        Player = playerGameobject;
        _playerController = Player.GetComponent<PlayerController>();
    }

    public void RegisterEnemy(GameObject enemyGameobject)
    {
        AllEnemies.Add(enemyGameobject);
    }

    public void CheckWinCondition()
    {
        bool enemiesRemaining = false;

        foreach (GameObject go in AllEnemies)
        {
            enemiesRemaining |= go.activeSelf;
        }

        if (!enemiesRemaining)
        {
            ChangeState(GameState.LevelFinished);
        }
    }
}

public enum GameState
{
    Setup,
    Playing,
    LevelFinished,
    Dead,
    GoToNextLevel,
    GameCompleted,
    ExitGame,
    RestartGame
}