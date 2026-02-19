using System;

public struct TaskInfo
{
	public TaskInfo(Task task, TaskTemplate template, int progress, bool complete)
	{
		this.task = task;
		this.template = template;
		this.progress = progress;
		this.complete = complete;
	}

	public Task task;

	public TaskTemplate template;

	public int progress;

	public bool complete;
}
