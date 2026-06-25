using System;
using Config;
using FrameWork.UISystem.UIElements;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Building.BuildingManage.Production
{
	// Token: 0x02000C1C RID: 3100
	public class ProductionResourceItem : MonoBehaviour
	{
		// Token: 0x06009D87 RID: 40327 RVA: 0x0049C520 File Offset: 0x0049A720
		public void Set(sbyte resourceType, int current, int add)
		{
			this.icon.SetSprite("ui9_btn_resource_bar_{0}_0".GetFormat(resourceType.ToString()), false, null);
			this.nameText.text = ResourceType.Instance[resourceType].Name;
			this.valueText.text = current.ToString() + "+" + add.ToString().SetColor("brightblue");
		}

		// Token: 0x040079F7 RID: 31223
		public CToggle toggle;

		// Token: 0x040079F8 RID: 31224
		[SerializeField]
		private CImage icon;

		// Token: 0x040079F9 RID: 31225
		[SerializeField]
		private TextMeshProUGUI nameText;

		// Token: 0x040079FA RID: 31226
		[SerializeField]
		private TextMeshProUGUI valueText;
	}
}
