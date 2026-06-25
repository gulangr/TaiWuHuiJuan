using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameData.Adventure;
using GameData.Domains.Adventure;
using UnityEngine;

namespace Game.Views.Adventure
{
	// Token: 0x02000C72 RID: 3186
	public class AdventureTaiwuStateLine : MonoBehaviour
	{
		// Token: 0x0600A1F9 RID: 41465 RVA: 0x004BB7DC File Offset: 0x004B99DC
		public void SetValue([TupleElementNames(new string[]
		{
			"parameterData",
			"parameterValue"
		})] List<ValueTuple<AdventureParameterData, AdventureParameterValue>> paramList)
		{
			for (int i = 0; i < this.items.Length; i++)
			{
				AdventureTaiwuStateItem item = this.items[i];
				bool flag = i < paramList.Count;
				if (flag)
				{
					ValueTuple<AdventureParameterData, AdventureParameterValue> data = paramList[i];
					item.SetValue(data.Item1, data.Item2);
					item.gameObject.SetActive(true);
				}
				else
				{
					item.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x04007DF4 RID: 32244
		[SerializeField]
		private AdventureTaiwuStateItem[] items;
	}
}
