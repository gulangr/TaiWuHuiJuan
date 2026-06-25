using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract
{
	// Token: 0x020009AF RID: 2479
	public class ViewSwapSoulSelectFeatureLine : MonoBehaviour
	{
		// Token: 0x0600781A RID: 30746 RVA: 0x0037E020 File Offset: 0x0037C220
		private void Awake()
		{
			this.cToggleGroup.Init(-1);
			this._onActiveIndexChangeHandler = delegate(int newIndex, int oldIndex)
			{
				bool flag = this._currentIds.CheckIndex(newIndex);
				if (flag)
				{
					Action<short> addAction = this._addAction;
					if (addAction != null)
					{
						addAction(this._currentIds[newIndex]);
					}
				}
				bool flag2 = this._currentIds.CheckIndex(oldIndex);
				if (flag2)
				{
					Action<short> removeAction = this._removeAction;
					if (removeAction != null)
					{
						removeAction(this._currentIds[oldIndex]);
					}
				}
				this.icon.gameObject.SetActive(newIndex < 0);
			};
			this.cToggleGroup.OnActiveIndexChange += this._onActiveIndexChangeHandler;
		}

		// Token: 0x0600781B RID: 30747 RVA: 0x0037E054 File Offset: 0x0037C254
		public void Set(int index, List<short> ids, Action<short> addAction, Action<short> removeAction, HashSet<short> selectedFeatureIds)
		{
			this._currentIds = ids;
			this._addAction = addAction;
			this._removeAction = removeAction;
			this.title.text = LanguageKey.UI_SoulSwap_SelectFeature_Line.TrFormat(LocalStringManager.Get(string.Format("LK_Number{0}", index + 1)));
			CommonUtils.PrepareEnoughChildren(this.cToggleGroup.transform, this.tempFeatureItem.gameObject, ids.Count, null);
			this.cToggleGroup.AddAllChildToggles();
			for (int i = 0; i < ids.Count; i++)
			{
				this.cToggleGroup.GetAll()[i].GetComponent<Feature>().Set(ids[i], -1, false, -1);
			}
			for (int j = 0; j < ids.Count; j++)
			{
				bool flag = selectedFeatureIds.Contains(ids[j]);
				if (flag)
				{
					this.cToggleGroup.Set(j, false);
					return;
				}
			}
			this.cToggleGroup.DeSelect(false);
		}

		// Token: 0x04005ABB RID: 23227
		[SerializeField]
		private CToggleGroup cToggleGroup;

		// Token: 0x04005ABC RID: 23228
		[SerializeField]
		private Feature tempFeatureItem;

		// Token: 0x04005ABD RID: 23229
		[SerializeField]
		private CImage icon;

		// Token: 0x04005ABE RID: 23230
		[SerializeField]
		private TextMeshProUGUI title;

		// Token: 0x04005ABF RID: 23231
		private Action<int, int> _onActiveIndexChangeHandler;

		// Token: 0x04005AC0 RID: 23232
		private Action<short> _addAction;

		// Token: 0x04005AC1 RID: 23233
		private Action<short> _removeAction;

		// Token: 0x04005AC2 RID: 23234
		private List<short> _currentIds;
	}
}
