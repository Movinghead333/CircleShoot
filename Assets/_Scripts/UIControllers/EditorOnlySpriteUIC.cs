using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorOnlySpriteUIC : MonoBehaviour
{
    // Disable the sprite renderer on awake, since it should only be displayed in editor.
    void Awake()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
