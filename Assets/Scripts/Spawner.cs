using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private Obstacle ObstaclePrefab;

    [SerializeField]
    private Obstacle SquareObstaclePrefab;

    [SerializeField]
    private Obstacle CapsuleObstaclePrefab;

    private const int SphereDamages = 1;
    private const int SquareDamages = 3;
    private const int CapsuleDamages = 5;

    [SerializeField]
    private Vector2 SpawnBounds;

    [SerializeField]
    private Vector2 SpawnDelay;

    private float _nextSpawn;

    // Update is called once per frame
    void Update()
    {
        if(Time.time > _nextSpawn)
        {
            float r = Random.value;
            if (r < 0.40f)
            {
                SpawnSphere();
            }
            else if (r < 0.75f)
            {
                SpawnSquare();
            }
            else
            {
                SpawnCapsule();
            }

            _nextSpawn = Time.time + Random.Range(SpawnDelay.x, SpawnDelay.y);
        }
    }

    private void SpawnSphere()
    {
        Spawn(ObstaclePrefab, SphereDamages);
    }

    private void SpawnSquare()
    {
        Spawn(SquareObstaclePrefab, SquareDamages);
    }

    private void SpawnCapsule()
    {
        Spawn(CapsuleObstaclePrefab, CapsuleDamages);
    }

    private void Spawn(Obstacle prefab, int damages)
    {
        Obstacle o = Instantiate(prefab, transform);
        o.transform.localPosition = new Vector3(
            Random.Range(-SpawnBounds.x, SpawnBounds.x),
            Random.Range(-SpawnBounds.y, SpawnBounds.y),
            0);
        o.SetDamages(damages);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawCube(transform.position, (Vector3)SpawnBounds);
    }
}
