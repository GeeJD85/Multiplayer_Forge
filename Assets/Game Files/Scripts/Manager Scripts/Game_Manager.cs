using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;
using UnityEngine.UI;
using BeardedManStudios.Forge.Networking;
using System.Collections.Generic;

namespace GW.Multi
{
    public class Game_Manager : GameManagerBehavior
    {
        public Transform[] spawns;
        public GameObject teamSelectPanel;
        public GameObject nameInputPanel;
        public GameObject menuCanvas;

        Event_Master eventMaster;
        Player myPlayer;
        Player_Name myPlayerName;
        public string playerName;
        private bool isPlayerLoaded;
        bool menuOpen;

        protected override void NetworkStart()
        {
            base.NetworkStart();

            SetupDisconnect();      
        }

        void SetupDisconnect()
        {
            //safety check 
            if (!networkObject.IsServer)
            {
                return;
            }

            NetworkManager.Instance.Networker.playerAccepted += (player, sender) =>
            {
                MainThreadManager.Run(() =>
                {
                    //Do some counting logic here for a gamemode, eg, assign team to newly joined player, or restart round if enough people joined
                    //Remember to remove players from counter in playerDisconnected event as well
                });
            };

            //Handle disconnection
            NetworkManager.Instance.Networker.playerDisconnected += (player, sender) =>
            {
                MainThreadManager.Run(() =>
                {
                    //Loop through all players and find the player who disconnected, store all it's networkobjects to a list
                    List<NetworkObject> toDelete = new List<NetworkObject>();
                    foreach (var no in sender.NetworkObjectList)
                    {
                        if (no.Owner == player)
                        {
                            //Found him
                            toDelete.Add(no);
                        }
                    }

                    //Remove the actual network object outside of the foreach loop, as we would modify the collection at runtime elsewise. (could also use a return, too late)
                    if (toDelete.Count > 0)
                    {
                        for (int i = toDelete.Count - 1; i >= 0; i--)
                        {
                            sender.NetworkObjectList.Remove(toDelete[i]);
                            toDelete[i].Destroy();
                        }
                    }
                });
            };
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
            myPlayerName.myTeam = team;

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