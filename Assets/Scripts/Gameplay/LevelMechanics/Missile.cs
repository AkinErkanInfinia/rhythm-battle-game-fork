using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.LevelMechanics
{
    public class Missile : MonoBehaviour
    {
        public GameObject hitEffect;
        [HideInInspector] public Vector2 startPoint;
        [HideInInspector] public Vector2 destinationPoint;
        
        private Vector2 _middlePoint;
        private bool _startedMove;
        private float _count;
        
        private void Update()
        {
            if (!_startedMove) { return; }

            if (_count < 1f)
            {
                _count += Time.deltaTime * 3;

                Vector2 m1 = Vector2.Lerp(startPoint, _middlePoint, _count);
                Vector2 m2 = Vector2.Lerp(_middlePoint, destinationPoint, _count);
                transform.localPosition = Vector2.Lerp(m1, m2, _count);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("GameBound")) { return; }
            
            var effect = Instantiate(hitEffect, transform.parent.transform);
            effect.transform.position = transform.position;
            Destroy(effect, 1f);
            Destroy(gameObject, 0.1f);
        }

        public void SendMissile(Vector2 start, Vector2 dest)
        {
            startPoint = start;
            destinationPoint = dest;
            var dir = Random.Range(0, 2) == 0 ? Vector2.right : Vector2.left;
            _middlePoint = startPoint + (destinationPoint - startPoint) / 2 + dir * 1000f;

            _startedMove = true;
        }
    }
}
