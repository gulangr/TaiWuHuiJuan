using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Bottom
{
	// Token: 0x02000C34 RID: 3124
	public class MainShortcutPanel : MonoBehaviour, IMainShortcutButton, IAsyncMethodRequestHandler
	{
		// Token: 0x06009EBD RID: 40637 RVA: 0x004A4190 File Offset: 0x004A2390
		private void Awake()
		{
			this.left.onClick.ResetListener(new Action(this.PreviousPage));
			this.right.onClick.ResetListener(new Action(this.NextPage));
			foreach (MainShortcutButton btn in this.buttons)
			{
				btn.gameObject.SetActive(false);
			}
		}

		// Token: 0x06009EBE RID: 40638 RVA: 0x004A4204 File Offset: 0x004A2404
		private void OnEnable()
		{
			this.isChange = false;
			this.isNotClick = false;
			this.selectingImage.enabled = false;
			this.currPage = 0;
			this.isShowClickedObj = false;
			this.notClickedObj.SetActive(true);
			this.clickedObj.SetActive(false);
			this.clickPrompt.gameObject.SetActive(false);
			UIManager.Instance.SetEscHandler(new Action(this.CloseView));
			this.RequestData();
		}

		// Token: 0x06009EBF RID: 40639 RVA: 0x004A4285 File Offset: 0x004A2485
		private void OnDisable()
		{
			UIManager.Instance.SetEscHandler(null);
		}

		// Token: 0x06009EC0 RID: 40640 RVA: 0x004A4294 File Offset: 0x004A2494
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
						MainShortcutButton button = this.buttons[buttonId];
						button.Init(this, (byte)buttonId);
						bool isThisActive = button.IsThisActive;
						if (isThisActive)
						{
							this.buttonShown.Add(button);
						}
					}
				}
				this.SetPage(0);
			});
		}

		// Token: 0x06009EC1 RID: 40641 RVA: 0x004A42B2 File Offset: 0x004A24B2
		public void NextPage()
		{
			this.SetPage(this.currPage + 1);
		}

		// Token: 0x06009EC2 RID: 40642 RVA: 0x004A42C3 File Offset: 0x004A24C3
		public void PreviousPage()
		{
			this.SetPage(this.currPage - 1);
		}

		// Token: 0x06009EC3 RID: 40643 RVA: 0x004A42D4 File Offset: 0x004A24D4
		public void OnChildClicked(MainShortcutButton child)
		{
			this.OnChildExit(child);
			bool flag = !this.isShowClickedObj;
			if (flag)
			{
				this.clickImage1.sprite = child.NormalImage;
				bool activeSelf = this.notClickedObj.activeSelf;
				if (activeSelf)
				{
					this.notClickedObj.SetActive(false);
					UIManager.Instance.SetEscHandler(new Action(this.CheckMouseButtonDown));
					this.clickedObj.SetActive(true);
					this.isShowClickedObj = true;
				}
			}
			bool flag2 = !this.switchButtonItemStart && !this.switchButtonItemOver;
			if (flag2)
			{
				this.switchButtonItemStart = child;
			}
			else
			{
				bool flag3 = !this.switchButtonItemOver && this.switchButtonItemStart.GetTemplateId() != child.GetTemplateId();
				if (flag3)
				{
					this.switchButtonItemOver = child;
					this.SetSwitchPos();
				}
			}
		}

		// Token: 0x06009EC4 RID: 40644 RVA: 0x004A43BB File Offset: 0x004A25BB
		public void OnChildEnter(MainShortcutButton child)
		{
			child.SetText(this.selectingImage, this.btnName, this.simple, this.complex);
			this.isNotClick = false;
		}

		// Token: 0x06009EC5 RID: 40645 RVA: 0x004A43E4 File Offset: 0x004A25E4
		public void OnChildExit(MainShortcutButton child)
		{
			this.selectingImage.enabled = false;
			this.btnName.text = (this.simple.text = (this.complex.text = ""));
		}

		// Token: 0x06009EC6 RID: 40646 RVA: 0x004A4430 File Offset: 0x004A2630
		public void SetButtonData(Sprite sprite, int templateId, string name)
		{
			bool flag = !this.isShowClickedObj && !this.isNotClick;
			if (flag)
			{
				bool activeSelf = this.notClickedObj.activeSelf;
				if (activeSelf)
				{
					this.notClickedObj.SetActive(false);
					UIManager.Instance.SetEscHandler(new Action(this.CheckMouseButtonDown));
					this.clickedObj.SetActive(true);
					this.isShowClickedObj = true;
				}
			}
			bool flag2 = !this.switchButtonItemStart && !this.switchButtonItemOver && !this.isNotClick;
			if (flag2)
			{
				this.clickImage1.sprite = sprite;
			}
			else
			{
				bool flag3 = !this.switchButtonItemOver && !this.isNotClick;
				if (flag3)
				{
					bool flag4 = this.switchButtonItemStart.GetTemplateId() != templateId;
					if (flag4)
					{
						this.clickImage2.sprite = sprite;
						this.clickPrompt.text = LocalStringManager.GetFormat(LanguageKey.LK_MainShortcutPanel_Exchange, this.switchButtonItemStart.NameText, name);
						this.clickPrompt.gameObject.SetActive(true);
					}
				}
			}
		}

		// Token: 0x06009EC7 RID: 40647 RVA: 0x004A4558 File Offset: 0x004A2758
		public bool CleanData()
		{
			this.isShowClickedObj = false;
			this.notClickedObj.SetActive(true);
			this.clickedObj.SetActive(false);
			this.clickPrompt.gameObject.SetActive(false);
			MainShortcutButton mainShortcutButton = this.switchButtonItemStart;
			if (mainShortcutButton != null)
			{
				mainShortcutButton.RestoreStatus();
			}
			this.switchButtonItemStart = null;
			MainShortcutButton mainShortcutButton2 = this.switchButtonItemOver;
			if (mainShortcutButton2 != null)
			{
				mainShortcutButton2.RestoreStatus();
			}
			this.switchButtonItemOver = null;
			this.clickImage1.SetSprite(this.normalImage, false, null);
			this.clickImage2.SetSprite(this.normalImage, false, null);
			return this.isChange;
		}

		// Token: 0x06009EC8 RID: 40648 RVA: 0x004A4600 File Offset: 0x004A2800
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
				MainShortcutButton btn = this.buttonShown[index];
				btn.transform.localPosition = new Vector2(Mathf.Sin(theta), Mathf.Cos(theta)) * this.radius + this.circlePosition;
				theta += deltaTheta;
				btn.gameObject.SetActive(true);
				index++;
			}
			while (index < this.buttonShown.Count)
			{
				MainShortcutButton btn2 = this.buttonShown[index];
				btn2.gameObject.SetActive(false);
				index++;
			}
		}

		// Token: 0x06009EC9 RID: 40649 RVA: 0x004A4798 File Offset: 0x004A2998
		private void SetSwitchPos()
		{
			int indexStart = -1;
			int indexOver = -1;
			for (int i = 0; i < this.buttonShown.Count; i++)
			{
				bool flag = this.buttonShown[i].GetTemplateId() == this.switchButtonItemStart.GetTemplateId();
				if (flag)
				{
					indexStart = i;
				}
				bool flag2 = this.buttonShown[i].GetTemplateId() == this.switchButtonItemOver.GetTemplateId();
				if (flag2)
				{
					indexOver = i;
				}
			}
			this.isChange = true;
			this.isNotClick = true;
			MainShortcutButton btnStart = this.buttonShown[indexStart];
			MainShortcutButton btnEnd = this.buttonShown[indexOver];
			this.buttonShown[indexStart] = btnEnd;
			this.buttonShown[indexOver] = btnStart;
			this.SetPage(this.currPage);
			this.CleanData();
			int[] switchButton = new int[this.buttonShown.Count + 1];
			switchButton[0] = this._buttonIndex[0];
			for (int j = 0; j < this.buttonShown.Count; j++)
			{
				switchButton[j + 1] = this.buttonShown[j].GetTemplateId();
			}
			TaiwuDomainMethod.Call.SetMainOperationOrder(switchButton);
		}

		// Token: 0x06009ECA RID: 40650 RVA: 0x004A48DB File Offset: 0x004A2ADB
		private void CheckMouseButtonDown()
		{
			this.CleanData();
			UIManager.Instance.SetEscHandler(new Action(this.CloseView));
		}

		// Token: 0x06009ECB RID: 40651 RVA: 0x004A48FC File Offset: 0x004A2AFC
		private void CloseView()
		{
			base.gameObject.SetActive(false);
			UIManager.Instance.SetEscHandler(null);
		}

		// Token: 0x06009ECC RID: 40652 RVA: 0x004A4918 File Offset: 0x004A2B18
		public void RegisterAsyncMethodCall(int requestId)
		{
		}

		// Token: 0x06009ECD RID: 40653 RVA: 0x004A491B File Offset: 0x004A2B1B
		public void ClearAsyncMethodCalls()
		{
		}

		// Token: 0x04007AD0 RID: 31440
		[SerializeField]
		private MainShortcutButton[] buttons;

		// Token: 0x04007AD1 RID: 31441
		[SerializeField]
		private RectTransform pager;

		// Token: 0x04007AD2 RID: 31442
		[SerializeField]
		private int currPage;

		// Token: 0x04007AD3 RID: 31443
		[SerializeField]
		private int maxPage;

		// Token: 0x04007AD4 RID: 31444
		[SerializeField]
		private CImage selectingImage;

		// Token: 0x04007AD5 RID: 31445
		[SerializeField]
		private CImage clickImage1;

		// Token: 0x04007AD6 RID: 31446
		[SerializeField]
		private CImage clickImage2;

		// Token: 0x04007AD7 RID: 31447
		[SerializeField]
		private float radius = 100f;

		// Token: 0x04007AD8 RID: 31448
		[SerializeField]
		private Vector2 circlePosition = new Vector2(0f, 0f);

		// Token: 0x04007AD9 RID: 31449
		[SerializeField]
		private TMP_Text btnName;

		// Token: 0x04007ADA RID: 31450
		[SerializeField]
		private TMP_Text simple;

		// Token: 0x04007ADB RID: 31451
		[SerializeField]
		private TMP_Text complex;

		// Token: 0x04007ADC RID: 31452
		[SerializeField]
		private TMP_Text currentPageText;

		// Token: 0x04007ADD RID: 31453
		[SerializeField]
		private TMP_Text hintText;

		// Token: 0x04007ADE RID: 31454
		[SerializeField]
		private TMP_Text clickPrompt;

		// Token: 0x04007ADF RID: 31455
		[SerializeField]
		private CButton left;

		// Token: 0x04007AE0 RID: 31456
		[SerializeField]
		private CButton right;

		// Token: 0x04007AE1 RID: 31457
		[SerializeField]
		private GameObject notClickedObj;

		// Token: 0x04007AE2 RID: 31458
		[SerializeField]
		private GameObject clickedObj;

		// Token: 0x04007AE3 RID: 31459
		[SerializeField]
		private List<MainShortcutButton> buttonShown;

		// Token: 0x04007AE4 RID: 31460
		private int[] _buttonIndex = Array.Empty<int>();

		// Token: 0x04007AE5 RID: 31461
		private bool isShowClickedObj;

		// Token: 0x04007AE6 RID: 31462
		private string normalImage = "ui9_btn_main_menu_circle_villager_empty_0";

		// Token: 0x04007AE7 RID: 31463
		private bool isNotClick;

		// Token: 0x04007AE8 RID: 31464
		private bool isChange;

		// Token: 0x04007AE9 RID: 31465
		private readonly int secretInformationIndex = 4;

		// Token: 0x04007AEA RID: 31466
		private MainShortcutButton switchButtonItemStart;

		// Token: 0x04007AEB RID: 31467
		private MainShortcutButton switchButtonItemOver;
	}
}
