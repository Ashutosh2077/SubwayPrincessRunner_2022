using UnityEngine;
using System.Collections.Generic;

public class BonePoseFollower : MonoBehaviour
{
    [System.Serializable]
    public class BoneMap
    {
        public Transform source;
        public Transform target;

        public bool followPosition;
        public bool followRotation = true;
    }

    public List<BoneMap> boneMaps = new List<BoneMap>();

    void LateUpdate()
    {
        foreach (var map in boneMaps)
        {
            if (!map.source || !map.target) continue;

            if (map.followPosition)
                map.target.localPosition = map.source.localPosition;

            if (map.followRotation)
                map.target.localRotation = map.source.localRotation;
        }
    }
}