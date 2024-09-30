using UnityEngine;

namespace Util
{
    public class OffsetScrolling : MonoBehaviour
    {
        public float scrollSpeed;

        private Renderer _renderer;
        private Vector2 _savedOffset;

        void Start () {
            _renderer = GetComponent<MeshRenderer>();
        }

        void Update () {
            var x = Mathf.Repeat(Time.time * scrollSpeed, 1);
            var offset = new Vector2 (x, 0);
            _renderer.material.SetTextureOffset("_MainTex", offset);
        }
    }
}
