using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class UIelements : MonoBehaviour {

    public Button ResetButton;
    public Text health;
    public Player player;
    public Text ScoreText;

    private float timer;
    private int score;

    public void Start() {
        ResetButton.gameObject.SetActive(false);
    }
    private void Update() {
        health.GetComponent<Text>().text = "Health: " + player.GetComponent<Player>().curHealth;
        if (player.GetComponent<Player>().curHealth < 0) {
            health.GetComponent<Text>().text = "DEAD!";
        }
        Timer();
    }

    public void RestartGame() {
        ResetButton.gameObject.SetActive(true);
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // loads current scene
    }

    void Timer() {
        timer += Time.deltaTime;

        score += 1 + Mathf.RoundToInt(timer);       
        ScoreText.text = "Score: " + score.ToString();
        //Reset the timer to 0.
        timer = 0;
    }

}