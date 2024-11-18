using System;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;

namespace Gameplay.LevelMechanics
{
    public class Missile : MonoBehaviour
    {
        public GameObject hitEffect;
        public float lockTime;
        public float speed = 3;
        public float deviation = 1000f;
        [HideInInspector] public Team sender;
        [HideInInspector] public Vector2 startPoint;
        [HideInInspector] public Vector2 destinationPoint;
        
        private Vector2 _middlePoint;
        private bool _startedMove;
        private float _count;
        private MechanicType _mechanicType;
        private Quaternion _fromRotation;
        private Quaternion _toRotation;
        
        private void Update()
        {
            if (!_startedMove) { return; }

            if (_count < 1f)
            {
                _count += Time.deltaTime * speed;
                
                var lastPos = transform.localPosition;
                Vector2 m1 = Vector2.Lerp(startPoint, _middlePoint, _count);
                Vector2 m2 = Vector2.Lerp(_middlePoint, destinationPoint, _count);
                transform.localPosition = Vector2.Lerp(m1, m2, _count);
                
                transform.localRotation = Quaternion.Lerp(_fromRotation, _toRotation, _count);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Hit();
            if (_mechanicType == MechanicType.Lock && other.TryGetComponent<CircleSpawner>(out var spawner))
            {
                spawner.LockSpawner(lockTime);
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
            SetQuaternions(dir);
            
            _middlePoint = startPoint + (destinationPoint - startPoint) / 2 + dir * deviation;
            _startedMove = true;
        }

        private void SetQuaternions(Vector2 dir)
        {
            var startZRotation = transform.localRotation.eulerAngles.z;
            var rotationMultiply = startZRotation == 0 ? 1 : -1;
            if (dir.normalized == Vector2.right)
            {
                _fromRotation = Quaternion.Euler(0, 0, startZRotation + -45f * rotationMultiply);
                _toRotation = Quaternion.Euler(0, 0, startZRotation + 45f * rotationMultiply);
            }
            else
            {
                _fromRotation = Quaternion.Euler(0, 0, startZRotation + 45f * rotationMultiply);
                _toRotation = Quaternion.Euler(0, 0, startZRotation + -45f * rotationMultiply);
            }
        }
    }
}
