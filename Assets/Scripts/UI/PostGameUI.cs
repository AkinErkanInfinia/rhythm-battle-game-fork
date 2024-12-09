using Managers;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PostGameUI : MonoBehaviour
{
    [SerializeField] private List<TeamScoreHolderSO> teamScoreHolders;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject uiToActivate;

    [SerializeField] private TextMeshProUGUI winnerTeamName;
    [SerializeField] private TextMeshProUGUI winnerTeamScore;

    [SerializeField] private TextMeshProUGUI team1Names;
    [SerializeField] private TextMeshProUGUI team1Score;

    [SerializeField] private TextMeshProUGUI team2Names;
    [SerializeField] private TextMeshProUGUI team2Score;

    [SerializeField] private TMP_FontAsset blueFont;
    [SerializeField] private TMP_FontAsset greenFont;

    private void Start()
    {
        gameManager.OnGameEnd += OnGameEnd;
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
            winnerTeamName.text = winner.GetNames()[0] + "-" + winner.GetNames()[1];
        else
            winnerTeamName.text = winner.GetNames()[0];

        team1Names.text = winnerTeamName.text;

        winnerTeamScore.text = winner.GetScore().ToString();
        team1Score.text = winnerTeamScore.text;

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