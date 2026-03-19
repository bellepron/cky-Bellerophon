using UnityEngine;
using DG.Tweening;

public class FloatAndRotate : MonoBehaviour
{
    [Header("Float (Up - Down)")]
    [SerializeField] private float floatHeight = 0.5f;
    [SerializeField] private float floatDuration = 1.5f;
    [SerializeField] private Ease floatEase = Ease.InOutSine;

    [Header("Tilt")]
    [SerializeField] private float tiltAngleX = 15f;

    [Header("Rotaation (Y Axis)")]
    [SerializeField] private float rotateDuration = 3f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;

        transform.rotation = Quaternion.Euler(tiltAngleX, transform.eulerAngles.y, 0f);

        ApplyFloat();
        ApplyRotation();
    }

    private void ApplyFloat()
    {
        transform
            .DOMoveY(startPosition.y + floatHeight, floatDuration)
            .SetEase(floatEase)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void ApplyRotation()
    {
        transform
            .DORotate(
                new Vector3(tiltAngleX, transform.eulerAngles.y + 360f, 0f),
                rotateDuration,
                RotateMode.FastBeyond360
            )
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying)
            transform.rotation = Quaternion.Euler(tiltAngleX, transform.eulerAngles.y, 0f);
    }
#endif
}
