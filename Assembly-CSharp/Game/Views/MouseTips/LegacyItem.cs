using System;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.World;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000833 RID: 2099
	public class LegacyItem : MonoBehaviour
	{
		// Token: 0x0600669B RID: 26267 RVA: 0x002EC810 File Offset: 0x002EAA10
		public void RefreshLegacyItem(Vector2Int legacyProperty, int legacyMaxPoint, WorldCreationInfo worldCreationInfo, int times, int legacyPointBonusFactor)
		{
			int value = legacyProperty.y;
			int templateId = legacyProperty.x;
			LegacyPointItem config = LegacyPoint.Instance[templateId];
			this.pointName.text = LanguageKey.LK_Legacy_Detail_Max_Item.TrFormat(new object[]
			{
				config.Name,
				(value < legacyMaxPoint) ? "lightgrey" : "brightyellow",
				value,
				legacyMaxPoint
			}).ColorReplace();
			this.pointPerTimeValue.text = LegacyItem.CalculateBasePointForLegacy(templateId, worldCreationInfo, legacyPointBonusFactor).ToString();
			this.pointTimes.text = times.ToString();
			this.conditionText.text = LanguageKey.LK_Legacy_Gain_With.TrFormat(config.ConditionDesc).ColorReplace();
			this.maxIcon.SetActive(value >= legacyMaxPoint);
			this.worldPercentLayout.Rebuild<LegacyItemWorldPercent>(config.BonusTypes.Length, delegate(LegacyItemWorldPercent percent, int i)
			{
				byte creationType = config.BonusTypes[i];
				int bonusIndex = MouseTipLegacy.GetWorldCreationPercentValue(worldCreationInfo, creationType);
				short[] bonus = WorldCreation.Instance[creationType].LegacyPointBonus;
				string iconName = WorldCreation.Instance[creationType].Icons[bonusIndex].Replace("newgame_shijie_icon_", "mousetip_shijiexijie_");
				percent.Set((int)bonus[bonusIndex], iconName);
			});
		}

		// Token: 0x0600669C RID: 26268 RVA: 0x002EC934 File Offset: 0x002EAB34
		private static int CalculateBasePointForLegacy(int legacyTemplateId, WorldCreationInfo worldCreationInfo, int legacyPointBonusFactor)
		{
			LegacyPointItem config = LegacyPoint.Instance[legacyTemplateId];
			int settingsPercent = 100;
			foreach (byte creationType in config.BonusTypes)
			{
				int bonusIndex = MouseTipLegacy.GetWorldCreationPercentValue(worldCreationInfo, creationType);
				bool flag = bonusIndex < 0;
				if (!flag)
				{
					short[] bonus = WorldCreation.Instance[creationType].LegacyPointBonus;
					bool flag2 = bonus.CheckIndex(bonusIndex);
					if (flag2)
					{
						settingsPercent += (int)bonus[bonusIndex];
					}
				}
			}
			return (int)config.BasePoint * settingsPercent * (100 + legacyPointBonusFactor) / 10000;
		}

		// Token: 0x0600669D RID: 26269 RVA: 0x002EC9C8 File Offset: 0x002EABC8
		public void RefreshWhenAltChange(bool altDown)
		{
			this.conditionArea.SetActive(altDown);
			this.worldPercentArea.SetActive(altDown);
			this.detailBase.SetActive(altDown);
		}

		// Token: 0x040047CE RID: 18382
		[SerializeField]
		private TMP_Text pointName;

		// Token: 0x040047CF RID: 18383
		[SerializeField]
		private TMP_Text pointPerTimeValue;

		// Token: 0x040047D0 RID: 18384
		[SerializeField]
		private TMP_Text pointTimes;

		// Token: 0x040047D1 RID: 18385
		[SerializeField]
		private TMP_Text conditionText;

		// Token: 0x040047D2 RID: 18386
		[SerializeField]
		private GameObject conditionArea;

		// Token: 0x040047D3 RID: 18387
		[SerializeField]
		private GameObject worldPercentArea;

		// Token: 0x040047D4 RID: 18388
		[SerializeField]
		private GameObject maxIcon;

		// Token: 0x040047D5 RID: 18389
		[SerializeField]
		private GameObject detailBase;

		// Token: 0x040047D6 RID: 18390
		[SerializeField]
		private TemplatedContainerAssemblyNew worldPercentLayout;
	}
}
