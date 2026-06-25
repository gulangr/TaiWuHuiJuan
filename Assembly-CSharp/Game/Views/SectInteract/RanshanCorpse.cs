using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Views.Select;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.SectInteract
{
	// Token: 0x020009B5 RID: 2485
	public class RanshanCorpse : MonoBehaviour
	{
		// Token: 0x06007864 RID: 30820 RVA: 0x0037FF54 File Offset: 0x0037E154
		public void Init(int corpseIndex, List<GeneralLineData> bookKeepTipsData, Action<ItemKey, int, int> onKeepingBookChange, Action<int> onGiveUpClick)
		{
			this._onBookChange = onKeepingBookChange;
			this._bookSlot = new RanshanBookKeepingSlot[this.slots.childCount];
			for (int i = 0; i < this.slots.childCount; i++)
			{
				RanshanBookKeepingSlot slot = this.slots.GetChild(i).GetComponent<RanshanBookKeepingSlot>();
				slot.Init(corpseIndex, i, bookKeepTipsData, new Action<int>(this.OnSelectedBook));
				this._bookSlot[i] = slot;
			}
			this.btnGiveUp.ClearAndAddListener(delegate
			{
				onGiveUpClick(corpseIndex);
			});
			this.btnCorpse.ClearAndAddListener(new Action(this.ShowCharacterMenu));
			this.btnCheck.ClearAndAddListener(new Action(this.ShowGiveUpCharacterMenu));
			this._spine = this.spines.GetChild(corpseIndex).GetComponent<SkeletonGraphic>();
			this._spine.gameObject.SetActive(true);
			this._spine.AnimationState.Complete += delegate(TrackEntry entry)
			{
				bool flag = entry.Animation.Name == "animation";
				if (flag)
				{
					this._spine.AnimationState.SetAnimation(0, "idle", true);
				}
			};
		}

		// Token: 0x06007865 RID: 30821 RVA: 0x00380080 File Offset: 0x0037E280
		public void Set(CharacterDisplayData corpse, CharacterDisplayData target, SectStoryThreeCorpsesCharacter data, int totalBookCount, int totalTargetCount)
		{
			this._target = target;
			this._data = data;
			SectStoryThreeCorpsesCharacter data2 = this._data;
			if (data2.LegendaryBooks == null)
			{
				data2.LegendaryBooks = new List<sbyte>
				{
					-1,
					-1,
					-1,
					-1
				};
			}
			this._totalBookCount = totalBookCount;
			this._totalTargetCount = totalTargetCount;
			for (int i = 0; i < this.slots.childCount; i++)
			{
				this.slots.GetChild(i).GetComponent<RanshanBookKeepingSlot>().Set(this._data.Id);
			}
			this._corpseName = NameCenter.GetMonasticTitleOrDisplayName(corpse, false);
			this.corpseName.text = this._corpseName;
			this.RefreshSlots();
			this.RefreshGiveUp();
		}

		// Token: 0x06007866 RID: 30822 RVA: 0x00380150 File Offset: 0x0037E350
		private void OnSelectedBook(int slotIndex)
		{
			RanshanBookKeepingSlot selectedSlot = this._bookSlot[slotIndex];
			List<SelectedItemData> initialSelectedItem = new List<SelectedItemData>();
			foreach (RanshanBookKeepingSlot slotData in this._bookSlot)
			{
				bool flag = slotData.CurrentTemplateId < 0;
				if (!flag)
				{
					initialSelectedItem.Add(new SelectedItemData(new ItemDisplayData(12, slotData.CurrentTemplateId), 1));
				}
			}
			int validCount = this._bookSlot.Count((RanshanBookKeepingSlot t) => t.GetComponent<CButton>().interactable);
			ExtraDomainMethod.AsyncCall.GetItemListForRanshanTreeCorpsesLegendaryBookKeeping(null, selectedSlot.CorpseId, delegate(int offset, RawDataPool pool)
			{
				List<ItemDisplayData> itemDisplayDataList = null;
				Serializer.Deserialize(pool, offset, ref itemDisplayDataList);
				if (itemDisplayDataList == null)
				{
					itemDisplayDataList = new List<ItemDisplayData>();
				}
				SelectItemConfig config = SelectItemConfig.CreateSingleSelectConfig(new SelectItemRules
				{
					ItemSubType = 1202,
					OnlyFromInventory = false
				}, new SelectItemsCallback(this.OnSelectedBooks), "", new ESelectItemColumnType?(ESelectItemColumnType.IconAndName | ESelectItemColumnType.CombatSkillType | ESelectItemColumnType.AmountWithSelected));
				config.OperationMode = ESelectItemOperationMode.Slot;
				for (int j = 0; j < this._bookSlot.Length; j++)
				{
					RanshanBookKeepingSlot slotData2 = this._bookSlot[j];
					bool flag2 = slotData2.CurrentTemplateId < 0;
					if (!flag2)
					{
						ItemDisplayData currentBook = new ItemDisplayData(12, slotData2.CurrentTemplateId);
						itemDisplayDataList.Insert(0, currentBook);
					}
				}
				config.InitialSelectedItems = initialSelectedItem;
				config.AllowEmpty = true;
				config.ExternalItems = itemDisplayDataList;
				config.HideSortAndFilter = true;
				config.MaxSelectCount = validCount;
				config.HideSourceToggles = true;
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.SetObject("SelectItemConfig", config);
				UIElement.SelectItem.SetOnInitArgs(argBox);
				UIManager.Instance.MaskUI(UIElement.SelectItem);
			});
		}

		// Token: 0x06007867 RID: 30823 RVA: 0x00380218 File Offset: 0x0037E418
		private void OnSelectedBooks(List<SelectedItemData> selectedItems)
		{
			for (int i = 0; i < this._bookSlot.Length; i++)
			{
				RanshanBookKeepingSlot slot = this._bookSlot[i];
				this._onBookChange((i < selectedItems.Count) ? selectedItems[i].ItemData.Key : ItemKey.Invalid, slot.CorpseIndex, slot.SlotIndex);
			}
		}

		// Token: 0x06007868 RID: 30824 RVA: 0x00380284 File Offset: 0x0037E484
		public void ShowBubble(string text)
		{
			this.bubbleText.text = text;
			this.bubble.SetActive(true);
			bool flag = this._closeBubbleCoroutines != null;
			if (flag)
			{
				SingletonObject.getInstance<YieldHelper>().StopYield(this._closeBubbleCoroutines);
			}
			this._closeBubbleCoroutines = SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(5f, new Action(this.HideBubble));
			this._spine.AnimationState.SetAnimation(0, "animation", false);
		}

		// Token: 0x06007869 RID: 30825 RVA: 0x00380304 File Offset: 0x0037E504
		public void HideBubble()
		{
			bool flag = this.bubble != null;
			if (flag)
			{
				this.bubble.SetActive(false);
			}
		}

		// Token: 0x0600786A RID: 30826 RVA: 0x00380330 File Offset: 0x0037E530
		public void ShowCharacterMenu()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CharacterId", this._data.Id);
			argBox.Set("CanOperate", false);
			UIElement.CharacterMenu.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x0600786B RID: 30827 RVA: 0x00380388 File Offset: 0x0037E588
		public void ShowGiveUpCharacterMenu()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CharacterId", this._target.CharacterId);
			argBox.Set("CanOperate", false);
			UIElement.CharacterMenu.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x0600786C RID: 30828 RVA: 0x003803E0 File Offset: 0x0037E5E0
		private void RefreshGiveUp()
		{
			bool flag = this._target == null;
			if (flag)
			{
				this.characterMask.SetActive(false);
				this.characterProperties.SetActive(false);
				this.btnCheck.gameObject.SetActive(false);
				this.btnGiveUp.spriteState = this.noTargetSprites;
				this.btnGiveUp.GetComponent<CImage>().sprite = this.noTargetSp;
				bool flag2 = this._totalTargetCount > 0;
				if (flag2)
				{
					this.targetLabelText.text = LanguageKey.LK_LegendaryBook_GiveUp.Tr();
					this.targetLabel.SetActive(true);
					this.invalidLabel.SetActive(false);
					this.btnGiveUp.interactable = true;
				}
				else
				{
					this.targetLabel.SetActive(false);
					this.invalidLabel.SetActive(true);
					this.btnGiveUp.interactable = false;
				}
				this.btnTips.Type = TipType.LegendaryBookGiveUp;
			}
			else
			{
				string targetName = NameCenter.GetMonasticTitleOrDisplayName(this._target, false);
				Dictionary<int, int> giveUpCount = this._data.GiveUpCount;
				int count = (giveUpCount != null) ? giveUpCount.GetValueOrDefault(this._target.CharacterId, 0) : 0;
				string notch = LocalStringManager.Get(string.Format("LK_LegendaryBook_GiveUp_NotchButton_{0}", (int)(this._data.Notch + 1)));
				string happinessStr = CommonUtils.GetHappinessString(HappinessType.GetHappinessType(this._target.Happiness));
				string duration = string.Format("{0}/{1}", this._data.EndDate - SingletonObject.getInstance<BasicGameData>().CurrDate, RanshanGiveupLegendaryBook.Instance[(int)this._data.Notch].FollowDuration);
				this.btnGiveUp.interactable = false;
				this.btnGiveUp.spriteState = this.hasTargetSprites;
				this.btnGiveUp.GetComponent<CImage>().sprite = this.hasTargetSp;
				this.targetLabelText.text = targetName;
				this.btnCheck.gameObject.SetActive(true);
				this.targetLabel.SetActive(true);
				this.invalidLabel.SetActive(false);
				this.characterMask.SetActive(true);
				this.characterProperties.SetActive(true);
				this.avatar.Refresh(this._target, true);
				this.happiness.SetSprite(string.Format("{0}{1}", "ui9_icon_happiness_big_", HappinessType.GetHappinessType(this._target.Happiness)), false, null);
				TMP_Text tmp_Text = this.countText;
				Dictionary<int, int> giveUpCount2 = this._data.GiveUpCount;
				tmp_Text.text = ((giveUpCount2 != null) ? giveUpCount2.GetValueOrDefault(this._target.CharacterId, 0) : 0).ToString();
				this.durationText.text = Math.Max(0, this._data.EndDate - SingletonObject.getInstance<BasicGameData>().CurrDate).ToString();
				this.btnTips.Type = TipType.Simple;
				this.btnTips.PresetParam = new string[]
				{
					LanguageKey.LK_LegendaryBook_GiveUp.Tr(),
					LanguageKey.LK_LegendaryBook_GiveUp_Tip_Content.TrFormat(new object[]
					{
						this._corpseName,
						targetName,
						notch,
						count,
						happinessStr,
						duration
					})
				};
				this.countTips.PresetParam = new string[]
				{
					LanguageKey.LK_LegendaryBook_GiveUp_Tip_Count.TrFormat(count)
				};
				this.happinessTips.PresetParam = new string[]
				{
					LanguageKey.LK_LegendaryBook_GiveUp_Tip_Happiness.TrFormat(happinessStr)
				};
				this.durationTips.PresetParam = new string[]
				{
					LanguageKey.LK_LegendaryBook_GiveUp_Tip_Duration.TrFormat(duration)
				};
			}
		}

		// Token: 0x0600786D RID: 30829 RVA: 0x00380790 File Offset: 0x0037E990
		public void RefreshSlots()
		{
			for (int i = 0; i < this.slots.childCount; i++)
			{
				this.RefreshSlot(i);
			}
		}

		// Token: 0x0600786E RID: 30830 RVA: 0x003807C0 File Offset: 0x0037E9C0
		private void RefreshSlot(int slotIndex)
		{
			RanshanBookKeepingSlot slot = this.slots.GetChild(slotIndex).GetComponent<RanshanBookKeepingSlot>();
			bool isLocked = slotIndex >= 2 && !this._data.IsUpgraded;
			short templateId = (this._data.LegendaryBooks[slotIndex] >= 0) ? ((short)((int)this._data.LegendaryBooks[slotIndex] + 240)) : ((short)this._data.LegendaryBooks[slotIndex]);
			slot.Refresh(templateId, isLocked, this.GetInvalid());
		}

		// Token: 0x0600786F RID: 30831 RVA: 0x00380848 File Offset: 0x0037EA48
		public bool TryRemove(sbyte type)
		{
			for (int i = 0; i < this.slots.childCount; i++)
			{
				bool flag = this._data.LegendaryBooks[i] == type;
				if (flag)
				{
					this._data.LegendaryBooks[i] = -1;
					this.RefreshSlot(i);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06007870 RID: 30832 RVA: 0x003808AF File Offset: 0x0037EAAF
		public void SetType(sbyte type, int slotIndex)
		{
			this._data.LegendaryBooks[slotIndex] = type;
			this.RefreshSlot(slotIndex);
		}

		// Token: 0x06007871 RID: 30833 RVA: 0x003808D0 File Offset: 0x0037EAD0
		private bool GetInvalid()
		{
			int count = this._totalBookCount;
			foreach (sbyte type in this._data.LegendaryBooks)
			{
				bool flag = type != -1;
				if (flag)
				{
					count--;
				}
			}
			return count <= 0;
		}

		// Token: 0x04005AFE RID: 23294
		public Transform spines;

		// Token: 0x04005AFF RID: 23295
		public TextMeshProUGUI corpseName;

		// Token: 0x04005B00 RID: 23296
		public Transform slots;

		// Token: 0x04005B01 RID: 23297
		public GameObject bubble;

		// Token: 0x04005B02 RID: 23298
		public TextMeshProUGUI bubbleText;

		// Token: 0x04005B03 RID: 23299
		public CButton btnGiveUp;

		// Token: 0x04005B04 RID: 23300
		public CButton btnCorpse;

		// Token: 0x04005B05 RID: 23301
		public CButton btnCheck;

		// Token: 0x04005B06 RID: 23302
		public CImage happiness;

		// Token: 0x04005B07 RID: 23303
		public TextMeshProUGUI countText;

		// Token: 0x04005B08 RID: 23304
		public TextMeshProUGUI durationText;

		// Token: 0x04005B09 RID: 23305
		public TextMeshProUGUI targetLabelText;

		// Token: 0x04005B0A RID: 23306
		public GameObject targetLabel;

		// Token: 0x04005B0B RID: 23307
		public GameObject characterProperties;

		// Token: 0x04005B0C RID: 23308
		public GameObject characterMask;

		// Token: 0x04005B0D RID: 23309
		public GameObject invalidLabel;

		// Token: 0x04005B0E RID: 23310
		public Game.Components.Avatar.Avatar avatar;

		// Token: 0x04005B0F RID: 23311
		public TooltipInvoker btnTips;

		// Token: 0x04005B10 RID: 23312
		public TooltipInvoker countTips;

		// Token: 0x04005B11 RID: 23313
		public TooltipInvoker happinessTips;

		// Token: 0x04005B12 RID: 23314
		public TooltipInvoker durationTips;

		// Token: 0x04005B13 RID: 23315
		public Sprite noTargetSp;

		// Token: 0x04005B14 RID: 23316
		public SpriteState noTargetSprites;

		// Token: 0x04005B15 RID: 23317
		public Sprite hasTargetSp;

		// Token: 0x04005B16 RID: 23318
		public SpriteState hasTargetSprites;

		// Token: 0x04005B17 RID: 23319
		private const int Duration = 5;

		// Token: 0x04005B18 RID: 23320
		private CharacterDisplayData _target;

		// Token: 0x04005B19 RID: 23321
		private SectStoryThreeCorpsesCharacter _data;

		// Token: 0x04005B1A RID: 23322
		private int _totalBookCount;

		// Token: 0x04005B1B RID: 23323
		private int _totalTargetCount;

		// Token: 0x04005B1C RID: 23324
		private SkeletonGraphic _spine;

		// Token: 0x04005B1D RID: 23325
		private Coroutine _closeBubbleCoroutines;

		// Token: 0x04005B1E RID: 23326
		private string _corpseName;

		// Token: 0x04005B1F RID: 23327
		private Action<ItemKey, int, int> _onBookChange;

		// Token: 0x04005B20 RID: 23328
		private RanshanBookKeepingSlot[] _bookSlot;
	}
}
