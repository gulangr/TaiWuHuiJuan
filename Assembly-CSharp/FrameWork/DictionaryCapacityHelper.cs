using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace FrameWork
{
	// Token: 0x02000FE5 RID: 4069
	public static class DictionaryCapacityHelper
	{
		// Token: 0x0600B9E0 RID: 47584 RVA: 0x0054AC1C File Offset: 0x00548E1C
		public static int GetCapacity(object dictionary)
		{
			bool flag = dictionary == null;
			if (flag)
			{
				throw new ArgumentNullException("dictionary");
			}
			Type type = dictionary.GetType();
			bool flag2 = !type.IsGenericType || type.GetGenericTypeDefinition() != typeof(Dictionary<, >);
			if (flag2)
			{
				throw new ArgumentException("The provided object is not a Dictionary<TKey, TValue>.", "dictionary");
			}
			Func<object, int> capacityDelegate;
			bool flag3 = !DictionaryCapacityHelper.CachedDelegates.TryGetValue(type, out capacityDelegate);
			if (flag3)
			{
				ParameterExpression dictParam = Expression.Parameter(typeof(object), "dictionary");
				UnaryExpression castDict = Expression.Convert(dictParam, type);
				FieldInfo entriesField = type.GetField("_entries", BindingFlags.Instance | BindingFlags.NonPublic);
				bool flag4 = entriesField == null;
				if (flag4)
				{
					throw new InvalidOperationException("The _entries field was not found.");
				}
				MemberExpression fieldAccess = Expression.Field(castDict, entriesField);
				BinaryExpression nullCheck = Expression.NotEqual(fieldAccess, Expression.Constant(null, entriesField.FieldType));
				UnaryExpression arrayLength = Expression.ArrayLength(fieldAccess);
				ConstantExpression defaultCapacity = Expression.Constant(0, typeof(int));
				ConditionalExpression capacityExpression = Expression.Condition(nullCheck, arrayLength, defaultCapacity);
				Expression<Func<object, int>> lambda = Expression.Lambda<Func<object, int>>(capacityExpression, new ParameterExpression[]
				{
					dictParam
				});
				capacityDelegate = lambda.Compile();
				DictionaryCapacityHelper.CachedDelegates[type] = capacityDelegate;
			}
			bool flag5 = capacityDelegate != null;
			int result;
			if (flag5)
			{
				result = capacityDelegate(dictionary);
			}
			else
			{
				Debug.LogWarning(string.Format("Dictionary cannot find delegate for {0}. returning 0.", type));
				result = 0;
			}
			return result;
		}

		// Token: 0x04008FBE RID: 36798
		private static readonly Dictionary<Type, Func<object, int>> CachedDelegates = new Dictionary<Type, Func<object, int>>();
	}
}
