using System;
using UnityEngine;

[Serializable]
public class KeyFrameAudio : KeyFrameBase
{
	public AudioKeyFrameType audioKeyFrameType;

	public string audio;

	public Transform point;

	public KeyFrameAudio.ExtraKeyframeCall Callback;

	public delegate void ExtraKeyframeCall(KeyFrameAudio info);
}
