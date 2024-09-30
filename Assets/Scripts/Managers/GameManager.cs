using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("Settings")]
        public float roundTime;
        public float betweenRoundsTime;
        
        [Space(10)]
        [Header("References")]
        public Player[] players;
        public Timer timer;
        public GameObject playersCanvas;
        public TextMeshProUGUI timerText;

        [Space(10)] [Header("Audio")] 
        public SoundClip prepareTimeFinished;
        public SoundClip roundTimeFinished;
        
        [Space(10)]
        [Header("Background")]
        public OffsetScrolling[] scrollers;
        public GameObject leftBgParticle;
        public GameObject rightBgParticle;
        
        [Space(10)]
        [Header("Countdown Screen Popup")]
        public GameObject popupObject;
        public RectTransform popupBg;
        public TextMeshProUGUI popupTitle;
        public TextMeshProUGUI popupCountdown;

        [Space(10)]
        [Header("End Screen Popup")]
        public GameObject endScreenPopup;
        public RectTransform endScreenBg;
        public TextMeshProUGUI endScreenTitle;

        public static List<GameObject> SpawnedCircles;

        private int _round;
        private int _leftSideMissedCircles;
        private int _rightSideMissedCircles;
        
        private void Start()
        {
            Timer.TimeIsUp += OnTimeIsUp;
            
            SpawnedCircles = new List<GameObject>();
            NextRound();
        }

        private void OnDestroy()
        {
            Timer.TimeIsUp -= OnTimeIsUp;
        }

        private void OnTimeIsUp(TimerType type)
        {
            if (type == TimerType.GetReady)
            {
                AudioManager.Instance.PlaySoundFXClip(prepareTimeFinished, transform);
                return;
            }
            
            AudioManager.Instance.PlaySoundFXClip(roundTimeFinished, transform);
            if (_round == 2)
            {
                FinishGame();
            }
            else
            {
                SwapRoles();
                NextRound();
            }
        }

        private async void NextRound()
        {
            _round++;
            
            var title = _round == 1 ? "Get Ready!" : "Next Round!";
            popupTitle.text = title;
            UIAnimations.PopupFadeIn(popupObject.GetComponent<Image>(), popupBg, 1f);

            ClearAllCirclesOnTheBoard();
            playersCanvas.SetActive(false);
            timer.StartTimer(betweenRoundsTime, popupCountdown, TimerType.GetReady);
            
            await UniTask.WaitForSeconds(betweenRoundsTime);
            
            UIAnimations.PopupFadeOut(popupObject.GetComponent<Image>(), popupBg, 1f);
            playersCanvas.SetActive(true);
            timer.StartTimer(roundTime, timerText, TimerType.RoundEnd);

            ResetAttackTime();
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
            ChangeScrollerDirection();
        }

        private void ResetAttackTime()
        {
            foreach (var player in players)
            {
                player.lastAttackTime = Time.time;
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

                player.isGameFinished = true;
            }

            var winner = _leftSideMissedCircles > _rightSideMissedCircles
                ? "Winner is <#0041FF>BLUE</color>"
                : "Winner is <#FF000C>RED</color>";
            if (_leftSideMissedCircles == _rightSideMissedCircles) { winner = "Tie!"; }
            
            endScreenTitle.text = winner;
            UIAnimations.PopupFadeIn(endScreenPopup.GetComponent<Image>(), endScreenBg, 1f);
        }

        private void ClearAllCirclesOnTheBoard()
        {
            if (SpawnedCircles.Count == 0) { return; }
            
            foreach (var circle in SpawnedCircles)
            {
                Destroy(circle);
            }
        }

        private void ChangeScrollerDirection()
        {
            foreach (var scroller in scrollers)
            {
                scroller.scrollSpeed *= -1;
            }
            leftBgParticle.SetActive(false);
            rightBgParticle.SetActive(true);
        }
    }
}
