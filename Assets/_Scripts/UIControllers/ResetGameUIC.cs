using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGameUIC : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("resetgameuic");
        LevelManager.Instance.gameResetMenu = gameObject;
    }

    public void GameResetButtonPressed()
    {
        Debug.Log("GameResetButtonPressed triggered");
        LevelManager.Instance.ChangeState(GameState.Setup);
    }
}
