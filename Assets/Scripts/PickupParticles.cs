using System;
using System.Collections;
using UnityEngine;

public class PickupParticles : MonoBehaviour
{
	private void Start()
	{
		this.flypack = Flypack.Instance;
		this.flypack.OnStop = (Flypack.OnStopDelegate)Delegate.Combine(this.flypack.OnStop, new Flypack.OnStopDelegate(this.OnFlypackStop));
	}

	private IEnumerator EffectCoroutine(PickupParticles.EffectDetails details)
	{
		details.target.Rotate(0f, 0f, UnityEngine.Random.Range(0f, 360f));
		Renderer render = details.target.GetComponent<Renderer>();
		Material material = render.sharedMaterial;
		yield return base.StartCoroutine(myTween.To(details.duration, delegate(float t)
		{
			render.enabled = true;
			material.SetColor(Shaders.Instance.MainColor, Color.Lerp(Color.white, Color.black, t));
			details.target.localScale = Vector3.one * details.scaleCurve.Evaluate(t);
		}));
		render.enabled = false;
		yield break;
	}

	private void OnDestroy()
	{
		this.flypack.OnStop = (Flypack.OnStopDelegate)Delegate.Remove(this.flypack.OnStop, new Flypack.OnStopDelegate(this.OnFlypackStop));
	}

	private void OnFlypackStop()
	{
		this.flyCount = 0;
	}

	public void PickedupCoin(IPickup pickup)
	{
		float pitch = AudioPlayer.Instance.GetAudioSource("leyou_Hr_coin").pitch;
		if (80f < pickup.transform.position.y)
		{
			this.coinStairway = 0;
			pitch = Mathf.Pow(2f, this.compressCurve.Evaluate((float)this.flyCount / 48f));
			this.flyCount++;
		}
		else if (pickup.transform.position.y < 0.1f || (8.795f < pickup.transform.position.y && pickup.transform.position.y < 8.805f) || (9.95f < pickup.transform.position.y && pickup.transform.position.y < 10.05f) || (28.95f < pickup.transform.position.y && pickup.transform.position.y < 29.05f) || (34.95f < pickup.transform.position.y && pickup.transform.position.y < 35.05f))
		{
			this.flyCount = 0;
			this.coinStairway = 0;
			pitch = Mathf.Pow(2f, (float)this.slendro[this.coinStairway % this.slendro.Length] / 12f) * 0.5f;
		}
		else
		{
			this.flyCount = 0;
			if (this.coinStairway < this.slendro.Length - 1)
			{
				this.coinStairway++;
			}
			pitch = Mathf.Pow(2f, (float)this.slendro[this.coinStairway % this.slendro.Length] / 12f) * 0.5f;
		}
		AudioPlayer.Instance.PlaySound("leyou_Hr_coin", pitch, pickup.transform.position);
		base.StartCoroutine(this.EffectCoroutine(this.coinEffect));
	}

	public void PickedupDefaultPowerUp()
	{
		base.StartCoroutine(this.EffectCoroutine(this.powerupEffect));
	}

	public void PickedupPowerUp()
	{
		AudioPlayer.Instance.PlaySound("leyou_Hr_powerUp", true);
		this.PickedupDefaultPowerUp();
	}

	public GameObject CoinEFX;

	public GameObject PowerUpEFX;

	public AnimationCurve compressCurve;

	[SerializeField]
	private PickupParticles.EffectDetails coinEffect;

	[SerializeField]
	private PickupParticles.EffectDetails powerupEffect;

	public static Vector3 coinEfxOffset = 1.2f * Vector3.forward;

	private int coinStairway;

	private int flyCount;

	private Flypack flypack;

	private int[] slendro = new int[]
	{
		12,
		13,
		14,
		15,
		16,
		17,
		18,
		19,
		20,
		21,
		22,
		23,
		24,
		25,
		26,
		27,
		28
	};

	[Serializable]
	private class EffectDetails
	{
		public Transform target;

		public float duration = 0.05f;

		public AnimationCurve scaleCurve;
	}
}
