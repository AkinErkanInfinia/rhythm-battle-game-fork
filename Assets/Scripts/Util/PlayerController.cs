using System;
using Gameplay;
using UnityEngine;

namespace Util
{
    public class PlayerController : MonoBehaviour
    {
        public PlayerSide playerSide;
        public float moveSpeed;
        public bool isDebug = false;

        private RectTransform _rectTransform;
        
        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (!isDebug)
                return;

            if (playerSide == PlayerSide.Green)
            {
                RedPlayerController();
            }
            else
            {
                BluePlayerController();
            }
        }

        private void RedPlayerController()
        {
            var pos = _rectTransform.anchoredPosition;
            if (Input.GetKey(KeyCode.W))
            {
                pos = _rectTransform.anchoredPosition + new Vector2(0, moveSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.A))
            {
                pos = _rectTransform.anchoredPosition + new Vector2(-moveSpeed * Time.deltaTime, 0);
            }

            if (Input.GetKey(KeyCode.S))
            {
                pos = _rectTransform.anchoredPosition + new Vector2(0, -moveSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.D))
            {
                pos = _rectTransform.anchoredPosition + new Vector2(moveSpeed * Time.deltaTime, 0);
            }

            var w = _rectTransform.rect.width;
            var h = _rectTransform.rect.height;
            //pos.x = Mathf.Clamp(pos.x, -1500 + w, 1200 - w);
            //pos.y = Mathf.Clamp(pos.y, 0 + h, 1465 - h);
            _rectTransform.anchoredPosition = pos;
        }

        private void BluePlayerController()
        {
            var pos = _rectTransform.anchoredPosition;
            if (Input.GetKey(KeyCode.UpArrow))
            {
                pos = _rectTransform.anchoredPosition + new Vector2(0, moveSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                pos = _rectTransform.anchoredPosition + new Vector2(-moveSpeed * Time.deltaTime, 0);
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                pos = _rectTransform.anchoredPosition + new Vector2(0, -moveSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                pos = _rectTransform.anchoredPosition + new Vector2(moveSpeed * Time.deltaTime, 0);
            }
            var w = _rectTransform.rect.width;
            var h = _rectTransform.rect.height;
            //pos.x = Mathf.Clamp(pos.x, -1500 + w, 1200 - w);
            //pos.y = Mathf.Clamp(pos.y, -1465 + h, 0 - h);
            _rectTransform.anchoredPosition = pos;
        }

        private void Movement()
        {
            var pos = _rectTransform.anchoredPosition;

            var w = _rectTransform.rect.width;
            var h = _rectTransform.rect.height;

            if (playerSide is PlayerSide.Blue)
            {
                pos.x = Mathf.Clamp(pos.x, -1500 + w, 1200 - w);
                pos.y = Mathf.Clamp(pos.y, -1465 + h, 0 - h);
            }

            else
            {
                pos.x = Mathf.Clamp(pos.x, -1500 + w, 1200 - w);
                pos.y = Mathf.Clamp(pos.y, 0 + h, 1465 - h);
            }

            _rectTransform.anchoredPosition = pos;
        }
    }
}
