using System;
using System.Collections.Generic;
using GameData.GameDataBridge;

namespace FrameWork.CommandSystem
{
	// Token: 0x02001061 RID: 4193
	public class CommandCallMethod : BaseCommand, ICollectable<ArgumentBox>, ICollectable
	{
		// Token: 0x0600BEE3 RID: 48867 RVA: 0x00566A7C File Offset: 0x00564C7C
		public override bool Execute()
		{
			bool isValid = this._isValid;
			if (isValid)
			{
				this.DoCallMethod();
			}
			return this._isValid;
		}

		// Token: 0x1700157C RID: 5500
		// (get) Token: 0x0600BEE4 RID: 48868 RVA: 0x00566AA5 File Offset: 0x00564CA5
		public override bool Done
		{
			get
			{
				return this._responded;
			}
		}

		// Token: 0x0600BEE5 RID: 48869 RVA: 0x00566AB0 File Offset: 0x00564CB0
		public void Reset(ArgumentBox arg)
		{
			bool flag = arg == null;
			if (flag)
			{
				this._isValid = false;
			}
			else
			{
				this._isValid = (arg.Get("domain", out this.DomainId) && arg.Get("method", out this.MethodId));
				arg.Get<CallMethodSkipHandler>("onSkip", out this._skipHandler);
				arg.Get<CallMethodRespHandler>("onResp", out this._respHandler);
				this.AnalysisArgument(arg);
			}
			bool flag2 = !this._isValid;
			if (!flag2)
			{
				this.ListenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyData));
			}
		}

		// Token: 0x0600BEE6 RID: 48870 RVA: 0x00566B52 File Offset: 0x00564D52
		public override void Reset()
		{
			this.Reset(null);
		}

		// Token: 0x0600BEE7 RID: 48871 RVA: 0x00566B60 File Offset: 0x00564D60
		public override void Collect()
		{
			bool isValid = this._isValid;
			if (isValid)
			{
				GameDataBridge.UnregisterListener(this.ListenerId);
			}
			bool flag = !this._responded;
			if (flag)
			{
				CallMethodSkipHandler skipHandler = this._skipHandler;
				if (skipHandler != null)
				{
					skipHandler();
				}
			}
			this._isValid = (this._responded = false);
			this._skipHandler = null;
			this._respHandler = null;
			this.ReleaseArguments();
		}

		// Token: 0x0600BEE8 RID: 48872 RVA: 0x00566BC8 File Offset: 0x00564DC8
		private void OnNotifyData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				bool flag = notification.Type == 1 && notification.DomainId == this.DomainId && notification.MethodId == this.MethodId;
				if (flag)
				{
					this._responded = true;
					CallMethodRespHandler respHandler = this._respHandler;
					if (respHandler != null)
					{
						respHandler(notification.ValueOffset, wrapper.DataPool);
					}
				}
			}
		}

		// Token: 0x0600BEE9 RID: 48873 RVA: 0x00566C70 File Offset: 0x00564E70
		protected virtual void AnalysisArgument(ArgumentBox argBox)
		{
		}

		// Token: 0x0600BEEA RID: 48874 RVA: 0x00566C73 File Offset: 0x00564E73
		protected virtual void ReleaseArguments()
		{
		}

		// Token: 0x0600BEEB RID: 48875 RVA: 0x00566C76 File Offset: 0x00564E76
		protected virtual void DoCallMethod()
		{
			GameDataBridge.AddMethodCall(this.ListenerId, this.DomainId, this.MethodId);
		}

		// Token: 0x0400925C RID: 37468
		private bool _responded;

		// Token: 0x0400925D RID: 37469
		private bool _isValid;

		// Token: 0x0400925E RID: 37470
		private CallMethodSkipHandler _skipHandler;

		// Token: 0x0400925F RID: 37471
		private CallMethodRespHandler _respHandler;

		// Token: 0x04009260 RID: 37472
		protected int ListenerId;

		// Token: 0x04009261 RID: 37473
		protected ushort DomainId;

		// Token: 0x04009262 RID: 37474
		protected ushort MethodId;
	}
}
