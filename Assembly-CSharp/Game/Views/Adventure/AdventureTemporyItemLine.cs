using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Item;
using Game.Components.ListStyleGeneralScroll.Item;
using GameData.Domains.Adventure;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Adventure
{
	// Token: 0x02000C73 RID: 3187
	public class AdventureTemporyItemLine : MonoBehaviour
	{
		// Token: 0x0600A1FB RID: 41467 RVA: 0x004BB860 File Offset: 0x004B9A60
		public void RefreshDisplay(List<ItemDisplayData> itemDisplayDatas)
		{
			this.templatedContainer.Rebuild<RectTransform>(itemDisplayDatas.Count, delegate(RectTransform refer, int index)
			{
				ItemDisplayData itemDisplayData = itemDisplayDatas[index];
				refer.GetComponent<CButton>().onClick.ResetListener(delegate()
				{
					AdventureDomainMethod.AsyncCall.UseTemporaryItem(null, itemDisplayData.Key, delegate(int offset, RawDataPool dataPool)
					{
						bool result = false;
						Serializer.Deserialize(dataPool, offset, ref result);
					});
				});
				ItemBack itemBack = refer.GetComponent<ItemBack>();
				itemBack.Set(itemDisplayData, false);
				RowItemLine.SetMouseTipDisplayer(true, itemDisplayData, refer.GetComponent<TooltipInvoker>());
				itemBack.SetCount(itemDisplayData.Amount, false);
			});
			LayoutRebuilder.ForceRebuildLayoutImmediate(base.gameObject.GetComponent<RectTransform>());
		}

		// Token: 0x04007DF5 RID: 32245
		[SerializeField]
		private TemplatedContainerAssemblyNew templatedContainer;

		// Token: 0x04007DF6 RID: 32246
		public const int LineCount = 5;
	}
}
