using System;
using GameData.Utilities;

namespace FrameWork.CommandSystem
{
	// Token: 0x02001073 RID: 4211
	public class CommandWaitEvent : BaseCommand, ICollectable<ArgumentBox>, ICollectable
	{
		// Token: 0x0600BF2E RID: 48942 RVA: 0x0056721C File Offset: 0x0056541C
		public override bool Execute()
		{
			bool isValid = this._isValid;
			if (isValid)
			{
				GEvent.AddOneShot(this._waitingEvent, new GEvent.Callback(this.OnEvent));
				this._executeTime = DateTime.Now;
			}
			return this._isValid;
		}

		// Token: 0x1700157E RID: 5502
		// (get) Token: 0x0600BF2F RID: 48943 RVA: 0x00567263 File Offset: 0x00565463
		public override bool Done
		{
			get
			{
				return this.GetDone();
			}
		}

		// Token: 0x0600BF30 RID: 48944 RVA: 0x0056726C File Offset: 0x0056546C
		public void Reset(ArgumentBox argBox)
		{
			bool flag = argBox == null;
			if (flag)
			{
				this._isValid = false;
			}
			else
			{
				this._isValid = (argBox.Get("event", out this._waitingEvent) && argBox.Get("timeout", out this._timeoutSeconds) && this._timeoutSeconds > 0f);
				argBox.Get<ArgumentBox>("extraArgs", out this._extraArgs);
				argBox.Get<WaitEventExtraHandler>("extraHandler", out this._extraHandler);
				argBox.Get<WaitEventSimpleHandler>("simpleHandler", out this._simpleHandler);
				argBox.Get<WaitEventCompleteHandler>("completeHandler", out this._completeHandler);
			}
		}

		// Token: 0x0600BF31 RID: 48945 RVA: 0x00567314 File Offset: 0x00565514
		public override void Reset()
		{
			this.Reset(null);
		}

		// Token: 0x0600BF32 RID: 48946 RVA: 0x00567320 File Offset: 0x00565520
		public override void Collect()
		{
			this._isValid = (this._eventTriggered = false);
			this._executeTime = DateTime.MinValue;
			bool flag = this._extraArgs != null;
			if (flag)
			{
				EasyPool.Free<ArgumentBox>(this._extraArgs);
			}
			this._waitingEvent = null;
			this._timeoutSeconds = -1f;
			this._extraArgs = null;
			this._extraHandler = null;
			this._simpleHandler = null;
			this._completeHandler = null;
		}

		// Token: 0x0600BF33 RID: 48947 RVA: 0x00567390 File Offset: 0x00565590
		private void OnEvent(ArgumentBox argBox)
		{
			this._eventTriggered = true;
			WaitEventExtraHandler extraHandler = this._extraHandler;
			if (extraHandler != null)
			{
				extraHandler(argBox, this._extraArgs);
			}
			WaitEventSimpleHandler simpleHandler = this._simpleHandler;
			if (simpleHandler != null)
			{
				simpleHandler(argBox);
			}
			WaitEventCompleteHandler completeHandler = this._completeHandler;
			if (completeHandler != null)
			{
				completeHandler(argBox, this._extraArgs, this._waitingEvent, (float)(DateTime.Now - this._executeTime).TotalSeconds);
			}
		}

		// Token: 0x0600BF34 RID: 48948 RVA: 0x0056740C File Offset: 0x0056560C
		private bool GetDone()
		{
			bool flag = !this._eventTriggered && (DateTime.Now - this._executeTime).TotalSeconds > (double)this._timeoutSeconds;
			if (flag)
			{
				this._eventTriggered = true;
				AdaptableLog.TagWarning("CMD", string.Format("CommandWaitEvent timeout in {0:HH:mm:ss}, event={1}", DateTime.Now, this._waitingEvent), false);
			}
			return this._eventTriggered;
		}

		// Token: 0x0400926E RID: 37486
		private bool _isValid;

		// Token: 0x0400926F RID: 37487
		private Enum _waitingEvent;

		// Token: 0x04009270 RID: 37488
		private float _timeoutSeconds;

		// Token: 0x04009271 RID: 37489
		private ArgumentBox _extraArgs;

		// Token: 0x04009272 RID: 37490
		private WaitEventExtraHandler _extraHandler;

		// Token: 0x04009273 RID: 37491
		private WaitEventSimpleHandler _simpleHandler;

		// Token: 0x04009274 RID: 37492
		private WaitEventCompleteHandler _completeHandler;

		// Token: 0x04009275 RID: 37493
		private DateTime _executeTime;

		// Token: 0x04009276 RID: 37494
		private bool _eventTriggered;
	}
}
