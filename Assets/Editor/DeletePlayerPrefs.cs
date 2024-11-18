using UnityEditor;
using UnityEngine;

namespace Util
{
    public class DeletePlayerPrefs : MonoBehaviour
    {
        [MenuItem("Edit/Reset Playerprefs")]
        public static void DeletePrefs()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
