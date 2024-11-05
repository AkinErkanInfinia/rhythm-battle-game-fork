using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.LevelMechanics
{
    public class Missile : MonoBehaviour
    {
        public GameObject hitEffect;
        public float lockTime;
        public float speed = 3;
        public float deviation = 1000f;
        [HideInInspector] public Vector2 startPoint;
        [HideInInspector] public Vector2 destinationPoint;
        
        private Vector2 _middlePoint;
        private bool _startedMove;
        private float _count;
        private MechanicType _mechanicType;
        
        private void Update()
        {
            if (!_startedMove) { return; }

            if (_count < 1f)
            {
                _count += Time.deltaTime * speed;

                Vector2 m1 = Vector2.Lerp(startPoint, _middlePoint, _count);
                Vector2 m2 = Vector2.Lerp(_middlePoint, destinationPoint, _count);
                transform.localPosition = Vector2.Lerp(m1, m2, _count);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("GameBound") && _mechanicType == MechanicType.Missile)
            {
                Hit();
            }
            
            if (other.CompareTag("Circle") && _mechanicType == MechanicType.Alien)
            {
                Hit();
                other.GetComponent<Circle>().DestroyCircle();
            }
            
            if (other.CompareTag("CircleSpawner") && _mechanicType == MechanicType.Lock)
            {
                Hit();
                if (other.TryGetComponent<CircleSpawner>(out var spawner))
                {
                    spawner.LockSpawner(lockTime);
                }
            }
        }

        private void Hit()
        {
            var effect = Instantiate(hitEffect, transform.parent.transform);
            effect.transform.position = transform.position;
            Destroy(effect, 1f);
            Destroy(gameObject, 0.1f);
        }

        public void SendMissile(Vector2 start, Vector2 dest, MechanicType mechanicType)
        {
            _mechanicType = mechanicType;
            startPoint = start;
            destinationPoint = dest;
            var dir = Random.Range(0, 2) == 0 ? Vector2.right : Vector2.left;
            _middlePoint = startPoint + (destinationPoint - startPoint) / 2 + dir * deviation;

            _startedMove = true;
        }
    }
}
