using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B8A RID: 2954
	public static class SkillBreakCellPool
	{
		// Token: 0x060091DA RID: 37338 RVA: 0x0043EAAC File Offset: 0x0043CCAC
		public static bool TryGetCell(Type cellType, out SkillBreakCellBase cell)
		{
			List<SkillBreakCellBase> cells;
			bool flag = SkillBreakCellPool.Pool.TryGetValue(cellType, out cells) && cells.Count > 0;
			bool result;
			if (flag)
			{
				cell = cells[0];
				cells.RemoveAt(0);
				cell.gameObject.SetActive(true);
				result = true;
			}
			else
			{
				cell = null;
				result = false;
			}
			return result;
		}

		// Token: 0x060091DB RID: 37339 RVA: 0x0043EB08 File Offset: 0x0043CD08
		public static void RecycleCell(SkillBreakCellBase cell)
		{
			bool flag = cell == null;
			if (!flag)
			{
				Type cellType = cell.GetType();
				bool flag2 = !SkillBreakCellPool.Pool.ContainsKey(cellType);
				if (flag2)
				{
					SkillBreakCellPool.Pool[cellType] = new List<SkillBreakCellBase>();
				}
				cell.gameObject.SetActive(false);
				SkillBreakCellPool.Pool[cellType].Add(cell);
				cell.transform.SetParent(null, true);
			}
		}

		// Token: 0x060091DC RID: 37340 RVA: 0x0043EB7C File Offset: 0x0043CD7C
		public static void ClearPool()
		{
			foreach (List<SkillBreakCellBase> cellList in SkillBreakCellPool.Pool.Values)
			{
				foreach (SkillBreakCellBase cell in cellList)
				{
					bool flag = cell != null;
					if (flag)
					{
						Object.Destroy(cell.gameObject);
					}
				}
				cellList.Clear();
			}
			SkillBreakCellPool.Pool.Clear();
		}

		// Token: 0x0400705E RID: 28766
		private static readonly Dictionary<Type, List<SkillBreakCellBase>> Pool = new Dictionary<Type, List<SkillBreakCellBase>>();
	}
}
