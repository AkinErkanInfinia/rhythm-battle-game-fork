using System.IO;
using UnityEngine;

public class JSONReader : MonoBehaviour
{
    public GameData data;

    void Awake()
    {
        string filePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), "settings.json");

        if (File.Exists(filePath))
        {
            ReadAllText(filePath);
            Debug.Log($"Room Name: {data.room_name}, Broker Address: {data.brokerAddress}, Broker Port: {data.brokerPort}, Heartbeat Interval: {data.heartbeatInterval}");
        }
        else
        {
            Debug.LogError("File not found");
            CreateDefaultSettingsFile(filePath);
        }
    }

    private void ReadAllText(string filePath)
    {
        string jsonContent = File.ReadAllText(filePath);
        data = JsonUtility.FromJson<GameData>(jsonContent);
    }


    void CreateDefaultSettingsFile(string filePath)
    {
        data = new GameData
        {
            room_name = "room1",
            brokerAddress = "127.0.0.1",
            brokerPort = 1883,
            heartbeatInterval = 1,
        };

        string jsonContent = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, jsonContent);
        Debug.Log("Default settings file created");
    }

    public string GetRoomName()
    {
        return data.room_name;
    }

    public string GetBrokerAddress()
    {
        return data.brokerAddress;
    }

    public int GetBrokerPort()
    {
        return data.brokerPort;
    }

    public float GetHeartbeatInterval()
    {
        return data.heartbeatInterval;
    }

    public string GetRoomName(string gameName)
    {
        return data.room_name;
    }


    [System.Serializable]
    public class GameData
    {
        public string room_name;
        public string brokerAddress;
        public int brokerPort;
        public float heartbeatInterval;
    }
}