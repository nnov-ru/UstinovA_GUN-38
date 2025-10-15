using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void OpenMainScene()
    {
        throw new NotImplementedException();
        SceneManager.LoadScene(0);
    }
    public void OpenGameScene()
    {
        SceneManager.LoadScene(1//, LoadSceneMode.Additive);
            );
    }
}
