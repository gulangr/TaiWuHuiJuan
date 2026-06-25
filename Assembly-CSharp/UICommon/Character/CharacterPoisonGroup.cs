using System;
using CharacterDataMonitor;
using TMPro;
using UnityEngine;

namespace UICommon.Character
{
	// Token: 0x020005E4 RID: 1508
	public class CharacterPoisonGroup : CharacterUIElement
	{
		// Token: 0x170008F8 RID: 2296
		// (get) Token: 0x0600472E RID: 18222 RVA: 0x0021554B File Offset: 0x0021374B
		private InjuryPoisonMonitor Item
		{
			get
			{
				return this.MonitorDataItem as InjuryPoisonMonitor;
			}
		}

		// Token: 0x0600472F RID: 18223 RVA: 0x00215558 File Offset: 0x00213758
		public CharacterPoisonGroup(Refers[] poisonRefersArray, GameObject[] poisonedMarkObjs = null)
		{
			bool flag = poisonRefersArray == null || poisonRefersArray.Length == 0;
			if (flag)
			{
				throw new Exception("CharacterPoisonGroup must handle at least one poisonItem");
			}
			this._poisonItems = new CharacterPoisonItem[6];
			for (sbyte i = 0; i < 6; i += 1)
			{
				bool flag2 = poisonRefersArray.CheckIndex((int)i) && null != poisonRefersArray[(int)i];
				if (flag2)
				{
					GameObject mark = null;
					bool flag3 = poisonedMarkObjs != null;
					if (flag3)
					{
						mark = poisonedMarkObjs[(int)i];
					}
					this._poisonItems[(int)i] = new CharacterPoisonItem(poisonRefersArray[(int)i], i, mark, this);
				}
			}
		}

		// Token: 0x06004730 RID: 18224 RVA: 0x002155EC File Offset: 0x002137EC
		public void SetInitValue(int[] initPoisonValueArray)
		{
			for (sbyte i = 0; i < 6; i += 1)
			{
				CharacterPoisonItem characterPoisonItem = this._poisonItems[(int)i];
				if (characterPoisonItem != null)
				{
					characterPoisonItem.SetInitValue(initPoisonValueArray[(int)i]);
				}
			}
		}

		// Token: 0x06004731 RID: 18225 RVA: 0x00215624 File Offset: 0x00213824
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<InjuryPoisonMonitor>(charId, this.IsDead);
		}

		// Token: 0x06004732 RID: 18226 RVA: 0x00215648 File Offset: 0x00213848
		internal override void BindEvent()
		{
			for (int i = 0; i < this._poisonItems.Length; i++)
			{
				bool flag = this._poisonItems[i] != null;
				if (flag)
				{
					this._poisonItems[i].CharacterId = base.CharacterId;
				}
				this._poisonItems[i].BindEvent();
			}
			this.Item.AddPoisonsListener(new Action<sbyte>(this.FillAllItems));
			this.Item.AddPoisonResistsListener(new Action<sbyte>(this.FillAllItemsWithCount));
		}

		// Token: 0x06004733 RID: 18227 RVA: 0x002156D0 File Offset: 0x002138D0
		public override void UnbindEvent()
		{
			for (int i = 0; i < this._poisonItems.Length; i++)
			{
				bool flag = this._poisonItems[i] != null;
				if (flag)
				{
					this._poisonItems[i].UnbindEvent();
				}
			}
			this.Item.RemovePoisonsListener(new Action<sbyte>(this.FillAllItems));
			this.Item.RemovePoisonResistsListener(new Action<sbyte>(this.FillAllItemsWithCount));
		}

		// Token: 0x06004734 RID: 18228 RVA: 0x00215744 File Offset: 0x00213944
		private void FillAllItemsWithCount(sbyte _t)
		{
			this.FillAllItems(6);
		}

		// Token: 0x06004735 RID: 18229 RVA: 0x00215750 File Offset: 0x00213950
		public override void FillElement()
		{
			for (int i = 0; i < this._poisonItems.Length; i++)
			{
				bool flag = this._poisonItems[i] != null;
				if (flag)
				{
					this._poisonItems[i].CharacterId = base.CharacterId;
				}
			}
			this.FillAllItems(6);
		}

		// Token: 0x06004736 RID: 18230 RVA: 0x002157A4 File Offset: 0x002139A4
		public override void ResetToEmpty()
		{
			for (sbyte i = 0; i < 6; i += 1)
			{
				bool flag = this._poisonItems[(int)i] != null;
				if (flag)
				{
					this._poisonItems[(int)i].CharacterId = -1;
				}
			}
		}

		// Token: 0x06004737 RID: 18231 RVA: 0x002157E4 File Offset: 0x002139E4
		private void FillAllItems(sbyte type)
		{
			bool flag = type == 6;
			if (flag)
			{
				bool flag2 = this.IsCombatCharacter && this.GetOldPoisonHandler != null;
				if (flag2)
				{
					ValueTuple<int[], int[]> valueTuple = this.GetOldPoisonHandler();
					int[] oldPoison = valueTuple.Item1;
					int[] oldPoisonResists = valueTuple.Item2;
					bool flag3 = oldPoison != null;
					if (flag3)
					{
						for (sbyte i = 0; i < 6; i += 1)
						{
							int num = (oldPoisonResists != null) ? oldPoisonResists[(int)i] : this.Item.PoisonResists[(int)i];
							this._poisonItems[(int)i].FillAsCombatState(oldPoison[(int)i]);
						}
						TextMeshProUGUI totalLabel = this._totalLabel;
						if (totalLabel != null)
						{
							totalLabel.SetText(this.GetAllPoisonSum().ToString().SetColor("poisoned"), true);
						}
						return;
					}
				}
				for (sbyte j = 0; j < 6; j += 1)
				{
					CharacterPoisonItem characterPoisonItem = this._poisonItems[(int)j];
					if (characterPoisonItem != null)
					{
						characterPoisonItem.FillElement();
					}
				}
				TextMeshProUGUI totalLabel2 = this._totalLabel;
				if (totalLabel2 != null)
				{
					totalLabel2.SetText(this.GetAllPoisonSum().ToString().SetColor("poisoned"), true);
				}
			}
			Action onPoisonChanged = this.OnPoisonChanged;
			if (onPoisonChanged != null)
			{
				onPoisonChanged();
			}
		}

		// Token: 0x06004738 RID: 18232 RVA: 0x00215920 File Offset: 0x00213B20
		private int GetAllPoisonSum()
		{
			int sum = 0;
			for (sbyte i = 0; i < 6; i += 1)
			{
				int num = sum;
				CharacterPoisonItem characterPoisonItem = this._poisonItems[(int)i];
				sum = num + (int)((characterPoisonItem != null) ? characterPoisonItem.PoisonLevel : 0);
			}
			return sum;
		}

		// Token: 0x06004739 RID: 18233 RVA: 0x0021595F File Offset: 0x00213B5F
		public void InitTotalLabel(TextMeshProUGUI label)
		{
			this._totalLabel = label;
		}

		// Token: 0x04003129 RID: 12585
		public bool IsCombatCharacter;

		// Token: 0x0400312A RID: 12586
		public Func<ValueTuple<int[], int[]>> GetOldPoisonHandler;

		// Token: 0x0400312B RID: 12587
		private readonly CharacterPoisonItem[] _poisonItems;

		// Token: 0x0400312C RID: 12588
		private TextMeshProUGUI _totalLabel;

		// Token: 0x0400312D RID: 12589
		public Action OnPoisonChanged;
	}
}
