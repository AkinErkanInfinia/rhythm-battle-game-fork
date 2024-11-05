using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Gameplay;
using Gameplay.LevelMechanics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("Settings")]
        public int collisionPoint;
        public int normalDamagePoint;
        public int missileDamagePoint;
        public int inactivityPenaltyPoint;
        
        [Space(10)]
        [Header("References")]
        public GameConfigReader gameConfigReader;
        public GameObject redSpawnerParent;
        public GameObject blueSpawnerParent;
        public GameObject redSpawnerPrefab;
        public GameObject blueSpawnerPrefab;
        public Timer timer;
        public GameObject playersCanvas;
        public TextMeshProUGUI timerText;
        public LevelMechanicBase[] mechanics;
        public Team redTeam;
        public Team blueTeam;

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
        public static List<GameObject> RedSpawners;
        public static List<GameObject> BlueSpawners;

        private int _round;
        private int _redTotalScore;
        private int _blueTotalScore;
        private bool _isRoundStarted;
        
        private void Start()
        {
            Timer.TimeIsUp += OnTimeIsUp;
            GameBoundCollider.NormalDamageTaken += OnNormalDamageTaken;
            GameBoundCollider.MissileDamageTaken += OnMissileDamageTaken;
            Team.InactivityActivated += OnInactivityActivated;
            Circle.CirclesCollided += OnCirclesCollided;

            SpawnedCircles = new List<GameObject>();
            RedSpawners = new List<GameObject>();
            BlueSpawners = new List<GameObject>();
            
            CreateSpawners();
            NextRound();
        }

        private void Update()
        {
            if (_isRoundStarted)
            {
                CheckMechanicsLoop(timer.GetCurrentGameTime());
            }
        }

        private void CreateSpawners()
        {
            for (int i = 0; i < gameConfigReader.data.circleCount; i++)
            {
                var redSpawner = Instantiate(redSpawnerPrefab, redSpawnerParent.transform);
                var blueSpawner = Instantiate(blueSpawnerPrefab, blueSpawnerParent.transform);

                redSpawner.transform.localScale = Vector3.one * gameConfigReader.data.circleScale;
                blueSpawner.transform.localScale = Vector3.one * gameConfigReader.data.circleScale;
                
                redSpawner.GetComponent<CircleSpawner>().team = redTeam;
                blueSpawner.GetComponent<CircleSpawner>().team = blueTeam;
                
                RedSpawners.Add(redSpawner);
                BlueSpawners.Add(blueSpawner);
            }
        }

        private void OnCirclesCollided(Team sender)
        {
            sender.AddScore(collisionPoint);
        }

        private void OnInactivityActivated(Team sender)
        {
            sender.DecreaseScore(inactivityPenaltyPoint);
        }

        private void OnNormalDamageTaken(Team sender)
        {
            GetOpponentOf(sender).AddScore(normalDamagePoint);
        }

        private void OnMissileDamageTaken(Team sender)
        {
            GetOpponentOf(sender).AddScore(missileDamagePoint);
        }

        private Team GetOpponentOf(Team team)
        {
            return team.playerSide == PlayerSide.Blue ? redTeam : blueTeam;
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
            if (_round == 5) { FinishGame(); }
            else { NextRound(); }
        }

        private async void NextRound()
        {
            _round++;
            _isRoundStarted = false;
            
            var title = _round == 1 ? "Get Ready!" : "Next Round!";
            popupTitle.text = title;
            UIAnimations.PopupFadeIn(popupObject.GetComponent<Image>(), popupBg, 1f);

            ClearAllCirclesOnTheBoard();
            playersCanvas.SetActive(false);
            timer.StartTimer(gameConfigReader.data.timeBetweenRounds, popupCountdown, TimerType.GetReady);
            
            await UniTask.WaitForSeconds(gameConfigReader.data.timeBetweenRounds);
            
            UIAnimations.PopupFadeOut(popupObject.GetComponent<Image>(), popupBg, 1f);
            playersCanvas.SetActive(true);
            timer.StartTimer(gameConfigReader.data.roundDuration, timerText, TimerType.RoundEnd);
            
            ResetAttackTime();
            _isRoundStarted = true;
        }

        private void CheckMechanicsLoop(int time)
        {
            foreach (var mechanic in mechanics)
            {
                if (mechanic.activateMechanicAfterRound <= _round)
                {
                    mechanic.MechanicLoop(time);
                }
            }
        }

        private void ResetAttackTime()
        {
            redTeam.lastAttackTime = Time.time;
            blueTeam.lastAttackTime = Time.time;
        }

        private void FinishGame()
        {
            ClearAllCirclesOnTheBoard();

            _redTotalScore += redTeam.totalScore;
            _blueTotalScore += blueTeam.totalScore;
            
            redTeam.isGameFinished = true;
            blueTeam.isGameFinished = true;

            var winner = _blueTotalScore > _redTotalScore
                ? "Winner is <#0041FF>BLUE</color>"
                : "Winner is <#FF000C>RED</color>";
            if (_blueTotalScore == _redTotalScore) { winner = "Tie!"; }
            
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
