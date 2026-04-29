using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;                     // Pour TextMeshProUGUI

public class GameOver : MonoBehaviour
  {
      [SerializeField]
      private TextMeshProUGUI FinalScoreText;   // Texte affiché sur l'écran Game Over

      [SerializeField]
      private Score Score;                      // Référence au ScoreManager

      // OnEnable est appelé chaque fois que ce GameObject passe d'inactif à actif
      // Donc à chaque fois que Player fait GameOverScreen.SetActive(true)
      void OnEnable()
      {
          FinalScoreText.text = "Score final : " + Score.CurrentScore;
      }

      public void PlayAgain()
      {

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
      }

      public void Quit()
      {
          Application.Quit();
      }
  }