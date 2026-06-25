using System;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Map;
using TMPro;
using UnityEngine;

namespace Game.Views.Bottom
{
	// Token: 0x02000C36 RID: 3126
	public class MapBlockFindItemBase : MonoBehaviour
	{
		// Token: 0x170010C7 RID: 4295
		// (get) Token: 0x06009ED5 RID: 40661 RVA: 0x004A4A27 File Offset: 0x004A2C27
		// (set) Token: 0x06009ED6 RID: 40662 RVA: 0x004A4A2F File Offset: 0x004A2C2F
		public EFilterItemKey EFilterItemKey
		{
			get
			{
				return this._eFilterItemKey;
			}
			set
			{
				this._eFilterItemKey = value;
			}
		}

		// Token: 0x170010C8 RID: 4296
		// (get) Token: 0x06009ED7 RID: 40663 RVA: 0x004A4A38 File Offset: 0x004A2C38
		protected MapBlockFindData Data
		{
			get
			{
				return UIElement.FindMapBlock.UiBaseAs<ViewFindMapBlock>().CurrentFilterData;
			}
		}

		// Token: 0x06009ED8 RID: 40664 RVA: 0x004A4A49 File Offset: 0x004A2C49
		private void Awake()
		{
			this.InitComponents();
		}

		// Token: 0x06009ED9 RID: 40665 RVA: 0x004A4A53 File Offset: 0x004A2C53
		protected virtual void InitComponents()
		{
			this.resetBtn.ClearAndAddListener(new Action(this.Reset));
		}

		// Token: 0x06009EDA RID: 40666 RVA: 0x004A4A70 File Offset: 0x004A2C70
		public virtual void Set(ViewFindMapBlock mapBlockFind, EFilterItemKey key, FilterItemConfig itemConfig)
		{
			this._mapBlockFind = mapBlockFind;
			this._eFilterItemKey = key;
			this._itemConfig = itemConfig;
			bool flag = !this._inited;
			if (flag)
			{
				this.Init();
				this._inited = true;
			}
			this.title.text = this._itemConfig.DisplayName;
		}

		// Token: 0x06009EDB RID: 40667 RVA: 0x004A4AC7 File Offset: 0x004A2CC7
		protected virtual void Init()
		{
		}

		// Token: 0x06009EDC RID: 40668 RVA: 0x004A4ACA File Offset: 0x004A2CCA
		public virtual void Reset()
		{
			this._mapBlockFind.ResetFilterItem(this.EFilterItemKey);
		}

		// Token: 0x06009EDD RID: 40669 RVA: 0x004A4ADF File Offset: 0x004A2CDF
		public virtual void SetWithoutNotify()
		{
		}

		// Token: 0x06009EDE RID: 40670 RVA: 0x004A4AE4 File Offset: 0x004A2CE4
		public virtual bool HasFilterValue()
		{
			return false;
		}

		// Token: 0x04007AEC RID: 31468
		[SerializeField]
		protected CButton resetBtn;

		// Token: 0x04007AED RID: 31469
		[SerializeField]
		protected TextMeshProUGUI title;

		// Token: 0x04007AEE RID: 31470
		private EFilterItemKey _eFilterItemKey;

		// Token: 0x04007AEF RID: 31471
		protected ViewFindMapBlock _mapBlockFind;

		// Token: 0x04007AF0 RID: 31472
		protected FilterItemConfig _itemConfig;

		// Token: 0x04007AF1 RID: 31473
		private bool _inited;
	}
}
