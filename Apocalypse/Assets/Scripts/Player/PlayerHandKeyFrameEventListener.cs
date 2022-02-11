using UnityEditor;
using UnityEngine;

namespace Apocalypse.Player
{
    public class PlayerHandKeyFrameEventListener : PlayerComponent
    {
        public void EnterMagazine()
        {
            Player.EnterMagazine.Send();
        }

        public void EndReload()
        {
            Player.EndReload.Send();
        }
    }
}