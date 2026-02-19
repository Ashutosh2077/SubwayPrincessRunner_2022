using System;
using System.Collections.Generic;
using UnityEngine;

public class TasksManager
{
	private TasksManager()
	{
		if (this.playerinfo.currentTaskSet == -1)
		{
			this.playerinfo.InitCurrentTaskSet(0, this.tasks[0].Length, true);
		}
	}

	private void CheckAllCompleteAndIncrement()
	{
		for (int i = 3; i < this.combinedArray.Length; i++)
		{
			if (this.templates[i].singleRun && this.templates[i].completeIfLess)
			{
				if (this._currentRunProgress[i] < this.combinedArray[i].aim)
				{
					this.playerinfo.SetCurrentTaskProgress(i, this.combinedArray[i].aim);
					this.Complete(i, 1f, true);
				}
			}
			else if (this._currentRunProgress != null)
			{
				if ((float)this._currentRunProgress[i] / (float)this.combinedArray[i].aim > (float)this.playerinfo.GetCurrentTaskProgress(i) / (float)this.combinedArray[i].aim)
				{
					this.Complete(i, (float)this._currentRunProgress[i] / (float)this.combinedArray[i].aim, true);
				}
				else
				{
					this.Complete(i, (float)this.playerinfo.GetCurrentTaskProgress(i) / (float)this.combinedArray[i].aim, true);
				}
			}
			else
			{
				this.Complete(i, (float)this.playerinfo.GetCurrentTaskProgress(i) / (float)this.combinedArray[i].aim, true);
			}
		}
	}

	private void Complete(int task, float completedFactor = 1f, bool sendEvenIfInRun = false)
	{
		int num = 3;
		if (task >= num)
		{
			if (!this.inRun || sendEvenIfInRun)
			{
				int num2 = task - num;
				if (num2 < 0 || num2 >= Achievements.NUMBER_OF_ACHIEVEMENTS)
				{
					TasksManager.LogError("Tasks.Complete: achievement index is out of bounds", null);
				}
				else if (completedFactor == 1f)
				{
					if (this.onAchievementComplete != null)
					{
						string format = (this.combinedArray[task].aim == 1) ? Strings.Get(this.templates[task].ultraShortDescriptionSingle) : Strings.Get(this.templates[task].ultraShortDescription);
						this.onAchievementComplete(string.Format(format, this.combinedArray[task].aim));
					}
					NotificationsObserver.Instance.NotifyNotificationDataChange(NotificationType.AchiementFinished);
				}
			}
		}
		else if (completedFactor == 1f)
		{
			TasksManager.TaskCompleteHandler taskCompleteHandler = this.onTaskComplete;
			if (taskCompleteHandler != null)
			{
				string format2 = (this.combinedArray[task].aim == 1) ? Strings.Get(this.templates[task].ultraShortDescriptionSingle) : Strings.Get(this.templates[task].ultraShortDescription);
				taskCompleteHandler(string.Format(format2, this.combinedArray[task].aim));
			}
			Statistics stats= PlayerInfo.Instance.stats;
			(stats )[Stat.TaskCompleted] = stats[Stat.TaskCompleted] + 1;
			bool flag = true;
			int num3 = 0;
			for (int i = 0; i < 3; i++)
			{
				int currentTaskProgress = this.playerinfo.GetCurrentTaskProgress(i);
				if (currentTaskProgress >= this.combinedArray[i].aim)
				{
					num3++;
				}
				if (currentTaskProgress < this.combinedArray[i].aim)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				this.PlayerDidThis(TaskTarget.TaskSet, 1, -1);
				if (this._currentRunProgress != null)
				{
					for (int j = 0; j < 3; j++)
					{
						if (this._currentRunProgress[j] != 0)
						{
							this._currentRunProgress[j] = 0;
						}
					}
				}
				if (this.currentTaskSet + 1 > this.taskSetStoryCount)
				{
					RewardManager.AddRewardToUnlock(CelebrationRewardOrigin.SuperChest);
					if (!this.inRun)
					{
						UIScreenController.Instance.QueueChest();
					}
				}
				int num4;
				if (this.currentTaskSet >= this.taskSetCount - 1)
				{
					num4 = this.playerinfo.currentTaskSet - TasksData.repeatableTasks.Length + 1;
				}
				else
				{
					num4 = this.playerinfo.currentTaskSet + 1;
				}
				this.playerinfo.taskCompletedSum++;
				int taskCount = 0;
				if (num4 < this.taskSetCount)
				{
					taskCount = this.tasks[num4].Length;
				}
				this.playerinfo.InitCurrentTaskSet(num4, taskCount, true);
				TasksManager.TaskSetCompleteHandler taskSetCompleteHandler = this.onTaskSetComplete;
				if (taskSetCompleteHandler != null)
				{
					taskSetCompleteHandler();
				}
				this.PlayerDidThis(TaskTarget.ReachTaskSet, 1, -1);
			}
		}
	}

	public TaskInfo[] GetTaskInfo()
	{
		TaskInfo[] array = new TaskInfo[this.combinedArray.Length];
		for (int i = 0; i < this.combinedArray.Length; i++)
		{
			this.GetTaskInfo(i, ref array[i]);
		}
		return array;
	}

	public TaskInfo GetTaskInfo(int taskNumber)
	{
		TaskInfo result = default(TaskInfo);
		this.GetTaskInfo(taskNumber, ref result);
		return result;
	}

	public AchievementInfo GetAchievementInfo(int taskNumber)
	{
		if (taskNumber < 3 || taskNumber >= this.combinedArray.Length)
		{
			return null;
		}
		return Achievements.achievementInfo[taskNumber - 3];
	}

	private void GetTaskInfo(int taskNumber, ref TaskInfo info)
	{
		info.task = this.combinedArray[taskNumber];
		info.template = this.templates[taskNumber];
		info.progress = this.playerinfo.GetCurrentTaskProgress(taskNumber);
		info.complete = (info.progress >= info.task.aim);
		if (!info.complete && info.template.singleRun && this.inRun)
		{
			info.progress = this._currentRunProgress[taskNumber];
		}
	}

	public bool IsTaskTargetActive(TaskTarget target)
	{
		for (int i = 0; i < this.combinedArray.Length; i++)
		{
			if (this.templates[i].taskTarget == target)
			{
				return this.playerinfo.GetCurrentTaskProgress(i) < this.combinedArray[i].aim;
			}
		}
		return false;
	}

	private static void LogError(string msg, UnityEngine.Object context)
	{
		UnityEngine.Debug.LogError(msg, context);
	}

	public void OnChangeIsCharacterOnGround(Transform characterTransform)
	{
		if (characterTransform.localPosition.y < 1f)
		{
			this.RemoveProgressForThis(TaskTarget.EarnCoinWithoutTouchingGround);
		}
	}

	public void PlayerDidThis(TaskTarget myTask, int magnitude = 1, int taskToIgnore = -1)
	{
		if (this.templates == null)
		{
			TasksManager.LogError("currentTemplates == null", null);
		}
		for (int i = 0; i < this.combinedArray.Length; i++)
		{
			if (i != taskToIgnore && (!this.templates[i].singleRun || this.inRun) && this.templates[i].taskTarget == myTask)
			{
				int num = this.playerinfo.GetCurrentTaskProgress(i);
				if (this.templates[i].singleRun && this.inRun && num < this.combinedArray[i].aim && this._currentRunProgress != null)
				{
					num = this._currentRunProgress[i];
				}
				int num2 = num + magnitude;
				if (this.templates[i].completeIfLess)
				{
					if (num2 > this.combinedArray[i].aim)
					{
						if (this.templates[i].singleRun)
						{
							if (this._currentRunProgress != null)
							{
								this._currentRunProgress[i] = num2;
							}
							else
							{
								TasksManager.LogError("_currentRunProgress is null - PlayerDidThis called outside a run for a singleRun task with TaskTarget: " + myTask, null);
							}
						}
						else
						{
							this.playerinfo.SetCurrentTaskProgress(i, num2);
							this.Complete(i, (float)num2 / (float)this.combinedArray[i].aim, false);
						}
					}
					else
					{
						if (this.templates[i].singleRun && this.inRun)
						{
							if (this._currentRunProgress != null)
							{
								this._currentRunProgress[i] = num2;
							}
							else
							{
								TasksManager.LogError("_currentRunProgress is null - PlayerDidThis called outside a run for a singleRun task with TaskTarget: " + myTask, null);
							}
						}
						this.playerinfo.SetCurrentTaskProgress(i, this.combinedArray[i].aim);
						this.Complete(i, 1f, false);
					}
				}
				else if (this.templates[i].completeIfEqual)
				{
					if (myTask == TaskTarget.GetExactlyAmountOfCoins)
					{
						num2 = magnitude;
					}
					if (num2 == this.combinedArray[i].aim)
					{
						this.playerinfo.SetCurrentTaskProgress(i, this.tasks[this.playerinfo.currentTaskSet][i].aim);
						this.Complete(i, 1f, false);
					}
				}
				else if (num2 < this.combinedArray[i].aim)
				{
					if (this.templates[i].singleRun)
					{
						if (this._currentRunProgress != null)
						{
							this._currentRunProgress[i] = num2;
						}
						else
						{
							TasksManager.LogError("_currentRunProgress is null - PlayerDidThis called outside a run for a singleRun task with TaskTarget: " + myTask, null);
						}
					}
					else
					{
						if (this._currentRunProgress != null)
						{
							this._currentRunProgress[i] = num2;
						}
						this.playerinfo.SetCurrentTaskProgress(i, num2);
						this.Complete(i, (float)num2 / (float)this.combinedArray[i].aim, false);
					}
				}
				else
				{
					this.playerinfo.SetCurrentTaskProgress(i, this.combinedArray[i].aim);
					if (num < this.combinedArray[i].aim)
					{
						this.Complete(i, 1f, false);
					}
				}
			}
		}
	}

	public void RemoveProgressForThis(TaskTarget myTask)
	{
		if (this.templates == null)
		{
			TasksManager.LogError("currentTemplates == null", null);
		}
		for (int i = 0; i < this.combinedArray.Length; i++)
		{
			if (this.templates[i].taskTarget == myTask)
			{
				this.ResetProgressForTaskIndex(i);
			}
		}
	}

	private void ResetProgressForTaskIndex(int questIndex)
	{
		if (!this.GetTaskInfo(questIndex).complete)
		{
			this.playerinfo.SetCurrentTaskProgress(questIndex, 0);
			if (this._currentRunProgress != null)
			{
				this._currentRunProgress[questIndex] = 0;
			}
		}
	}

	public void SkipTask(int taskNumber)
	{
		this.PlayerDidThis(TaskTarget.SkipTask, 1, -1);
		this.playerinfo.SetCurrentTaskProgress(taskNumber, this.tasks[this.playerinfo.currentTaskSet][taskNumber].aim);
		this.Complete(taskNumber, 1f, false);
	}

	private Task[] combinedArray
	{
		get
		{
			if (this._currentTaskSetLoaded != this.playerinfo.currentTaskSet || this._combinedArray == null)
			{
				if (this._combinedArray == null)
				{
					this._combinedArray = new Task[Achievements.NUMBER_OF_ACHIEVEMENTS + 3];
				}
				for (int i = 0; i < 3; i++)
				{
					this._combinedArray[i] = this.tasks[this.playerinfo.currentTaskSet][i];
				}
				if (this._currentTaskSetLoaded == -1)
				{
					for (int j = 0; j < Achievements.NUMBER_OF_ACHIEVEMENTS; j++)
					{
						this._combinedArray[j + 3] = Achievements.achievementArray[j];
					}
				}
			}
			this._currentTaskSetLoaded = this.currentTaskSet;
			return this._combinedArray;
		}
	}

	public int currentTaskSet
	{
		get
		{
			return this.playerinfo.currentTaskSet;
		}
		set
		{
			if (value != this.playerinfo.currentTaskSet)
			{
				int num = Mathf.Clamp(value, 0, this.taskSetCount);
				this.playerinfo.InitCurrentTaskSet(num, this.tasks[num].Length, true);
			}
		}
	}

	public bool inRun
	{
		get
		{
			return this._currentRunProgress != null;
		}
		set
		{
			if (value)
			{
				this._currentRunProgress = new int[this.combinedArray.Length];
			}
			else if (this._currentRunProgress != null)
			{
				if (UIScreenController.Instance.GetTopScreenName() != "PauseUI")
				{
					for (int i = 0; i < this.combinedArray.Length; i++)
					{
						if (this.templates[i].singleRun && this.templates[i].completeIfLess && this._currentRunProgress[i] < this.combinedArray[i].aim)
						{
							this.playerinfo.SetCurrentTaskProgress(i, this.combinedArray[i].aim);
							this.Complete(i, 1f, false);
						}
					}
				}
				this.PlayerDidThis(TaskTarget.StayInOneLane, (int)(Time.time - Character.Instance.sameLaneTimeStamp), -1);
				this.RemoveProgressForThis(TaskTarget.StayInOneLane);
				this._currentRunProgress = null;
			}
		}
	}

	public static TasksManager Instance
	{
		get
		{
			if (TasksManager._instance == null)
			{
				TasksManager._instance = new TasksManager();
			}
			return TasksManager._instance;
		}
	}

	private Task[][] tasks
	{
		get
		{
			if (this._tasks == null)
			{
				Task[][] repeatableTasks = TasksData.repeatableTasks;
				Task[][] singleuseTasks = TasksData.singleuseTasks;
				this._tasks = new Task[singleuseTasks.Length + repeatableTasks.Length][];
				for (int i = 0; i < singleuseTasks.Length; i++)
				{
					this._tasks[i] = singleuseTasks[i];
				}
				for (int j = 0; j < repeatableTasks.Length; j++)
				{
					this._tasks[singleuseTasks.Length + j] = repeatableTasks[j];
				}
			}
			return this._tasks;
		}
	}

	public int taskSetCount
	{
		get
		{
			return this.tasks.Length;
		}
	}

	public int taskSetStoryCount
	{
		get
		{
			return this.tasks.Length - TasksData.repeatableTasks.Length;
		}
	}

	private TaskTemplate[] templates
	{
		get
		{
			if (this._currentTaskTemplateSetLoaded != this.playerinfo.currentTaskSet || this._templates == null)
			{
				Dictionary<TaskType, TaskTemplate> taskTemplates = TasksData.taskTemplates;
				if (this._templates == null)
				{
					this._templates = new TaskTemplate[Achievements.NUMBER_OF_ACHIEVEMENTS + 3];
				}
				for (int i = 0; i < 3; i++)
				{
					this._templates[i] = taskTemplates[this.tasks[this.playerinfo.currentTaskSet][i].type];
				}
				if (this._currentTaskTemplateSetLoaded == -1)
				{
					for (int j = 0; j < Achievements.NUMBER_OF_ACHIEVEMENTS; j++)
					{
						this._templates[j + 3] = taskTemplates[Achievements.achievementArray[j].type];
					}
				}
				this._currentTaskTemplateSetLoaded = this.playerinfo.currentTaskSet;
			}
			return this._templates;
		}
	}

	public TasksManager.TaskSetCompleteHandler onTaskSetComplete;

	public TasksManager.TaskCompleteHandler onTaskComplete;

	public TasksManager.AchievementCompleteHandler onAchievementComplete;

	private Task[] _combinedArray;

	private int _currentTaskSetLoaded = -1;

	private int _currentTaskTemplateSetLoaded = -1;

	private int[] _currentRunProgress;

	private static TasksManager _instance;

	private Task[][] _tasks;

	private TaskTemplate[] _templates;

	private PlayerInfo playerinfo = PlayerInfo.Instance;

	public delegate void AchievementCompleteHandler(string msg);

	public delegate void TaskCompleteHandler(string msg);

	public delegate void TaskSetCompleteHandler();
}
