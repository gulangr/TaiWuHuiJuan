using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Inscription;
using GameData.Domains.Character;
using GameData.Domains.Global;
using GameData.Domains.Global.Inscription;
using GameData.GameDataBridge;
using TMPro;
using UnityEngine;

namespace Game.Views.Main.Inscription
{
	// Token: 0x0200097A RID: 2426
	public class ViewCheckInscription : UIBase
	{
		// Token: 0x06007453 RID: 29779 RVA: 0x00362F78 File Offset: 0x00361178
		public override void OnInit(ArgumentBox argsBox)
		{
			this.closeButton.ClearAndAddListener(new Action(this.OnCloseClicked));
			this.charScroll.OnItemRender += this.OnCharItemRender;
			this._sortAndFilterController = new InscriptionSortAndFilterController(this.sortAndFilter, true);
			this._sortAndFilterController.Init(new Action(this.OnSortFilterChanged), "InscriptionSort");
			bool flag = this.searchingField != null;
			if (flag)
			{
				this.searchingField.onValueChanged.RemoveAllListeners();
				this.searchingField.onValueChanged.AddListener(delegate(string _)
				{
					this.OnSortFilterChanged();
				});
			}
			GEvent.Add(EEvents.InscriptionChange, new GEvent.Callback(this.OnInscriptionChange));
			this.BuildDataAndRefresh();
			GlobalDomainMethod.Call.InvokeGuidingTrigger(113);
		}

		// Token: 0x06007454 RID: 29780 RVA: 0x00363050 File Offset: 0x00361250
		public override void NotifyUIHide()
		{
			base.NotifyUIHide();
			this.charScroll.OnItemRender -= this.OnCharItemRender;
			GEvent.Remove(EEvents.InscriptionChange, new GEvent.Callback(this.OnInscriptionChange));
			this.OnConfirmClicked = null;
			this.OnQuickHide = null;
		}

		// Token: 0x06007455 RID: 29781 RVA: 0x003630A4 File Offset: 0x003612A4
		private void BuildDataAndRefresh()
		{
			this._allCharData.Clear();
			Dictionary<InscribedCharacterKey, InscribedCharacter> inscribedChars = GlobalOperations.InscribedCharacters;
			Dictionary<InscribedCharacterKey, int> pinOrders = GlobalOperations.InscribedCharacterPinOrders;
			foreach (KeyValuePair<InscribedCharacterKey, InscribedCharacter> kv in inscribedChars)
			{
				int order;
				CheckInscriptionCharData data = new CheckInscriptionCharData
				{
					Key = kv.Key,
					Character = kv.Value,
					PinOrder = (pinOrders.TryGetValue(kv.Key, out order) ? order : -1),
					Charm = kv.Value.CalcAttraction(kv.Value.ActualAge, kv.Value.ClothingDisplayId),
					MainAttributeSum = ViewCheckInscription.CalcMainAttributeSum(kv.Value),
					LifeSkillSum = ViewCheckInscription.CalcSkillSum(kv.Value.BaseLifeSkillQualifications),
					CombatSkillSum = ViewCheckInscription.CalcSkillSum(kv.Value.BaseCombatSkillQualifications)
				};
				this._allCharData.Add(data);
			}
			this.ApplySortAndFilter();
		}

		// Token: 0x06007456 RID: 29782 RVA: 0x003631CC File Offset: 0x003613CC
		private void ApplySortAndFilter()
		{
			Func<CheckInscriptionCharData, bool> filter = this._sortAndFilterController.GenerateFilter();
			Comparison<CheckInscriptionCharData> comparer = this._sortAndFilterController.GenerateComparer(this._allCharData);
			this._displayedCharData = this._allCharData.Where(delegate(CheckInscriptionCharData d)
			{
				bool flag5 = !filter(d);
				bool result;
				if (flag5)
				{
					result = false;
				}
				else
				{
					bool flag6 = this.searchingField != null && !string.IsNullOrEmpty(this.searchingField.text);
					if (flag6)
					{
						string fullName = d.Character.Surname + d.Character.GivenName;
						result = fullName.Contains(this.searchingField.text);
					}
					else
					{
						result = true;
					}
				}
				return result;
			}).ToList<CheckInscriptionCharData>();
			this._displayedCharData.Sort(comparer);
			this.charScroll.SetDataCount(this._displayedCharData.Count);
			bool flag = !this._currentSelectedKey.Equals(InscribedCharacterKey.Invalid);
			if (flag)
			{
				int index = this._displayedCharData.FindIndex((CheckInscriptionCharData d) => d.Key.Equals(this._currentSelectedKey));
				bool flag2 = index >= 0;
				if (flag2)
				{
					this.SelectChar(index);
				}
				else
				{
					bool flag3 = this._displayedCharData.Count > 0;
					if (flag3)
					{
						this.SelectChar(0);
					}
					else
					{
						this.detailPanel.Clear();
					}
				}
			}
			else
			{
				bool flag4 = this._displayedCharData.Count > 0;
				if (flag4)
				{
					this.SelectChar(0);
				}
				else
				{
					this.detailPanel.Clear();
				}
			}
			this.emptyGo.SetActive(this._displayedCharData.Count == 0);
			this._sortAndFilterController.AfterFilter(this._allCharData);
		}

		// Token: 0x06007457 RID: 29783 RVA: 0x00363320 File Offset: 0x00361520
		private void OnCharItemRender(int index, GameObject cell)
		{
			bool flag = index < 0 || index >= this._displayedCharData.Count;
			if (!flag)
			{
				CheckInscriptionCharCard card = cell.GetComponent<CheckInscriptionCharCard>();
				bool flag2 = card != null;
				if (flag2)
				{
					card.SetData(this._displayedCharData[index], this._currentSelectedKey, this, false);
				}
			}
		}

		// Token: 0x06007458 RID: 29784 RVA: 0x0036337C File Offset: 0x0036157C
		public void SelectChar(int index)
		{
			bool flag = index < 0 || index >= this._displayedCharData.Count;
			if (!flag)
			{
				this._currentSelectedKey = this._displayedCharData[index].Key;
				this.charScroll.ReRender();
				this.detailPanel.SetData(this._displayedCharData[index]);
			}
		}

		// Token: 0x06007459 RID: 29785 RVA: 0x003633E4 File Offset: 0x003615E4
		public void SelectCharByKey(InscribedCharacterKey key)
		{
			int index = this._displayedCharData.FindIndex((CheckInscriptionCharData d) => d.Key.Equals(key));
			bool flag = index >= 0;
			if (flag)
			{
				this.SelectChar(index);
			}
		}

		// Token: 0x0600745A RID: 29786 RVA: 0x0036342A File Offset: 0x0036162A
		private void OnSortFilterChanged()
		{
			this.ApplySortAndFilter();
		}

		// Token: 0x0600745B RID: 29787 RVA: 0x00363434 File Offset: 0x00361634
		private void OnInscriptionChange(ArgumentBox _ = null)
		{
			this.BuildDataAndRefresh();
		}

		// Token: 0x0600745C RID: 29788 RVA: 0x0036343E File Offset: 0x0036163E
		private void OnCloseClicked()
		{
			Action onQuickHide = this.OnQuickHide;
			if (onQuickHide != null)
			{
				onQuickHide();
			}
			this.QuickHide();
		}

		// Token: 0x0600745D RID: 29789 RVA: 0x0036345C File Offset: 0x0036165C
		private void OnConfirmButtonClicked()
		{
			bool flag = this._currentSelectedKey.Equals(InscribedCharacterKey.Invalid);
			if (!flag)
			{
				InscribedCharacter character;
				bool flag2 = GlobalOperations.InscribedCharacters.TryGetValue(this._currentSelectedKey, out character);
				if (flag2)
				{
					Action<InscribedCharacterKey, InscribedCharacter> onConfirmClicked = this.OnConfirmClicked;
					if (onConfirmClicked != null)
					{
						onConfirmClicked(this._currentSelectedKey, character);
					}
				}
				this.QuickHide();
			}
		}

		// Token: 0x0600745E RID: 29790 RVA: 0x003634B8 File Offset: 0x003616B8
		public void DeleteInscribedCharacter(InscribedCharacterKey key)
		{
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new DialogCmd
			{
				Title = LanguageKey.UI_NewGame_Tip_Delete_Mingke_Character.Tr(),
				Content = LanguageKey.UI_NewGame_Tip_Delete_Mingke_Character_Confirm.Tr(),
				Yes = delegate()
				{
					GlobalDomainMethod.Call.RemoveInscribedCharacter(key);
				}
			}));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x0600745F RID: 29791 RVA: 0x00363534 File Offset: 0x00361734
		public void TogglePin(InscribedCharacterKey key)
		{
			Dictionary<InscribedCharacterKey, int> pinOrders = GlobalOperations.InscribedCharacterPinOrders;
			bool flag = pinOrders.ContainsKey(key);
			if (flag)
			{
				GlobalDomainMethod.Call.RemoveInscribedCharacterPinOrder(key);
			}
			else
			{
				int maxOrder = (pinOrders.Count > 0) ? (pinOrders.Values.Max() + 1) : 0;
				GlobalDomainMethod.Call.SetInscribedCharacterPinOrder(key, maxOrder);
			}
		}

		// Token: 0x06007460 RID: 29792 RVA: 0x00363584 File Offset: 0x00361784
		private static int CalcMainAttributeSum(InscribedCharacter c)
		{
			MainAttributes attrs = c.BaseMainAttributes;
			return attrs.GetSum();
		}

		// Token: 0x06007461 RID: 29793 RVA: 0x003635A4 File Offset: 0x003617A4
		private static int CalcSkillSum(LifeSkillShorts skills)
		{
			return skills.GetSum();
		}

		// Token: 0x06007462 RID: 29794 RVA: 0x003635C0 File Offset: 0x003617C0
		private static int CalcSkillSum(CombatSkillShorts skills)
		{
			return skills.GetSum();
		}

		// Token: 0x06007463 RID: 29795 RVA: 0x003635D9 File Offset: 0x003617D9
		public override void QuickHide()
		{
			Action onQuickHide = this.OnQuickHide;
			if (onQuickHide != null)
			{
				onQuickHide();
			}
			base.QuickHide();
		}

		// Token: 0x040056E3 RID: 22243
		public Action<InscribedCharacterKey, InscribedCharacter> OnConfirmClicked;

		// Token: 0x040056E4 RID: 22244
		public Action OnQuickHide;

		// Token: 0x040056E5 RID: 22245
		[SerializeField]
		private InfinityScroll charScroll;

		// Token: 0x040056E6 RID: 22246
		[SerializeField]
		private CheckInscriptionCharDetail detailPanel;

		// Token: 0x040056E7 RID: 22247
		[SerializeField]
		private SortAndFilter sortAndFilter;

		// Token: 0x040056E8 RID: 22248
		[SerializeField]
		private CButton closeButton;

		// Token: 0x040056E9 RID: 22249
		[SerializeField]
		private TMP_InputField searchingField;

		// Token: 0x040056EA RID: 22250
		[SerializeField]
		private GameObject emptyGo;

		// Token: 0x040056EB RID: 22251
		private InscriptionSortAndFilterController _sortAndFilterController;

		// Token: 0x040056EC RID: 22252
		private List<CheckInscriptionCharData> _allCharData = new List<CheckInscriptionCharData>();

		// Token: 0x040056ED RID: 22253
		private List<CheckInscriptionCharData> _displayedCharData = new List<CheckInscriptionCharData>();

		// Token: 0x040056EE RID: 22254
		private InscribedCharacterKey _currentSelectedKey = InscribedCharacterKey.Invalid;
	}
}
