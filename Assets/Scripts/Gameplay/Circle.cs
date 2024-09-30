using System;
using Managers;
using UnityEngine;

namespace Gameplay
{
    public class Circle : MonoBehaviour
    {
        public Vector3 dir;
        public float speed;
        public SoundClip collectedSound;
        public SoundClip missedSound;
        public SoundClip spawnedSound;

        private void Start()
        {
            AudioManager.Instance.PlaySoundFXClip(spawnedSound, transform);
        }

        private void Update()
        {
            transform.position += dir * (speed * 100f * Time.deltaTime);
        }

        public void PlayCollectedSound()
        {
            AudioManager.Instance.PlaySoundFXClip(collectedSound, transform);
        }
        
        public void PlayMissedSound()
        {
            AudioManager.Instance.PlaySoundFXClip(missedSound, transform);
        }
    }
}
