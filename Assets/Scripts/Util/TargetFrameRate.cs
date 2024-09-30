using UnityEngine;

namespace Util
{
    public class TargetFrameRate : MonoBehaviour
    {
        private void Awake()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }
    }
}
