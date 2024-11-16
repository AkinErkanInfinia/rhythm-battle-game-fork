using Coffee.UIExtensions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Util
{
    public static class UIAnimations
    {
        public static void PopupFadeIn(RectTransform popup, float fadeTime)
        {
            popup.transform.localPosition = new Vector3(-3100, 0, 0);
            popup.DOAnchorPos(new Vector2(0, 0), fadeTime).SetEase(Ease.InOutQuint);
        }
        
        public static void PopupFadeOut(RectTransform popup, float fadeTime)
        {
            popup.transform.localPosition = new Vector3(0, 0, 0);
            popup.DOAnchorPos(new Vector2(-3100, 0), fadeTime).SetEase(Ease.InOutQuint);
        }
        
        public static void PopupDissolveIn(GameObject bg, RectTransform content, float fadeTime)
        {
            var dissolve = bg.GetComponent<UIDissolve>();
            DOTween.To(() => dissolve.effectFactor, x => dissolve.effectFactor = x, 0, fadeTime);
            
            content.transform.localPosition = new Vector3(0, 3500, 0);
            content.DOAnchorPos(new Vector2(0, 0), fadeTime).SetEase(Ease.InOutQuint);
        }
        
        public static void PopupDissolveOut(GameObject bg, RectTransform content, float fadeTime)
        {
            var dissolve = bg.GetComponent<UIDissolve>();
            DOTween.To(() => dissolve.effectFactor, x => dissolve.effectFactor = x, 1, fadeTime);
            
            content.transform.localPosition = new Vector3(0, 0, 0);
            content.DOAnchorPos(new Vector2(0, 3500), fadeTime).SetEase(Ease.InOutQuint);
        }

        public static async void MissedCircleEffect(Image bg, float effectTime)
        {
            await bg.DOFade(0.5f, effectTime).SetEase(Ease.InBounce).ToUniTask();
            await bg.DOFade(0f, effectTime / 2).SetEase(Ease.OutFlash).ToUniTask();
        }

        public static async void InactivityTextAnimation(TextMeshProUGUI text)
        {
            await text.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBounce).ToUniTask();
            await UniTask.WaitForSeconds(1f);
            await text.transform.DOScale(0, 0.5f).SetEase(Ease.OutFlash).ToUniTask();
        }

        public static async void LogoSplashAnimation(Image bg, Image logo)
        {
            logo.transform.DOScale(1f, 1.5f).SetEase(Ease.OutBounce).ToUniTask();
            logo.transform.DOPunchRotation(new Vector3(0, 0, 45f), 1f).ToUniTask();
            await UniTask.WaitForSeconds(3);
            bg.DOFade(0, 1f).ToUniTask();
            await logo.DOFade(0, 1f).ToUniTask();
            bg.gameObject.SetActive(false);
            logo.gameObject.SetActive(false);
        }
    }
}
