using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class TestAnimation : MonoBehaviour
{
	private void Start()
	{
		this._Animation = base.GetComponent<Animation>();
	}

	private void OnGUI()
	{
		if (GUILayout.Button("1", new GUILayoutOption[0]))
		{
			this._Animation.CrossFadeQueued(this.Animations[0].name);
		}
		if (GUILayout.Button("2", new GUILayoutOption[0]))
		{
			this._Animation.Stop();
		}
		if (GUILayout.Button("3", new GUILayoutOption[0]))
		{
			this._Animation["run2"].enabled = true;
		}
		if (GUILayout.Button("4", new GUILayoutOption[0]))
		{
			IEnumerator enumerator = this._Animation.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					AnimationState animationState = (AnimationState)obj;
					UnityEngine.Debug.Log(animationState.clip.name);
					UnityEngine.Debug.Log(animationState.name);
					UnityEngine.Debug.Log(animationState.blendMode);
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
		}
		if (GUILayout.Button("5", new GUILayoutOption[0]))
		{
			this._Animation[this.Animations[0].name].wrapMode = WrapMode.PingPong;
			this._Animation.Play(this.Animations[0].name);
		}
	}

	public AnimationClip[] Animations;

	private Animation _Animation;
}
