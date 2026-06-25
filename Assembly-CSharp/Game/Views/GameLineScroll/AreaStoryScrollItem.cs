using System;
using Config;
using FrameWork.UISystem.UIElements;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.GameLineScroll
{
	// Token: 0x02000A18 RID: 2584
	public class AreaStoryScrollItem : MonoBehaviour
	{
		// Token: 0x06007EAA RID: 32426 RVA: 0x003AEFD4 File Offset: 0x003AD1D4
		public void Set(int index, string areaStoryName, bool interactable, Action onClick)
		{
			this.scrollName.text = areaStoryName;
			this.scrollBtn.interactable = interactable;
			this.scrollBtn.ClearAndAddListener(onClick);
			this.scrollBtn.GetComponent<CImage>().SetSprite("ui9_btn_area_sect_scroll_{0}_{1}".GetFormat(index, 0), false, null);
			AtlasInfo.Instance.GetSprite("ui9_btn_area_sect_scroll_{0}_{1}".GetFormat(index, 1), delegate(Sprite sprite)
			{
				SpriteState state = this.scrollBtn.spriteState;
				state.highlightedSprite = sprite;
				state.pressedSprite = sprite;
				state.selectedSprite = sprite;
				this.scrollBtn.spriteState = state;
			});
			AtlasInfo.Instance.GetSprite("ui9_btn_area_sect_scroll_{0}_{1}".GetFormat(index, 3), delegate(Sprite sprite)
			{
				SpriteState state = this.scrollBtn.spriteState;
				state.disabledSprite = sprite;
				this.scrollBtn.spriteState = state;
			});
			this.tipDisplayer.PresetParam[0] = LanguageKey.LK_Scroll_Tip_AreaStory_1.TrFormat(Organization.Instance[index + 1].Name);
		}

		// Token: 0x040060B6 RID: 24758
		[SerializeField]
		private TextMeshProUGUI scrollName;

		// Token: 0x040060B7 RID: 24759
		[SerializeField]
		private CButton scrollBtn;

		// Token: 0x040060B8 RID: 24760
		[SerializeField]
		private TooltipInvoker tipDisplayer;
	}
}
