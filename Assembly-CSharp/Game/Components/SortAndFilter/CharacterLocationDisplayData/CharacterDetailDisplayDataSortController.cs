using System;
using System.Collections.Generic;
using System.Linq;
using GameData.Domains.Taiwu.Display;
using GameData.Domains.Taiwu.Display.VillagerRoleArrangement;

namespace Game.Components.SortAndFilter.CharacterLocationDisplayData
{
	// Token: 0x02000E51 RID: 3665
	public class CharacterDetailDisplayDataSortController : SortController<int>
	{
		// Token: 0x0600AC2C RID: 44076 RVA: 0x004EDB36 File Offset: 0x004EBD36
		public CharacterDetailDisplayDataSortController(CharacterDetailDisplayDataSortAndFilterControllerController controller)
		{
			this.Controller = controller;
		}

		// Token: 0x0600AC2D RID: 44077 RVA: 0x004EDB48 File Offset: 0x004EBD48
		public override Comparison<int> GenerateComparer(SortStateData sortData)
		{
			return (int x, int y) => this.CompareData(x, y, sortData);
		}

		// Token: 0x0600AC2E RID: 44078 RVA: 0x004EDB7C File Offset: 0x004EBD7C
		private int CompareData(int x, int y, SortStateData sortData)
		{
			Func<int, int> <>9__2;
			Func<int, int> <>9__3;
			Func<int, int> <>9__4;
			Func<int, int> <>9__5;
			Func<int, int> <>9__6;
			Func<int, int> <>9__7;
			Func<int, int> <>9__8;
			Func<int, int> <>9__9;
			Func<int, int> <>9__10;
			Func<int, int> <>9__11;
			Func<int, int> <>9__12;
			Func<int, int> <>9__13;
			Func<int, int> <>9__14;
			Func<int, int> <>9__15;
			Func<int, int> <>9__16;
			Func<int, int> <>9__17;
			Func<int, int> <>9__18;
			Func<int, int> <>9__19;
			Func<int, int> <>9__20;
			Func<int, int> <>9__21;
			Func<int, int> <>9__22;
			Func<int, int> <>9__23;
			return sortData.ItemStates.Select(delegate(SortItemState itemState)
			{
				short sortId = itemState.SortId;
				if (!true)
				{
				}
				Func<int, int> func2;
				switch (sortId)
				{
				case 179:
				{
					Func<int, int> func;
					if ((func = <>9__2) == null)
					{
						func = (<>9__2 = delegate(int charId)
						{
							VillagerRoleCharacterDisplayData valueOrDefault = this.Controller.Data.Villagers.GetValueOrDefault(charId);
							object obj;
							if (valueOrDefault == null)
							{
								obj = null;
							}
							else
							{
								VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = valueOrDefault.ArrangementDisplayData;
								obj = ((arrangementDisplayData != null) ? arrangementDisplayData.ArrangementData : null);
							}
							FarmerDisplayData fd = obj as FarmerDisplayData;
							return (fd != null) ? fd.CollectResourceActionCount : -1;
						});
					}
					func2 = func;
					break;
				}
				case 180:
				{
					Func<int, int> func3;
					if ((func3 = <>9__3) == null)
					{
						func3 = (<>9__3 = delegate(int charId)
						{
							VillagerRoleCharacterDisplayData valueOrDefault = this.Controller.Data.Villagers.GetValueOrDefault(charId);
							object obj;
							if (valueOrDefault == null)
							{
								obj = null;
							}
							else
							{
								VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = valueOrDefault.ArrangementDisplayData;
								obj = ((arrangementDisplayData != null) ? arrangementDisplayData.ArrangementData : null);
							}
							FarmerDisplayData fd = obj as FarmerDisplayData;
							return (fd != null) ? fd.MigrateResourceSuccessRate : -1;
						});
					}
					func2 = func3;
					break;
				}
				case 181:
				{
					Func<int, int> func4;
					if ((func4 = <>9__4) == null)
					{
						func4 = (<>9__4 = delegate(int charId)
						{
							VillagerRoleCharacterDisplayData valueOrDefault = this.Controller.Data.Villagers.GetValueOrDefault(charId);
							object obj;
							if (valueOrDefault == null)
							{
								obj = null;
							}
							else
							{
								VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = valueOrDefault.ArrangementDisplayData;
								obj = ((arrangementDisplayData != null) ? arrangementDisplayData.ArrangementData : null);
							}
							FarmerDisplayData fd = obj as FarmerDisplayData;
							return (fd != null) ? fd.MigrateResourceSuccessRateBonus : -1;
						});
					}
					func2 = func4;
					break;
				}
				case 182:
				{
					Func<int, int> func5;
					if ((func5 = <>9__5) == null)
					{
						func5 = (<>9__5 = delegate(int charId)
						{
							VillagerRoleCharacterDisplayData valueOrDefault = this.Controller.Data.Villagers.GetValueOrDefault(charId);
							object obj;
							if (valueOrDefault == null)
							{
								obj = null;
							}
							else
							{
								VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = valueOrDefault.ArrangementDisplayData;
								obj = ((arrangementDisplayData != null) ? arrangementDisplayData.ArrangementData : null);
							}
							FarmerDisplayData fd = obj as FarmerDisplayData;
							return (fd != null) ? fd.UpgradeBuildingCoreRate : -1;
						});
					}
					func2 = func5;
					break;
				}
				case 183:
				{
					Func<int, int> func6;
					if ((func6 = <>9__6) == null)
					{
						func6 = (<>9__6 = delegate(int charId)
						{
							VillagerRoleCharacterDisplayData valueOrDefault = this.Controller.Data.Villagers.GetValueOrDefault(charId);
							object obj;
							if (valueOrDefault == null)
							{
								obj = null;
							}
							else
							{
								VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = valueOrDefault.ArrangementDisplayData;
								obj = ((arrangementDisplayData != null) ? arrangementDisplayData.ArrangementData : null);
							}
							HealingDisplayData hd = obj as HealingDisplayData;
							return (hd != null) ? hd.InteractTargetGrade : -1;
						});
					}
					func2 = func6;
					break;
				}
				case 184:
				{
					Func<int, int> func7;
					if ((func7 = <>9__7) == null)
					{
						func7 = (<>9__7 = delegate(int charId)
						{
							VillagerRoleCharacterDisplayData valueOrDefault = this.Controller.Data.Villagers.GetValueOrDefault(charId);
							object obj;
							if (valueOrDefault == null)
							{
								obj = null;
							}
							else
							{
								VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = valueOrDefault.ArrangementDisplayData;
								obj = ((arrangementDisplayData != null) ? arrangementDisplayData.ArrangementData : null);
							}
							HealingDisplayData hd = obj as HealingDisplayData;
							return (hd != null) ? hd.HealXiangshuInfectionAmount : -1;
						});
					}
					func2 = func7;
					break;
				}
				case 185:
				{
					Func<int, int> func8;
					if ((func8 = <>9__8) == null)
					{
						func8 = (<>9__8 = delegate(int charId)
						{
							VillagerRoleCharacterDisplayData valueOrDefault = this.Controller.Data.Villagers.GetValueOrDefault(charId);
							object obj;
							if (valueOrDefault == null)
							{
								obj = null;
							}
							else
							{
								VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = valueOrDefault.ArrangementDisplayData;
								obj = ((arrangementDisplayData != null) ? arrangementDisplayData.ArrangementData : null);
							}
							PeddlingDisplayData pd = obj as PeddlingDisplayData;
							return (int)((pd != null) ? pd.InteractTargetGrade : -1);
						});
					}
					func2 = func8;
					break;
				}
				case 186:
				{
					Func<int, int> func9;
					if ((func9 = <>9__9) == null)
					{
						func9 = (<>9__9 = delegate(int charId)
						{
							VillagerRoleCharacterDisplayData valueOrDefault = this.Controller.Data.Villagers.GetValueOrDefault(charId);
							object obj;
							if (valueOrDefault == null)
							{
								obj = null;
							}
							else
							{
								VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = valueOrDefault.ArrangementDisplayData;
								obj = ((arrangementDisplayData != null) ? arrangementDisplayData.ArrangementData : null);
							}
							PeddlingDisplayData pd = obj as PeddlingDisplayData;
							return (pd != null) ? pd.BuyPriceRate : -1;
						});
					}
					func2 = func9;
					break;
				}
				case 187:
				{
					Func<int, int> func10;
					if ((func10 = <>9__10) == null)
					{
						func10 = (<>9__10 = delegate(int charId)
						{
							VillagerRoleCharacterDisplayData valueOrDefault = this.Controller.Data.Villagers.GetValueOrDefault(charId);
							object obj;
							if (valueOrDefault == null)
							{
								obj = null;
							}
							else
							{
								VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = valueOrDefault.ArrangementDisplayData;
								obj = ((arrangementDisplayData != null) ? arrangementDisplayData.ArrangementData : null);
							}
							PeddlingDisplayData pd = obj as PeddlingDisplayData;
							return (pd != null) ? pd.SellPriceRate : -1;
						});
					}
					func2 = func10;
					break;
				}
				case 188:
				{
					Func<int, int> func11;
					if ((func11 = <>9__11) == null)
					{
						func11 = (<>9__11 = delegate(int charId)
						{
							VillagerRoleCharacterDisplayData valueOrDefault = this.Controller.Data.Villagers.GetValueOrDefault(charId);
							object obj;
							if (valueOrDefault == null)
							{
								obj = null;
							}
							else
							{
								VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = valueOrDefault.ArrangementDisplayData;
								obj = ((arrangementDisplayData != null) ? arrangementDisplayData.ArrangementData : null);
							}
							PeddlingDisplayData pd = obj as PeddlingDisplayData;
							return (pd != null) ? pd.AddFavorA : -1;
						});
					}
					func2 = func11;
					break;
				}
				case 189:
				{
					Func<int, int> func12;
					if ((func12 = <>9__12) == null)
					{
						func12 = (<>9__12 = delegate(int charId)
						{
							VillagerRoleCharacterDisplayData valueOrDefault = this.Controller.Data.Villagers.GetValueOrDefault(charId);
							object obj;
							if (valueOrDefault == null)
							{
								obj = null;
							}
							else
							{
								VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = valueOrDefault.ArrangementDisplayData;
								obj = ((arrangementDisplayData != null) ? arrangementDisplayData.ArrangementData : null);
							}
							PeddlingDisplayData pd = obj as PeddlingDisplayData;
							return (pd != null) ? pd.AddFavorB : -1;
						});
					}
					func2 = func12;
					break;
				}
				case 190:
				{
					Func<int, int> func13;
					if ((func13 = <>9__13) == null)
					{
						func13 = (<>9__13 = delegate(int charId)
						{
							VillagerRoleCharacterDisplayData valueOrDefault = this.Controller.Data.Villagers.GetValueOrDefault(charId);
							object obj;
							if (valueOrDefault == null)
							{
								obj = null;
							}
							else
							{
								VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = valueOrDefault.ArrangementDisplayData;
								obj = ((arrangementDisplayData != null) ? arrangementDisplayData.ArrangementData : null);
							}
							EntertainingDisplayData ed = obj as EntertainingDisplayData;
							return (ed != null) ? ed.ActionEffectCount : -1;
						});
					}
					func2 = func13;
					break;
				}
				case 191:
				{
					Func<int, int> func14;
					if ((func14 = <>9__14) == null)
					{
						func14 = (<>9__14 = delegate(int charId)
						{
							VillagerRoleCharacterDisplayData valueOrDefault = this.Controller.Data.Villagers.GetValueOrDefault(charId);
							object obj;
							if (valueOrDefault == null)
							{
								obj = null;
							}
							else
							{
								VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = valueOrDefault.ArrangementDisplayData;
								obj = ((arrangementDisplayData != null) ? arrangementDisplayData.ArrangementData : null);
							}
							EntertainingDisplayData ed = obj as EntertainingDisplayData;
							return (ed != null) ? ed.ActionEffectValue : -1;
						});
					}
					func2 = func14;
					break;
				}
				case 192:
				{
					Func<int, int> func15;
					if ((func15 = <>9__15) == null)
					{
						func15 = (<>9__15 = delegate(int charId)
						{
							VillagerRoleCharacterDisplayData valueOrDefault = this.Controller.Data.Villagers.GetValueOrDefault(charId);
							object obj;
							if (valueOrDefault == null)
							{
								obj = null;
							}
							else
							{
								VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = valueOrDefault.ArrangementDisplayData;
								obj = ((arrangementDisplayData != null) ? arrangementDisplayData.ArrangementData : null);
							}
							EntertainingDisplayData ed = obj as EntertainingDisplayData;
							return (ed != null) ? ed.ExtraPeopleCount : -1;
						});
					}
					func2 = func15;
					break;
				}
				case 193:
				{
					Func<int, int> func16;
					if ((func16 = <>9__16) == null)
					{
						func16 = (<>9__16 = delegate(int charId)
						{
							VillagerRoleCharacterDisplayData valueOrDefault = this.Controller.Data.Villagers.GetValueOrDefault(charId);
							object obj;
							if (valueOrDefault == null)
							{
								obj = null;
							}
							else
							{
								VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = valueOrDefault.ArrangementDisplayData;
								obj = ((arrangementDisplayData != null) ? arrangementDisplayData.ArrangementData : null);
							}
							EntertainingDisplayData ed = obj as EntertainingDisplayData;
							return (ed != null) ? ed.RelationChange : -1;
						});
					}
					func2 = func16;
					break;
				}
				case 194:
				{
					Func<int, int> func17;
					if ((func17 = <>9__17) == null)
					{
						func17 = (<>9__17 = delegate(int charId)
						{
							VillagerRoleCharacterDisplayData valueOrDefault = this.Controller.Data.Villagers.GetValueOrDefault(charId);
							object obj;
							if (valueOrDefault == null)
							{
								obj = null;
							}
							else
							{
								VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = valueOrDefault.ArrangementDisplayData;
								obj = ((arrangementDisplayData != null) ? arrangementDisplayData.ArrangementData : null);
							}
							GuardingSwordTombDisplayData gd = obj as GuardingSwordTombDisplayData;
							return (gd != null) ? gd.InformationGatheringSuccessRate : -1;
						});
					}
					func2 = func17;
					break;
				}
				case 195:
				{
					Func<int, int> func18;
					if ((func18 = <>9__18) == null)
					{
						func18 = (<>9__18 = delegate(int charId)
						{
							VillagerRoleCharacterDisplayData valueOrDefault = this.Controller.Data.Villagers.GetValueOrDefault(charId);
							object obj;
							if (valueOrDefault == null)
							{
								obj = null;
							}
							else
							{
								VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = valueOrDefault.ArrangementDisplayData;
								obj = ((arrangementDisplayData != null) ? arrangementDisplayData.ArrangementData : null);
							}
							GuardingSwordTombDisplayData gd = obj as GuardingSwordTombDisplayData;
							return (gd != null) ? gd.InjuryProbability : -1;
						});
					}
					func2 = func18;
					break;
				}
				case 196:
				{
					Func<int, int> func19;
					if ((func19 = <>9__19) == null)
					{
						func19 = (<>9__19 = delegate(int charId)
						{
							VillagerRoleCharacterDisplayData valueOrDefault = this.Controller.Data.Villagers.GetValueOrDefault(charId);
							object obj;
							if (valueOrDefault == null)
							{
								obj = null;
							}
							else
							{
								VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = valueOrDefault.ArrangementDisplayData;
								obj = ((arrangementDisplayData != null) ? arrangementDisplayData.ArrangementData : null);
							}
							GuardingSwordTombDisplayData gd = obj as GuardingSwordTombDisplayData;
							return (gd != null) ? gd.FeatureGainRateA : -1;
						});
					}
					func2 = func19;
					break;
				}
				case 197:
				{
					Func<int, int> func20;
					if ((func20 = <>9__20) == null)
					{
						func20 = (<>9__20 = delegate(int charId)
						{
							VillagerRoleCharacterDisplayData valueOrDefault = this.Controller.Data.Villagers.GetValueOrDefault(charId);
							object obj;
							if (valueOrDefault == null)
							{
								obj = null;
							}
							else
							{
								VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = valueOrDefault.ArrangementDisplayData;
								obj = ((arrangementDisplayData != null) ? arrangementDisplayData.ArrangementData : null);
							}
							GuardingSwordTombDisplayData gd = obj as GuardingSwordTombDisplayData;
							return (gd != null) ? gd.FeatureGainRateB : -1;
						});
					}
					func2 = func20;
					break;
				}
				case 198:
				{
					Func<int, int> func21;
					if ((func21 = <>9__21) == null)
					{
						func21 = (<>9__21 = delegate(int charId)
						{
							VillagerRoleCharacterDisplayData valueOrDefault = this.Controller.Data.Villagers.GetValueOrDefault(charId);
							object obj;
							if (valueOrDefault == null)
							{
								obj = null;
							}
							else
							{
								VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = valueOrDefault.ArrangementDisplayData;
								obj = ((arrangementDisplayData != null) ? arrangementDisplayData.ArrangementData : null);
							}
							GuardingSwordTombDisplayData gd = obj as GuardingSwordTombDisplayData;
							return (gd != null) ? gd.InfectionDecreaseRate : -1;
						});
					}
					func2 = func21;
					break;
				}
				case 199:
				{
					Func<int, int> func22;
					if ((func22 = <>9__22) == null)
					{
						func22 = (<>9__22 = delegate(int charId)
						{
							VillagerRoleCharacterDisplayData valueOrDefault = this.Controller.Data.Villagers.GetValueOrDefault(charId);
							object obj;
							if (valueOrDefault == null)
							{
								obj = null;
							}
							else
							{
								VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = valueOrDefault.ArrangementDisplayData;
								obj = ((arrangementDisplayData != null) ? arrangementDisplayData.ArrangementData : null);
							}
							TaiwuEnvoyDisplayData td = obj as TaiwuEnvoyDisplayData;
							return (td != null) ? td.SpecialRuleCount : -1;
						});
					}
					func2 = func22;
					break;
				}
				case 200:
				{
					Func<int, int> func23;
					if ((func23 = <>9__23) == null)
					{
						func23 = (<>9__23 = delegate(int charId)
						{
							VillagerRoleCharacterDisplayData valueOrDefault = this.Controller.Data.Villagers.GetValueOrDefault(charId);
							object obj;
							if (valueOrDefault == null)
							{
								obj = null;
							}
							else
							{
								VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = valueOrDefault.ArrangementDisplayData;
								obj = ((arrangementDisplayData != null) ? arrangementDisplayData.ArrangementData : null);
							}
							TaiwuEnvoyDisplayData td = obj as TaiwuEnvoyDisplayData;
							return (td != null) ? td.MonthlyAuthorityCost : -1;
						});
					}
					func2 = func23;
					break;
				}
				default:
					func2 = null;
					break;
				}
				if (!true)
				{
				}
				Func<int, int> compareKey = func2;
				if (compareKey != null)
				{
					int result = compareKey(x).CompareTo(compareKey(y));
					if (result != 0)
					{
						return (itemState.SortDirection == ESortDirection.Ascending) ? result : (-result);
					}
				}
				return 0;
			}).FirstOrDefault((int comparisonResult) => comparisonResult != 0);
		}

		// Token: 0x0600AC2F RID: 44079 RVA: 0x004EDBE0 File Offset: 0x004EBDE0
		public override SortUiConfig GenerateConfig()
		{
			return new SortUiConfig
			{
				SortIds = new List<short>
				{
					179,
					180,
					181,
					182,
					183,
					184,
					185,
					186,
					187,
					188,
					189,
					190,
					191,
					192,
					193,
					194,
					195,
					196,
					197,
					198,
					199,
					200
				}
			};
		}

		// Token: 0x04008533 RID: 34099
		public CharacterDetailDisplayDataSortAndFilterControllerController Controller;
	}
}
