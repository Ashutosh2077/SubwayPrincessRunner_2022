using System;
using System.Collections;
using UnityEngine;

public class LongMagnet : MonoBehaviour
{
	public void Activate()
	{
		this.longMagnetSuction.Add(this);
	}

	public void CoinHit(Collider collider)
	{
		if (!this.game.IsInFlypackMode)
		{
			Coin component = collider.GetComponent<Coin>();
			if (component != null)
			{
				component.GetComponent<Collider>().enabled = false;
				if (component.meshRenderer != null)
				{
					component.meshRenderer.enabled = false;
				}
				base.StartCoroutine(this.Pull(component));
			}
			else
			{
				IPickup componentInChildren = collider.GetComponentInChildren<IPickup>();
				if (componentInChildren != null && !(componentInChildren is BoundJumpPickup))
				{
					componentInChildren.NotifyPickup(this.character.CharacterPickupParticleSystem);
				}
			}
		}
	}

	public void Deactivate()
	{
		this.longMagnetSuction.Remove(this);
	}

	private IEnumerator Pull(Coin coin)
	{
		Vector3 coinPosition = coin.PivotTransform.position;
		Vector3 vector = coinPosition - this.characterController.transform.position;
		Vector3 offsetCoinHitPosition = new Vector3(0f, -6f, 0f);
		yield return base.StartCoroutine(myTween.To(vector.magnitude / (this.pullSpeed * this.game.NormalizedGameSpeed), delegate(float t)
		{
			coin.PivotTransform.position = Vector3.Lerp(coinPosition, this.characterModel.meshSuperShoes.transform.position + offsetCoinHitPosition, t * t);
		}));
		IPickup pickup = coin.GetComponent<IPickup>();
		if (pickup != null)
		{
			this.character.NotifyPickup(pickup);
		}
		yield break;
	}

	public void DelegteInGameOne(bool _value)
	{
		if (_value)
		{
			this.longMagnetSuction.Clear();
		}
	}

	public void DelegteInGameTwo(bool _value)
	{
		if (_value)
		{
			this.coinMagnetCollider.OnEnter = new OnTriggerObject.OnEnterDelegate(this.CoinHit);
			this.coinMagnetCollider.GetComponent<Collider>().enabled = true;
		}
		else
		{
			this.coinMagnetCollider.GetComponent<Collider>().enabled = false;
			this.coinMagnetCollider.OnEnter = (OnTriggerObject.OnEnterDelegate)Delegate.Remove(this.coinMagnetCollider.OnEnter, new OnTriggerObject.OnEnterDelegate(this.CoinHit));
		}
	}

	public void Start()
	{
		this.character = Character.Instance;
		this.characterRendering = CharacterRendering.Instance;
		this.characterModel = this.characterRendering.CharacterModel;
		this.coinMagnetCollider = this.character.coinMagnetLongCollider;
		this.characterController = this.character.characterController;
		this.game = Game.Instance;
		this.game.IsInGame.OnChange = (Variable<bool>.OnChangeDelegate)Delegate.Combine(this.game.IsInGame.OnChange, new Variable<bool>.OnChangeDelegate(this.DelegteInGameOne));
		this.longMagnetSuction.OnChange = (VariableBool.OnChangeDelegate)Delegate.Combine(this.longMagnetSuction.OnChange, new VariableBool.OnChangeDelegate(this.DelegteInGameTwo));
	}

	public bool ShouldBeActive
	{
		get
		{
			return Helmet.Instance.IsActive || SuperShoes.Instance.IsActive;
		}
	}

	private Character character;

	private CharacterController characterController;

	private CharacterModel characterModel;

	private CharacterRendering characterRendering;

	private OnTriggerObject coinMagnetCollider;

	private Game game;

	private VariableBool longMagnetSuction = new VariableBool();

	public float pullSpeed = 200f;
}
