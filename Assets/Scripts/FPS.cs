using System;
using UnityEngine;

public class FPS : MonoBehaviour
{
	private void Start()
	{
		this.timeleft = this.updateInterval;
	}

	private void Update()
	{
		this.timeleft -= Time.deltaTime;
		this.accum += Time.timeScale / Time.deltaTime;
		this.frames++;
		if ((double)this.timeleft <= 0.0)
		{
			this.fps = this.accum / (float)this.frames;
			this.timeleft = this.updateInterval;
			this.accum = 0f;
			this.frames = 0;
		}
	}

	private void OnGUI()
	{
		if (this.fps < 20f)
		{
			GUI.color = Color.red;
		}
		else if (this.fps < 30f)
		{
			GUI.color = Color.yellow;
		}
		else
		{
			GUI.color = Color.green;
		}
		GUI.Label(new Rect(100f, 100f, 300f, 300f), string.Format("{0:F2} FPS", this.fps));
	}

	public float updateInterval = 0.5f;

	private float accum;

	private int frames;

	private float timeleft;

	private float fps;
}
