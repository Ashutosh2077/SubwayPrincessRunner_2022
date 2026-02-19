using System;
using UnityEngine;

public class CharacterCustomization : MonoBehaviour
{
	public void Customize(string name, int themeIndex, ref SkinnedMeshRenderer model)
	{
		for (int i = 0; i < this.Customs.Length; i++)
		{
			if (this.Customs[i].name.Equals(name))
			{
				this.Customs[i].customSets.gameObject.SetActive(true);
				this.Customs[i].customSets.ChangeSkin(themeIndex);
				model = this.Customs[i].customSets.body;
			}
			else
			{
				this.Customs[i].customSets.gameObject.SetActive(false);
			}
		}
	}

	private Texture2D gradient;

	public CharacterCustomization.CustomControl[] Customs;

	[Serializable]
	public class CustomControl
	{
		public string name;

		public CustomizeControl customSets;
	}
}
