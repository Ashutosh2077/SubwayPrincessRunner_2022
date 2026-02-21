using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BonePoseFollower))]
public class BoneAutoMapper : MonoBehaviour
{
    public Transform sourceRoot;   // Bip001 / Slick
    public Transform targetRoot;   // Any animal

    [ContextMenu("Auto Map Bones (Multi-Animal Safe)")]
    public void AutoMapBones()
    {
        if (!sourceRoot || !targetRoot)
        {
            Debug.LogError("SourceRoot or TargetRoot missing");
            return;
        }

        var follower = GetComponent<BonePoseFollower>();
        follower.boneMaps.Clear();

        var sourceBones = sourceRoot.GetComponentsInChildren<Transform>(true);
        var targetBones = targetRoot.GetComponentsInChildren<Transform>(true);

        // Build lookup table for target bones
        Dictionary<string, Transform> targetLookup = new Dictionary<string, Transform>();

        foreach (var t in targetBones)
        {
            string clean = CleanBoneName(t.name);
            if (!targetLookup.ContainsKey(clean))
                targetLookup.Add(clean, t);
        }

        int mapped = 0;

        foreach (var s in sourceBones)
        {
            string cleanSource = CleanBoneName(s.name);

            if (targetLookup.TryGetValue(cleanSource, out Transform target))
            {
                bool followPosition =
                    cleanSource.Contains("pelvis") ||
                    cleanSource.Contains("hips");

                follower.boneMaps.Add(new BonePoseFollower.BoneMap
                {
                    source = s,
                    target = target,
                    followPosition = followPosition, // ONLY root
                    followRotation = true
                });

                mapped++;
            }
        }

        Debug.Log($"✅ Auto-mapped {mapped} bones for {targetRoot.name}");
    }

    // 🔑 THE IMPORTANT PART
    string CleanBoneName(string name)
    {
        name = name.ToLower();

        // Remove animation rig prefixes
        name = name.Replace("bip001", "");
        name = name.Replace("armature", "");
        name = name.Replace("mixamorig", "");

        // Remove first word prefix (animal name)
        // Example: "elephant l thigh" → "l thigh"
        int firstSpace = name.IndexOf(" ");
        if (firstSpace > 0)
            name = name.Substring(firstSpace + 1);

        // Normalize
        name = name.Replace("_", "");
        name = name.Replace(" ", "");

        return name;
    }
}