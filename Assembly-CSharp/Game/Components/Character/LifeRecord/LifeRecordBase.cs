using System;
using GameData.Domains.LifeRecord;
using UnityEngine;

namespace Game.Components.Character.LifeRecord
{
	// Token: 0x02000F4A RID: 3914
	public class LifeRecordBase : RecordBase
	{
		// Token: 0x17001458 RID: 5208
		// (get) Token: 0x0600B398 RID: 45976 RVA: 0x0051BF75 File Offset: 0x0051A175
		public override string TitleKey
		{
			get
			{
				return LifeRecord.LifeRecordTitleKey;
			}
		}

		// Token: 0x17001459 RID: 5209
		// (get) Token: 0x0600B399 RID: 45977 RVA: 0x0051BF7C File Offset: 0x0051A17C
		public override string ContentKey
		{
			get
			{
				return LifeRecord.LifeRecordContentKey;
			}
		}

		// Token: 0x1700145A RID: 5210
		// (get) Token: 0x0600B39A RID: 45978 RVA: 0x0051BF83 File Offset: 0x0051A183
		public override string FootKey
		{
			get
			{
				return LifeRecord.LifeRecordFootKey;
			}
		}

		// Token: 0x1700145B RID: 5211
		// (get) Token: 0x0600B39B RID: 45979 RVA: 0x0051BF8A File Offset: 0x0051A18A
		// (set) Token: 0x0600B39C RID: 45980 RVA: 0x0051BF94 File Offset: 0x0051A194
		public override RecordType Type
		{
			get
			{
				return base.Type;
			}
			set
			{
				bool flag = this.type == value;
				if (!flag)
				{
					switch (this.type)
					{
					case RecordType.Title:
					{
						bool flag2 = this.title;
						if (flag2)
						{
							PoolManager.Destroy(this.TitleKey, this.title.gameObject);
						}
						else
						{
							Debug.LogWarning(string.Format("Title is null while type == {0}.", this.type));
						}
						this.title = null;
						break;
					}
					case RecordType.Content:
					{
						bool flag3 = this.content;
						if (flag3)
						{
							PoolManager.Destroy(this.ContentKey, this.content.gameObject);
						}
						else
						{
							Debug.LogWarning(string.Format("Content is null while type == {0}.", this.type));
						}
						this.content = null;
						break;
					}
					case RecordType.Foot:
					{
						bool flag4 = this.foot;
						if (flag4)
						{
							PoolManager.Destroy(this.FootKey, this.foot.gameObject);
						}
						else
						{
							Debug.LogWarning(string.Format("Foot is null while type == {0}.", this.type));
						}
						this.foot = null;
						break;
					}
					}
					this.type = value;
					switch (value)
					{
					case RecordType.Title:
					{
						this.title = PoolManager.GetObject<LifeRecordTitle>(this.TitleKey);
						bool flag5 = this.title == null || !this.title;
						if (flag5)
						{
							Debug.LogError("failed to get title");
							this.type = RecordType.Invalid;
						}
						else
						{
							this.title.transform.SetParent(base.transform, false);
						}
						break;
					}
					case RecordType.Content:
					{
						this.content = PoolManager.GetObject<LifeRecordContent>(this.ContentKey);
						bool flag6 = this.content == null || !this.content;
						if (flag6)
						{
							Debug.LogError("failed to get content");
							this.type = RecordType.Invalid;
						}
						else
						{
							this.content.transform.SetParent(base.transform, false);
						}
						break;
					}
					case RecordType.Foot:
					{
						this.foot = PoolManager.GetObject<RecordFoot>(this.FootKey);
						bool flag7 = this.foot == null || !this.foot;
						if (flag7)
						{
							Debug.LogError("failed to get foot");
							this.type = RecordType.Invalid;
						}
						else
						{
							this.foot.transform.SetParent(base.transform, false);
						}
						break;
					}
					}
				}
			}
		}

		// Token: 0x0600B39D RID: 45981 RVA: 0x0051C20C File Offset: 0x0051A40C
		public void Set(RenderedRecordData renderedData, TransferableRecordDataBase fullData)
		{
			short? num = (renderedData != null) ? new short?(renderedData.TemplateId) : null;
			short? num2 = num;
			if (num2 != null)
			{
				short valueOrDefault = num2.GetValueOrDefault();
				if (valueOrDefault != -3)
				{
					if (valueOrDefault != -2)
					{
						this.Type = RecordType.Content;
						(this.content as LifeRecordContent).Set(renderedData, fullData, renderedData.Main, renderedData.Sub);
						RectTransform tf = base.GetComponent<RectTransform>();
						tf.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 36f);
						return;
					}
					this.Type = RecordType.Title;
					this.title.Set(renderedData.Main);
					RectTransform tf2 = base.GetComponent<RectTransform>();
					tf2.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 36f);
					return;
				}
			}
			this.Type = RecordType.Foot;
			this.foot.Set();
			RectTransform tf3 = base.GetComponent<RectTransform>();
			tf3.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 16f);
		}
	}
}
