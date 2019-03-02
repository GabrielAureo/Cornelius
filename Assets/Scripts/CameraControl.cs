using UnityEngine;
using DG.Tweening;

public class CameraControl : MonoBehaviour{

    [SerializeField] Transform initialTarget;
    [SerializeField] CameraSettings defaultSettings;

    float posZ;
    Transform target;
    CameraSettings settings;

    void Awake(){
        target = initialTarget;
        settings = defaultSettings;
        posZ = transform.position.z;
    }

    void Update(){
        if(target != null){
            var pos = target.position + settings.offset;
            transform.position = new Vector3(pos.x, pos.y, posZ);
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
            transform.DOMove(new Vector3(newPos.x, newPos.y, posZ), .2f).onComplete = () => this.target = target;
        }else{
            this.target = target;
        }
    }

    public void ReturnToPlayer(){
        SetTarget(GameManager.instance.player.transform, true);
    }
}