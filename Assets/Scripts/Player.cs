using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float maxHealth = 100;
    public float curHealth = 100;
    public float decHealth = 100;
    public float regHealth = 0.1f;
    public UIelements restart;

    void Start() {
    }

    public void Update() {
        if (curHealth < maxHealth) {
            if (curHealth < 0) {
                Debug.LogError("noot");
                Destroy(gameObject);
                //restart.RestartGame();
                restart.GetComponent<UIelements>().RestartGame();
            }
            //curHealth += regHealth * Time.deltaTime;
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.tag == "Enemy") {
            curHealth -= decHealth * Time.deltaTime;
        }
    }
}
