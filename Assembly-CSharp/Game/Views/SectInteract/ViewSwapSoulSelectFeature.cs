using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.Components;
using UnityEngine;

namespace Game.Views.SectInteract
{
	// Token: 0x020009AE RID: 2478
	public class ViewSwapSoulSelectFeature : UIBase
	{
		// Token: 0x0600780F RID: 30735 RVA: 0x0037DCF1 File Offset: 0x0037BEF1
		private void Awake()
		{
			this.infinityScroll.OnItemRender += this.OnItemRender;
		}

		// Token: 0x06007810 RID: 30736 RVA: 0x0037DD0C File Offset: 0x0037BF0C
		private void OnItemRender(int index, GameObject obj)
		{
			ViewSwapSoulSelectFeatureLine featureLine = obj.GetComponent<ViewSwapSoulSelectFeatureLine>();
			ValueTuple<int, List<short>> valueTuple = this._featureIdGroupList[index];
			int mutexGroupId = valueTuple.Item1;
			List<short> ids = valueTuple.Item2;
			featureLine.Set(index, ids, new Action<short>(this.SelectFeaureId), new Action<short>(this.DeSelectFeaureId), this._selectedFeatureIds);
		}

		// Token: 0x06007811 RID: 30737 RVA: 0x0037DD64 File Offset: 0x0037BF64
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get<HashSet<short>>("SelectedFeatureIds", out this._selectedFeatureIds);
			argsBox.Get<List<short>>("AllFeatureIds", out this._allFeatureIds);
			argsBox.Get<Action>("OnConfirm", out this._onConfirm);
			argsBox.Get<Dictionary<int, List<short>>>("FeatureIdDict", out this._featureIdDict);
			argsBox.Get<List<ValueTuple<int, List<short>>>>("FeatureIdGroupList", out this._featureIdGroupList);
			this._initialSelectedFeatureIds.Clear();
			foreach (short item in this._selectedFeatureIds)
			{
				this._initialSelectedFeatureIds.Add(item);
			}
			this.infinityScroll.SetDataCount(this._featureIdGroupList.Count);
		}

		// Token: 0x06007812 RID: 30738 RVA: 0x0037DE40 File Offset: 0x0037C040
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			string text = btnName;
			string a = text;
			if (!(a == "ButtonCloseView"))
			{
				if (a == "ConfirmBtn")
				{
					Action onConfirm = this._onConfirm;
					if (onConfirm != null)
					{
						onConfirm();
					}
					this.QuickHide();
				}
			}
			else
			{
				this.TryCancelAndHide();
			}
		}

		// Token: 0x06007813 RID: 30739 RVA: 0x0037DE9C File Offset: 0x0037C09C
		private bool HasFeatureModified()
		{
			bool flag = this._initialSelectedFeatureIds == null || this._selectedFeatureIds == null;
			return !flag && !this._initialSelectedFeatureIds.SetEquals(this._selectedFeatureIds);
		}

		// Token: 0x06007814 RID: 30740 RVA: 0x0037DEE0 File Offset: 0x0037C0E0
		private void RestoreInitialState()
		{
			this._selectedFeatureIds.Clear();
			foreach (short id in this._initialSelectedFeatureIds)
			{
				this._selectedFeatureIds.Add(id);
			}
		}

		// Token: 0x06007815 RID: 30741 RVA: 0x0037DF4C File Offset: 0x0037C14C
		private void TryCancelAndHide()
		{
			bool flag = this.HasFeatureModified();
			if (flag)
			{
				DialogCmd cmd = new DialogCmd();
				cmd.Type = 1;
				cmd.Title = LanguageKey.UI_SoulSwap_SelectFeature_Cancel.Tr();
				cmd.Content = LanguageKey.UI_SoulSwap_SelectFeature_Cancel_Content.Tr();
				cmd.Yes = delegate()
				{
					this.RestoreInitialState();
					this.QuickHide();
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
			else
			{
				this.QuickHide();
			}
		}

		// Token: 0x06007816 RID: 30742 RVA: 0x0037DFDB File Offset: 0x0037C1DB
		public void SelectFeaureId(short id)
		{
			this._selectedFeatureIds.Add(id);
		}

		// Token: 0x06007817 RID: 30743 RVA: 0x0037DFEB File Offset: 0x0037C1EB
		public void DeSelectFeaureId(short id)
		{
			this._selectedFeatureIds.Remove(id);
		}

		// Token: 0x04005AB4 RID: 23220
		[SerializeField]
		private InfinityScroll infinityScroll;

		// Token: 0x04005AB5 RID: 23221
		private HashSet<short> _selectedFeatureIds;

		// Token: 0x04005AB6 RID: 23222
		private List<short> _allFeatureIds;

		// Token: 0x04005AB7 RID: 23223
		private Dictionary<int, List<short>> _featureIdDict;

		// Token: 0x04005AB8 RID: 23224
		private List<ValueTuple<int, List<short>>> _featureIdGroupList;

		// Token: 0x04005AB9 RID: 23225
		private Action _onConfirm;

		// Token: 0x04005ABA RID: 23226
		private readonly HashSet<short> _initialSelectedFeatureIds = new HashSet<short>();
	}
}
