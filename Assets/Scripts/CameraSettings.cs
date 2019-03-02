using UnityEngine;
[System.Serializable]
public class CameraSettings{
    public Vector3 offset;
    public float size;

    public CameraSettings(Vector3 offset, float size){
        this.offset = offset;
        this.size = size;
    }

}