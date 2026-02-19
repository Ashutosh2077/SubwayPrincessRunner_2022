using System;
using System.Collections;
using UnityEngine;

public class Tunnel : MonoBehaviour
{
	private void Awake()
	{
		this.game = Game.Instance;
		if (this.start == null || this.end == null)
		{
			this.tunnelLength = base.GetComponent<Collider>().bounds.size.z;
		}
		else if (this.start && this.end)
		{
			this.tunnelLength = this.end.position.z - this.start.position.z;
			this.slope = (this.end.position.y - this.start.position.y) / this.tunnelLength;
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.CompareTag("Player"))
		{
			this.inTunnel = true;
			Tunnel.TunnelType tunnelType = this.type;
			if (tunnelType != Tunnel.TunnelType.Tunnel)
			{
				if (tunnelType != Tunnel.TunnelType.Upstair)
				{
					if (tunnelType == Tunnel.TunnelType.Downstair)
					{
						this.game.Running.StartDownstair(this.tunnelLength, this.slope);
						this.fadeDistance = this.fadeInOut.position.z - this.start.position.z;
						this.startZ = this.start.position.z;
						base.StartCoroutine(this.FadeInOut(1f, 0.5f, this.fadeDistance));
					}
				}
				else
				{
					this.game.Running.StartUpstair(this.tunnelLength, this.slope);
					this.fadeDistance = this.end.position.z - this.fadeInOut.position.z;
					this.startZ = this.fadeInOut.position.z;
					base.StartCoroutine(this.FadeInOut(0.5f, 1f, this.fadeDistance));
				}
			}
			else
			{
				this.game.Running.StartTunnel(this.tunnelLength);
			}
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (collider.CompareTag("Player"))
		{
			Tunnel.TunnelType tunnelType = this.type;
			if (tunnelType != Tunnel.TunnelType.Upstair)
			{
				if (tunnelType == Tunnel.TunnelType.Downstair)
				{
					this.game.Running.EndDownstair();
				}
			}
			else
			{
				this.game.Running.EndUpstair();
			}
			this.inTunnel = false;
		}
	}

	private IEnumerator FadeInOut(float start, float end, float distance)
	{
		while (this.game.character.z < this.startZ)
		{
			yield return null;
		}
		float factor = 0f;
		float rate = start;
		while (factor < 1f)
		{
			if (!this.inTunnel)
			{
				factor = 1f;
			}
			else
			{
				factor = (this.game.character.z - this.startZ) / distance;
			}
			rate = Mathf.SmoothStep(start, end, factor);
			InitAssets.Instance.FieolnPubWmhniTmjfkwVyduit(rate);
			for (int i = 0; i < 5; i++)
			{
				yield return null;
			}
		}
		yield break;
	}

	public float GetYFromZ(float z)
	{
		if (z < this.tunnelLength)
		{
			return z * this.slope + this.start.position.y;
		}
		return this.end.position.y;
	}

	public float GetDeltaY(float deltaZ)
	{
		return deltaZ * this.slope;
	}

	public float GetEndY()
	{
		if (this.end == null)
		{
			return 0f;
		}
		return this.end.position.y;
	}

	public Tunnel.TunnelType type;

	private Game game;

	private float tunnelLength;

	public float slope = 1f;

	public Transform start;

	public Transform end;

	public Transform fadeInOut;

	private float fadeDistance;

	private float startZ;

	private bool inTunnel;

	public enum TunnelType
	{
		Tunnel,
		Upstair,
		Downstair,
		None
	}
}
