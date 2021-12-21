using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Apocalypse
{
    public class PageManager : FrameWork.Page.PageManager
    {
        public override void Initialize()
        {
            base.Initialize();
            TryAddPage(new Lobby());
            TryAddPage(new Ingame());
        }

        public override void Terminate()
        {
            base.Terminate();
        }
    }

}