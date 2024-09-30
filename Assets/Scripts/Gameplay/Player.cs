using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Util;

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
        public SoundClip inactivitySound;
        [HideInInspector] public bool isGameFinished;

        [Header("Inactivity")]
        [HideInInspector] public float lastAttackTime;
        public TextMeshProUGUI inactivityText;
        public int inactivityDamage;
        public float inactivityLimit;
        private bool _isInactive;

        private void Update()
        {
            if (playerType != PlayerType.Attacker || isGameFinished) { return; }
            
            _isInactive = Time.time - lastAttackTime > inactivityLimit;
            
            if (_isInactive)
            {
                AudioManager.Instance.PlaySoundFXClip(inactivitySound, transform);
                CircleMissed(inactivityDamage);
                UIAnimations.InactivityTextAnimation(inactivityText);
                lastAttackTime = Time.time;
            }
        }

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

        public void CircleMissed(int count = 1)
        {
            missedCircleCount += count;
            missedCircleText.text = missedCircleCount.ToString();
        }

        public void CircleCollected()
        {
            collectedCircleCount++;
            collectedCircleText.text = collectedCircleCount.ToString();
        }
    }
    
}
