using System;
using Coffee.UIExtensions;
using Managers;
using UnityEngine;

namespace Gameplay
{
    public class Circle : MonoBehaviour
    {
        public float speed;
        public SoundClip collectedSound;
        public SoundClip missedSound;
        public SoundClip spawnedSound;
        public GameObject destroyedVFX;
        public ParticleSystem circleParticle;

        [HideInInspector] public Team sender;
        [HideInInspector] public Vector3 dir;
        
        public static event Action<Team> CirclesCollided;

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

            DestroyCircle();

            CirclesCollided?.Invoke(sender);
        }

        public void DestroyCircle()
        {
            speed = 0;
            GetComponent<BoxCollider2D>().enabled = false;
            circleParticle.gameObject.SetActive(false);
            var particle = Instantiate(destroyedVFX, gameObject.transform.position, Quaternion.identity);
            GetComponent<UIDissolve>().Play();
            GameManager.SpawnedCircles.Remove(gameObject);
            
            Destroy(particle, 1.5f);
            Destroy(gameObject, 1.5f);
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
