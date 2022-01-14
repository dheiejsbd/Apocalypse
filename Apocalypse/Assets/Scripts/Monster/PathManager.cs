using UnityEditor;
using UnityEngine;

namespace Apocalypse
{
    public class PathManager : MonoBehaviour
    {
        static Transform[] Paths;
        public void Awake()
        {
            Paths = new Transform[1];
            Paths[0] = transform.GetChild(0);
        }
        public static Transform GetPath(Vector3 pos)
        {
            Transform tr = Paths[0];
            float dist = float.MaxValue;
            for (int i = 0; i < Paths.Length; i++)
            {
                if(dist > Vector3.Distance(pos, Paths[i].position))
                {
                    tr = Paths[i];
                }
            }
            return tr;
        }
    }
}