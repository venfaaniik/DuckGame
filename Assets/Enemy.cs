using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float damage = 10f;
    public Player playerScript;
    void Start() {
       
    }

    //private void OnTriggerEnter(Collider collision) {
    //    Debug.Log("yeet");
    //    if (collision.tag == "Player") {
    //        Debug.Log("yaat");
    //        playerScript.AddjustCurrentHealth(-10);
    //    }
    //}
}
