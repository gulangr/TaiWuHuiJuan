using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Game.Views.SystemSetting
{
	// Token: 0x0200076B RID: 1899
	public static class HotKeyService
	{
		// Token: 0x06005BE2 RID: 23522 RVA: 0x002AA6DC File Offset: 0x002A88DC
		public static void RegisterCommand(ESettingSubCategory subCategory, HotKeyCommand command)
		{
			List<HotKeyCommand> list;
			bool flag = !HotKeyService._subCategoryToCommands.TryGetValue(subCategory, out list);
			if (flag)
			{
				list = new List<HotKeyCommand>();
				HotKeyService._subCategoryToCommands[subCategory] = list;
			}
			bool flag2 = !list.Contains(command);
			if (flag2)
			{
				list.Add(command);
			}
		}

		// Token: 0x06005BE3 RID: 23523 RVA: 0x002AA72C File Offset: 0x002A892C
		public static bool IsCommandKeyConflict(HotKeyCommand command)
		{
			ValueTuple<bool, bool> state;
			return command != null && HotKeyService._conflictStates.TryGetValue(command, out state) && state.Item1;
		}

		// Token: 0x06005BE4 RID: 23524 RVA: 0x002AA75C File Offset: 0x002A895C
		public static bool IsCommandMouseKeyConflict(HotKeyCommand command)
		{
			ValueTuple<bool, bool> state;
			return command != null && HotKeyService._conflictStates.TryGetValue(command, out state) && state.Item2;
		}

		// Token: 0x06005BE5 RID: 23525 RVA: 0x002AA78C File Offset: 0x002A898C
		public static void SetCommandConflictState(HotKeyCommand command, bool keyConflict, bool mouseKeyConflict)
		{
			bool flag = command == null;
			if (!flag)
			{
				HotKeyService._conflictStates[command] = new ValueTuple<bool, bool>(keyConflict, mouseKeyConflict);
			}
		}

		// Token: 0x06005BE6 RID: 23526 RVA: 0x002AA7B8 File Offset: 0x002A89B8
		public static bool IsHotKeyGroupFirst(ESettingSubCategory subCategory)
		{
			return false;
		}

		// Token: 0x06005BE7 RID: 23527 RVA: 0x002AA7CC File Offset: 0x002A89CC
		public static string GetHotKeyGroupName(ESettingSubCategory subCategory)
		{
			SubCategoryInfo[] hotKeySubs = ViewSystemSetting.Categories[4].SubCategories;
			for (int i = 0; i < hotKeySubs.Length; i++)
			{
				bool flag = hotKeySubs[i].SubCategory == subCategory;
				if (flag)
				{
					return hotKeySubs[i].Title.Tr();
				}
			}
			return string.Empty;
		}

		// Token: 0x06005BE8 RID: 23528 RVA: 0x002AA828 File Offset: 0x002A8A28
		public static List<HotKeyConflictInfo> FindConflicts(ESettingSubCategory currentSubCategory, HotKeyCommand currentCommand, KeyCode key, KeyCode fnKey)
		{
			List<HotKeyConflictInfo> result = new List<HotKeyConflictInfo>();
			bool flag = key == KeyCode.None || currentCommand == null;
			List<HotKeyConflictInfo> result2;
			if (flag)
			{
				result2 = result;
			}
			else
			{
				HotKeyService.CheckSubCategoryConflicts(currentSubCategory, currentCommand, key, fnKey, result);
				result2 = result;
			}
			return result2;
		}

		// Token: 0x06005BE9 RID: 23529 RVA: 0x002AA860 File Offset: 0x002A8A60
		[return: TupleElementNames(new string[]
		{
			"isConflict",
			"canConfirm",
			"sameGroup",
			"groupLangId",
			"conflictCommand"
		})]
		public static ValueTuple<bool, bool, bool, LanguageKey, HotKeyCommand> CheckConflict(ESettingSubCategory subCategory, HotKeyCommand currentCommand, KeyCode key, KeyCode fnKey)
		{
			bool flag = currentCommand == null;
			ValueTuple<bool, bool, bool, LanguageKey, HotKeyCommand> result;
			if (flag)
			{
				result = new ValueTuple<bool, bool, bool, LanguageKey, HotKeyCommand>(false, true, false, LanguageKey.Invalid, null);
			}
			else
			{
				List<HotKeyConflictInfo> conflicts = HotKeyService.FindConflicts(subCategory, currentCommand, key, fnKey);
				bool flag2 = conflicts.Count > 0;
				if (flag2)
				{
					int index = conflicts.FindIndex((HotKeyConflictInfo x) => x.KeyCannotConfirm);
					HotKeyConflictInfo first = (index >= 0) ? conflicts[index] : conflicts[0];
					result = new ValueTuple<bool, bool, bool, LanguageKey, HotKeyCommand>(true, !first.KeyCannotConfirm, true, first.GroupLangId, first.Command);
				}
				else
				{
					result = new ValueTuple<bool, bool, bool, LanguageKey, HotKeyCommand>(false, true, false, LanguageKey.Invalid, null);
				}
			}
			return result;
		}

		// Token: 0x06005BEA RID: 23530 RVA: 0x002AA910 File Offset: 0x002A8B10
		private static void CheckSubCategoryConflicts(ESettingSubCategory subCategory, HotKeyCommand currentCommand, KeyCode key, KeyCode fnKey, List<HotKeyConflictInfo> result)
		{
			List<HotKeyCommand> commands;
			bool flag = !HotKeyService._subCategoryToCommands.TryGetValue(subCategory, out commands);
			if (!flag)
			{
				Func<SubCategoryInfo, bool> <>9__0;
				for (int i = 0; i < commands.Count; i++)
				{
					HotKeyCommand cmd = commands[i];
					bool flag2 = cmd == currentCommand;
					if (!flag2)
					{
						ValueTuple<bool, bool, bool> valueTuple = cmd.IsConflictDetail(key, fnKey);
						bool keyConflict = valueTuple.Item1;
						bool keyCannotConfirm = valueTuple.Item2;
						bool mouseKeyConflict = valueTuple.Item3;
						bool flag3 = keyConflict || keyCannotConfirm || mouseKeyConflict;
						if (flag3)
						{
							IEnumerable<SubCategoryInfo> subCategories = ViewSystemSetting.Categories[4].SubCategories;
							Func<SubCategoryInfo, bool> predicate;
							if ((predicate = <>9__0) == null)
							{
								predicate = (<>9__0 = ((SubCategoryInfo x) => x.SubCategory == subCategory));
							}
							LanguageKey groupLangId = subCategories.FirstOrDefault(predicate).Title;
							result.Add(new HotKeyConflictInfo
							{
								Command = cmd,
								SubCategory = subCategory,
								GroupLangId = groupLangId,
								KeyConflict = keyConflict,
								KeyCannotConfirm = keyCannotConfirm,
								MouseKeyConflict = mouseKeyConflict
							});
						}
					}
				}
			}
		}

		// Token: 0x06005BEB RID: 23531 RVA: 0x002AAA3C File Offset: 0x002A8C3C
		public static void ResetSubCategory(ESettingSubCategory subCategory)
		{
			List<HotKeyCommand> commands;
			bool flag = !HotKeyService._subCategoryToCommands.TryGetValue(subCategory, out commands) || commands == null;
			if (!flag)
			{
				for (int i = 0; i < commands.Count; i++)
				{
					commands[i].Reset();
					HotKeyService.SetCommandConflictState(commands[i], false, false);
				}
				CommandKitBase.SaveHotKeyConfig();
			}
		}

		// Token: 0x04003F70 RID: 16240
		private static readonly Dictionary<ESettingSubCategory, List<HotKeyCommand>> _subCategoryToCommands = new Dictionary<ESettingSubCategory, List<HotKeyCommand>>();

		// Token: 0x04003F71 RID: 16241
		[TupleElementNames(new string[]
		{
			"keyConflict",
			"mouseKeyConflict"
		})]
		private static readonly Dictionary<HotKeyCommand, ValueTuple<bool, bool>> _conflictStates = new Dictionary<HotKeyCommand, ValueTuple<bool, bool>>();
	}
}
