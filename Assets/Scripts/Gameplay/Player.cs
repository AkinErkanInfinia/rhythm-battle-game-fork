using System;
using UnityEngine;

namespace Gameplay
{
    public enum PlayerType
    {
        Attacker,
        Defender
    }

    public enum AttackDirection
    {
        Right,
        Left
    }
    
    public class Player : MonoBehaviour
    {
        public GameObject circlePrefab;
        public PlayerType playerType;
        public AttackDirection attackDirection;
        public int missedCircleCount;
        public int collectedCircleCount;

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
            return attackDirection switch
            {
                AttackDirection.Left => Vector3.left,
                AttackDirection.Right => Vector3.right,
                _ => Vector3.zero
            };
        }

        public void CircleMissed()
        {
            missedCircleCount++;
        }

        public void CircleCollected()
        {
            collectedCircleCount++;
        }
    }
    
}
