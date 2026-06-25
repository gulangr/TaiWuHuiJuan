using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using GameData.Domains.Building;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.MouseTips
{
	// Token: 0x0200083A RID: 2106
	public class MouseTipBuildingProduceCollectResource : MouseTipBase
	{
		// Token: 0x17000C5B RID: 3163
		// (get) Token: 0x060066B6 RID: 26294 RVA: 0x002ED795 File Offset: 0x002EB995
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060066B7 RID: 26295 RVA: 0x002ED798 File Offset: 0x002EB998
		protected override void Init(ArgumentBox argsBox)
		{
			argsBox.Get<BuildingManageYieldTipsData>("ProduceData", out this._data);
			ResourceTypeItem resourceTypeItem = ResourceType.Instance[this._data.ProduceResourceType];
			this.typeIcon.SetSprite(resourceTypeItem.Icon, false, null);
			this.type.text = resourceTypeItem.Name;
			this.amount.text = this._data.ResourceOutputValuation.ToString();
			KeyValuePair<BuildingBlockKey, BuildingProduceDependencyData>[] render = (from x in this._data.ProduceDependencies
			orderby x.Key.BlockId
			select x).ToArray<KeyValuePair<BuildingBlockKey, BuildingProduceDependencyData>>();
			this.grid.constraintCount = ((render.Length > 8) ? 3 : 2);
			this.back3.SetActive(render.Length > 2);
			GameObject gameObject = this.back9;
			int num = render.Length;
			gameObject.SetActive(num == 7 || num == 8 || num > 9);
			this.vert1.SetActive(render.Length > 1);
			this.vert2.SetActive(render.Length > 8);
			this.content.Rebuild<BuildingProduceCollectResourceCell>(render.Length, delegate(BuildingProduceCollectResourceCell cell, int i)
			{
				cell.Set(render[i]);
			});
		}

		// Token: 0x0400480D RID: 18445
		private BuildingManageYieldTipsData _data;

		// Token: 0x0400480E RID: 18446
		[SerializeField]
		private TMP_Text type;

		// Token: 0x0400480F RID: 18447
		[SerializeField]
		private TMP_Text amount;

		// Token: 0x04004810 RID: 18448
		[SerializeField]
		private CImage typeIcon;

		// Token: 0x04004811 RID: 18449
		[SerializeField]
		private GameObject back3;

		// Token: 0x04004812 RID: 18450
		[SerializeField]
		private GameObject back9;

		// Token: 0x04004813 RID: 18451
		[SerializeField]
		private GameObject vert1;

		// Token: 0x04004814 RID: 18452
		[SerializeField]
		private GameObject vert2;

		// Token: 0x04004815 RID: 18453
		[SerializeField]
		private TemplatedContainerAssemblyNew content;

		// Token: 0x04004816 RID: 18454
		[SerializeField]
		private GridLayoutGroup grid;
	}
}
