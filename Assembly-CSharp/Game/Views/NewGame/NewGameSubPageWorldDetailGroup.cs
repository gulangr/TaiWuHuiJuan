using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.World;
using GameData.GameDataBridge;
using TMPro;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x02000812 RID: 2066
	public class NewGameSubPageWorldDetailGroup : MonoBehaviour
	{
		// Token: 0x17000C35 RID: 3125
		// (get) Token: 0x0600656A RID: 25962 RVA: 0x002E5AE0 File Offset: 0x002E3CE0
		public IReadOnlyList<NewGameSubPageWorldDetailItem> DetailItemArray
		{
			get
			{
				return this.detailItemArray;
			}
		}

		// Token: 0x17000C36 RID: 3126
		// (get) Token: 0x0600656B RID: 25963 RVA: 0x002E5AE8 File Offset: 0x002E3CE8
		// (set) Token: 0x0600656C RID: 25964 RVA: 0x002E5AF0 File Offset: 0x002E3CF0
		public int SettingCount { get; private set; }

		// Token: 0x17000C37 RID: 3127
		// (get) Token: 0x0600656D RID: 25965 RVA: 0x002E5AF9 File Offset: 0x002E3CF9
		public WorldCreationGroupItem ConfigItem
		{
			get
			{
				return WorldCreationGroup.Instance[this._groupId];
			}
		}

		// Token: 0x17000C38 RID: 3128
		// (get) Token: 0x0600656E RID: 25966 RVA: 0x002E5B0B File Offset: 0x002E3D0B
		public sbyte GroupId
		{
			get
			{
				return this._groupId;
			}
		}

		// Token: 0x17000C39 RID: 3129
		// (get) Token: 0x0600656F RID: 25967 RVA: 0x002E5B13 File Offset: 0x002E3D13
		private bool IsRegular
		{
			get
			{
				return this._groupId == 3;
			}
		}

		// Token: 0x06006570 RID: 25968 RVA: 0x002E5B20 File Offset: 0x002E3D20
		public void Init(sbyte groupId, Action<byte, byte> onSettingChangedHandler, Action<NewGameSubPageWorldDetailItem> onEnter = null, Action<NewGameSubPageWorldDetailItem> onExit = null)
		{
			this._groupId = groupId;
			this._onSettingChanged = onSettingChangedHandler;
			this._level = 0;
			this.SettingCount = 0;
			foreach (NewGameSubPageWorldDetailItem setting in this.detailItemArray)
			{
				setting.gameObject.SetActive(false);
			}
			WorldCreationGroupItem groupCfg = WorldCreationGroup.Instance[groupId];
			for (int i = 0; i < groupCfg.WorldCreations.Length; i++)
			{
				byte templateId = groupCfg.WorldCreations[i];
				NewGameSubPageWorldDetailItem settingItem = this.GetOrCreateNextSettingItem();
				settingItem.Init(templateId, groupId, new Action<byte, byte>(this.OnValueChanged), onEnter, onExit);
				settingItem.gameObject.SetActive(true);
			}
			bool flag = !this.IsRegular;
			if (flag)
			{
				bool isLocked = NewGameSubPageWorldDetail.IsDifficultyLocked(WorldCreationInfo.EDifficultyLevel.Level4.ToInt());
				foreach (NewGameSubPageWorldDetailItem detailItem in this.detailItemArray)
				{
					bool flag2 = isLocked;
					if (flag2)
					{
						detailItem.SetValueRange(0, 2);
					}
					else
					{
						detailItem.SetValueRangeDefault();
					}
				}
			}
			bool flag3 = this.tipLegacyLevel;
			if (flag3)
			{
				this.tipLegacyLevel.Type = TipType.LegacyLevel;
			}
			this.UpdateProgress();
		}

		// Token: 0x06006571 RID: 25969 RVA: 0x002E5CA0 File Offset: 0x002E3EA0
		private void OnValueChanged(byte templateId, byte value)
		{
			this._creationInfo.Set(templateId, value);
			this._level = this._creationInfo.GetGroupLevel(this._groupId);
			this.UpdateProgress();
			Action<byte, byte> onSettingChanged = this._onSettingChanged;
			if (onSettingChanged != null)
			{
				onSettingChanged(templateId, value);
			}
		}

		// Token: 0x06006572 RID: 25970 RVA: 0x002E5CF0 File Offset: 0x002E3EF0
		private NewGameSubPageWorldDetailItem GetOrCreateNextSettingItem()
		{
			int settingCount = this.SettingCount;
			this.SettingCount = settingCount + 1;
			bool flag = this.detailItemArray.Count > settingCount;
			NewGameSubPageWorldDetailItem result;
			if (flag)
			{
				result = this.detailItemArray[settingCount];
			}
			else
			{
				NewGameSubPageWorldDetailItem item = Object.Instantiate<NewGameSubPageWorldDetailItem>(this.detailItemArray[0], this.detailItemArray[0].transform.parent);
				this.detailItemArray.Add(item);
				result = item;
			}
			return result;
		}

		// Token: 0x06006573 RID: 25971 RVA: 0x002E5D6C File Offset: 0x002E3F6C
		private void UpdateProgress()
		{
			this.textTitle.text = this.GetTitle();
			int sum = this._creationInfo.GetGroupLegacyBonusSum(this._groupId);
			TMP_Text tmp_Text = this.textTotalPoint;
			string text;
			if (!this.IsRegular)
			{
				string format = "{0}/{1}";
				object arg = sum.ToString().SetGradeColor(3);
				sbyte[] legacyGroupLevelThresholds = GlobalConfig.Instance.LegacyGroupLevelThresholds;
				text = string.Format(format, arg, legacyGroupLevelThresholds[legacyGroupLevelThresholds.Length - 1]);
			}
			else
			{
				text = string.Empty;
			}
			tmp_Text.text = text;
			for (int index = 0; index < this.detailItemArray.Count; index++)
			{
				NewGameSubPageWorldDetailItem setting = this.detailItemArray[index];
				CImage imageProgress = this.imageProgressArray[index];
				imageProgress.gameObject.SetActive(setting.gameObject.activeSelf && !this.IsRegular);
				imageProgress.color = NewGameSubPageWorldDetailGroup.GetLevelColor(setting.SettingValue);
			}
			bool flag = this.imageLevel;
			if (flag)
			{
				this.imageLevel.SetTexture("ui9_tex_world_detail_difficulty_group_" + this._level.ToString());
			}
		}

		// Token: 0x06006574 RID: 25972 RVA: 0x002E5E84 File Offset: 0x002E4084
		public string GetTitle()
		{
			WorldCreationGroupItem groupCfg = WorldCreationGroup.Instance[this._groupId];
			bool isRegular = this.IsRegular;
			string result;
			if (isRegular)
			{
				result = groupCfg.Name;
			}
			else
			{
				string dot = LocalStringManager.Get(LanguageKey.LK_Dot_Symbol);
				LanguageKey levelKey = LanguageKey.LK_WorldCreation_GroupLevel_0 + this._level;
				string levelText = levelKey.Tr();
				Color color = NewGameSubPageWorldDetailGroup.GetLevelColor(this._level);
				result = (groupCfg.Name + dot + levelText).SetColor(color);
			}
			return result;
		}

		// Token: 0x06006575 RID: 25973 RVA: 0x002E5F00 File Offset: 0x002E4100
		public void RefreshInteractable(EInteractType type)
		{
			bool flag = type == EInteractType.NewGamePreset;
			if (flag)
			{
				foreach (NewGameSubPageWorldDetailItem detailItem in this.detailItemArray)
				{
					detailItem.SetValueRangeDefault();
					detailItem.SetInteractable(true);
				}
			}
			else
			{
				bool flag2 = type == EInteractType.NewGameCustom;
				if (flag2)
				{
					foreach (NewGameSubPageWorldDetailItem detailItem2 in this.detailItemArray)
					{
						detailItem2.SetValueRangeDefault();
						detailItem2.SetInteractable(true);
					}
				}
				else
				{
					foreach (NewGameSubPageWorldDetailItem detailItem3 in this.detailItemArray)
					{
						int key = detailItem3.GetSettingValue();
						bool flag3 = type == EInteractType.Inherit;
						if (flag3)
						{
							bool flag4 = SingletonObject.getInstance<BasicGameData>().ChallengeModeData.IsEnabled(EChallengeModeImplement.LockWorldSettings);
							if (flag4)
							{
								detailItem3.SetInteractable(false);
							}
							else
							{
								detailItem3.SetValueRange(key - 1, key + 1);
								NewGameSubPageWorldDetailItem newGameSubPageWorldDetailItem = detailItem3;
								WorldCreationItem configItem = detailItem3.ConfigItem;
								newGameSubPageWorldDetailItem.SetInteractable(configItem != null && configItem.ShowInLegacy);
							}
						}
						else
						{
							bool canResetWorldSettings = GlobalOperations.CanResetWorldSettings;
							if (canResetWorldSettings)
							{
								detailItem3.SetValueRangeDefault();
								NewGameSubPageWorldDetailItem newGameSubPageWorldDetailItem2 = detailItem3;
								WorldCreationItem configItem2 = detailItem3.ConfigItem;
								newGameSubPageWorldDetailItem2.SetInteractable(configItem2 != null && configItem2.ShowInLegacy);
							}
							else
							{
								detailItem3.SetValueRangeDefault();
								detailItem3.SetInteractable(false);
							}
						}
					}
				}
			}
		}

		// Token: 0x06006576 RID: 25974 RVA: 0x002E60C0 File Offset: 0x002E42C0
		public void LoadByDifficultyPreset(int difficulty)
		{
			this._creationInfo = WorldCreationInfo.CreateByDifficultyPreset((sbyte)difficulty);
			this.LoadFromWorldCreationInfo(this._creationInfo);
		}

		// Token: 0x06006577 RID: 25975 RVA: 0x002E60E0 File Offset: 0x002E42E0
		public void LoadFromWorldCreationInfo(WorldCreationInfo creationInfo)
		{
			for (int i = 0; i < this.SettingCount; i++)
			{
				NewGameSubPageWorldDetailItem setting = this.detailItemArray[i];
				int value = creationInfo.Get(setting.ConfigItem.TemplateId);
				setting.SetWithoutNotify(value);
			}
			this._creationInfo = creationInfo;
			this._level = this._creationInfo.GetGroupLevel(this._groupId);
			this.UpdateProgress();
		}

		// Token: 0x06006578 RID: 25976 RVA: 0x002E6154 File Offset: 0x002E4354
		public void SaveToWorldCreationInfo(ref WorldCreationInfo creationInfo)
		{
			for (int i = 0; i < this.SettingCount; i++)
			{
				NewGameSubPageWorldDetailItem setting = this.detailItemArray[i];
				int value = setting.GetSettingValue();
				creationInfo.Set(setting.ConfigItem.TemplateId, (byte)value);
			}
		}

		// Token: 0x06006579 RID: 25977 RVA: 0x002E61A4 File Offset: 0x002E43A4
		public static Color GetLevelColor(int level)
		{
			if (!true)
			{
			}
			Color result;
			switch (level)
			{
			case 0:
				result = Colors.Instance.GradeColors[1];
				break;
			case 1:
				result = Colors.Instance.GradeColors[4];
				break;
			case 2:
				result = Colors.Instance.GradeColors[6];
				break;
			case 3:
				result = Colors.Instance.GradeColors[8];
				break;
			default:
				throw new ArgumentOutOfRangeException("level", level, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x040046AD RID: 18093
		[SerializeField]
		private CRawImage imageLevel;

		// Token: 0x040046AE RID: 18094
		[SerializeField]
		private TooltipInvoker tipLegacyLevel;

		// Token: 0x040046AF RID: 18095
		[SerializeField]
		private TextMeshProUGUI textTitle;

		// Token: 0x040046B0 RID: 18096
		[SerializeField]
		private TextMeshProUGUI textTotalPoint;

		// Token: 0x040046B1 RID: 18097
		[SerializeField]
		private CImage[] imageProgressArray;

		// Token: 0x040046B2 RID: 18098
		[SerializeField]
		private List<NewGameSubPageWorldDetailItem> detailItemArray;

		// Token: 0x040046B4 RID: 18100
		private sbyte _groupId;

		// Token: 0x040046B5 RID: 18101
		private int _level;

		// Token: 0x040046B6 RID: 18102
		private Action<byte, byte> _onSettingChanged;

		// Token: 0x040046B7 RID: 18103
		private WorldCreationInfo _creationInfo;
	}
}
