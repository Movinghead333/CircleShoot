using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFinishedUIC : MonoBehaviour
{
    public void ContinueToNextLevelButtonPressed()
    {
        LevelManager.Instance.ChangeState(GameState.GoToNextLevel);
    }
}
