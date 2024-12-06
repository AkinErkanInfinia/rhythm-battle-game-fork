using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    [SerializeField] private TeamScoreHolderSO teamScoreHolder;
    [SerializeField] private List<TextMeshProUGUI> playerNames;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject crown;

    [SerializeField] private TeamScoreHolderSO enemyTeam;

    private int playerCount = 0;

    private void Start()
    {
        RegisterToEvents();
    }

    private void RegisterToEvents()
    {
        teamScoreHolder.SubscribeToScoreEvent(OnNewScore);
        teamScoreHolder.SubscribeToNameEvent(OnNewPlayer);
    }

    private void OnNewPlayer(string playerName)
    {
        playerNames[playerCount].gameObject.SetActive(true);
        playerNames[playerCount].text = playerName;

        playerCount++;
    }

    private void OnNewScore(int score)
    {
        scoreText.text = score.ToString();

        if (score > enemyTeam.GetScore())
            crown.SetActive(true);
        else crown.SetActive(false);
    }

    private void OnDestroy()
    {
        UnregisterFromEvents();
    }

    private void UnregisterFromEvents()
    {
        teamScoreHolder.UnsubscribeFromScoreEvent(OnNewScore);
        teamScoreHolder.UnsubscribeFromNameEvent(OnNewPlayer);
    }
}