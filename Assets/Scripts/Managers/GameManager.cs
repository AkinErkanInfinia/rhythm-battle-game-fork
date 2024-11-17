using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gameplay;
using Gameplay.LevelMechanics;
using TMPro;
using UnityEngine;
using Util;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [Space(10)]
        [Header("References")]
        public GameObject redSpawnerParent;
        public GameObject blueSpawnerParent;
        public GameObject redSpawnerPrefab;
        public GameObject blueSpawnerPrefab;
        public Timer timer;
        public GameObject playersCanvas;
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI currentLevelText;
        public LevelMechanicBase[] mechanics;
        public Team redTeam;
        public Team blueTeam;

        [Space(10)] [Header("Audio")] 
        public SoundClip roundTimeFinished;
        
        [Space(10)]
        [Header("Game Start Popup")]
        public GameObject startScreenBackground;
        public RectTransform startScreenContent;
        public TextMeshProUGUI startScreenLevelText;
        public TextMeshProUGUI startScreenCountdown;
        public GameObject[] highScores;

        [Space(10)]
        [Header("Round End Popup")]
        public RectTransform roundEndContent;
        public TextMeshProUGUI[] roundEndLevelTexts;

        public static List<Circle> SpawnedCircles;
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

            SpawnedCircles = new List<Circle>();
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
            for (int i = 0; i < GameConfigReader.Instance.data.circleCount; i++)
            {
                var redSpawner = Instantiate(redSpawnerPrefab, redSpawnerParent.transform);
                var blueSpawner = Instantiate(blueSpawnerPrefab, blueSpawnerParent.transform);

                redSpawner.transform.localScale = Vector3.one * GameConfigReader.Instance.data.circleScale;
                blueSpawner.transform.localScale = Vector3.one * GameConfigReader.Instance.data.circleScale;
                
                redSpawner.GetComponent<CircleSpawner>().team = redTeam;
                blueSpawner.GetComponent<CircleSpawner>().team = blueTeam;
                
                RedSpawners.Add(redSpawner);
                BlueSpawners.Add(blueSpawner);
            }
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

            UpdateLevelTexts();
            ClearAllCirclesOnTheBoard();
            UIAnimations.PopupFadeIn(roundEndContent, 1f);
            playersCanvas.SetActive(false);
            timer.StartTimer(GameConfigReader.Instance.data.timeBetweenRounds, TimerType.RoundEnd);
            
            await UniTask.WaitForSeconds(GameConfigReader.Instance.data.timeBetweenRounds);
            
            UIAnimations.PopupFadeOut(roundEndContent, 1f);
            playersCanvas.SetActive(true);
            timer.StartTimer(GameConfigReader.Instance.data.roundDuration, TimerType.RoundEnd, timerText);
            
            _isRoundStarted = true;
        }

        private void UpdateLevelTexts()
        {
            roundEndLevelTexts[0].text = $"LEVEL {_round}";
            roundEndLevelTexts[1].text = $"LEVEL {_round}";
            currentLevelText.text = $"LVL {_round}";
        }

        private async void StartGame()
        {
            _round++;
            playersCanvas.SetActive(false);
            startScreenLevelText.text = $"LVL {_round}";
            timer.StartTimer(GameConfigReader.Instance.data.timeBetweenRounds, TimerType.RoundEnd, startScreenCountdown);
            
            await UniTask.WaitForSeconds(GameConfigReader.Instance.data.timeBetweenRounds);

            playersCanvas.SetActive(true);
            timer.StartTimer(GameConfigReader.Instance.data.roundDuration, TimerType.RoundEnd, timerText);
            
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
            
            // roundEndText.text = winner;
            // roundEndText.fontSize = 150f;
            // UIAnimations.PopupDissolveIn(roundEndBackground, roundEndContent, 1f);
        }

        private void ClearAllCirclesOnTheBoard()
        {
            if (SpawnedCircles.Count == 0) { return; }
            
            foreach (var circle in SpawnedCircles)
            {
                Destroy(circle.gameObject);
            }
        }
    }
}
