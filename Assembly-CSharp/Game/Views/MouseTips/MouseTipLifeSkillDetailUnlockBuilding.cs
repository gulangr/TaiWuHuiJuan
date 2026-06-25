using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using Game.Views.Encyclopedia;
using Game.Views.MouseTips.LifeSkill;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x0200086B RID: 2155
	public class MouseTipLifeSkillDetailUnlockBuilding : MouseTipBase
	{
		// Token: 0x060067FB RID: 26619 RVA: 0x002F8094 File Offset: 0x002F6294
		protected override void Init(ArgumentBox argsBox)
		{
			this.Element.ForceListenCommand = true;
			MouseTipLifeSkillDetailUnlockInformation.ProgressProvider progressProvider;
			LifeSkillTypeItem lifeSkillTypeConfig;
			bool flag = !argsBox.Get<MouseTipLifeSkillDetailUnlockInformation.ProgressProvider>(MouseTipLifeSkillDetailUnlockInformation.ArgKeyProgressProvider, out progressProvider) || !argsBox.Get<LifeSkillTypeItem>(MouseTipLifeSkillDetailUnlockInformation.ArgKeyLifeSkillType, out lifeSkillTypeConfig);
			if (!flag)
			{
				short dependBuildingTemplateId = 0;
				int index = 0;
				foreach (LifeSkillItem lifeSkillConfig in ((IEnumerable<LifeSkillItem>)LifeSkill.Instance))
				{
					bool flag2 = lifeSkillConfig == null || lifeSkillConfig.Type != lifeSkillTypeConfig.TemplateId;
					if (!flag2)
					{
						sbyte[] readingProgress = progressProvider(lifeSkillConfig.TemplateId);
						for (int pageIndex = 0; pageIndex < lifeSkillConfig.UnlockBuildingList.Count; pageIndex++)
						{
							foreach (short buildingTemplateId in lifeSkillConfig.UnlockBuildingList[pageIndex].DataList)
							{
								BuildingBlockItem buildingConfig = BuildingBlock.Instance.GetItem(buildingTemplateId);
								bool flag3 = buildingConfig == null || buildingConfig.TemplateId == 0;
								if (!flag3)
								{
									List<short> dependBuildings = buildingConfig.DependBuildings;
									bool flag4 = dependBuildings != null && dependBuildings.Count > 0;
									if (flag4)
									{
										dependBuildingTemplateId = buildingConfig.DependBuildings[0];
									}
									bool flag5 = index >= this.holder.childCount;
									if (flag5)
									{
										Object.Instantiate<Transform>(this.holder.GetChild(0), this.holder);
									}
									UnlockBuilding obj = this.holder.GetChild(index++).GetComponent<UnlockBuilding>();
									sbyte progress = readingProgress[pageIndex];
									bool unlocked = progress == 100;
									obj.Set(buildingConfig, lifeSkillConfig.TemplateId, pageIndex, unlocked);
									obj.gameObject.SetActive(true);
								}
							}
						}
					}
				}
				for (int i = index; i < this.holder.childCount; i++)
				{
					this.holder.GetChild(i).gameObject.SetActive(false);
				}
				this.desc.text = LocalStringManager.GetFormat(LanguageKey.LK_CharacterMenu_LifeSkill_UnlockBuilding_Tips_Text_2, lifeSkillTypeConfig.Name, BuildingBlock.Instance[dependBuildingTemplateId].Name.SetColor("orange"));
			}
		}

		// Token: 0x060067FC RID: 26620 RVA: 0x002F8330 File Offset: 0x002F6530
		private void Update()
		{
			bool flag = CommonCommandKit.PrimaryInteraction.Check(this.Element, false, false, false, true, false);
			if (flag)
			{
				ViewEncyclopediaPanel.OpenLink(EncyclopediaTipLink.DefValue.Buildings);
			}
		}

		// Token: 0x04004975 RID: 18805
		[SerializeField]
		private TextMeshProUGUI desc;

		// Token: 0x04004976 RID: 18806
		[SerializeField]
		private Transform holder;
	}
}
