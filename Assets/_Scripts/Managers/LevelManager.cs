using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : SingletonPersistent<LevelManager>
{
    public GameState GameState { get; private set; }

    public static event Action<GameState> OnGameStateChanged;

    private int _currentSceneIndex = 0;

    private const int _firstLevelIndex = 0;
    private const int _lastLevelSceneIndex = 1;

    private void Start()
    {
        Debug.Log($"Final level index: {_lastLevelSceneIndex}");
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
                Setup();
                break;
            case GameState.Playing:
                break;
            case GameState.LevelFinished:
                break;
            case GameState.Dead:
                break;
            case GameState.GoToNextLevel:
                GoToNextLevel();                 
                break;
            case GameState.GameCompleted:
                break;
            case GameState.ExitGame:
                ExitGame();
                break;
            case GameState.RestartGame:
                RestartGame();
                break;
;       }

        OnGameStateChanged?.Invoke(newState);
    }
    public void CheckWinCondition()
    {
        bool enemiesRemaining = false;

        foreach (GameObject go in UnitManager.Instance.AllEnemies)
        {
            enemiesRemaining |= go.activeSelf;
        }

        if (!enemiesRemaining)
        {
            ChangeState(GameState.LevelFinished);
        }
    }

    private void Setup()
    {
        SceneManager.LoadScene(_currentSceneIndex);
        
        ChangeState(GameState.Playing);
    }

    private void GoToNextLevel()
    {
        UnitManager.Instance.Clear();

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        Debug.Log($"Scene index: {nextSceneIndex}");

        if (nextSceneIndex > _lastLevelSceneIndex)
        {
            ChangeState(GameState.GameCompleted);
        }
        else
        {
            _currentSceneIndex = nextSceneIndex;
            ChangeState(GameState.Setup);
        }
    }

    private void ExitGame()
    {
        Debug.Log("Quitting game");
        Application.Quit();
    }

    private void RestartGame()
    {
        Debug.Log("Restarting game");
        SceneManager.LoadScene(_firstLevelIndex);
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