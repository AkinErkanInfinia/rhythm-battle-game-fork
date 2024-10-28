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
        public GameObject destroyedVFX;

        private void Start()
        {
            AudioManager.Instance.PlaySoundFXClip(spawnedSound, transform);
        }

        private void Update()
        {
            transform.position += dir * (speed * 100f * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Circle")) return;
            
            var particle = Instantiate(destroyedVFX, gameObject.transform.position, Quaternion.identity);
            Destroy(particle, 1.5f);
            Destroy(gameObject);
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
