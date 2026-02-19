using System;
using UnityEngine;

[CreateAssetMenu(menuName = "City")]
[Serializable]
public class City : ScriptableObject
{
	public string cityName;

	public Keepsake[] keepsakes;

	public string sceneName;

	public Vector4 distort;

	public SubScene[] subScenes;

	public int minLength;

	public int minIntervalLength;

	public int maxIntervalLength;

	public bool allowSnow;

	public int order;
}
