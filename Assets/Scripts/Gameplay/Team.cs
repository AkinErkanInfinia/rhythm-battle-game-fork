using TMPro;
using UnityEngine;

namespace Gameplay
{
    public enum PlayerSide
    {
        Blue,
        Green,
        Orange,
        Purple
    }
    
    public class Team : MonoBehaviour
    {
        public GameObject circlePrefab;
        public PlayerSide playerSide;
        public int totalScore;
        public TextMeshProUGUI[] scoreTexts;
        public TextMeshProUGUI[] nameTexts;
        
        private string _teamName;
        
        public string TeamName { get; set; }

        public Vector3 GetDirectionVector()
        {
            return playerSide switch
            {
                PlayerSide.Green => Vector3.down,
                PlayerSide.Blue => Vector3.up,
                PlayerSide.Orange => Vector3.up,
                PlayerSide.Purple => Vector3.down,
                _ => Vector3.zero
            };
        }

        public void AddScore(int score)
        {
            totalScore += score;
            foreach (var text in scoreTexts)
                text.text = totalScore.ToString();
            
        }

        public void SetTeamName(string teamName)
        {
            TeamName = teamName;
            foreach (var nt in nameTexts)
                nt.text = teamName;
        }
    }
    
}
