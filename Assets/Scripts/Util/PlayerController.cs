using System;
using Gameplay;
using UnityEngine;

namespace Util
{
    public class PlayerController : MonoBehaviour
    {
        public PlayerSide playerSide;
        public float moveSpeed;

        private RectTransform _rectTransform;
        
        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (playerSide == PlayerSide.Red)
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

            pos.x = Mathf.Clamp(pos.x, -910, -50);
            pos.y = Mathf.Clamp(pos.y, -290, 290);
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

            pos.x = Mathf.Clamp(pos.x, 50, 910);
            pos.y = Mathf.Clamp(pos.y, -290, 290);
            _rectTransform.anchoredPosition = pos;
        }
    }
}
