using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views
{
	// Token: 0x02000705 RID: 1797
	public class ViewDialog : UIBase
	{
		// Token: 0x060054E8 RID: 21736 RVA: 0x00276100 File Offset: 0x00274300
		public override void OnInit(ArgumentBox argsBox)
		{
			if (argsBox != null)
			{
				argsBox.Get<DialogCmd>("Cmd", out this._showingCmd);
			}
			GlobalSettings settingData = SingletonObject.getInstance<GlobalSettings>();
			bool checkDoNotShow = settingData.CheckDialogDoNotShow(this._showingCmd.DialogType);
			bool yesOnly = this._showingCmd.Type == 2;
			GameObject desc = yesOnly ? this.normalDesc : this.strongDesc;
			this.yesLabel.text = (this._showingCmd.GroupYesText.IsNullOrEmpty() ? LanguageKey.LK_Confirm.Tr() : this._showingCmd.GroupYesText);
			this.noLabel.text = (this._showingCmd.GroupNoText.IsNullOrEmpty() ? LanguageKey.LK_Cancel.Tr() : this._showingCmd.GroupNoText);
			this.doNotShowAgain.onValueChanged.RemoveAllListeners();
			this.doNotShowAgain.isOn = checkDoNotShow;
			this.doNotShowAgain.onValueChanged.AddListener(new UnityAction<bool>(this.SetDoNotShow));
			this.doNotShowAgain.interactable = false;
			PointerTrigger pointerTrigger = this.doNotShowAgain.GetComponent<PointerTrigger>();
			pointerTrigger.EnterEvent.RemoveAllListeners();
			pointerTrigger.EnterEvent.AddListener(delegate()
			{
				this.doNotShowAgainDesc.SetActive(true);
			});
			pointerTrigger.ExitEvent.RemoveAllListeners();
			pointerTrigger.ExitEvent.AddListener(delegate()
			{
				this.doNotShowAgainDesc.SetActive(false);
			});
			this.title.text = this._showingCmd.Title.ColorReplace();
			desc.GetComponent<TextMeshProUGUI>().text = this._showingCmd.Content.ColorReplace();
			TMPTextSpriteHelper descSprite = desc.GetComponent<TMPTextSpriteHelper>();
			descSprite.SpriteSizeFitType = this._showingCmd.SpriteHelperFitType;
			descSprite.StaticSpriteSize = this._showingCmd.SpriteHelperSize;
			descSprite.Parse();
			this.btnNo.SetActive(!yesOnly);
			this.strongDesc.SetActive(!yesOnly);
			this.normalDesc.SetActive(yesOnly);
		}

		// Token: 0x060054E9 RID: 21737 RVA: 0x00276304 File Offset: 0x00274504
		private void SetDoNotShow(bool doNotShow)
		{
			GlobalSettings settingData = SingletonObject.getInstance<GlobalSettings>();
			if (doNotShow)
			{
				settingData.AddDialogDoNotShow(this._showingCmd.DialogType);
			}
			else
			{
				settingData.RemoveDialogDoNotShow(this._showingCmd.DialogType);
			}
			settingData.SaveSettings();
		}

		// Token: 0x060054EA RID: 21738 RVA: 0x0027634C File Offset: 0x0027454C
		protected override void OnClick(Transform btn)
		{
			UIManager.Instance.HideUI(this.Element);
			string btnName = btn.name;
			string text = btnName;
			string a = text;
			if (!(a == "BtnYes"))
			{
				if (a == "BtnNo")
				{
					Action no = this._showingCmd.No;
					if (no != null)
					{
						no();
					}
				}
			}
			else
			{
				Action yes = this._showingCmd.Yes;
				if (yes != null)
				{
					yes();
				}
			}
		}

		// Token: 0x060054EB RID: 21739 RVA: 0x002763C4 File Offset: 0x002745C4
		private void Update()
		{
			bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false) || CommonCommandKit.Enter.Check(this.Element, false, false, false, true, false);
			if (flag)
			{
				UIManager.Instance.HideUI(this.Element);
				Action yes = this._showingCmd.Yes;
				if (yes != null)
				{
					yes();
				}
			}
			else
			{
				bool flag2 = CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false) || CommonCommandKit.RightMouse.Check(this.Element, false, false, false, true, false);
				if (flag2)
				{
					AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
					UIManager.Instance.HideUI(this.Element);
					Action no = this._showingCmd.No;
					if (no != null)
					{
						no();
					}
				}
			}
		}

		// Token: 0x040039C4 RID: 14788
		private DialogCmd _showingCmd;

		// Token: 0x040039C5 RID: 14789
		public GameObject btnNo;

		// Token: 0x040039C6 RID: 14790
		public CToggle doNotShowAgain;

		// Token: 0x040039C7 RID: 14791
		public TextMeshProUGUI title;

		// Token: 0x040039C8 RID: 14792
		public TextMeshProUGUI yesLabel;

		// Token: 0x040039C9 RID: 14793
		public TextMeshProUGUI noLabel;

		// Token: 0x040039CA RID: 14794
		public GameObject strongDesc;

		// Token: 0x040039CB RID: 14795
		public GameObject normalDesc;

		// Token: 0x040039CC RID: 14796
		public GameObject doNotShowAgainDesc;
	}
}
