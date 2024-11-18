using System;
using Cysharp.Threading.Tasks;
using Gameplay.LevelMechanics;
using Managers;
using UnityEngine;
using UnityEngine.Timeline;
using Util;

namespace Gameplay
{
    public class CircleSpawner : MonoBehaviour
    {
        public Team team;
        public float defenceSeconds;
        public GameObject circleSentVFX;
        public GameObject getHitVFX;
        [HideInInspector] public GameObject circleParent;
        
        private GameObject _circle;
        private float _lastActivatedTime;
        private ParticleSystem _particle;
        public bool IsLocked { get; private set; }

        private void Start()
        { 
            _particle = GetComponentInChildren<ParticleSystem>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<Circle>(out var circle))
            {
                PlayHitVfx();
                circle.PlayMissedSound();
                GameManager.SpawnedCircles.Remove(circle);
                LockSpawner(1f);
                circle.sender.AddScore(GameConfigReader.Instance.data.circleHitEnemyWeaponPoint);
                Destroy(circle.gameObject);
            }

            if (other.TryGetComponent<Missile>(out var missile))
            {
                PlayHitVfx();
                missile.sender.AddScore(GameConfigReader.Instance.data.missileDamagePoint);
            }
        }

        private void PlayHitVfx()
        {
            var vfx = Instantiate(getHitVFX, transform);
            Destroy(vfx, 0.5f);
        }

        public void Activate()
        {
            if (Time.time - _lastActivatedTime < defenceSeconds) { return; }

            _lastActivatedTime = Time.time;
            Attack();
        }

        private void Attack()
        {
            var circle = Instantiate(team.circlePrefab, circleParent.transform).GetComponent<Circle>();
            circle.transform.localPosition = transform.localPosition;
            circle.dir = team.GetDirectionVector();
            circle.sender = team;
            
            GameManager.SpawnedCircles.Add(circle);
            
            var particle = Instantiate(circleSentVFX, circle.transform.position, Quaternion.identity);
            var pos = particle.transform.position;
            particle.transform.position = new Vector3(pos.x, pos.y, 89.95f);
            Destroy(particle, 2f);
        }

        public async void LockSpawner(float time)
        {
            IsLocked = true;
            _particle.Play();
            await UniTask.WaitForSeconds(time);
            _particle.Stop();
            IsLocked = false;
        }
    }
    
}
