using System;
using System.Collections;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter.Item.Apply;
using Game.Components.Switch;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Story.SectMainStory;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract.Emei
{
	// Token: 0x020009EE RID: 2542
	public class EmeiAddBonusProgress : MonoBehaviour
	{
		// Token: 0x17000DB1 RID: 3505
		// (get) Token: 0x06007CFE RID: 31998 RVA: 0x003A132F File Offset: 0x0039F52F
		// (set) Token: 0x06007CFF RID: 31999 RVA: 0x003A1337 File Offset: 0x0039F537
		public List<ItemDisplayData> Items
		{
			get
			{
				return this._items;
			}
			set
			{
				this._items = value;
				this.RefreshItems();
				this.RefreshButtonConfirm();
			}
		}

		// Token: 0x06007D00 RID: 32000 RVA: 0x003A134F File Offset: 0x0039F54F
		private void OnDisable()
		{
			this.Set(ViewEmeiCombatSkillSpecialBreak.InvalidBonus);
		}

		// Token: 0x06007D01 RID: 32001 RVA: 0x003A1360 File Offset: 0x0039F560
		public void Init(Action<short, bool, List<ItemKey>> confirmAction)
		{
			this._confirmAction = confirmAction;
			this.scroll.CustomAmountDataGenerator = new Func<ITradeableContent, string>(this.AmountCellDataGenerator);
			this.scroll.Init("EmeiSelectItem", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnRenderItem), new Action<ITradeableContent, RowItemLine>(this.OnClickItem), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.SpecialBreakProgress, EmeiAddBonusProgress.ItemColumnLayoutOptions, null, null);
			this.btnConfirm.ClearAndAddListener(new Action(this.ShowDialogAndConfirm));
			this.switchSelected.onValueChanged.RemoveAllListeners();
			this.switchSelected.onValueChanged.AddListener(delegate(bool _)
			{
				this.RefreshItems();
			});
			this.switchFit.onValueChanged.RemoveAllListeners();
			this.switchFit.onValueChanged.AddListener(delegate(bool _)
			{
				this.RefreshItems();
			});
			this.RefreshButtonConfirm();
		}

		// Token: 0x06007D02 RID: 32002 RVA: 0x003A143F File Offset: 0x0039F63F
		public void Set(SectEmeiBreakBonusData bonus)
		{
			this.SelectedBonus = ((this.SelectedBonus == bonus.TemplateId) ? -1 : bonus.TemplateId);
			this._progress = bonus.BonusProgress;
			this._count = bonus.BonusCount;
			this.RefreshBonus();
		}

		// Token: 0x06007D03 RID: 32003 RVA: 0x003A1480 File Offset: 0x0039F680
		public void RefreshBonus()
		{
			bool hasContent = this.SelectedBonus >= 0;
			bool flag = this._animRoutine != null;
			if (flag)
			{
				base.StopCoroutine(this._animRoutine);
				this._animRoutine = null;
			}
			this.content.SetActive(hasContent);
			this.noContent.SetActive(!hasContent);
			this.scrollContent.SetActive(hasContent);
			this.scrollNoContent.SetActive(!hasContent);
			EmeiAddBonusProgress._selectedItems.Clear();
			this.RefreshProgress();
			this.RefreshItems();
			this.RefreshButtonConfirm();
		}

		// Token: 0x06007D04 RID: 32004 RVA: 0x003A1518 File Offset: 0x0039F718
		public void AnimProgressBar(SectEmeiBreakBonusData bonus)
		{
			this.mask.SetActive(true);
			this._animRoutine = base.StartCoroutine(this.Animate(bonus.BonusCount - this._count, (float)this._progress / (float)GlobalConfig.Instance.SectStoryEmeiBonusProgressPerCount, (float)bonus.BonusProgress / (float)GlobalConfig.Instance.SectStoryEmeiBonusProgressPerCount, delegate
			{
				this.progressBarPreview.fillAmount = 0f;
				this.SelectedBonus = bonus.TemplateId;
				this._progress = bonus.BonusProgress;
				this._count = bonus.BonusCount;
				this.RefreshBonus();
				this.mask.SetActive(false);
				this._animRoutine = null;
			}));
		}

		// Token: 0x06007D05 RID: 32005 RVA: 0x003A15A2 File Offset: 0x0039F7A2
		private IEnumerator Animate(int increaseCount, float startPercent, float endPercent, Action onComplete)
		{
			float timePerStage = 2f / (float)(increaseCount + 1);
			float target = (increaseCount > 0) ? 1f : endPercent;
			yield return base.StartCoroutine(this.AnimateProgress(startPercent, target, timePerStage));
			bool flag = increaseCount > 0;
			if (flag)
			{
				int num;
				for (int i = 0; i < increaseCount - 1; i = num + 1)
				{
					yield return base.StartCoroutine(this.AnimateProgress(0f, 1f, timePerStage));
					num = i;
				}
				yield return base.StartCoroutine(this.AnimateProgress(0f, endPercent, timePerStage));
			}
			if (onComplete != null)
			{
				onComplete();
			}
			yield break;
		}

		// Token: 0x06007D06 RID: 32006 RVA: 0x003A15CE File Offset: 0x0039F7CE
		private IEnumerator AnimateProgress(float start, float end, float duration)
		{
			float elapsed = 0f;
			while (elapsed < duration)
			{
				elapsed += Time.deltaTime;
				float t = elapsed / duration;
				this.progressBar.fillAmount = Mathf.Lerp(start, end, t);
				yield return null;
			}
			this.progressBar.fillAmount = end;
			yield break;
		}

		// Token: 0x06007D07 RID: 32007 RVA: 0x003A15F4 File Offset: 0x0039F7F4
		private void RefreshItems()
		{
			this._filteredItems.Clear();
			bool flag = this.Items != null;
			if (flag)
			{
				foreach (ItemDisplayData data in this.Items)
				{
					bool isOn = this.switchSelected.isOn;
					if (isOn)
					{
						bool flag2 = !EmeiAddBonusProgress._selectedItems.ContainsKey(data.RealKey);
						if (flag2)
						{
							continue;
						}
					}
					bool isOn2 = this.switchFit.isOn;
					if (isOn2)
					{
						bool flag3 = !this.IsFit(data.RealKey);
						if (flag3)
						{
							continue;
						}
					}
					this._filteredItems.Add(data);
				}
			}
			bool flag4 = this.SelectedBonus < 0;
			if (flag4)
			{
				for (int i = 0; i < this.fitTypes.childCount; i++)
				{
					this.fitTypes.GetChild(i).gameObject.SetActive(false);
				}
				foreach (ItemDisplayData data2 in this.Items)
				{
					data2.SpecialBreakProgress = 0;
				}
			}
			else
			{
				int index = 0;
				SkillBreakPlateGridBonusTypeItem config = SkillBreakPlateGridBonusType.Instance[this.SelectedBonus];
				bool flag5 = config.ExtraBonusFitCombatSkillTypes != null;
				if (flag5)
				{
					foreach (sbyte type in config.ExtraBonusFitCombatSkillTypes)
					{
						Transform obj = this.fitTypes.GetChild(index++);
						obj.GetComponent<CImage>().SetSprite(string.Format("{0}{1}", "ui9_back_combatskill_small_1_", type), false, null);
						obj.gameObject.SetActive(true);
					}
				}
				bool flag6 = config.ExtraBonusFitLifeSkillTypes != null;
				if (flag6)
				{
					foreach (sbyte type2 in config.ExtraBonusFitLifeSkillTypes)
					{
						Transform obj2 = this.fitTypes.GetChild(index++);
						obj2.GetComponent<CImage>().SetSprite(string.Format("{0}{1}", "ui9_back_lifeskill_small_1_", type2), false, null);
						obj2.gameObject.SetActive(true);
					}
				}
				for (int j = index; j < this.fitTypes.childCount; j++)
				{
					this.fitTypes.GetChild(j).gameObject.SetActive(false);
				}
				foreach (ItemDisplayData data3 in this.Items)
				{
					data3.SpecialBreakProgress = SectMainStorySharedMethods.CalcEmeiBonusItemProgress(this.SelectedBonus, data3);
				}
			}
			this.scroll.SetItemList(this._filteredItems);
		}

		// Token: 0x06007D08 RID: 32008 RVA: 0x003A1910 File Offset: 0x0039FB10
		private void RefreshProgress()
		{
			bool flag = this.SelectedBonus < 0;
			if (!flag)
			{
				int addProgress = this.GetAddProgress();
				SkillBreakPlateGridBonusTypeItem config = SkillBreakPlateGridBonusType.Instance[this.SelectedBonus];
				int progress = this._progress + addProgress;
				int count = progress / GlobalConfig.Instance.SectStoryEmeiBonusProgressPerCount;
				this.RefreshProgressBar(this._progress, addProgress);
				int progressCurr = this._progress * 100 / GlobalConfig.Instance.SectStoryEmeiBonusProgressPerCount;
				int progressAdd = Math.Max(0, progress % GlobalConfig.Instance.SectStoryEmeiBonusProgressPerCount - this._progress) * 100 / GlobalConfig.Instance.SectStoryEmeiBonusProgressPerCount;
				this.bonusImg.SetSprite(string.Format("{0}_{1}", "ui9_back_special_break_title", config.FilterGroup), false, null);
				this.bonusTitle.text = config.Name.SetColor(ViewEmeiCombatSkillSpecialBreak.GetColor(config.FilterGroup));
				this.bonusProgress.text = ((addProgress > 0) ? (progressCurr.ToString() + "%" + ("+" + progressAdd.ToString() + "%").SetColor("brightblue")) : (progressCurr.ToString() + "%"));
				this.bonusCount.text = ((count > 0) ? (this._count.ToString() + ("+" + count.ToString()).SetColor("brightblue")) : (this._count.ToString() ?? ""));
				this.countTextLabel.text = ((count > 0) ? LanguageKey.LK_CombatSkill_SpecialBreak_ItemSelect_SubTitle4.Tr() : LanguageKey.LK_CombatSkill_SpecialBreak_ItemSelect_SubTitle3.Tr());
				TooltipInvoker tips = this.content.GetComponent<TooltipInvoker>();
				TooltipInvoker tooltipInvoker = tips;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				tips.RuntimeParam.Set("arg0", config.Name);
				tips.RuntimeParam.Set("arg1", config.Desc);
			}
		}

		// Token: 0x06007D09 RID: 32009 RVA: 0x003A1B1C File Offset: 0x0039FD1C
		private void RefreshProgressBar(int curr, int add)
		{
			float total = (float)(curr + add) / (float)GlobalConfig.Instance.SectStoryEmeiBonusProgressPerCount;
			bool flag = total >= 1f;
			if (flag)
			{
				this.progressBar.fillAmount = 0f;
				this.progressBarPreview.fillAmount = total % 1f;
			}
			else
			{
				this.progressBar.fillAmount = (float)(curr % GlobalConfig.Instance.SectStoryEmeiBonusProgressPerCount) / (float)GlobalConfig.Instance.SectStoryEmeiBonusProgressPerCount;
				this.progressBarPreview.fillAmount = (float)(add % GlobalConfig.Instance.SectStoryEmeiBonusProgressPerCount) / (float)GlobalConfig.Instance.SectStoryEmeiBonusProgressPerCount;
			}
		}

		// Token: 0x06007D0A RID: 32010 RVA: 0x003A1BC0 File Offset: 0x0039FDC0
		private void RefreshButtonConfirm()
		{
			TooltipInvoker tips = this.btnConfirm.GetComponent<TooltipInvoker>();
			bool isEmpty = EmeiAddBonusProgress._selectedItems.Count == 0;
			this.btnConfirm.interactable = (!isEmpty && this.SelectedBonus >= 0);
			tips.enabled = isEmpty;
			bool flag = isEmpty;
			if (flag)
			{
				TooltipInvoker tooltipInvoker = tips;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				tips.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_CombatSkill_SpecialBreak_ItemSelect_Tip2).SetColor("brightred"));
			}
		}

		// Token: 0x06007D0B RID: 32011 RVA: 0x003A1C54 File Offset: 0x0039FE54
		private void OnRenderItem(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, false);
			rowItemMain.SetFavoriteStatus(this.IsFit(itemData.RealKey) ? RowItemMain.FavoriteStatus.Love : RowItemMain.FavoriteStatus.None);
			rowItemLine.SetSelected(EmeiAddBonusProgress._selectedItems.ContainsKey(itemData.RealKey));
			bool interactable = !itemData.IsLocked;
			rowItemLine.SetInteractable(interactable, true);
			rowItemLine.SetDisabled(!interactable);
		}

		// Token: 0x06007D0C RID: 32012 RVA: 0x003A1CC8 File Offset: 0x0039FEC8
		private void OnClickItem(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			bool flag = EmeiAddBonusProgress._selectedItems.ContainsKey(itemData.RealKey);
			if (flag)
			{
				this.OnRefreshScroll(itemData.Clone(-1));
			}
			else
			{
				bool flag2 = itemData.Amount > 1;
				if (flag2)
				{
					this.scroll.SetItemToSelectCountMode(rowItemLine, delegate(int count)
					{
						this.OnRefreshScroll(itemData.Clone(count));
					}, delegate
					{
						this.OnRefreshScroll(null);
					}, 0, 0, 1, null, false, null, false);
				}
				else
				{
					this.OnRefreshScroll(itemData.Clone(-1));
				}
			}
		}

		// Token: 0x06007D0D RID: 32013 RVA: 0x003A1D6C File Offset: 0x0039FF6C
		private void SetSelectedItem(ITradeableContent itemData)
		{
			bool flag = !EmeiAddBonusProgress._selectedItems.TryAdd(itemData.RealKey, itemData);
			if (flag)
			{
				EmeiAddBonusProgress._selectedItems.Remove(itemData.RealKey);
			}
		}

		// Token: 0x06007D0E RID: 32014 RVA: 0x003A1DA4 File Offset: 0x0039FFA4
		private void OnRefreshScroll(ITradeableContent data = null)
		{
			bool flag = data != null;
			if (flag)
			{
				this.SetSelectedItem(data);
			}
			this.scroll.ReRender();
			this.RefreshButtonConfirm();
			this.RefreshProgress();
		}

		// Token: 0x06007D0F RID: 32015 RVA: 0x003A1DDC File Offset: 0x0039FFDC
		private void ShowDialogAndConfirm()
		{
			DialogCmd dialogCmd = new DialogCmd
			{
				Type = 1,
				Title = LocalStringManager.Get(LanguageKey.LK_CombatSkill_SpecialBreak_AddBonusProgress_Confirm),
				Content = LocalStringManager.GetFormat(LanguageKey.LK_CombatSkill_SpecialBreak_AddBonusProgress_Confirm_Dialog, Character.DefValue.EmeiWhiteGibbon.GivenName).ColorReplace(),
				Yes = new Action(this.Confirm)
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x06007D10 RID: 32016 RVA: 0x003A1E64 File Offset: 0x003A0064
		private void Confirm()
		{
			List<ItemKey> items = new List<ItemKey>();
			foreach (KeyValuePair<ItemKey, ITradeableContent> keyValuePair in EmeiAddBonusProgress._selectedItems)
			{
				ItemKey itemKey;
				ITradeableContent tradeableContent;
				keyValuePair.Deconstruct(out itemKey, out tradeableContent);
				ItemKey key = itemKey;
				ITradeableContent data = tradeableContent;
				bool flag = data.Amount > 1;
				if (flag)
				{
					for (int i = 0; i < data.Amount; i++)
					{
						items.Add(key);
					}
				}
				else
				{
					items.Add(key);
				}
			}
			this._confirmAction(this.SelectedBonus, this._progress + this.GetAddProgress() >= GlobalConfig.Instance.SectStoryEmeiBonusProgressPerCount, items);
		}

		// Token: 0x06007D11 RID: 32017 RVA: 0x003A1F3C File Offset: 0x003A013C
		private bool IsFit(ItemKey key)
		{
			return this.SelectedBonus >= 0 && SectMainStorySharedMethods.IsEmeiBonusFit(this.SelectedBonus, key);
		}

		// Token: 0x06007D12 RID: 32018 RVA: 0x003A1F68 File Offset: 0x003A0168
		private int GetAddProgress()
		{
			int res = 0;
			foreach (ITradeableContent data in EmeiAddBonusProgress._selectedItems.Values)
			{
				res += SectMainStorySharedMethods.CalcEmeiBonusItemProgress(this.SelectedBonus, data as ItemDisplayData) * ((data.Amount > 1) ? data.Amount : 1);
			}
			return res;
		}

		// Token: 0x06007D13 RID: 32019 RVA: 0x003A1FEC File Offset: 0x003A01EC
		private string AmountCellDataGenerator(ITradeableContent item)
		{
			string maxAmountStr = CommonUtils.GetDisplayStringForNum(item.Amount, 100000);
			ITradeableContent data;
			bool flag = !EmeiAddBonusProgress._selectedItems.TryGetValue(item.RealKey, out data);
			string result;
			if (flag)
			{
				result = maxAmountStr;
			}
			else
			{
				string selectedAmountStr = CommonUtils.GetDisplayStringForNum(data.Amount, 100000);
				result = selectedAmountStr + "/" + maxAmountStr;
			}
			return result;
		}

		// Token: 0x04005F0C RID: 24332
		public GameObject content;

		// Token: 0x04005F0D RID: 24333
		public GameObject noContent;

		// Token: 0x04005F0E RID: 24334
		public CImage bonusImg;

		// Token: 0x04005F0F RID: 24335
		public TextMeshProUGUI bonusTitle;

		// Token: 0x04005F10 RID: 24336
		public TextMeshProUGUI bonusCount;

		// Token: 0x04005F11 RID: 24337
		public TextMeshProUGUI bonusProgress;

		// Token: 0x04005F12 RID: 24338
		public TextMeshProUGUI countTextLabel;

		// Token: 0x04005F13 RID: 24339
		public CImage progressBar;

		// Token: 0x04005F14 RID: 24340
		public CImage progressBarPreview;

		// Token: 0x04005F15 RID: 24341
		public CButton btnConfirm;

		// Token: 0x04005F16 RID: 24342
		public ItemListScroll scroll;

		// Token: 0x04005F17 RID: 24343
		public GameObject scrollContent;

		// Token: 0x04005F18 RID: 24344
		public GameObject scrollNoContent;

		// Token: 0x04005F19 RID: 24345
		public Transform fitTypes;

		// Token: 0x04005F1A RID: 24346
		public SwitchToggleSmall switchSelected;

		// Token: 0x04005F1B RID: 24347
		public SwitchToggleSmall switchFit;

		// Token: 0x04005F1C RID: 24348
		public GameObject mask;

		// Token: 0x04005F1D RID: 24349
		private const ItemListScroll.EColumnType Columns = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.SpecialBreakProgress;

		// Token: 0x04005F1E RID: 24350
		private static readonly Dictionary<ItemListScroll.EColumnType, LayoutOption> ItemColumnLayoutOptions = new Dictionary<ItemListScroll.EColumnType, LayoutOption>
		{
			{
				ItemListScroll.EColumnType.Amount,
				new LayoutOption(128f, 0f, 96f, 1)
			},
			{
				ItemListScroll.EColumnType.SpecialBreakProgress,
				new LayoutOption(130f, 0f, 128f, 1)
			}
		};

		// Token: 0x04005F1F RID: 24351
		[NonSerialized]
		public short SelectedBonus = -1;

		// Token: 0x04005F20 RID: 24352
		private Coroutine _animRoutine;

		// Token: 0x04005F21 RID: 24353
		private List<ItemDisplayData> _items;

		// Token: 0x04005F22 RID: 24354
		private int _progress = 0;

		// Token: 0x04005F23 RID: 24355
		private int _count = 0;

		// Token: 0x04005F24 RID: 24356
		private List<ItemDisplayData> _filteredItems = new List<ItemDisplayData>();

		// Token: 0x04005F25 RID: 24357
		private static Dictionary<ItemKey, ITradeableContent> _selectedItems = new Dictionary<ItemKey, ITradeableContent>();

		// Token: 0x04005F26 RID: 24358
		private Action<short, bool, List<ItemKey>> _confirmAction;
	}
}
