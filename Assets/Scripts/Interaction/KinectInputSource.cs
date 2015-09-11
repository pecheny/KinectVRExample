using System.Collections;
using UnityEngine;

public class KinectInputSource : MonoBehaviour {
    public Transform leftHip;
    public Transform rightHip;
    public float R;
    public float scaleFactor = 1;
    Vector3 output = Vector3.zero;
    Vector3 zeroPosition = Vector3.zero;

    void Start() {
        Invoke("StoreOrigin", 2);
    }

    void StoreOrigin() {
        zeroPosition = GetMiddlePos();
    }

    public Vector3 GetPosition() {
        if (Input.GetKey(KeyCode.Space)) {
            StoreOrigin();
        }
        var currentMiddlePos = GetMiddlePos();
        output.x = (currentMiddlePos.x - zeroPosition.x);
        output.z = -(currentMiddlePos.z - zeroPosition.z);
        if (output.magnitude > R) {
            output.Normalize();
            return output * R;
        } else {
            return  output;
        }
    }

    Vector3 GetMiddlePos() {
        return scaleFactor * (leftHip.localPosition + rightHip.localPosition) / 2;
    }
}
