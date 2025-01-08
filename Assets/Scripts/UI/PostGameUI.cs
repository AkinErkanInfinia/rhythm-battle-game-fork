using InfiniaGamingCore.XMS;
using Managers;
using Sirenix.Utilities;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

public class PostGameUI : MonoBehaviour
{
    [SerializeField] private List<TeamScoreHolderSO> teamScoreHolders;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject uiToActivate;

    [SerializeField] private TextMeshProUGUI[] team1Texts;
    [SerializeField] private TextMeshProUGUI team1Name_1;
    [SerializeField] private TextMeshProUGUI team1Name_2;
    [SerializeField] private TextMeshProUGUI team1Score;

    [SerializeField] private TextMeshProUGUI[] team2Texts;
    [SerializeField] private TextMeshProUGUI team2Name_1;
    [SerializeField] private TextMeshProUGUI team2Name_2;
    [SerializeField] private TextMeshProUGUI team2Score;

    [SerializeField] private TMP_FontAsset blueFont;
    [SerializeField] private TMP_FontAsset greenFont;

    [SerializeField] private GameObject[] objectsToClose;

    private void Start()
    {
        gameManager.OnGameEnd += OnGameEnd;
    }

    private void OnGameEnd()
    {
        objectsToClose.ForEach(obj => obj.SetActive(false));
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
        {
            team1Name_1.text = winner.GetNames()[0];
            team1Name_2.text = winner.GetNames()[1];
        }

        else
            team1Name_1.text = winner.GetNames()[0];

        team1Score.text = winner.GetScore().ToString();

        if (winner.GetSide() is Gameplay.PlayerSide.Blue)
        {
            team1Name_1.font = blueFont;
            team1Name_2.font = blueFont;
            team1Score.font = blueFont;
            team1Texts.ForEach(text => text.font = blueFont);

            team2Name_1.font = greenFont;
            team2Name_2.font = greenFont;
            team2Score.font = greenFont;
            team2Texts.ForEach(text => text.font = greenFont);
        }

        else
        {
            team1Name_1.font = greenFont;
            team1Name_2.font = greenFont;
            team1Score.font = greenFont;
            team1Texts.ForEach(text => text.font = greenFont);

            team2Name_1.font = blueFont;
            team2Name_2.font = blueFont;
            team2Score.font = blueFont;
            team2Texts.ForEach(text => text.font = blueFont);
        }


        if (second.GetNames().Count > 1)
        { 
            team2Name_1.text = second.GetNames()[0];
            team2Name_2.text = second.GetNames()[1];
        }

        else
            team2Name_1.text = second.GetNames()[0];

        team2Score.text = second.GetScore().ToString();
    }

    private void OnDestroy()
    {
        gameManager.OnGameEnd -= OnGameEnd;
    }
}