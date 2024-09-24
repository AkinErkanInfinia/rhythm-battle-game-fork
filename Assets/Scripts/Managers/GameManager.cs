using System;
using Gameplay;
using UnityEngine;
using Util;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public Player[] players;
        public float roundTime;
        public Timer timer;

        private int _round = 1;
        private int _leftSideMissedCircles;
        private int _rightSideMissedCircles;
        
        private void Start()
        {
            Timer.TimeIsUp += OnTimeIsUp;
            
            timer.StartTimer(roundTime);
        }

        private void OnTimeIsUp()
        {
            Debug.Log("Time is up!");
            if (_round == 2) { FinishGame(); }
            else { _round++; }
        }

        private void FinishGame()
        {
            foreach (var player in players)
            {
                if (player.attackDirection == AttackDirection.Right)
                {
                    _leftSideMissedCircles += player.missedCircleCount;
                }
                else
                {
                    _rightSideMissedCircles += player.missedCircleCount;
                }
            }
            
            // compare points....
        }
    }
}
