using System.Collections.Generic;
using System.Threading;

namespace Lotus.Threading
{
	public class ManagedThread
	{
		private Thread thread;

		private int processorAffinity;

		private bool killThread;

		private Queue<ThreadTask> tasks = new Queue<ThreadTask>();

		public ManagedThread(int processorAffinity)
		{
			this.processorAffinity = processorAffinity;
			ThreadStart start = taskRunner;
			this.thread = new Thread(start);
			this.thread.IsBackground = true;
			this.thread.Start();
		}

		public ManagedThread()
			: this(-1)
		{
		}

		private void taskRunner()
		{
			while (!this.killThread)
			{
				if (this.tasks.Count > 0)
				{
					ThreadTask threadTask;
					lock (this.tasks)
					{
						threadTask = this.tasks.Dequeue();
					}
					threadTask.Task();
					if (threadTask.TaskFinished != null)
					{
						threadTask.TaskFinished(threadTask.TaskId);
					}
				}
				else
				{
					Thread.Sleep(10);
				}
			}
			this.tasks.Clear();
			this.tasks = null;
		}

		public void Kill()
		{
			this.killThread = true;
		}

		public void KillImmediately()
		{
			this.killThread = true;
			this.thread.Abort();
		}

		public void AddTask(ThreadTask task)
		{
			lock (this.tasks)
			{
				this.tasks.Enqueue(task);
			}
		}
	}
}
