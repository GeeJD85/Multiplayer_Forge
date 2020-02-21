using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace GW.Multi
{
    public class Game_Manager : MonoBehaviour
    {
        public Transform[] spawns;
        public GameObject teamSelectPanel;
        public GameObject nameInputPanel;
        public GameObject menuPanel;

        Event_Master eventMaster;
        Player myPlayer;
        Player_Name myPlayerName;
        public string playerName;
        private bool isPlayerLoaded;

        public void ExitGame()
        {
            Application.Quit();
            Debug.Log("ExitCalled");
        }

        public void SetPlayerName()
        {
            InputField nameField = GameObject.Find("Input_PlayerName").GetComponent<InputField>();

            if (nameField.text == null || nameField.text.Length < 3)
                return;
            playerName = nameField.text;

            GameObject go = NetworkManager.Instance.InstantiatePlayerConnection().gameObject;
            nameInputPanel.SetActive(false);
            go.GetComponent<Player_Connection>().playerName = playerName;
        }

        public void SpawnPlayer(int team)
        {
            Camera.main.gameObject.SetActive(false);
            GameObject go = NetworkManager.Instance.InstantiatePlayerControl(position: spawns[team].position).gameObject;

            myPlayer = go.GetComponent<Player>();
            myPlayer.team = team;

            myPlayerName = go.GetComponent<Player_Name>();
            myPlayerName.playerName = playerName;

            teamSelectPanel.SetActive(false);
            isPlayerLoaded = true;
        }

        #region Enable/Disable
        private void OnEnable()
        {
            eventMaster = GetComponent<Event_Master>();
            eventMaster.ToggleMenuEvent += ToggleMenuPanel;
            eventMaster.ToggleMenuEvent += ToggleCursor;
        }

        private void OnDisable()
        {
            eventMaster.ToggleMenuEvent -= ToggleMenuPanel;
            eventMaster.ToggleMenuEvent -= ToggleCursor;
        }
        #endregion

        void ToggleMenuPanel()
        {
            menuPanel.SetActive(!menuPanel.activeSelf);
        }

        void ToggleCursor()
        {
            if (!isPlayerLoaded)
                return;

            if (myPlayer.myController.menuOpen)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else if (!myPlayer.myController.menuOpen)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                eventMaster.CallEventToggleMenu();
            }
        }
    }
}