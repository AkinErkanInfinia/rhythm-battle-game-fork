using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Gameplay.LevelMechanics
{
    public class MissileMechanic : LevelMechanicBase
    {
        public GameObject circlePrefab;
        public float spawnEverySeconds;

        private bool _isSpawned;
        
        public override void MechanicLoop(int currentGameTime)
        {
            if (currentGameTime % spawnEverySeconds != 0 || _isSpawned) { return; }

            var pos = GetRandomPosition();
            var circle = Instantiate(circlePrefab, spawnParent.transform);
            circle.transform.localPosition = new Vector3(pos.x, pos.y, 0);
            SpawnDelay();
        }

        private async void SpawnDelay()
        {
            _isSpawned = true;
            await UniTask.WaitForSeconds(1);
            _isSpawned = false;
        }

        private Vector2 GetRandomPosition()
        {
            var x = Random.Range(spawnBorders.xMin, spawnBorders.xMax);
            var y = Random.Range(spawnBorders.yMin, spawnBorders.yMax);
            
            return new Vector2(x, y);
        }
    }
}
