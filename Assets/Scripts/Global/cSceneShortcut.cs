using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class cSceneShortcut : MonoBehaviour
{
    public string scene1;
    public string scene2;
    public string scene3;
    public string scene4;

    void Start()
    {
        Application.targetFrameRate = 144;
    }

    // Update is called once per frame
    void Update()
    {   
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Alpha1))
        { 
            // Application.LoadLevel(scene1);
            SceneManager.LoadScene(scene1);
            
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) SceneManager.LoadScene(scene2);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) SceneManager.LoadScene(scene3);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) SceneManager.LoadScene(scene4);
    }

    public void Close()
    {
        Application.Quit();
    }
}
