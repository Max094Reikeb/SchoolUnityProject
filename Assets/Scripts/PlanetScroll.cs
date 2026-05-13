using UnityEngine;

public class PlanetScroll : MonoBehaviour
{
    [SerializeField] private float Speed = 1.5f;
    [SerializeField] private float ResetZ = -10f;
    [SerializeField] private float StartZ = 120f;
    [SerializeField] private Spawner Spawner;

    void Update()
    {
        float multiplier = Spawner != null ? Spawner.SpeedMultiplier : 1f;
        transform.position += Vector3.back * Speed * multiplier * Time.deltaTime;

        if (transform.position.z < ResetZ)
        {
            Vector3 p = transform.position;
            p.z = StartZ;
            transform.position = p;
        }
    }
}
