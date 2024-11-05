using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;

namespace Gameplay
{
    public class CircleSpawner : MonoBehaviour
    {
        public Team team;
        public float defenceSeconds;
        public GameObject circleSentVFXRed;
        public GameObject circleSentVFXBlue;
        
        private GameObject _circle;
        private float _lastActivatedTime;
        private BoxCollider2D _boxCollider;
        private ParticleSystem _particle;

        private void Start()
        { 
            _boxCollider = GetComponent<BoxCollider2D>();
            _particle = GetComponentInChildren<ParticleSystem>();
        }

        public void Activate()
        {
            if (Time.time - _lastActivatedTime < defenceSeconds) { return; }

            _lastActivatedTime = Time.time;
            Attack();
        }

        private void Attack()
        {
            var circle = Instantiate(team.circlePrefab, transform).GetComponent<Circle>();
            circle.transform.localPosition = Vector3.zero;
            circle.dir = team.GetDirectionVector();
            circle.sender = team;
            team.lastAttackTime = Time.time;
            
            GameManager.SpawnedCircles.Add(circle.gameObject);
            
            var prefab = team.playerSide == PlayerSide.Blue ? circleSentVFXBlue : circleSentVFXRed;
            var particle = Instantiate(prefab, circle.transform.position, Quaternion.identity);
            var pos = particle.transform.position;
            particle.transform.position = new Vector3(pos.x, pos.y, 89.95f);
            Destroy(particle, 2f);
        }

        public async void LockSpawner(float time)
        {
            _boxCollider.enabled = false;
            _particle.Play();
            await UniTask.WaitForSeconds(time);
            _boxCollider.enabled = true;
            _particle.Stop();
        }
    }
    
}
