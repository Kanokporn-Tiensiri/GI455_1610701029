using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{
    public InputField IP;
    public InputField Port;
    public void LoadSceneNextChat()
    {
        if (IP.text + ":" + Port.text == "127.0.0.1:31124")
        {
            SceneManager.LoadScene(1);
        }
    }
    public void LoadSceneBackLogin()   
    {
        SceneManager.LoadScene(0);
    }
}
