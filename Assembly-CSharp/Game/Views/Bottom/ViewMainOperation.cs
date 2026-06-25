using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Bottom
{
	// Token: 0x02000C57 RID: 3159
	public class ViewMainOperation : UIBase, IMainMenuButtonParent, IAsyncMethodRequestHandler
	{
		// Token: 0x0600A0DE RID: 41182 RVA: 0x004B2210 File Offset: 0x004B0410
		private void Awake()
		{
			this.quickHide.onClick.ResetListener(new Action(this.QuickHide));
			this.left.onClick.ResetListener(new Action(this.PreviousPage));
			this.right.onClick.ResetListener(new Action(this.NextPage));
			this.shortcutButton.onClick.ResetListener(new Action(this.OnShortcutClick));
		}

		// Token: 0x0600A0DF RID: 41183 RVA: 0x004B2294 File Offset: 0x004B0494
		public override void OnInit(ArgumentBox argsBox)
		{
			this.selectingImage.enabled = false;
			this.currPage = 0;
			this.buttonShown.Clear();
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.RequestData));
			this.mainShortcutPanelRect.gameObject.SetActive(false);
		}

		// Token: 0x0600A0E0 RID: 41184 RVA: 0x004B22FB File Offset: 0x004B04FB
		public void RequestData()
		{
			this.OnChildExit(null);
			TaiwuDomainMethod.AsyncCall.RequestMainOperationOrder(this, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._buttonIndex);
				this.buttonShown.Clear();
				foreach (int buttonId in this._buttonIndex)
				{
					bool flag = buttonId != 0 && buttonId != this.secretInformationIndex;
					if (flag)
					{
						MainMenuButton button = this.buttons[buttonId];
						button.Init(this, (byte)buttonId);
						bool isThisActive = button.IsThisActive;
						if (isThisActive)
						{
							this.buttonShown.Add(button);
						}
					}
				}
				this.maxPage = (this.buttonShown.Count + 11) / 12;
				this.hotkey.text = ((this.maxPage < 2) ? LanguageKey.LK_Main_Operation_HotKey_Page_Disable.TrFormat(MainInterfaceFunctionCommandKit.QuickEntry) : LanguageKey.LK_Main_Operation_HotKey.TrFormat(MainInterfaceFunctionCommandKit.QuickEntry));
				this.SetPage(0);
			});
		}

		// Token: 0x0600A0E1 RID: 41185 RVA: 0x004B2319 File Offset: 0x004B0519
		public void NextPage()
		{
			this.SetPage(this.currPage + 1);
		}

		// Token: 0x0600A0E2 RID: 41186 RVA: 0x004B232A File Offset: 0x004B052A
		public void PreviousPage()
		{
			this.SetPage(this.currPage - 1);
		}

		// Token: 0x0600A0E3 RID: 41187 RVA: 0x004B233B File Offset: 0x004B053B
		public void OnChildClicked(MainMenuButton child)
		{
			this._playAudioOut = false;
			this.OnChildExit(child);
			this.QuickHide();
		}

		// Token: 0x0600A0E4 RID: 41188 RVA: 0x004B2354 File Offset: 0x004B0554
		public void OnChildEnter(MainMenuButton child)
		{
			child.SetText(this.selectingImage, this.btnName, this.simple, this.complex);
		}

		// Token: 0x0600A0E5 RID: 41189 RVA: 0x004B2378 File Offset: 0x004B0578
		public void OnChildExit(MainMenuButton child)
		{
			this.selectingImage.enabled = false;
			this.btnName.text = (this.simple.text = (this.complex.text = ""));
		}

		// Token: 0x0600A0E6 RID: 41190 RVA: 0x004B23C4 File Offset: 0x004B05C4
		private void Update()
		{
			bool flag = MainInterfaceFunctionCommandKit.QuickEntry.Check(this.Element, false, false, false, true, false);
			if (flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x0600A0E7 RID: 41191 RVA: 0x004B23F4 File Offset: 0x004B05F4
		public void SetPage(int index)
		{
			bool flag = this.maxPage == 1;
			if (flag)
			{
				index = (this.currPage = 0);
				this.pager.gameObject.SetActive(false);
			}
			else
			{
				bool flag2 = (this.currPage = index) >= this.maxPage;
				if (flag2)
				{
					index = (this.currPage = this.maxPage - 1);
				}
				else
				{
					bool flag3 = index < 0;
					if (flag3)
					{
						index = (this.currPage = 0);
					}
				}
				this.currentPageText.text = (index + 1).ToString();
				this.left.interactable = (index > 0);
				this.right.interactable = (index < this.maxPage - 1);
			}
			index *= 13;
			int end = Math.Min(index + 13, this.buttonShown.Count);
			float deltaTheta = 6.2831855f / (float)(end - index);
			float theta = 0f;
			while (index < end)
			{
				MainMenuButton btn = this.buttonShown[index];
				btn.transform.localPosition = new Vector2(Mathf.Sin(theta), Mathf.Cos(theta)) * this.radius + this.circlePosition;
				theta += deltaTheta;
				btn.gameObject.SetActive(true);
				index++;
			}
			while (index < this.buttonShown.Count)
			{
				MainMenuButton btn2 = this.buttonShown[index];
				btn2.gameObject.SetActive(false);
				index++;
			}
		}

		// Token: 0x0600A0E8 RID: 41192 RVA: 0x004B258C File Offset: 0x004B078C
		public void OnShortcutClick()
		{
			this.mainShortcutPanelRect.gameObject.SetActive(true);
		}

		// Token: 0x0600A0E9 RID: 41193 RVA: 0x004B25A4 File Offset: 0x004B07A4
		public void OnCloseShortcutClick()
		{
			bool isChange = this.mainShortcutPanel.CleanData();
			bool flag = isChange;
			if (flag)
			{
				this.RequestData();
			}
			this.mainShortcutPanelRect.gameObject.SetActive(false);
		}

		// Token: 0x0600A0EA RID: 41194 RVA: 0x004B25E0 File Offset: 0x004B07E0
		public override void PlayAudioOut()
		{
			bool playAudioOut = this._playAudioOut;
			if (playAudioOut)
			{
				base.PlayAudioOut();
			}
			else
			{
				this._playAudioOut = true;
			}
		}

		// Token: 0x04007CBB RID: 31931
		[SerializeField]
		private MainMenuButton[] buttons;

		// Token: 0x04007CBC RID: 31932
		[SerializeField]
		private RectTransform pager;

		// Token: 0x04007CBD RID: 31933
		[SerializeField]
		private int currPage;

		// Token: 0x04007CBE RID: 31934
		[SerializeField]
		private int maxPage;

		// Token: 0x04007CBF RID: 31935
		[SerializeField]
		private CImage selectingImage;

		// Token: 0x04007CC0 RID: 31936
		[SerializeField]
		private float radius = 100f;

		// Token: 0x04007CC1 RID: 31937
		[SerializeField]
		private Vector2 circlePosition = new Vector2(0f, 0f);

		// Token: 0x04007CC2 RID: 31938
		[SerializeField]
		private TMP_Text btnName;

		// Token: 0x04007CC3 RID: 31939
		[SerializeField]
		private TMP_Text simple;

		// Token: 0x04007CC4 RID: 31940
		[SerializeField]
		private TMP_Text complex;

		// Token: 0x04007CC5 RID: 31941
		[SerializeField]
		private TMP_Text hotkey;

		// Token: 0x04007CC6 RID: 31942
		[SerializeField]
		private TMP_Text currentPageText;

		// Token: 0x04007CC7 RID: 31943
		[SerializeField]
		private CButton left;

		// Token: 0x04007CC8 RID: 31944
		[SerializeField]
		private CButton right;

		// Token: 0x04007CC9 RID: 31945
		[SerializeField]
		private CButton shortcutButton;

		// Token: 0x04007CCA RID: 31946
		[SerializeField]
		private RectTransform mainShortcutPanelRect;

		// Token: 0x04007CCB RID: 31947
		[SerializeField]
		private MainShortcutPanel mainShortcutPanel;

		// Token: 0x04007CCC RID: 31948
		[SerializeField]
		private CButton quickHide;

		// Token: 0x04007CCD RID: 31949
		[SerializeField]
		private List<MainMenuButton> buttonShown;

		// Token: 0x04007CCE RID: 31950
		private int[] _buttonIndex = Array.Empty<int>();

		// Token: 0x04007CCF RID: 31951
		private readonly int secretInformationIndex = 4;

		// Token: 0x04007CD0 RID: 31952
		private bool _playAudioOut = true;
	}
}
