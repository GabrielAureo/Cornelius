using UnityEngine;
using DG.Tweening;

public class CameraControl : MonoBehaviour{

    [SerializeField] Transform initialTarget;
    [SerializeField] CameraSettings defaultSettings;
    Tweener followMotion;

    float posZ;
    Transform target;
    CameraSettings settings;

    void Awake(){
        ChangeTargetFollow(initialTarget);
        settings = defaultSettings;
        posZ = transform.position.z;
    }

    void Update(){
        if(target != null && false){
            var pos = target.position + settings.offset;
            //transform.position = new Vector3(pos.x, pos.y, posZ);
        }
        

    }

    public void SetTarget(Transform target,bool interpolate = false, CameraSettings settings = null){
        if(settings == null){ 
            settings = defaultSettings;
        }else{
            this.settings = settings;
        }
        Camera.main.DOOrthoSize(settings.size, .2f);
        if(interpolate){
            var newPos = target.position + settings.offset;
            //transform.DOMove(new Vector3(newPos.x, newPos.y, posZ), .2f).SetEase(Ease.InOutSine).
            //OnComplete(()=>
            ChangeTargetFollow(target);
        }else{
            ChangeTargetFollow(target);
        }
    }

    void ChangeTargetFollow(Transform _target){
        if(followMotion != null){
            followMotion.Kill();
        }
        target = _target;

        print( GetCameraSpeed());
        followMotion = transform.DOMove(new Vector3(target.position.x, target.position.y, posZ), .2f).
        SetEase(Ease.InOutSine).
        SetUpdate(UpdateType.Fixed);
        followMotion.OnUpdate(()=>{
            followMotion.ChangeEndValue(new Vector3(target.position.x, target.position.y, posZ),.05f, true);
        });
    }

    float GetCameraSpeed(){
        return .2f - 1/(Vector2.Distance(transform.position, target.position));
    }

    

    public void ReturnToPlayer(){
        SetTarget(GameManager.instance.player.transform, true, defaultSettings);
    }
}