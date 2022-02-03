using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public GameObject Target;
    [SerializeField] private int _targetOffsetX;
    [SerializeField] private int _targetOffsetY;
    [SerializeField] private int _targetOffsetZ;
    [SerializeField] private int _cameraRotationX;
    [SerializeField] private int _cameraRotationY;
    [SerializeField] private int _cameraRotationZ;

    private void Update()
    {
        if (Target == null) { return;}
        
        //TODO: CameraZoom
        /*if (Input.mouseScrollDelta.y != 0)
        {
            
            //print(mouseScrollValue);
            //_targetOffsetY -= 
        }
        
        if(Input.GetAxis("Mouse ScrollWheel") != 0) {
            var mouseScrollValue = Vector3.forward * Input.GetAxis("Mouse ScrollWheel") * 20;
            print(mouseScrollValue);
        }*/
        
        transform.position = Target.transform.position + new Vector3(_targetOffsetX, _targetOffsetY, _targetOffsetZ);
        transform.rotation = Quaternion.Euler(_cameraRotationX, _cameraRotationY ,_cameraRotationZ);
    }
}

