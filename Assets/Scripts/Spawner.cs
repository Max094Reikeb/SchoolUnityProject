using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private Obstacle ObstaclePrefab;

    [SerializeField]
    private Obstacle SquareObstaclePrefab;

    [SerializeField]
    private Obstacle CapsuleObstaclePrefab;

    [SerializeField]
    private Pickup SlowDownPickupPrefab;

    [SerializeField]
    private Pickup FewerDangersPickupPrefab;

    [SerializeField]
    private Pickup SpeedUpPickupPrefab;

    [SerializeField]
    private Pickup MoreDangersPickupPrefab;

    [SerializeField]
    private Pickup ScoreUpPickupPrefab;

    [SerializeField]
    private Pickup ScoreDownPickupPrefab;

    [SerializeField]
    private Pickup HealthPickupPrefab;

    private Player _player;
    private Score _score;

    void Awake()
    {
        _player = FindAnyObjectByType<Player>();
        _score = FindAnyObjectByType<Score>();
    }

    private const int SphereDamages = 1;
    private const int SquareDamages = 3;
    private const int CapsuleDamages = 5;

    private const float PickupSpawnChance = 0.10f;
    private const float HealthPickupChance = 0.05f;

    private const float SlowFactor = 0.5f;
    private const float SpeedUpFactor = 1.6f;
    private const float FewerDangersFactor = 0.4f;
    private const float MoreDangersFactor = 2f;

    [SerializeField]
    private Vector2 SpawnBounds;

    [SerializeField]
    private Vector2 SpawnDelay;

    private float _nextSpawn;

    private float _slowEndTime;
    private float _speedUpEndTime;
    private float _fewerDangersEndTime;
    private float _moreDangersEndTime;

    public float SpeedMultiplier
    {
        get
        {
            float m = 1f;
            if (Time.time < _slowEndTime) m *= SlowFactor;
            if (Time.time < _speedUpEndTime) m *= SpeedUpFactor;
            return m;
        }
    }

    private float DangerWeight
    {
        get
        {
            float m = 1f;
            if (Time.time < _fewerDangersEndTime) m *= FewerDangersFactor;
            if (Time.time < _moreDangersEndTime) m *= MoreDangersFactor;
            return m;
        }
    }

    void Update()
    {
        if (Time.time > _nextSpawn)
        {
            if (Random.value < PickupSpawnChance)
            {
                SpawnPickup();
            }
            else
            {
                SpawnObstacle();
            }

            float delay = Random.Range(SpawnDelay.x, SpawnDelay.y) / SpeedMultiplier;
            _nextSpawn = Time.time + delay;
        }
    }

    private void SpawnObstacle()
    {
        float weight = DangerWeight;
        float sphereW = 0.40f;
        float squareW = 0.35f * weight;
        float capsuleW = 0.25f * weight;
        float total = sphereW + squareW + capsuleW;

        float r = Random.value * total;
        if (r < sphereW)
        {
            Spawn(ObstaclePrefab, SphereDamages);
        }
        else if (r < sphereW + squareW)
        {
            Spawn(SquareObstaclePrefab, SquareDamages);
        }
        else
        {
            Spawn(CapsuleObstaclePrefab, CapsuleDamages);
        }
    }

    private void SpawnPickup()
    {
        Pickup prefab;
        if (Random.value < HealthPickupChance)
        {
            prefab = HealthPickupPrefab;
        }
        else
        {
            float r = Random.value;
            if (r < 1f / 6f) prefab = SlowDownPickupPrefab;
            else if (r < 2f / 6f) prefab = FewerDangersPickupPrefab;
            else if (r < 3f / 6f) prefab = SpeedUpPickupPrefab;
            else if (r < 4f / 6f) prefab = MoreDangersPickupPrefab;
            else if (r < 5f / 6f) prefab = ScoreUpPickupPrefab;
            else prefab = ScoreDownPickupPrefab;
        }

        if (prefab == null) return;

        Pickup p = Instantiate(prefab, transform);
        p.transform.localPosition = new Vector3(
            Random.Range(-SpawnBounds.x, SpawnBounds.x),
            Random.Range(-SpawnBounds.y, SpawnBounds.y),
            0);
        p.Initialize(this);
    }

    private void Spawn(Obstacle prefab, int damages)
    {
        Obstacle o = Instantiate(prefab, transform);
        o.transform.localPosition = new Vector3(
            Random.Range(-SpawnBounds.x, SpawnBounds.x),
            Random.Range(-SpawnBounds.y, SpawnBounds.y),
            0);
        o.SetDamages(damages);
        o.Initialize(this);
    }

    public void ApplyEffect(Pickup.EffectType type, float duration)
    {
        switch (type)
        {
            case Pickup.EffectType.SlowDown:
                _slowEndTime = Mathf.Max(Time.time, _slowEndTime) + duration;
                break;
            case Pickup.EffectType.SpeedUp:
                _speedUpEndTime = Mathf.Max(Time.time, _speedUpEndTime) + duration;
                break;
            case Pickup.EffectType.FewerDangers:
                _fewerDangersEndTime = Mathf.Max(Time.time, _fewerDangersEndTime) + duration;
                break;
            case Pickup.EffectType.MoreDangers:
                _moreDangersEndTime = Mathf.Max(Time.time, _moreDangersEndTime) + duration;
                break;
            case Pickup.EffectType.ScoreUp:
                if (_score != null) _score.ApplyScoreUp(duration);
                break;
            case Pickup.EffectType.ScoreDown:
                if (_score != null) _score.ApplyScoreDown(duration);
                break;
            case Pickup.EffectType.RestoreHealth:
                if (_player != null) _player.Heal();
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawCube(transform.position, (Vector3)SpawnBounds);
    }
}
