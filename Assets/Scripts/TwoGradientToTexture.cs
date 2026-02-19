using System;
using UnityEngine;

[ExecuteInEditMode]
public class TwoGradientToTexture : MonoBehaviour
{
	private void BuildGradient()
	{
		this.gradientTexture.Reinitialize(this.pixelWidth, 1);
		for (int i = 0; i < this.pixelWidth; i++)
		{
			if ((float)i / (float)this.pixelWidth < 0.5f)
			{
				this.gradientTexture.SetPixel(i, 0, this.firstGradient.Evaluate((float)i / ((float)(this.pixelWidth - 1) * 0.5f)));
			}
			else
			{
				this.gradientTexture.SetPixel(i, 0, this.secondGradient.Evaluate(((float)i - (float)(this.pixelWidth - 1) * 0.5f) / ((float)(this.pixelWidth - 1) * 0.5f)));
			}
		}
		this.gradientTexture.Apply();
	}

	public Texture2D Initialize()
	{
		this.gradientTexture = new Texture2D(this.pixelWidth, 2);
		this.BuildGradient();
		return this.gradientTexture;
	}

	public Gradient firstGradient = new Gradient();

	public Gradient secondGradient = new Gradient();

	public Texture2D gradientTexture;

	public int pixelWidth = 64;
}
