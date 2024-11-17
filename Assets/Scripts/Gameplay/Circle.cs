using System;
using Coffee.UIExtensions;
using Managers;
using UnityEngine;
using Util;

namespace Gameplay
{
    public class Circle : MonoBehaviour
    {
        public float speed;
        public SoundClip missedSound;
        public SoundClip spawnedSound;
        public GameObject destroyedVFX;
        public ParticleSystem circleParticle;

        [HideInInspector] public Team sender;
        [HideInInspector] public Vector3 dir;
        
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
            if (other.CompareTag("Circle"))
            {
                sender.AddScore(GameConfigReader.Instance.data.circleCollisionPoint);
            }
            
            DestroyCircle();
        }

        public void DestroyCircle()
        {
            speed = 0;
            GetComponent<BoxCollider2D>().enabled = false;
            circleParticle.gameObject.SetActive(false);
            var particle = Instantiate(destroyedVFX, gameObject.transform.position, Quaternion.identity);
            GetComponent<UIDissolve>().Play();
            GameManager.SpawnedCircles.Remove(this);
            
            Destroy(particle, 1.5f);
            Destroy(gameObject, 1.5f);
        }
        
        public void PlayMissedSound()
        {
            AudioManager.Instance.PlaySoundFXClip(missedSound, transform);
        }
    }
}
