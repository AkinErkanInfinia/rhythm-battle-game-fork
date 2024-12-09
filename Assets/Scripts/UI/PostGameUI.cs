using InfiniaGamingCore.XMS;
using Managers;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

public class PostGameUI : MonoBehaviour
{
    [SerializeField] private List<TeamScoreHolderSO> teamScoreHolders;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject uiToActivate;

    [SerializeField] private TextMeshProUGUI bestScoreName;
    [SerializeField] private TextMeshProUGUI bestScore;

    [SerializeField] private TextMeshProUGUI team1Names;
    [SerializeField] private TextMeshProUGUI team1Score;

    [SerializeField] private TextMeshProUGUI team2Names;
    [SerializeField] private TextMeshProUGUI team2Score;

    [SerializeField] private TMP_FontAsset blueFont;
    [SerializeField] private TMP_FontAsset greenFont;

    private void Start()
    {
        gameManager.OnGameEnd += OnGameEnd;
        MessageBroker.Default.Receive<HighScoreReceivedMessage>().Subscribe(OnHighScoreReceived).AddTo(this);
    }

    private void OnHighScoreReceived(HighScoreReceivedMessage message)
    {
        bestScoreName.text = message.highScore.username;
        bestScore.text = message.highScore.score.ToString();
    }

    private void OnGameEnd()
    {
        uiToActivate.SetActive(true);

        TeamScoreHolderSO winner;
        TeamScoreHolderSO second;

        if (teamScoreHolders[0].GetScore() > teamScoreHolders[1].GetScore())
        {
            winner = teamScoreHolders[0];
            second = teamScoreHolders[1];
        }

        else
        {
            winner = teamScoreHolders[1];
            second = teamScoreHolders[0];
        }

        if (winner.GetNames().Count > 1)
            team1Names.text = winner.GetNames()[0] + "-" + winner.GetNames()[1];
        else
            team1Names.text = winner.GetNames()[0];

        team1Score.text = winner.GetScore().ToString();

        if (winner.GetSide() is Gameplay.PlayerSide.Blue)
        {
            team1Names.font = blueFont;
            team1Score.font = blueFont;

            team2Names.font = greenFont;
            team2Score.font = greenFont;
        }

        else
        {
            team1Names.font = greenFont;
            team1Score.font = greenFont;

            team2Names.font = blueFont;
            team2Score.font = blueFont;
        }


        if (second.GetNames().Count > 1)
            team2Names.text = second.GetNames()[0] + "-" + second.GetNames()[1];
        else
            team2Names.text = second.GetNames()[0];

        team2Score.text = second.GetScore().ToString();
    }

    private void OnDestroy()
    {
        gameManager.OnGameEnd -= OnGameEnd;
    }
}