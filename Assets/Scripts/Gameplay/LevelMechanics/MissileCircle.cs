using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.LevelMechanics
{
    public class MissileCircle : MonoBehaviour
    {
        public float disappearAfterSeconds;
        public GameObject missile;
        public int missileCount;

        private void Start()
        {
            DisappearTimer();
        }

        private async void DisappearTimer()
        {
            await UniTask.WaitForSeconds(disappearAfterSeconds);

            if (gameObject == null) { return; }
            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            
            AttackMissiles();
        }

        private async void AttackMissiles()
        {
            for (int i = 0; i < missileCount; i++)
            {
                var dest = new Vector2(Random.Range(-1370, 1072), 1565);
                var obj = Instantiate(missile, transform.parent.transform).GetComponent<Missile>();
                obj.SendMissile(transform.localPosition, dest);
                await UniTask.WaitForSeconds(1);
            }
        }
    }
}
