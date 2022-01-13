using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class cSceneLink : MonoBehaviour
{
    [SerializeField] private string sceneName;

    public void ChangeScene()
    {
        //if (SceneManager.GetSceneByName(sceneName).IsValid())
        //{
            Debug.Log($"Loading scene '{sceneName}'...");
            SceneManager.LoadScene(sceneName);
        //}

        //else Debug.Log($"Scene '{sceneName}' is invalid.");
    }
}
