using System;
using CharacterDataMonitor;
using UnityEngine;

namespace Game.Views.Buildings.Migrate
{
	// Token: 0x02000BCB RID: 3019
	[Obsolete]
	public abstract class BuildingOverviewCharacterInfoBase : MonoBehaviour
	{
		// Token: 0x1700104E RID: 4174
		// (get) Token: 0x06009836 RID: 38966 RVA: 0x0046F31E File Offset: 0x0046D51E
		protected bool IsTaiwu
		{
			get
			{
				return this.MonitorDataItem.IsTaiwu;
			}
		}

		// Token: 0x1700104F RID: 4175
		// (get) Token: 0x06009837 RID: 38967 RVA: 0x0046F32C File Offset: 0x0046D52C
		// (set) Token: 0x06009838 RID: 38968 RVA: 0x0046F35C File Offset: 0x0046D55C
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

		// Token: 0x06009839 RID: 38969 RVA: 0x0046F405 File Offset: 0x0046D605
		public void Refresh()
		{
			MonitorDataItemBase monitorDataItem = this.MonitorDataItem;
			if (monitorDataItem != null)
			{
				monitorDataItem.Refresh();
			}
		}

		// Token: 0x0600983A RID: 38970
		public abstract MonitorDataItemBase GetMonitorItem(int charId);

		// Token: 0x0600983B RID: 38971
		internal abstract void BindEvent();

		// Token: 0x0600983C RID: 38972
		public abstract void UnbindEvent();

		// Token: 0x0600983D RID: 38973
		public abstract void ResetToEmpty();

		// Token: 0x040074FC RID: 29948
		protected MonitorDataItemBase MonitorDataItem;
	}
}
