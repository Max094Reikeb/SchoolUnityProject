using UnityEngine;            // API Unity
using TMPro;                  // Espace de noms de TextMeshPro

public class Score : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI ScoreText;   // Texte UI à mettre à jour (lié dans l'Inspector)

    private float _elapsedTime;          // Temps de jeu accumulé en secondes
    private int _currentScore;           // Score affiché, arrondi à l'entier

    void Update()
    {
        _elapsedTime += Time.deltaTime;                  // +durée de la frame
        _currentScore = Mathf.FloorToInt(_elapsedTime);  // Conversion float → int
        ScoreText.text = "Score : " + _currentScore;    // Mise à jour du texte
    }

    public void Stop()
    {
        enabled = false;   // Update() ne sera plus appelé → score figé
    }
}