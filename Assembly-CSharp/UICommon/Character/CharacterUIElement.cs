using System;
using CharacterDataMonitor;

namespace UICommon.Character
{
	// Token: 0x020005EC RID: 1516
	public abstract class CharacterUIElement
	{
		// Token: 0x17000904 RID: 2308
		// (get) Token: 0x06004789 RID: 18313 RVA: 0x00218444 File Offset: 0x00216644
		// (set) Token: 0x0600478A RID: 18314 RVA: 0x00218474 File Offset: 0x00216674
		public int CharacterId
		{
			get
			{
				bool flag = this.MonitorDataItem == null;
				int result;
				if (flag)
				{
					result = -1;
				}
				else
				{
					result = this.MonitorDataItem.CharacterId;
				}
				return result;
			}
			set
			{
				bool flag = value < 0;
				if (flag)
				{
					bool flag2 = this.MonitorDataItem != null;
					if (flag2)
					{
						this.UnbindEvent();
					}
					this.ResetToEmpty();
					this.MonitorDataItem = null;
				}
				else
				{
					bool flag3 = this.MonitorDataItem != null && this.MonitorDataItem.CharacterId == value;
					if (flag3)
					{
						this.Refresh();
					}
					else
					{
						bool flag4 = this.MonitorDataItem != null;
						if (flag4)
						{
							this.UnbindEvent();
						}
						this.MonitorDataItem = this.GetMonitorItem(value);
						this.BindEvent();
						bool init = this.MonitorDataItem.Init;
						if (init)
						{
							this.MonitorDataItem.OnDataInit();
						}
					}
				}
			}
		}

		// Token: 0x17000905 RID: 2309
		// (get) Token: 0x0600478B RID: 18315 RVA: 0x0021851D File Offset: 0x0021671D
		protected bool IsTaiwu
		{
			get
			{
				return this.MonitorDataItem.IsTaiwu;
			}
		}

		// Token: 0x0600478C RID: 18316 RVA: 0x0021852C File Offset: 0x0021672C
		public void SetIsDead(bool value)
		{
			this.IsDead = value;
			bool flag = this.MonitorDataItem != null;
			if (flag)
			{
				this.MonitorDataItem.Character.SetDeadState(value);
			}
		}

		// Token: 0x0600478D RID: 18317 RVA: 0x00218560 File Offset: 0x00216760
		public void Refresh()
		{
			MonitorDataItemBase monitorDataItem = this.MonitorDataItem;
			if (monitorDataItem != null)
			{
				monitorDataItem.Refresh();
			}
		}

		// Token: 0x0600478E RID: 18318
		public abstract MonitorDataItemBase GetMonitorItem(int charId);

		// Token: 0x0600478F RID: 18319
		internal abstract void BindEvent();

		// Token: 0x06004790 RID: 18320
		public abstract void UnbindEvent();

		// Token: 0x06004791 RID: 18321
		public abstract void FillElement();

		// Token: 0x06004792 RID: 18322
		public abstract void ResetToEmpty();

		// Token: 0x06004793 RID: 18323 RVA: 0x00218578 File Offset: 0x00216778
		public T GetMonitor<T>() where T : MonitorDataItemBase
		{
			return this.MonitorDataItem as T;
		}

		// Token: 0x04003151 RID: 12625
		protected MonitorDataItemBase MonitorDataItem;

		// Token: 0x04003152 RID: 12626
		protected bool IsDead;
	}
}
