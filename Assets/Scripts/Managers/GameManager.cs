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
        [Header("Game Start Popup")]
        public GameObject startScreenBackground;
        public RectTransform startScreenContent;
        public TextMeshProUGUI startScreenLevelText;
        public TextMeshProUGUI startScreenCountdown;
        public GameObject[] highScores;

        [Space(10)]
        [Header("Round End Popup")]
        public GameObject roundEndBackground;
        public RectTransform roundEndContent;
        public TextMeshProUGUI roundEndText;
        public TextMeshProUGUI roundEndLevelText;
        public GameObject[] playerScores;

        public static List<GameObject> SpawnedCircles;
        public static List<GameObject> RedSpawners;
        public static List<GameObject> BlueSpawners;

        private int _round;
        private int _redTotalScore;
        private int _blueTotalScore;
        private bool _isRoundStarted;
        
        private void Start()
        {
            redTeam.SetTeamName("Red Team");
            blueTeam.SetTeamName("Blue Team");
            
            Timer.TimeIsUp += OnTimeIsUp;
            GameBoundCollider.NormalDamageTaken += OnNormalDamageTaken;
            GameBoundCollider.MissileDamageTaken += OnMissileDamageTaken;
            Team.InactivityActivated += OnInactivityActivated;
            Circle.CirclesCollided += OnCirclesCollided;

            SpawnedCircles = new List<GameObject>();
            RedSpawners = new List<GameObject>();
            BlueSpawners = new List<GameObject>();
            
            CreateSpawners();
            StartGame();
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
            AudioManager.Instance.PlaySoundFXClip(roundTimeFinished, transform);
            if (_round == 5) { IncreaseSpeed(); }
            if (_round == 6) { FinishGame(); }
            else { NextRound(); }
        }

        private void IncreaseSpeed()
        {
            Time.timeScale += 0.5f;
            _round++;
        }

        private async void NextRound()
        {
            _round++;
            _isRoundStarted = false;

            roundEndLevelText.text = $"LVL{_round}";
            UpdateTeamScores();
            ClearAllCirclesOnTheBoard();
            UIAnimations.PopupDissolveIn(roundEndBackground, roundEndContent, 1f);
            playersCanvas.SetActive(false);
            timer.StartTimer(gameConfigReader.data.timeBetweenRounds, roundEndText, TimerType.RoundEnd);
            
            await UniTask.WaitForSeconds(gameConfigReader.data.timeBetweenRounds);
            
            UIAnimations.PopupDissolveOut(roundEndBackground, roundEndContent, 1f);
            playersCanvas.SetActive(true);
            timer.StartTimer(gameConfigReader.data.roundDuration, timerText, TimerType.RoundEnd);
            
            _isRoundStarted = true;
        }

        private void UpdateTeamScores()
        {
            var mostScoredTeam = redTeam.totalScore >= blueTeam.totalScore ? redTeam : blueTeam;
            var leastScoredTeam = redTeam.totalScore < blueTeam.totalScore ? redTeam : blueTeam;
            
            playerScores[0].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = mostScoredTeam.TeamName;
            playerScores[0].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = mostScoredTeam.totalScore.ToString();
            
            playerScores[1].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = leastScoredTeam.TeamName;
            playerScores[1].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = leastScoredTeam.totalScore.ToString();
        }

        private async void StartGame()
        {
            _round++;
            playersCanvas.SetActive(false);
            startScreenLevelText.text = $"LVL{_round}";
            timer.StartTimer(gameConfigReader.data.timeBetweenRounds, startScreenCountdown, TimerType.RoundEnd);
            
            await UniTask.WaitForSeconds(gameConfigReader.data.timeBetweenRounds);

            playersCanvas.SetActive(true);
            timer.StartTimer(gameConfigReader.data.roundDuration, timerText, TimerType.RoundEnd);
            
            UIAnimations.PopupDissolveOut(startScreenBackground, startScreenContent, 1f);
            
            _isRoundStarted = true;
        }

        private void CheckMechanicsLoop(int time)
        {
            foreach (var mechanic in mechanics)
            {
                if (mechanic.activateMechanicAfterRound == _round)
                {
                    mechanic.MechanicLoop(time);
                }
            }
        }

        private void FinishGame()
        {
            Time.timeScale = 1;
            ClearAllCirclesOnTheBoard();
            playersCanvas.SetActive(false);

            _redTotalScore += redTeam.totalScore;
            _blueTotalScore += blueTeam.totalScore;

            var winner = _blueTotalScore > _redTotalScore
                ? $"Winner is <#0041FF>{blueTeam.TeamName}</color>"
                : $"Winner is <#FF000C>{redTeam.TeamName}</color>";
            if (_blueTotalScore == _redTotalScore) { winner = "Tie!"; }
            
            roundEndText.text = winner;
            roundEndText.fontSize = 150f;
            UIAnimations.PopupDissolveIn(roundEndBackground, roundEndContent, 1f);
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
