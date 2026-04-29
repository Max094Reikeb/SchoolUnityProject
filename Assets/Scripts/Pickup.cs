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

    private Spawner _spawner;

    public EffectType GetEffect() => Effect;
    public float GetDuration() => Duration;

    public void Initialize(Spawner spawner)
    {
        _spawner = spawner;
    }

    void Update()
    {
        float multiplier = _spawner != null ? _spawner.SpeedMultiplier : 1f;
        transform.position += new Vector3(0, 0, -Speed * multiplier * Time.deltaTime);

        if (transform.position.z < DestroyDistance)
        {
            Destroy(gameObject);
        }
    }

    public void Collect()
    {
        if (_spawner != null)
        {
            _spawner.ApplyEffect(Effect, Duration);
        }
        Destroy(gameObject);
    }
}
