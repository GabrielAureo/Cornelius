using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public enum CameraBehaviour{FollowTarget, Anchored}

public class CameraControl : MonoBehaviour{

    [SerializeField] Transform initialTarget;
    [SerializeField] CameraSettings defaultSettings;
    Tweener motion;
    CameraBehaviour state;

    float posZ;
    Transform target;
    CameraSettings settings;

    void Awake(){
        ChangeTargetFollow(initialTarget);
        settings = defaultSettings;
        posZ = transform.position.z;
    }


    public void SetMode(CameraBehaviour mode, Transform target, bool interpolate = true, UnityAction interpolateCallback = null){
        state = mode;
        switch(state){
            case CameraBehaviour.Anchored:
                motion.Kill();
                if(interpolate){
                    motion = transform.DOMove(target.position, 1f);
                    if(interpolateCallback != null){
                        interpolateCallback();
                    }
                }else{
                    transform.position = target.position;
                }
                break;
            case CameraBehaviour.FollowTarget:
                SetTarget(target, interpolate);
                break;
        }
    }
    public void Shake(float duration){
        if(motion.IsPlaying()){
            motion.onComplete += () => transform.DOShakePosition(duration, .5f, 2, 45,false, false).SetEase(Ease.OutSine);
        }
        
    }
    public void SetTarget(Transform target,bool interpolate = true, CameraSettings settings = null){
        if(state == CameraBehaviour.Anchored) return;
        if(settings == null){ 
            settings = defaultSettings;
        }else{
            this.settings = settings;
        }
        //Camera.main.DOOrthoSize(settings.size, .2f);
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
        if(motion != null){
            motion.Kill();
        }
        target = _target;

        motion = transform.DOMove(new Vector3(target.position.x, target.position.y, posZ), .2f).
        SetEase(Ease.InOutSine).
        SetUpdate(UpdateType.Fixed);
        motion.OnUpdate(()=>{
            motion.ChangeEndValue(new Vector3(target.position.x, target.position.y, posZ),.05f, true);
        });
    }

    float GetCameraSpeed(){
        return .2f - 1/(Vector2.Distance(transform.position, target.position));
    }

    

    public void ReturnToPlayer(){
        SetTarget(GameManager.instance.player.transform, true, defaultSettings);
    }
}