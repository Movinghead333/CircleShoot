using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFinishedUIC : MonoBehaviour
{
    private void Start()
    {
        LevelManager.Instance.gameFinishedMenu = gameObject;
    }

    public void RestartGameButtonPressed()
    {
        LevelManager.Instance.ChangeState(GameState.RestartGame);
    }

    public void ExitGameButtonPressed()
    {
        LevelManager.Instance.ChangeState(GameState.ExitGame);
    }
}
