using System;
using Gameplay.LevelMechanics;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Gameplay
{
    public class GameBoundCollider : MonoBehaviour
    {
        public Team owner;
        public GameObject vfxHitRed;
        public GameObject vfxHitBlue;

        private Image _image;
        
        public static event Action<Team> NormalDamageTaken;
        public static event Action<Team> MissileDamageTaken;

        private void Start()
        {
            _image = GetComponent<Image>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<Circle>(out var circle))
            {
                PlayHitVfx(other.ClosestPoint(transform.position));
                
                circle.PlayMissedSound();
                GameManager.SpawnedCircles.Remove(circle.gameObject);
                Destroy(circle.gameObject);
                UIAnimations.MissedCircleEffect(_image, 0.25f);
                
                NormalDamageTaken?.Invoke(owner);
            }

            if (other.TryGetComponent<Missile>(out var missile))
            {
                UIAnimations.MissedCircleEffect(_image, 0.25f);
                
                MissileDamageTaken?.Invoke(owner);
            }
        }

        private void PlayHitVfx(Vector3 position)
        {
            var prefab = owner.playerSide == PlayerSide.Blue ? vfxHitBlue : vfxHitRed;
            var particle = Instantiate(prefab, position, Quaternion.identity);
            Destroy(particle, 1f);
        }
    }
}
