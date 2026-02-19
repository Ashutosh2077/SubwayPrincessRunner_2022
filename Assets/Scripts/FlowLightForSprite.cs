using System;
using UnityEngine;

public class FlowLightForSprite : MonoBehaviour
{
	private void Start()
	{
		UISprite component = base.GetComponent<UISprite>();
		UISpriteData atlasSprite = component.GetAtlasSprite();
		if (component.mGlitterMaterial != null)
		{
			component.mGlitterMaterial.SetFloat(Shaders.Instance.WidthRate, (float)atlasSprite.width / (float)component.atlas.texture.width);
			component.mGlitterMaterial.SetFloat(Shaders.Instance.HeightRate, (float)atlasSprite.height / (float)component.atlas.texture.height);
			component.mGlitterMaterial.SetFloat(Shaders.Instance.OffsetXRate, (float)atlasSprite.x / (float)component.atlas.texture.width);
			component.mGlitterMaterial.SetFloat(Shaders.Instance.OffsetYRate, (float)(component.atlas.texture.height - atlasSprite.y - atlasSprite.height) / (float)component.atlas.texture.height);
		}
		if (component.mGlitterMaterial != null)
		{
			component.mGlitterMaterial.SetTexture(Shaders.Instance.MainTex, component.atlas.texture);
		}
	}
}
