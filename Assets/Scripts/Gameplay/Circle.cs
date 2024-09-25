using System;
using UnityEngine;

namespace Gameplay
{
    public class Circle : MonoBehaviour
    {
        public Vector3 dir;
        public float speed;
        
        private void Update()
        {
            transform.position += dir * (speed * 100f * Time.deltaTime);
        }
    }
}
