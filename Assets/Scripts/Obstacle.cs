using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    private float Speed;

    [SerializeField]
    private float DestroyDistance;

    [SerializeField]
    private int Damages;

    private Spawner _spawner;

    public void Initialize(Spawner spawner)
    {
        _spawner = spawner;
    }

    void Update()
    {
        float multiplier = _spawner != null ? _spawner.SpeedMultiplier : 1f;
        transform.position += new Vector3(0, 0, -Speed * multiplier * Time.deltaTime);

        if(transform.position.z < DestroyDistance)
        {
            Destroy(gameObject);
        }
    }

    public void SetDamages(int damages)
    {
        Damages = damages;
    }

    public int Explode()
    {
        SpawnExplosion();
        Destroy(gameObject);
        return Damages;
    }

    private void SpawnExplosion()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        Renderer rend = GetComponent<Renderer>();
        if (mf == null || rend == null) return;

        GameObject go = new GameObject("ObstacleExplosion");
        go.transform.position = transform.position;

        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.duration = 0.6f;
        main.loop = false;
        main.startLifetime = 0.6f;
        main.startSpeed = 6f;
        main.startSize = 0.25f;
        main.gravityModifier = 0.5f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.stopAction = ParticleSystemStopAction.Destroy;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 24) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.2f;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0f));

        var rotationOverLifetime = ps.rotationOverLifetime;
        rotationOverLifetime.enabled = true;
        rotationOverLifetime.x = new ParticleSystem.MinMaxCurve(-3f, 3f);
        rotationOverLifetime.y = new ParticleSystem.MinMaxCurve(-3f, 3f);
        rotationOverLifetime.z = new ParticleSystem.MinMaxCurve(-3f, 3f);

        ParticleSystemRenderer r = go.GetComponent<ParticleSystemRenderer>();
        r.renderMode = ParticleSystemRenderMode.Mesh;
        r.mesh = mf.sharedMesh;
        r.sharedMaterial = rend.sharedMaterial;

        ps.Play();
    }
}
