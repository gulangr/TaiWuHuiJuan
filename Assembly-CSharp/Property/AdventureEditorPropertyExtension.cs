using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork.UISystem.UIElements;
using TMPro;

namespace Property
{
	// Token: 0x020006A2 RID: 1698
	public static class AdventureEditorPropertyExtension
	{
		// Token: 0x06004F75 RID: 20341 RVA: 0x00252BD8 File Offset: 0x00250DD8
		public static void SetupEditor<T>(this TMP_Dropdown dropdown, Action<T> onSelected, Predicate<T> isInitialItem, bool callSelectedFirst = true) where T : Enum
		{
			Type type = typeof(T);
			Array values = Enum.GetValues(type);
			dropdown.SetupEditor(values.Cast<int>(), new Func<int, string>(Enum.GetNames(type).ElementAt<string>), delegate(int idx)
			{
				onSelected((T)((object)values.GetValue(idx)));
			}, (int idx) => isInitialItem((T)((object)values.GetValue(idx))), callSelectedFirst);
		}

		// Token: 0x06004F76 RID: 20342 RVA: 0x00252C50 File Offset: 0x00250E50
		public static void SetupEditor<T>(this TMP_Dropdown dropdown, string enumLocalizationPattern, Action<T> onSelected, Predicate<T> isInitialItem, bool callSelectedFirst = true) where T : Enum
		{
			Type type = typeof(T);
			Array values = Enum.GetValues(type);
			string[] names = (from name in Enum.GetNames(type)
			select LocalStringManager.Get(string.Format(enumLocalizationPattern, name))).ToArray<string>();
			dropdown.SetupEditor(values.Cast<int>(), new Func<int, string>(names.ElementAt<string>), delegate(int idx)
			{
				onSelected((T)((object)values.GetValue(idx)));
			}, (int idx) => isInitialItem((T)((object)values.GetValue(idx))), callSelectedFirst);
		}

		// Token: 0x06004F77 RID: 20343 RVA: 0x00252CE8 File Offset: 0x00250EE8
		public static void SetupEditor<T>(this TMP_Dropdown dropdown, IEnumerable<T> range, Func<T, string> names, Action<T> onSelected, Predicate<T> isInitialItem, bool callSelectedFirst = true)
		{
			List<T> items = range.ToList<T>();
			dropdown.ClearOptions();
			dropdown.onValueChanged.ResetListener(delegate(int idx)
			{
				onSelected(items[idx]);
			});
			dropdown.AddOptions(items.Select(names).ToList<string>());
			int idx2 = Math.Max(0, items.FindIndex(isInitialItem));
			if (callSelectedFirst)
			{
				bool flag = dropdown.value == idx2;
				if (flag)
				{
					onSelected(items[idx2]);
				}
				else
				{
					dropdown.value = idx2;
				}
			}
			else
			{
				dropdown.SetValueWithoutNotify(idx2);
			}
		}

		// Token: 0x06004F78 RID: 20344 RVA: 0x00252DA0 File Offset: 0x00250FA0
		public static void SetupEditor<T>(this CDropdown dropdown, Action<T> onSelected, Predicate<T> isInitialItem, bool callSelectedFirst = true) where T : Enum
		{
			Type type = typeof(T);
			Array values = Enum.GetValues(type);
			dropdown.SetupEditor(values.Cast<int>(), new Func<int, string>(Enum.GetNames(type).ElementAt<string>), delegate(int idx)
			{
				onSelected((T)((object)values.GetValue(idx)));
			}, (int idx) => isInitialItem((T)((object)values.GetValue(idx))), callSelectedFirst);
		}

		// Token: 0x06004F79 RID: 20345 RVA: 0x00252E18 File Offset: 0x00251018
		public static void SetupEditor<T>(this CDropdown dropdown, string enumLocalizationPattern, Action<T> onSelected, Predicate<T> isInitialItem, bool callSelectedFirst = true) where T : Enum
		{
			Type type = typeof(T);
			Array values = Enum.GetValues(type);
			string[] names = (from name in Enum.GetNames(type)
			select LocalStringManager.Get(string.Format(enumLocalizationPattern, name))).ToArray<string>();
			dropdown.SetupEditor(values.Cast<int>(), new Func<int, string>(names.ElementAt<string>), delegate(int idx)
			{
				onSelected((T)((object)values.GetValue(idx)));
			}, (int idx) => isInitialItem((T)((object)values.GetValue(idx))), callSelectedFirst);
		}

		// Token: 0x06004F7A RID: 20346 RVA: 0x00252EB0 File Offset: 0x002510B0
		public static void SetupEditor<T>(this CDropdown dropdown, IEnumerable<T> range, Func<T, string> names, Action<T> onSelected, Predicate<T> isInitialItem, bool callSelectedFirst = true)
		{
			List<T> items = range.ToList<T>();
			dropdown.ClearOptions();
			dropdown.onValueChanged.ResetListener(delegate(int idx)
			{
				onSelected(items[idx]);
			});
			dropdown.AddOptions(items.Select(names).ToList<string>());
			int idx2 = Math.Max(0, items.FindIndex(isInitialItem));
			if (callSelectedFirst)
			{
				bool flag = dropdown.value == idx2;
				if (flag)
				{
					onSelected(items[idx2]);
				}
				else
				{
					dropdown.value = idx2;
				}
			}
			else
			{
				dropdown.SetValueWithoutNotify(idx2);
			}
		}
	}
}
