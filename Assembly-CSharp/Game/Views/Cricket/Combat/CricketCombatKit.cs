using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Combat.Cricket;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using UnityEngine;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000AC9 RID: 2761
	public static class CricketCombatKit
	{
		// Token: 0x06008821 RID: 34849 RVA: 0x003F3104 File Offset: 0x003F1304
		static CricketCombatKit()
		{
			CricketExternalBridge.Initialize(new CricketCombatBridge());
		}

		// Token: 0x06008822 RID: 34850 RVA: 0x003F311C File Offset: 0x003F131C
		public static string GetCricketName(ItemDisplayData displayData)
		{
			ValueTuple<short, short> tuple = new ValueTuple<short, short>(displayData.CricketColorId, displayData.CricketPartId);
			return tuple.CalcCricketName().SetGradeColor((int)tuple.CalcCricketGrade());
		}

		// Token: 0x06008823 RID: 34851 RVA: 0x003F3154 File Offset: 0x003F1354
		public static string WrapProperty(int property, int injury)
		{
			string propertyText = Mathf.Max(property, 0).ToString();
			return (injury > 0) ? propertyText.SetColor("brightred") : propertyText;
		}

		// Token: 0x06008824 RID: 34852 RVA: 0x003F318C File Offset: 0x003F138C
		public static int SumCricketGrades(IEnumerable<ItemDisplayData> crickets)
		{
			return (from cricket in crickets
			let color = CricketParts.Instance[cricket.CricketColorId]
			select new
			{
				<>h__TransparentIdentifier0 = <>h__TransparentIdentifier0,
				part = CricketParts.Instance[cricket.CricketPartId]
			}).Select(delegate(<>h__TransparentIdentifier1)
			{
				int level = (int)<>h__TransparentIdentifier1.<>h__TransparentIdentifier0.color.Level;
				CricketPartsItem part = <>h__TransparentIdentifier1.part;
				return Mathf.Max(level, (int)((part != null) ? part.Level : 0));
			}).Sum();
		}

		// Token: 0x06008825 RID: 34853 RVA: 0x003F3210 File Offset: 0x003F1410
		public static void DelayCallRealTime(MonoBehaviour mono, Action action, float delay)
		{
			bool activeInHierarchy = mono.gameObject.activeInHierarchy;
			if (activeInHierarchy)
			{
				mono.StartCoroutine(CricketCombatKit.CoDelayCallRealTime(action, delay));
			}
		}

		// Token: 0x06008826 RID: 34854 RVA: 0x003F323B File Offset: 0x003F143B
		private static IEnumerator CoDelayCallRealTime(Action action, float delay)
		{
			yield return new WaitForSecondsRealtime(delay);
			if (action != null)
			{
				action();
			}
			yield break;
		}

		// Token: 0x0400685B RID: 26715
		public const sbyte TotalRoundCount = 3;

		// Token: 0x0400685C RID: 26716
		public static readonly CricketCombatBlackBoard Board = new CricketCombatBlackBoard();
	}
}
