using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay
{
    public enum PlayerType
    {
        Attacker,
        Defender
    }

    public enum PlayerSide
    {
        Blue,
        Red
    }
    
    public class Player : MonoBehaviour
    {
        public GameObject circlePrefab;
        public PlayerType playerType;
        public PlayerSide playerSide;
        public int missedCircleCount;
        public int collectedCircleCount;
        public TextMeshProUGUI missedCircleText;
        public TextMeshProUGUI collectedCircleText;

        // Add inactivity penalty
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<CircleSpawner>(out var spawner))
            {
                spawner.Activate();
            }
        }

        public Vector3 GetDirectionVector()
        {
            return playerSide switch
            {
                PlayerSide.Red => Vector3.right,
                PlayerSide.Blue => Vector3.left,
                _ => Vector3.zero
            };
        }

        public void CircleMissed()
        {
            missedCircleCount++;
            missedCircleText.text = missedCircleCount.ToString();
        }

        public void CircleCollected()
        {
            collectedCircleCount++;
            collectedCircleText.text = collectedCircleCount.ToString();
        }
    }
    
}
