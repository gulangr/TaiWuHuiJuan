using System;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Map;
using UnityEngine;

namespace Game.Views.Bottom
{
	// Token: 0x02000C39 RID: 3129
	public class MapBlockFindItem_SingleSelect : MapBlockFindItemBase
	{
		// Token: 0x06009EF7 RID: 40695 RVA: 0x004A5486 File Offset: 0x004A3686
		protected override void InitComponents()
		{
			base.InitComponents();
			this.toggleGroup.OnActiveIndexChange += this.OnActiveIndexChange;
		}

		// Token: 0x06009EF8 RID: 40696 RVA: 0x004A54A8 File Offset: 0x004A36A8
		public override void Set(ViewFindMapBlock mapBlockFind, EFilterItemKey key, FilterItemConfig itemConfig)
		{
			base.Set(mapBlockFind, key, itemConfig);
			this.SetWithoutNotify();
		}

		// Token: 0x06009EF9 RID: 40697 RVA: 0x004A54BC File Offset: 0x004A36BC
		protected override void Init()
		{
			base.Init();
			bool flag = this._itemConfig.Options != null;
			if (flag)
			{
				CommonUtils.PrepareEnoughChildren(this.toggleGroup.transform, this.toggleGroup.transform.GetChild(0).gameObject, this._itemConfig.Options.Length, null);
				for (int i = 0; i < this._itemConfig.Options.Length; i++)
				{
					ToggleStyle toggleStyle = this.toggleGroup.transform.GetChild(i).GetComponent<ToggleStyle>();
					toggleStyle.SetLabelText(this._itemConfig.Options[i]);
				}
				this.toggleGroup.AddAllChildToggles();
			}
			this.toggleGroup.Init(-1);
		}

		// Token: 0x06009EFA RID: 40698 RVA: 0x004A5589 File Offset: 0x004A3789
		private void OnActiveIndexChange(int newIndex, int oldIndex)
		{
			this._mapBlockFind.UpdateSingleSelectData(base.EFilterItemKey, newIndex);
		}

		// Token: 0x06009EFB RID: 40699 RVA: 0x004A559F File Offset: 0x004A379F
		public override void Reset()
		{
			this.toggleGroup.DeSelectWithoutNotify();
			base.Reset();
		}

		// Token: 0x06009EFC RID: 40700 RVA: 0x004A55B8 File Offset: 0x004A37B8
		public override void SetWithoutNotify()
		{
			int value;
			bool flag = base.Data.SingleSelectData.TryGetValue(base.EFilterItemKey, out value) && value >= 0;
			if (flag)
			{
				this.toggleGroup.SetWithoutNotify(value);
			}
			else
			{
				this.toggleGroup.DeSelectWithoutNotify();
			}
			bool flag2 = base.EFilterItemKey == EFilterItemKey.CharacterAttributeType;
			if (flag2)
			{
				this._mapBlockFind.UpdateSkillTypeVisibility();
			}
		}

		// Token: 0x06009EFD RID: 40701 RVA: 0x004A562C File Offset: 0x004A382C
		public override bool HasFilterValue()
		{
			return this.toggleGroup.GetActiveIndex() >= 0;
		}

		// Token: 0x04007AFB RID: 31483
		[SerializeField]
		private CToggleGroup toggleGroup;
	}
}
