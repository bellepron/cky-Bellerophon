using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

namespace Bellepron.UI
{
    public class DashChargeUI : MonoBehaviour
    {
        [SerializeField] Image icon;
        [SerializeField] Image fill;
        // [SerializeField] Material radialGlowMat;

        // Material runtimeMat;

        void Awake()
        {
            // runtimeMat = new Material(radialGlowMat);
            // fill.material = runtimeMat;
        }

        public void SetFull(bool animate)
        {
            icon.enabled = true;
            fill.fillAmount = 1f;

            if (!animate) return;

            transform.localScale = Vector3.one * 0.6f;

            transform
                .DOScale(1f, 0.35f)
                .SetEase(Ease.OutBack);
        }

        public void SetEmpty()
        {
            icon.enabled = false;
            fill.fillAmount = 0;
        }

        public void SetRecharge(float t)
        {
            icon.enabled = true;
            fill.fillAmount = t;

            // runtimeMat.SetFloat("_GlowStrength", Mathf.Lerp(0.2f,1f,t));
        }

        public void Break()
        {
            transform.DOShakeScale(0.2f, 0.3f);
        }

        public void RechargeShake()
        {
            transform.DOShakePosition(0.15f, 3f);
        }
    }
}