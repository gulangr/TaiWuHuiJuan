using System;
using GameData.Domains.Item;

namespace EventEditor
{
	// Token: 0x02000636 RID: 1590
	public class EventEditorItem
	{
		// Token: 0x1700095A RID: 2394
		// (get) Token: 0x06004B2D RID: 19245 RVA: 0x002355BF File Offset: 0x002337BF
		public string Name
		{
			get
			{
				return ItemTemplateHelper.GetName(this.Type, this.TemplateId);
			}
		}

		// Token: 0x1700095B RID: 2395
		// (get) Token: 0x06004B2E RID: 19246 RVA: 0x002355D2 File Offset: 0x002337D2
		public string Desc
		{
			get
			{
				return ItemTemplateHelper.GetDesc(this.Type, this.TemplateId);
			}
		}

		// Token: 0x1700095C RID: 2396
		// (get) Token: 0x06004B2F RID: 19247 RVA: 0x002355E5 File Offset: 0x002337E5
		public sbyte Grade
		{
			get
			{
				return ItemTemplateHelper.GetGrade(this.Type, this.TemplateId);
			}
		}

		// Token: 0x1700095D RID: 2397
		// (get) Token: 0x06004B30 RID: 19248 RVA: 0x002355F8 File Offset: 0x002337F8
		public string ColorName
		{
			get
			{
				string colorString = Colors.Instance.GradeColors[(int)(this.Grade - 1)].ColorToHexString("#");
				return string.Concat(new string[]
				{
					"<color=",
					colorString,
					">",
					this.Name,
					"</color>"
				});
			}
		}

		// Token: 0x1700095E RID: 2398
		// (get) Token: 0x06004B31 RID: 19249 RVA: 0x0023565C File Offset: 0x0023385C
		public string Icon
		{
			get
			{
				return ItemTemplateHelper.GetIcon(this.Type, this.TemplateId);
			}
		}

		// Token: 0x1700095F RID: 2399
		// (get) Token: 0x06004B32 RID: 19250 RVA: 0x0023566F File Offset: 0x0023386F
		public string IconBack
		{
			get
			{
				return ItemUtils.GetItemIconBack(this.Type, this.TemplateId);
			}
		}

		// Token: 0x06004B33 RID: 19251 RVA: 0x00235684 File Offset: 0x00233884
		public void SendDirty()
		{
			bool dirty = this.Dirty;
			if (dirty)
			{
				Action<EventEditorItem> onDirty = this.OnDirty;
				if (onDirty != null)
				{
					onDirty(this);
				}
			}
			this.Dirty = false;
		}

		// Token: 0x04003435 RID: 13365
		public string Key;

		// Token: 0x04003436 RID: 13366
		public sbyte Type;

		// Token: 0x04003437 RID: 13367
		public short TemplateId;

		// Token: 0x04003438 RID: 13368
		public bool Dirty;

		// Token: 0x04003439 RID: 13369
		public Action<EventEditorItem> OnDirty;
	}
}
