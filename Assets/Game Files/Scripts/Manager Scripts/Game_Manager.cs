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
        public GameObject menuCanvas;

        Event_Master eventMaster;
        Player myPlayer;
        Player_Name myPlayerName;
        public static string playerName;
        private bool isPlayerLoaded;
        bool menuOpen;

        public void SetPlayerName()
        {
            InputField nameField = GameObject.Find("Input_PlayerName").GetComponent<InputField>();

            if (nameField.text == null || nameField.text.Length < 3)
                return;
            playerName = nameField.text;

            GameObject go = NetworkManager.Instance.InstantiatePlayerConnection().gameObject;
            nameInputPanel.SetActive(false);
            go.GetComponent<Player_Connection>().playerName = playerName;
            go.GetComponent<Player_Connection>().connectionState = 1;
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
            menuCanvas.SetActive(!menuCanvas.activeSelf);
            menuOpen = !menuOpen;
        }

        void ToggleCursor()
        {
            if (!isPlayerLoaded)
                return;

            if (menuOpen)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
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