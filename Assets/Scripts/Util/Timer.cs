using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Cysharp.Threading.Tasks;
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
        private TimerType _currentTimerType;

        public static event Action<TimerType> TimeIsUp;
        
        private Coroutine _timerCoroutine;

        public void StartTimer(float startFrom, TimerType type, TextMeshProUGUI targetText)
        {
            _timerText = targetText;
            _timeRemaining = startFrom;
            _currentTimerType = type;
            if(_timerCoroutine != null)
                StopCoroutine(_timerCoroutine);
            _timerCoroutine = StartCoroutine(StartTimerCoroutine(startFrom, type, targetText));
        }
        
        private IEnumerator StartTimerCoroutine(float startFrom, TimerType type, TextMeshProUGUI targetText)
        {
            _timerText = targetText;
            _timeRemaining = startFrom;
            _currentTimerType = type;
            for (int i = 0; i < startFrom; i++)
            {
                _timerText.text = (startFrom - i).ToString();
                yield return new WaitForSeconds(1);
            }
            TimeIsUp?.Invoke(type);
        }
        
        
        public void StopTimer()
        {
            if(_timerCoroutine != null)
                StopCoroutine(_timerCoroutine);
            _timeRemaining = 0;
        }

        public int GetCurrentGameTime()
        {
            if (_currentTimerType != TimerType.RoundEnd) { return 0; }
            
            return Mathf.FloorToInt(_timeRemaining) + 1;
        }
    }
}
