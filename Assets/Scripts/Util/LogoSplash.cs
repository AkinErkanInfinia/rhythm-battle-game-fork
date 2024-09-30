using System;
using UnityEngine;
using UnityEngine.UI;

namespace Util
{
    public class LogoSplash : MonoBehaviour
    {
        public Image background;
        public Image logo;

        private void Start()
        {
            UIAnimations.LogoSplashAnimation(background, logo);
        }
    }
}
