using UnityEngine;            // API Unity
using TMPro;                  // Espace de noms de TextMeshPro

public class Score : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI ScoreText;   // Texte UI à mettre à jour (lié dans l'Inspector)

    private const float ScoreUpFactor = 1.5f;
    private const float ScoreDownFactor = 0.5f;

    private float _elapsedTime;          // Temps de jeu accumulé en secondes
    private int _currentScore;           // Score affiché, arrondi à l'entier

    private float _scoreUpEndTime;
    private float _scoreDownEndTime;

    private float Multiplier
    {
        get
        {
            float m = 1f;
            if (Time.time < _scoreUpEndTime) m *= ScoreUpFactor;
            if (Time.time < _scoreDownEndTime) m *= ScoreDownFactor;
            return m;
        }
    }

    void Update()
    {
        _elapsedTime += Time.deltaTime * Multiplier;     // +durée de la frame, pondérée
        _currentScore = Mathf.FloorToInt(_elapsedTime);  // Conversion float → int
        ScoreText.text = "Score : " + _currentScore;    // Mise à jour du texte
    }

    public void ApplyScoreUp(float duration)
    {
        _scoreUpEndTime = Mathf.Max(Time.time, _scoreUpEndTime) + duration;
    }

    public void ApplyScoreDown(float duration)
    {
        _scoreDownEndTime = Mathf.Max(Time.time, _scoreDownEndTime) + duration;
    }

    public void Stop()
    {
        enabled = false;   // Update() ne sera plus appelé → score figé
    }
}