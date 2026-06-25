using System;
using System.IO;
using Config;
using Game.Views.Building.BuildingManage;
using TMPro;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EB9 RID: 3769
	public class ChickenCell : MonoBehaviour, ICellContent<ChickenData>, ICellContent
	{
		// Token: 0x0600AEE2 RID: 44770 RVA: 0x004FAC0C File Offset: 0x004F8E0C
		public void SetData(ChickenData data)
		{
			Config.ChickenItem chickenItem = Chicken.Instance.GetItem(data.TemplateId);
			ResLoader.Load<Sprite>(Path.Combine("RemakeResources/Textures/Chicken", chickenItem.Display), delegate(Sprite sprite)
			{
				this.icon.sprite = sprite;
				this.icon.enabled = true;
			}, null, false);
			this.chickenName.text = data.Name;
			this.gradeBack.color = Colors.Instance.GradeColors[(int)chickenItem.Grade];
		}

		// Token: 0x0400873E RID: 34622
		[SerializeField]
		private CImage gradeBack;

		// Token: 0x0400873F RID: 34623
		[SerializeField]
		private CImage icon;

		// Token: 0x04008740 RID: 34624
		[SerializeField]
		private TextMeshProUGUI chickenName;
	}
}
