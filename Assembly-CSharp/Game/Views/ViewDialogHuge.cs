using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;

namespace Game.Views
{
	// Token: 0x02000706 RID: 1798
	public class ViewDialogHuge : UIBase
	{
		// Token: 0x060054EF RID: 21743 RVA: 0x002764C8 File Offset: 0x002746C8
		private void Awake()
		{
			this.leftBtn.onClick.ResetListener(delegate()
			{
				UIManager.Instance.HideUI(this.Element);
				Action action = this._showingCmd.Left;
				if (action != null)
				{
					action();
				}
			});
			this.middleBtn.onClick.ResetListener(delegate()
			{
				UIManager.Instance.HideUI(this.Element);
				Action action = this._showingCmd.Middle;
				if (action != null)
				{
					action();
				}
			});
			this.rightBtn.onClick.ResetListener(delegate()
			{
				UIManager.Instance.HideUI(this.Element);
				Action action = this._showingCmd.Right;
				if (action != null)
				{
					action();
				}
			});
		}

		// Token: 0x060054F0 RID: 21744 RVA: 0x00276530 File Offset: 0x00274730
		private void Update()
		{
			bool flag = this.Element == null;
			if (!flag)
			{
				bool flag2 = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false) || CommonCommandKit.Enter.Check(this.Element, false, false, false, true, false);
				if (flag2)
				{
					UIManager.Instance.HideUI(this.Element);
					Action action = this._showingCmd.Left;
					if (action != null)
					{
						action();
					}
				}
				else
				{
					bool flag3 = CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false) || CommonCommandKit.RightMouse.Check(this.Element, false, false, false, true, false);
					if (flag3)
					{
						AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
						UIManager.Instance.HideUI(this.Element);
						Action action2 = this._showingCmd.Right;
						if (action2 != null)
						{
							action2();
						}
					}
				}
			}
		}

		// Token: 0x060054F1 RID: 21745 RVA: 0x00276620 File Offset: 0x00274820
		public override void OnInit(ArgumentBox argsBox)
		{
			if (argsBox != null)
			{
				argsBox.Get<DialogCmdHuge>("Cmd", out this._showingCmd);
			}
			this.title.text = this._showingCmd.Title.ColorReplace();
			this.content.text = this._showingCmd.Content.ColorReplace();
			this.content.rectTransform.sizeDelta = this.content.rectTransform.sizeDelta.SetX(this._showingCmd.TextWidth);
			this.descSprite.SpriteSizeFitType = this._showingCmd.SpriteHelperFitType;
			this.descSprite.StaticSpriteSize = this._showingCmd.SpriteHelperSize;
			this.descSprite.Parse();
			bool flag = this._showingCmd.LeftText.IsNullOrEmpty();
			if (flag)
			{
				this.leftBtn.gameObject.SetActive(false);
			}
			else
			{
				this.leftBtn.gameObject.SetActive(true);
				this.leftTips.enabled = !string.IsNullOrEmpty(this._showingCmd.LeftTips);
				this.leftTips.PresetParam[0] = this._showingCmd.LeftTips;
				this.left.text = this._showingCmd.LeftText.ColorReplace();
			}
			bool flag2 = this._showingCmd.MiddleText.IsNullOrEmpty();
			if (flag2)
			{
				this.middleBtn.gameObject.SetActive(false);
			}
			else
			{
				this.middleBtn.gameObject.SetActive(true);
				this.middleTips.enabled = !string.IsNullOrEmpty(this._showingCmd.MiddleText);
				this.middleTips.PresetParam[0] = this._showingCmd.MiddleText;
				this.middle.text = this._showingCmd.MiddleText.ColorReplace();
			}
			bool flag3 = this._showingCmd.RightText.IsNullOrEmpty();
			if (flag3)
			{
				this.rightBtn.gameObject.SetActive(false);
			}
			else
			{
				this.rightBtn.gameObject.SetActive(true);
				this.rightTips.enabled = !string.IsNullOrEmpty(this._showingCmd.RightTips);
				this.rightTips.PresetParam[0] = this._showingCmd.RightTips;
				this.right.text = this._showingCmd.RightText.ColorReplace();
			}
		}

		// Token: 0x040039CD RID: 14797
		private DialogCmdHuge _showingCmd;

		// Token: 0x040039CE RID: 14798
		public TMP_Text title;

		// Token: 0x040039CF RID: 14799
		public TMP_Text content;

		// Token: 0x040039D0 RID: 14800
		public TMP_Text left;

		// Token: 0x040039D1 RID: 14801
		public TMP_Text middle;

		// Token: 0x040039D2 RID: 14802
		public TMP_Text right;

		// Token: 0x040039D3 RID: 14803
		public CButton leftBtn;

		// Token: 0x040039D4 RID: 14804
		public CButton middleBtn;

		// Token: 0x040039D5 RID: 14805
		public CButton rightBtn;

		// Token: 0x040039D6 RID: 14806
		public TooltipInvoker leftTips;

		// Token: 0x040039D7 RID: 14807
		public TooltipInvoker middleTips;

		// Token: 0x040039D8 RID: 14808
		public TooltipInvoker rightTips;

		// Token: 0x040039D9 RID: 14809
		public TMPTextSpriteHelper descSprite;
	}
}
