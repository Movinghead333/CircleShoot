using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGameUIC : MonoBehaviour
{
    public void GameResetButtonPressed()
    {
        Debug.Log("GameResetButtonPressed triggered");
        LevelManager.Instance.ChangeState(GameState.Setup);
    }
}
