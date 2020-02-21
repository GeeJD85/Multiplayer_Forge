using UnityEngine;

namespace GW.Multi
{
    public class Event_Master : MonoBehaviour
    {
        public void ExitGame()
        {
            Application.Quit();
            Debug.Log("ExitCalled");
        }

        public delegate void MenuEvent();
        public event MenuEvent ToggleMenuEvent;

        public void CallEventToggleMenu()
        {
            if (ToggleMenuEvent != null)
                ToggleMenuEvent();
        }

        
    }
}