using System;
using Coffee.UIExtensions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Managers;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace Gameplay.LevelMechanics
{
    public enum MechanicType
    {
        Missile,
        Lock,
        Alien
    }
    
    public class MissileCircle : MonoBehaviour
    {
        public float targetXMin, targetXMax, targetY;
        public float disappearAfterSeconds;
        public GameObject missile;
        public GameObject shootParticle;
        public MechanicType mechanicType;

        private Animator _animator;
        private BoxCollider2D _collider;
        private UIDissolve _dissolve;
        private PlayerSide _side;
        private static readonly int startAttack = Animator.StringToHash("StartAttack");

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _collider = GetComponent<BoxCollider2D>();
            _dissolve = GetComponent<UIDissolve>();
            
            DissolveIn();
            
            if (mechanicType == MechanicType.Alien) { return; }
            
            DisappearTimer();
        }

        private async void DisappearTimer()
        {
            await UniTask.WaitForSeconds(disappearAfterSeconds);

            if (gameObject == null) { return; }
            
            if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                DestroyMe();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player") || mechanicType == MechanicType.Alien) { return; }
            
            _side = other.GetComponent<PlayerController>().playerSide;
            _collider.enabled = false;
            _animator.SetTrigger(startAttack);
        }

        public void AttackMissile()
        {
            var dest = Vector2.zero;
            if (mechanicType == MechanicType.Lock)
            {
                var spawners = _side == PlayerSide.Blue ? GameManager.RedSpawners : GameManager.BlueSpawners;
                dest = spawners[Random.Range(0, spawners.Count)].transform.position;
                dest = transform.parent.transform.InverseTransformPoint(dest);
            }
            else if (mechanicType == MechanicType.Missile)
            {
                dest = new Vector2(Random.Range(targetXMin, targetXMax), targetY);
            }
            else
            {
                var circles = GameManager.SpawnedCircles;
                if (circles.Count == 0) { return; }
                var randomCirclePosition = circles[Random.Range(0, circles.Count)].transform.position;
                dest = transform.parent.transform.InverseTransformPoint(randomCirclePosition);
            }

            var obj = Instantiate(missile, transform.parent.transform).GetComponent<Missile>();
            obj.transform.localPosition = transform.localPosition;
            obj.SendMissile(transform.localPosition, dest, mechanicType);
        }

        private void DestroyMe()
        {
            DissolveOut();
            Destroy(gameObject, 0.4f);
        }

        public void PlayShootParticle()
        {
            shootParticle.GetComponent<ParticleSystem>().Play();
        }

        public void DissolveIn()
        {
            DOTween.To(() => _dissolve.effectFactor, x => _dissolve.effectFactor = x, 0, 0.3f);
        }
        
        public void DissolveOut()
        {
            DOTween.To(() => _dissolve.effectFactor, x => _dissolve.effectFactor = x, 1, 0.3f);
        }
    }
}
