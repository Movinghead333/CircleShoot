using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvasUIC : MonoBehaviour
{
    public GameObject LevelFinishedMenu;
    public GameObject ResetGameMenu;
    public GameObject GameFinishedMenu;

    // Start is called before the first frame update
    void Start()
    {
        LevelManager.Instance.LevelFinishedMenu = LevelFinishedMenu;
        LevelManager.Instance.GameResetMenu = ResetGameMenu;
        LevelManager.Instance.GameFinishedMenu = GameFinishedMenu;
    }
}
