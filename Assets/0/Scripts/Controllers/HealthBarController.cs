using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using TMPro;

namespace Bellepron.UI
{
    public class HealthBarController : MonoBehaviour
    {
        [SerializeField] Slider slider;
        [SerializeField] TextMeshProUGUI sliderValueTMP;
        float _currentValue;
        // Sequence _seq;

        public void Initialize(int currentHealth, int maxHealth)
        {
            float _currentValue = (float)currentHealth / (float)maxHealth;

            UpdateSlider(_currentValue);
            UpdateTMP(currentHealth, maxHealth);
            slider.gameObject.SetActive(true);
            sliderValueTMP.gameObject.SetActive(true);
        }

        public void UpdateValue(int currentHealth, int maxHealth)
        {
            _currentValue = (float)currentHealth / (float)maxHealth;

            // if (_seq != null)
            //     _seq.Kill();
            // _seq = DOTween.Sequence();
            // _seq.Append(DOTween.To(float _currentValue => x))

            UpdateSlider(_currentValue);
            UpdateTMP(currentHealth, maxHealth);
        }

        void UpdateSlider(float value)
        {
            slider.value = value;

            if (value <= 0)
            {
                slider.gameObject.SetActive(false);
                sliderValueTMP.gameObject.SetActive(false);
            }
        }

        void UpdateTMP(int currentHealth, int maxHealth)
        {
            sliderValueTMP.text = $"{currentHealth}/{maxHealth}";
        }
    }
}