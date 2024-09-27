using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Util
{
    public static class UIAnimations
    {
        public static void PopupFadeIn(Image bg, RectTransform popup, float fadeTime)
        {
            bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, 0f);
            popup.transform.localPosition = new Vector3(-2000f, 0, 0);
            popup.DOAnchorPos(new Vector2(0, 0), fadeTime).SetEase(Ease.InOutQuint);
            bg.DOFade(0.85f, fadeTime);
        }

        public static void PopupFadeOut(Image bg, RectTransform popup, float fadeTime)
        {
            bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, 1f);
            popup.transform.localPosition = new Vector3(0, 0, 0);
            popup.DOAnchorPos(new Vector2(-2000, 0), fadeTime).SetEase(Ease.InOutQuint);
            bg.DOFade(0, fadeTime);
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
    }
}
