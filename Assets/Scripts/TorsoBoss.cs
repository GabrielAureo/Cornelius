using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class TorsoBoss : Boss{
    [DraggablePoint]
    public Vector3 stompHoverPosition;
    [DraggablePoint]
    public Vector3 defaultPosition;
    [DraggablePoint]
    public Vector3 eyeEllipse;
    public Vector2 eyeEllipseRadius;
    public float timeDistanceEyes;

    public List<TorsoBossHand> hands;
    List<TorsoBossEye> eyes;
    public Transform cameraAnchor;
    TorsoBossHand activeHand;

    public GameObject eyeProjectile;

    public override void StartFight(){
        SimplePool.Preload(eyeProjectile,10);
        eyes = new List<TorsoBossEye>();
        activeHand = hands[0];
        foreach(var hand in hands){
            eyes.Add(hand.eye);
        }
        activeHand.setAsActive();
        StartCoroutine(EmergeAnimation());
    }

    IEnumerator EmergeAnimation(){
        GameManager.instance.cameraController.Shake(5f);
        var motion = transform.DOMoveY(defaultPosition.y, 5f).SetEase(Ease.OutSine);
        motion.onComplete+=
        () =>{
            foreach(var hand in hands){
                hand.FadeIn(2f);
            }
        };
        //Isso vai mudar, mega gambiarra
        yield return new WaitForSeconds(7.5f);
        StartCoroutine(AttackPattern());
    }

    IEnumerator AttackPattern(){
        StartCoroutine(EyeCircleAttack());
        activeHand.StompAttack(stompHoverPosition);
        yield return null;
    }

    IEnumerator EyeCircleAttack(){
        float partAngle = (2 * Mathf.PI)/eyes.Count;

        for(int i = 0; i < eyes.Count; i++){
            var posX = Mathf.Cos(partAngle * i) * eyeEllipseRadius.x;
            var posY = Mathf.Sin(partAngle * i) * eyeEllipseRadius.y;
            var position =  new Vector2(posX + eyeEllipse.x, posY + eyeEllipse.y);
            eyes[i].RotateAttack(eyeEllipse, eyeEllipseRadius);
            yield return new WaitForSeconds(timeDistanceEyes);
        }
        StartCoroutine(ProjectilesSpawn());
        
    }
    
    IEnumerator ProjectilesSpawn(){
        var eye = eyes[Random.Range(0,3)];
        while(true){
            SimplePool.Spawn(eyeProjectile, eye.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(5f);
        }
            
                
            
    }


    void ChangeHand(int index){
        if(index > hands.Count) Debug.LogError("There's no hand" + index);
        activeHand.setAsInactive();
        activeHand = hands[index];
        activeHand.setAsActive();
    }

    void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawLine(eyeEllipse + Vector3.up *  eyeEllipseRadius.y, eyeEllipse - Vector3.up *  eyeEllipseRadius.y);
        Gizmos.DrawLine(eyeEllipse + Vector3.right * eyeEllipseRadius.x, eyeEllipse - Vector3.right * eyeEllipseRadius.x);
    }

}