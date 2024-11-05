using System;
using System.IO;
using UnityEngine;

namespace Util
{
    public class GameConfigReader : MonoBehaviour
    {
        public GameConfigData data;
        
        private void Awake()
        {
            string filePath = Application.streamingAssetsPath + "/config.json";
            
            if (File.Exists(filePath))
            {
                ReadAllText(filePath);
            }
            else
            {
                Debug.LogError("File not found");
                CreateDefaultSettingsFile(filePath);
            }
        }
        
        void CreateDefaultSettingsFile(string filePath)
        {
            data = new GameConfigData
            {
                circleCount = 9,
                circleScale = 1,
                roundDuration = 60,
                timeBetweenRounds = 5
            };

            string jsonContent = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, jsonContent);
            Debug.Log("Default settings file created");
        }
        
        private void ReadAllText(string filePath)
        {
            string jsonContent = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<GameConfigData>(jsonContent);
        }

        [Serializable]
        public class GameConfigData
        {
            public int circleCount;
            public float circleScale;
            public int roundDuration;
            public int timeBetweenRounds;
        }
    }
}
