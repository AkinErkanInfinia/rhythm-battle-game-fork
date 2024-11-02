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
        public int totalScore;
        public TextMeshProUGUI scoreText;
        public SoundClip inactivitySound;
        [HideInInspector] public bool isGameFinished;

        [Header("Inactivity")]
        [HideInInspector] public float lastAttackTime;
        public TextMeshProUGUI inactivityText;
        public float inactivityLimit;
        private bool _isInactive;
        
        public static event Action<Player> InactivityActivated;

        private void Update()
        {
            if (playerType != PlayerType.Attacker || isGameFinished) { return; }
            
            _isInactive = Time.time - lastAttackTime > inactivityLimit;
            
            if (_isInactive)
            {
                AudioManager.Instance.PlaySoundFXClip(inactivitySound, transform);
                UIAnimations.InactivityTextAnimation(inactivityText);
                lastAttackTime = Time.time;
                
                InactivityActivated?.Invoke(this);
            }
        }

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
                PlayerSide.Red => Vector3.down,
                PlayerSide.Blue => Vector3.up,
                _ => Vector3.zero
            };
        }

        public void AddScore(int score)
        {
            totalScore += score;
            scoreText.text = totalScore.ToString();
        }

        public void DecreaseScore(int score)
        {
            totalScore -= score;
            if (totalScore < 0) { totalScore = 0; }
            scoreText.text = totalScore.ToString();
        }
    }
    
}
