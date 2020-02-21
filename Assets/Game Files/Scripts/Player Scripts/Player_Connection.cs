using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GW.Multi
{
    public class Player_Connection : PlayerConnectionBehavior
    {
        public Transform connectionStatusContent;
        public string playerName;

        private Text connectionText;

        private void Awake()
        {
            connectionStatusContent = GameObject.Find("Content_ConnectionInfo").transform;
            connectionText = GetComponentInChildren<Text>();
        }

        public void SetPlayerName(string name)
        {
            playerName = name;
        }

        protected override void NetworkStart()
        {
            base.NetworkStart();

            gameObject.SetActive(false);
            transform.SetParent(connectionStatusContent);

            if (!networkObject.IsOwner)
                return;

            networkObject.SendRpc(RPC_PLAYER_CONNECTED, Receivers.All, playerName);
        }

        void SetTextConnected()
        {
            connectionText.text = " " + playerName + " connected to server!";
            gameObject.SetActive(true);
        }

        public override void playerConnected(RpcArgs args)
        {
            playerName = args.GetNext<string>();
            SetTextConnected();
            StartCoroutine(DestroyTextPanel());
        }

        IEnumerator DestroyTextPanel()
        {
            yield return new WaitForSeconds(2);
            Destroy(gameObject);
        }
    }
}