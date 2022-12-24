namespace Lotus.Threading
{
	public struct ThreadTask
	{
		private ThreadTaskDelegate task;

		private TaskFinishedDelegate taskFinished;

		private int taskId;

		public ThreadTaskDelegate Task => this.task;

		public TaskFinishedDelegate TaskFinished => this.taskFinished;

		public int TaskId => this.taskId;

		public ThreadTask(ThreadTaskDelegate task, TaskFinishedDelegate taskFinished, int taskId)
		{
			this.task = task;
			this.taskFinished = taskFinished;
			this.taskId = taskId;
		}

		public ThreadTask(ThreadTaskDelegate task, TaskFinishedDelegate taskFinished)
			: this(task, taskFinished, -1)
		{
		}

		public ThreadTask(ThreadTaskDelegate task)
			: this(task, null, -1)
		{
		}
	}
}
