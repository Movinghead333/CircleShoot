using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvasUIC : MonoBehaviour
{
    public GameObject levelFinishedMenu;
    public GameObject resetGameMenu;
    public GameObject gameFinishedMenu;

    // Start is called before the first frame update
    void Start()
    {
        LevelManager.Instance.levelFinishedMenu = levelFinishedMenu;
        LevelManager.Instance.gameResetMenu = resetGameMenu;
        LevelManager.Instance.gameFinishedMenu = gameFinishedMenu;
    }
}
