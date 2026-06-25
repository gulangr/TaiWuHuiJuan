using System;
using System.Collections.Generic;
using System.Globalization;
using AdventureEditor.Beta;
using GameData.Utilities;
using MoonSharp.Interpreter;

namespace FrameWork.ModSystem
{
	// Token: 0x02001048 RID: 4168
	public static class MoonSharpTableExtensions
	{
		// Token: 0x0600BE22 RID: 48674 RVA: 0x005643E4 File Offset: 0x005625E4
		public static void Save<TK, TV>(this Table table, TK saveKey, List<TV> list)
		{
			bool flag = list == null || list.Count <= 0;
			if (flag)
			{
				table.Remove(saveKey);
			}
			else
			{
				Table arrayTable = new Table(null);
				for (int i = 0; i < list.Count; i++)
				{
					arrayTable.Save(i + 1, list[i]);
				}
				table.Save(saveKey, arrayTable);
			}
		}

		// Token: 0x0600BE23 RID: 48675 RVA: 0x00564450 File Offset: 0x00562650
		public static void Save<TK, TV>(this Table table, TK saveKey, TV[] array)
		{
			bool flag = array == null || array.Length == 0;
			if (flag)
			{
				table.Remove(saveKey);
			}
			else
			{
				Table arrayTable = new Table(null);
				for (int i = 0; i < array.Length; i++)
				{
					arrayTable.Save(i + 1, array[i]);
				}
				table.Save(saveKey, arrayTable);
			}
		}

		// Token: 0x0600BE24 RID: 48676 RVA: 0x005644B4 File Offset: 0x005626B4
		public static void Save<TK, TV>(this Table table, TK saveKey, HashSet<TV> hashset)
		{
			bool flag = hashset == null || hashset.Count <= 0;
			if (flag)
			{
				table.Remove(saveKey);
			}
			else
			{
				Table arrayTable = new Table(null);
				int i = 1;
				foreach (TV element in hashset)
				{
					arrayTable.Save(i, element);
					i++;
				}
				table.Save(saveKey, arrayTable);
			}
		}

		// Token: 0x0600BE25 RID: 48677 RVA: 0x00564548 File Offset: 0x00562748
		public static void Save<TK, TV1, TV2>(this Table table, TK saveKey, ValueTuple<TV1, TV2>[] array)
		{
			bool flag = array == null || array.Length == 0;
			if (flag)
			{
				table.Remove(saveKey);
			}
			else
			{
				Table arrayTable = new Table(null);
				for (int i = 0; i < array.Length; i++)
				{
					ValueTuple<TV1, TV2> tuple = array[i];
					Table cellTable = new Table(null);
					cellTable.Save(1, tuple.Item1);
					cellTable.Save(2, tuple.Item2);
					arrayTable.Save(i + 1, cellTable);
				}
				table.Save(saveKey, arrayTable);
			}
		}

		// Token: 0x0600BE26 RID: 48678 RVA: 0x005645D8 File Offset: 0x005627D8
		public static void Save<TK, TV1, TV2>(this Table table, TK saveKey, List<ValueTuple<TV1, TV2>> list)
		{
			bool flag = list == null || list.Count <= 0;
			if (flag)
			{
				table.Remove(saveKey);
			}
			else
			{
				Table arrayTable = new Table(null);
				for (int i = 0; i < list.Count; i++)
				{
					ValueTuple<TV1, TV2> tuple = list[i];
					Table cellTable = new Table(null);
					cellTable.Save(1, tuple.Item1);
					cellTable.Save(2, tuple.Item2);
					arrayTable.Save(i + 1, cellTable);
				}
				table.Save(saveKey, arrayTable);
			}
		}

		// Token: 0x0600BE27 RID: 48679 RVA: 0x00564670 File Offset: 0x00562870
		public static void Save<TK, TV1, TV2, TV3>(this Table table, TK saveKey, List<ValueTuple<TV1, TV2, TV3>> list)
		{
			bool flag = list == null || list.Count <= 0;
			if (flag)
			{
				table.Remove(saveKey);
			}
			else
			{
				Table arrayTable = new Table(null);
				for (int i = 0; i < list.Count; i++)
				{
					ValueTuple<TV1, TV2, TV3> tuple = list[i];
					Table cellTable = new Table(null);
					cellTable.Save(1, tuple.Item1);
					cellTable.Save(2, tuple.Item2);
					cellTable.Save(3, tuple.Item3);
					arrayTable.Save(i + 1, cellTable);
				}
				table.Save(saveKey, arrayTable);
			}
		}

		// Token: 0x0600BE28 RID: 48680 RVA: 0x00564718 File Offset: 0x00562918
		public static void Save<TK, TV1, TV2, TV3, TV4>(this Table table, TK saveKey, List<ValueTuple<TV1, TV2, TV3, TV4>> list)
		{
			bool flag = list == null || list.Count <= 0;
			if (flag)
			{
				table.Remove(saveKey);
			}
			else
			{
				Table arrayTable = new Table(null);
				for (int i = 0; i < list.Count; i++)
				{
					ValueTuple<TV1, TV2, TV3, TV4> tuple = list[i];
					Table cellTable = new Table(null);
					cellTable.Save(1, tuple.Item1);
					cellTable.Save(2, tuple.Item2);
					cellTable.Save(3, tuple.Item3);
					cellTable.Save(4, tuple.Item4);
					arrayTable.Save(i + 1, cellTable);
				}
				table.Save(saveKey, arrayTable);
			}
		}

		// Token: 0x0600BE29 RID: 48681 RVA: 0x005647D0 File Offset: 0x005629D0
		public static void Save<TK, TV>(this Table table, TK saveKey, TV obj)
		{
			DynValue dynVal = DynValue.FromObject(null, obj);
			table.Set(DynValue.FromObject(table.OwnerScript, saveKey), dynVal);
		}

		// Token: 0x0600BE2A RID: 48682 RVA: 0x00564804 File Offset: 0x00562A04
		public static bool LoadObject<TK>(this Table table, TK saveKey, Type type, out object obj)
		{
			bool isEnum = type.IsEnum;
			bool result;
			if (isEnum)
			{
				obj = Enum.Parse(type, table.Get(DynValue.FromObject(table.OwnerScript, saveKey)).String);
				result = true;
			}
			else
			{
				try
				{
					obj = table.Get(DynValue.FromObject(table.OwnerScript, saveKey)).ToObject(type);
					result = true;
				}
				catch (Exception)
				{
					obj = null;
					AdaptableLog.Warning(string.Format("{0}: object type {1} is not yet supported.", saveKey, type.Name), false);
					result = false;
				}
			}
			return result;
		}

		// Token: 0x0600BE2B RID: 48683 RVA: 0x005648A4 File Offset: 0x00562AA4
		public static void Load<TK>(this Table table, TK key, out Table val)
		{
			DynValue dynVal = table.Get(DynValue.FromObject(table.OwnerScript, key));
			bool flag = dynVal.Type == DataType.Table;
			if (flag)
			{
				val = dynVal.Table;
			}
			else
			{
				val = null;
			}
		}

		// Token: 0x0600BE2C RID: 48684 RVA: 0x005648E4 File Offset: 0x00562AE4
		public static void Load<TK, TV>(this Table table, TK key, out TV val)
		{
			DynValue dynVal = table.Get(DynValue.FromObject(table.OwnerScript, key));
			bool flag = dynVal.IsNil();
			if (flag)
			{
				val = default(TV);
			}
			else
			{
				val = dynVal.ToObject<TV>();
			}
		}

		// Token: 0x0600BE2D RID: 48685 RVA: 0x0056492C File Offset: 0x00562B2C
		public static void Load<TK, TV1, TV2>(this Table table, TK key, out List<ValueTuple<TV1, TV2>> list)
		{
			bool flag = !table.ContainsKey(key);
			if (flag)
			{
				list = new List<ValueTuple<TV1, TV2>>();
			}
			else
			{
				Table arrayTable = table.Get(DynValue.FromObject(table.OwnerScript, key)).Table;
				list = new List<ValueTuple<TV1, TV2>>(arrayTable.Length);
				for (int i = 0; i < arrayTable.Length; i++)
				{
					Table cellTable;
					arrayTable.Load(i + 1, out cellTable);
					TV1 t;
					cellTable.Load(1, out t);
					TV2 t2;
					cellTable.Load(2, out t2);
					list.Add(new ValueTuple<TV1, TV2>(t, t2));
				}
			}
		}

		// Token: 0x0600BE2E RID: 48686 RVA: 0x005649C8 File Offset: 0x00562BC8
		public static void Load<TK, TV1, TV2, TV3>(this Table table, TK saveKey, out List<ValueTuple<TV1, TV2, TV3>> list)
		{
			bool flag = !table.ContainsKey(saveKey);
			if (flag)
			{
				list = new List<ValueTuple<TV1, TV2, TV3>>();
			}
			else
			{
				Table arrayTable = table.Get(DynValue.FromObject(table.OwnerScript, saveKey)).Table;
				list = new List<ValueTuple<TV1, TV2, TV3>>(arrayTable.Length);
				for (int i = 0; i < arrayTable.Length; i++)
				{
					Table cellTable;
					arrayTable.Load(i + 1, out cellTable);
					TV1 t;
					cellTable.Load(1, out t);
					TV2 t2;
					cellTable.Load(2, out t2);
					TV3 t3;
					cellTable.Load(3, out t3);
					list.Add(new ValueTuple<TV1, TV2, TV3>(t, t2, t3));
				}
			}
		}

		// Token: 0x0600BE2F RID: 48687 RVA: 0x00564A74 File Offset: 0x00562C74
		public static void Load<TK, TV1, TV2, TV3, TV4>(this Table table, TK saveKey, out List<ValueTuple<TV1, TV2, TV3, TV4>> list)
		{
			bool flag = !table.ContainsKey(saveKey);
			if (flag)
			{
				list = new List<ValueTuple<TV1, TV2, TV3, TV4>>();
			}
			else
			{
				Table arrayTable = table.Get(DynValue.FromObject(table.OwnerScript, saveKey)).Table;
				list = new List<ValueTuple<TV1, TV2, TV3, TV4>>(arrayTable.Length);
				for (int i = 0; i < arrayTable.Length; i++)
				{
					Table cellTable;
					arrayTable.Load(i + 1, out cellTable);
					TV1 t;
					cellTable.Load(1, out t);
					TV2 t2;
					cellTable.Load(2, out t2);
					TV3 t3;
					cellTable.Load(3, out t3);
					TV4 t4;
					cellTable.Load(4, out t4);
					list.Add(new ValueTuple<TV1, TV2, TV3, TV4>(t, t2, t3, t4));
				}
			}
		}

		// Token: 0x0600BE30 RID: 48688 RVA: 0x00564B2C File Offset: 0x00562D2C
		public static void Load<TK>(this Table table, TK key, out AdvPersonalityEventWeights[] array)
		{
			bool flag = !table.ContainsKey(key);
			if (flag)
			{
				array = Array.Empty<AdvPersonalityEventWeights>();
			}
			else
			{
				Table arrayTable;
				table.Load(key, out arrayTable);
				array = new AdvPersonalityEventWeights[arrayTable.Length];
				for (int i = 0; i < arrayTable.Length; i++)
				{
					arrayTable.Load(i + 1, out array[i]);
				}
			}
		}

		// Token: 0x0600BE31 RID: 48689 RVA: 0x00564B94 File Offset: 0x00562D94
		public static void Load<TK>(this Table table, TK saveKey, out AdvPersonalityEventWeights weights)
		{
			weights = new AdvPersonalityEventWeights();
			Table weightsTable;
			table.Get(saveKey, out weightsTable);
			bool flag = weightsTable.ContainsKey("EmptyBlockWeight");
			if (flag)
			{
				weightsTable.Get("EmptyBlockWeight", out weights.EmptyBlockWeight);
			}
			weightsTable.Load("EventWeights", out weights.EventWeights);
			weightsTable.Load("ItemRewardWeights", out weights.ItemRewardWeights);
			weightsTable.Load("ResRewardWeights", out weights.ResRewardWeights);
			weightsTable.Load("BonusWeights", out weights.BonusWeights);
		}

		// Token: 0x0600BE32 RID: 48690 RVA: 0x00564C24 File Offset: 0x00562E24
		public static void Load<TK, TV>(this Table table, TK key, out TV[] array)
		{
			bool flag = !table.ContainsKey(key);
			if (flag)
			{
				array = Array.Empty<TV>();
			}
			else
			{
				Table arrayTable;
				table.Load(key, out arrayTable);
				array = new TV[arrayTable.Length];
				for (int i = 0; i < arrayTable.Length; i++)
				{
					arrayTable.Load(i + 1, out array[i]);
				}
			}
		}

		// Token: 0x0600BE33 RID: 48691 RVA: 0x00564C8C File Offset: 0x00562E8C
		public static void Load<TK, TV1, TV2>(this Table table, TK key, out ValueTuple<TV1, TV2>[] array)
		{
			bool flag = !table.ContainsKey(key);
			if (flag)
			{
				array = Array.Empty<ValueTuple<TV1, TV2>>();
			}
			else
			{
				Table arrayTable;
				table.Load(key, out arrayTable);
				array = new ValueTuple<TV1, TV2>[arrayTable.Length];
				for (int i = 0; i < arrayTable.Length; i++)
				{
					Table cellTable;
					arrayTable.Load(i + 1, out cellTable);
					TV1 t;
					cellTable.Load(1, out t);
					TV2 t2;
					cellTable.Load(2, out t2);
					array[i] = new ValueTuple<TV1, TV2>(t, t2);
				}
			}
		}

		// Token: 0x0600BE34 RID: 48692 RVA: 0x00564D14 File Offset: 0x00562F14
		public static void Load<TK, TV>(this Table table, TK key, out List<TV> list)
		{
			bool flag = !table.ContainsKey(key);
			if (flag)
			{
				list = new List<TV>();
			}
			else
			{
				Table arrayTable;
				table.Load(key, out arrayTable);
				list = new List<TV>(arrayTable.Length);
				for (int i = 0; i < arrayTable.Length; i++)
				{
					TV val;
					arrayTable.Load(i + 1, out val);
					list.Add(val);
				}
			}
		}

		// Token: 0x0600BE35 RID: 48693 RVA: 0x00564D80 File Offset: 0x00562F80
		public static bool ContainsKey<TK>(this Table table, TK key)
		{
			return !DynValue.Nil.Equals(table.Get(key));
		}

		// Token: 0x0600BE36 RID: 48694 RVA: 0x00564DAC File Offset: 0x00562FAC
		public static void Get<TKey, TValue>(this Table table, TKey key, out TValue value)
		{
			object raw = table.Get(DynValue.FromObject(table.OwnerScript, key)).ToObject();
			IConvertible rawConv = raw as IConvertible;
			value = (TValue)((object)((rawConv != null) ? rawConv.ToType(typeof(TValue), CultureInfo.InvariantCulture) : raw));
		}

		// Token: 0x0600BE37 RID: 48695 RVA: 0x00564E04 File Offset: 0x00563004
		public static TValue Get<TKey, TValue>(this Table table, TKey key)
		{
			TValue value;
			table.Get(key, out value);
			return value;
		}

		// Token: 0x0600BE38 RID: 48696 RVA: 0x00564E24 File Offset: 0x00563024
		public static TValue Get<TValue>(this Table table, string key)
		{
			TValue value;
			table.Get(key, out value);
			return value;
		}

		// Token: 0x0600BE39 RID: 48697 RVA: 0x00564E44 File Offset: 0x00563044
		public static TValue GetOrDefault<TKey, TValue>(this Table table, TKey key, TValue defaultValue)
		{
			bool flag = table.ContainsKey(key);
			TValue value;
			if (flag)
			{
				table.Get(key, out value);
			}
			else
			{
				value = defaultValue;
			}
			return value;
		}

		// Token: 0x0600BE3A RID: 48698 RVA: 0x00564E70 File Offset: 0x00563070
		public static void Set<TKey, TValue>(this Table table, TKey key, TValue value)
		{
			table.Set(DynValue.FromObject(table.OwnerScript, key), DynValue.FromObject(table.OwnerScript, value));
		}

		// Token: 0x0600BE3B RID: 48699 RVA: 0x00564E9C File Offset: 0x0056309C
		private static IEnumerable<DynValue> GetRawKeys<TKey>(this Table table)
		{
			foreach (DynValue key in table.Keys)
			{
				bool flag = !MoonSharpTableExtensions.CanTranslate<TKey>(key.ToObject());
				if (!flag)
				{
					yield return key;
					key = null;
				}
			}
			IEnumerator<DynValue> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x0600BE3C RID: 48700 RVA: 0x00564EAC File Offset: 0x005630AC
		public static IEnumerable<TKey> GetKeys<TKey>(this Table table)
		{
			foreach (DynValue key in table.GetRawKeys<TKey>())
			{
				yield return key.ToObject<TKey>();
				key = null;
			}
			IEnumerator<DynValue> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x0600BE3D RID: 48701 RVA: 0x00564EBC File Offset: 0x005630BC
		public static void SetInPath<TV>(this Table table, string path, TV val)
		{
			string[] segments = path.Split('.', StringSplitOptions.None);
			Table cur = table;
			for (int i = 0; i < segments.Length - 1; i++)
			{
				string seg = segments[i];
				bool flag = !(cur[seg] is Table);
				if (flag)
				{
					cur[seg] = DynValue.NewTable(table.OwnerScript);
				}
				cur = cur.Get(seg).Table;
			}
			Table table2 = cur;
			string[] array = segments;
			table2.Set(array[array.Length - 1], val);
		}

		// Token: 0x0600BE3E RID: 48702 RVA: 0x00564F3C File Offset: 0x0056313C
		public static TV GetInPath<TV>(this Table root, string path)
		{
			bool flag = root == null;
			if (flag)
			{
				throw new ArgumentNullException("root");
			}
			bool flag2 = string.IsNullOrEmpty(path);
			if (flag2)
			{
				throw new ArgumentException("path is null or empty");
			}
			string[] segments = path.Split('.', StringSplitOptions.None);
			DynValue cur = DynValue.NewTable(root);
			string[] array = segments;
			int i = 0;
			TV result;
			while (i < array.Length)
			{
				string seg = array[i];
				bool flag3 = cur.Type != DataType.Table;
				if (flag3)
				{
					result = default(TV);
				}
				else
				{
					cur = cur.Table.Get(seg);
					bool flag4 = cur.IsNil();
					if (!flag4)
					{
						i++;
						continue;
					}
					result = default(TV);
				}
				return result;
			}
			try
			{
				result = cur.ToObject<TV>();
			}
			catch (InvalidCastException)
			{
				result = default(TV);
			}
			return result;
		}

		// Token: 0x0600BE3F RID: 48703 RVA: 0x0056501C File Offset: 0x0056321C
		public static void ForEach<TKey, TValue>(this Table table, Action<TKey, TValue> action)
		{
			TValue defaultValue = default(TValue);
			foreach (DynValue key in table.GetRawKeys<TKey>())
			{
				bool flag = !MoonSharpTableExtensions.CanTranslate<TKey>(key.ToObject());
				if (!flag)
				{
					DynValue value = table.Get(key);
					bool flag2 = MoonSharpTableExtensions.CanTranslate<TValue>(value.ToObject());
					if (flag2)
					{
						action(key.ToObject<TKey>(), value.ToObject<TValue>());
					}
					else
					{
						AdaptableLog.TagWarning("MoonSharpTableExtensions", string.Format("{0} is not compatible with {1}, using default value", value, typeof(TValue)), false);
						action(key.ToObject<TKey>(), defaultValue);
					}
				}
			}
		}

		// Token: 0x0600BE40 RID: 48704 RVA: 0x005650EC File Offset: 0x005632EC
		private static bool CanTranslate<TP>(object val)
		{
			bool flag;
			if (val != null)
			{
				DynValue valDynValue = val as DynValue;
				flag = (valDynValue != null && valDynValue.IsNil());
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				Type typeTp = typeof(TP);
				IConvertible valConvertible;
				bool flag3;
				if (val.GetType().IsValueType)
				{
					valConvertible = (val as IConvertible);
					flag3 = (valConvertible != null);
				}
				else
				{
					flag3 = false;
				}
				bool flag4 = flag3;
				if (flag4)
				{
					val = valConvertible.ToType(typeTp, CultureInfo.InvariantCulture);
				}
				bool flag5 = !typeTp.IsAssignableFrom(val.GetType());
				result = !flag5;
			}
			return result;
		}

		// Token: 0x0600BE41 RID: 48705 RVA: 0x00565177 File Offset: 0x00563377
		public static Table NewTable(this Script env)
		{
			return new Table(env);
		}
	}
}
