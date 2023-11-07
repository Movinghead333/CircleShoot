using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : SingletonPersistent<LevelManager>
{
    public GameState gameState { get; private set; }

    public List<GameObject> allEnemies = new List<GameObject>();

    public GameObject levelFinishedMenu;
    public GameObject gameResetMenu;
    public GameObject gameFinishedMenu;

    public GameObject player;
    private PlayerController playerController;

    private string lastLevelSceneName = "Level2";

    private void Start()
    {
        ChangeState(GameState.Playing);
    }

    public void ChangeState(GameState newState)
    {
        Debug.Log("Trasitioning from state " + gameState + " to state " + newState);

        // We cannot complete a level if we died before.
        if (gameState == GameState.Dead && newState == GameState.LevelFinished)
        {
            return;
        }

        // We cannot fail a level if we already completed it.
        if (gameState  == GameState.LevelFinished && newState == GameState.Dead)
        {
            return;
        }


        gameState = newState;

        switch (newState)
        {
            case GameState.Setup:
                allEnemies.Clear();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                gameResetMenu.SetActive(false);
                ChangeState(GameState.Playing);
                break;
            case GameState.Playing:
                break;
            case GameState.LevelFinished:
                // Disable player controls when the level is won
                playerController.enabled = false;
                levelFinishedMenu.SetActive(true);
                break;
            case GameState.Dead:
                gameResetMenu.SetActive(true);
                break;
            case GameState.GoToNextLevel:
                levelFinishedMenu.SetActive(false);

                allEnemies.Clear();

                if (SceneManager.GetActiveScene().name == lastLevelSceneName)
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
                gameFinishedMenu.SetActive(true);
                break;
            case GameState.ExitGame:
                gameFinishedMenu.SetActive(false);
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
        player = playerGameobject;
        playerController = player.GetComponent<PlayerController>();
    }

    public void RegisterEnemy(GameObject enemyGameobject)
    {
        allEnemies.Add(enemyGameobject);
    }

    public void CheckWinCondition()
    {
        bool enemiesRemaining = false;

        foreach (GameObject go in allEnemies)
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