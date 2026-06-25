using System;
using Game.Views.Encyclopedia.Save;
using GameData.Domains.Combat;
using GameData.Domains.Story;
using UnityEngine;

namespace Game.Views.SystemSetting
{
	// Token: 0x02000783 RID: 1923
	public class SystemSettingMapping
	{
		// Token: 0x17000AF7 RID: 2807
		// (get) Token: 0x06005C66 RID: 23654 RVA: 0x002ABAED File Offset: 0x002A9CED
		private static GlobalSettings Settings
		{
			get
			{
				return SingletonObject.getInstance<GlobalSettings>();
			}
		}

		// Token: 0x17000AF8 RID: 2808
		// (get) Token: 0x06005C67 RID: 23655 RVA: 0x002ABAF4 File Offset: 0x002A9CF4
		// (set) Token: 0x06005C68 RID: 23656 RVA: 0x002ABAFB File Offset: 0x002A9CFB
		public static AiOptions AiOptionsRef { get; set; }

		// Token: 0x17000AF9 RID: 2809
		// (get) Token: 0x06005C69 RID: 23657 RVA: 0x002ABB03 File Offset: 0x002A9D03
		// (set) Token: 0x06005C6A RID: 23658 RVA: 0x002ABB0F File Offset: 0x002A9D0F
		[EnumSetting(ESettingSubCategory.Localization, 1, LanguageKey.LK_SystemSetting_LocalizationSetting_Language, Key = ESettingKey.Language, TipLanguageKey = LanguageKey.LK_SystemSetting_LocalizationSetting_Language_Tips)]
		public string Language
		{
			get
			{
				return SystemSettingMapping.Settings.Language;
			}
			set
			{
				SystemSettingMapping.Settings.Language = value;
			}
		}

		// Token: 0x17000AFA RID: 2810
		// (get) Token: 0x06005C6B RID: 23659 RVA: 0x002ABB1D File Offset: 0x002A9D1D
		// (set) Token: 0x06005C6C RID: 23660 RVA: 0x002ABB29 File Offset: 0x002A9D29
		[BoolSetting(ESettingSubCategory.Saving, 1, LanguageKey.LK_SystemSetting_ArchiveFilesBackupOn, TipLanguageKey = LanguageKey.LK_SystemSetting_ArchiveFilesBackupOn_Tips, Key = ESettingKey.ArchiveFilesBackupOn)]
		public bool ArchiveFilesBackupOn
		{
			get
			{
				return SystemSettingMapping.Settings.ArchiveFilesBackupOn;
			}
			set
			{
				SystemSettingMapping.Settings.ArchiveFilesBackupOn = value;
			}
		}

		// Token: 0x17000AFB RID: 2811
		// (get) Token: 0x06005C6D RID: 23661 RVA: 0x002ABB37 File Offset: 0x002A9D37
		// (set) Token: 0x06005C6E RID: 23662 RVA: 0x002ABB43 File Offset: 0x002A9D43
		[IntSliderSetting(ESettingSubCategory.Saving, 2, LanguageKey.LK_SystemSetting_ArchiveFilesBackupCount, Min = 1, Max = 20, TipLanguageKey = LanguageKey.LK_SystemSetting_ArchiveFilesBackupCount_Tips, DependsOn = ESettingKey.ArchiveFilesBackupOn)]
		public sbyte ArchiveFilesBackupCount
		{
			get
			{
				return SystemSettingMapping.Settings.ArchiveFilesBackupCount;
			}
			set
			{
				SystemSettingMapping.Settings.ArchiveFilesBackupCount = value;
			}
		}

		// Token: 0x17000AFC RID: 2812
		// (get) Token: 0x06005C6F RID: 23663 RVA: 0x002ABB51 File Offset: 0x002A9D51
		// (set) Token: 0x06005C70 RID: 23664 RVA: 0x002ABB5D File Offset: 0x002A9D5D
		[IntSliderSetting(ESettingSubCategory.Saving, 3, LanguageKey.LK_SystemSetting_ArchiveFilesBackupInterval, Min = 1, Max = 12, TipLanguageKey = LanguageKey.LK_SystemSetting_ArchiveFilesBackupInterval_Tips, DependsOn = ESettingKey.ArchiveFilesBackupOn)]
		public sbyte ArchiveFilesBackupInterval
		{
			get
			{
				return SystemSettingMapping.Settings.ArchiveFilesBackupInterval;
			}
			set
			{
				SystemSettingMapping.Settings.ArchiveFilesBackupInterval = value;
			}
		}

		// Token: 0x17000AFD RID: 2813
		// (get) Token: 0x06005C71 RID: 23665 RVA: 0x002ABB6B File Offset: 0x002A9D6B
		// (set) Token: 0x06005C72 RID: 23666 RVA: 0x002ABB7E File Offset: 0x002A9D7E
		[SwitchButtonSetting(ESettingSubCategory.Saving, 4, LanguageKey.LK_SystemSetting_SaveSpeedType, Options = new LanguageKey[]
		{
			LanguageKey.LK_SystemSetting_SaveSpeedType_SpeedFirst,
			LanguageKey.LK_SystemSetting_SaveSpeedType_VolumeFirst
		})]
		public sbyte ImproveSaveSpeed
		{
			get
			{
				return SystemSettingMapping.Settings.ImproveSaveSpeed ? 0 : 1;
			}
			set
			{
				SystemSettingMapping.Settings.ImproveSaveSpeed = (value == 0);
			}
		}

		// Token: 0x17000AFE RID: 2814
		// (get) Token: 0x06005C73 RID: 23667 RVA: 0x002ABB8F File Offset: 0x002A9D8F
		// (set) Token: 0x06005C74 RID: 23668 RVA: 0x002ABB9B File Offset: 0x002A9D9B
		[BoolSetting(ESettingSubCategory.Game, 1, LanguageKey.LK_TaiwuSurname, TipLanguageKey = LanguageKey.LK_TaiwuSurname_Tips)]
		public bool HideTaiwuOriginalSurname
		{
			get
			{
				return SystemSettingMapping.Settings.HideTaiwuOriginalSurname;
			}
			set
			{
				SystemSettingMapping.Settings.HideTaiwuOriginalSurname = value;
			}
		}

		// Token: 0x17000AFF RID: 2815
		// (get) Token: 0x06005C75 RID: 23669 RVA: 0x002ABBA9 File Offset: 0x002A9DA9
		// (set) Token: 0x06005C76 RID: 23670 RVA: 0x002ABBB5 File Offset: 0x002A9DB5
		[BoolSetting(ESettingSubCategory.Game, 2, LanguageKey.LK_InteractCheckResultAnimation, TipLanguageKey = LanguageKey.LK_InteractCheckResultAnimation_Tips)]
		public bool EnableInteractCheckResultAnimation
		{
			get
			{
				return SystemSettingMapping.Settings.EnableInteractCheckResultAnimation;
			}
			set
			{
				SystemSettingMapping.Settings.EnableInteractCheckResultAnimation = value;
			}
		}

		// Token: 0x17000B00 RID: 2816
		// (get) Token: 0x06005C77 RID: 23671 RVA: 0x002ABBC3 File Offset: 0x002A9DC3
		// (set) Token: 0x06005C78 RID: 23672 RVA: 0x002ABBCF File Offset: 0x002A9DCF
		[BoolSetting(ESettingSubCategory.Game, 3, LanguageKey.LK_MiniScene_Title, TipLanguageKey = LanguageKey.LK_MiniScene_Title_Tips)]
		public bool MiniScene
		{
			get
			{
				return SystemSettingMapping.Settings.MiniScene;
			}
			set
			{
				SystemSettingMapping.Settings.MiniScene = value;
			}
		}

		// Token: 0x17000B01 RID: 2817
		// (get) Token: 0x06005C79 RID: 23673 RVA: 0x002ABBDD File Offset: 0x002A9DDD
		// (set) Token: 0x06005C7A RID: 23674 RVA: 0x002ABBE9 File Offset: 0x002A9DE9
		[BoolSetting(ESettingSubCategory.Game, 4, LanguageKey.LK_SystemSetting_MapWeather, TipLanguageKey = LanguageKey.LK_SystemSetting_MapWeather_Tips)]
		public bool MapWeather
		{
			get
			{
				return SystemSettingMapping.Settings.MapWeather;
			}
			set
			{
				SystemSettingMapping.Settings.MapWeather = value;
			}
		}

		// Token: 0x17000B02 RID: 2818
		// (get) Token: 0x06005C7B RID: 23675 RVA: 0x002ABBF7 File Offset: 0x002A9DF7
		// (set) Token: 0x06005C7C RID: 23676 RVA: 0x002ABC03 File Offset: 0x002A9E03
		[BoolSetting(ESettingSubCategory.Game, 5, LanguageKey.LK_SystemSetting_CombatWeather, TipLanguageKey = LanguageKey.LK_SystemSetting_CombatWeather_Tips)]
		public bool CombatWeather
		{
			get
			{
				return SystemSettingMapping.Settings.CombatWeather;
			}
			set
			{
				SystemSettingMapping.Settings.CombatWeather = value;
			}
		}

		// Token: 0x17000B03 RID: 2819
		// (get) Token: 0x06005C7D RID: 23677 RVA: 0x002ABC11 File Offset: 0x002A9E11
		// (set) Token: 0x06005C7E RID: 23678 RVA: 0x002ABC1D File Offset: 0x002A9E1D
		[BoolSetting(ESettingSubCategory.Game, 6, LanguageKey.LK_Building_Villager_Animation, TipLanguageKey = LanguageKey.LK_Building_Villager_Animation_Tips)]
		public bool VillagerAnimation
		{
			get
			{
				return SystemSettingMapping.Settings.VillagerAnimation;
			}
			set
			{
				SystemSettingMapping.Settings.VillagerAnimation = value;
			}
		}

		// Token: 0x17000B04 RID: 2820
		// (get) Token: 0x06005C7F RID: 23679 RVA: 0x002ABC2B File Offset: 0x002A9E2B
		// (set) Token: 0x06005C80 RID: 23680 RVA: 0x002ABC37 File Offset: 0x002A9E37
		[BoolSetting(ESettingSubCategory.Game, 7, LanguageKey.LK_Dynamic_Avatar, TipLanguageKey = LanguageKey.LK_Dynamic_Avatar_Tips)]
		public bool ShowDynamicAvatarIfPossible
		{
			get
			{
				return SystemSettingMapping.Settings.ShowDynamicAvatarIfPossible;
			}
			set
			{
				SystemSettingMapping.Settings.ShowDynamicAvatarIfPossible = value;
			}
		}

		// Token: 0x17000B05 RID: 2821
		// (get) Token: 0x06005C81 RID: 23681 RVA: 0x002ABC45 File Offset: 0x002A9E45
		// (set) Token: 0x06005C82 RID: 23682 RVA: 0x002ABC51 File Offset: 0x002A9E51
		[BoolSetting(ESettingSubCategory.Game, 8, LanguageKey.LK_SystemSetting_Adventure_Lighting, Key = ESettingKey.AdventureLighting, TipLanguageKey = LanguageKey.LK_SystemSetting_Adventure_Lighting_Tips)]
		public bool AdventureLighting
		{
			get
			{
				return SystemSettingMapping.Settings.AdventureLighting;
			}
			set
			{
				SystemSettingMapping.Settings.AdventureLighting = value;
			}
		}

		// Token: 0x17000B06 RID: 2822
		// (get) Token: 0x06005C83 RID: 23683 RVA: 0x002ABC5F File Offset: 0x002A9E5F
		// (set) Token: 0x06005C84 RID: 23684 RVA: 0x002ABC6B File Offset: 0x002A9E6B
		[FloatSliderSetting(ESettingSubCategory.Game, 9, LanguageKey.LK_SystemSetting_CombatSpeed, Min = 0.5f, Max = 3f, Format = "F1", SnapValues = new float[]
		{
			0.5f,
			1f,
			1.5f,
			2f,
			3f
		}, TipLanguageKey = LanguageKey.LK_SystemSetting_CombatSpeed_Tips)]
		public float CombatSpeed
		{
			get
			{
				return SystemSettingMapping.Settings.CombatSpeed;
			}
			set
			{
				SystemSettingMapping.Settings.CombatSpeed = value;
				GEvent.OnEvent(UiEvents.CombatSpeedChanged, null);
			}
		}

		// Token: 0x17000B07 RID: 2823
		// (get) Token: 0x06005C85 RID: 23685 RVA: 0x002ABC8B File Offset: 0x002A9E8B
		// (set) Token: 0x06005C86 RID: 23686 RVA: 0x002ABC97 File Offset: 0x002A9E97
		[BoolSetting(ESettingSubCategory.Game, 10, LanguageKey.LK_SystemSetting_AutoSaveCombatSpeed, TipLanguageKey = LanguageKey.LK_SystemSetting_AutoSaveCombatSpeed_Tips)]
		public bool AutoSaveCombatSpeed
		{
			get
			{
				return SystemSettingMapping.Settings.AutoSaveCombatSpeed;
			}
			set
			{
				SystemSettingMapping.Settings.AutoSaveCombatSpeed = value;
			}
		}

		// Token: 0x17000B08 RID: 2824
		// (get) Token: 0x06005C87 RID: 23687 RVA: 0x002ABCA5 File Offset: 0x002A9EA5
		// (set) Token: 0x06005C88 RID: 23688 RVA: 0x002ABCB1 File Offset: 0x002A9EB1
		[FloatSliderSetting(ESettingSubCategory.Game, 11, LanguageKey.LK_SystemSetting_DebateSpeed, Min = 0.5f, Max = 3f, Format = "F1", SnapValues = new float[]
		{
			0.5f,
			1f,
			1.5f,
			2f,
			3f
		}, TipLanguageKey = LanguageKey.LK_SystemSetting_DebateSpeed_Tips)]
		public float DebateSpeed
		{
			get
			{
				return SystemSettingMapping.Settings.DebateSpeed;
			}
			set
			{
				SystemSettingMapping.Settings.DebateSpeed = value;
			}
		}

		// Token: 0x17000B09 RID: 2825
		// (get) Token: 0x06005C89 RID: 23689 RVA: 0x002ABCBF File Offset: 0x002A9EBF
		// (set) Token: 0x06005C8A RID: 23690 RVA: 0x002ABCCB File Offset: 0x002A9ECB
		[BoolSetting(ESettingSubCategory.Game, 12, LanguageKey.LK_SystemSetting_AutoSaveDebateSpeed, TipLanguageKey = LanguageKey.LK_SystemSetting_AutoSaveDebateSpeed_Tips)]
		public bool AutoSaveDebateSpeed
		{
			get
			{
				return SystemSettingMapping.Settings.AutoSaveDebateSpeed;
			}
			set
			{
				SystemSettingMapping.Settings.AutoSaveDebateSpeed = value;
			}
		}

		// Token: 0x17000B0A RID: 2826
		// (get) Token: 0x06005C8B RID: 23691 RVA: 0x002ABCD9 File Offset: 0x002A9ED9
		// (set) Token: 0x06005C8C RID: 23692 RVA: 0x002ABCE5 File Offset: 0x002A9EE5
		[FloatSliderSetting(ESettingSubCategory.Game, 13, LanguageKey.LK_CricketCombat_Speed_Option, Min = 0.5f, Max = 3f, Format = "F1", SnapValues = new float[]
		{
			0.5f,
			1f,
			1.5f,
			2f,
			3f
		}, TipLanguageKey = LanguageKey.LK_CricketCombat_Speed_Option_Tips)]
		public float CricketCombatSpeed
		{
			get
			{
				return SystemSettingMapping.Settings.CricketCombatSpeed;
			}
			set
			{
				SystemSettingMapping.Settings.CricketCombatSpeed = value;
			}
		}

		// Token: 0x17000B0B RID: 2827
		// (get) Token: 0x06005C8D RID: 23693 RVA: 0x002ABCF3 File Offset: 0x002A9EF3
		// (set) Token: 0x06005C8E RID: 23694 RVA: 0x002ABCFF File Offset: 0x002A9EFF
		[BoolSetting(ESettingSubCategory.Game, 14, LanguageKey.LK_SystemSetting_AutoSaveCricketCombatSpeed, TipLanguageKey = LanguageKey.LK_SystemSetting_AutoSaveCricketCombatSpeed_Tips)]
		public bool AutoSaveCricketCombatSpeed
		{
			get
			{
				return SystemSettingMapping.Settings.AutoSaveCricketCombatSpeed;
			}
			set
			{
				SystemSettingMapping.Settings.AutoSaveCricketCombatSpeed = value;
			}
		}

		// Token: 0x17000B0C RID: 2828
		// (get) Token: 0x06005C8F RID: 23695 RVA: 0x002ABD0D File Offset: 0x002A9F0D
		// (set) Token: 0x06005C90 RID: 23696 RVA: 0x002ABD19 File Offset: 0x002A9F19
		[BoolSetting(ESettingSubCategory.Game, 15, LanguageKey.LK_Tutorial_Title, TipLanguageKey = LanguageKey.LK_Tutorial_Title_Tips)]
		public bool Guiding
		{
			get
			{
				return SystemSettingMapping.Settings.Guiding;
			}
			set
			{
				SystemSettingMapping.Settings.Guiding = value;
			}
		}

		// Token: 0x17000B0D RID: 2829
		// (get) Token: 0x06005C91 RID: 23697 RVA: 0x002ABD27 File Offset: 0x002A9F27
		// (set) Token: 0x06005C92 RID: 23698 RVA: 0x002ABD33 File Offset: 0x002A9F33
		[BoolSetting(ESettingSubCategory.Game, 16, LanguageKey.LK_SystemSetting_MakeToolAutoSelect, TipLanguageKey = LanguageKey.LK_SystemSetting_MakeToolAutoSelect_Tips)]
		public bool AutoSelectToolOnMake
		{
			get
			{
				return SystemSettingMapping.Settings.AutoSelectToolOnMake;
			}
			set
			{
				SystemSettingMapping.Settings.AutoSelectToolOnMake = value;
			}
		}

		// Token: 0x17000B0E RID: 2830
		// (get) Token: 0x06005C93 RID: 23699 RVA: 0x002ABD44 File Offset: 0x002A9F44
		// (set) Token: 0x06005C94 RID: 23700 RVA: 0x002ABDA4 File Offset: 0x002A9FA4
		[ToggleGroupSetting(ESettingSubCategory.Encyclopedia, 1, LanguageKey.LK_SystemSetting_EncyclopediaDisplayLevel, Options = new LanguageKey[]
		{
			LanguageKey.LK_Encyclopedia_LevelButton_Low,
			LanguageKey.LK_Encyclopedia_LevelButton_Mid,
			LanguageKey.LK_Encyclopedia_LevelButton_High
		}, TipLanguageKey = LanguageKey.LK_SystemSetting_EncyclopediaDisplayLevel_Tips)]
		public int EncyclopediaDisplayLevel
		{
			get
			{
				bool flag = Save.SaveData == null;
				if (flag)
				{
					Save.LoadInfo();
				}
				byte globalLabelStatus = Save.SaveData.GlobalLabelStatus;
				if (!true)
				{
				}
				int result;
				if (globalLabelStatus != 9)
				{
					if (globalLabelStatus != 11)
					{
						if (globalLabelStatus != 15)
						{
							result = 0;
						}
						else
						{
							result = 2;
						}
					}
					else
					{
						result = 1;
					}
				}
				else
				{
					result = 0;
				}
				if (!true)
				{
				}
				return result;
			}
			set
			{
				Save.EncyclopediaSaveData saveData = Save.SaveData;
				if (!true)
				{
				}
				byte globalLabelStatus;
				switch (value)
				{
				case 0:
					globalLabelStatus = 9;
					break;
				case 1:
					globalLabelStatus = 11;
					break;
				case 2:
					globalLabelStatus = 15;
					break;
				default:
					globalLabelStatus = 9;
					break;
				}
				if (!true)
				{
				}
				saveData.GlobalLabelStatus = globalLabelStatus;
			}
		}

		// Token: 0x17000B0F RID: 2831
		// (get) Token: 0x06005C95 RID: 23701 RVA: 0x002ABDEF File Offset: 0x002A9FEF
		// (set) Token: 0x06005C96 RID: 23702 RVA: 0x002ABDFB File Offset: 0x002A9FFB
		[BoolSetting(ESettingSubCategory.Tips, 1, LanguageKey.LK_SystemSetting_TipsAutoFixed, TipLanguageKey = LanguageKey.LK_HotKeyGroup_Tip_TipsAutoFixed, Key = ESettingKey.TipsAutoFixed)]
		public bool TipsAutoFixed
		{
			get
			{
				return SystemSettingMapping.Settings.TipsAutoFixed;
			}
			set
			{
				SystemSettingMapping.Settings.TipsAutoFixed = value;
			}
		}

		// Token: 0x17000B10 RID: 2832
		// (get) Token: 0x06005C97 RID: 23703 RVA: 0x002ABE09 File Offset: 0x002AA009
		// (set) Token: 0x06005C98 RID: 23704 RVA: 0x002ABE15 File Offset: 0x002AA015
		[FloatSliderSetting(ESettingSubCategory.Tips, 2, LanguageKey.LK_SystemSetting_TipsAutoFixedTime, ShowSliderLines = false, TipLanguageKey = LanguageKey.LK_HotKeyGroup_Tip_TipsAutoFixed_Time, Min = 0f, Max = 5f, Step = 0.05f, Format = "F2", DependsOn = ESettingKey.TipsAutoFixed)]
		public float TipsAutoFixedTime
		{
			get
			{
				return SystemSettingMapping.Settings.TipsAutoFixedTime;
			}
			set
			{
				SystemSettingMapping.Settings.TipsAutoFixedTime = value;
			}
		}

		// Token: 0x17000B11 RID: 2833
		// (get) Token: 0x06005C99 RID: 23705 RVA: 0x002ABE23 File Offset: 0x002AA023
		// (set) Token: 0x06005C9A RID: 23706 RVA: 0x002ABE2F File Offset: 0x002AA02F
		[FloatSliderSetting(ESettingSubCategory.Tips, 3, LanguageKey.LK_SystemSetting_TipsTriggerTime, ShowSliderLines = false, TipLanguageKey = LanguageKey.LK_HotKeyGroup_Tip_TipsTriggerTime, Min = 0f, Max = 1f, Step = 0.01f, Format = "F2")]
		public float TipsTriggerTime
		{
			get
			{
				return SystemSettingMapping.Settings.TipsTriggerTime;
			}
			set
			{
				SystemSettingMapping.Settings.TipsTriggerTime = value;
			}
		}

		// Token: 0x17000B12 RID: 2834
		// (get) Token: 0x06005C9B RID: 23707 RVA: 0x002ABE3D File Offset: 0x002AA03D
		// (set) Token: 0x06005C9C RID: 23708 RVA: 0x002ABE49 File Offset: 0x002AA049
		[IntSliderSetting(ESettingSubCategory.Tips, 4, LanguageKey.LK_SystemSetting_TipsTriggerSpeed, ShowSliderLines = false, TipLanguageKey = LanguageKey.LK_HotKeyGroup_Tip_TipsTriggerSpeed)]
		public sbyte TipsTriggerSpeed
		{
			get
			{
				return SystemSettingMapping.Settings.TipsTriggerSpeed;
			}
			set
			{
				SystemSettingMapping.Settings.TipsTriggerSpeed = value;
			}
		}

		// Token: 0x17000B13 RID: 2835
		// (get) Token: 0x06005C9D RID: 23709 RVA: 0x002ABE57 File Offset: 0x002AA057
		// (set) Token: 0x06005C9E RID: 23710 RVA: 0x002ABE63 File Offset: 0x002AA063
		[BoolSetting(ESettingSubCategory.Tips, 5, LanguageKey.LK_SystemSetting_TipsContinuousTrigger, TipLanguageKey = LanguageKey.LK_HotKeyGroup_Tip_TipsContinuousTrigger)]
		public bool TipsContinuousTrigger
		{
			get
			{
				return SystemSettingMapping.Settings.TipsContinuousTrigger;
			}
			set
			{
				SystemSettingMapping.Settings.TipsContinuousTrigger = value;
			}
		}

		// Token: 0x17000B14 RID: 2836
		// (get) Token: 0x06005C9F RID: 23711 RVA: 0x002ABE71 File Offset: 0x002AA071
		// (set) Token: 0x06005CA0 RID: 23712 RVA: 0x002ABE7D File Offset: 0x002AA07D
		[BoolSetting(ESettingSubCategory.MapExplore, 1, LanguageKey.LK_SystemSetting_EnableAutoTriggerNormalMapPickup, Key = ESettingKey.EnableAutoTriggerNormalMapPickup, TipLanguageKey = LanguageKey.LK_SystemSetting_EnableAutoTriggerNormalMapPickup_Tips)]
		public bool EnableAutoTriggerNormalMapPickup
		{
			get
			{
				return SystemSettingMapping.Settings.EnableAutoTriggerNormalMapPickup;
			}
			set
			{
				SystemSettingMapping.Settings.EnableAutoTriggerNormalMapPickup = value;
			}
		}

		// Token: 0x17000B15 RID: 2837
		// (get) Token: 0x06005CA1 RID: 23713 RVA: 0x002ABE8B File Offset: 0x002AA08B
		// (set) Token: 0x06005CA2 RID: 23714 RVA: 0x002ABE9A File Offset: 0x002AA09A
		[EnumSetting(ESettingSubCategory.MapExplore, 2, LanguageKey.LK_SystemSetting_AutoTriggerNormalMapPickup_Detail_Title_MinGrade, Options = new LanguageKey[]
		{
			LanguageKey.LK_SelectAll,
			LanguageKey.LK_Grade_0,
			LanguageKey.LK_Grade_1,
			LanguageKey.LK_Grade_2,
			LanguageKey.LK_Grade_3,
			LanguageKey.LK_Grade_4,
			LanguageKey.LK_Grade_5,
			LanguageKey.LK_Grade_6,
			LanguageKey.LK_Grade_7,
			LanguageKey.LK_Grade_8
		}, DependsOn = ESettingKey.EnableAutoTriggerNormalMapPickup, TipLanguageKey = LanguageKey.LK_SystemSetting_AutoTriggerNormalMapPickup_Detail_Title_MinGrade_Tips)]
		public sbyte AutoTriggerNormalPickupMinGrade
		{
			get
			{
				return SystemSettingMapping.Settings.AutoTriggerNormalPickupMinGrade + 1;
			}
			set
			{
				SystemSettingMapping.Settings.AutoTriggerNormalPickupMinGrade = value - 1;
			}
		}

		// Token: 0x17000B16 RID: 2838
		// (get) Token: 0x06005CA3 RID: 23715 RVA: 0x002ABEAA File Offset: 0x002AA0AA
		// (set) Token: 0x06005CA4 RID: 23716 RVA: 0x002ABEB6 File Offset: 0x002AA0B6
		[BoolSetting(ESettingSubCategory.MapExplore, 3, LanguageKey.LK_SystemSetting_AutoTriggerNormalMapPickup_Detail_Title_IncludeXiangshuMinion, DependsOn = ESettingKey.EnableAutoTriggerNormalMapPickup, TipLanguageKey = LanguageKey.LK_SystemSetting_AutoTriggerNormalMapPickup_Detail_Title_IncludeXiangshuMinion_Tips)]
		public bool AutoTriggerNormalPickupIncludeXiangshuMinion
		{
			get
			{
				return SystemSettingMapping.Settings.AutoTriggerNormalPickupIncludeXiangshuMinion;
			}
			set
			{
				SystemSettingMapping.Settings.AutoTriggerNormalPickupIncludeXiangshuMinion = value;
			}
		}

		// Token: 0x17000B17 RID: 2839
		// (get) Token: 0x06005CA5 RID: 23717 RVA: 0x002ABEC4 File Offset: 0x002AA0C4
		// (set) Token: 0x06005CA6 RID: 23718 RVA: 0x002ABED0 File Offset: 0x002AA0D0
		[MultiToggleGroupSetting(ESettingSubCategory.MapExplore, 4, LanguageKey.LK_SystemSetting_AutoTriggerNormalMapPickup_Detail_Title_PickupTypes, Options = new LanguageKey[]
		{
			LanguageKey.LK_MapPickupType2_0,
			LanguageKey.LK_MapPickupType2_1,
			LanguageKey.LK_MapPickupType2_2,
			LanguageKey.LK_MapPickupType2_3,
			LanguageKey.LK_MapPickupType2_4,
			LanguageKey.LK_MapPickupType2_5,
			LanguageKey.LK_MapPickupType2_6,
			LanguageKey.LK_MapPickupType2_7,
			LanguageKey.LK_MapPickupType2_8,
			LanguageKey.LK_MapPickupType2_9,
			LanguageKey.LK_MapPickupType2_10,
			LanguageKey.LK_MapPickupType2_11,
			LanguageKey.LK_MapPickupType2_12,
			LanguageKey.LK_MapPickupType2_13
		}, DependsOn = ESettingKey.EnableAutoTriggerNormalMapPickup, TipLanguageKey = LanguageKey.LK_SystemSetting_AutoTriggerNormalMapPickup_Detail_Title_PickupTypes_Tips)]
		public int AutoTriggerNormalPickupTypes
		{
			get
			{
				return SystemSettingMapping.Settings.AutoTriggerNormalPickupTypes;
			}
			set
			{
				SystemSettingMapping.Settings.AutoTriggerNormalPickupTypes = value;
			}
		}

		// Token: 0x17000B18 RID: 2840
		// (get) Token: 0x06005CA7 RID: 23719 RVA: 0x002ABEDD File Offset: 0x002AA0DD
		// (set) Token: 0x06005CA8 RID: 23720 RVA: 0x002ABEE9 File Offset: 0x002AA0E9
		[MultiToggleGroupSetting(ESettingSubCategory.MapExplore, 5, LanguageKey.LK_SystemSetting_AutoWipeOut, Options = new LanguageKey[]
		{
			LanguageKey.LK_SystemSetting_AutoWipeOut_Type2,
			LanguageKey.LK_SystemSetting_AutoWipeOut_Type0,
			LanguageKey.LK_SystemSetting_AutoWipeOut_Type1,
			LanguageKey.LK_SystemSetting_AutoWipeOut_Type3
		}, TipLanguageKey = LanguageKey.LK_SystemSetting_AutoWipeOut_Tip, ExtraTipLanguageKeys = new LanguageKey[]
		{
			LanguageKey.LK_SystemSetting_AutoWipeOut_Tip_Type2,
			LanguageKey.LK_SystemSetting_AutoWipeOut_Tip_Type0,
			LanguageKey.LK_SystemSetting_AutoWipeOut_Tip_Type1,
			LanguageKey.LK_SystemSetting_AutoWipeOut_Tip_Type3
		})]
		public int AutoWipeOut
		{
			get
			{
				return SystemSettingMapping.Settings.AutoWipeOut;
			}
			set
			{
				SystemSettingMapping.Settings.AutoWipeOut = value;
			}
		}

		// Token: 0x17000B19 RID: 2841
		// (get) Token: 0x06005CA9 RID: 23721 RVA: 0x002ABEF6 File Offset: 0x002AA0F6
		// (set) Token: 0x06005CAA RID: 23722 RVA: 0x002ABEF9 File Offset: 0x002AA0F9
		[BoolSetting(ESettingSubCategory.RegionStory, 1, LanguageKey.LK_SystemSetting_Story_Shaolin, Key = ESettingKey.SectStory, TipLanguageKey = LanguageKey.LK_SystemSetting_SectStoryToggle_Tips)]
		public bool StoryShaolin
		{
			get
			{
				return true;
			}
			set
			{
				this.SetSectMainStoryActiveStatus(1, value);
			}
		}

		// Token: 0x17000B1A RID: 2842
		// (get) Token: 0x06005CAB RID: 23723 RVA: 0x002ABF04 File Offset: 0x002AA104
		// (set) Token: 0x06005CAC RID: 23724 RVA: 0x002ABF07 File Offset: 0x002AA107
		[BoolSetting(ESettingSubCategory.RegionStory, 2, LanguageKey.LK_SystemSetting_Story_Emei, Key = ESettingKey.SectStory, TipLanguageKey = LanguageKey.LK_SystemSetting_SectStoryToggle_Tips)]
		public bool StoryEmei
		{
			get
			{
				return true;
			}
			set
			{
				this.SetSectMainStoryActiveStatus(2, value);
			}
		}

		// Token: 0x17000B1B RID: 2843
		// (get) Token: 0x06005CAD RID: 23725 RVA: 0x002ABF12 File Offset: 0x002AA112
		// (set) Token: 0x06005CAE RID: 23726 RVA: 0x002ABF15 File Offset: 0x002AA115
		[BoolSetting(ESettingSubCategory.RegionStory, 3, LanguageKey.LK_SystemSetting_Story_Baihua, Key = ESettingKey.SectStory, TipLanguageKey = LanguageKey.LK_SystemSetting_SectStoryToggle_Tips)]
		public bool StoryBaihua
		{
			get
			{
				return true;
			}
			set
			{
				this.SetSectMainStoryActiveStatus(3, value);
			}
		}

		// Token: 0x17000B1C RID: 2844
		// (get) Token: 0x06005CAF RID: 23727 RVA: 0x002ABF20 File Offset: 0x002AA120
		// (set) Token: 0x06005CB0 RID: 23728 RVA: 0x002ABF23 File Offset: 0x002AA123
		[BoolSetting(ESettingSubCategory.RegionStory, 4, LanguageKey.LK_SystemSetting_Story_Wudang, Key = ESettingKey.SectStory, TipLanguageKey = LanguageKey.LK_SystemSetting_SectStoryToggle_Tips)]
		public bool StoryWudang
		{
			get
			{
				return true;
			}
			set
			{
				this.SetSectMainStoryActiveStatus(4, value);
			}
		}

		// Token: 0x17000B1D RID: 2845
		// (get) Token: 0x06005CB1 RID: 23729 RVA: 0x002ABF2E File Offset: 0x002AA12E
		// (set) Token: 0x06005CB2 RID: 23730 RVA: 0x002ABF31 File Offset: 0x002AA131
		[BoolSetting(ESettingSubCategory.RegionStory, 5, LanguageKey.LK_SystemSetting_Story_Yuanshan, Key = ESettingKey.SectStory, TipLanguageKey = LanguageKey.LK_SystemSetting_SectStoryToggle_Tips)]
		public bool StoryYuanshan
		{
			get
			{
				return true;
			}
			set
			{
				this.SetSectMainStoryActiveStatus(5, value);
			}
		}

		// Token: 0x17000B1E RID: 2846
		// (get) Token: 0x06005CB3 RID: 23731 RVA: 0x002ABF3C File Offset: 0x002AA13C
		// (set) Token: 0x06005CB4 RID: 23732 RVA: 0x002ABF3F File Offset: 0x002AA13F
		[BoolSetting(ESettingSubCategory.RegionStory, 6, LanguageKey.LK_SystemSetting_Story_Shixiang, Key = ESettingKey.SectStory, TipLanguageKey = LanguageKey.LK_SystemSetting_SectStoryToggle_Tips)]
		public bool StoryShixiang
		{
			get
			{
				return true;
			}
			set
			{
				this.SetSectMainStoryActiveStatus(6, value);
			}
		}

		// Token: 0x17000B1F RID: 2847
		// (get) Token: 0x06005CB5 RID: 23733 RVA: 0x002ABF4A File Offset: 0x002AA14A
		// (set) Token: 0x06005CB6 RID: 23734 RVA: 0x002ABF4D File Offset: 0x002AA14D
		[BoolSetting(ESettingSubCategory.RegionStory, 7, LanguageKey.LK_SystemSetting_Story_Ranshan, Key = ESettingKey.SectStory, TipLanguageKey = LanguageKey.LK_SystemSetting_SectStoryToggle_Tips)]
		public bool StoryRanshan
		{
			get
			{
				return true;
			}
			set
			{
				this.SetSectMainStoryActiveStatus(7, value);
			}
		}

		// Token: 0x17000B20 RID: 2848
		// (get) Token: 0x06005CB7 RID: 23735 RVA: 0x002ABF58 File Offset: 0x002AA158
		// (set) Token: 0x06005CB8 RID: 23736 RVA: 0x002ABF5B File Offset: 0x002AA15B
		[BoolSetting(ESettingSubCategory.RegionStory, 8, LanguageKey.LK_SystemSetting_Story_Xuannv, Key = ESettingKey.SectStory, TipLanguageKey = LanguageKey.LK_SystemSetting_SectStoryToggle_Tips)]
		public bool StoryXuannv
		{
			get
			{
				return true;
			}
			set
			{
				this.SetSectMainStoryActiveStatus(8, value);
			}
		}

		// Token: 0x17000B21 RID: 2849
		// (get) Token: 0x06005CB9 RID: 23737 RVA: 0x002ABF66 File Offset: 0x002AA166
		// (set) Token: 0x06005CBA RID: 23738 RVA: 0x002ABF69 File Offset: 0x002AA169
		[BoolSetting(ESettingSubCategory.RegionStory, 9, LanguageKey.LK_SystemSetting_Story_Zhujian, Key = ESettingKey.SectStory, TipLanguageKey = LanguageKey.LK_SystemSetting_SectStoryToggle_Tips)]
		public bool StoryZhujian
		{
			get
			{
				return true;
			}
			set
			{
				this.SetSectMainStoryActiveStatus(9, value);
			}
		}

		// Token: 0x17000B22 RID: 2850
		// (get) Token: 0x06005CBB RID: 23739 RVA: 0x002ABF75 File Offset: 0x002AA175
		// (set) Token: 0x06005CBC RID: 23740 RVA: 0x002ABF78 File Offset: 0x002AA178
		[BoolSetting(ESettingSubCategory.RegionStory, 10, LanguageKey.LK_SystemSetting_Story_Kongsang, Key = ESettingKey.SectStory, TipLanguageKey = LanguageKey.LK_SystemSetting_SectStoryToggle_Tips)]
		public bool StoryKongsang
		{
			get
			{
				return true;
			}
			set
			{
				this.SetSectMainStoryActiveStatus(10, value);
			}
		}

		// Token: 0x17000B23 RID: 2851
		// (get) Token: 0x06005CBD RID: 23741 RVA: 0x002ABF84 File Offset: 0x002AA184
		// (set) Token: 0x06005CBE RID: 23742 RVA: 0x002ABF87 File Offset: 0x002AA187
		[BoolSetting(ESettingSubCategory.RegionStory, 11, LanguageKey.LK_SystemSetting_Story_Jingang, Key = ESettingKey.SectStory, TipLanguageKey = LanguageKey.LK_SystemSetting_SectStoryToggle_Tips)]
		public bool StoryJingang
		{
			get
			{
				return true;
			}
			set
			{
				this.SetSectMainStoryActiveStatus(11, value);
			}
		}

		// Token: 0x17000B24 RID: 2852
		// (get) Token: 0x06005CBF RID: 23743 RVA: 0x002ABF93 File Offset: 0x002AA193
		// (set) Token: 0x06005CC0 RID: 23744 RVA: 0x002ABF96 File Offset: 0x002AA196
		[BoolSetting(ESettingSubCategory.RegionStory, 12, LanguageKey.LK_SystemSetting_Story_Wuxian, Key = ESettingKey.SectStory, TipLanguageKey = LanguageKey.LK_SystemSetting_SectStoryToggle_Tips)]
		public bool StoryWuxian
		{
			get
			{
				return true;
			}
			set
			{
				this.SetSectMainStoryActiveStatus(12, value);
			}
		}

		// Token: 0x17000B25 RID: 2853
		// (get) Token: 0x06005CC1 RID: 23745 RVA: 0x002ABFA2 File Offset: 0x002AA1A2
		// (set) Token: 0x06005CC2 RID: 23746 RVA: 0x002ABFA5 File Offset: 0x002AA1A5
		[BoolSetting(ESettingSubCategory.RegionStory, 13, LanguageKey.LK_SystemSetting_Story_Jieqing, Key = ESettingKey.SectStory, TipLanguageKey = LanguageKey.LK_SystemSetting_SectStoryToggle_Tips)]
		public bool StoryJieqing
		{
			get
			{
				return true;
			}
			set
			{
				this.SetSectMainStoryActiveStatus(13, value);
			}
		}

		// Token: 0x17000B26 RID: 2854
		// (get) Token: 0x06005CC3 RID: 23747 RVA: 0x002ABFB1 File Offset: 0x002AA1B1
		// (set) Token: 0x06005CC4 RID: 23748 RVA: 0x002ABFB4 File Offset: 0x002AA1B4
		[BoolSetting(ESettingSubCategory.RegionStory, 14, LanguageKey.LK_SystemSetting_Story_Fulong, Key = ESettingKey.SectStory, TipLanguageKey = LanguageKey.LK_SystemSetting_SectStoryToggle_Tips)]
		public bool StoryFulong
		{
			get
			{
				return true;
			}
			set
			{
				this.SetSectMainStoryActiveStatus(14, value);
			}
		}

		// Token: 0x17000B27 RID: 2855
		// (get) Token: 0x06005CC5 RID: 23749 RVA: 0x002ABFC0 File Offset: 0x002AA1C0
		// (set) Token: 0x06005CC6 RID: 23750 RVA: 0x002ABFC3 File Offset: 0x002AA1C3
		[BoolSetting(ESettingSubCategory.RegionStory, 15, LanguageKey.LK_SystemSetting_Story_Xuehou, Key = ESettingKey.SectStory, TipLanguageKey = LanguageKey.LK_SystemSetting_SectStoryToggle_Tips)]
		public bool StoryXuehou
		{
			get
			{
				return true;
			}
			set
			{
				this.SetSectMainStoryActiveStatus(15, value);
			}
		}

		// Token: 0x06005CC7 RID: 23751 RVA: 0x002ABFD0 File Offset: 0x002AA1D0
		private void SetSectMainStoryActiveStatus(sbyte orgTemplateId, bool isActive)
		{
			bool flag = GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame;
			if (flag)
			{
				StoryDomainMethod.Call.SetSectMainStoryActiveStatus(orgTemplateId, !isActive);
			}
		}

		// Token: 0x17000B28 RID: 2856
		// (get) Token: 0x06005CC8 RID: 23752 RVA: 0x002ABFFC File Offset: 0x002AA1FC
		// (set) Token: 0x06005CC9 RID: 23753 RVA: 0x002AC008 File Offset: 0x002AA208
		[ResolutionSetting(ESettingSubCategory.Video, 1, LanguageKey.LK_SystemSetting_Resolution)]
		public Vector2Int Resolution
		{
			get
			{
				return SystemSettingMapping.Settings.Resolution;
			}
			set
			{
				SystemSettingMapping.Settings.Resolution = value;
			}
		}

		// Token: 0x17000B29 RID: 2857
		// (get) Token: 0x06005CCA RID: 23754 RVA: 0x002AC016 File Offset: 0x002AA216
		// (set) Token: 0x06005CCB RID: 23755 RVA: 0x002AC022 File Offset: 0x002AA222
		[BoolSetting(ESettingSubCategory.Video, 2, LanguageKey.LK_FullScreen, TipType = TipType.Simple)]
		public bool FullScreen
		{
			get
			{
				return SystemSettingMapping.Settings.FullScreen;
			}
			set
			{
				SystemSettingMapping.Settings.FullScreen = value;
			}
		}

		// Token: 0x17000B2A RID: 2858
		// (get) Token: 0x06005CCC RID: 23756 RVA: 0x002AC030 File Offset: 0x002AA230
		// (set) Token: 0x06005CCD RID: 23757 RVA: 0x002AC03C File Offset: 0x002AA23C
		[BoolSetting(ESettingSubCategory.Video, 3, LanguageKey.LK_VSync, TipType = TipType.Simple)]
		public bool VSync
		{
			get
			{
				return SystemSettingMapping.Settings.VSync;
			}
			set
			{
				SystemSettingMapping.Settings.VSync = value;
			}
		}

		// Token: 0x17000B2B RID: 2859
		// (get) Token: 0x06005CCE RID: 23758 RVA: 0x002AC04A File Offset: 0x002AA24A
		// (set) Token: 0x06005CCF RID: 23759 RVA: 0x002AC056 File Offset: 0x002AA256
		[BoolSetting(ESettingSubCategory.Audio, 1, LanguageKey.LK_Bgm, Key = ESettingKey.BgmOn)]
		public bool BgmOn
		{
			get
			{
				return SystemSettingMapping.Settings.BgmOn;
			}
			set
			{
				SystemSettingMapping.Settings.BgmOn = value;
			}
		}

		// Token: 0x17000B2C RID: 2860
		// (get) Token: 0x06005CD0 RID: 23760 RVA: 0x002AC064 File Offset: 0x002AA264
		// (set) Token: 0x06005CD1 RID: 23761 RVA: 0x002AC070 File Offset: 0x002AA270
		[IntSliderSetting(ESettingSubCategory.Audio, 2, LanguageKey.LK_BgmVolume, Min = 0, Max = 100, Step = 10, DependsOn = ESettingKey.BgmOn)]
		public sbyte BgmVolume
		{
			get
			{
				return SystemSettingMapping.Settings.BgmVolume;
			}
			set
			{
				SystemSettingMapping.Settings.BgmVolume = value;
			}
		}

		// Token: 0x17000B2D RID: 2861
		// (get) Token: 0x06005CD2 RID: 23762 RVA: 0x002AC07E File Offset: 0x002AA27E
		// (set) Token: 0x06005CD3 RID: 23763 RVA: 0x002AC08A File Offset: 0x002AA28A
		[BoolSetting(ESettingSubCategory.Audio, 3, LanguageKey.LK_SoundEffect, Key = ESettingKey.SeOn)]
		public bool SeOn
		{
			get
			{
				return SystemSettingMapping.Settings.SeOn;
			}
			set
			{
				SystemSettingMapping.Settings.SeOn = value;
			}
		}

		// Token: 0x17000B2E RID: 2862
		// (get) Token: 0x06005CD4 RID: 23764 RVA: 0x002AC098 File Offset: 0x002AA298
		// (set) Token: 0x06005CD5 RID: 23765 RVA: 0x002AC0A4 File Offset: 0x002AA2A4
		[IntSliderSetting(ESettingSubCategory.Audio, 4, LanguageKey.LK_SoundEffectVolume, Min = 0, Max = 100, Step = 10, DependsOn = ESettingKey.SeOn)]
		public sbyte SeVolume
		{
			get
			{
				return SystemSettingMapping.Settings.SeVolume;
			}
			set
			{
				SystemSettingMapping.Settings.SeVolume = value;
			}
		}

		// Token: 0x17000B2F RID: 2863
		// (get) Token: 0x06005CD6 RID: 23766 RVA: 0x002AC0B2 File Offset: 0x002AA2B2
		// (set) Token: 0x06005CD7 RID: 23767 RVA: 0x002AC0BE File Offset: 0x002AA2BE
		[IntSliderSetting(ESettingSubCategory.Audio, 5, LanguageKey.LK_VideoVolume, Min = 0, Max = 100, Step = 10)]
		public sbyte VideoVolume
		{
			get
			{
				return SystemSettingMapping.Settings.VideoVolume;
			}
			set
			{
				SystemSettingMapping.Settings.VideoVolume = value;
			}
		}

		// Token: 0x17000B30 RID: 2864
		// (get) Token: 0x06005CD8 RID: 23768 RVA: 0x002AC0CC File Offset: 0x002AA2CC
		// (set) Token: 0x06005CD9 RID: 23769 RVA: 0x002AC0D8 File Offset: 0x002AA2D8
		[BoolSetting(ESettingSubCategory.Audio, 6, LanguageKey.LK_SystemSetting_Audio_MuteIfNotFocus, TipLanguageKey = LanguageKey.LK_SystemSetting_Audio_MuteIfNotFocus_Tips)]
		public bool MuteIfNotFocus
		{
			get
			{
				return SystemSettingMapping.Settings.MuteIfNotFocus;
			}
			set
			{
				SystemSettingMapping.Settings.MuteIfNotFocus = value;
			}
		}

		// Token: 0x17000B31 RID: 2865
		// (get) Token: 0x06005CDA RID: 23770 RVA: 0x002AC0E6 File Offset: 0x002AA2E6
		// (set) Token: 0x06005CDB RID: 23771 RVA: 0x002AC0F2 File Offset: 0x002AA2F2
		[BoolSetting(ESettingSubCategory.Function, 1, LanguageKey.LK_Auto_Clear_Defend_In_Block_Attack_Skill, TipLanguageKey = LanguageKey.LK_Auto_Clear_Defend_In_Block_Attack_Skill_Tips)]
		public bool AutoClearDefendInBlockAttackSkill
		{
			get
			{
				return SystemSettingMapping.Settings.AutoClearDefendInBlockAttackSkill;
			}
			set
			{
				SystemSettingMapping.Settings.AutoClearDefendInBlockAttackSkill = value;
			}
		}

		// Token: 0x17000B32 RID: 2866
		// (get) Token: 0x06005CDC RID: 23772 RVA: 0x002AC100 File Offset: 0x002AA300
		// (set) Token: 0x06005CDD RID: 23773 RVA: 0x002AC10C File Offset: 0x002AA30C
		[BoolSetting(ESettingSubCategory.Function, 2, LanguageKey.LK_Auto_Allocate_Neili_To_Max_Option, TipLanguageKey = LanguageKey.LK_Auto_Allocate_Neili_To_Max_Option_Tips)]
		public bool AutoAllocateNeiliToMax
		{
			get
			{
				return SystemSettingMapping.Settings.AutoAllocateNeiliToMax;
			}
			set
			{
				SystemSettingMapping.Settings.AutoAllocateNeiliToMax = value;
			}
		}

		// Token: 0x17000B33 RID: 2867
		// (get) Token: 0x06005CDE RID: 23774 RVA: 0x002AC11A File Offset: 0x002AA31A
		// (set) Token: 0x06005CDF RID: 23775 RVA: 0x002AC126 File Offset: 0x002AA326
		[BoolSetting(ESettingSubCategory.Function, 3, LanguageKey.LK_Auto_Pause_In_Cast_Skill, TipLanguageKey = LanguageKey.LK_Auto_Pause_In_Cast_Skill_Tips)]
		public bool AutoPauseInCastSkill
		{
			get
			{
				return SystemSettingMapping.Settings.AutoPauseInCastSkill;
			}
			set
			{
				SystemSettingMapping.Settings.AutoPauseInCastSkill = value;
			}
		}

		// Token: 0x17000B34 RID: 2868
		// (get) Token: 0x06005CE0 RID: 23776 RVA: 0x002AC134 File Offset: 0x002AA334
		// (set) Token: 0x06005CE1 RID: 23777 RVA: 0x002AC140 File Offset: 0x002AA340
		[BoolSetting(ESettingSubCategory.Function, 4, LanguageKey.LK_Auto_Pause_In_Ally_Cast_Skill_Option, TipLanguageKey = LanguageKey.LK_Auto_Pause_In_Ally_Cast_Skill_Option_Tips)]
		public bool AutoPauseInAllyCastSkill
		{
			get
			{
				return SystemSettingMapping.Settings.AutoPauseInAllyCastSkill;
			}
			set
			{
				SystemSettingMapping.Settings.AutoPauseInAllyCastSkill = value;
			}
		}

		// Token: 0x17000B35 RID: 2869
		// (get) Token: 0x06005CE2 RID: 23778 RVA: 0x002AC14E File Offset: 0x002AA34E
		// (set) Token: 0x06005CE3 RID: 23779 RVA: 0x002AC15A File Offset: 0x002AA35A
		[BoolSetting(ESettingSubCategory.Display, 1, LanguageKey.LK_Combat_Tutorial_Option, TipLanguageKey = LanguageKey.LK_Combat_Tutorial_Option_Tips)]
		public bool ShowCombatTutorial
		{
			get
			{
				return SystemSettingMapping.Settings.ShowCombatTutorial;
			}
			set
			{
				SystemSettingMapping.Settings.ShowCombatTutorial = value;
			}
		}

		// Token: 0x17000B36 RID: 2870
		// (get) Token: 0x06005CE4 RID: 23780 RVA: 0x002AC168 File Offset: 0x002AA368
		// (set) Token: 0x06005CE5 RID: 23781 RVA: 0x002AC174 File Offset: 0x002AA374
		[BoolSetting(ESettingSubCategory.Display, 2, LanguageKey.LK_Abbreviated_Information, TipLanguageKey = LanguageKey.LK_Abbreviated_Information_Tips)]
		public bool AbbreviatedInformation
		{
			get
			{
				return SystemSettingMapping.Settings.AbbreviatedInformation;
			}
			set
			{
				SystemSettingMapping.Settings.AbbreviatedInformation = value;
			}
		}

		// Token: 0x17000B37 RID: 2871
		// (get) Token: 0x06005CE6 RID: 23782 RVA: 0x002AC182 File Offset: 0x002AA382
		// (set) Token: 0x06005CE7 RID: 23783 RVA: 0x002AC18E File Offset: 0x002AA38E
		[BoolSetting(ESettingSubCategory.Display, 3, LanguageKey.LK_Distance_AttackRange_Option, TipLanguageKey = LanguageKey.LK_Distance_AttackRange_Option_Tips)]
		public bool DistanceAttackRange
		{
			get
			{
				return SystemSettingMapping.Settings.DistanceAttackRange;
			}
			set
			{
				SystemSettingMapping.Settings.DistanceAttackRange = value;
			}
		}

		// Token: 0x17000B38 RID: 2872
		// (get) Token: 0x06005CE8 RID: 23784 RVA: 0x002AC19C File Offset: 0x002AA39C
		// (set) Token: 0x06005CE9 RID: 23785 RVA: 0x002AC1A8 File Offset: 0x002AA3A8
		[BoolSetting(ESettingSubCategory.Display, 4, LanguageKey.LK_Combat_Damage_Num_Option, TipLanguageKey = LanguageKey.LK_Combat_Damage_Num_Option_Tips)]
		public bool ShowDamageNumber
		{
			get
			{
				return SystemSettingMapping.Settings.ShowDamageNumber;
			}
			set
			{
				SystemSettingMapping.Settings.ShowDamageNumber = value;
			}
		}

		// Token: 0x17000B39 RID: 2873
		// (get) Token: 0x06005CEA RID: 23786 RVA: 0x002AC1B6 File Offset: 0x002AA3B6
		// (set) Token: 0x06005CEB RID: 23787 RVA: 0x002AC1C2 File Offset: 0x002AA3C2
		[BoolSetting(ESettingSubCategory.Display, 5, LanguageKey.LK_Combat_Shake_Option, TipLanguageKey = LanguageKey.LK_Combat_Shake_Option_Tips)]
		public bool CombatShake
		{
			get
			{
				return SystemSettingMapping.Settings.CombatShake;
			}
			set
			{
				SystemSettingMapping.Settings.CombatShake = value;
			}
		}

		// Token: 0x17000B3A RID: 2874
		// (get) Token: 0x06005CEC RID: 23788 RVA: 0x002AC1D0 File Offset: 0x002AA3D0
		// (set) Token: 0x06005CED RID: 23789 RVA: 0x002AC1DC File Offset: 0x002AA3DC
		[BoolSetting(ESettingSubCategory.Display, 6, LanguageKey.LK_Execute_Option, TipLanguageKey = LanguageKey.LK_Execute_Option_Tips)]
		public bool AllowExecute
		{
			get
			{
				return SystemSettingMapping.Settings.AllowExecute;
			}
			set
			{
				SystemSettingMapping.Settings.AllowExecute = value;
			}
		}

		// Token: 0x17000B3B RID: 2875
		// (get) Token: 0x06005CEE RID: 23790 RVA: 0x002AC1EA File Offset: 0x002AA3EA
		// (set) Token: 0x06005CEF RID: 23791 RVA: 0x002AC1F6 File Offset: 0x002AA3F6
		[BoolSetting(ESettingSubCategory.Display, 7, LanguageKey.LK_CombatPure_Information, TipLanguageKey = LanguageKey.LK_CombatPure_Information_Tips)]
		public bool CombatPure
		{
			get
			{
				return SystemSettingMapping.Settings.CombatPure;
			}
			set
			{
				SystemSettingMapping.Settings.CombatPure = value;
			}
		}

		// Token: 0x17000B3C RID: 2876
		// (get) Token: 0x06005CF0 RID: 23792 RVA: 0x002AC204 File Offset: 0x002AA404
		// (set) Token: 0x06005CF1 RID: 23793 RVA: 0x002AC218 File Offset: 0x002AA418
		[BoolSetting(ESettingSubCategory.AutoAttack, 1, LanguageKey.LK_SystemSetting_AutoAttack, Key = ESettingKey.AutoAttack, TipLanguageKey = LanguageKey.LK_Combat_Ai_Auto_Attack_Tips)]
		public bool AutoAttack
		{
			get
			{
				AiOptions aiOptionsRef = SystemSettingMapping.AiOptionsRef;
				return aiOptionsRef != null && aiOptionsRef.AutoAttack;
			}
			set
			{
				bool flag = SystemSettingMapping.AiOptionsRef != null;
				if (flag)
				{
					SystemSettingMapping.AiOptionsRef.AutoAttack = value;
				}
			}
		}

		// Token: 0x17000B3D RID: 2877
		// (get) Token: 0x06005CF2 RID: 23794 RVA: 0x002AC23D File Offset: 0x002AA43D
		// (set) Token: 0x06005CF3 RID: 23795 RVA: 0x002AC250 File Offset: 0x002AA450
		[BoolSetting(ESettingSubCategory.AutoAttack, 2, LanguageKey.LK_SystemSetting_AutoChangeWeapon, DependsOn = ESettingKey.AutoAttack, TipLanguageKey = LanguageKey.LK_SystemSetting_AutoChangeWeapon_Tips)]
		public bool AutoChangeWeapon
		{
			get
			{
				AiOptions aiOptionsRef = SystemSettingMapping.AiOptionsRef;
				return aiOptionsRef != null && aiOptionsRef.AutoChangeWeapon;
			}
			set
			{
				bool flag = SystemSettingMapping.AiOptionsRef != null;
				if (flag)
				{
					SystemSettingMapping.AiOptionsRef.AutoChangeWeapon = value;
				}
			}
		}

		// Token: 0x17000B3E RID: 2878
		// (get) Token: 0x06005CF4 RID: 23796 RVA: 0x002AC275 File Offset: 0x002AA475
		// (set) Token: 0x06005CF5 RID: 23797 RVA: 0x002AC288 File Offset: 0x002AA488
		[BoolSetting(ESettingSubCategory.AutoAttack, 3, LanguageKey.LK_SystemSetting_AutoChangeTrick, DependsOn = ESettingKey.AutoAttack, TipLanguageKey = LanguageKey.LK_SystemSetting_AutoChangeTrick_Tips)]
		public bool AutoChangeTrick
		{
			get
			{
				AiOptions aiOptionsRef = SystemSettingMapping.AiOptionsRef;
				return aiOptionsRef != null && aiOptionsRef.AutoChangeTrick;
			}
			set
			{
				bool flag = SystemSettingMapping.AiOptionsRef != null;
				if (flag)
				{
					SystemSettingMapping.AiOptionsRef.AutoChangeTrick = value;
				}
			}
		}

		// Token: 0x17000B3F RID: 2879
		// (get) Token: 0x06005CF6 RID: 23798 RVA: 0x002AC2AD File Offset: 0x002AA4AD
		// (set) Token: 0x06005CF7 RID: 23799 RVA: 0x002AC2C0 File Offset: 0x002AA4C0
		[BoolSetting(ESettingSubCategory.AutoAttack, 4, LanguageKey.LK_SystemSetting_AutoUnlock, TipLanguageKey = LanguageKey.LK_SystemSetting_AutoUnlock_Tips, Key = ESettingKey.AutoUnlock)]
		public bool AutoUnlock
		{
			get
			{
				AiOptions aiOptionsRef = SystemSettingMapping.AiOptionsRef;
				return aiOptionsRef != null && aiOptionsRef.AutoUnlock;
			}
			set
			{
				bool flag = SystemSettingMapping.AiOptionsRef != null;
				if (flag)
				{
					SystemSettingMapping.AiOptionsRef.AutoUnlock = value;
				}
			}
		}

		// Token: 0x17000B40 RID: 2880
		// (get) Token: 0x06005CF8 RID: 23800 RVA: 0x002AC2E5 File Offset: 0x002AA4E5
		// (set) Token: 0x06005CF9 RID: 23801 RVA: 0x002AC2F8 File Offset: 0x002AA4F8
		[BoolSetting(ESettingSubCategory.AutoAttack, 5, LanguageKey.LK_SystemSetting_SkipRawCreate, DependsOn = ESettingKey.AutoUnlock, TipLanguageKey = LanguageKey.LK_SystemSetting_SkipRawCreate_Tips)]
		public bool SkipRawCreate
		{
			get
			{
				AiOptions aiOptionsRef = SystemSettingMapping.AiOptionsRef;
				return aiOptionsRef != null && aiOptionsRef.SkipRawCreate;
			}
			set
			{
				bool flag = SystemSettingMapping.AiOptionsRef != null;
				if (flag)
				{
					SystemSettingMapping.AiOptionsRef.SkipRawCreate = value;
				}
			}
		}

		// Token: 0x17000B41 RID: 2881
		// (get) Token: 0x06005CFA RID: 23802 RVA: 0x002AC31D File Offset: 0x002AA51D
		// (set) Token: 0x06005CFB RID: 23803 RVA: 0x002AC330 File Offset: 0x002AA530
		[BoolSetting(ESettingSubCategory.AutoMove, 1, LanguageKey.LK_SystemSetting_AutoMove, Key = ESettingKey.AutoMove, TipLanguageKey = LanguageKey.LK_SystemSetting_AutoMove_Tips)]
		public bool AutoMove
		{
			get
			{
				AiOptions aiOptionsRef = SystemSettingMapping.AiOptionsRef;
				return aiOptionsRef != null && aiOptionsRef.AutoMove;
			}
			set
			{
				bool flag = SystemSettingMapping.AiOptionsRef != null;
				if (flag)
				{
					SystemSettingMapping.AiOptionsRef.AutoMove = value;
				}
			}
		}

		// Token: 0x17000B42 RID: 2882
		// (get) Token: 0x06005CFC RID: 23804 RVA: 0x002AC355 File Offset: 0x002AA555
		// (set) Token: 0x06005CFD RID: 23805 RVA: 0x002AC368 File Offset: 0x002AA568
		[BoolSetting(ESettingSubCategory.AutoMove, 2, LanguageKey.LK_SystemSetting_TryDodge, DependsOn = ESettingKey.AutoMove, TipLanguageKey = LanguageKey.LK_SystemSetting_TryDodge_Tips)]
		public bool TryDodge
		{
			get
			{
				AiOptions aiOptionsRef = SystemSettingMapping.AiOptionsRef;
				return aiOptionsRef != null && aiOptionsRef.TryDodge;
			}
			set
			{
				bool flag = SystemSettingMapping.AiOptionsRef != null;
				if (flag)
				{
					SystemSettingMapping.AiOptionsRef.TryDodge = value;
				}
			}
		}

		// Token: 0x17000B43 RID: 2883
		// (get) Token: 0x06005CFE RID: 23806 RVA: 0x002AC38D File Offset: 0x002AA58D
		// (set) Token: 0x06005CFF RID: 23807 RVA: 0x002AC3A0 File Offset: 0x002AA5A0
		[BoolSetting(ESettingSubCategory.AutoMove, 3, LanguageKey.LK_SystemSetting_SaveMoveTarget, DependsOn = ESettingKey.AutoMove, Key = ESettingKey.SaveMoveTarget, TipLanguageKey = LanguageKey.LK_SystemSetting_SaveMoveTarget_Tips)]
		public bool SaveMoveTarget
		{
			get
			{
				AiOptions aiOptionsRef = SystemSettingMapping.AiOptionsRef;
				return aiOptionsRef != null && aiOptionsRef.SaveMoveTarget;
			}
			set
			{
				bool flag = SystemSettingMapping.AiOptionsRef != null;
				if (flag)
				{
					SystemSettingMapping.AiOptionsRef.SaveMoveTarget = value;
				}
			}
		}

		// Token: 0x17000B44 RID: 2884
		// (get) Token: 0x06005D00 RID: 23808 RVA: 0x002AC3C5 File Offset: 0x002AA5C5
		// (set) Token: 0x06005D01 RID: 23809 RVA: 0x002AC3F8 File Offset: 0x002AA5F8
		[BoolSetting(ESettingSubCategory.AutoAction, 1, LanguageKey.LK_SystemSetting_Heal_AutoInjury, Key = ESettingKey.AutoHealInjury, TipLanguageKey = LanguageKey.LK_SystemSetting_Heal_AutoInjury_Tips)]
		public bool AutoHealInjury
		{
			get
			{
				return SystemSettingMapping.AiOptionsRef != null && SystemSettingMapping.AiOptionsRef.AutoUseOtherAction != null && SystemSettingMapping.AiOptionsRef.AutoUseOtherAction.Length != 0 && SystemSettingMapping.AiOptionsRef.AutoUseOtherAction[0];
			}
			set
			{
				AiOptions aiOptionsRef = SystemSettingMapping.AiOptionsRef;
				bool flag = ((aiOptionsRef != null) ? aiOptionsRef.AutoUseOtherAction : null) != null && SystemSettingMapping.AiOptionsRef.AutoUseOtherAction.Length != 0;
				if (flag)
				{
					SystemSettingMapping.AiOptionsRef.AutoUseOtherAction[0] = value;
				}
			}
		}

		// Token: 0x17000B45 RID: 2885
		// (get) Token: 0x06005D02 RID: 23810 RVA: 0x002AC43B File Offset: 0x002AA63B
		// (set) Token: 0x06005D03 RID: 23811 RVA: 0x002AC470 File Offset: 0x002AA670
		[BoolSetting(ESettingSubCategory.AutoAction, 2, LanguageKey.LK_SystemSetting_Heal_AutoPoison, Key = ESettingKey.AutoHealPoison, TipLanguageKey = LanguageKey.LK_SystemSetting_Heal_AutoPoison_Tips)]
		public bool AutoHealPoison
		{
			get
			{
				return SystemSettingMapping.AiOptionsRef != null && SystemSettingMapping.AiOptionsRef.AutoUseOtherAction != null && SystemSettingMapping.AiOptionsRef.AutoUseOtherAction.Length > 1 && SystemSettingMapping.AiOptionsRef.AutoUseOtherAction[1];
			}
			set
			{
				AiOptions aiOptionsRef = SystemSettingMapping.AiOptionsRef;
				bool flag = ((aiOptionsRef != null) ? aiOptionsRef.AutoUseOtherAction : null) != null && SystemSettingMapping.AiOptionsRef.AutoUseOtherAction.Length > 1;
				if (flag)
				{
					SystemSettingMapping.AiOptionsRef.AutoUseOtherAction[1] = value;
				}
			}
		}

		// Token: 0x17000B46 RID: 2886
		// (get) Token: 0x06005D04 RID: 23812 RVA: 0x002AC4B4 File Offset: 0x002AA6B4
		// (set) Token: 0x06005D05 RID: 23813 RVA: 0x002AC4E8 File Offset: 0x002AA6E8
		[BoolSetting(ESettingSubCategory.AutoAction, 3, LanguageKey.LK_SystemSetting_AutoFlee, Key = ESettingKey.AutoFlee, TipLanguageKey = LanguageKey.LK_SystemSetting_AutoFlee_Tips)]
		public bool AutoFlee
		{
			get
			{
				return SystemSettingMapping.AiOptionsRef != null && SystemSettingMapping.AiOptionsRef.AutoUseOtherAction != null && SystemSettingMapping.AiOptionsRef.AutoUseOtherAction.Length > 2 && SystemSettingMapping.AiOptionsRef.AutoUseOtherAction[2];
			}
			set
			{
				AiOptions aiOptionsRef = SystemSettingMapping.AiOptionsRef;
				bool flag = ((aiOptionsRef != null) ? aiOptionsRef.AutoUseOtherAction : null) != null && SystemSettingMapping.AiOptionsRef.AutoUseOtherAction.Length > 2;
				if (flag)
				{
					SystemSettingMapping.AiOptionsRef.AutoUseOtherAction[2] = value;
				}
			}
		}

		// Token: 0x17000B47 RID: 2887
		// (get) Token: 0x06005D06 RID: 23814 RVA: 0x002AC52C File Offset: 0x002AA72C
		// (set) Token: 0x06005D07 RID: 23815 RVA: 0x002AC560 File Offset: 0x002AA760
		[BoolSetting(ESettingSubCategory.AutoCast, 1, LanguageKey.LK_SystemSetting_AutoCastAttack, Key = ESettingKey.AutoCastAttack, TipLanguageKey = LanguageKey.LK_SystemSetting_AutoCastAttack_Tips)]
		public bool AutoCastAttack
		{
			get
			{
				AiOptions aiOptionsRef = SystemSettingMapping.AiOptionsRef;
				return ((aiOptionsRef != null) ? aiOptionsRef.AutoCastSkill : null) != null && SystemSettingMapping.AiOptionsRef.AutoCastSkill.Length != 0 && SystemSettingMapping.AiOptionsRef.AutoCastSkill[0];
			}
			set
			{
				AiOptions aiOptionsRef = SystemSettingMapping.AiOptionsRef;
				bool flag = ((aiOptionsRef != null) ? aiOptionsRef.AutoCastSkill : null) != null && SystemSettingMapping.AiOptionsRef.AutoCastSkill.Length != 0;
				if (flag)
				{
					SystemSettingMapping.AiOptionsRef.AutoCastSkill[0] = value;
				}
			}
		}

		// Token: 0x17000B48 RID: 2888
		// (get) Token: 0x06005D08 RID: 23816 RVA: 0x002AC5A3 File Offset: 0x002AA7A3
		// (set) Token: 0x06005D09 RID: 23817 RVA: 0x002AC5D8 File Offset: 0x002AA7D8
		[BoolSetting(ESettingSubCategory.AutoCast, 2, LanguageKey.LK_SystemSetting_AutoCastAgile, Key = ESettingKey.AutoCastAgile, TipLanguageKey = LanguageKey.LK_SystemSetting_AutoCastAgile_Tips)]
		public bool AutoCastAgile
		{
			get
			{
				AiOptions aiOptionsRef = SystemSettingMapping.AiOptionsRef;
				return ((aiOptionsRef != null) ? aiOptionsRef.AutoCastSkill : null) != null && SystemSettingMapping.AiOptionsRef.AutoCastSkill.Length > 1 && SystemSettingMapping.AiOptionsRef.AutoCastSkill[1];
			}
			set
			{
				AiOptions aiOptionsRef = SystemSettingMapping.AiOptionsRef;
				bool flag = ((aiOptionsRef != null) ? aiOptionsRef.AutoCastSkill : null) != null && SystemSettingMapping.AiOptionsRef.AutoCastSkill.Length > 1;
				if (flag)
				{
					SystemSettingMapping.AiOptionsRef.AutoCastSkill[1] = value;
				}
			}
		}

		// Token: 0x17000B49 RID: 2889
		// (get) Token: 0x06005D0A RID: 23818 RVA: 0x002AC61C File Offset: 0x002AA81C
		// (set) Token: 0x06005D0B RID: 23819 RVA: 0x002AC650 File Offset: 0x002AA850
		[BoolSetting(ESettingSubCategory.AutoCast, 3, LanguageKey.LK_SystemSetting_AutoCastDefense, Key = ESettingKey.AutoCastDefense, TipLanguageKey = LanguageKey.LK_SystemSetting_AutoCastDefense_Tips)]
		public bool AutoCastDefense
		{
			get
			{
				AiOptions aiOptionsRef = SystemSettingMapping.AiOptionsRef;
				return ((aiOptionsRef != null) ? aiOptionsRef.AutoCastSkill : null) != null && SystemSettingMapping.AiOptionsRef.AutoCastSkill.Length > 2 && SystemSettingMapping.AiOptionsRef.AutoCastSkill[2];
			}
			set
			{
				AiOptions aiOptionsRef = SystemSettingMapping.AiOptionsRef;
				bool flag = ((aiOptionsRef != null) ? aiOptionsRef.AutoCastSkill : null) != null && SystemSettingMapping.AiOptionsRef.AutoCastSkill.Length > 2;
				if (flag)
				{
					SystemSettingMapping.AiOptionsRef.AutoCastSkill[2] = value;
				}
			}
		}

		// Token: 0x17000B4A RID: 2890
		// (get) Token: 0x06005D0C RID: 23820 RVA: 0x002AC694 File Offset: 0x002AA894
		// (set) Token: 0x06005D0D RID: 23821 RVA: 0x002AC6A8 File Offset: 0x002AA8A8
		[BoolSetting(ESettingSubCategory.AutoCast, 4, LanguageKey.LK_SystemSetting_AutoInterrupt, TipLanguageKey = LanguageKey.LK_SystemSetting_AutoInterrupt_Tips)]
		public bool AutoInterrupt
		{
			get
			{
				AiOptions aiOptionsRef = SystemSettingMapping.AiOptionsRef;
				return aiOptionsRef != null && aiOptionsRef.AutoInterrupt;
			}
			set
			{
				bool flag = SystemSettingMapping.AiOptionsRef != null;
				if (flag)
				{
					SystemSettingMapping.AiOptionsRef.AutoInterrupt = value;
				}
			}
		}

		// Token: 0x17000B4B RID: 2891
		// (get) Token: 0x06005D0E RID: 23822 RVA: 0x002AC6CD File Offset: 0x002AA8CD
		// (set) Token: 0x06005D0F RID: 23823 RVA: 0x002AC6E0 File Offset: 0x002AA8E0
		[BoolSetting(ESettingSubCategory.AutoCast, 5, LanguageKey.LK_SystemSetting_AutoClearAgile, TipLanguageKey = LanguageKey.LK_SystemSetting_AutoClearAgile_Tips)]
		public bool AutoClearAgile
		{
			get
			{
				AiOptions aiOptionsRef = SystemSettingMapping.AiOptionsRef;
				return aiOptionsRef != null && aiOptionsRef.AutoClearAgile;
			}
			set
			{
				bool flag = SystemSettingMapping.AiOptionsRef != null;
				if (flag)
				{
					SystemSettingMapping.AiOptionsRef.AutoClearAgile = value;
				}
			}
		}

		// Token: 0x17000B4C RID: 2892
		// (get) Token: 0x06005D10 RID: 23824 RVA: 0x002AC705 File Offset: 0x002AA905
		// (set) Token: 0x06005D11 RID: 23825 RVA: 0x002AC718 File Offset: 0x002AA918
		[BoolSetting(ESettingSubCategory.AutoCast, 6, LanguageKey.LK_SystemSetting_AutoClearDefense, TipLanguageKey = LanguageKey.LK_SystemSetting_AutoClearDefense_Tips)]
		public bool AutoClearDefense
		{
			get
			{
				AiOptions aiOptionsRef = SystemSettingMapping.AiOptionsRef;
				return aiOptionsRef != null && aiOptionsRef.AutoClearDefense;
			}
			set
			{
				bool flag = SystemSettingMapping.AiOptionsRef != null;
				if (flag)
				{
					SystemSettingMapping.AiOptionsRef.AutoClearDefense = value;
				}
			}
		}

		// Token: 0x17000B4D RID: 2893
		// (get) Token: 0x06005D12 RID: 23826 RVA: 0x002AC73D File Offset: 0x002AA93D
		// (set) Token: 0x06005D13 RID: 23827 RVA: 0x002AC750 File Offset: 0x002AA950
		[BoolSetting(ESettingSubCategory.AutoCast, 7, LanguageKey.LK_SystemSetting_AutoCostNeiliAllocation, TipLanguageKey = LanguageKey.LK_SystemSetting_AutoCostNeiliAllocation_Tips)]
		public bool AutoCostNeiliAllocation
		{
			get
			{
				AiOptions aiOptionsRef = SystemSettingMapping.AiOptionsRef;
				return aiOptionsRef != null && aiOptionsRef.AutoCostNeiliAllocation;
			}
			set
			{
				bool flag = SystemSettingMapping.AiOptionsRef != null;
				if (flag)
				{
					SystemSettingMapping.AiOptionsRef.AutoCostNeiliAllocation = value;
				}
			}
		}

		// Token: 0x17000B4E RID: 2894
		// (get) Token: 0x06005D14 RID: 23828 RVA: 0x002AC775 File Offset: 0x002AA975
		// (set) Token: 0x06005D15 RID: 23829 RVA: 0x002AC788 File Offset: 0x002AA988
		[BoolSetting(ESettingSubCategory.AutoCast, 8, LanguageKey.LK_SystemSetting_AutoCostTrick, TipLanguageKey = LanguageKey.LK_SystemSetting_AutoCostTrick_Tips)]
		public bool AutoCostTrick
		{
			get
			{
				AiOptions aiOptionsRef = SystemSettingMapping.AiOptionsRef;
				return aiOptionsRef != null && aiOptionsRef.AutoCostTrick;
			}
			set
			{
				bool flag = SystemSettingMapping.AiOptionsRef != null;
				if (flag)
				{
					SystemSettingMapping.AiOptionsRef.AutoCostTrick = value;
				}
			}
		}

		// Token: 0x17000B4F RID: 2895
		// (get) Token: 0x06005D16 RID: 23830 RVA: 0x002AC7AD File Offset: 0x002AA9AD
		// (set) Token: 0x06005D17 RID: 23831 RVA: 0x002AC7C0 File Offset: 0x002AA9C0
		[TeammateCommandSetting(ESettingSubCategory.AutoAssist, 1, LanguageKey.LK_SystemSetting_TeammateCommand, Key = ESettingKey.AutoUseTeammateCommand, TipLanguageKey = LanguageKey.LK_Combat_Ai_Auto_Teammate_Cmd_Tips)]
		public bool[] AutoUseTeammateCommand
		{
			get
			{
				AiOptions aiOptionsRef = SystemSettingMapping.AiOptionsRef;
				return (aiOptionsRef != null) ? aiOptionsRef.AutoUseTeammateCommand : null;
			}
			set
			{
				AiOptions aiOptionsRef = SystemSettingMapping.AiOptionsRef;
				bool flag = ((aiOptionsRef != null) ? aiOptionsRef.AutoUseTeammateCommand : null) != null && value != null;
				if (flag)
				{
					int i = 0;
					while (i < value.Length && i < SystemSettingMapping.AiOptionsRef.AutoUseTeammateCommand.Length)
					{
						SystemSettingMapping.AiOptionsRef.AutoUseTeammateCommand[i] = value[i];
						i++;
					}
				}
			}
		}
	}
}
