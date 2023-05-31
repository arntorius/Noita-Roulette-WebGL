using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneSwitcher : MonoBehaviour
{
    // This function loads the scene with the specified name
    public void SwitchScene(string RouletteWebTest)
    {
        SceneManager.LoadScene(RouletteWebTest);
    }
}
