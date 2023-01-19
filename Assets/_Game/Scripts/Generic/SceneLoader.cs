using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Generics
{
    public class SceneLoader : MonoBehaviour
    {
        public static void LoadScene(string scene)
        {
            SceneManager.LoadScene(scene);
        }

        public static void QuitGame()
        {
            Application.Quit();
        }
    }
}
