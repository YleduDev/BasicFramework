using UnityEngine;

namespace Framework
{
    public class TargetFPS : MonoBehaviour
    {
        public int frameRate = 60;

        void Start()
        {
#if !UNITY_EDITOR
            Application.targetFrameRate = frameRate;
#endif
        }

    }
}
