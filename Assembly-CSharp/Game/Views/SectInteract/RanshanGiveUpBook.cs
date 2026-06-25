using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.Common;
using Game.Views.Select;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Item.Display;
using GameData.Domains.Story.SectMainStory;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract
{
	// Token: 0x020009B6 RID: 2486
	public class RanshanGiveUpBook : MonoBehaviour
	{
		// Token: 0x17000D6A RID: 3434
		// (get) Token: 0x06007873 RID: 30835 RVA: 0x00380951 File Offset: 0x0037EB51
		private SectStoryThreeCorpsesCharacter CurrCorpse
		{
			get
			{
				return this._data.ThreeCorpses[this._corpseIndex];
			}
		}

		// Token: 0x06007874 RID: 30836 RVA: 0x0038096C File Offset: 0x0037EB6C
		public void Init(Action<int, sbyte, sbyte> onConfirm)
		{
			for (int i = 0; i < this.spines.childCount; i++)
			{
				SkeletonGraphic spine = this.spines.GetChild(i).GetComponent<SkeletonGraphic>();
				spine.AnimationState.Complete += delegate(TrackEntry entry)
				{
					bool flag = entry.Animation.Name == "animation";
					if (flag)
					{
						spine.AnimationState.SetAnimation(0, "idle", true);
					}
				};
			}
			for (int j = 0; j < 6; j++)
			{
				int type = ECharacterPropertyDisplayType.Strength.ToInt() + j;
				CharacterPropertyDisplayItem config = CharacterPropertyDisplay.Instance[type];
				this.attributes.GetChild(j).GetComponent<TooltipInvoker>().PresetParam = new string[]
				{
					config.Name,
					config.Desc
				};
			}
			this.btnClose.ClearAndAddListener(new Action(this.OnClickClose));
			this.btnPrev.ClearAndAddListener(new Action(this.OnClickPrev));
			this.btnNext.ClearAndAddListener(new Action(this.OnClickNext));
			this.btnSelect.ClearAndAddListener(new Action(this.OnClickSelectChar));
			this.btnConfirm.ClearAndAddListener(new Action(this.OnClickConfirm));
			this.btnCorpse.ClearAndAddListener(delegate
			{
				this.ShowCharacterMenu(this.CurrCorpse.Id);
			});
			this.btnTarget.ClearAndAddListener(delegate
			{
				this.ShowCharacterMenu(this._selectedCharId);
			});
			this.notchToggle.Init(-1);
			this.notchToggle.OnActiveIndexChange += this.OnNotchToggleChange;
			this._onConfirm = onConfirm;
		}

		// Token: 0x06007875 RID: 30837 RVA: 0x00380B08 File Offset: 0x0037ED08
		public void Set(SectRanshanThreeCorpsesData data, int corpseIndex, ResourceInts resourceInts, int exp)
		{
			this._data = data;
			this._resource = resourceInts;
			this._exp = exp;
			int count = 0;
			foreach (SectStoryThreeCorpsesCharacter corpse in data.ThreeCorpses)
			{
				bool flag = corpse.IsGoodEnd && corpse.Target < 0;
				if (flag)
				{
					count++;
				}
			}
			this.btnNext.interactable = (count > 1);
			this.btnPrev.interactable = (count > 1);
			this.slot.gameObject.SetActive(false);
			this.OnSelectChar(-1);
			this.OnNotchToggleChange(-1, -1);
			this.SetCorpse(corpseIndex);
		}

		// Token: 0x06007876 RID: 30838 RVA: 0x00380BD8 File Offset: 0x0037EDD8
		public void SetCorpse(int corpseIndex)
		{
			this._corpseIndex = corpseIndex;
			for (int i = 0; i < this.spines.childCount; i++)
			{
				this.spines.GetChild(i).gameObject.SetActive(i == this._corpseIndex);
			}
			CharacterDisplayData sourceData = this._data.CharacterDisplayData[this.CurrCorpse.Id];
			string sourceName = NameCenter.GetMonasticTitleOrDisplayName(sourceData, false);
			this.corpseName.text = sourceName;
			this.desc.text = LocalStringManager.Get(string.Format("LK_LegendaryBook_GiveUp_Desc_Corpse{0}", this._corpseIndex + 1));
			this.desc.GetComponent<TMPTextSpriteHelper>().Parse();
			RanshanGiveupLegendaryBookItem config = this.GetRanshanGiveUpLegendaryBookItem(this.CurrCorpse.TemplateId, (sbyte)this.notchToggle.GetActiveIndex());
			sbyte j = 0;
			while ((int)j < this.attributes.childCount)
			{
				Transform attribute = this.attributes.GetChild((int)j);
				attribute.GetChild(attribute.childCount - 1).gameObject.SetActive(config.JudgeAttribute.Contains(j));
				j += 1;
			}
			this.UpdateNotchInfo();
		}

		// Token: 0x06007877 RID: 30839 RVA: 0x00380D14 File Offset: 0x0037EF14
		public void ShowCharacterMenu(int id)
		{
			bool flag = id >= 0;
			if (flag)
			{
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.Set("CharacterId", id);
				argBox.Set("CanOperate", false);
				argBox.Set("PreviousView", 14);
				UIElement.CharacterMenu.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
			}
		}

		// Token: 0x06007878 RID: 30840 RVA: 0x00380D7A File Offset: 0x0037EF7A
		private void UpdateBtnConfirm()
		{
			this.btnConfirm.interactable = (this._selectedCharId >= 0 && this.notchToggle.GetActiveIndex() >= 0 && this._isEnoughResource);
		}

		// Token: 0x06007879 RID: 30841 RVA: 0x00380DA9 File Offset: 0x0037EFA9
		private void OnClickSelectChar()
		{
			this.SelectChar(null);
		}

		// Token: 0x0600787A RID: 30842 RVA: 0x00380DB4 File Offset: 0x0037EFB4
		public void SelectChar(Action callBack)
		{
			bool flag = this._data.LegendaryBookOwners == null || this._data.LegendaryBookOwners.Count == 0;
			if (!flag)
			{
				bool flag2 = UIManager.Instance.IsFocusElement(UIElement.SelectChar);
				if (!flag2)
				{
					List<int> charIds = new List<int>(this._data.LegendaryBookOwners.Keys);
					List<int> charIds2 = charIds;
					object selectedCharIds;
					if (this._selectedCharId < 0)
					{
						selectedCharIds = null;
					}
					else
					{
						(selectedCharIds = new List<int>()).Add(this._selectedCharId);
					}
					VillagerSelectCharacterSelectionHelper.OpenDefaultSelectChar(charIds2, selectedCharIds, delegate(List<int> selectedIds)
					{
						bool flag3 = selectedIds != null && selectedIds.Count > 0;
						if (flag3)
						{
							this.OnSelectChar(selectedIds[0]);
							Action callBack2 = callBack;
							if (callBack2 != null)
							{
								callBack2();
							}
						}
					}, ESelectCharacterInteractionMode.Instant, ESelectCharacterSelectionMode.Single, 1, null, null, null);
				}
			}
		}

		// Token: 0x0600787B RID: 30843 RVA: 0x00380E64 File Offset: 0x0037F064
		public void OnSelectChar(int charId)
		{
			this._selectedCharId = charId;
			this.btnTarget.gameObject.SetActive(this._selectedCharId >= 0);
			bool flag = this._selectedCharId < 0;
			if (flag)
			{
				this.slot.gameObject.SetActive(false);
				this.targetName.text = "";
				this.targetAvatar.gameObject.SetActive(false);
				for (int i = 0; i < 6; i++)
				{
					this.attributes.GetChild(i).GetComponent<PropertyItem>().Set(string.Format("{0}{1}", "ui9_icon_attribute_major_big_", i), LocalStringManager.Get(string.Format("MonthlyEvent_MainAttributeType{0}", i)), 0.ToString(), null, false);
				}
			}
			else
			{
				this._selectedBook = this._data.LegendaryBookOwners[charId];
				int book = 240 + (int)this._selectedBook;
				CharacterDisplayData displayData = this._data.CharacterDisplayData[charId];
				this.slot.gameObject.SetActive(true);
				this.slot.Refresh((short)book, false, false);
				this.targetName.text = NameCenter.GetMonasticTitleOrDisplayName(displayData, false);
				this.targetAvatar.Refresh(displayData, true);
				this.targetAvatar.gameObject.SetActive(true);
				for (int j = 0; j < 6; j++)
				{
					this.attributes.GetChild(j).GetComponent<PropertyItem>().Set(string.Format("{0}{1}", "ui9_icon_attribute_major_big_", j), LocalStringManager.Get(string.Format("MonthlyEvent_MainAttributeType{0}", j)), this._data.CharacterAttributes[charId][j].ToString(), null, false);
				}
				TooltipInvoker tips = this.slot.GetComponent<TooltipInvoker>();
				tips.Type = TipType.Misc;
				tips.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("TemplateDataOnly", false).SetObject("ItemData", new ItemDisplayData(12, (short)book));
			}
			this.UpdateBtnConfirm();
		}

		// Token: 0x0600787C RID: 30844 RVA: 0x003810AF File Offset: 0x0037F2AF
		private void OnClickPrev()
		{
			this.SetCorpse(this.GetValidCorpseIndex(-1));
		}

		// Token: 0x0600787D RID: 30845 RVA: 0x003810C0 File Offset: 0x0037F2C0
		private void OnClickNext()
		{
			this.SetCorpse(this.GetValidCorpseIndex(1));
		}

		// Token: 0x0600787E RID: 30846 RVA: 0x003810D1 File Offset: 0x0037F2D1
		private void OnClickClose()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x0600787F RID: 30847 RVA: 0x003810E1 File Offset: 0x0037F2E1
		private void OnClickConfirm()
		{
			this._onConfirm(this._corpseIndex, this._selectedBook, (sbyte)this.notchToggle.GetActiveIndex());
			base.gameObject.SetActive(false);
		}

		// Token: 0x06007880 RID: 30848 RVA: 0x00381115 File Offset: 0x0037F315
		private void OnNotchToggleChange(int togNew, int togOld)
		{
			this.UpdateNotchInfo();
		}

		// Token: 0x06007881 RID: 30849 RVA: 0x00381120 File Offset: 0x0037F320
		private void UpdateNotchInfo()
		{
			sbyte notch = (sbyte)this.notchToggle.GetActiveIndex();
			bool flag = !SectStoryThreeCorpsesCharacterNotch.IsValid(notch);
			if (!flag)
			{
				RanshanGiveupLegendaryBookItem config = this.GetRanshanGiveUpLegendaryBookItem(this.CurrCorpse.TemplateId, notch);
				ValueTuple<string, int, int> notchConsume = this.GetNotchConsume(config);
				string costName = notchConsume.Item1;
				int costAmount = notchConsume.Item2;
				int total = notchConsume.Item3;
				this._isEnoughResource = (total >= costAmount);
				string costStr = costAmount.ToString().SetColor(this._isEnoughResource ? "brightblue" : "brightred");
				string totalStr = CommonUtils.GetDisplayStringForNum(total, 100000);
				this.notchDuration.text = config.FollowDuration.ToString();
				this.notchHappiness.text = this._happinessLevel[notch];
				this.notchConsumeTitle.text = LanguageKey.LK_LegendaryBook_GiveUp_Consume.TrFormat(costName);
				this.notchConsumeAmount.text = costStr + "/" + totalStr;
				this.UpdateBtnConfirm();
			}
		}

		// Token: 0x06007882 RID: 30850 RVA: 0x00381224 File Offset: 0x0037F424
		private unsafe ValueTuple<string, int, int> GetNotchConsume(RanshanGiveupLegendaryBookItem config)
		{
			return (config.ExpCost > 0) ? new ValueTuple<string, int, int>(Misc.DefValue.ResourceExp.Name, config.ExpCost, this._exp) : new ValueTuple<string, int, int>(Config.ResourceType.Instance[config.ResourceType].Name, config.ResourceCost, *this._resource[(int)config.ResourceType]);
		}

		// Token: 0x06007883 RID: 30851 RVA: 0x00381290 File Offset: 0x0037F490
		private RanshanGiveupLegendaryBookItem GetRanshanGiveUpLegendaryBookItem(short templateId, sbyte notch)
		{
			RanshanGiveupLegendaryBook instance = RanshanGiveupLegendaryBook.Instance;
			if (!true)
			{
			}
			byte b;
			if (templateId != 698)
			{
				if (templateId != 699)
				{
					b = 6;
				}
				else
				{
					b = 3;
				}
			}
			else
			{
				b = 0;
			}
			if (!true)
			{
			}
			return instance[(int)(b + (byte)notch)];
		}

		// Token: 0x06007884 RID: 30852 RVA: 0x003812DC File Offset: 0x0037F4DC
		private int GetValidCorpseIndex(int delta)
		{
			int index = this._corpseIndex;
			for (;;)
			{
				index += delta;
				bool flag = index == this._corpseIndex;
				if (flag)
				{
					break;
				}
				bool flag2 = index < 0;
				if (flag2)
				{
					index = this._data.ThreeCorpses.Count - 1;
				}
				bool flag3 = index >= this._data.ThreeCorpses.Count;
				if (flag3)
				{
					index = 0;
				}
				SectStoryThreeCorpsesCharacter data = this._data.ThreeCorpses[index];
				bool flag4 = data.IsGoodEnd && data.Target < 0;
				if (flag4)
				{
					goto Block_5;
				}
			}
			return index;
			Block_5:
			return index;
		}

		// Token: 0x04005B21 RID: 23329
		public Transform spines;

		// Token: 0x04005B22 RID: 23330
		public TextMeshProUGUI corpseName;

		// Token: 0x04005B23 RID: 23331
		public TextMeshProUGUI desc;

		// Token: 0x04005B24 RID: 23332
		public Transform attributes;

		// Token: 0x04005B25 RID: 23333
		public TextMeshProUGUI targetName;

		// Token: 0x04005B26 RID: 23334
		public Game.Components.Avatar.Avatar targetAvatar;

		// Token: 0x04005B27 RID: 23335
		public RanshanBookKeepingSlot slot;

		// Token: 0x04005B28 RID: 23336
		public CToggleGroup notchToggle;

		// Token: 0x04005B29 RID: 23337
		public TextMeshProUGUI notchDuration;

		// Token: 0x04005B2A RID: 23338
		public TextMeshProUGUI notchHappiness;

		// Token: 0x04005B2B RID: 23339
		public TextMeshProUGUI notchConsumeTitle;

		// Token: 0x04005B2C RID: 23340
		public TextMeshProUGUI notchConsumeAmount;

		// Token: 0x04005B2D RID: 23341
		public CButton btnClose;

		// Token: 0x04005B2E RID: 23342
		public CButton btnPrev;

		// Token: 0x04005B2F RID: 23343
		public CButton btnNext;

		// Token: 0x04005B30 RID: 23344
		public CButton btnSelect;

		// Token: 0x04005B31 RID: 23345
		public CButton btnConfirm;

		// Token: 0x04005B32 RID: 23346
		public CButton btnCorpse;

		// Token: 0x04005B33 RID: 23347
		public CButton btnTarget;

		// Token: 0x04005B34 RID: 23348
		private SectRanshanThreeCorpsesData _data;

		// Token: 0x04005B35 RID: 23349
		private int _corpseIndex;

		// Token: 0x04005B36 RID: 23350
		private ResourceInts _resource;

		// Token: 0x04005B37 RID: 23351
		private int _exp;

		// Token: 0x04005B38 RID: 23352
		private int _selectedCharId;

		// Token: 0x04005B39 RID: 23353
		private sbyte _selectedBook;

		// Token: 0x04005B3A RID: 23354
		private bool _isEnoughResource;

		// Token: 0x04005B3B RID: 23355
		private Action<int, sbyte, sbyte> _onConfirm;

		// Token: 0x04005B3C RID: 23356
		private Dictionary<sbyte, string> _happinessLevel = new Dictionary<sbyte, string>
		{
			{
				0,
				LanguageKey.LK_Small.Tr()
			},
			{
				1,
				LanguageKey.LK_Mid.Tr()
			},
			{
				2,
				LanguageKey.LK_Big.Tr()
			}
		};
	}
}
