using UnityEngine;

namespace Gameplay
{
    public class Player : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<CircleSpawner>(out var spawner) && !spawner.IsLocked)
            {
                spawner.Activate();
            }
        }
    }
}
