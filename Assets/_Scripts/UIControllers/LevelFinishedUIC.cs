using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFinishedUIC : MonoBehaviour
{
    private void Start()
    {
        LevelManager.Instance.levelFinishedMenu = gameObject;
    }

    public void ContinueToNextLevelButtonPressed()
    {
        LevelManager.Instance.ChangeState(GameState.GoToNextLevel);
    }
}
