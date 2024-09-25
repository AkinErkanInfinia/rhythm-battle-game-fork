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

        private int _round;
        private int _leftSideMissedCircles;
        private int _rightSideMissedCircles;
        
        private void Start()
        {
            Timer.TimeIsUp += OnTimeIsUp;
            
            NextRound();
        }

        private void OnTimeIsUp()
        {
            Debug.Log("Time is up!");
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

        private void NextRound()
        {
            _round++;
            timer.StartTimer(roundTime);
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

            if (_leftSideMissedCircles > _rightSideMissedCircles)
            {
                Debug.Log("Winner is right side");
            }
            else
            {
                Debug.Log("Winner is left side");
            }
        }
    }
}
