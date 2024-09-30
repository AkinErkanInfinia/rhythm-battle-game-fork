using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Managers
{
    public class SceneManager : MonoBehaviour
    {
        public GameObject tips;
        
        private string _selectedScene = "1v1_Scene";
        
        public void SceneLoad(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }

        public void SelectMode(string sceneName)
        {
            _selectedScene = sceneName;
        }

        public async void StartGame()
        {
            tips.SetActive(true);

            await UniTask.WaitForSeconds(5);
            
            SceneLoad(_selectedScene);
        }
    }
}
