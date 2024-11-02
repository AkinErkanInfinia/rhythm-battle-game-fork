using M2MqttUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt.Messages;

public class XMS_Listener : M2MqttUnityClient
{
    private JSONReader reader;

    private static readonly string b_player_name = "/b_player_name";
    private static readonly string b_play = "/b_play";
    private static readonly string b_pause = "/b_pause";
    private static readonly string b_restart = "/b_restart";
    private static readonly string b_countdown = "/b_countdown";
    private static readonly string b_highscore = "/b_highscore";
    private static readonly string g_heartbeat = "/g_heartbeat";
    private static readonly string g_state = "/g_state";
    private static readonly string g_score = "/g_score";
    private static readonly string g_score_i = "/g_score_i";
    private static readonly string opt_state = "/opt/state";
    private static readonly string transaction = "/transaction/b_state";

    private string restartState = "1";


    public event Action<string> OnPlayerNameReceived;
    public event Action<int> OnPlayReceived;
    public event Action OnPauseReceived;
    public event Action OnRestartReceived;
    public event Action<int> OnCountdownReceived;
    public event Action<int> OnDifficultyReceived;
    public event Action<int> OnPlayerCountReceived;
    public event Action<int> OnTypeReceived;
    public event Action<int> OnTransactionReceived;

    public event Action<HighScores[]> OnHighScoreReceived;
    public event Action<HighScores[]> OnInternalScoreReceived;

    public float heartbeatInterval = 1.0f;
    public string gameName;

    public static XMS_Listener instance;

    protected override void Awake()
    {
        base.Awake();
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);  // Changed to destroy the entire GameObject
        }
    }

    protected override void Start()
    {
        reader = GetComponent<JSONReader>();
        brokerAddress = reader.GetBrokerAddress();
        brokerPort = reader.GetBrokerPort();
        heartbeatInterval = reader.GetHeartbeatInterval();
        Connect();
        StartCoroutine(Heartbeat());
    }

    protected override void Update()
    {
        base.Update();
    }


    IEnumerator Heartbeat()
    {
        while (true)
        {
            if (MQTTClientConnected)
            {
                SendMessage(reader.GetRoomName() + g_heartbeat, true);
            }
            yield return new WaitForSecondsRealtime(heartbeatInterval);
        }
    }

    protected override void SubscribeTopics()
    {
        string roomName = reader.GetRoomName();  // Optimized repeated method call
        client.Subscribe(new string[] { roomName + b_player_name }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
        client.Subscribe(new string[] { roomName + b_play }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
        client.Subscribe(new string[] { roomName + b_pause }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
        client.Subscribe(new string[] { roomName + b_restart }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
        client.Subscribe(new string[] { roomName + b_countdown }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
        client.Subscribe(new string[] { roomName + b_highscore }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
        client.Subscribe(new string[] { roomName + transaction }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
        client.Subscribe(new string[] { roomName + g_score_i }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
    }

    // State 
    // 0 => Ready, Ready to play
    // 1 => Countdown, Waiting for players to join
    // 2 => Playing, Game is running
    // 3 => Paused,  !! Does not work for now !!
    // 9 => Error, Error state
    public void SetGameState(int state)
    {
        SendMessage(reader.GetRoomName() + g_state, state);
    }

    public void SetHighScores(HighScores scores)
    {
        string json = JsonUtility.ToJson(scores);
        SendMessage(reader.GetRoomName() + g_score, json, false);
    }

    public void SetInternalScores(HighScores scores)
    {
        string json = JsonUtility.ToJson(scores);
        SendMessage(reader.GetRoomName() + g_score_i, json, false);
    }

    public void SendMessage(string topic, string message, bool retain = true)
    {
        client.Publish(topic, System.Text.Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, retain);
    }


    public void SendMessage(string topic, int message)
    {
        client.Publish(topic, System.Text.Encoding.UTF8.GetBytes(message.ToString()), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, true);
    }

    public void SendMessage(string topic, bool message)
    {
        client.Publish(topic, System.Text.Encoding.UTF8.GetBytes(message.ToString()), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, true);
    }

    public void SendRestartMessage()
    {
        SendMessage(reader.GetRoomName() + opt_state, restartState);
    }


    protected override void UnsubscribeTopics()
    {
        string roomName = reader.GetRoomName();  // Optimized repeated method call
        client.Unsubscribe(new string[] { roomName + b_player_name });
        client.Unsubscribe(new string[] { roomName + b_play });
        client.Unsubscribe(new string[] { roomName + b_pause });
        client.Unsubscribe(new string[] { roomName + b_restart });
        client.Unsubscribe(new string[] { roomName + b_countdown });
        client.Unsubscribe(new string[] { roomName + b_highscore });
        client.Unsubscribe(new string[] { roomName + transaction });
        client.Unsubscribe(new string[] { roomName + g_score_i });
    }

    protected override void OnConnectionFailed(string errorMessage)
    {
        Debug.LogError("CONNECTION FAILED! " + errorMessage);  // Changed to Debug.LogError for error message
        StartCoroutine(TryReconnect());
    }

    protected override void OnDisconnected()
    {
        Debug.LogWarning("DISCONNECTED.");  // Changed to Debug.LogWarning for warnings
        StartCoroutine(TryReconnect());
    }

    protected override void OnConnectionLost()
    {
        Debug.LogWarning("CONNECTION LOST!");  // Changed to Debug.LogWarning for warnings
        StartCoroutine(TryReconnect());
    }

    IEnumerator TryReconnect()
    {
        while (MQTTClientConnected)
        {
            yield return new WaitForSeconds(5);
        }

        Connect();
    }

    protected override void OnConnected()
    {
        base.OnConnected();
        string roomName = reader.GetRoomName();  // Optimized repeated method call
        SendMessage(roomName + g_state, 0);
    }

    protected override void DecodeMessage(string topic, byte[] message)
    {
        string msg = System.Text.Encoding.UTF8.GetString(message);

        if (topic.EndsWith(b_player_name))
        {
            OnPlayerNameReceived?.Invoke(msg);
        }
        else if (topic.EndsWith(b_play))
        {
            if (int.TryParse(msg, out int play))
            {
                OnPlayReceived?.Invoke(play);
            }
            else
            {
                Debug.LogError("Failed to parse play: " + msg);
            }
        }
        else if (topic.EndsWith(b_pause))
        {
            OnPauseReceived?.Invoke();
        }
        else if (topic.EndsWith(b_restart))
        {
            OnRestartReceived?.Invoke();
        }
        else if (topic.EndsWith(b_countdown))
        {
            if (int.TryParse(msg, out int countdown))
            {
                OnCountdownReceived?.Invoke(countdown);
            }
            else
            {
                Debug.LogError("Failed to parse countdown: " + msg);
            }
        }
        else if (topic.EndsWith(b_highscore))
        {
            try
            {
                HighScores[] highScoresArray = JsonUtility.FromJson<WrapperClass>("{\"scores\":" + msg + "}").scores;
                OnHighScoreReceived?.Invoke(highScoresArray);
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to parse high scores: " + ex.Message);
            }
        }
        else if (topic.EndsWith(transaction))
        {
            if (int.TryParse(msg, out int transaction))
            {
                OnTransactionReceived?.Invoke(transaction);
            }
            else
            {
                Debug.LogError("Failed to parse transaction: " + msg);
            }
        }
        else if (topic.EndsWith(g_score_i))
        {
            try
            {
                HighScores[] highScoresArray = JsonUtility.FromJson<WrapperClass>(msg).scores;
                OnInternalScoreReceived?.Invoke(highScoresArray);
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to parse high scores: " + ex.Message);
            }
        }
    }

    private void OnDestroy()
    {
        Disconnect();
    }

    public struct EventMessage
    {
        public string message;
        public string topic;
    }

    [System.Serializable]
    public class WrapperClass
    {
        public HighScores[] scores;
    }

    [System.Serializable]
    public class HighScores
    {
        public string mode = "";
        public string difficulty = "";
        public Score[] scores;
    }

    [System.Serializable]
    public class Score
    {
        public int score;
        public string username;
    }


    [ContextMenu("HighScore")]
    public void HighScoreFunc()
    {
        HighScores scores = new HighScores();
        scores.scores = new Score[]
        {
            new Score { username = "test", score = 100 },
            new Score { username = "test::2", score = 200 },
            new Score { username = "test::3", score = 300 },
            new Score { username = "test::4", score = 400 },
            new Score { username = "test::5", score = 500 },
            new Score { username = "test::6", score = 600 },
        };

        SetHighScores(scores);
    }

    [ContextMenu("InternalScore")]
    public void InternalScoreFunc()
    {
        HighScores scores = new HighScores();
        scores.scores = new Score[]
        {
            new Score { username = "test", score = 100 },
            new Score { username = "test::2", score = 200 },
            new Score { username = "test::3", score = 300 },
            new Score { username = "test::4", score = 400 },
            new Score { username = "test::5", score = 500 },
            new Score { username = "test::6", score = 600 },
        };

        SetInternalScores(scores);
    }
}
