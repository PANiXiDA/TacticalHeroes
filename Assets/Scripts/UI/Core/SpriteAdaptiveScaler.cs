using Cysharp.Threading.Tasks;

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAdaptiveScaler : MonoBehaviour
{
    public enum ScaleMode { Fit, Fill }
    public ScaleMode scaleMode = ScaleMode.Fill;

    private const float HeightMultiplier = 2f;

    private SpriteRenderer _sr;
    private Vector3 _initialScale;
    private Vector2 _initialSize;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _initialScale = transform.localScale;
        var bound = _sr.sprite.bounds;
        _initialSize = new Vector2(bound.size.x * _initialScale.x, bound.size.y * _initialScale.y);
    }

    private void Start()
    {
        InitializeAsync().Forget();
    }

    private async UniTask InitializeAsync()
    {
        await UniTask.WaitForEndOfFrame();
        ApplyScaling();
    }

    private void ApplyScaling()
    {
        var cam = Camera.main;
        if (cam == null)
        {
            return;
        }

        float worldH = cam.orthographicSize * HeightMultiplier;
        float worldW = worldH * cam.aspect;

        float sx = worldW / _initialSize.x;
        float sy = worldH / _initialSize.y;
        float factor = (scaleMode == ScaleMode.Fill)
            ? Mathf.Max(sx, sy)
            : Mathf.Min(sx, sy);

        transform.localScale = _initialScale * factor;
    }
}
