using System;
using System.Linq;
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
        public float disappearAfterSeconds;
        public GameObject missile;
        public GameObject shootParticle;
        public MechanicType mechanicType;
        [HideInInspector] public Team sender;

        private Animator _animator;
        private BoxCollider2D _collider;
        private UIDissolve _dissolve;
        private PlayerSide _side;
        private bool _canBeDestroyed;
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

        private void Update()
        {
            if (!_canBeDestroyed) { return; }
            
            if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                DestroyMe();
            }
        }

        private async void DisappearTimer()
        {
            await UniTask.WaitForSeconds(disappearAfterSeconds);

            if (gameObject == null) { return; }

            _canBeDestroyed = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Circle") && mechanicType == MechanicType.Alien)
            {
                GetComponentInChildren<ParticleSystem>().Play();
                GameManager.SpawnedCircles.Remove(other.GetComponent<Circle>());
                Destroy(other.gameObject);
                return;
            }
            
            if (!other.CompareTag("Player") || mechanicType == MechanicType.Alien) { return; }
            
            _side = other.GetComponent<PlayerController>().playerSide;
            _collider.enabled = false;
            _animator.SetTrigger(startAttack);
        }

        public void AttackMissile()
        {
            var spawners = _side == PlayerSide.Blue ? GameManager.GreenSpawners : GameManager.BlueSpawners;
            var dest = spawners[Random.Range(0, spawners.Count)].transform.position;
            dest = transform.parent.transform.InverseTransformPoint(dest);
            
            var obj = Instantiate(missile, transform.parent.transform).GetComponent<Missile>();
            obj.sender = sender;
            obj.transform.localPosition = transform.localPosition;
            obj.SendMissile(transform.localPosition, dest, mechanicType);
        }

        public void TeleportAlien()
        {
            var side = sender.playerSide;
            var circles = GameManager.SpawnedCircles;
            if (circles.Count == 0) { return; }

            transform.localScale = Vector3.zero;
            _animator.SetTrigger("AlienTeleportIn");
            
            var enemyCircles = circles
                .Where(c => c.sender.playerSide != side)
                .ToList();
            var randomCircle = enemyCircles.ElementAtOrDefault(Random.Range(0, enemyCircles.Count));
            if (randomCircle == null) { return; }
            
            var dest = transform.parent.transform.InverseTransformPoint(randomCircle.transform.position);
            dest += randomCircle.dir.normalized * 750;
            dest.y = Mathf.Clamp(dest.y, -1000, 1000);
            transform.localPosition = dest;
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

        private void DissolveIn()
        {
            DOTween.To(() => _dissolve.effectFactor, x => _dissolve.effectFactor = x, 0, 0.3f);
        }
        
        public void DissolveOut()
        {
            DOTween.To(() => _dissolve.effectFactor, x => _dissolve.effectFactor = x, 1, 0.3f);
        }
    }
}
