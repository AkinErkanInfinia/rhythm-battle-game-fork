using System;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace Util
{
    public class Timer : MonoBehaviour
    {
        public TextMeshProUGUI timerText;
        
        private float _timeRemaining;
        private bool _isTimerRunning;

        public static event Action TimeIsUp;

        private void Update()
        {
            if (!_isTimerRunning) { return; }

            if (_timeRemaining > 0)
            {
                _timeRemaining -= Time.deltaTime;
                timerText.text = (Mathf.FloorToInt(_timeRemaining) + 1).ToString();
            }
            else
            {
                _timeRemaining = 0;
                _isTimerRunning = false;
                TimeIsUp?.Invoke();
            }
        }

        public void StartTimer(float startFrom)
        {
            _timeRemaining = startFrom;
            _isTimerRunning = true;
        }
    }
}
