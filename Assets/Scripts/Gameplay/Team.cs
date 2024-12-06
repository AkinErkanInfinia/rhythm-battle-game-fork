using TMPro;
using UnityEngine;

namespace Gameplay
{
    public enum PlayerSide
    {
        Blue,
        Green,
    }
    
    public class Team : MonoBehaviour
    {
        public GameObject circlePrefab;
        public PlayerSide playerSide;
        public TextMeshProUGUI[] scoreTexts;
        
        private TeamScoreHolderSO teamScoreHolder;

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
            teamScoreHolder.AddScore(score);

            foreach (var text in scoreTexts)
                text.text = teamScoreHolder.GetScore().ToString();
        }

        public void SetScoreHolder(TeamScoreHolderSO scoreHolder)
        {
            teamScoreHolder = scoreHolder;
        }
    }
}