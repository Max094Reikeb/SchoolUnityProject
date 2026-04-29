using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum EffectType { SlowDown, FewerDangers, SpeedUp, MoreDangers, ScoreUp, ScoreDown, RestoreHealth }

    [SerializeField]
    private float Speed;

    [SerializeField]
    private float DestroyDistance;

    [SerializeField]
    private EffectType Effect;

    [SerializeField]
    private float Duration = 5f;

    private const float HomingDuration = 4f;
    private const float OrbitRotationsPerSecond = 1.5f;
    private const float MinOrbitRadius = 1f;

    private Spawner _spawner;
    private bool _homing;
    private Transform _homingTarget;
    private float _homingTime;
    private Vector3 _homingStartScale;
    private float _orbitAngle;
    private float _orbitInitialRadius;

    public EffectType GetEffect() => Effect;
    public float GetDuration() => Duration;

    public void Initialize(Spawner spawner)
    {
        _spawner = spawner;
    }

    void Update()
    {
        if (_homing)
        {
            UpdateHoming();
            return;
        }

        float multiplier = _spawner != null ? _spawner.SpeedMultiplier : 1f;
        transform.position += new Vector3(0, 0, -Speed * multiplier * Time.deltaTime);

        if (transform.position.z < DestroyDistance)
        {
            Destroy(gameObject);
        }
    }

    public void Collect(Transform target)
    {
        if (_homing) return;
        _homing = true;
        _homingTarget = target;
        _homingTime = 0f;
        _homingStartScale = transform.localScale;

        Vector3 offset = transform.position - target.position;
        _orbitAngle = Mathf.Atan2(offset.y, offset.x);
        float xyDist = new Vector2(offset.x, offset.y).magnitude;
        _orbitInitialRadius = Mathf.Max(xyDist, MinOrbitRadius);

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }

    private void UpdateHoming()
    {
        _homingTime += Time.deltaTime;

        if (_homingTarget == null || _homingTime >= HomingDuration)
        {
            ApplyAndDestroy();
            return;
        }

        float t = _homingTime / HomingDuration;
        float radius = _orbitInitialRadius * (1f - t * t);
        _orbitAngle += OrbitRotationsPerSecond * 2f * Mathf.PI * Time.deltaTime;

        Vector3 orbitOffset = new Vector3(
            Mathf.Cos(_orbitAngle) * radius,
            Mathf.Sin(_orbitAngle) * radius,
            0f);
        transform.position = _homingTarget.position + orbitOffset;
        transform.localScale = Vector3.Lerp(_homingStartScale, Vector3.zero, t);
    }

    private void ApplyAndDestroy()
    {
        if (_spawner != null)
        {
            _spawner.ApplyEffect(Effect, Duration);
        }
        Destroy(gameObject);
    }
}
