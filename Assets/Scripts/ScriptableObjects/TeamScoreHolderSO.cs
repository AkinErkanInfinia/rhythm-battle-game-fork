using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TeamScoreHolderSO", menuName = "Scriptable Objects/Game/TeamScoreHolderSO")]
public class TeamScoreHolderSO : ScriptableObject
{
    private event Action<int> intEvent;
    private event Action<string> stringEvent;

    private List<string> names;
    private int currentScore;
    private bool isDouble = false;

    private void OnEnable()
    {
        names = new List<string>();
        currentScore = 0;
    }

    public void SubscribeToScoreEvent(Action<int> subscriber)
    {
        intEvent += subscriber;
    }

    public void UnsubscribeFromScoreEvent(Action<int> subscriber)
    {
        intEvent -= subscriber;
    }

    public void FireScoreEvent(int value)
    {
        intEvent?.Invoke(value);
    }

    public void SubscribeToNameEvent(Action<string> subscriber)
    {
        stringEvent += subscriber;
    }

    public void UnsubscribeFromNameEvent(Action<string> subscriber)
    {
        stringEvent -= subscriber;
    }

    public void FireNameEvent(string value)
    {
        stringEvent?.Invoke(value);
    }

    public void AddPlayer(string playerName)
    {
        names.Add(playerName);
        FireNameEvent(playerName);
    }

    public void AddScore(int score)
    {
        if (isDouble)
            currentScore += (score * 2);
        else
            currentScore += score;

        FireScoreEvent(currentScore);
    }

    public int GetScore() => currentScore;
    public List<string> GetNames() => names;
}