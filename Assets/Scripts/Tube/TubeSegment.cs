using System.Collections.Generic;
using UnityEngine;

public class TubeSegment : MonoBehaviour {

    public List<GameObject> obstacles = new List<GameObject>();

    public void SetState(SegmentType type) {
        for (int i = 0; i < obstacles.Count; i++) {
            obstacles[i].SetActive(i == (int)type);
        }
    }
}
