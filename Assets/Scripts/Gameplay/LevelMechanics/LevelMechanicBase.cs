using System;
using UnityEngine;

namespace Gameplay.LevelMechanics
{
    [Serializable]
    public class SpawnBorders
    {
        public float xMin;
        public float xMax;
        public float yMin;
        public float yMax;
    }
    public abstract class LevelMechanicBase : MonoBehaviour
    {
        public SpawnBorders spawnBorders;
        public GameObject spawnParent;
        public int activateMechanicAfterRound;
        public abstract void MechanicLoop(int currentGameTime);
    }
}
