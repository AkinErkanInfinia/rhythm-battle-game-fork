using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gameplay;
using Gameplay.LevelMechanics;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Util;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [Space(10)]
        [Header("References")]
        public GameObject greenSpawnerParent;
        public GameObject blueSpawnerParent;
        public GameObject greenCircleParent;
        public GameObject blueCircleParent;
        public GameObject greenSpawnerPrefab;
        public GameObject blueSpawnerPrefab;
        public Timer timer;
        public GameObject playersCanvas;
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI currentLevelText;
        public LevelMechanicBase[] mechanics;
        public Team greenTeam;
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
        public static List<GameObject> GreenSpawners;
        public static List<GameObject> BlueSpawners;

        private int _round;
        private int _greenTotalScore;
        private int _blueTotalScore;
        private bool _isRoundStarted;
        
        private void Start()
        {
            greenTeam.SetTeamName("Green Team");
            blueTeam.SetTeamName("Blue Team");
            
            Timer.TimeIsUp += OnTimeIsUp;

            SpawnedCircles = new List<Circle>();
            GreenSpawners = new List<GameObject>();
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
                var redSpawner = Instantiate(greenSpawnerPrefab, greenSpawnerParent.transform);
                var blueSpawner = Instantiate(blueSpawnerPrefab, blueSpawnerParent.transform);

                redSpawner.transform.localScale = Vector3.one * GameConfigReader.Instance.data.circleScale;
                blueSpawner.transform.localScale = Vector3.one * GameConfigReader.Instance.data.circleScale;
                
                redSpawner.GetComponent<CircleSpawner>().team = greenTeam;
                blueSpawner.GetComponent<CircleSpawner>().team = blueTeam;
                
                redSpawner.GetComponent<CircleSpawner>().circleParent = greenCircleParent;
                blueSpawner.GetComponent<CircleSpawner>().circleParent = blueCircleParent;
                
                GreenSpawners.Add(redSpawner);
                BlueSpawners.Add(blueSpawner);
            }
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
            Time.timeScale += 1f;
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

            _greenTotalScore += greenTeam.totalScore;
            _blueTotalScore += blueTeam.totalScore;

            var winner = _blueTotalScore > _greenTotalScore
                ? $"Winner is <#0041FF>{blueTeam.TeamName}</color>"
                : $"Winner is <#FF000C>{greenTeam.TeamName}</color>";
            if (_blueTotalScore == _greenTotalScore) { winner = "Tie!"; }
            
            startScreenCountdown.text = winner;
            startScreenCountdown.fontSize = 150f;
            UIAnimations.PopupDissolveIn(startScreenBackground, startScreenContent, 1f);
        }

        private void ClearAllCirclesOnTheBoard()
        {
            if (SpawnedCircles.Count == 0) { return; }
            
            foreach (var circle in SpawnedCircles)
            {
                if (!circle) { return; }
                
                Destroy(circle.gameObject);
            }
        }
    }
}
