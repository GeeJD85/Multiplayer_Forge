using UnityEngine;
using UnityEngine.SceneManagement;

namespace GW.Multi
{
    public class Exit_Game : MonoBehaviour
    {
        Event_Master eventMaster;
        Scene current;

        public void ExitGame()
        {
            Application.Quit();
        }

        public void LoadLevel(int scene)
        {
            SceneManager.LoadScene(scene);
        }
    }
}
