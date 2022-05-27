using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;


public class SceneSwitch : MonoBehaviour
{
    public void SwitchScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void Finish()
    {
        Application.Quit();
    }
}
