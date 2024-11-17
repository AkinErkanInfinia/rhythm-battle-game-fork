using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Util;

namespace Gameplay
{
    public enum PlayerSide
    {
        Blue,
        Green
    }
    
    public class Team : MonoBehaviour
    {
        public GameObject circlePrefab;
        public PlayerSide playerSide;
        public int totalScore;
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI nameText;
        
        private string _teamName;
        
        public string TeamName { get; set; }

        public Vector3 GetDirectionVector()
        {
            return playerSide switch
            {
                PlayerSide.Green => Vector3.down,
                PlayerSide.Blue => Vector3.up,
                _ => Vector3.zero
            };
        }

        public void AddScore(int score)
        {
            totalScore += score;
            scoreText.text = totalScore.ToString();
        }

        public void SetTeamName(string teamName)
        {
            TeamName = teamName;
            nameText.text = teamName;
        }
    }
    
}
