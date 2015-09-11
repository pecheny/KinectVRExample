using UnityEngine;

public class InputTester : MonoBehaviour {
    void Update() {
        transform.localPosition = GetComponent<KinectInputSource>().GetPosition();
    }

}
