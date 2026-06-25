using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using GameData.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.World
{
	// Token: 0x02000724 RID: 1828
	public class AreaEffect : MonoBehaviour, ILayoutIgnorer
	{
		// Token: 0x0600575F RID: 22367 RVA: 0x002894B4 File Offset: 0x002876B4
		public void Refresh(BoolArray8 loongStatus)
		{
			this._activeType = -1;
			for (sbyte i = 0; i < 5; i += 1)
			{
				bool flag = loongStatus[(int)i];
				if (flag)
				{
					this._activeType = i;
					break;
				}
			}
			this.RefreshObj();
			bool flag2 = this._activeType < 0;
			if (!flag2)
			{
				bool flag3 = !this._effectObjDict.ContainsKey(this._activeType);
				if (flag3)
				{
					string name = this._effectNameDict[this._activeType];
					ResLoader.LoadByName<GameObject>(name, delegate(GameObject prefab)
					{
						GameObject obj = Object.Instantiate<GameObject>(prefab, base.transform);
						this._effectObjDict[this._activeType] = obj;
						obj.transform.localPosition = Vector3.zero;
						this.RefreshObj();
					}, null);
				}
			}
		}

		// Token: 0x06005760 RID: 22368 RVA: 0x0028954C File Offset: 0x0028774C
		private void RefreshObj()
		{
			foreach (KeyValuePair<sbyte, GameObject> pair in this._effectObjDict)
			{
				pair.Value.SetActive(pair.Key == this._activeType);
			}
			base.GetComponent<UIParticle>().RefreshParticles();
		}

		// Token: 0x17000A87 RID: 2695
		// (get) Token: 0x06005761 RID: 22369 RVA: 0x002895C4 File Offset: 0x002877C4
		public bool ignoreLayout
		{
			get
			{
				return true;
			}
		}

		// Token: 0x04003BDB RID: 15323
		private readonly Dictionary<sbyte, string> _effectNameDict = new Dictionary<sbyte, string>
		{
			{
				3,
				"eff_PartWorldMap_Area_LoongTrace_huo_temp"
			},
			{
				0,
				"eff_PartWorldMap_Area_LoongTrace_jin_temp"
			},
			{
				1,
				"eff_PartWorldMap_Area_LoongTrace_mu_temp"
			},
			{
				2,
				"eff_PartWorldMap_Area_LoongTrace_shui_temp"
			},
			{
				4,
				"eff_PartWorldMap_Area_LoongTrace_tu_temp"
			}
		};

		// Token: 0x04003BDC RID: 15324
		private readonly Dictionary<sbyte, GameObject> _effectObjDict = new Dictionary<sbyte, GameObject>();

		// Token: 0x04003BDD RID: 15325
		private sbyte _activeType = -1;
	}
}
