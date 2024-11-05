using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Gameplay.LevelMechanics
{
    public class MissileMechanic : LevelMechanicBase
    {
        public GameObject circlePrefab;
        public float minSpawnTime;
        public float maxSpawnTime;
        public bool spawnOnlyOnce;

        private float _spawnTimer;
        private float _lastSpawnTime;
        private bool _isSpawned;
        
        public override void MechanicLoop(int currentGameTime)
        {
            if (CanSpawn())
            {
                _lastSpawnTime = Time.time;
                _spawnTimer = Random.Range(minSpawnTime, maxSpawnTime);
                SpawnMissileCircle();
            }
        }

        private void SpawnMissileCircle()
        {
            _isSpawned = true;
            var pos = GetRandomPosition();
            var circle = Instantiate(circlePrefab, spawnParent.transform);
            circle.transform.localPosition = new Vector3(pos.x, pos.y, 0);
        }

        private bool CanSpawn()
        {
            if (spawnOnlyOnce && _isSpawned) { return false; }
            return Time.time - _lastSpawnTime > _spawnTimer;
        }

        private Vector2 GetRandomPosition()
        {
            var x = Random.Range(spawnBorders.xMin, spawnBorders.xMax);
            var y = Random.Range(spawnBorders.yMin, spawnBorders.yMax);
            
            return new Vector2(x, y);
        }
    }
}
