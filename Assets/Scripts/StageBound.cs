using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageBound : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D coll){
        if(coll.tag == "Player") {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
