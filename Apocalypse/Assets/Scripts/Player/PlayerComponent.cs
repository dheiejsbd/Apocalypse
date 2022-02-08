using System;
using System.Collections.Generic;
using UnityEngine;

namespace Apocalypse.Player
{
    public class PlayerComponent : MonoBehaviour
    {
        protected Player Player
        {
            get
            {
                if(player ==null)
                {
                    player = GetComponent<Player>();
                }
                if(player ==null)
                {
                    player = GetComponentInParent<Player>();
                }
                return player;
            }
        }
        private Player player;
    }
}
