using System.Collections;
using UnityEngine;

public class ResetCameraDirection : MonoBehaviour {

    public Transform innerCamera;

    void Start() {
        Invoke("RotateToFront", 2);
    }

    public void RotateToFront() {
        var vec1 = Vector3.ProjectOnPlane(Vector3.forward, Vector3.up);
        var vec2 = Vector3.ProjectOnPlane(innerCamera.forward, Vector3.up);
        var q = Quaternion.FromToRotation(vec2, vec1);
        transform.Rotate(0, q.eulerAngles.y, 0);
    }

    void Update() {
        if (Input.GetKeyUp(KeyCode.M)) {
            RotateToFront();
        }
    }
}
