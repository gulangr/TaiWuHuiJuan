using System;
using GameData.Domains.LifeRecord;
using UnityEngine;

namespace Game.Components.Character.LifeRecord
{
	// Token: 0x02000F5B RID: 3931
	public abstract class RecordBase : MonoBehaviour
	{
		// Token: 0x1700146E RID: 5230
		// (get) Token: 0x0600B3FE RID: 46078
		public abstract string TitleKey { get; }

		// Token: 0x1700146F RID: 5231
		// (get) Token: 0x0600B3FF RID: 46079
		public abstract string ContentKey { get; }

		// Token: 0x17001470 RID: 5232
		// (get) Token: 0x0600B400 RID: 46080
		public abstract string FootKey { get; }

		// Token: 0x17001471 RID: 5233
		// (get) Token: 0x0600B401 RID: 46081 RVA: 0x0051E44B File Offset: 0x0051C64B
		// (set) Token: 0x0600B402 RID: 46082 RVA: 0x0051E454 File Offset: 0x0051C654
		public virtual RecordType Type
		{
			get
			{
				return this.type;
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
						this.content = null;
						break;
					}
					}
					this.type = value;
					switch (value)
					{
					case RecordType.Title:
					{
						this.title = PoolManager.GetObject<RecordTitle>(this.TitleKey);
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
						this.content = PoolManager.GetObject<RecordContent>(this.ContentKey);
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

		// Token: 0x0600B403 RID: 46083 RVA: 0x0051E6CC File Offset: 0x0051C8CC
		public virtual void Set(RenderedRecordDataBase renderedData, TransferableRecordDataBase fullData)
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
						this.content.Set(fullData, renderedData.Main, renderedData.Sub);
						return;
					}
					this.Type = RecordType.Title;
					this.title.Set(renderedData.Main);
					return;
				}
			}
			this.Type = RecordType.Foot;
			this.foot.Set();
		}

		// Token: 0x0600B404 RID: 46084 RVA: 0x0051E76A File Offset: 0x0051C96A
		public void Reset()
		{
			this.Type = RecordType.Invalid;
		}

		// Token: 0x04008C00 RID: 35840
		[SerializeField]
		protected RecordTitle title;

		// Token: 0x04008C01 RID: 35841
		[SerializeField]
		protected RecordContent content;

		// Token: 0x04008C02 RID: 35842
		[SerializeField]
		protected RecordFoot foot;

		// Token: 0x04008C03 RID: 35843
		[SerializeField]
		protected RecordType type = RecordType.Invalid;
	}
}
