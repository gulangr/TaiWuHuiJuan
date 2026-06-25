using System;
using TMPro;
using UnityEngine;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000C10 RID: 3088
	public class TeaHouseCaravanAwarenessLevelItems : MonoBehaviour
	{
		// Token: 0x06009CC2 RID: 40130 RVA: 0x00496AAC File Offset: 0x00494CAC
		public void RefreshLevelItem(int myLevel, int level)
		{
			bool unlocked = level >= myLevel;
			this.point.sprite = (unlocked ? this.unlockedPointSp : this.lockedPointSp);
			this.icon.SetSprite(string.Format("ui_buildingpopup_teahorse_placeicon_{0}_{1}", TeaHouseCaravanAwarenessLevelItems.ItemIcons[myLevel - 1], unlocked ? 1 : 0), false, null);
			this.currentMark.SetActive(level == myLevel);
			this.effect.SetActive(level == myLevel);
			this.point.gameObject.SetActive(level != myLevel);
			this.awarenessTitle.text = LanguageKey.LK_Building_TeaHorse_Awareness_Title.Tr();
			this.awarenessLabel.text = GlobalConfig.Instance.TeaHorseCaravanLevelToAwareness[myLevel - 1].ToString();
		}

		// Token: 0x040079A2 RID: 31138
		private static readonly sbyte[] ItemIcons = new sbyte[]
		{
			1,
			0,
			0,
			1,
			2,
			0,
			0,
			0,
			1,
			2,
			2,
			2,
			1,
			0,
			1,
			0,
			1,
			2,
			0,
			1
		};

		// Token: 0x040079A3 RID: 31139
		[SerializeField]
		private CImage point;

		// Token: 0x040079A4 RID: 31140
		[SerializeField]
		private TMP_Text awarenessLabel;

		// Token: 0x040079A5 RID: 31141
		[SerializeField]
		private TMP_Text awarenessTitle;

		// Token: 0x040079A6 RID: 31142
		[SerializeField]
		private CImage icon;

		// Token: 0x040079A7 RID: 31143
		[SerializeField]
		private GameObject effect;

		// Token: 0x040079A8 RID: 31144
		[SerializeField]
		private GameObject currentMark;

		// Token: 0x040079A9 RID: 31145
		[SerializeField]
		private Sprite lockedPointSp;

		// Token: 0x040079AA RID: 31146
		[SerializeField]
		private Sprite unlockedPointSp;
	}
}
