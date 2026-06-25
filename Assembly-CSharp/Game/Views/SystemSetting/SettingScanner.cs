using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Game.Views.SystemSetting
{
	// Token: 0x02000781 RID: 1921
	public static class SettingScanner
	{
		// Token: 0x06005C61 RID: 23649 RVA: 0x002AB8EC File Offset: 0x002A9AEC
		public static Dictionary<ESettingSubCategory, List<ISettingItemInfo>> ScanSettings(SystemSettingMapping dataInstance)
		{
			Dictionary<ESettingSubCategory, List<ISettingItemInfo>> results = new Dictionary<ESettingSubCategory, List<ISettingItemInfo>>();
			Type type = dataInstance.GetType();
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			foreach (PropertyInfo prop in properties)
			{
				SettingItemBaseAttribute attr = prop.GetCustomAttribute<SettingItemBaseAttribute>();
				bool flag = attr == null;
				if (!flag)
				{
					ISettingItemInfo info = SettingScanner.CreateSettingItemInfo(attr, prop, dataInstance);
					bool flag2 = info != null;
					if (flag2)
					{
						List<ISettingItemInfo> list;
						bool flag3 = !results.TryGetValue(info.Attribute.SubCategory, out list);
						if (flag3)
						{
							results.Add(info.Attribute.SubCategory, new List<ISettingItemInfo>
							{
								info
							});
						}
						else
						{
							list.Add(info);
						}
					}
				}
			}
			return results;
		}

		// Token: 0x06005C62 RID: 23650 RVA: 0x002AB9B4 File Offset: 0x002A9BB4
		private static ISettingItemInfo CreateSettingItemInfo(SettingItemBaseAttribute attr, PropertyInfo prop, SystemSettingMapping target)
		{
			Type propertyType = prop.PropertyType;
			MethodInfo method = typeof(SettingScanner).GetMethod("CreateGenericInfo", BindingFlags.Static | BindingFlags.NonPublic);
			MethodInfo genericMethod = method.MakeGenericMethod(new Type[]
			{
				propertyType
			});
			return (ISettingItemInfo)genericMethod.Invoke(null, new object[]
			{
				attr,
				prop,
				target
			});
		}

		// Token: 0x06005C63 RID: 23651 RVA: 0x002ABA14 File Offset: 0x002A9C14
		private static SettingItemInfo<T> CreateGenericInfo<T>(SettingItemBaseAttribute attr, PropertyInfo prop, SystemSettingMapping target)
		{
			ConstantExpression param = Expression.Constant(target);
			MemberExpression propAccess = Expression.Property(param, prop);
			Func<T> getter = Expression.Lambda<Func<T>>(propAccess, Array.Empty<ParameterExpression>()).Compile();
			ParameterExpression valueParam = Expression.Parameter(typeof(T), "value");
			BinaryExpression assign = Expression.Assign(Expression.Property(param, prop), valueParam);
			Action<T> setter = Expression.Lambda<Action<T>>(assign, new ParameterExpression[]
			{
				valueParam
			}).Compile();
			return new SettingItemInfo<T>(attr, getter, setter);
		}
	}
}
