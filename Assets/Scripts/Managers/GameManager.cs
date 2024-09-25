using System.Collections;
using System.Collections.Generic;
using Gameplay;
using TMPro;
using UnityEngine;
using Util;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public Player[] players;
        public float roundTime;
        public float betweenRoundsTime;
        public Timer timer;
        public GameObject playersCanvas;
        public GameObject popupObject;
        public TextMeshProUGUI popupTitle;
        public TextMeshProUGUI popupCountdown;
        public TextMeshProUGUI timerText;
        public GameObject endScreenPopup;
        public TextMeshProUGUI endScreenTitle;

        public static List<GameObject> SpawnedCircles;

        private int _round;
        private int _leftSideMissedCircles;
        private int _rightSideMissedCircles;
        
        private void Start()
        {
            Timer.TimeIsUp += OnTimeIsUp;

            SpawnedCircles = new List<GameObject>();
            StartCoroutine(NextRound());
        }

        private void OnTimeIsUp(TimerType type)
        {
            if (type == TimerType.GetReady) { return; }
            
            Debug.Log("Time is up!");
            if (_round == 2)
            {
                FinishGame();
            }
            else
            {
                SwapRoles();
                StartCoroutine(NextRound());
            }
        }

        private IEnumerator NextRound()
        {
            _round++;
            
            var title = _round == 1 ? "Get Ready!" : "Next Round!";
            popupObject.SetActive(true);
            popupTitle.text = title;
            
            ClearAllCirclesOnTheBoard();
            playersCanvas.SetActive(false);
            timer.StartTimer(betweenRoundsTime, popupCountdown, TimerType.GetReady);
            yield return new WaitForSeconds(betweenRoundsTime);
            popupObject.SetActive(false);
            playersCanvas.SetActive(true);
            
            timer.StartTimer(roundTime, timerText, TimerType.RoundEnd);
        }

        private void SwapRoles()
        {
            foreach (var player in players)
            {
                player.playerType = player.playerType switch
                {
                    PlayerType.Attacker => PlayerType.Defender,
                    PlayerType.Defender => PlayerType.Attacker,
                    _ => player.playerType
                };
            }
        }

        private void FinishGame()
        {
            ClearAllCirclesOnTheBoard();
            foreach (var player in players)
            {
                if (player.playerSide == PlayerSide.Red)
                {
                    _leftSideMissedCircles += player.missedCircleCount;
                }
                else
                {
                    _rightSideMissedCircles += player.missedCircleCount;
                }
            }

            var winner = _leftSideMissedCircles > _rightSideMissedCircles
                ? "Winner is <#0041FF>BLUE</color>"
                : "Winner is <#FF000C>RED</color>";
            if (_leftSideMissedCircles == _rightSideMissedCircles) { winner = "Tie!"; }
            
            endScreenPopup.SetActive(true);
            endScreenTitle.text = winner;
        }

        private void ClearAllCirclesOnTheBoard()
        {
            if (SpawnedCircles.Count == 0) { return; }
            
            foreach (var circle in SpawnedCircles)
            {
                Destroy(circle);
            }
        }
    }
}
