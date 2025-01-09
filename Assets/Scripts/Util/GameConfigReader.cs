using InfiniaGamingCore.XMS;
using System;
using System.IO;
using UniRx;
using UnityEngine;

namespace Util
{
    public class GameConfigReader : MonoBehaviour
    {
        public GameConfigData data;

        public int numberOfRounds;
        public float waitTime = 5f;

        public Action OnGameStart;
        
        public static GameConfigReader Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;

            //data = new GameConfigData
            //{
            //    circleCount = 9,
            //    circleScale = 1,
            //    roundDuration = (int)44,
            //    timeBetweenRounds = (int)5,
            //    circleCollisionPoint = 1,
            //    circleHitEnemyWeaponPoint = 3,
            //    missileDamagePoint = 1
            //};
        }

        private void Start()
        {
            MessageBroker.Default.Receive<PlayReceivedMessage>().Subscribe(CalculateTime).AddTo(this);
        }

        private void CalculateTime(PlayReceivedMessage msg)
        {
            float duration = msg.gameTime;
            int numberOfWaits = numberOfRounds - 1;
            float totalWaitTime = waitTime * numberOfWaits;

            float roundTime = (duration - totalWaitTime) / numberOfRounds;

            Debug.Log("Total Game Time: " + (roundTime* numberOfRounds + totalWaitTime));


            data = new GameConfigData
            {
                circleCount = 5,
                circleScale = 1.5f,
                roundDuration = (int)roundTime,
                timeBetweenRounds = (int)waitTime,
                circleCollisionPoint = 1,
                circleHitEnemyWeaponPoint = 3,
                missileDamagePoint = 1,
                gameDuration = (int)duration
            };
            Debug.Log("Game Started Time " + Time.time);
            OnGameStart?.Invoke();
        }

        //private void Awake()
        //{
        //    string filePath = Application.streamingAssetsPath + "/config.json";

        //    if (File.Exists(filePath))
        //    {
        //        ReadAllText(filePath);
        //    }
        //    else
        //    {
        //        Debug.LogError("File not found");
        //        CreateDefaultSettingsFile(filePath);
        //    }

        //    // Singleton
        //    if (Instance != null && Instance != this)
        //    {
        //        Destroy(this);
        //        return;
        //    }
        //    Instance = this;
        //}

        //void CreateDefaultSettingsFile(string filePath)
        //{
        //    data = new GameConfigData
        //    {
        //        circleCount = 9,
        //        circleScale = 1,
        //        roundDuration = 60,
        //        timeBetweenRounds = 5,
        //        circleCollisionPoint = 1,
        //        circleHitEnemyWeaponPoint = 3,
        //        missileDamagePoint = 1
        //    };

        //    string jsonContent = JsonUtility.ToJson(data, true);
        //    File.WriteAllText(filePath, jsonContent);
        //    Debug.Log("Default settings file created");
        //}

        //private void ReadAllText(string filePath)
        //{
        //    string jsonContent = File.ReadAllText(filePath);
        //    data = JsonUtility.FromJson<GameConfigData>(jsonContent);
        //}

        [Serializable]
        public class GameConfigData
        {
            public int circleCount;
            public float circleScale;
            public int roundDuration;
            public int timeBetweenRounds;
            public int circleCollisionPoint;
            public int circleHitEnemyWeaponPoint;
            public int missileDamagePoint;
            public int gameDuration;

        }
    }
}
