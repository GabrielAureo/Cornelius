using UnityEngine;

public class BossFight : MonoBehaviour{
    public CameraBehaviour cameraMode;
    public Transform cameraTarget;
    public Boss boss;
    
    void OnTriggerEnter2D(Collider2D coll){
        if(coll.tag == "Player"){
            GameManager.instance.cameraController.SetMode(cameraMode, cameraTarget, true, boss.StartFight);
            GetComponent<Collider2D>().enabled = false;
        }

    }
}