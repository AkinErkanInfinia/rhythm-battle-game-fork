using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Gameplay
{
    public class GameBoundCollider : MonoBehaviour
    {
        public Player targetPlayer;
        public GameObject vfxHitRed;
        public GameObject vfxHitBlue;

        private Image _image;

        private void Start()
        {
            _image = GetComponent<Image>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<Circle>(out var circle))
            {
                PlayHitVfx(other.ClosestPoint(transform.position));
                
                GameManager.SpawnedCircles.Remove(circle.gameObject);
                Destroy(circle.gameObject);
                targetPlayer.CircleMissed();
                UIAnimations.MissedCircleEffect(_image, 0.25f);
            }
        }

        private void PlayHitVfx(Vector3 position)
        {
            var prefab = targetPlayer.playerSide == PlayerSide.Blue ? vfxHitBlue : vfxHitRed;
            var particle = Instantiate(prefab, position, Quaternion.identity);
            Destroy(particle, 1f);
        }
    }
}
