using System;
using System.Collections.Generic;
using FrameWork;
using Game.Components.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F21 RID: 3873
	public class Fame : MonoBehaviour
	{
		// Token: 0x0600B25B RID: 45659 RVA: 0x00512C28 File Offset: 0x00510E28
		public void Set(CharacterDisplayData data, bool isTaiwu)
		{
			bool flag = data == null;
			if (flag)
			{
				this.SetEmpty();
			}
			else
			{
				this._currentData = data;
				this._currentFameRecords = null;
				this.Set(data.FameType, isTaiwu);
			}
		}

		// Token: 0x0600B25C RID: 45660 RVA: 0x00512C64 File Offset: 0x00510E64
		public void Set(CharacterMenuInfoDisplayData menuData, bool isShowBack = true)
		{
			bool flag = ((menuData != null) ? menuData.CharacterDisplayData : null) == null;
			if (flag)
			{
				this.SetEmpty();
			}
			else
			{
				this._currentData = menuData.CharacterDisplayData;
				this._currentFameRecords = menuData.FameActionRecords;
				this.Set(menuData.CharacterDisplayData.FameType, menuData.IsTaiwu);
				this.propertyItem.SetShowBack(isShowBack);
			}
		}

		// Token: 0x0600B25D RID: 45661 RVA: 0x00512CCC File Offset: 0x00510ECC
		public void Set(sbyte fameType, bool isTaiwu)
		{
			bool flag = this.propertyItem != null;
			if (flag)
			{
				int iconIndex = CommonUtils.GetFameIconIndex(fameType);
				string text = CommonUtils.GetFameString(fameType);
				this.propertyItem.Set(this.sprites[iconIndex], LanguageKey.LK_Main_SummaryInfo_Fame.Tr(), text, null, false);
			}
			this.RefreshTip(isTaiwu);
		}

		// Token: 0x0600B25E RID: 45662 RVA: 0x00512D2C File Offset: 0x00510F2C
		public void SetEmpty()
		{
			bool flag = this.propertyItem != null;
			if (flag)
			{
				this.propertyItem.Set(string.Empty, string.Empty, string.Empty, null, false);
			}
			bool flag2 = this.mouseTip != null;
			if (flag2)
			{
				this.mouseTip.enabled = false;
			}
		}

		// Token: 0x0600B25F RID: 45663 RVA: 0x00512D8C File Offset: 0x00510F8C
		private void RefreshTip(bool isTaiwu)
		{
			bool flag = this.mouseTip == null;
			if (!flag)
			{
				this.mouseTip.enabled = true;
				this.mouseTip.IsLanguageKey = false;
				this.mouseTip.Type = TipType.Fame;
				TooltipInvoker tooltipInvoker = this.mouseTip;
				ArgumentBox argumentBox;
				if ((argumentBox = tooltipInvoker.RuntimeParam) == null)
				{
					argumentBox = (tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>());
				}
				string key = "FeatureIds";
				CharacterDisplayData currentData = this._currentData;
				IReadOnlyList<short> readOnlyList = (currentData != null) ? currentData.FeatureIds : null;
				ArgumentBox argumentBox2 = argumentBox.SetObject(key, readOnlyList ?? Array.Empty<short>());
				string key2 = "FameRecords";
				IReadOnlyList<FameActionRecord> currentFameRecords = this._currentFameRecords;
				ArgumentBox argumentBox3 = argumentBox2.SetObject(key2, currentFameRecords ?? Array.Empty<FameActionRecord>());
				string key3 = "OrgInfo";
				CharacterDisplayData currentData2 = this._currentData;
				argumentBox3.SetObject(key3, (currentData2 != null) ? currentData2.OrgInfo : default(OrganizationInfo)).Set("IsTaiwu", isTaiwu);
			}
		}

		// Token: 0x04008A5F RID: 35423
		[SerializeField]
		private PropertyItem propertyItem;

		// Token: 0x04008A60 RID: 35424
		[SerializeField]
		private TooltipInvoker mouseTip;

		// Token: 0x04008A61 RID: 35425
		[Header("名誉图标")]
		[SerializeField]
		private Sprite[] sprites;

		// Token: 0x04008A62 RID: 35426
		private CharacterDisplayData _currentData;

		// Token: 0x04008A63 RID: 35427
		private List<FameActionRecord> _currentFameRecords;
	}
}
