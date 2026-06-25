using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000035 RID: 53
[Serializable]
public class Refers : RefersBase
{
	// Token: 0x060001D8 RID: 472 RVA: 0x0000B96C File Offset: 0x00009B6C
	public override Object LGet(string aName, bool logEnable = true)
	{
		Object baseResult = base.LGet(aName, false);
		bool flag = baseResult;
		Object result;
		if (flag)
		{
			result = baseResult;
		}
		else
		{
			bool namesContains = this.Names.Contains(aName);
			bool flag2 = this == null || base.gameObject == null;
			if (flag2)
			{
				result = null;
			}
			else
			{
				base.GetComponents<ExtraRefers>(Refers.ReuseExtraRefers);
				foreach (ExtraRefers extraRefer in Refers.ReuseExtraRefers)
				{
					Object extraResult = extraRefer.LGet(aName, false);
					bool flag3 = extraResult;
					if (flag3)
					{
						return extraResult;
					}
				}
				bool flag4 = logEnable && !namesContains;
				if (flag4)
				{
					GLog.Error(aName + " does not exists in Refers name list!:" + base.name);
				}
				result = null;
			}
		}
		return result;
	}

	// Token: 0x060001D9 RID: 473 RVA: 0x0000BA60 File Offset: 0x00009C60
	public override bool CTryGet<T>(string aName, out T t)
	{
		bool flag = base.CTryGet<T>(aName, out t);
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			ExtraRefers[] extraRefers = base.GetComponents<ExtraRefers>();
			foreach (ExtraRefers extraRefer in extraRefers)
			{
				bool flag2 = extraRefer.CTryGet<T>(aName, out t);
				if (flag2)
				{
					return true;
				}
			}
			result = false;
		}
		return result;
	}

	// Token: 0x060001DA RID: 474 RVA: 0x0000BABA File Offset: 0x00009CBA
	public override IEnumerable<T> CGetIter<T>(string aNamePre)
	{
		IEnumerable<T> baseResult = base.CGetIter<T>(aNamePre);
		bool flag = baseResult != null;
		if (flag)
		{
			foreach (T item in baseResult)
			{
				yield return item;
				item = default(T);
			}
			IEnumerator<T> enumerator = null;
		}
		ExtraRefers[] extraRefers = base.GetComponents<ExtraRefers>();
		foreach (ExtraRefers extraRefer in extraRefers)
		{
			IEnumerable<T> extraResult = extraRefer.CGetIter<T>(aNamePre);
			bool flag2 = extraResult == null;
			if (!flag2)
			{
				foreach (T item2 in extraResult)
				{
					yield return item2;
					item2 = default(T);
				}
				IEnumerator<T> enumerator2 = null;
				extraResult = null;
				extraRefer = null;
			}
		}
		ExtraRefers[] array = null;
		yield break;
		yield break;
	}

	// Token: 0x040000F0 RID: 240
	private static readonly List<ExtraRefers> ReuseExtraRefers = new List<ExtraRefers>();
}
