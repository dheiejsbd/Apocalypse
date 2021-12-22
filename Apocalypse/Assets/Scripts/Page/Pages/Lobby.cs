using UnityEditor;
using UnityEngine;
using FrameWork.Page;
namespace Apocalypse
{
    public class Lobby : IPage
    {
        public int ID => (int)PageID.Lobby;
        BlackBoard blackBoard;
        public void Initialize(BlackBoard blackBoard)
        {
            this.blackBoard = blackBoard;
        }

        public void Prepare()
        {
        }
        public void Enter()
        {
        }
        public void Update()
        {
        }
        public void LateUpdate()
        {
        }
        public void Exit()
        {
        }

        public void Terminate()
        {
        }
    }
}