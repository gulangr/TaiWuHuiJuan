using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.Select;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Views.SectInteract.Shaolin
{
	// Token: 0x020009E8 RID: 2536
	public class ShaolinLuohanBreak : MonoBehaviour
	{
		// Token: 0x06007C81 RID: 31873 RVA: 0x0039DD0C File Offset: 0x0039BF0C
		public void Init(Action<sbyte> onSelect)
		{
			this._onSelect = onSelect;
			PointerTrigger pointerTrigger = base.GetComponent<PointerTrigger>();
			pointerTrigger.EnterEvent.RemoveAllListeners();
			pointerTrigger.EnterEvent.AddListener(new UnityAction(this.SetHoverVisible));
			pointerTrigger.ExitEvent.RemoveAllListeners();
			pointerTrigger.ExitEvent.AddListener(new UnityAction(this.SetHoverInvisible));
			base.GetComponent<CButton>().ClearAndAddListener(new Action(this.OnClickButton));
			this.btnCancel.ClearAndAddListener(new Action(this.OnClickCancel));
		}

		// Token: 0x06007C82 RID: 31874 RVA: 0x0039DDA0 File Offset: 0x0039BFA0
		public void SetData(List<sbyte> data)
		{
			CButton button = base.GetComponent<CButton>();
			bool flag = data == null || data.Count == 0;
			if (flag)
			{
				button.interactable = false;
				this.nameBack.SetActive(false);
			}
			else
			{
				button.interactable = true;
				this.nameBack.SetActive(true);
				this._canSelect.Clear();
				foreach (sbyte templateId in data)
				{
					this._canSelect.Add(templateId, new ItemDisplayData(2, Luohan.Instance[templateId].Accessory));
				}
			}
		}

		// Token: 0x06007C83 RID: 31875 RVA: 0x0039DE64 File Offset: 0x0039C064
		public void Set(sbyte templateId)
		{
			this._templateId = templateId;
			bool flag = this._templateId >= 0;
			if (flag)
			{
				LuohanItem config = Luohan.Instance[templateId];
				SpriteState spriteState = base.GetComponent<CButton>().spriteState;
				spriteState.highlightedSprite = this.normalHover;
				base.GetComponent<CButton>().spriteState = spriteState;
				this.back.sprite = this.normalBack;
				this.nameLabel.text = config.Name.SetColor(Colors.Instance.GradeColors[8]);
				this.icon.SetSprite(Accessory.Instance[config.Accessory].Icon, false, null);
				this.icon.gameObject.SetActive(true);
				this.btnCancel.gameObject.SetActive(true);
			}
			else
			{
				SpriteState spriteState2 = base.GetComponent<CButton>().spriteState;
				spriteState2.highlightedSprite = this.emptyHover;
				base.GetComponent<CButton>().spriteState = spriteState2;
				this.back.sprite = this.emptyBack;
				this.nameLabel.text = LanguageKey.LK_CombatSkill_Luohan_Break_Empty.Tr();
				this.icon.gameObject.SetActive(false);
				this.btnCancel.gameObject.SetActive(false);
			}
		}

		// Token: 0x06007C84 RID: 31876 RVA: 0x0039DFB8 File Offset: 0x0039C1B8
		private void SetHoverVisible()
		{
			bool flag = this._templateId >= 0;
			if (flag)
			{
				this.hover.SetActive(true);
			}
		}

		// Token: 0x06007C85 RID: 31877 RVA: 0x0039DFE4 File Offset: 0x0039C1E4
		private void SetHoverInvisible()
		{
			bool flag = this._templateId >= 0;
			if (flag)
			{
				this.hover.SetActive(false);
			}
		}

		// Token: 0x06007C86 RID: 31878 RVA: 0x0039E010 File Offset: 0x0039C210
		private void OnClickButton()
		{
			SelectItemConfig config = SelectItemConfig.CreateSingleSelectConfig(new SelectItemRules
			{
				ItemSubType = 200,
				OnlyFromInventory = true
			}, new SelectItemsCallback(this.OnConfirm), "", new ESelectItemColumnType?(ESelectItemColumnType.IconAndName));
			config.HideSourceToggles = true;
			bool flag = this._templateId >= 0;
			if (flag)
			{
				config.InitialSelectedItems = new List<SelectedItemData>
				{
					new SelectedItemData(this._canSelect[this._templateId], 1)
				};
				config.AllowEmpty = true;
			}
			config.ExternalItems = this._canSelect.Values.ToList<ItemDisplayData>();
			config.HideSortAndFilter = true;
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.SetObject("SelectItemConfig", config);
			UIElement.SelectItem.SetOnInitArgs(argBox);
			UIManager.Instance.MaskUI(UIElement.SelectItem);
		}

		// Token: 0x06007C87 RID: 31879 RVA: 0x0039E0EB File Offset: 0x0039C2EB
		private void OnClickCancel()
		{
			this._onSelect(-1);
			this.SetHoverInvisible();
		}

		// Token: 0x06007C88 RID: 31880 RVA: 0x0039E104 File Offset: 0x0039C304
		private void OnConfirm(List<SelectedItemData> data)
		{
			bool flag = data.Count <= 0;
			if (flag)
			{
				this._onSelect(-1);
			}
			else
			{
				ItemKey key = data[0].ItemData.Key;
				bool flag2 = this._templateId >= 0 && key.TemplateId == Luohan.Instance[this._templateId].Accessory;
				if (flag2)
				{
					this._onSelect(-1);
				}
				else
				{
					foreach (LuohanItem config in ((IEnumerable<LuohanItem>)Luohan.Instance))
					{
						bool flag3 = config.Accessory == key.TemplateId;
						if (flag3)
						{
							this._onSelect(config.TemplateId);
							break;
						}
					}
				}
			}
		}

		// Token: 0x04005EAC RID: 24236
		public CImage back;

		// Token: 0x04005EAD RID: 24237
		public CImage icon;

		// Token: 0x04005EAE RID: 24238
		public GameObject hover;

		// Token: 0x04005EAF RID: 24239
		public GameObject nameBack;

		// Token: 0x04005EB0 RID: 24240
		public TextMeshProUGUI nameLabel;

		// Token: 0x04005EB1 RID: 24241
		public CButton btnCancel;

		// Token: 0x04005EB2 RID: 24242
		public Sprite emptyBack;

		// Token: 0x04005EB3 RID: 24243
		public Sprite normalBack;

		// Token: 0x04005EB4 RID: 24244
		public Sprite emptyHover;

		// Token: 0x04005EB5 RID: 24245
		public Sprite normalHover;

		// Token: 0x04005EB6 RID: 24246
		private sbyte _templateId;

		// Token: 0x04005EB7 RID: 24247
		private Action<sbyte> _onSelect;

		// Token: 0x04005EB8 RID: 24248
		private Dictionary<sbyte, ItemDisplayData> _canSelect = new Dictionary<sbyte, ItemDisplayData>();
	}
}
