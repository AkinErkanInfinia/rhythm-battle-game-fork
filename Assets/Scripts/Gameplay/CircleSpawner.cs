using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gameplay
{
    public class CircleSpawner : MonoBehaviour
    {
        public Player player;
        public float defenceSeconds;
        
        private bool _isDefending;
        private Image _spawnerImage;

        private void Start()
        {
            _spawnerImage = GetComponent<Image>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_isDefending) { return; }

            if (other.TryGetComponent<Circle>(out var circle))
            {
                Destroy(circle.gameObject);
                player.CircleCollected();
            }
        }

        public void Activate()
        {
            switch (player.playerType)
            {
                case PlayerType.Attacker:
                    Attack();
                    break;
                case PlayerType.Defender:
                    Defend();
                    break;
            }
        }

        private void Attack()
        {
            var circle = Instantiate(player.circlePrefab, transform).GetComponent<Circle>();
            circle.transform.localPosition = Vector3.zero;
            circle.dir = player.GetDirectionVector();
        }

        private void Defend()
        {
            if (_isDefending) { return; }

            StartCoroutine(DefendCoroutine());
        }

        private IEnumerator DefendCoroutine()
        {
            _isDefending = true;
            _spawnerImage.color = Color.black;
            yield return new WaitForSeconds(defenceSeconds);
            _spawnerImage.color = Color.white;
            _isDefending = false;
        }
    }
    
}
