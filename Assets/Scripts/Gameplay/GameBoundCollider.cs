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
            
        }

        private void PlayHitVfx(Vector3 position)
        {
            var prefab = owner.playerSide == PlayerSide.Blue ? vfxHitBlue : vfxHitRed;
            var particle = Instantiate(prefab, position, Quaternion.identity);
            Destroy(particle, 1f);
        }
    }
}
