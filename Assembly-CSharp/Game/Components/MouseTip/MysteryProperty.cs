using System;
using System.Collections.Generic;
using System.Text;
using Config;
using Config.ConfigCells;
using FrameWork;
using GameData.Combat.Math;
using TMPro;
using UnityEngine;

namespace Game.Components.MouseTip
{
	// Token: 0x02000E9D RID: 3741
	public class MysteryProperty : MonoBehaviour
	{
		// Token: 0x0600AD77 RID: 44407 RVA: 0x004F212C File Offset: 0x004F032C
		public void RefreshInfo(MysteryEffectItem config, int index, short requirementsPower)
		{
			this.indexIcon.SetSprite("ui9_icon_mouse_tip_mystery_level_" + index.ToString(), false, null);
			int powerRequirement = config.PowerRequirements[index];
			this.disableStyleRoot.SetStyleEffect(powerRequirement > (int)requirementsPower, false);
			this.powerRequirements.SetText(powerRequirement.ToString() + "%", true);
			List<PropertyAndValueAndModifyType> propertyList = config.BonusValues[index];
			StringBuilder propertyDescText = EasyPool.Get<StringBuilder>();
			for (int i = 0; i < propertyList.Count; i++)
			{
				PropertyAndValueAndModifyType property = propertyList[i];
				CharacterPropertyReferencedItem propertyReferencedItem = CharacterPropertyReferenced.Instance[(int)property.Type];
				CharacterPropertyDisplayItem propertyDisplayItem = CharacterPropertyDisplay.Instance[propertyReferencedItem.DisplayType];
				propertyDescText.Append("<SpName=" + propertyDisplayItem.TipsBigIcon + ">");
				string desc = propertyDisplayItem.Name + "：+" + property.Value.ToString() + ((property.Modify == EDataModifyType.AddPercent) ? "%" : "");
				propertyDescText.Append(desc);
				propertyDescText.Append("  ");
			}
			string text = propertyDescText.ToString();
			bool flag = text != this._cacheText;
			if (flag)
			{
				this._cacheText = text;
				this.propertyDesc.SetText(text, true);
				this.propertyDesc.transform.GetComponent<TMPTextSpriteHelper>().Parse();
			}
			EasyPool.Free<StringBuilder>(propertyDescText);
		}

		// Token: 0x040085FB RID: 34299
		[SerializeField]
		private CImage indexIcon;

		// Token: 0x040085FC RID: 34300
		[SerializeField]
		private TextMeshProUGUI powerRequirements;

		// Token: 0x040085FD RID: 34301
		[SerializeField]
		private TextMeshProUGUI propertyDesc;

		// Token: 0x040085FE RID: 34302
		[SerializeField]
		private DisableStyleRoot disableStyleRoot;

		// Token: 0x040085FF RID: 34303
		private string _cacheText;
	}
}
