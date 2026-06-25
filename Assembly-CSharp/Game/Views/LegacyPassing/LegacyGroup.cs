using System;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.LegacyPassing
{
	// Token: 0x02000991 RID: 2449
	[RequireComponent(typeof(CToggle))]
	public class LegacyGroup : MonoBehaviour
	{
		// Token: 0x06007613 RID: 30227 RVA: 0x00370E7D File Offset: 0x0036F07D
		private void Awake()
		{
			this.legacyDisplayer.onClick.ResetListener(new Action(this.CheckGroupLegacies));
		}

		// Token: 0x06007614 RID: 30228 RVA: 0x00370EA0 File Offset: 0x0036F0A0
		public void Set(int lv)
		{
			this.level = lv;
			this.title.text = (WorldCreationGroup.Instance[this.templateId].Name + LanguageKey.LK_Dot_Symbol.Tr() + LocalStringManager.Get(string.Format("LK_WorldCreation_GroupLevel_{0}", this.level))).SetColor(WorldDetailSettingGroup.GetLevelColor(this.level));
		}

		// Token: 0x06007615 RID: 30229 RVA: 0x00370F10 File Offset: 0x0036F110
		private void CheckGroupLegacies()
		{
			UIElement.DisplayConfigLegacy.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("GroupId", this.templateId).Set("Level", this.level));
			UIManager.Instance.MaskUI(UIElement.DisplayConfigLegacy);
		}

		// Token: 0x040058E1 RID: 22753
		[SerializeField]
		private sbyte templateId;

		// Token: 0x040058E2 RID: 22754
		[SerializeField]
		private int level;

		// Token: 0x040058E3 RID: 22755
		[SerializeField]
		private TMP_Text title;

		// Token: 0x040058E4 RID: 22756
		[SerializeField]
		private CButton legacyDisplayer;
	}
}
