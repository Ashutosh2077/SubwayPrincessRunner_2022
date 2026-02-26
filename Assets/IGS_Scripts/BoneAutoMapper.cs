using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BonePoseFollower))]
public class BoneAutoMapper : MonoBehaviour
{
    public Transform sourceRoot;   // Bip001 / Slick
    public Transform targetRoot;   // Any character

    [ContextMenu("Auto Map Bones (Universal)")]
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

        Dictionary<string, Transform> targetLookup = new Dictionary<string, Transform>();

        foreach (var t in targetBones)
        {
            if (ShouldIgnore(t.name)) continue;

            string clean = CleanBoneName(t.name);
            if (!targetLookup.ContainsKey(clean))
                targetLookup.Add(clean, t);
        }

        int mapped = 0;

        foreach (var s in sourceBones)
        {
            if (ShouldIgnore(s.name)) continue;

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
                    followPosition = followPosition,
                    // followRotation = true
                });

                mapped++;
            }
        }

        Debug.Log($"✅ Auto-mapped {mapped} bones for {targetRoot.name}");
    }

    // ------------------ HELPERS ------------------

    string CleanBoneName(string name)
    {
        name = name.ToLower();

        // Remove animation system prefixes
        name = name.Replace("bip001", "");
        name = name.Replace("armature", "");
        name = name.Replace("mixamorig", "");

        // Remove numeric character prefixes (boy 1, boy, elephant, etc.)
        // Keep only the LAST meaningful words
        string[] parts = name.Split(' ');
        if (parts.Length > 1)
        {
            name = parts[parts.Length - 2] + parts[parts.Length - 1];
        }

        name = name.Replace("_", "");
        name = name.Replace(" ", "");

        return name;
    }

    bool ShouldIgnore(string name)
    {
        name = name.ToLower();

        // Ignore non-skeleton / accessory bones
        return
            name.Contains("mesh") ||
            name.Contains("root") ||
            name.Contains("backpack") ||
            name.Contains("strap") ||
            name.Contains("hoodie") ||
            name.Contains("zipper") ||
            name.Contains("lace") ||
            name.Contains("shoelace") ||
            name.Contains("headphone") ||
            name.Contains("prop");
    }
}