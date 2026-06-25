using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
	// Token: 0x02000FDB RID: 4059
	public class AsyncJobSystem : MonoBehaviour
	{
		// Token: 0x0600B991 RID: 47505 RVA: 0x00549864 File Offset: 0x00547A64
		public void Update()
		{
			int i = 0;
			while (i < this._mRunningJobs.Count)
			{
				AsyncJobSystem.Job job = this._mRunningJobs[i];
				bool done = job.Done;
				if (done)
				{
					int last = this._mRunningJobs.Count - 1;
					bool flag = i != last;
					if (flag)
					{
						this._mRunningJobs[i] = this._mRunningJobs[last];
					}
					this._mRunningJobs.RemoveAt(last);
					job.Finish();
				}
				else
				{
					i++;
				}
			}
			int maxCount = Mathf.Max(1, this.maxWorkingJobCount);
			while (this._mRunningJobs.Count < maxCount)
			{
				bool flag2 = this._mJobs.Count > 0;
				if (!flag2)
				{
					break;
				}
				AsyncJobSystem.IJob job2 = this._mJobs[0];
				this._mJobs.RemoveAt(0);
				AsyncJobSystem.Job obj = job2 as AsyncJobSystem.Job;
				bool flag3 = obj != null;
				if (!flag3)
				{
					obj = new AsyncJobSystem.Job
					{
						job = job2
					};
				}
				this._mRunningJobs.Add(obj);
				obj.Start(this);
			}
		}

		// Token: 0x0600B992 RID: 47506 RVA: 0x0054999C File Offset: 0x00547B9C
		private void OnDestroy()
		{
			foreach (AsyncJobSystem.Job job in this._mRunningJobs)
			{
				job.job.Drop();
			}
			this._mRunningJobs.Clear();
			foreach (AsyncJobSystem.IJob job2 in this._mJobs)
			{
				job2.Drop();
			}
			this._mJobs.Clear();
		}

		// Token: 0x0600B993 RID: 47507 RVA: 0x00549A58 File Offset: 0x00547C58
		public void add_job(AsyncJobSystem.IJob job)
		{
			this._mJobs.Add(job);
		}

		// Token: 0x0600B994 RID: 47508 RVA: 0x00549A68 File Offset: 0x00547C68
		public void add_job(Action action)
		{
			bool flag = action != null;
			if (flag)
			{
				this._mJobs.Add(new AsyncJobSystem.ActionJob(action));
			}
		}

		// Token: 0x0600B995 RID: 47509 RVA: 0x00549A94 File Offset: 0x00547C94
		public bool cancel_not_running_job(AsyncJobSystem.IJob job)
		{
			bool flag = this._mJobs.Remove(job);
			bool result;
			if (flag)
			{
				job.Drop();
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600B996 RID: 47510 RVA: 0x00549AC4 File Offset: 0x00547CC4
		public AsyncJobSystem.IJob find_not_running_job(Predicate<AsyncJobSystem.IJob> match)
		{
			return this._mJobs.Find(match);
		}

		// Token: 0x04008FA0 RID: 36768
		public int maxWorkingJobCount = 1;

		// Token: 0x04008FA1 RID: 36769
		private List<AsyncJobSystem.IJob> _mJobs = new List<AsyncJobSystem.IJob>();

		// Token: 0x04008FA2 RID: 36770
		private List<AsyncJobSystem.Job> _mRunningJobs = new List<AsyncJobSystem.Job>();

		// Token: 0x0200262B RID: 9771
		public interface IJob
		{
			// Token: 0x06011B2D RID: 72493
			IEnumerator Start();

			// Token: 0x06011B2E RID: 72494
			void Drop();
		}

		// Token: 0x0200262C RID: 9772
		private class Job : IEnumerator
		{
			// Token: 0x17001BA6 RID: 7078
			// (get) Token: 0x06011B2F RID: 72495 RVA: 0x00686EC6 File Offset: 0x006850C6
			public bool Done
			{
				get
				{
					return this._state == 2;
				}
			}

			// Token: 0x17001BA7 RID: 7079
			// (get) Token: 0x06011B30 RID: 72496 RVA: 0x00686ED1 File Offset: 0x006850D1
			object IEnumerator.Current
			{
				get
				{
					return this.job.Start();
				}
			}

			// Token: 0x06011B31 RID: 72497 RVA: 0x00686EDE File Offset: 0x006850DE
			public void Start(MonoBehaviour mb)
			{
				this._coroutine = mb.StartCoroutine(this);
			}

			// Token: 0x06011B32 RID: 72498 RVA: 0x00686EF0 File Offset: 0x006850F0
			public void Cancel(MonoBehaviour mb)
			{
				bool flag = this._coroutine != null;
				if (flag)
				{
					mb.StopCoroutine(this._coroutine);
					this._coroutine = null;
				}
				this.job.Drop();
				this.job = null;
				this._state = 0;
			}

			// Token: 0x06011B33 RID: 72499 RVA: 0x00686F3B File Offset: 0x0068513B
			public void Finish()
			{
				this.job.Drop();
				this.job = null;
				this._state = 0;
			}

			// Token: 0x06011B34 RID: 72500 RVA: 0x00686F58 File Offset: 0x00685158
			bool IEnumerator.MoveNext()
			{
				int state = this._state;
				int num = state;
				bool result;
				if (num != 0)
				{
					if (num == 1)
					{
						this._coroutine = null;
						this._state = 2;
					}
					result = false;
				}
				else
				{
					this._state = 1;
					result = true;
				}
				return result;
			}

			// Token: 0x06011B35 RID: 72501 RVA: 0x00686F9A File Offset: 0x0068519A
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x0400E9DB RID: 59867
			public AsyncJobSystem.IJob job;

			// Token: 0x0400E9DC RID: 59868
			private Coroutine _coroutine;

			// Token: 0x0400E9DD RID: 59869
			private int _state;
		}

		// Token: 0x0200262D RID: 9773
		private class ActionJob : AsyncJobSystem.Job, AsyncJobSystem.IJob
		{
			// Token: 0x06011B37 RID: 72503 RVA: 0x00686FAB File Offset: 0x006851AB
			public ActionJob(Action action)
			{
				this.job = this;
				this._mAction = action;
			}

			// Token: 0x06011B38 RID: 72504 RVA: 0x00686FC3 File Offset: 0x006851C3
			public void Drop()
			{
			}

			// Token: 0x06011B39 RID: 72505 RVA: 0x00686FC6 File Offset: 0x006851C6
			public IEnumerator Start()
			{
				this._mAction();
				yield break;
			}

			// Token: 0x0400E9DE RID: 59870
			private Action _mAction;
		}
	}
}
