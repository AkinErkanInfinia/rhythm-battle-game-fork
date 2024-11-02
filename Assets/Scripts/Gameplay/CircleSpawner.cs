using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public class CircleSpawner : MonoBehaviour
    {
        public Player player;
        public float defenceSeconds;
        public GameObject circleCollectedVFXRed;
        public GameObject circleCollectedVFXBlue;
        public GameObject circleSentVFXRed;
        public GameObject circleSentVFXBlue;
        
        private bool _isDefending;
        private GameObject _circle;
        private float _lastActivatedTime;

        private void Start()
        { 
            _circle = transform.Find("Circle").gameObject;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!_isDefending) { return; }

            if (other.TryGetComponent<Circle>(out var circle))
            {
                var prefab = player.playerSide == PlayerSide.Blue ? circleCollectedVFXBlue : circleCollectedVFXRed;
                var particle = Instantiate(prefab, transform.position, Quaternion.identity);
                var pos = particle.transform.position;
                particle.transform.position = new Vector3(pos.x, pos.y, 89.95f);
                Destroy(particle, 1f);
                
                circle.PlayCollectedSound();
                GameManager.SpawnedCircles.Remove(circle.gameObject);
                Destroy(circle.gameObject);
            }
        }

        public void Activate()
        {
            if (Time.time - _lastActivatedTime < defenceSeconds) { return; }

            _lastActivatedTime = Time.time;
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
            circle.sender = player;
            player.lastAttackTime = Time.time;
            
            GameManager.SpawnedCircles.Add(circle.gameObject);
            
            var prefab = player.playerSide == PlayerSide.Blue ? circleSentVFXBlue : circleSentVFXRed;
            var particle = Instantiate(prefab, circle.transform.position, Quaternion.identity);
            var pos = particle.transform.position;
            particle.transform.position = new Vector3(pos.x, pos.y, 89.95f);
            Destroy(particle, 2f);
        }

        private void Defend()
        {
            if (_isDefending) { return; }

            StartCoroutine(DefendCoroutine());
        }

        private IEnumerator DefendCoroutine()
        {
            _isDefending = true;
            _circle.SetActive(true);
            yield return new WaitForSeconds(defenceSeconds);
            _circle.SetActive(false);
            _isDefending = false;
        }
    }
    
}
