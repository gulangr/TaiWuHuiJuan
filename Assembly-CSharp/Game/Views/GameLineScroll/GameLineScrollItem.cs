using System;
using Config;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.GameLineScroll
{
	// Token: 0x02000A19 RID: 2585
	public class GameLineScrollItem : MonoBehaviour
	{
		// Token: 0x17000DBC RID: 3516
		// (get) Token: 0x06007EAE RID: 32430 RVA: 0x003AF134 File Offset: 0x003AD334
		public TextMeshProUGUI ScrollName
		{
			get
			{
				return this.scrollName;
			}
		}

		// Token: 0x17000DBD RID: 3517
		// (get) Token: 0x06007EAF RID: 32431 RVA: 0x003AF13C File Offset: 0x003AD33C
		public CButton ScrollBtn
		{
			get
			{
				return this.scrollBtn;
			}
		}

		// Token: 0x06007EB0 RID: 32432 RVA: 0x003AF144 File Offset: 0x003AD344
		public void Set(short scrollTemplateId, bool isUnlock, bool isAllUnlock)
		{
			this.Index = (int)scrollTemplateId;
			string imgBase = "ui9_btn_game_line_scroll_boss" + string.Format("_{0}_", scrollTemplateId);
			string imgName = imgBase + "0";
			this.scrollImg.SetSprite(imgName, false, null);
			CharacterItem charConfig = Character.Instance[Boss.Instance[(int)scrollTemplateId].CharacterIdList[0]];
			string charName = LocalStringManager.GetFormat(LanguageKey.LK_CharacterName_Display_Format, charConfig.Surname ?? string.Empty, charConfig.GivenName);
			this.scrollName.text = charName;
			this.scrollBtn.interactable = isUnlock;
			ResLoader.Load<Sprite>("RemakeResources/UIGraphics5.0/Ui9GameLineScroll/" + imgBase + "1", delegate(Sprite hoverSprite)
			{
				this.scrollBtn.spriteState = new SpriteState
				{
					highlightedSprite = hoverSprite,
					pressedSprite = hoverSprite,
					selectedSprite = hoverSprite,
					disabledSprite = this.scrollBtn.spriteState.disabledSprite
				};
			}, null, false);
			this.statusTipDisplayer.PresetParam[0] = (isUnlock ? (isAllUnlock ? LanguageKey.LK_Scroll_Tip_GameLine_3.TrFormat(charName) : LanguageKey.LK_Scroll_Tip_GameLine_2.TrFormat(charName)) : LanguageKey.LK_Scroll_Tip_GameLine_1.TrFormat(charName));
		}

		// Token: 0x06007EB1 RID: 32433 RVA: 0x003AF242 File Offset: 0x003AD442
		public void SetScrollBtn(bool enable)
		{
			this.scrollBtn.enabled = enable;
			this.statusTipDisplayer.enabled = enable;
		}

		// Token: 0x040060B9 RID: 24761
		[SerializeField]
		private CImage scrollImg;

		// Token: 0x040060BA RID: 24762
		[SerializeField]
		private TextMeshProUGUI scrollName;

		// Token: 0x040060BB RID: 24763
		[SerializeField]
		private CButton scrollBtn;

		// Token: 0x040060BC RID: 24764
		[SerializeField]
		private TooltipInvoker statusTipDisplayer;

		// Token: 0x040060BD RID: 24765
		public int Index;
	}
}
