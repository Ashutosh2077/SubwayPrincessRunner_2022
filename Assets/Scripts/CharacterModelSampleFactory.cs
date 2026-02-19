using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModelSampleFactory : MonoBehaviour
{
	private GameObject BuildCharacter(string name, int version)
	{
		GameObject gameObject = null;
		if (!this.CMPreviews.TryGetValue(name, out gameObject))
		{
			gameObject = UnityEngine.Object.Instantiate<GameObject>(this.characterModelSamplePrefab);
			this.CMPreviews.Add(name, gameObject);
		}
		this.SelectCustom(gameObject, name, version);
		return gameObject;
	}

	public GameObject GetCharacterModelSample(string name, int version)
	{
		return this.BuildCharacter(name, version);
	}

	public CharacterModelSample BuildCharacterModelSample(string name, int version)
	{
		return this.BuildCharacterSample(name, version);
	}

	private CharacterModelSample BuildCharacterSample(string name, int version)
	{
		GameObject go = UnityEngine.Object.Instantiate<GameObject>(this.characterModelSamplePrefab);
		return this.SelectCustom(go, name, version);
	}

	private CharacterModelSample SelectCustom(GameObject go, string name, int version)
	{
		CharacterModelSample component = go.GetComponent<CharacterModelSample>();
		for (int i = 0; i < component.Customs.Length; i++)
		{
			if (component.Customs[i].name.Equals(name))
			{
				component.Customs[i].customSets.gameObject.SetActive(true);
				component.Customs[i].customSets.ChangeSkin(version);
			}
			else
			{
				component.Customs[i].customSets.gameObject.SetActive(false);
			}
		}
		return component;
	}

	public static CharacterModelSampleFactory Instance
	{
		get
		{
			if (CharacterModelSampleFactory.instance == null)
			{
				CharacterModelSampleFactory.instance = (UnityEngine.Object.FindObjectOfType(typeof(CharacterModelSampleFactory)) as CharacterModelSampleFactory);
			}
			return CharacterModelSampleFactory.instance;
		}
	}

	[SerializeField]
	private GameObject characterModelSamplePrefab;

	private Dictionary<string, GameObject> CMPreviews = new Dictionary<string, GameObject>();

	private static CharacterModelSampleFactory instance;
}
