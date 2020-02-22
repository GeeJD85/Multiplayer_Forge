using UnityEngine;

namespace GW.Multi
{
    public class Event_Master : MonoBehaviour
    {
        public delegate void GeneralEvent();
        public event GeneralEvent ToggleMenuEvent;

        public void CallEventToggleMenu()
        {
            if (ToggleMenuEvent != null)
                ToggleMenuEvent();
        }        
    }
}