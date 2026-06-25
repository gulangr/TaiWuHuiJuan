using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Building;
using TMPro;
using UnityEngine;

namespace Game.Views.Building
{
	// Token: 0x02000BE8 RID: 3048
	public class RecruitFlatModeScrollView : MonoBehaviour
	{
		// Token: 0x06009A8B RID: 39563 RVA: 0x00485FB8 File Offset: 0x004841B8
		public void Set(List<BuildingRecruitCharacterData> characterDataList, HashSet<BuildingRecruitCharacterData> selectedCharacterIds, short buildingTemplateId, Action<BuildingRecruitCharacterData> onClick)
		{
			this._data = characterDataList;
			this._onClick = onClick;
			BuildingBlockItem buildingConfig = BuildingBlock.Instance[buildingTemplateId];
			sbyte requireLifeSkillType = buildingConfig.RequireLifeSkillType;
			sbyte requireCombatSkillType = buildingConfig.RequireCombatSkillType;
			this.txtTitle.text = buildingConfig.Name;
			CommonUtils.PrepareEnoughChildren(this.contentRect, this.characterViewTemplate.gameObject, characterDataList.Count, null);
			int selectedAmount = 0;
			for (int i = 0; i < characterDataList.Count; i++)
			{
				int currentIndex = i;
				RecruitFlatModeCharacterView view = this.contentRect.GetChild(i).GetComponent<RecruitFlatModeCharacterView>();
				bool viewSelected = selectedCharacterIds.Contains(characterDataList[i]);
				bool flag = viewSelected;
				if (flag)
				{
					selectedAmount++;
				}
				view.Set(characterDataList[i], viewSelected, requireLifeSkillType, requireCombatSkillType, delegate
				{
					this.OnClickChar(currentIndex);
				}, buildingTemplateId);
			}
			this.txtAmount.text = string.Format("({0}/{1})", selectedAmount, characterDataList.Count);
		}

		// Token: 0x06009A8C RID: 39564 RVA: 0x004860DB File Offset: 0x004842DB
		private void OnClickChar(int currentIndex)
		{
			Action<BuildingRecruitCharacterData> onClick = this._onClick;
			if (onClick != null)
			{
				onClick(this._data[currentIndex]);
			}
		}

		// Token: 0x04007788 RID: 30600
		[SerializeField]
		private RecruitFlatModeCharacterView characterViewTemplate;

		// Token: 0x04007789 RID: 30601
		[SerializeField]
		private RectTransform contentRect;

		// Token: 0x0400778A RID: 30602
		[SerializeField]
		private TextMeshProUGUI txtTitle;

		// Token: 0x0400778B RID: 30603
		[SerializeField]
		private TextMeshProUGUI txtAmount;

		// Token: 0x0400778C RID: 30604
		private List<BuildingRecruitCharacterData> _data;

		// Token: 0x0400778D RID: 30605
		private Action<BuildingRecruitCharacterData> _onClick;
	}
}
