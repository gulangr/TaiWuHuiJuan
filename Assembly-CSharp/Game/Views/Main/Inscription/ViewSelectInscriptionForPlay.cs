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
	// Token: 0x0200097C RID: 2428
	public class ViewSelectInscriptionForPlay : UIBase
	{
		// Token: 0x0600748C RID: 29836 RVA: 0x00364B08 File Offset: 0x00362D08
		public override void OnInit(ArgumentBox argsBox)
		{
			CButton cbutton = this.closeButton;
			if (cbutton != null)
			{
				cbutton.ClearAndAddListener(new Action(this.OnCloseClicked));
			}
			CButton cbutton2 = this.confirmButton;
			if (cbutton2 != null)
			{
				cbutton2.ClearAndAddListener(new Action(this.OnConfirmButtonClicked));
			}
			this.charScroll.OnItemRender += this.OnCharItemRender;
			this._sortAndFilterController = new InscriptionSortAndFilterController(this.sortAndFilter, false);
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
		}

		// Token: 0x0600748D RID: 29837 RVA: 0x00364BFC File Offset: 0x00362DFC
		public override void NotifyUIHide()
		{
			base.NotifyUIHide();
			this.charScroll.OnItemRender -= this.OnCharItemRender;
			GEvent.Remove(EEvents.InscriptionChange, new GEvent.Callback(this.OnInscriptionChange));
			ViewSelectInscriptionForPlay.OnConfirmClicked = null;
			this.OnQuickHide = null;
		}

		// Token: 0x0600748E RID: 29838 RVA: 0x00364C50 File Offset: 0x00362E50
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
					MainAttributeSum = ViewSelectInscriptionForPlay.CalcMainAttributeSum(kv.Value),
					LifeSkillSum = ViewSelectInscriptionForPlay.CalcSkillSum(kv.Value.BaseLifeSkillQualifications),
					CombatSkillSum = ViewSelectInscriptionForPlay.CalcSkillSum(kv.Value.BaseCombatSkillQualifications)
				};
				this._allCharData.Add(data);
			}
			this.ApplySortAndFilter();
		}

		// Token: 0x0600748F RID: 29839 RVA: 0x00364D78 File Offset: 0x00362F78
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
		}

		// Token: 0x06007490 RID: 29840 RVA: 0x00364EA0 File Offset: 0x003630A0
		private void OnCharItemRender(int index, GameObject cell)
		{
			bool flag = index < 0 || index >= this._displayedCharData.Count;
			if (!flag)
			{
				CheckInscriptionCharCard card = cell.GetComponent<CheckInscriptionCharCard>();
				bool flag2 = card != null;
				if (flag2)
				{
					card.SetData(this._displayedCharData[index], this._currentSelectedKey, this, true);
				}
			}
		}

		// Token: 0x06007491 RID: 29841 RVA: 0x00364EFC File Offset: 0x003630FC
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

		// Token: 0x06007492 RID: 29842 RVA: 0x00364F64 File Offset: 0x00363164
		public void SelectCharByKey(InscribedCharacterKey key)
		{
			int index = this._displayedCharData.FindIndex((CheckInscriptionCharData d) => d.Key.Equals(key));
			bool flag = index >= 0;
			if (flag)
			{
				this.SelectChar(index);
			}
		}

		// Token: 0x06007493 RID: 29843 RVA: 0x00364FAA File Offset: 0x003631AA
		private void OnSortFilterChanged()
		{
			this.ApplySortAndFilter();
		}

		// Token: 0x06007494 RID: 29844 RVA: 0x00364FB4 File Offset: 0x003631B4
		private void OnInscriptionChange(ArgumentBox _ = null)
		{
			this.BuildDataAndRefresh();
		}

		// Token: 0x06007495 RID: 29845 RVA: 0x00364FBE File Offset: 0x003631BE
		private void OnCloseClicked()
		{
			Action onQuickHide = this.OnQuickHide;
			if (onQuickHide != null)
			{
				onQuickHide();
			}
			this.QuickHide();
		}

		// Token: 0x06007496 RID: 29846 RVA: 0x00364FDC File Offset: 0x003631DC
		private void OnConfirmButtonClicked()
		{
			bool flag = this._currentSelectedKey.Equals(InscribedCharacterKey.Invalid);
			if (!flag)
			{
				InscribedCharacter character;
				bool flag2 = GlobalOperations.InscribedCharacters.TryGetValue(this._currentSelectedKey, out character);
				if (flag2)
				{
					Action<InscribedCharacterKey, InscribedCharacter> onConfirmClicked = ViewSelectInscriptionForPlay.OnConfirmClicked;
					if (onConfirmClicked != null)
					{
						onConfirmClicked(this._currentSelectedKey, character);
					}
				}
				this.QuickHide();
			}
		}

		// Token: 0x06007497 RID: 29847 RVA: 0x00365037 File Offset: 0x00363237
		public void DeleteInscribedCharacter(InscribedCharacterKey key)
		{
			GlobalDomainMethod.Call.RemoveInscribedCharacter(key);
		}

		// Token: 0x06007498 RID: 29848 RVA: 0x00365044 File Offset: 0x00363244
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

		// Token: 0x06007499 RID: 29849 RVA: 0x00365094 File Offset: 0x00363294
		private static int CalcMainAttributeSum(InscribedCharacter c)
		{
			MainAttributes attrs = c.BaseMainAttributes;
			return attrs.GetSum();
		}

		// Token: 0x0600749A RID: 29850 RVA: 0x003650B4 File Offset: 0x003632B4
		private static int CalcSkillSum(LifeSkillShorts skills)
		{
			return skills.GetSum();
		}

		// Token: 0x0600749B RID: 29851 RVA: 0x003650D0 File Offset: 0x003632D0
		private static int CalcSkillSum(CombatSkillShorts skills)
		{
			return skills.GetSum();
		}

		// Token: 0x0600749C RID: 29852 RVA: 0x003650E9 File Offset: 0x003632E9
		public override void QuickHide()
		{
			Action onQuickHide = this.OnQuickHide;
			if (onQuickHide != null)
			{
				onQuickHide();
			}
			base.QuickHide();
		}

		// Token: 0x04005718 RID: 22296
		public static Action<InscribedCharacterKey, InscribedCharacter> OnConfirmClicked;

		// Token: 0x04005719 RID: 22297
		public Action OnQuickHide;

		// Token: 0x0400571A RID: 22298
		[SerializeField]
		private InfinityScroll charScroll;

		// Token: 0x0400571B RID: 22299
		[SerializeField]
		private CheckInscriptionCharDetail detailPanel;

		// Token: 0x0400571C RID: 22300
		[SerializeField]
		private SortAndFilterLegacy sortAndFilter;

		// Token: 0x0400571D RID: 22301
		[SerializeField]
		private CButton closeButton;

		// Token: 0x0400571E RID: 22302
		[SerializeField]
		private CButton confirmButton;

		// Token: 0x0400571F RID: 22303
		[SerializeField]
		private TMP_InputField searchingField;

		// Token: 0x04005720 RID: 22304
		private InscriptionSortAndFilterController _sortAndFilterController;

		// Token: 0x04005721 RID: 22305
		private List<CheckInscriptionCharData> _allCharData = new List<CheckInscriptionCharData>();

		// Token: 0x04005722 RID: 22306
		private List<CheckInscriptionCharData> _displayedCharData = new List<CheckInscriptionCharData>();

		// Token: 0x04005723 RID: 22307
		private InscribedCharacterKey _currentSelectedKey = InscribedCharacterKey.Invalid;
	}
}
