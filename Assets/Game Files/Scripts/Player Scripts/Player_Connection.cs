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
        public int connectionState;

        private Text connectionText;

        private void Awake()
        {
            connectionStatusContent = GameObject.Find("Content_ConnectionInfo").transform;
            connectionText = GetComponentInChildren<Text>();
        }

        protected override void NetworkStart()
        {
            base.NetworkStart();

            gameObject.SetActive(false);
            transform.SetParent(connectionStatusContent);

            if (!networkObject.IsOwner)
                return;

            if(connectionState == 1)
                networkObject.SendRpc(RPC_PLAYER_CONNECTED, Receivers.All, playerName);
            if (connectionState == 2)
                networkObject.SendRpc(RPC_PLAYER_DISCONNECTED, Receivers.All, playerName);
        }

        void SetTextConnected()
        {
            connectionText.text = " " + playerName + " connected to server!";
            gameObject.SetActive(true);
        }

        void SetTextDisconnected()
        {
            connectionText.text = " " + playerName + " disconnected!";
            gameObject.SetActive(true);
        }

        public override void playerConnected(RpcArgs args)
        {
            playerName = args.GetNext<string>();
            SetTextConnected();
            StartCoroutine(DestroyTextPanel());
        }

        public override void playerDisconnected(RpcArgs args)
        {
            playerName = args.GetNext<string>();
            SetTextDisconnected();
            StartCoroutine(DestroyTextPanel());
        }

        IEnumerator DestroyTextPanel()
        {
            yield return new WaitForSeconds(2);
            Destroy(gameObject);
        }
    }
}