using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.SkillBreak;
using GameData.Domains.Character;
using GameData.Domains.Extra;
using GameData.Domains.Story;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B6C RID: 2924
	public class ConflickSkillDisplayItem : MonoBehaviour
	{
		// Token: 0x060090B4 RID: 37044 RVA: 0x004373F4 File Offset: 0x004355F4
		public void Init(Action<int, int> onPresetChange, Action onSelect)
		{
			bool flag = this.presetToggleGroup != null;
			if (flag)
			{
				this.presetToggleGroup.Init(-1);
				this.presetToggleGroup.OnActiveIndexChange += onPresetChange;
			}
			CButton cbutton = this.selectPracticeBtn;
			if (cbutton != null)
			{
				cbutton.ClearAndAddListener(onSelect);
			}
		}

		// Token: 0x060090B5 RID: 37045 RVA: 0x00437444 File Offset: 0x00435644
		public void SetEmpty(bool isEmpty)
		{
			bool flag = this.emptyState != null;
			if (flag)
			{
				this.emptyState.SetActive(isEmpty);
			}
			this.normalState.SetActive(true);
			this.maxPowerArea.gameObject.SetActive(true);
			if (isEmpty)
			{
				this.maxPowerArea.gameObject.SetActive(false);
				this.bonusLayout.Clean();
			}
		}

		// Token: 0x060090B6 RID: 37046 RVA: 0x004374B4 File Offset: 0x004356B4
		public void UpdatePresetIndex(int presetIndex)
		{
			bool flag = this.presetToggleGroup == null;
			if (!flag)
			{
				this.presetToggleGroup.SetWithoutNotify(presetIndex);
				this.presetToggleGroup.gameObject.SetActive(true);
			}
		}

		// Token: 0x060090B7 RID: 37047 RVA: 0x004374F4 File Offset: 0x004356F4
		public void RefreshMaxPower(short skillTemplateId, SkillBreakPlate plate)
		{
			bool flag = this.maxPowerArea != null;
			if (flag)
			{
				this.maxPowerArea.SetActive(plate != null);
			}
			bool flag2 = plate == null || this.maxPowerLabel == null || this.powerLevelName == null;
			if (!flag2)
			{
				this.maxPowerLabel.text = string.Format("{0}", plate.AddMaxPower);
				this.powerLevelName.text = ViewCharacterMenuSkillBreakPlate.GetBreakoutMaxPowerName(skillTemplateId, plate.AddMaxPower, false);
			}
		}

		// Token: 0x060090B8 RID: 37048 RVA: 0x00437584 File Offset: 0x00435784
		public void RefreshContent(short skillTemplateId, SkillBreakPlate plate, bool showEmpty, LifeSkillShorts lifeSkillAttainments, IAsyncMethodRequestHandler requestHandler, Func<bool> isStillValid)
		{
			this.SetEmpty(showEmpty);
			if (!showEmpty)
			{
				this.RefreshMaxPower(skillTemplateId, plate);
				bool flag = plate != null;
				if (flag)
				{
					List<SkillBreakPlateBonus> bonusList = ConflickSkillDisplayItem.CollectPlateBonuses(plate);
					StoryDomainMethod.AsyncCall.GetEmeiBreakBonusDisplayData(requestHandler, skillTemplateId, delegate(int offset, RawDataPool pool)
					{
						bool flag2 = !isStillValid();
						if (flag2)
						{
							EasyPool.Free<List<SkillBreakPlateBonus>>(bonusList);
						}
						else
						{
							SectStoryBonusDisplayData emeiData = new SectStoryBonusDisplayData();
							Serializer.Deserialize(pool, offset, ref emeiData);
							SkillBreakPlateBonusGridLayout skillBreakPlateBonusGridLayout = this.bonusLayout;
							if (skillBreakPlateBonusGridLayout != null)
							{
								skillBreakPlateBonusGridLayout.RefreshWithExtraBonus(bonusList, emeiData, skillTemplateId, lifeSkillAttainments, requestHandler);
							}
							EasyPool.Free<List<SkillBreakPlateBonus>>(bonusList);
						}
					});
				}
				else
				{
					StoryDomainMethod.AsyncCall.GetEmeiBreakBonusDisplayData(requestHandler, skillTemplateId, delegate(int offset, RawDataPool pool)
					{
						bool flag2 = !isStillValid();
						if (!flag2)
						{
							SectStoryBonusDisplayData emeiData = new SectStoryBonusDisplayData();
							Serializer.Deserialize(pool, offset, ref emeiData);
							SkillBreakPlateBonusGridLayout skillBreakPlateBonusGridLayout = this.bonusLayout;
							if (skillBreakPlateBonusGridLayout != null)
							{
								skillBreakPlateBonusGridLayout.RefreshWithExtraBonus(null, emeiData, skillTemplateId, lifeSkillAttainments, requestHandler);
							}
						}
					});
				}
			}
		}

		// Token: 0x060090B9 RID: 37049 RVA: 0x00437648 File Offset: 0x00435848
		private static List<SkillBreakPlateBonus> CollectPlateBonuses(SkillBreakPlate plate)
		{
			List<SkillBreakPlateBonus> bonusList = EasyPool.Get<List<SkillBreakPlateBonus>>();
			foreach (SkillBreakPlateIndex coordinate in plate.GetIndexes())
			{
				SkillBreakPlateBonus bonus = plate.GetBonus(coordinate);
				bool flag = bonus.Type == ESkillBreakPlateBonusType.None;
				if (!flag)
				{
					bonusList.Add(bonus);
				}
			}
			return bonusList;
		}

		// Token: 0x04006F64 RID: 28516
		[SerializeField]
		private GameObject normalState;

		// Token: 0x04006F65 RID: 28517
		[SerializeField]
		private GameObject emptyState;

		// Token: 0x04006F66 RID: 28518
		[SerializeField]
		private GameObject maxPowerArea;

		// Token: 0x04006F67 RID: 28519
		[SerializeField]
		private TextMeshProUGUI maxPowerLabel;

		// Token: 0x04006F68 RID: 28520
		[SerializeField]
		private TextMeshProUGUI powerLevelName;

		// Token: 0x04006F69 RID: 28521
		[SerializeField]
		private SkillBreakPlateBonusGridLayout bonusLayout;

		// Token: 0x04006F6A RID: 28522
		[SerializeField]
		private CToggleGroup presetToggleGroup;

		// Token: 0x04006F6B RID: 28523
		[SerializeField]
		private CButton selectPracticeBtn;
	}
}
