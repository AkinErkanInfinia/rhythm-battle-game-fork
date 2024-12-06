using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gameplay;
using Gameplay.LevelMechanics;
using InfiniaGamingCore.XMS;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> playerPrefabs;
        [SerializeField] private List<TeamScoreHolderSO> teamScoreHolders;
        [SerializeField] private List<GameObject> footImgs;
        [SerializeField] private GameObject endgameCanvas;
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

        [Space(10)]
        [Header("Round End Popup")]
        public RectTransform roundEndContent;
        public TextMeshProUGUI[] roundEndLevelTexts;

        public static List<Circle> SpawnedCircles;
        public static List<GameObject> GreenSpawners;
        public static List<GameObject> BlueSpawners;

        private List<GameObject> players = new List<GameObject>();

        private int _round;
        private bool _isRoundStarted;

        private int _playerCounter;

        private void Awake()
        {
            Application.runInBackground = true;
            Timer.TimeIsUp += OnTimeIsUp;
        }

        private void Start()
        {

            SpawnedCircles = new List<Circle>();
            GreenSpawners = new List<GameObject>();
            BlueSpawners = new List<GameObject>();

            InitTeams();
            
            MessageBroker.Default.Receive<PlayReceivedMessage>().Subscribe(OnPlayReceived).AddTo(this);
            MessageBroker.Default.Receive<PlayerReceivedMessage>().Subscribe(OnPlayerNameReceived).AddTo(this);
            MessageBroker.Default.Receive<CountDownUIMessage>().Subscribe(OnCountdownReceived).AddTo(this);
        }

        private void InitTeams()
        {
            blueTeam.SetScoreHolder(teamScoreHolders[0]);
            greenTeam.SetScoreHolder(teamScoreHolders[1]);
        }

        private void OnPlayerNameReceived(PlayerReceivedMessage message)
        {
            if (_playerCounter >= playerPrefabs.Count)
                return;

            footImgs[_playerCounter].SetActive(true);
            teamScoreHolders[_playerCounter % 2].AddPlayer(message.playerName);
            GameObject go = Instantiate(playerPrefabs[_playerCounter], playersCanvas.transform);
            players.Add(go);
            _playerCounter++;
        }

        private void OnCountdownReceived(CountDownUIMessage message)
        {
            timer.StartTimer(message.countDownTime, TimerType.GetReady, startScreenCountdown);
        }

        private void OnPlayReceived(PlayReceivedMessage message)
        {
            timer.StopTimer();
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
            timer.StartTimer(GameConfigReader.Instance.data.timeBetweenRounds, TimerType.RoundEnd,timerText);
            
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
            playersCanvas.SetActive(true);
            timer.StartTimer(GameConfigReader.Instance.data.roundDuration, TimerType.RoundEnd, timerText);
            
            //UIAnimations.PopupDissolveOut(startScreenBackground, startScreenContent, 1f);
            footImgs.ForEach(foot => foot.SetActive(false));
            startScreenBackground.SetActive(false);
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
            endgameCanvas.SetActive(true);

            TeamScoreHolderSO winnerTeam;

            if (teamScoreHolders[0].GetScore() > teamScoreHolders[1].GetScore())
                winnerTeam = teamScoreHolders[0];
            else
                winnerTeam = teamScoreHolders[1];
            
            //MessageBroker.Default.Publish();

            //startScreenCountdown.text = winnerTeam.GetNames()[0] + "\n" + winnerTeam.GetNames()[1];
            //startScreenCountdown.fontSize = 150f;
            //UIAnimations.PopupDissolveIn(startScreenBackground, startScreenContent, 1f);
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

        public List<GameObject> GetPlayers() => players;
    }
}
