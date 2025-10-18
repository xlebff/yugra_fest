using UnityEngine;
using UnityEngine.SceneManagement;

enum Scenes { MENU = 0, STORY }

public class LoadSceneManager : MonoBehaviour
{
    public void ToMenu() => SceneManager.LoadScene((int)Scenes.MENU);

    public void ToStory() => SceneManager.LoadScene((int)Scenes.STORY);
}
