using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using Game.Components.Character;
using Game.Components.ListStyleGeneralScroll.CellContent;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000871 RID: 2161
	public class MouseTipMonthNotify : MouseTipBase
	{
		// Token: 0x17000C7A RID: 3194
		// (get) Token: 0x06006827 RID: 26663 RVA: 0x002F9D15 File Offset: 0x002F7F15
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06006828 RID: 26664 RVA: 0x002F9D18 File Offset: 0x002F7F18
		protected override void Init(ArgumentBox argsBox)
		{
			argsBox.Get("TemplateId", out this._templateId);
			argsBox.Get("Content", out this._content);
			bool flag = !argsBox.Get<List<AvatarWithNameCellData>>("CharacterList", out this._dataList);
			if (flag)
			{
				this._dataList = null;
			}
			this.Refresh();
		}

		// Token: 0x06006829 RID: 26665 RVA: 0x002F9D70 File Offset: 0x002F7F70
		public override void Refresh()
		{
			MonthlyNotificationItem config = MonthlyNotification.Instance[this._templateId];
			this.title.text = config.Name;
			this.desc.text = this._content.ColorReplace();
			this.desc.GetComponent<TMPTextSpriteHelper>().Parse();
			List<AvatarWithNameCellData> dataList = this._dataList;
			bool flag = dataList != null && dataList.Count > 0;
			if (flag)
			{
				Transform templateTransform = this.template.transform;
				bool isTemplateChild = templateTransform.parent == this.characterHolder;
				bool flag2 = isTemplateChild;
				if (flag2)
				{
					templateTransform.SetParent(null, false);
				}
				for (int i = this.characterHolder.childCount - 1; i >= 0; i--)
				{
					Object.Destroy(this.characterHolder.GetChild(i).gameObject);
				}
				bool flag3 = isTemplateChild;
				if (flag3)
				{
					templateTransform.SetParent(this.characterHolder, false);
				}
				this.template.gameObject.SetActive(false);
				foreach (AvatarWithNameCellData data in this._dataList)
				{
					AvatarWithNameSimple obj = Object.Instantiate<AvatarWithNameSimple>(this.template, this.characterHolder);
					obj.GetComponent<AvatarWithNameSimple>().Set(data.AsGrave ? null : data.AvatarRelatedData, data.TemplateId, data.DisplayName);
					obj.gameObject.SetActive(true);
				}
				this.subtitle.SetActive(true);
				this.characterHolder.gameObject.SetActive(true);
			}
			else
			{
				this.subtitle.SetActive(false);
				this.characterHolder.gameObject.SetActive(false);
			}
			this.groupText.text = MonthlyNotificationSortingGroup.Instance[config.SortingGroup].Name;
		}

		// Token: 0x040049B2 RID: 18866
		[SerializeField]
		private TextMeshProUGUI title;

		// Token: 0x040049B3 RID: 18867
		[SerializeField]
		private TextMeshProUGUI desc;

		// Token: 0x040049B4 RID: 18868
		[SerializeField]
		private GameObject subtitle;

		// Token: 0x040049B5 RID: 18869
		[SerializeField]
		private Transform characterHolder;

		// Token: 0x040049B6 RID: 18870
		[SerializeField]
		private TextMeshProUGUI groupText;

		// Token: 0x040049B7 RID: 18871
		[SerializeField]
		private AvatarWithNameSimple template;

		// Token: 0x040049B8 RID: 18872
		private short _templateId;

		// Token: 0x040049B9 RID: 18873
		private string _content;

		// Token: 0x040049BA RID: 18874
		private List<AvatarWithNameCellData> _dataList;
	}
}
