using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TeamScoreHolderSO", menuName = "Scriptable Objects/Game/TeamScoreHolderSO")]
public class TeamScoreHolderSO : ScriptableObject
{
    private event Action<int> intEvent;

    private List<string> names;
    private int currentScore;
    private bool isDouble = false;

    private void OnEnable()
    {
        names = new List<string>();
        currentScore = 0;
    }

    public void Subscribe(Action<int> subscriber)
    {
        intEvent += subscriber;
    }

    public void Unsubscribe(Action<int> subscriber)
    {
        intEvent -= subscriber;
    }

    public void FireEvent(int value)
    {
        intEvent?.Invoke(value);
    }

    public void AddPlayer(string playerName)
    {
        names.Add(playerName);
    }

    public void AddScore(int score)
    {
        if (isDouble)
            currentScore += (score * 2);
        else
            currentScore += score;

        FireEvent(currentScore);
    }

    public int GetScore() => currentScore;
}