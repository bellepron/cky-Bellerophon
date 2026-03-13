using System.Collections.Generic;
using Bellepron.Player;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using Zenject;

namespace Bellepron.UI
{
    public class DashUI : MonoBehaviour
    {
        [Inject] PlayerDashController.Settings _playerDashControlllerSettings;

        [SerializeField] DashChargeUI dashPrefab;
        [SerializeField] Transform container;

        [SerializeField] RectTransform screenPulse;
        Image _screenPulseImage;

        [SerializeField] List<DashChargeUI> charges = new();

        [Inject] SignalBus _signalBus;

        void OnEnable()
        {
            _signalBus.Subscribe<PlayerDashControllerCreatedSignal>(OnPlayerDashControllerCreated);
            _signalBus.Subscribe<DashChargeUsedSignal>(OnDashChargeUsed);
            _signalBus.Subscribe<DashChargeRestoredSignal>(OnDashChargeRestored);
            _signalBus.Subscribe<DashRechargeSignal>(OnRecharge);
        }

        void OnDisable()
        {
            _signalBus.Unsubscribe<PlayerDashControllerCreatedSignal>(OnPlayerDashControllerCreated);
            _signalBus.Unsubscribe<DashChargeUsedSignal>(OnDashChargeUsed);
            _signalBus.Unsubscribe<DashChargeRestoredSignal>(OnDashChargeRestored);
            _signalBus.Unsubscribe<DashRechargeSignal>(OnRecharge);
        }

        void OnPlayerDashControllerCreated()
        {
            ResetCharges();
            CreateCharges();

            _screenPulseImage = screenPulse.GetComponent<Image>();
            _screenPulseImage.color = new Color(1, 1, 1, 0f);
        }

        void OnRecharge(DashRechargeSignal signal)
        {
            UpdateRecharge(signal.currentDash, signal.progress);
        }

        private void ResetCharges()
        {
            foreach (DashChargeUI charge in charges)
            {
                Destroy(charge.gameObject);
            }

            charges.Clear();
        }

        void CreateCharges()
        {
            for (int i = 0; i < _playerDashControlllerSettings.maxDashCharges; i++)
            {
                var charge = Instantiate(dashPrefab, container);
                charges.Add(charge);
            }
        }

        void OnDashChargeUsed(DashChargeUsedSignal signal)
        {
            var used = signal.currentDashCharges;
            if (used + 1 < charges.Count) charges[used + 1].SetEmpty();
            charges[used].Break();

            PulseScreen();
        }

        void OnDashChargeRestored(DashChargeRestoredSignal signal)
        {
            var restored = signal.currentDashCharges - 1;
            charges[restored].SetFull(true);
        }

        void UpdateRecharge(int currentDash, float t)
        {
            int index = currentDash;

            if (index < charges.Count)
            {
                charges[index].SetRecharge(t);

                if (t > 0.95f)
                    charges[index].RechargeShake();
            }
        }

        void PulseScreen()
        {
            screenPulse.DOKill();

            screenPulse.localScale = Vector3.one * 0.7f;

            _screenPulseImage.color = new Color(1, 1, 1, 0.2f);

            Sequence s = DOTween.Sequence();
            s.Append(screenPulse.DOScale(1.2f, 0.25f).SetEase(Ease.OutQuad));
            s.Join(_screenPulseImage.DOFade(0f, 0.25f));
        }
    }
}