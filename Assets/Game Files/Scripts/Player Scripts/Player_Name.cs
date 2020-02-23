using UnityEngine;
using UnityEngine.UI;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;

namespace GW.Multi
{
    public class Player_Name : PlayerNamePlateBehavior
    {
        public string playerName;
        public int myTeam;
        public Camera myCamera;

        Text myText;

        protected override void NetworkStart()
        {
            Debug.Log("PN START");
            base.NetworkStart();

            if (!networkObject.IsOwner)
                return;

            networkObject.SendRpc(RPC_SET_NAME_PLATE, Receivers.AllBuffered, playerName, myTeam);                     
        }

        void Start()
        {
            myText = GetComponentInChildren<Text>();
        }

        private void Update()
        {
            FacePlatesToCamera();
        }

        void FacePlatesToCamera()
        {
            RaycastHit[] playersHit = Physics.SphereCastAll(transform.position, 40, transform.forward);

            for(int i = 0; i < playersHit.Length; i++)
            {
                if(playersHit[i].transform.GetComponent<Player_Name>() != null && playersHit[i].transform.GetComponentInChildren<Player_Name>() != this)
                {
                    Canvas namePlate = playersHit[i].transform.GetComponentInChildren<Canvas>();
                    namePlate.transform.LookAt(myCamera.transform);
                    namePlate.transform.Rotate(0, 180, 0);
                }
            }
        }

        void SetNamePanelText()
        {            
            myText.text = playerName;           

            if (networkObject.IsOwner)
                myText.enabled = false;
        }

        public override void setNamePlate(RpcArgs args)
        {
            playerName = args.GetNext<string>();
            int team = args.GetNext<int>();

            if (team == myTeam)
                myText.color = Color.green;
            if (team != myTeam)
                myText.color = Color.red;

            SetNamePanelText();
        }
    }
}