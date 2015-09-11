using Mono.Xml.Xsl;
using UnityEditor;
using UnityEngine;

public class GameplayMechanics : MonoBehaviour {
    public GameObject levelContainer;
    TubeConfig config;
    TubeSegment[] segments;
    Vector3 segmentPosition = Vector3.zero;
    int firstSegment;
    float visibleLevelLen;
    float pos;

    void Start() {
        config = GameObject.FindObjectOfType<TubeConfig>();
        pos = -config.segmentLength * config.numSegments / 2;
        segments = new TubeSegment[config.numSegments];
        visibleLevelLen  = config.segmentLength * config.numSegments;

        for (int i = 0; i < config.numSegments; i++) {
            var segment = GameObject.Instantiate(config.segmentPrefab).GetComponent<TubeSegment>();
            segment.transform.SetParent(levelContainer.transform);
            segmentPosition.y = config.segmentLength * i;
            SegmentType state = GetSegmentTypeAt(config.numSegments - i);
            segment.SetState(state);
            segment.transform.localPosition = segmentPosition;
            segments[i] = segment;
        }
    }

    void Update() {
        pos += config.speed * Time.deltaTime;
        var currentFirstSegment = Mathf.FloorToInt(pos / config.segmentLength);
        var fullLevelLevelOffsets = pos % visibleLevelLen;
        var indexOffset = config.numSegments - Mathf.FloorToInt(fullLevelLevelOffsets / config.segmentLength);
        var segmentOffset = fullLevelLevelOffsets % config.segmentLength;
        while ( firstSegment <= currentFirstSegment) {
            SegmentType state = GetSegmentTypeAt(firstSegment);
            segments[GetSegmentIdxAt(firstSegment)].SetState(state);
            firstSegment++;
        }
        for (int i = 0; i < segments.Length; i++) {
            if (i > (config.numSegments - indexOffset)) {
                segmentPosition.y = (i + indexOffset - config.numSegments) * config.segmentLength - segmentOffset;
            } else {
                segmentPosition.y = (i + indexOffset) * config.segmentLength - segmentOffset;
            }
            segments[i].transform.localPosition = segmentPosition;
        }
    }
    /// <summary>
    /// Kicks level back if player hits the obstacle
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other) {
        pos = ((firstSegment - 2) * config.segmentLength);
    }


    /// <summary>
    /// Define what kind of obstacle should be placed at certain segment of level.
    /// You can replace this implementation with certain sequence insted of random obstacle in each third segment.
    /// </summary>
    /// <param name="n">Number of segment in level to define</param>
    /// <returns></returns>
    SegmentType GetSegmentTypeAt(int n) {
        if (n % 3 != 0) {
            return SegmentType.Empty;
        } else {
            return (SegmentType) Random.Range(0, 3);
        }
    }

    /// <summary>
    /// Give index of game object which should be used at given place of the level.
    /// </summary>
    /// <param name="n">Index of segment in the level sequence</param>
    /// <returns></returns>
    int GetSegmentIdxAt(int n) {
        return n % config.numSegments;
    }
}
