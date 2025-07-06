using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public LayerMask cameraMask; // What objects can the third person camera clip through?
    public LayerMask interactionMask; // Which objects can be interacted with?
    public UI ui;

    [System.NonSerialized]
    public bool paused;

    public static GameManager instance;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    public void togglePause()
    {
        if (paused)
            resume();
        else
            pause();
    }
    public void pause()
    {
        paused = true;
        Cursor.lockState = CursorLockMode.None;
        ui.pauseMenu.SetActive(true);
    }
    public void resume()
    {
        paused = false;
        Cursor.lockState = CursorLockMode.Locked;
        ui.pauseMenu.SetActive(false);
    }

}
