using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

namespace GW.Multi
{
    public class Player : PlayerControlBehavior
    {
        public int team;
        public Material[] teamMats;
        public MeshRenderer myRenderer;
        public FirstPersonController myController;

        Event_Master eventMaster;
        public float oldWalkSpeed;
        public float oldRunSpeed;

        // NetworkStart() is **automatically** called, when a networkObject 
        // has been fully setup on the network and ready/finalized on the network!
        // In simpler words, think of it like Unity's Start() but for the network ;)
        protected override void NetworkStart()
        {
            base.NetworkStart();

            // If this networkObject is actually not the one we will control and own
            if (!networkObject.IsOwner)
            {
                // Don't render through a camera that is not ours
                // Don't listen to audio through a listener that is not ours
                transform.GetChild(0).gameObject.SetActive(false);

                // Don't accept inputs from objects that are not ours
                GetComponent<FirstPersonController>().enabled = false;

                // There is no reason to try and simulate physics since 
                // the position is being sent across the network anyway
                Destroy(GetComponent<Rigidbody>());

                //Stop here so we dont change other players materials or name after an RPC call
                return;
            }

            SetInitialState();
            SetTeam(team);
        }

        #region Enable/Disable
        private void OnEnable()
        {
            eventMaster = FindObjectOfType<Event_Master>();
            eventMaster.ToggleMenuEvent += ToggleInputs;
        }

        private void OnDisable()
        {
            eventMaster.ToggleMenuEvent -= ToggleInputs;
        }
        #endregion

        private void SetInitialState()
        {
            oldWalkSpeed = myController.m_WalkSpeed;
            oldRunSpeed = myController.m_RunSpeed;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update()
        {
            if (!networkObject.IsOwner)
            {
                transform.position = networkObject.position;
                transform.rotation = networkObject.rotation;

                return;
            }

            networkObject.position = transform.position;
            networkObject.rotation = transform.rotation;
        }

        public void SetTeam(int team)
        {
            networkObject.SendRpc(RPC_SET_MATERIAL, Receivers.AllBuffered, team);
        }

        void ToggleInputs()
        {
            myController.menuOpen = !myController.menuOpen;
            if (myController.menuOpen)
            {
                myController.m_WalkSpeed = 0;
                myController.m_RunSpeed = 0;
            }
            else
            {
                myController.m_WalkSpeed = oldWalkSpeed;
                myController.m_RunSpeed = oldRunSpeed;
            }
        }

        public override void setMaterial(RpcArgs args)
        {
            team = args.GetNext<int>();
            myRenderer.material = teamMats[team];
        }
    }
}