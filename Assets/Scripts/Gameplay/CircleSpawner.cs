using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;

namespace Gameplay
{
    public class CircleSpawner : MonoBehaviour
    {
        public Team team;
        public float defenceSeconds;
        public GameObject circleSentVFX;
        
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
            
            GameManager.SpawnedCircles.Add(circle.gameObject);
            
            var particle = Instantiate(circleSentVFX, circle.transform.position, Quaternion.identity);
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
