using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x020007E3 RID: 2019
	public class NewGameCustomPresetFeatureSelectionPanel : MonoBehaviour
	{
		// Token: 0x0600626E RID: 25198 RVA: 0x002D1868 File Offset: 0x002CFA68
		public void Open(List<CharacterFeatureItem> features, short selectedFeatureId, int availablePoints, int totalPoints, Action<short> onConfirmSelection, Action onCancelSelection, sbyte currentGender)
		{
			this.EnsureUiInitialized();
			this._features.Clear();
			this._features.AddRange(features);
			this._pendingSelectedFeatureId = selectedFeatureId;
			this._availablePoints = availablePoints;
			this._totalPoints = totalPoints;
			this._onConfirmSelection = onConfirmSelection;
			this._onCancelSelection = onCancelSelection;
			this._currentGender = currentGender;
			this.RefreshPointText();
			this.featureScroll.UpdateData(this._features.Count);
		}

		// Token: 0x0600626F RID: 25199 RVA: 0x002D18E4 File Offset: 0x002CFAE4
		private void EnsureUiInitialized()
		{
			bool uiInitialized = this._uiInitialized;
			if (!uiInitialized)
			{
				this.featureScroll.OnItemRender += this.OnFeatureItemRender;
				this.confirmButton.onClick.ResetListener(new Action(this.OnClickConfirmButton));
				this.closeButton.onClick.ResetListener(new Action(this.OnClickCloseButton));
				this._uiInitialized = true;
			}
		}

		// Token: 0x06006270 RID: 25200 RVA: 0x002D1958 File Offset: 0x002CFB58
		private void OnFeatureItemRender(int index, GameObject itemObj)
		{
			CharacterFeatureItem featureItem = this._features[index];
			int cost = NewGameCustomPresetFeatureSelectionPanel.GetFeatureCost(featureItem.Level);
			bool isSelected = this._pendingSelectedFeatureId == featureItem.TemplateId;
			bool genderMatch = featureItem.Gender == -1 || featureItem.Gender == this._currentGender;
			bool canSelect = (isSelected || cost <= this._availablePoints) && (isSelected || genderMatch);
			NewGameCustomPresetFeatureItem itemView = itemObj.GetComponent<NewGameCustomPresetFeatureItem>();
			itemView.Initialize(featureItem.TemplateId, new Action<short>(this.OnClickFeatureItem));
			itemView.RefreshItem(featureItem, cost, isSelected, !canSelect);
		}

		// Token: 0x06006271 RID: 25201 RVA: 0x002D19F0 File Offset: 0x002CFBF0
		private void OnClickFeatureItem(short featureId)
		{
			bool flag = this._pendingSelectedFeatureId == featureId;
			if (flag)
			{
				this._pendingSelectedFeatureId = -1;
			}
			else
			{
				this._pendingSelectedFeatureId = featureId;
			}
			this.RefreshPointText();
			this.featureScroll.ReRender();
		}

		// Token: 0x06006272 RID: 25202 RVA: 0x002D1A30 File Offset: 0x002CFC30
		private void RefreshPointText()
		{
			int selectedCost = this.GetPendingSelectedCost();
			int remainingPoints = this._availablePoints - selectedCost;
			this.pointText.text = string.Format("{0}/{1}", remainingPoints.ToString().SetColor("brightblue"), this._totalPoints);
		}

		// Token: 0x06006273 RID: 25203 RVA: 0x002D1A80 File Offset: 0x002CFC80
		private int GetPendingSelectedCost()
		{
			bool flag = this._pendingSelectedFeatureId < 0;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				CharacterFeatureItem item = CharacterFeature.Instance[this._pendingSelectedFeatureId];
				result = NewGameCustomPresetFeatureSelectionPanel.GetFeatureCost(item.Level);
			}
			return result;
		}

		// Token: 0x06006274 RID: 25204 RVA: 0x002D1AC0 File Offset: 0x002CFCC0
		private static int GetFeatureCost(sbyte level)
		{
			int absLevel = Mathf.Abs((int)level);
			return (absLevel == 1 || absLevel == 2 || absLevel == 3) ? absLevel : 0;
		}

		// Token: 0x06006275 RID: 25205 RVA: 0x002D1AE9 File Offset: 0x002CFCE9
		private void OnClickConfirmButton()
		{
			Action<short> onConfirmSelection = this._onConfirmSelection;
			if (onConfirmSelection != null)
			{
				onConfirmSelection(this._pendingSelectedFeatureId);
			}
		}

		// Token: 0x06006276 RID: 25206 RVA: 0x002D1B04 File Offset: 0x002CFD04
		private void OnClickCloseButton()
		{
			Action onCancelSelection = this._onCancelSelection;
			if (onCancelSelection != null)
			{
				onCancelSelection();
			}
		}

		// Token: 0x04004481 RID: 17537
		[SerializeField]
		private TextMeshProUGUI pointText;

		// Token: 0x04004482 RID: 17538
		[SerializeField]
		private InfinityScroll featureScroll;

		// Token: 0x04004483 RID: 17539
		[SerializeField]
		private CButton confirmButton;

		// Token: 0x04004484 RID: 17540
		[SerializeField]
		private CButton closeButton;

		// Token: 0x04004485 RID: 17541
		private readonly List<CharacterFeatureItem> _features = new List<CharacterFeatureItem>();

		// Token: 0x04004486 RID: 17542
		private int _totalPoints;

		// Token: 0x04004487 RID: 17543
		private int _availablePoints;

		// Token: 0x04004488 RID: 17544
		private short _pendingSelectedFeatureId = -1;

		// Token: 0x04004489 RID: 17545
		private Action<short> _onConfirmSelection;

		// Token: 0x0400448A RID: 17546
		private Action _onCancelSelection;

		// Token: 0x0400448B RID: 17547
		private bool _uiInitialized;

		// Token: 0x0400448C RID: 17548
		private sbyte _currentGender;
	}
}
