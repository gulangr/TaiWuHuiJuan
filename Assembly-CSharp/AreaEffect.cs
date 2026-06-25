using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using GameData.Utilities;
using UnityEngine;

// Token: 0x020003BD RID: 957
public class AreaEffect : Refers
{
	// Token: 0x06003A10 RID: 14864 RVA: 0x001D9338 File Offset: 0x001D7538
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

	// Token: 0x06003A11 RID: 14865 RVA: 0x001D93D0 File Offset: 0x001D75D0
	private void RefreshObj()
	{
		foreach (KeyValuePair<sbyte, GameObject> pair in this._effectObjDict)
		{
			pair.Value.SetActive(pair.Key == this._activeType);
		}
		base.GetComponent<UIParticle>().RefreshParticles();
	}

	// Token: 0x040029DF RID: 10719
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

	// Token: 0x040029E0 RID: 10720
	private readonly Dictionary<sbyte, GameObject> _effectObjDict = new Dictionary<sbyte, GameObject>();

	// Token: 0x040029E1 RID: 10721
	private sbyte _activeType = -1;
}
