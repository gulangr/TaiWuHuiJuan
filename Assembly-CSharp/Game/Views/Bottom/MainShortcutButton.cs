using System;
using Config;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Building;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Views.Bottom
{
	// Token: 0x02000C33 RID: 3123
	[RequireComponent(typeof(CButton), typeof(CImage))]
	public class MainShortcutButton : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x170010C4 RID: 4292
		// (get) Token: 0x06009EB0 RID: 40624 RVA: 0x004A3E8A File Offset: 0x004A208A
		public bool IsThisActive
		{
			get
			{
				return this._isThisActive;
			}
		}

		// Token: 0x170010C5 RID: 4293
		// (get) Token: 0x06009EB1 RID: 40625 RVA: 0x004A3E92 File Offset: 0x004A2092
		public string NameText
		{
			get
			{
				return this.nameText;
			}
		}

		// Token: 0x170010C6 RID: 4294
		// (get) Token: 0x06009EB2 RID: 40626 RVA: 0x004A3E9A File Offset: 0x004A209A
		public Sprite NormalImage
		{
			get
			{
				return this.normalImage;
			}
		}

		// Token: 0x06009EB3 RID: 40627 RVA: 0x004A3EA2 File Offset: 0x004A20A2
		public void Awake()
		{
			this.button.onClick.ResetListener(new Action(this.OnClick));
			this.normalImage = this.image.sprite;
		}

		// Token: 0x06009EB4 RID: 40628 RVA: 0x004A3ED3 File Offset: 0x004A20D3
		public void OnEnable()
		{
			this.isClick = false;
		}

		// Token: 0x06009EB5 RID: 40629 RVA: 0x004A3EDD File Offset: 0x004A20DD
		public void OnPointerEnter(PointerEventData eventData)
		{
			IMainShortcutButton parent = this._parent;
			if (parent != null)
			{
				parent.OnChildEnter(this);
			}
		}

		// Token: 0x06009EB6 RID: 40630 RVA: 0x004A3EF2 File Offset: 0x004A20F2
		public void OnPointerExit(PointerEventData eventData)
		{
			IMainShortcutButton parent = this._parent;
			if (parent != null)
			{
				parent.OnChildExit(this);
			}
		}

		// Token: 0x06009EB7 RID: 40631 RVA: 0x004A3F08 File Offset: 0x004A2108
		public void Init(IMainShortcutButton parent, byte buttonTemplateId)
		{
			this._isThisActive = false;
			this._parent = parent;
			this.templateId = buttonTemplateId;
			MainMenuButtonItem cfg = MainMenuButton.Instance[this.templateId];
			bool isThisActive;
			if (buttonTemplateId != 0)
			{
				sbyte item = cfg.WorldFunction;
				if (item < 0 || SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock((byte)item))
				{
					BuildingBlockData buildingBlockData;
					isThisActive = (buttonTemplateId != 2 || SingletonObject.getInstance<BuildingModel>().GetBuilding(45, out buildingBlockData));
					goto IL_5C;
				}
			}
			isThisActive = false;
			IL_5C:
			this._isThisActive = isThisActive;
			this.buttonText.text = cfg.Name;
			SpriteState state = this.button.spriteState;
			this.image.SetSprite(cfg.IconPrefix + "_3", false, null);
			state.disabledSprite = this.image.sprite;
			this.image.SetSprite(cfg.IconPrefix + "_4", false, null);
			state.pressedSprite = (state.selectedSprite = this.image.sprite);
			this.image.SetSprite(cfg.IconPrefix + "_1", false, null);
			state.highlightedSprite = this.image.sprite;
			this.button.spriteState = state;
			this.image.SetSprite(cfg.IconPrefix + "_0", false, null);
		}

		// Token: 0x06009EB8 RID: 40632 RVA: 0x004A4060 File Offset: 0x004A2260
		public void SetText(CImage btnImage, TMP_Text btnName, TMP_Text btnSummary, TMP_Text btnDescription)
		{
			MainMenuButtonItem cfg = MainMenuButton.Instance[this.templateId];
			btnImage.sprite = this.image.sprite;
			btnImage.enabled = true;
			btnName.text = cfg.Name;
			btnSummary.text = cfg.Summary;
			btnDescription.text = cfg.Desc;
			this.nameText = cfg.Name;
			this._parent.SetButtonData(this.image.sprite, (int)this.templateId, cfg.Name);
		}

		// Token: 0x06009EB9 RID: 40633 RVA: 0x004A40F0 File Offset: 0x004A22F0
		public void OnClick()
		{
			bool flag = this.isClick;
			if (flag)
			{
				this._parent.CleanData();
			}
			else
			{
				this.isClick = true;
				this.image.sprite = this.button.spriteState.selectedSprite;
				this._parent.OnChildClicked(this);
			}
		}

		// Token: 0x06009EBA RID: 40634 RVA: 0x004A414C File Offset: 0x004A234C
		public int GetTemplateId()
		{
			return (int)this.templateId;
		}

		// Token: 0x06009EBB RID: 40635 RVA: 0x004A4164 File Offset: 0x004A2364
		public void RestoreStatus()
		{
			this.isClick = false;
			this.image.sprite = this.normalImage;
		}

		// Token: 0x04007AC7 RID: 31431
		private IMainShortcutButton _parent;

		// Token: 0x04007AC8 RID: 31432
		[SerializeField]
		private byte templateId;

		// Token: 0x04007AC9 RID: 31433
		[SerializeField]
		private TMP_Text buttonText;

		// Token: 0x04007ACA RID: 31434
		[SerializeField]
		private CButton button;

		// Token: 0x04007ACB RID: 31435
		[SerializeField]
		private CImage image;

		// Token: 0x04007ACC RID: 31436
		private bool isClick;

		// Token: 0x04007ACD RID: 31437
		private bool _isThisActive = false;

		// Token: 0x04007ACE RID: 31438
		private Sprite normalImage;

		// Token: 0x04007ACF RID: 31439
		private string nameText;
	}
}
