using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    private float Speed;

    [SerializeField]
    private float DestroyDistance;

    [SerializeField]
    private int Damages;

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0, 0, -Speed * Time.deltaTime);

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
        Destroy(gameObject);
        return Damages;
    }
}
