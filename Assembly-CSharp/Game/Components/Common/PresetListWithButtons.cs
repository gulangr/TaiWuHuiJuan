using System;
using FrameWork.UISystem.UIElements;
using GameData.Domains.LegendaryBook;
using UnityEngine;

namespace Game.Components.Common
{
	// Token: 0x02000F94 RID: 3988
	public class PresetListWithButtons : MonoBehaviour
	{
		// Token: 0x170014AE RID: 5294
		// (get) Token: 0x0600B762 RID: 46946 RVA: 0x00539698 File Offset: 0x00537898
		public CButton BtnAdd
		{
			get
			{
				return this.btnAdd;
			}
		}

		// Token: 0x170014AF RID: 5295
		// (get) Token: 0x0600B763 RID: 46947 RVA: 0x005396A0 File Offset: 0x005378A0
		public CButton BtnDuplicate
		{
			get
			{
				return this.btnDuplicate;
			}
		}

		// Token: 0x170014B0 RID: 5296
		// (get) Token: 0x0600B764 RID: 46948 RVA: 0x005396A8 File Offset: 0x005378A8
		public CButton BtnRemove
		{
			get
			{
				return this.btnRemove;
			}
		}

		// Token: 0x170014B1 RID: 5297
		// (get) Token: 0x0600B765 RID: 46949 RVA: 0x005396B0 File Offset: 0x005378B0
		public int CurrentSelectedIndex
		{
			get
			{
				return this.presetGroup.GetActiveIndex();
			}
		}

		// Token: 0x0600B766 RID: 46950 RVA: 0x005396C0 File Offset: 0x005378C0
		private void Awake()
		{
			this.presetGroup.Init(-1);
			this.presetGroup.OnActiveIndexChange += this.OnIndexChange;
			this.btnAdd.ClearAndAddListener(delegate
			{
				Action onClickAdd = this._onClickAdd;
				if (onClickAdd != null)
				{
					onClickAdd();
				}
			});
			this.btnDuplicate.ClearAndAddListener(delegate
			{
				Action onClickDuplicate = this._onClickDuplicate;
				if (onClickDuplicate != null)
				{
					onClickDuplicate();
				}
			});
			this.btnReset.ClearAndAddListener(delegate
			{
				Action onClickReset = this._onClickReset;
				if (onClickReset != null)
				{
					onClickReset();
				}
			});
			this.btnRemove.ClearAndAddListener(delegate
			{
				Action onClickRemove = this._onClickRemove;
				if (onClickRemove != null)
				{
					onClickRemove();
				}
			});
		}

		// Token: 0x0600B767 RID: 46951 RVA: 0x00539753 File Offset: 0x00537953
		private void OnIndexChange(int newTog, int oldTog)
		{
			Action<int, int> onTogIndexChange = this._onTogIndexChange;
			if (onTogIndexChange != null)
			{
				onTogIndexChange(newTog, oldTog);
			}
		}

		// Token: 0x0600B768 RID: 46952 RVA: 0x0053976C File Offset: 0x0053796C
		public void SetAmount(int amount)
		{
			int index = 0;
			foreach (CToggle item in this.presetGroup.GetAll())
			{
				item.gameObject.SetActive(index < amount);
				index++;
			}
		}

		// Token: 0x0600B769 RID: 46953 RVA: 0x005397D8 File Offset: 0x005379D8
		public void Setup(Action onClickAdd, Action onClickDuplicate, Action onClickReset, Action onClickRemove, Action<int, int> onTogIndexChange)
		{
			this._onClickAdd = onClickAdd;
			this._onClickDuplicate = onClickDuplicate;
			this._onClickReset = onClickReset;
			this._onClickRemove = onClickRemove;
			this._onTogIndexChange = onTogIndexChange;
		}

		// Token: 0x0600B76A RID: 46954 RVA: 0x00539800 File Offset: 0x00537A00
		public void SetCurrentPreset(int presetIndex)
		{
			this.presetGroup.SetWithoutNotify(presetIndex);
		}

		// Token: 0x0600B76B RID: 46955 RVA: 0x00539810 File Offset: 0x00537A10
		public void Setup(LegendaryBookPresetDisplayData data)
		{
			this.SetAmount(data.CurrentUnlockedAmount);
			this.presetGroup.SetWithoutNotify(data.CurrentUsingPresetIndex);
			this.UpdateButtonsInteractable(data.CurrentUnlockedAmount);
		}

		// Token: 0x0600B76C RID: 46956 RVA: 0x0053983F File Offset: 0x00537A3F
		public void UpdateButtonsInteractable(int currentAmount)
		{
			this.btnAdd.interactable = (currentAmount < this._maxAmount);
			this.btnDuplicate.interactable = (currentAmount < this._maxAmount);
			this.btnRemove.interactable = (currentAmount > 1);
		}

		// Token: 0x04008E67 RID: 36455
		[SerializeField]
		private CToggleGroup presetGroup;

		// Token: 0x04008E68 RID: 36456
		[SerializeField]
		private CButton btnAdd;

		// Token: 0x04008E69 RID: 36457
		[SerializeField]
		private CButton btnDuplicate;

		// Token: 0x04008E6A RID: 36458
		[SerializeField]
		private CButton btnReset;

		// Token: 0x04008E6B RID: 36459
		[SerializeField]
		private CButton btnRemove;

		// Token: 0x04008E6C RID: 36460
		private Action _onClickAdd;

		// Token: 0x04008E6D RID: 36461
		private Action _onClickDuplicate;

		// Token: 0x04008E6E RID: 36462
		private Action _onClickReset;

		// Token: 0x04008E6F RID: 36463
		private Action _onClickRemove;

		// Token: 0x04008E70 RID: 36464
		private Action<int, int> _onTogIndexChange;

		// Token: 0x04008E71 RID: 36465
		private int _maxAmount = 9;
	}
}
