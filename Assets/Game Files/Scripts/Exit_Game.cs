using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GW.Multi
{
    public class Exit_Game : MonoBehaviour
    {
        Event_Master eventMaster;
        Scene current;
        private void OnEnable()
        {
            current = SceneManager.GetActiveScene();
            if (current.buildIndex == 0)
                return;
            eventMaster = FindObjectOfType<Event_Master>();
            eventMaster.PlayerDisconnected += ExitGame;
        }

        private void OnDisable()
        {
            current = SceneManager.GetActiveScene();
            if (current.buildIndex == 0)
                return;
            eventMaster.PlayerDisconnected -= ExitGame;
        }

        public void ExitGame()
        {
            string playerName = Game_Manager.playerName;

            if (playerName == null)
                playerName = "Player";

            GameObject go = NetworkManager.Instance.InstantiatePlayerConnection().gameObject;
            go.GetComponent<Player_Connection>().playerName = playerName;
            go.GetComponent<Player_Connection>().connectionState = 2;

            Application.Quit();
        }
    }
}