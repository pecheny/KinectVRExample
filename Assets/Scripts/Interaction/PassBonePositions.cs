using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kinect = Windows.Kinect;

public class PassBonePositions : MonoBehaviour {
    public Material BoneMaterial;
    public GameObject BodySourceManager;

    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;
    public GameObject bodyView;
    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        {Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft}, {Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft}, {Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft}, {Kinect.JointType.HipLeft, Kinect.JointType.SpineBase},

        {Kinect.JointType.FootRight, Kinect.JointType.AnkleRight}, {Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight}, {Kinect.JointType.KneeRight, Kinect.JointType.HipRight}, {Kinect.JointType.HipRight, Kinect.JointType.SpineBase},

        {Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft}, {Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft}, {Kinect.JointType.HandLeft, Kinect.JointType.WristLeft}, {Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft}, {Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft}, {Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder},

        {Kinect.JointType.HandTipRight, Kinect.JointType.HandRight}, {Kinect.JointType.ThumbRight, Kinect.JointType.HandRight}, {Kinect.JointType.HandRight, Kinect.JointType.WristRight}, {Kinect.JointType.WristRight, Kinect.JointType.ElbowRight}, {Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight}, {Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder},

        {Kinect.JointType.SpineBase, Kinect.JointType.SpineMid}, {Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder}, {Kinect.JointType.SpineShoulder, Kinect.JointType.Neck}, {Kinect.JointType.Neck, Kinect.JointType.Head},
    };

    void Update() {
        if (BodySourceManager == null) {
            return;
        }
        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null) {
            return;
        }
        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null) {
            return;
        }
        List<ulong> trackedIds = new List<ulong>();

        if (!HasBody()) {
            foreach (var body in data) {
                if (body == null) {
                    continue;
                }
                if (body.IsTracked) {
                    trackedIds.Add(body.TrackingId);
                }
            }
        }
        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);
        // First delete untracked bodies
        foreach (ulong trackingId in knownIds) {
            if (!trackedIds.Contains(trackingId)) {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }
        foreach (var body in data) {
            if (body == null) {
                continue;
            }
            if (body.IsTracked) {
                if (!_Bodies.ContainsKey(body.TrackingId)) {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }

                RefreshBodyObject(body, _Bodies[body.TrackingId]);
            }
        }
    }

    public bool HasBody() {
        return _Bodies.Count > 0;
    }

    private GameObject CreateBodyObject(ulong id) {
        GameObject body = new GameObject("Body:" + id);

        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++) {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);

            LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            lr.SetVertexCount(2);
            lr.material = BoneMaterial;
            lr.SetWidth(0.05f, 0.05f);

            jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;
        }

        return body;
    }

    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject) {
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++) {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Transform jointObj = bodyView.transform.FindChild(jt.ToString());
            if (jointObj == null) continue;
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);
        }
    }



    private static Vector3 GetVector3FromJoint(Kinect.Joint joint) {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }
}
