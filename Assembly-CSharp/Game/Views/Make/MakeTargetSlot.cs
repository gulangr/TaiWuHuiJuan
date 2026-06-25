using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Item;
using Game.Components.ListStyleGeneralScroll.Item;
using GameData.Domains.Building;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameDataExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Make
{
	// Token: 0x0200095D RID: 2397
	public class MakeTargetSlot : MonoBehaviour
	{
		// Token: 0x17000D02 RID: 3330
		// (get) Token: 0x0600726F RID: 29295 RVA: 0x00352CF3 File Offset: 0x00350EF3
		public TooltipInvoker Tip
		{
			get
			{
				return this.tip;
			}
		}

		// Token: 0x17000D03 RID: 3331
		// (get) Token: 0x06007270 RID: 29296 RVA: 0x00352CFB File Offset: 0x00350EFB
		// (set) Token: 0x06007271 RID: 29297 RVA: 0x00352D03 File Offset: 0x00350F03
		public ItemDisplayData ItemData { get; private set; }

		// Token: 0x17000D04 RID: 3332
		// (get) Token: 0x06007272 RID: 29298 RVA: 0x00352D0C File Offset: 0x00350F0C
		// (set) Token: 0x06007273 RID: 29299 RVA: 0x00352D14 File Offset: 0x00350F14
		public EMakeTargetSlotType SlotType { get; private set; }

		// Token: 0x17000D05 RID: 3333
		// (get) Token: 0x06007274 RID: 29300 RVA: 0x00352D1D File Offset: 0x00350F1D
		// (set) Token: 0x06007275 RID: 29301 RVA: 0x00352D25 File Offset: 0x00350F25
		public EMakeTargetSlotInteract SlotInteract { get; private set; }

		// Token: 0x17000D06 RID: 3334
		// (get) Token: 0x06007276 RID: 29302 RVA: 0x00352D2E File Offset: 0x00350F2E
		// (set) Token: 0x06007277 RID: 29303 RVA: 0x00352D36 File Offset: 0x00350F36
		public bool IsValid { get; private set; }

		// Token: 0x17000D07 RID: 3335
		// (get) Token: 0x06007278 RID: 29304 RVA: 0x00352D3F File Offset: 0x00350F3F
		// (set) Token: 0x06007279 RID: 29305 RVA: 0x00352D4C File Offset: 0x00350F4C
		public bool IsToggleOn
		{
			get
			{
				return this.toggle.isOn;
			}
			set
			{
				this.toggle.SetIsOnWithoutNotify(value);
			}
		}

		// Token: 0x0600727A RID: 29306 RVA: 0x00352D5C File Offset: 0x00350F5C
		public void Init(EMakeTargetSlotInteract slotInteract, EMakeTargetSlotType slotType, Action<int, ItemDisplayData> onCancel, Action onSelect = null, Func<bool> getInteractable = null, Action<bool> onToggleValueChanged = null, int slotIndex = -1, Func<bool> getShowToggle = null, bool isSpecial = false, Action<bool, int> hoverShow = null)
		{
			this.SlotInteract = slotInteract;
			this.SlotType = slotType;
			this._onCancel = onCancel;
			this._onSelect = onSelect;
			this._getInteractable = getInteractable;
			this._slotIndex = slotIndex;
			this._getShowToggle = getShowToggle;
			this._isSpecial = isSpecial;
			this._hoverShow = hoverShow;
			TextMeshProUGUI textMeshProUGUI = this.textTitle;
			if (!true)
			{
			}
			string text;
			switch (slotType)
			{
			case EMakeTargetSlotType.Weapon:
				text = LanguageKey.LK_Profession_ChangeWeaponTrick_Weapon_Title.Tr();
				break;
			case EMakeTargetSlotType.Tool:
				text = LanguageKey.LK_Select_Making_Tool.Tr();
				break;
			case EMakeTargetSlotType.MakeMaterial:
				text = LanguageKey.LK_Select_Making_Material.Tr();
				break;
			case EMakeTargetSlotType.MakeTarget:
				text = LanguageKey.LK_Make_Target.Tr();
				break;
			case EMakeTargetSlotType.RefineTool:
				text = LanguageKey.LK_Select_Making_Tool.Tr();
				break;
			case EMakeTargetSlotType.AddPoisonTarget:
				text = LanguageKey.LK_Add_Poison_Target.Tr();
				break;
			case EMakeTargetSlotType.RemovePoisonTarget:
				text = LanguageKey.LK_Remove_Poison_Target.Tr();
				break;
			case EMakeTargetSlotType.CustomTarget:
				text = LanguageKey.LK_Make_Target.Tr();
				break;
			default:
				throw new ArgumentOutOfRangeException("slotType", slotType, null);
			}
			if (!true)
			{
			}
			textMeshProUGUI.text = text;
			this.button.ClearAndAddListener(new Action(this.OnClick));
			this.toggle.SetIsOnWithoutNotify(false);
			this.toggle.onValueChanged.RemoveAllListeners();
			bool flag = onToggleValueChanged != null;
			if (flag)
			{
				this.toggle.onValueChanged.AddListener(delegate(bool isOn)
				{
					onToggleValueChanged(isOn);
				});
			}
			this.Select(null, false);
			PointerTrigger btnTrigger = this.button.GetComponent<PointerTrigger>();
			btnTrigger.EnterEvent.RemoveAllListeners();
			btnTrigger.EnterEvent.AddListener(new UnityAction(this.OnButtonEnterTrigger));
			btnTrigger.ExitEvent.RemoveAllListeners();
			btnTrigger.ExitEvent.AddListener(new UnityAction(this.OnButtonExitTrigger));
			CButton cbutton = this.removerButton;
			if (cbutton != null)
			{
				cbutton.ClearAndAddListener(new Action(this.ClickRemoveButton));
			}
			bool flag2 = this.goChangeHover != null;
			if (flag2)
			{
				this.goChangeHover.SetActive(false);
			}
			this.goNormalHover.SetActive(false);
		}

		// Token: 0x0600727B RID: 29307 RVA: 0x00352F84 File Offset: 0x00351184
		public void Select(ItemDisplayData itemData, bool isPoison = false)
		{
			bool flag = !isPoison;
			if (flag)
			{
				this.HidePoison();
			}
			this.ItemData = itemData;
			this.IsValid = (itemData != null);
			this.Refresh(isPoison);
		}

		// Token: 0x0600727C RID: 29308 RVA: 0x00352FBC File Offset: 0x003511BC
		public void Refresh(bool isPoison = false)
		{
			bool flag = !isPoison;
			if (flag)
			{
				this.HidePoison();
			}
			CButton cbutton = this.button;
			EMakeTargetSlotInteract slotInteract = this.SlotInteract;
			if (!true)
			{
			}
			bool interactable;
			switch (slotInteract)
			{
			case EMakeTargetSlotInteract.Always:
				interactable = true;
				break;
			case EMakeTargetSlotInteract.Valid:
				interactable = this.IsValid;
				break;
			case EMakeTargetSlotInteract.Custom:
			{
				Func<bool> getInteractable = this._getInteractable;
				interactable = (getInteractable != null && getInteractable());
				break;
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (!true)
			{
			}
			cbutton.interactable = interactable;
			bool isRandomMake = MakeSubPageMakeHelper.CheckIsRandomMake(this.ItemData);
			this.tip.enabled = (this.button.interactable && this.IsValid && !isRandomMake);
			this.tip.HideTips();
			this.itemBack.gameObject.SetActive(this.IsValid && !isPoison);
			this.imageAdd.gameObject.SetActive(!this.IsValid && this._onSelect != null && !isPoison && !this._isSpecial);
			this.imageAdd.sprite = (this.button.interactable ? this.spriteImageAddEnable : this.spriteImageAddDisable);
			this.imageBack.sprite = (this.IsValid ? this.spriteImageBackNormal : this.spriteImageBackEmpty);
			bool flag2 = this.IsValid && this.ItemData.RealKey.HasTemplate && !isRandomMake;
			if (flag2)
			{
				this.itemBack.Set(this.ItemData, true);
				RowItemLine.SetMouseTipDisplayer(true, this.ItemData, this.tip);
			}
			bool isTool = this.SlotType == EMakeTargetSlotType.Tool;
			Func<bool> getShowToggle = this._getShowToggle;
			bool showToggle = getShowToggle != null && getShowToggle();
			bool isShowToggle = isTool || showToggle;
			this.toggle.gameObject.SetActive(isShowToggle);
			this.ShowName(this.IsValid && this.ItemData.RealKey.HasTemplate && !isRandomMake);
			CButton cbutton2 = this.removerButton;
			if (cbutton2 != null)
			{
				cbutton2.gameObject.SetActive(this.IsValid || isRandomMake);
			}
			bool flag3 = this.effectHandler != null;
			if (flag3)
			{
				this.effectHandler.Refresh();
			}
			this.RefreshRefine();
		}

		// Token: 0x0600727D RID: 29309 RVA: 0x00353210 File Offset: 0x00351410
		public void Cancel()
		{
			bool flag = !this.IsValid;
			if (!flag)
			{
				ItemDisplayData itemData = this.ItemData;
				this.Select(null, false);
				this._onCancel(this._slotIndex, itemData);
			}
		}

		// Token: 0x0600727E RID: 29310 RVA: 0x00353250 File Offset: 0x00351450
		private void OnClick()
		{
			bool flag = this.isValidClickToCancel;
			if (flag)
			{
				bool isValid = this.IsValid;
				if (isValid)
				{
					this.Cancel();
				}
				else
				{
					Action onSelect = this._onSelect;
					if (onSelect != null)
					{
						onSelect();
					}
				}
			}
			else
			{
				Action onSelect2 = this._onSelect;
				if (onSelect2 != null)
				{
					onSelect2();
				}
			}
		}

		// Token: 0x0600727F RID: 29311 RVA: 0x003532A5 File Offset: 0x003514A5
		private void ClickRemoveButton()
		{
			this.Cancel();
		}

		// Token: 0x06007280 RID: 29312 RVA: 0x003532B0 File Offset: 0x003514B0
		private void OnButtonEnterTrigger()
		{
			this.goNormalHover.SetActive(true);
			Action<bool, int> hoverShow = this._hoverShow;
			if (hoverShow != null)
			{
				hoverShow(true, this._slotIndex);
			}
			bool flag = this.goChangeHover != null && this.needShowChangeHover;
			if (flag)
			{
				bool isTargetUnsigned = MakeSubPageMakeHelper.CheckIsRandomMake(this.ItemData);
				this.goChangeHover.SetActive(this.IsValid || isTargetUnsigned);
			}
		}

		// Token: 0x06007281 RID: 29313 RVA: 0x00353324 File Offset: 0x00351524
		private void OnButtonExitTrigger()
		{
			this.goNormalHover.SetActive(false);
			Action<bool, int> hoverShow = this._hoverShow;
			if (hoverShow != null)
			{
				hoverShow(false, this._slotIndex);
			}
			bool flag = this.goChangeHover != null;
			if (flag)
			{
				this.goChangeHover.SetActive(false);
			}
		}

		// Token: 0x06007282 RID: 29314 RVA: 0x00353378 File Offset: 0x00351578
		public void ShowName(bool isShow)
		{
			GameObject gameObject = this.rootName;
			if (gameObject != null)
			{
				gameObject.SetActive(isShow && !this.hideName);
			}
			if (isShow)
			{
				TextMeshProUGUI textMeshProUGUI = this.textName;
				if (textMeshProUGUI != null)
				{
					textMeshProUGUI.SetText(this.ItemData.GetName(true), true);
				}
			}
		}

		// Token: 0x06007283 RID: 29315 RVA: 0x003533CC File Offset: 0x003515CC
		public void ChangeButtonAddSprite(MakeTargetSlot.EMakeTargetSlotBtnAddState newState)
		{
			bool isValid = this.IsValid;
			if (!isValid)
			{
				this.imageAdd.sprite = ((newState == MakeTargetSlot.EMakeTargetSlotBtnAddState.Disable) ? this.spriteImageAddDisable : this.spriteImageAddEnable);
				this.imageAdd.gameObject.SetActive(newState != MakeTargetSlot.EMakeTargetSlotBtnAddState.None && !this._isSpecial);
			}
		}

		// Token: 0x06007284 RID: 29316 RVA: 0x00353424 File Offset: 0x00351624
		public void SetEffectHandlerState(bool enable)
		{
			bool flag = this.effectHandler != null;
			if (flag)
			{
				this.effectHandler.IsSlotEnable = enable;
			}
		}

		// Token: 0x06007285 RID: 29317 RVA: 0x0035344F File Offset: 0x0035164F
		public void SetRandomIcon(string spriteKey)
		{
			this.itemBack.SetIcon(spriteKey);
			this.itemBack.SetBack(-1);
		}

		// Token: 0x06007286 RID: 29318 RVA: 0x0035346C File Offset: 0x0035166C
		public void ShowPreviewMakeResultTip(MakeResult makeResult, short materialId)
		{
			this.tip.enabled = true;
			this.tip.Type = TipType.MakeItem;
			string title = ItemTemplateHelper.GetName(5, materialId);
			this.tip.RuntimeParam = new ArgumentBox().SetObject("MakeResult", makeResult).Set("Title", title);
		}

		// Token: 0x06007287 RID: 29319 RVA: 0x003534C8 File Offset: 0x003516C8
		public void SwitchToPoison(bool toPoison)
		{
			this.itemBack.gameObject.SetActive(!toPoison);
			bool flag = this.poison;
			if (flag)
			{
				this.poison.gameObject.SetActive(toPoison);
				this.poison.raycastTarget = toPoison;
			}
		}

		// Token: 0x06007288 RID: 29320 RVA: 0x0035351B File Offset: 0x0035171B
		public void UnverifiedPoison()
		{
			CImage cimage = this.poison;
			if (cimage != null)
			{
				cimage.SetSprite("ui9_icon_poison_not_identified", false, null);
			}
		}

		// Token: 0x06007289 RID: 29321 RVA: 0x00353537 File Offset: 0x00351737
		public void HidePoison()
		{
			CImage cimage = this.poison;
			if (cimage != null)
			{
				cimage.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600728A RID: 29322 RVA: 0x00353552 File Offset: 0x00351752
		public void ShowPoison(sbyte poisonType)
		{
			this.SwitchToPoison(true);
			CImage cimage = this.poison;
			if (cimage != null)
			{
				cimage.SetSprite("ui9_icon_poison_4_" + poisonType.ToString(), false, null);
			}
		}

		// Token: 0x0600728B RID: 29323 RVA: 0x00353584 File Offset: 0x00351784
		public int GetTemplateId()
		{
			bool flag = this.ItemData == null;
			int result;
			if (flag)
			{
				result = -99;
			}
			else
			{
				result = (int)this.ItemData.Key.TemplateId;
			}
			return result;
		}

		// Token: 0x0600728C RID: 29324 RVA: 0x003535B8 File Offset: 0x003517B8
		public void SetRefine(ItemDisplayData data)
		{
			this._refineItemData = data;
		}

		// Token: 0x0600728D RID: 29325 RVA: 0x003535C4 File Offset: 0x003517C4
		private void RefreshRefine()
		{
			bool flag = this.refine == null;
			if (!flag)
			{
				bool flag2 = this._refineItemData == null;
				if (flag2)
				{
					this.refine.SetActive(false);
				}
				else
				{
					bool flag3 = this.ItemData != null;
					bool isShow;
					if (flag3)
					{
						bool flag4 = !this.ItemData.Key.IsValid() && this.ItemData.Key.TemplateId == this._refineItemData.Key.TemplateId;
						isShow = !flag4;
					}
					else
					{
						isShow = true;
					}
					this.refine.SetActive(isShow);
				}
			}
		}

		// Token: 0x040054D5 RID: 21717
		[SerializeField]
		private TextMeshProUGUI textTitle;

		// Token: 0x040054D6 RID: 21718
		[SerializeField]
		private GameObject rootName;

		// Token: 0x040054D7 RID: 21719
		[SerializeField]
		private TextMeshProUGUI textName;

		// Token: 0x040054D8 RID: 21720
		[SerializeField]
		private CButton button;

		// Token: 0x040054D9 RID: 21721
		[SerializeField]
		private TooltipInvoker tip;

		// Token: 0x040054DA RID: 21722
		[SerializeField]
		private ItemBack itemBack;

		// Token: 0x040054DB RID: 21723
		[SerializeField]
		private CImage imageBack;

		// Token: 0x040054DC RID: 21724
		[SerializeField]
		private CImage imageAdd;

		// Token: 0x040054DD RID: 21725
		[SerializeField]
		private CImage poison;

		// Token: 0x040054DE RID: 21726
		[SerializeField]
		private CToggle toggle;

		// Token: 0x040054DF RID: 21727
		[SerializeField]
		private Sprite spriteImageBackEmpty;

		// Token: 0x040054E0 RID: 21728
		[SerializeField]
		private Sprite spriteImageBackNormal;

		// Token: 0x040054E1 RID: 21729
		[SerializeField]
		private Sprite spriteImageAddEnable;

		// Token: 0x040054E2 RID: 21730
		[SerializeField]
		private Sprite spriteImageAddDisable;

		// Token: 0x040054E3 RID: 21731
		[SerializeField]
		public bool hideName = false;

		// Token: 0x040054E4 RID: 21732
		public GameObject goChangeHover;

		// Token: 0x040054E5 RID: 21733
		public GameObject goNormalHover;

		// Token: 0x040054E6 RID: 21734
		public CButton removerButton;

		// Token: 0x040054E7 RID: 21735
		public bool needShowChangeHover = true;

		// Token: 0x040054E8 RID: 21736
		public bool isValidClickToCancel = false;

		// Token: 0x040054E9 RID: 21737
		public MakeSlotEffectHandler effectHandler;

		// Token: 0x040054EE RID: 21742
		private Action<int, ItemDisplayData> _onCancel;

		// Token: 0x040054EF RID: 21743
		private Action _onSelect;

		// Token: 0x040054F0 RID: 21744
		private Func<bool> _getInteractable;

		// Token: 0x040054F1 RID: 21745
		private Func<bool> _getShowToggle;

		// Token: 0x040054F2 RID: 21746
		private int _slotIndex;

		// Token: 0x040054F3 RID: 21747
		private bool _isSpecial;

		// Token: 0x040054F4 RID: 21748
		private Action<bool, int> _hoverShow;

		// Token: 0x040054F5 RID: 21749
		private ItemDisplayData _refineItemData;

		// Token: 0x040054F6 RID: 21750
		[SerializeField]
		private GameObject refine;

		// Token: 0x02001E66 RID: 7782
		public enum EMakeTargetSlotBtnAddState
		{
			// Token: 0x0400C9A2 RID: 51618
			None,
			// Token: 0x0400C9A3 RID: 51619
			Enable,
			// Token: 0x0400C9A4 RID: 51620
			Disable
		}
	}
}
