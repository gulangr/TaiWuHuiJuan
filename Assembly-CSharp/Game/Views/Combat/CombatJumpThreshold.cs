using System;
using Config;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Combat;
using GameData.Domains.CombatSkill;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Combat
{
	// Token: 0x02000AE8 RID: 2792
	public class CombatJumpThreshold : MonoBehaviour
	{
		// Token: 0x17000F28 RID: 3880
		// (get) Token: 0x06008930 RID: 35120 RVA: 0x003F7CDE File Offset: 0x003F5EDE
		// (set) Token: 0x06008931 RID: 35121 RVA: 0x003F7CE6 File Offset: 0x003F5EE6
		public Action OnConfirm { get; set; }

		// Token: 0x17000F29 RID: 3881
		// (get) Token: 0x06008932 RID: 35122 RVA: 0x003F7CEF File Offset: 0x003F5EEF
		// (set) Token: 0x06008933 RID: 35123 RVA: 0x003F7CF7 File Offset: 0x003F5EF7
		public Action OnCancel { get; set; }

		// Token: 0x17000F2A RID: 3882
		// (get) Token: 0x06008934 RID: 35124 RVA: 0x003F7D00 File Offset: 0x003F5F00
		private CSlider ThresholdSlider
		{
			get
			{
				return this.thresholdSlider;
			}
		}

		// Token: 0x06008935 RID: 35125 RVA: 0x003F7D08 File Offset: 0x003F5F08
		public void Refresh(Canvas canvas, CombatSkillDisplayData displayData)
		{
			this._combatSkillCanvas = canvas;
			this._combatSkillDisplayData = displayData;
			this.SetComponents();
			this.SetOverrideSorting(true);
		}

		// Token: 0x06008936 RID: 35126 RVA: 0x003F7D28 File Offset: 0x003F5F28
		public void ForceCancel()
		{
			this.OnClickCancel();
		}

		// Token: 0x06008937 RID: 35127 RVA: 0x003F7D34 File Offset: 0x003F5F34
		private void Awake()
		{
			this.ThresholdSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnThresholdChanged));
			this.confirm.ClearAndAddListener(new Action(this.OnClickConfirm));
			this.cancel.ClearAndAddListener(new Action(this.OnClickCancel));
			this.resetToDefault.ClearAndAddListener(new Action(this.OnClickResetToDefault));
		}

		// Token: 0x06008938 RID: 35128 RVA: 0x003F7DA8 File Offset: 0x003F5FA8
		private void OnThresholdChanged(float arg0)
		{
			short newThreshold = (short)arg0;
			this.currentValue.text = ((float)newThreshold / 10f).ToString("F1");
		}

		// Token: 0x06008939 RID: 35129 RVA: 0x003F7DDC File Offset: 0x003F5FDC
		private void SetComponents()
		{
			this.ThresholdSlider.value = (float)this._combatSkillDisplayData.JumpThreshold;
			this.minValue.text = (this.ThresholdSlider.minValue / 10f).ToString("F1");
			this.maxValue.text = (this.ThresholdSlider.maxValue / 10f).ToString("F1");
			CombatSkillItem config = CombatSkill.Instance[this._combatSkillDisplayData.TemplateId];
			this.grade.Set(config.Grade);
			this.name.color = Colors.Instance.GradeColors[(int)config.Grade];
			this.name.text = config.Name;
		}

		// Token: 0x0600893A RID: 35130 RVA: 0x003F7EB4 File Offset: 0x003F60B4
		public void OnClickConfirm()
		{
			short currentThreshold = (short)this.ThresholdSlider.value;
			CombatDomainMethod.Call.SetJumpThreshold(-1, this._combatSkillDisplayData.TemplateId, currentThreshold);
			this._combatSkillDisplayData.JumpThreshold = currentThreshold;
			this.SetOverrideSorting(false);
			Action onConfirm = this.OnConfirm;
			if (onConfirm != null)
			{
				onConfirm();
			}
		}

		// Token: 0x0600893B RID: 35131 RVA: 0x003F7F08 File Offset: 0x003F6108
		private void OnClickCancel()
		{
			this.SetOverrideSorting(false);
			Action onCancel = this.OnCancel;
			if (onCancel != null)
			{
				onCancel();
			}
		}

		// Token: 0x0600893C RID: 35132 RVA: 0x003F7F28 File Offset: 0x003F6128
		private void OnClickResetToDefault()
		{
			short defaultThreshold = GlobalConfig.Instance.DefaultJumpThreshold;
			this.ThresholdSlider.value = (float)defaultThreshold;
			this.currentValue.text = ((float)defaultThreshold / 10f).ToString("F1");
		}

		// Token: 0x0600893D RID: 35133 RVA: 0x003F7F70 File Offset: 0x003F6170
		private void SetOverrideSorting(bool overrideSorting)
		{
			bool flag = null == this._combatSkillCanvas;
			if (!flag)
			{
				this._combatSkillCanvas.overrideSorting = overrideSorting;
				if (overrideSorting)
				{
					this._combatSkillCanvas.sortingOrder = this.sortingOrder;
				}
			}
		}

		// Token: 0x0400691F RID: 26911
		[SerializeField]
		private TextMeshProUGUI currentValue;

		// Token: 0x04006920 RID: 26912
		[SerializeField]
		private TextMeshProUGUI minValue;

		// Token: 0x04006921 RID: 26913
		[SerializeField]
		private TextMeshProUGUI maxValue;

		// Token: 0x04006922 RID: 26914
		[SerializeField]
		private CSlider thresholdSlider;

		// Token: 0x04006923 RID: 26915
		[SerializeField]
		private CButton confirm;

		// Token: 0x04006924 RID: 26916
		[SerializeField]
		private CButton cancel;

		// Token: 0x04006925 RID: 26917
		[SerializeField]
		private CButton resetToDefault;

		// Token: 0x04006926 RID: 26918
		[SerializeField]
		private CombatSkillGrade grade;

		// Token: 0x04006927 RID: 26919
		[SerializeField]
		private new TextMeshProUGUI name;

		// Token: 0x04006928 RID: 26920
		public int sortingOrder;

		// Token: 0x0400692B RID: 26923
		private Canvas _combatSkillCanvas;

		// Token: 0x0400692C RID: 26924
		private CombatSkillDisplayData _combatSkillDisplayData;
	}
}
