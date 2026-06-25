using System;
using System.IO;
using Config;
using DisplayConfig;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using GameData.Domains.Building;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Game.Views.VillagerRoleView
{
	// Token: 0x02000734 RID: 1844
	public class SelectableChickenItem : Refers
	{
		// Token: 0x06005887 RID: 22663 RVA: 0x002919BD File Offset: 0x0028FBBD
		private void Awake()
		{
			this.Init();
		}

		// Token: 0x06005888 RID: 22664 RVA: 0x002919C8 File Offset: 0x0028FBC8
		public void Init()
		{
			bool inited = this._inited;
			if (!inited)
			{
				this.mainButton.ClearAndAddListener(new Action(this.OnButtonClick));
				this.feedButton.ClearAndAddListener(new Action(this.OnFeedButtonClick));
				this.btnClear.ClearAndAddListener(new Action(this.OnClearClicked));
				this._inited = true;
				bool flag = this.pointerTrigger == null;
				if (!flag)
				{
					this.pointerTrigger.EnterEvent.RemoveAllListeners();
					this.pointerTrigger.EnterEvent.AddListener(delegate()
					{
						this._isEnterEvent = true;
						this.SetClearButton(this._isEnterEvent);
					});
					this.pointerTrigger.ExitEvent.RemoveAllListeners();
					this.pointerTrigger.ExitEvent.AddListener(delegate()
					{
						this._isEnterEvent = false;
						this.SetClearButton(this._isEnterEvent);
					});
				}
			}
		}

		// Token: 0x06005889 RID: 22665 RVA: 0x00291AA3 File Offset: 0x0028FCA3
		private void OnClearClicked()
		{
			Action<int> onClickClear = this._onClickClear;
			if (onClickClear != null)
			{
				onClickClear(this._index);
			}
		}

		// Token: 0x0600588A RID: 22666 RVA: 0x00291AC0 File Offset: 0x0028FCC0
		public void Refresh(int index, GameData.Domains.Building.Chicken chicken, string nickName = null, bool isSelected = false, bool isLeaved = false, Action<int> onClick = null, Action<int> onClickFeedButton = null, Action<int> onClickClearButton = null, bool isHoverShow = false)
		{
			this._chicken = chicken;
			this._config = Config.Chicken.Instance[chicken.TemplateId];
			this._index = index;
			this._isHoverShow = isHoverShow;
			this.RefreshEmpty(false);
			this.RefreshName(nickName);
			this.RefreshPersonality();
			this.RefreshImage();
			this.RefreshButton(onClick);
			this.RefreshFeedButton(onClickFeedButton);
			this.RefreshSelected(isSelected);
			this.RefreshHappiness(0);
			this.RefreshLeaveStatus(isLeaved);
			this.RefreshClearButton(onClickClearButton);
			this.RefreshGrade(this._config.Grade);
			bool flag = this.skeleton;
			if (flag)
			{
				this.skeleton.Skeleton.SetAttachment("side_body", string.Format("chicken_{0}_side", this._config.ChickenColor.ToInt()));
			}
			bool flag2 = this._isEnterEvent && this._isHoverShow;
			if (flag2)
			{
				this.SetClearButton(this._isEnterEvent);
			}
		}

		// Token: 0x0600588B RID: 22667 RVA: 0x00291BD0 File Offset: 0x0028FDD0
		public void SetDisable(bool isDisabled)
		{
			base.GetComponent<DisableStyleRoot>().SetStyleEffect(isDisabled, false);
			foreach (PointerTrigger pointerTrigger in base.GetComponentsInChildren<PointerTrigger>())
			{
				pointerTrigger.enabled = !isDisabled;
			}
		}

		// Token: 0x0600588C RID: 22668 RVA: 0x00291C14 File Offset: 0x0028FE14
		public void RefreshLeaveStatus(bool isLeaved)
		{
			this.chickenImage.GetComponent<DisableStyleRoot>().SetStyleEffect(isLeaved, false);
			this.feedButton.interactable = !isLeaved;
			this.personalityBack.SetActive(!isLeaved);
			this.leaveMark.SetActive(isLeaved);
		}

		// Token: 0x0600588D RID: 22669 RVA: 0x00291C64 File Offset: 0x0028FE64
		public void RefreshEmpty(bool isEmpty)
		{
			this.empty.SetActive(isEmpty);
			this.content.SetActive(!isEmpty);
			this.btnClear.gameObject.SetActive(!isEmpty && !this._isHoverShow);
			this.feedButton.gameObject.SetActive(!isEmpty);
		}

		// Token: 0x0600588E RID: 22670 RVA: 0x00291CC4 File Offset: 0x0028FEC4
		public void RefreshRoleIcon(short roleId)
		{
			this.roleIcon.SetSprite(string.Format("sp_icon_role_{0}", roleId), false, null);
		}

		// Token: 0x0600588F RID: 22671 RVA: 0x00291CE5 File Offset: 0x0028FEE5
		public void SetRoleIconVisible(bool visible)
		{
			this.roleIcon.gameObject.SetActive(visible);
		}

		// Token: 0x06005890 RID: 22672 RVA: 0x00291CFC File Offset: 0x0028FEFC
		public void SetClearButton(bool isShow)
		{
			bool flag = this.empty.activeSelf || !this._isHoverShow;
			if (!flag)
			{
				this.btnClear.gameObject.SetActive(isShow);
			}
		}

		// Token: 0x06005891 RID: 22673 RVA: 0x00291D3C File Offset: 0x0028FF3C
		private void RefreshPersonality()
		{
			PersonalityItem p = Personality.Instance[(int)this._config.PersonalityType];
			this.personalityNameLabel.text = LocalStringManager.Get(p.Name);
			this.personalityIcon.SetSprite("ui9_icon_personality_big_" + this._config.PersonalityType.ToString(), false, null);
			this.personalityCountLabel.text = string.Format("+{0}", this._config.PersonalityValue);
		}

		// Token: 0x06005892 RID: 22674 RVA: 0x00291DC5 File Offset: 0x0028FFC5
		public void RefreshButton(Action<int> onClick)
		{
			this._onClick = onClick;
		}

		// Token: 0x06005893 RID: 22675 RVA: 0x00291DCF File Offset: 0x0028FFCF
		private void RefreshFeedButton(Action<int> onClick)
		{
			this.feedButton.gameObject.SetActive(onClick != null);
			this._onClickFeedButton = onClick;
		}

		// Token: 0x06005894 RID: 22676 RVA: 0x00291DEE File Offset: 0x0028FFEE
		private void RefreshClearButton(Action<int> onClickClear)
		{
			this._onClickClear = onClickClear;
		}

		// Token: 0x06005895 RID: 22677 RVA: 0x00291DF8 File Offset: 0x0028FFF8
		private void RefreshSelected(bool isSelected)
		{
			this.selected.SetActive(isSelected);
		}

		// Token: 0x06005896 RID: 22678 RVA: 0x00291E08 File Offset: 0x00290008
		public void RefreshHappiness(sbyte extraHappiness = 0)
		{
			int happiness = Math.Min(100, (int)(this._chicken.Happiness + extraHappiness));
			this.happinessCircle.fillAmount = (float)happiness / 100f;
			this.happinessLabel.Refresh(happiness.ToString());
		}

		// Token: 0x06005897 RID: 22679 RVA: 0x00291E52 File Offset: 0x00290052
		private void RefreshImage()
		{
			ResLoader.Load<Sprite>(Path.Combine("RemakeResources/Textures/Chicken", this._config.Display), delegate(Sprite sprite)
			{
				bool flag = this.chickenImage == null;
				if (!flag)
				{
					this.chickenImage.sprite = sprite;
					this.chickenImage.enabled = true;
				}
			}, null, false);
		}

		// Token: 0x06005898 RID: 22680 RVA: 0x00291E7E File Offset: 0x0029007E
		private void RefreshGrade(sbyte grade)
		{
			this.gradeImage.SetSprite(string.Format("{0}{1}", "ui9_icon_item_grade_", grade), false, null);
		}

		// Token: 0x06005899 RID: 22681 RVA: 0x00291EA4 File Offset: 0x002900A4
		private void RefreshName(string nickName = null)
		{
			bool flag = nickName != null;
			if (flag)
			{
				this.nameLabel.text = nickName;
			}
			else
			{
				this.nameLabel.text = this._config.Name;
			}
		}

		// Token: 0x0600589A RID: 22682 RVA: 0x00291EE0 File Offset: 0x002900E0
		private void OnButtonClick()
		{
			Action<int> onClick = this._onClick;
			if (onClick != null)
			{
				onClick(this._index);
			}
		}

		// Token: 0x0600589B RID: 22683 RVA: 0x00291EFB File Offset: 0x002900FB
		private void OnFeedButtonClick()
		{
			Action<int> onClickFeedButton = this._onClickFeedButton;
			if (onClickFeedButton != null)
			{
				onClickFeedButton(this._index);
			}
		}

		// Token: 0x04003CEF RID: 15599
		public const string ChickenPath = "RemakeResources/Textures/Chicken";

		// Token: 0x04003CF0 RID: 15600
		private bool _inited;

		// Token: 0x04003CF1 RID: 15601
		private bool _isHoverShow;

		// Token: 0x04003CF2 RID: 15602
		private bool _isEnterEvent;

		// Token: 0x04003CF3 RID: 15603
		private int _index;

		// Token: 0x04003CF4 RID: 15604
		private GameData.Domains.Building.Chicken _chicken;

		// Token: 0x04003CF5 RID: 15605
		private ChickenItem _config;

		// Token: 0x04003CF6 RID: 15606
		private Action<int> _onClick;

		// Token: 0x04003CF7 RID: 15607
		private Action<int> _onClickFeedButton;

		// Token: 0x04003CF8 RID: 15608
		private Action<int> _onClickClear;

		// Token: 0x04003CF9 RID: 15609
		[SerializeField]
		private CButton mainButton;

		// Token: 0x04003CFA RID: 15610
		[SerializeField]
		private GameObject selected;

		// Token: 0x04003CFB RID: 15611
		[SerializeField]
		private CImage chickenImage;

		// Token: 0x04003CFC RID: 15612
		[SerializeField]
		private TextMeshProUGUI nameLabel;

		// Token: 0x04003CFD RID: 15613
		[SerializeField]
		private TextMeshProUGUI personalityNameLabel;

		// Token: 0x04003CFE RID: 15614
		[SerializeField]
		private GameObject content;

		// Token: 0x04003CFF RID: 15615
		[SerializeField]
		private GameObject empty;

		// Token: 0x04003D00 RID: 15616
		[SerializeField]
		private CImage personalityIcon;

		// Token: 0x04003D01 RID: 15617
		[SerializeField]
		private TextMeshProUGUI personalityCountLabel;

		// Token: 0x04003D02 RID: 15618
		[SerializeField]
		private CButton feedButton;

		// Token: 0x04003D03 RID: 15619
		[SerializeField]
		private CImage happinessCircle;

		// Token: 0x04003D04 RID: 15620
		[SerializeField]
		private SpriteLabel happinessLabel;

		// Token: 0x04003D05 RID: 15621
		[SerializeField]
		private GameObject leaveMark;

		// Token: 0x04003D06 RID: 15622
		[SerializeField]
		private GameObject personalityBack;

		// Token: 0x04003D07 RID: 15623
		[SerializeField]
		private CImage roleIcon;

		// Token: 0x04003D08 RID: 15624
		[SerializeField]
		private CButton btnClear;

		// Token: 0x04003D09 RID: 15625
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x04003D0A RID: 15626
		[SerializeField]
		private SkeletonGraphic skeleton;

		// Token: 0x04003D0B RID: 15627
		[SerializeField]
		private CImage gradeImage;
	}
}
