using System;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.LegacyPassing
{
	// Token: 0x02000995 RID: 2453
	public class ViewDisplayConfigLegacy : UIBase
	{
		// Token: 0x06007628 RID: 30248 RVA: 0x00371256 File Offset: 0x0036F456
		private void Awake()
		{
			this.quickHide.onClick.ResetListener(new Action(this.QuickHide));
		}

		// Token: 0x06007629 RID: 30249 RVA: 0x00371278 File Offset: 0x0036F478
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get("GroupId", out this._templateId);
			argsBox.Get("Level", out this._level);
			this.title.text = WorldCreationGroup.Instance[this._templateId].Name + LanguageKey.LK_Dot_Symbol.Tr() + LocalStringManager.Get(string.Format("LK_WorldCreation_GroupLevel_{0}", this._level));
			this.legacyContainer.RefreshItems(from legacyItem in Legacy.Instance
			where (int)legacyItem.Level == this._level && legacyItem.WorldCreationGroup == this._templateId && legacyItem.Weight > 0
			select legacyItem into x
			select x.TemplateId, false);
		}

		// Token: 0x040058F8 RID: 22776
		[SerializeField]
		private TMP_Text title;

		// Token: 0x040058F9 RID: 22777
		[SerializeField]
		private LegacyContainer legacyContainer;

		// Token: 0x040058FA RID: 22778
		[SerializeField]
		private CButton quickHide;

		// Token: 0x040058FB RID: 22779
		private sbyte _templateId;

		// Token: 0x040058FC RID: 22780
		private int _level;
	}
}
