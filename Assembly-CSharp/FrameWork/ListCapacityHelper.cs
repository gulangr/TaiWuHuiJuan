using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace FrameWork
{
	// Token: 0x02000FE4 RID: 4068
	public static class ListCapacityHelper
	{
		// Token: 0x0600B9DE RID: 47582 RVA: 0x0054AB2C File Offset: 0x00548D2C
		public static int GetCapacity(object list)
		{
			bool flag = list == null;
			if (flag)
			{
				throw new ArgumentNullException("list");
			}
			Type type = list.GetType();
			bool flag2 = !type.IsGenericType || type.GetGenericTypeDefinition() != typeof(List<>);
			if (flag2)
			{
				throw new ArgumentException("The provided object is not a List<T>.", "list");
			}
			Func<object, int> capacityDelegate;
			bool flag3 = !ListCapacityHelper.CachedDelegates.TryGetValue(type, out capacityDelegate);
			if (flag3)
			{
				ParameterExpression listParam = Expression.Parameter(typeof(object), "list");
				UnaryExpression castList = Expression.Convert(listParam, type);
				PropertyInfo capacityProperty = type.GetProperty("Capacity");
				MemberExpression propertyAccess = Expression.Property(castList, capacityProperty);
				Expression<Func<object, int>> lambda = Expression.Lambda<Func<object, int>>(propertyAccess, new ParameterExpression[]
				{
					listParam
				});
				capacityDelegate = lambda.Compile();
				ListCapacityHelper.CachedDelegates[type] = capacityDelegate;
			}
			return capacityDelegate(list);
		}

		// Token: 0x04008FBD RID: 36797
		private static readonly Dictionary<Type, Func<object, int>> CachedDelegates = new Dictionary<Type, Func<object, int>>();
	}
}
