using UnityEngine;
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
