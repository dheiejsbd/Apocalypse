using UnityEditor;
using UnityEngine;

namespace FrameWork.Page
{
    public class BlackBoard
    {
        public string SceneName { get; private set; }
        public void SaveSceneName(string SceneName)
        {
            this.SceneName = SceneName;
        }
    }
}