using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float Speed = 10f;

    [SerializeField]
    private float SpeedDecrease = 0.9f;

    [SerializeField]
    private Rigidbody Body;

    private Vector2 _movement;

    [SerializeField]
    private int HP = 10;

    [SerializeField]
    private Slider HPSlider;

    [SerializeField]
    private GameObject GameOverScreen;

    [SerializeField]
    private Score Score;          // Référence au ScoreManager

    void OnMove(InputValue value)
    {
        _movement = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        if(_movement.magnitude > 0)
        {
            Body.AddForce((Vector3)_movement * Speed);
        } else
        {
            Body.linearVelocity *= SpeedDecrease;
        }
    }

    public void Heal()
    {
        int max = (int)HPSlider.maxValue;
        int missing = max - HP;
        if (missing <= 0) return;
        int restore = Mathf.Max(1, missing / 3);
        HP = Mathf.Min(max, HP + restore);
        HPSlider.value = HP;
    }

    private void OnTriggerEnter(Collider other)
    {
        Obstacle o = other.GetComponent<Obstacle>();
        if(o != null)
        {
            int damages = o.Explode();
            HP -= damages;
            HPSlider.value = HP;
            if (HP <= 0)
            {
                enabled = false;
                Score.Stop(); 
                GameOverScreen.SetActive(true);
            }
            return;
        }

        Pickup p = other.GetComponent<Pickup>();
        if (p != null)
        {
            p.Collect(transform);
        }
    }
}
