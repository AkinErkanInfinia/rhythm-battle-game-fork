using System;
using Coffee.UIExtensions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.LevelMechanics
{
    public class MissileCircle : MonoBehaviour
    {
        public float targetXMin, targetXMax, targetY;
        public float disappearAfterSeconds;
        public GameObject missile;
        public GameObject shootParticle;

        private Animator _animator;
        private BoxCollider2D _collider;
        private UIDissolve _dissolve;
        private static readonly int startAttack = Animator.StringToHash("StartAttack");

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _collider = GetComponent<BoxCollider2D>();
            _dissolve = GetComponent<UIDissolve>();
            
            DissolveIn();
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
            if (!other.CompareTag("Player")) { return; }
            
            _collider.enabled = false;
            _animator.SetTrigger(startAttack);
        }

        public void AttackMissile()
        {
            var dest = new Vector2(Random.Range(targetXMin, targetXMax), targetY);
            var obj = Instantiate(missile, transform.parent.transform).GetComponent<Missile>();
            obj.transform.localPosition = transform.localPosition;
            obj.SendMissile(transform.localPosition, dest);
        }

        public void DestroyMe()
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
