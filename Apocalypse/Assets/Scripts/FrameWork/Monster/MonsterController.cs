using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Monster
{
    public class MonsterController : MonoBehaviour
    {
        private Dictionary<int, IAction> actions;
        IAction ActiveAction;


        public void Initialized()
        {
            
        }

        public void Update()
        {

        }

        public void TryAddAction(IAction action)
        {
            if(actions.ContainsKey(action.ID))
            {
                Debug.LogError("Fail Add Action : " + action.ID);
                return;
            }

            actions.Add(action.ID, action);
            action.Initialize();
        }
        
        public void TryChangeAction(int ID)
        {
            if (!actions.ContainsKey(ID)) return;
            if (ActiveAction.ID == ID) return;
            if (actions[ID].Priority > actions[ID].Priority) return;

            ChangeAction(ID);
        }

        void ChangeAction(int ID)
        {
            ActiveAction?.Exit();
            ActiveAction = actions[ID];
            ActiveAction.Enter();
        }
    }
}
