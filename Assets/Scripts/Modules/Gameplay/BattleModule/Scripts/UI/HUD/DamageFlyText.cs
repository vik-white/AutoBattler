using TMPro;
using UnityEngine;

public class DamageFlyText : MonoBehaviour
{
    private const float Lifetime = 0.8f;
    private const float FlyHeight = 0.8f;
    private const float CritScaleMultiplier = 1.5f;
    private static readonly Color EnemyDamageColor = new(1f, 0.2f, 0.2f, 1f);
    private static readonly Color SquadDamageColor = new(0.25f, 1f, 0.35f, 1f);
    private static readonly Color CritDamageColor = new(1f, 0.85f, 0.1f, 1f);

    public TMP_Text Text;

    private Camera _camera;
    private Vector3 _startPosition;
    private float _elapsed;
    private Vector3 _baseScale;

    public void Initialize(Vector3 position, float damage, bool isEnemyTarget, bool isCrit)
    {
        _camera = Camera.main;
        _startPosition = position + new Vector3(0, 0.5f, 0);
        _elapsed = 0;
        _baseScale = transform.localScale;
        if (isCrit) transform.localScale = _baseScale * CritScaleMultiplier;
        if (Text != null)
        {
            var damageText = Mathf.CeilToInt(damage).ToString();
            Text.text = isCrit ? $"{damageText}" : damageText;
            Text.color = isCrit
                ? CritDamageColor
                : (isEnemyTarget ? EnemyDamageColor : SquadDamageColor);
            Text.fontStyle = isCrit ? FontStyles.Bold : FontStyles.Normal;
        }
        UpdatePosition(0);
    }

    private void Update()
    {
        _elapsed += Time.deltaTime;
        var progress = Mathf.Clamp01(_elapsed / Lifetime);
        UpdatePosition(progress);
        UpdateAlpha(progress);

        if (_elapsed >= Lifetime) Destroy(gameObject);
    }

    private void UpdatePosition(float progress)
    {
        if (_camera == null) _camera = Camera.main;
        if (_camera == null) return;

        transform.position = _camera.WorldToScreenPoint(_startPosition + Vector3.up * (FlyHeight * progress));
    }

    private void UpdateAlpha(float progress)
    {
        if (Text == null) return;

        var color = Text.color;
        color.a = 1 - Mathf.Clamp01((progress - 0.6f) / 0.4f);
        Text.color = color;
    }
}
