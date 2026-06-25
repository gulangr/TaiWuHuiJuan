using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using Config;
using Config.Common;
using GameData.Utilities;
using TaiwuModdingLib.Core.Utils;

namespace FrameWork.ModSystem
{
	// Token: 0x02001042 RID: 4162
	public static class ConfigDataModificationUtils
	{
		// Token: 0x0600BDD8 RID: 48600 RVA: 0x005631AC File Offset: 0x005613AC
		public static void ReplaceConfig<TConfig, TItem>(this TConfig config, int templateId, TItem item) where TConfig : IEnumerable<TItem>, IConfigData
		{
			Type configType = config.GetType();
			FieldInfo field = configType.GetField("_dataArray", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			List<TItem> dataList = (List<TItem>)((field != null) ? field.GetValue(config) : null);
			Debug.Assert(dataList != null, "dataList != null");
			dataList[templateId] = item;
		}

		// Token: 0x0600BDD9 RID: 48601 RVA: 0x00563208 File Offset: 0x00561408
		[Obsolete]
		public static void AppendConfig<TConfig, TItem>(this TConfig config, TItem item) where TConfig : IEnumerable<TItem>, IConfigData
		{
			Type configType = config.GetType();
			FieldInfo field = configType.GetField("_dataArray", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			List<TItem> dataList = (List<TItem>)((field != null) ? field.GetValue(config) : null);
			Debug.Assert(dataList != null, "dataList != null");
			dataList.Add(item);
		}

		// Token: 0x0600BDDA RID: 48602 RVA: 0x00563260 File Offset: 0x00561460
		[Obsolete]
		public static void AppendConfig<TConfig>(this TConfig config, object item) where TConfig : IConfigData
		{
			Type configType = config.GetType();
			FieldInfo field = configType.GetField("_dataArray", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			IList dataList = (IList)((field != null) ? field.GetValue(config) : null);
			Debug.Assert(dataList != null, "dataList != null");
			dataList.Add(item);
		}

		// Token: 0x0600BDDB RID: 48603 RVA: 0x005632B8 File Offset: 0x005614B8
		public static void AppendConfigRange<TConfig, TItem>(this TConfig config, IEnumerable<TItem> items) where TConfig : IEnumerable<TItem>, IConfigData
		{
			Type configType = config.GetType();
			FieldInfo field = configType.GetField("_dataArray", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			List<TItem> dataList = (List<TItem>)((field != null) ? field.GetValue(config) : null);
			Debug.Assert(dataList != null, "dataList != null");
			dataList.AddRange(items);
		}

		// Token: 0x0600BDDC RID: 48604 RVA: 0x00563310 File Offset: 0x00561510
		public static TItem GetConfigItem<TConfig, TItem>(this TConfig config, string refName) where TConfig : IEnumerable<TItem>, IConfigData
		{
			int templateId = config.GetItemId(refName);
			return config.GetConfigItem(templateId);
		}

		// Token: 0x0600BDDD RID: 48605 RVA: 0x00563338 File Offset: 0x00561538
		public static object GetConfigItem<TConfig>(this TConfig config, string refName) where TConfig : IConfigData
		{
			int templateId = config.GetItemId(refName);
			return config.GetConfigItem(templateId);
		}

		// Token: 0x0600BDDE RID: 48606 RVA: 0x00563360 File Offset: 0x00561560
		public static TItem GetConfigItem<TConfig, TItem>(this TConfig config, int templateId) where TConfig : IEnumerable<TItem>, IConfigData
		{
			Type configType = config.GetType();
			FieldInfo field = configType.GetField("_dataArray", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			List<TItem> dataList = (List<TItem>)((field != null) ? field.GetValue(config) : null);
			Debug.Assert(dataList != null, "dataList != null");
			return dataList[templateId];
		}

		// Token: 0x0600BDDF RID: 48607 RVA: 0x005633BC File Offset: 0x005615BC
		public static object GetConfigItem<TConfig>(this TConfig config, int templateId) where TConfig : IConfigData
		{
			Type configType = config.GetType();
			FieldInfo field = configType.GetField("_dataArray", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			IList dataList = (IList)((field != null) ? field.GetValue(config) : null);
			Debug.Assert(dataList != null, "dataList != null");
			return dataList[templateId];
		}

		// Token: 0x0600BDE0 RID: 48608 RVA: 0x00563418 File Offset: 0x00561618
		public static int GetConfigCount<TConfig>(this TConfig config) where TConfig : IConfigData
		{
			FieldInfo field = config.GetType().GetField("_dataArray", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			IList dataList = (IList)((field != null) ? field.GetValue(config) : null);
			return (dataList != null) ? dataList.Count : 0;
		}

		// Token: 0x0600BDE1 RID: 48609 RVA: 0x00563468 File Offset: 0x00561668
		public static object GetConfigPropertyValue(string typeName, int templateId, string propertyName)
		{
			return ConfigCollection.NameMap[typeName].GetConfigItem(templateId).GetFieldValue(propertyName);
		}

		// Token: 0x0600BDE2 RID: 48610 RVA: 0x00563494 File Offset: 0x00561694
		public static void ModifyConfigObjectPropertyValue<TItem>(TItem config, string propertyName, string propertyTypeName, object[] parameters)
		{
			Type type = Assembly.GetExecutingAssembly().GetType(propertyTypeName);
			bool flag = type == null;
			if (!flag)
			{
				object propertyVal = Activator.CreateInstance(type, parameters);
				config.ModifyField(propertyName, propertyVal);
			}
		}

		// Token: 0x0600BDE3 RID: 48611 RVA: 0x005634CC File Offset: 0x005616CC
		[Obsolete("Use Config.ConfigCollection.NameMap[string] instead.")]
		public static IConfigData GetConfigData(string configTypeName)
		{
			return ConfigCollection.NameMap[configTypeName];
		}

		// Token: 0x0600BDE4 RID: 48612 RVA: 0x005634EC File Offset: 0x005616EC
		public static object CreateDeepCopy(this object obj)
		{
			int offset = ConfigDataModificationUtils.DataPool.GetWritingOffset();
			ConfigDataModificationUtils.BinaryFormatter.Serialize(ConfigDataModificationUtils.DataPool, obj);
			object result = ConfigDataModificationUtils.BinaryFormatter.Deserialize(ConfigDataModificationUtils.DataPool);
			ConfigDataModificationUtils.DataPool.SetStreamReadingOffset(offset);
			ConfigDataModificationUtils.DataPool.Clear();
			return result;
		}

		// Token: 0x0400921A RID: 37402
		private static readonly RawDataPool DataPool = new RawDataPool(1024);

		// Token: 0x0400921B RID: 37403
		private static readonly IFormatter BinaryFormatter = new BinaryFormatter
		{
			AssemblyFormat = FormatterAssemblyStyle.Simple
		};
	}
}
