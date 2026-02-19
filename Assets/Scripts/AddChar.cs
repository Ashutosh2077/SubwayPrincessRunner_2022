using System;
using System.Collections;
using UnityEngine;

public class AddChar : MonoBehaviour
{
	[ContextMenu("Add Chars")]
	public void ResetBonesForAllchild()
	{
		foreach (SkinnedMeshRenderer childSkin in this.childSkins)
		{
			this.ResetSkinnedMeshBones(childSkin);
		}
	}

	public void ResetSkinnedMeshBones(SkinnedMeshRenderer childSkin)
	{
		Transform[] array = new Transform[childSkin.bones.Length];
		for (int i = 0; i < childSkin.bones.Length; i++)
		{
			array[i] = this.FindNameInChild(childSkin.bones[i].name, this.parent);
		}
		childSkin.rootBone = this.parent.Find("Bip001 Pelvis");
		childSkin.bones = array;
	}

	private Transform FindNameInChild(string name, Transform parentRoot)
	{
		if (string.Equals(name, parentRoot.name))
		{
			return parentRoot;
		}
		IEnumerator enumerator = parentRoot.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Transform parentRoot2 = (Transform)obj;
				Transform transform = this.FindNameInChild(name, parentRoot2);
				if (transform)
				{
					return transform;
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		return null;
	}

	public SkinnedMeshRenderer[] childSkins;

	public Transform parent;
}
