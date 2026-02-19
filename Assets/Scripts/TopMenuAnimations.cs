using System;
using UnityEngine;

public class TopMenuAnimations : MonoBehaviour
{
	private void Start()
	{
	}

	public void AddPlayerOnStopEvent(Action action)
	{
		PointsManager pointsManager = this.player;
		pointsManager.onStop = (Action)Delegate.Combine(pointsManager.onStop, action);
	}

	public void AddEnemyOnStopEvent(Action action)
	{
		PointsManager pointsManager = this.enemy;
		pointsManager.onStop = (Action)Delegate.Combine(pointsManager.onStop, action);
	}

	public void RemovePlayerOnStopEvent(Action action)
	{
		PointsManager pointsManager = this.player;
		pointsManager.onStop = (Action)Delegate.Remove(pointsManager.onStop, action);
	}

	public void RemoveEnemyOnStopEvent(Action action)
	{
		PointsManager pointsManager = this.enemy;
		pointsManager.onStop = (Action)Delegate.Remove(pointsManager.onStop, action);
	}

	public void StartPlayIdleRummagesAnimation()
	{
		base.enabled = true;
		int i = 0;
		int num = this.managers.Length;
		while (i < num)
		{
			this.managers[i].Play();
			i++;
		}
		this.player.Play();
		this.enemy.Play();
	}

	public void StopPlayIdleRummagesAnimation()
	{
		base.enabled = false;
	}

	public void OnNewGameStart()
	{
		int i = 0;
		int num = this.managers.Length;
		while (i < num)
		{
			this.managers[i].BreakLoop();
			i++;
		}
		this.player.BreakLoop();
		this.enemy.BreakLoop();
	}

	public void Continue()
	{
		int i = 0;
		int num = this.managers.Length;
		while (i < num)
		{
			this.managers[i].Wait = false;
			i++;
		}
	}

	[SerializeField]
	private PointsManager[] managers;

	[SerializeField]
	private PointsManager player;

	[SerializeField]
	private PointsManager enemy;
}
