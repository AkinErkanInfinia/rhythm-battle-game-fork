using System;
using Managers;
using UnityEngine;

namespace Gameplay
{
    public class GameBoundCollider : MonoBehaviour
    {
        public Player targetPlayer;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<Circle>(out var circle))
            {
                GameManager.SpawnedCircles.Remove(circle.gameObject);
                Destroy(circle.gameObject);
                targetPlayer.CircleMissed();
            }
        }
    }
}
