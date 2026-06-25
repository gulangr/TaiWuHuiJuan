using System;
using Config;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.LegendaryBook
{
	// Token: 0x0200098D RID: 2445
	public class LegendaryBookUIItem : MonoBehaviour
	{
		// Token: 0x17000D40 RID: 3392
		// (get) Token: 0x060075AB RID: 30123 RVA: 0x0036CC10 File Offset: 0x0036AE10
		private string Icon_EmpthBook
		{
			get
			{
				return "ui9_back_legendbook_icon_unknowbook_0";
			}
		}

		// Token: 0x17000D41 RID: 3393
		// (get) Token: 0x060075AC RID: 30124 RVA: 0x0036CC17 File Offset: 0x0036AE17
		private Color color_nearlyShowText
		{
			get
			{
				return new Color(0.5529412f, 0.7647059f, 0.7647059f, 1f);
			}
		}

		// Token: 0x17000D42 RID: 3394
		// (get) Token: 0x060075AD RID: 30125 RVA: 0x0036CC34 File Offset: 0x0036AE34
		private Sprite selectedSprite
		{
			get
			{
				return this.btnMain.spriteState.selectedSprite;
			}
		}

		// Token: 0x17000D43 RID: 3395
		// (get) Token: 0x060075AE RID: 30126 RVA: 0x0036CC54 File Offset: 0x0036AE54
		private Sprite disableSprite
		{
			get
			{
				return this.btnMain.spriteState.disabledSprite;
			}
		}

		// Token: 0x060075AF RID: 30127 RVA: 0x0036CC74 File Offset: 0x0036AE74
		private void Awake()
		{
			this.btnMain.ClearAndAddListener(new Action(this.OnButtonClick));
		}

		// Token: 0x060075B0 RID: 30128 RVA: 0x0036CC8F File Offset: 0x0036AE8F
		public void Init(MiscItem miscConfig, string bookName)
		{
			this._miscConfig = miscConfig;
			this._bookName = bookName;
			this.hoverHintLayout.gameObject.SetActive(false);
		}

		// Token: 0x060075B1 RID: 30129 RVA: 0x0036CCB2 File Offset: 0x0036AEB2
		private void OnButtonClick()
		{
			Action onClick = this.OnClick;
			if (onClick != null)
			{
				onClick();
			}
		}

		// Token: 0x060075B2 RID: 30130 RVA: 0x0036CCC8 File Offset: 0x0036AEC8
		public void SetupNoShown(bool nearlyAppear = false)
		{
			this.mainIcon.sprite = this.notShownSprite;
			this.disableStyleRoot.SetStyleEffect(false, false);
			this.icon.SetSprite(this.Icon_EmpthBook, false, null);
			this.bookNameEnable.gameObject.SetActive(true);
			this.bookNameDisable.gameObject.SetActive(false);
			this.mouseTip.enabled = nearlyAppear;
			this.mouseTip.Type = TipType.SingleDesc;
			this.mouseTip.PresetParam = new string[]
			{
				nearlyAppear ? LanguageKey.LK_LegendaryBook_NearlyShow.Tr() : LanguageKey.LK_LegendaryBook_NotShown.Tr()
			};
			this.bookNameEnable.text = (nearlyAppear ? LanguageKey.LK_LegendaryBook_NearlyShow.Tr() : LanguageKey.LK_LegendaryBook_NotShown.Tr());
			this.bookNameEnable.color = (nearlyAppear ? this.color_nearlyShowText : this.color_notShowText);
			this.bookNameEnable.fontSize = 24f;
			this.btnMain.spriteState = default(SpriteState);
		}

		// Token: 0x060075B3 RID: 30131 RVA: 0x0036CDDC File Offset: 0x0036AFDC
		public void SetupNormal(bool interactable)
		{
			this.mainIcon.sprite = this.normalSprite;
			this.selectedImg.sprite = this.normalSelectedSprite;
			this.icon.SetSprite(this._miscConfig.Icon, false, null);
			this.disableStyleRoot.SetStyleEffect(false, false);
			int combatSkillType = (int)(this._miscConfig.TemplateId - 240);
			CombatSkillTypeItem combatSkillTypeConfig = CombatSkillType.Instance[combatSkillType];
			this.mouseTip.enabled = true;
			this.mouseTip.Type = TipType.Simple;
			this.mouseTip.PresetParam = new string[]
			{
				this._miscConfig.Name,
				string.Format("<SpName={0}{1}>{2}", "ui9_back_attainments_combat_3_", combatSkillTypeConfig.TemplateId, combatSkillTypeConfig.Name)
			};
			this.bookNameEnable.gameObject.SetActive(true);
			this.bookNameDisable.gameObject.SetActive(false);
			this.bookNameEnable.text = this._bookName;
			this.bookNameEnable.color = this.color_normalText;
			this.bookNameEnable.fontSize = 24f;
			this.btnMain.spriteState = new SpriteState
			{
				highlightedSprite = this.normalHighlighSprite,
				pressedSprite = this.normalHighlighSprite,
				selectedSprite = this.normalSelectedSprite
			};
		}

		// Token: 0x060075B4 RID: 30132 RVA: 0x0036CF48 File Offset: 0x0036B148
		public void SetupKeepByCorpses(bool interactable)
		{
			Debug.Log(string.Format("test SetupKeepByCorpses:{0}", this._miscConfig.TemplateId));
			this.mainIcon.sprite = this.corpsesSprite;
			this.selectedImg.sprite = this.corpsesHighlighSprite;
			this.icon.SetSprite(this._miscConfig.Icon, false, null);
			this.disableStyleRoot.SetStyleEffect(false, false);
			int combatSkillType = (int)(this._miscConfig.TemplateId - 240);
			CombatSkillTypeItem combatSkillTypeConfig = CombatSkillType.Instance[combatSkillType];
			this.mouseTip.enabled = true;
			this.mouseTip.Type = TipType.Simple;
			this.mouseTip.PresetParam = new string[]
			{
				this._miscConfig.Name,
				string.Format("<SpName={0}{1}>{2}", "ui9_back_attainments_combat_3_", combatSkillTypeConfig.TemplateId, combatSkillTypeConfig.Name)
			};
			this.bookNameEnable.gameObject.SetActive(true);
			this.bookNameDisable.gameObject.SetActive(true);
			this.bookNameEnable.text = this._bookName;
			this.bookNameEnable.color = this.color_normalText;
			this.bookNameEnable.fontSize = 24f;
			this.bookNameDisable.text = LanguageKey.LK_ItemDisplayData_ThreeCorpseKeepingLegendaryBook.Tr();
			this.bookNameDisable.color = this.color_corpses;
			this.bookNameDisable.fontSize = 22f;
			this.btnMain.spriteState = new SpriteState
			{
				highlightedSprite = this.corpsesHighlighSprite,
				pressedSprite = this.corpsesHighlighSprite,
				selectedSprite = this.corpsesHighlighSprite
			};
		}

		// Token: 0x060075B5 RID: 30133 RVA: 0x0036D10C File Offset: 0x0036B30C
		public void SetupOwnedByOthers()
		{
			this.mainIcon.sprite = this.otherSprite;
			this.selectedImg.sprite = this.otherHighlighSprite;
			this.icon.SetSprite(this._miscConfig.Icon, false, null);
			this.disableStyleRoot.SetStyleEffect(false, false);
			this.btnMain.spriteState = new SpriteState
			{
				highlightedSprite = this.otherHighlighSprite,
				pressedSprite = this.otherHighlighSprite,
				selectedSprite = this.otherHighlighSprite
			};
			this.bookNameEnable.gameObject.SetActive(true);
			this.bookNameDisable.gameObject.SetActive(true);
			this.bookNameEnable.text = this._bookName;
			this.bookNameEnable.color = this.color_normalText;
			this.bookNameEnable.fontSize = 24f;
			this.bookNameDisable.text = LanguageKey.LK_LegendaryBook_OwnByOthers.Tr();
			this.bookNameDisable.color = this.color_ownByOthers;
			this.bookNameDisable.fontSize = 22f;
		}

		// Token: 0x060075B6 RID: 30134 RVA: 0x0036D238 File Offset: 0x0036B438
		public void SetupContesting()
		{
			this.mainIcon.sprite = this.contestingSprite;
			this.selectedImg.sprite = this.contestingHighlighSprite;
			this.icon.SetSprite(this._miscConfig.Icon, false, null);
			this.disableStyleRoot.SetStyleEffect(false, false);
			this.btnMain.spriteState = new SpriteState
			{
				highlightedSprite = this.contestingHighlighSprite,
				pressedSprite = this.contestingHighlighSprite,
				selectedSprite = this.contestingHighlighSprite
			};
			this.bookNameEnable.gameObject.SetActive(true);
			this.bookNameDisable.gameObject.SetActive(true);
			this.bookNameEnable.text = this._bookName;
			this.bookNameEnable.color = this.color_normalText;
			this.bookNameEnable.fontSize = 24f;
			this.bookNameDisable.text = LanguageKey.LK_LegendaryBook_Contesting.Tr();
			this.bookNameDisable.color = this.color_contesting;
			this.bookNameDisable.fontSize = 22f;
		}

		// Token: 0x060075B7 RID: 30135 RVA: 0x0036D362 File Offset: 0x0036B562
		public void ShowHint(bool showIcon, string content, int fontSize = 24)
		{
			this.hoverHintLayout.SetActive(true);
			this.hoverHintIcon.SetActive(showIcon);
			this.hoverHintText.text = content;
		}

		// Token: 0x060075B8 RID: 30136 RVA: 0x0036D38C File Offset: 0x0036B58C
		public void HideHint()
		{
			this.hoverHintLayout.SetActive(false);
		}

		// Token: 0x0400585D RID: 22621
		public PointerTrigger pointerTrigger;

		// Token: 0x0400585E RID: 22622
		public TextMeshProUGUI bookNameEnable;

		// Token: 0x0400585F RID: 22623
		public TextMeshProUGUI bookNameDisable;

		// Token: 0x04005860 RID: 22624
		public CImage icon;

		// Token: 0x04005861 RID: 22625
		public DisableStyleRoot disableStyleRoot;

		// Token: 0x04005862 RID: 22626
		public GameObject selectedObs;

		// Token: 0x04005863 RID: 22627
		public CImage selectedImg;

		// Token: 0x04005864 RID: 22628
		public CButton btnMain;

		// Token: 0x04005865 RID: 22629
		public CImage mainIcon;

		// Token: 0x04005866 RID: 22630
		public TooltipInvoker mouseTip;

		// Token: 0x04005867 RID: 22631
		public Action OnClick;

		// Token: 0x04005868 RID: 22632
		[Header("悬停时的人数提示")]
		public GameObject hoverHintLayout;

		// Token: 0x04005869 RID: 22633
		public GameObject hoverHintIcon;

		// Token: 0x0400586A RID: 22634
		public TextMeshProUGUI hoverHintText;

		// Token: 0x0400586B RID: 22635
		[Header("Sprites")]
		public Sprite normalSprite;

		// Token: 0x0400586C RID: 22636
		public Sprite normalHighlighSprite;

		// Token: 0x0400586D RID: 22637
		public Sprite normalSelectedSprite;

		// Token: 0x0400586E RID: 22638
		public Sprite otherSprite;

		// Token: 0x0400586F RID: 22639
		public Sprite otherHighlighSprite;

		// Token: 0x04005870 RID: 22640
		public Sprite notShownSprite;

		// Token: 0x04005871 RID: 22641
		public Sprite contestingSprite;

		// Token: 0x04005872 RID: 22642
		public Sprite contestingHighlighSprite;

		// Token: 0x04005873 RID: 22643
		public Sprite corpsesSprite;

		// Token: 0x04005874 RID: 22644
		public Sprite corpsesHighlighSprite;

		// Token: 0x04005875 RID: 22645
		private const string Icon_OwnedByOther = "ui9_btn_legendbook_bookbtn_1_0";

		// Token: 0x04005876 RID: 22646
		private const string Icon_OwnedByOtherHover = "ui9_btn_legendbook_bookbtn_1_1";

		// Token: 0x04005877 RID: 22647
		private const string Icon_NoOwner = "ui9_btn_legendbook_bookbtn_2_0";

		// Token: 0x04005878 RID: 22648
		private const string Icon_NoOwnerHover = "ui9_btn_legendbook_bookbtn_2_1";

		// Token: 0x04005879 RID: 22649
		private const string Icon_Normal = "ui9_btn_legendbook_bookbtn_0_0";

		// Token: 0x0400587A RID: 22650
		private const string Icon_NormalHover = "ui9_btn_legendbook_bookbtn_0_1";

		// Token: 0x0400587B RID: 22651
		private Color color_notShowText = new Color(0.5490196f, 0.2117647f, 0.2117647f, 1f);

		// Token: 0x0400587C RID: 22652
		private Color color_normalText = new Color(0.8823529f, 0.8039216f, 0.7058824f, 1f);

		// Token: 0x0400587D RID: 22653
		private Color color_ownByOthers = new Color(0.8392157f, 0.1686275f, 0.2078431f, 1f);

		// Token: 0x0400587E RID: 22654
		private Color color_contesting = new Color(1f, 0.8862745f, 0.4901961f, 1f);

		// Token: 0x0400587F RID: 22655
		private Color color_corpses = new Color(0.4352941f, 0.7137255f, 1f, 1f);

		// Token: 0x04005880 RID: 22656
		private MiscItem _miscConfig;

		// Token: 0x04005881 RID: 22657
		private string _bookName;
	}
}
