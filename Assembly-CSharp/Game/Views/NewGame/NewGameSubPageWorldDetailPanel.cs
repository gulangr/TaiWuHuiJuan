using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.UIElements;
using GameData.Domains.World;
using TMPro;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x02000815 RID: 2069
	public class NewGameSubPageWorldDetailPanel : MonoBehaviour
	{
		// Token: 0x06006599 RID: 26009 RVA: 0x002E68C0 File Offset: 0x002E4AC0
		private void Awake()
		{
			this.toggleGroupDifficultyLevel.Init(-1);
			this.toggleGroupDifficultyLevel.OnActiveIndexChange += this.ToggleGroupDifficultyLevelOnOnActiveIndexChange;
			List<CToggle> all = this.toggleGroupDifficultyLevel.GetAll();
			for (int index = 0; index < all.Count; index++)
			{
				CToggle toggle = all[index];
				TextMeshProUGUI textTitle = toggle.GetComponentInChildren<TextMeshProUGUI>();
				textTitle.text = NewGameSubPageWorldDetail.DifficultyLevelKeys[index].Tr().ColorReplace();
			}
			sbyte i = 0;
			while ((int)i < WorldCreationGroup.Instance.Count)
			{
				NewGameSubPageWorldDetailGroup group = this.detailGroupList[(int)i];
				bool flag = i == 3;
				if (flag)
				{
					group.Init(i, null, null, null);
				}
				else
				{
					group.Init(i, new Action<byte, byte>(this.OnWorldSettingChangedSettingView), null, null);
				}
				i += 1;
			}
		}

		// Token: 0x0600659A RID: 26010 RVA: 0x002E69A8 File Offset: 0x002E4BA8
		public void Init(ref WorldCreationInfo worldCreationInfo, EInteractType type)
		{
			this._interactType = type;
			foreach (NewGameSubPageWorldDetailGroup group in this.detailGroupList)
			{
				group.LoadFromWorldCreationInfo(worldCreationInfo);
				group.RefreshInteractable(type);
			}
		}

		// Token: 0x0600659B RID: 26011 RVA: 0x002E6A18 File Offset: 0x002E4C18
		public void Init(Dictionary<string, string> creationInfoMap, TextMeshProUGUI textTabValue, NewGameSubPageWorldDetailToggleHelper toggleHelper)
		{
			this._textTabValue = textTabValue;
			this._toggleHelper = toggleHelper;
			string difficultyStr;
			creationInfoMap.TryGetValue("OverallDifficulty", out difficultyStr);
			sbyte difficulty;
			sbyte.TryParse(difficultyStr, out difficulty);
			this.toggleGroupDifficultyLevel.Set((int)difficulty, false);
			this._textTabValue.text = NewGameSubPageWorldDetail.DifficultyLevelKeys[(int)difficulty].Tr().ColorReplace();
			int maxDifficulty = NewGameSubPageWorldDetail.IsDifficultyLocked(WorldCreationInfo.EDifficultyLevel.Level4.ToInt()) ? WorldCreationInfo.EDifficultyLevel.Level3.ToInt() : WorldCreationInfo.EDifficultyLevel.Level4.ToInt();
			sbyte limitedDifficulty = (sbyte)Mathf.Clamp((int)difficulty, WorldCreationInfo.EDifficultyLevel.Level1.ToInt(), maxDifficulty);
			WorldCreationInfo creationInfo = WorldCreationInfo.CreateByDifficultyPreset(limitedDifficulty);
			foreach (WorldCreationItem creationCfg in ((IEnumerable<WorldCreationItem>)WorldCreation.Instance))
			{
				string valueStr;
				byte value;
				bool flag = creationInfoMap.TryGetValue(creationCfg.SaveFileKey, out valueStr) && byte.TryParse(valueStr, out value);
				if (flag)
				{
					creationInfo.Set(creationCfg.TemplateId, value);
				}
			}
			bool flag2 = (int)difficulty == WorldCreationInfo.EDifficultyLevel.Custom.ToInt();
			if (flag2)
			{
				foreach (WorldCreationGroupItem groupCfg in ((IEnumerable<WorldCreationGroupItem>)WorldCreationGroup.Instance))
				{
					NewGameSubPageWorldDetailGroup groupView = this.detailGroupList[(int)groupCfg.TemplateId];
					groupView.LoadFromWorldCreationInfo(creationInfo);
					groupView.RefreshInteractable(EInteractType.NewGameCustom);
				}
			}
			else
			{
				foreach (NewGameSubPageWorldDetailGroup group in this.detailGroupList)
				{
					bool flag3 = group.GroupId == 3;
					if (flag3)
					{
						NewGameSubPageWorldDetailGroup groupView2 = this.detailGroupList[3];
						groupView2.LoadFromWorldCreationInfo(creationInfo);
						group.RefreshInteractable(EInteractType.NewGameCustom);
					}
					else
					{
						group.RefreshInteractable(EInteractType.NewGameCustom);
						group.LoadByDifficultyPreset((int)difficulty);
					}
				}
			}
			this.RefreshDifficultyLevelLockState();
			this.RefreshToggleHelper();
		}

		// Token: 0x0600659C RID: 26012 RVA: 0x002E6C48 File Offset: 0x002E4E48
		public void Save(Dictionary<string, string> creationInfoMap, ref WorldCreationInfo worldCreationInfo)
		{
			foreach (NewGameSubPageWorldDetailGroup group in this.detailGroupList)
			{
				group.SaveToWorldCreationInfo(ref worldCreationInfo);
			}
			foreach (WorldCreationItem creationCfg in ((IEnumerable<WorldCreationItem>)WorldCreation.Instance))
			{
				int value = worldCreationInfo.Get(creationCfg.TemplateId);
				creationInfoMap[creationCfg.SaveFileKey] = value.ToString();
			}
			creationInfoMap["OverallDifficulty"] = this.toggleGroupDifficultyLevel.GetActiveIndex().ToString();
		}

		// Token: 0x0600659D RID: 26013 RVA: 0x002E6D1C File Offset: 0x002E4F1C
		private void RefreshDifficultyLevelLockState()
		{
			for (int index = 0; index < this.difficultyLevelItemList.Count; index++)
			{
				NewGameSubPageWorldDetailDifficultyLevelItem item = this.difficultyLevelItemList[index];
				bool isLocked = NewGameSubPageWorldDetail.IsDifficultyLocked(index);
				item.SetLocked(isLocked);
			}
		}

		// Token: 0x0600659E RID: 26014 RVA: 0x002E6D64 File Offset: 0x002E4F64
		private void OnWorldSettingChangedSettingView(byte templateId, byte value)
		{
			bool flag = this._interactType == EInteractType.NewGamePreset;
			if (flag)
			{
				this.toggleGroupDifficultyLevel.Set(WorldCreationInfo.EDifficultyLevel.Custom.ToInt(), false);
			}
			this.RefreshToggleHelper();
		}

		// Token: 0x0600659F RID: 26015 RVA: 0x002E6DA0 File Offset: 0x002E4FA0
		private void ToggleGroupDifficultyLevelOnOnActiveIndexChange(int newIndex, int oldIndex)
		{
			bool isLocked = NewGameSubPageWorldDetail.IsDifficultyLocked(newIndex);
			bool flag = isLocked;
			if (flag)
			{
				string title = LanguageKey.LK_NewGame_WorldDetail_DifficultyLevel_ForceOpen_Title.Tr();
				string content = LanguageKey.LK_NewGame_WorldDetail_DifficultyLevel_ForceOpen_Content.Tr();
				CommonUtils.ShowConfirmDialog(title, content, delegate
				{
					SingletonObject.getInstance<GlobalSettings>().ForceUnlockWorldDetailDifficultyLevel4 = true;
					this.ChangeDifficulty(newIndex);
					this.RefreshDifficultyLevelLockState();
				}, delegate
				{
					this.toggleGroupDifficultyLevel.SetWithoutNotify(oldIndex);
				}, EDialogType.None);
			}
			else
			{
				this.ChangeDifficulty(newIndex);
			}
		}

		// Token: 0x060065A0 RID: 26016 RVA: 0x002E6E24 File Offset: 0x002E5024
		private void ChangeDifficulty(int difficulty)
		{
			bool flag = this._textTabValue;
			if (flag)
			{
				this._textTabValue.text = NewGameSubPageWorldDetail.DifficultyLevelKeys[difficulty].Tr().ColorReplace();
			}
			foreach (NewGameSubPageWorldDetailGroup group in this.detailGroupList)
			{
				group.RefreshInteractable(EInteractType.NewGamePreset);
				bool flag2 = difficulty == WorldCreationInfo.EDifficultyLevel.Custom.ToInt();
				if (!flag2)
				{
					bool flag3 = group.GroupId == 3;
					if (!flag3)
					{
						group.LoadByDifficultyPreset((int)((sbyte)difficulty));
					}
				}
			}
			this.RefreshToggleHelper();
		}

		// Token: 0x060065A1 RID: 26017 RVA: 0x002E6EE0 File Offset: 0x002E50E0
		private void RefreshToggleHelper()
		{
			List<string> titleList = (from g in this.detailGroupList.Take(3)
			select g.GetTitle()).ToList<string>();
			NewGameSubPageWorldDetailToggleHelper toggleHelper = this._toggleHelper;
			if (toggleHelper != null)
			{
				toggleHelper.Refresh(titleList);
			}
		}

		// Token: 0x040046D7 RID: 18135
		[SerializeField]
		private CToggleGroup toggleGroupDifficultyLevel;

		// Token: 0x040046D8 RID: 18136
		[SerializeField]
		private List<NewGameSubPageWorldDetailDifficultyLevelItem> difficultyLevelItemList;

		// Token: 0x040046D9 RID: 18137
		[SerializeField]
		private List<NewGameSubPageWorldDetailGroup> detailGroupList;

		// Token: 0x040046DA RID: 18138
		private TextMeshProUGUI _textTabValue;

		// Token: 0x040046DB RID: 18139
		private NewGameSubPageWorldDetailToggleHelper _toggleHelper;

		// Token: 0x040046DC RID: 18140
		private const string CreationInfoMapKey = "OverallDifficulty";

		// Token: 0x040046DD RID: 18141
		private EInteractType _interactType;
	}
}
