using System;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace Util
{
    public enum TimerType
    {
        RoundEnd,
        GetReady
    }
    
    public class Timer : MonoBehaviour
    {
        private TextMeshProUGUI _timerText;
        private float _timeRemaining;
        private bool _isTimerRunning;
        private TimerType _currentTimerType;

        public static event Action<TimerType> TimeIsUp;

        private void Update()
        {
            if (!_isTimerRunning) { return; }

            if (_timeRemaining > 0)
            {
                _timeRemaining -= Time.deltaTime;
                _timerText.text = (Mathf.FloorToInt(_timeRemaining) + 1).ToString();
            }
            else
            {
                _timeRemaining = 0;
                _isTimerRunning = false;
                TimeIsUp?.Invoke(_currentTimerType);
            }
        }

        public void StartTimer(float startFrom, TextMeshProUGUI targetText, TimerType type)
        {
            _timerText = targetText;
            _timeRemaining = startFrom;
            _isTimerRunning = true;
            _currentTimerType = type;
        }

        public int GetCurrentGameTime()
        {
            if (_currentTimerType != TimerType.RoundEnd) { return 0; }
            
            return Mathf.FloorToInt(_timeRemaining) + 1;
        }
    }
}
