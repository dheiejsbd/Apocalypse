using System;
using System.Collections.Generic;
using UnityEngine;
namespace FrameWork.Monster
{
    public class StateMachine
    {
        Dictionary<int, IState> States;
        IState activeState;
        int LastState;
        GameObject owner;
        public virtual void Initialize(GameObject owner)
        {
            this.owner = owner;
            States = new Dictionary<int, IState>();

        }

        public virtual void Update()
        {
            activeState.Update();
        }

        public virtual void Terminate()
        {

        }




        public void TryAddState(IState state)
        {
            Debug.Log(this + " TryAddState : " + state.ID);
            if (States.ContainsKey(state.ID))
            {
                Debug.LogError(this + " already has : " + state.ID);
                return;
            }
            AddState(state);
        }
        void AddState(IState state)
        {
            Debug.Log(this + " Get State : " + state.ID);
            state.Initialize(owner);
            States.Add(state.ID, state);
            if (activeState == null) TryChangeState(state.ID);
        }
        public void TryRemoveState(int ID)
        {
            Debug.Log(this + "TryRemoveState : " + ID);
            if(activeState.ID == ID)
            {
                Debug.LogError(this + " running State : " + ID);
                return;
            }
            if (!States.ContainsKey(ID))
            {
                Debug.LogError(this + "Don't has State : " + ID);
                return;
            }
            RemoveState(ID);
        }
        void RemoveState(int ID)
        {
            Debug.Log(this + " Remove State : " + ID);
            States[ID].Terminate();
            States.Remove(ID);
        }

        public void TryChangeState(int ID)
        {
            Debug.Log(this + "TrySwitchState : " + ID);
            //tnwjd
            SwitchState(ID);
        }
        void SwitchState(int ID)
        {
            Debug.Log(this + "Swich State : " + activeState + " -> " + ID);

            LastState = activeState != null? activeState.ID:ID;
            activeState?.Exit();
            activeState = States[ID];
            activeState.Enter();
        }


    }
}
