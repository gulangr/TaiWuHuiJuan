using System;
using GameData.Domains.Character.AvatarSystem;

namespace EventEditor
{
	// Token: 0x02000633 RID: 1587
	public class EventEditorRole
	{
		// Token: 0x06004B1A RID: 19226 RVA: 0x00235164 File Offset: 0x00233364
		public string GetName()
		{
			bool flag = !string.IsNullOrEmpty(this.CustomName);
			string result;
			if (flag)
			{
				result = this.CustomName;
			}
			else
			{
				result = this.Key;
			}
			return result;
		}

		// Token: 0x06004B1B RID: 19227 RVA: 0x00235198 File Offset: 0x00233398
		public void SendDirty()
		{
			bool dirty = this.Dirty;
			if (dirty)
			{
				Action onDataChange = this.OnDataChange;
				if (onDataChange != null)
				{
					onDataChange();
				}
			}
			this.Dirty = false;
		}

		// Token: 0x04003420 RID: 13344
		public string Key;

		// Token: 0x04003421 RID: 13345
		public string CustomName;

		// Token: 0x04003422 RID: 13346
		public AvatarData AvatarData;

		// Token: 0x04003423 RID: 13347
		public int Behavior;

		// Token: 0x04003424 RID: 13348
		public int Identity;

		// Token: 0x04003425 RID: 13349
		public byte Age;

		// Token: 0x04003426 RID: 13350
		public bool Dirty;

		// Token: 0x04003427 RID: 13351
		public Action OnDataChange;
	}
}
