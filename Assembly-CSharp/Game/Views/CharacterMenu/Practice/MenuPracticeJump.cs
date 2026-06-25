using System;
using Config;
using FrameWork.UISystem.UIElements;
using GameData.Domains.CombatSkill;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.CharacterMenu.Practice
{
	// Token: 0x02000BB4 RID: 2996
	public class MenuPracticeJump : MonoBehaviour
	{
		// Token: 0x060096D8 RID: 38616 RVA: 0x004659D0 File Offset: 0x00463BD0
		public void Init(Action<short> onConfirm)
		{
			this.button.ClearAndAddListener(new Action(this.OnClickJumpSetting));
			this.confirm.ClearAndAddListener(delegate
			{
				onConfirm((short)this.slider.value);
				this.OnExitJumpSetting();
			});
			this.cancel.ClearAndAddListener(new Action(this.OnExitJumpSetting));
			this.panelCancel.ClearAndAddListener(new Action(this.OnExitJumpSetting));
			this.pointerTrigger.EnterEvent.AddListener(delegate()
			{
				this.panelCancel.enabled = false;
			});
			this.pointerTrigger.ExitEvent.AddListener(delegate()
			{
				this.panelCancel.enabled = true;
			});
			this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnThresholdChanged));
		}

		// Token: 0x060096D9 RID: 38617 RVA: 0x00465AAC File Offset: 0x00463CAC
		public void Set(CombatSkillDisplayData skillData)
		{
			bool flag = SingletonObject.getInstance<BasicGameData>().TaiwuCharId != skillData.CharId || !CombatSkillStateHelper.IsBrokenOut(skillData.ActivationState) || CombatSkill.Instance[skillData.TemplateId].MaxJumpDistance <= 0;
			if (flag)
			{
				base.gameObject.SetActive(false);
			}
			else
			{
				this._data = skillData;
				base.gameObject.SetActive(true);
				this.text.text = ((float)skillData.JumpThreshold / 10f).ToString("F1");
			}
		}

		// Token: 0x060096DA RID: 38618 RVA: 0x00465B45 File Offset: 0x00463D45
		public void SetInteractable(bool interactable)
		{
			this.button.interactable = interactable;
		}

		// Token: 0x060096DB RID: 38619 RVA: 0x00465B55 File Offset: 0x00463D55
		public void QuickHide()
		{
			this.OnExitJumpSetting();
		}

		// Token: 0x060096DC RID: 38620 RVA: 0x00465B60 File Offset: 0x00463D60
		private void OnClickJumpSetting()
		{
			this.panel.SetActive(true);
			CombatSkillItem config = CombatSkill.Instance[this._data.TemplateId];
			this.skillName.text = config.Name.SetGradeColor((int)config.Grade);
			this.slider.value = (float)this._data.JumpThreshold;
		}

		// Token: 0x060096DD RID: 38621 RVA: 0x00465BC6 File Offset: 0x00463DC6
		private void OnExitJumpSetting()
		{
			this.panel.SetActive(false);
		}

		// Token: 0x060096DE RID: 38622 RVA: 0x00465BD8 File Offset: 0x00463DD8
		private void OnThresholdChanged(float arg0)
		{
			short newThreshold = (short)arg0;
			this.value.text = ((float)newThreshold / 10f).ToString("F1");
		}

		// Token: 0x040073B1 RID: 29617
		[SerializeField]
		private CButton button;

		// Token: 0x040073B2 RID: 29618
		[SerializeField]
		private TextMeshProUGUI text;

		// Token: 0x040073B3 RID: 29619
		[SerializeField]
		private GameObject panel;

		// Token: 0x040073B4 RID: 29620
		[SerializeField]
		private TextMeshProUGUI skillName;

		// Token: 0x040073B5 RID: 29621
		[SerializeField]
		private TextMeshProUGUI value;

		// Token: 0x040073B6 RID: 29622
		[SerializeField]
		private CSlider slider;

		// Token: 0x040073B7 RID: 29623
		[SerializeField]
		private CButton confirm;

		// Token: 0x040073B8 RID: 29624
		[SerializeField]
		private CButton cancel;

		// Token: 0x040073B9 RID: 29625
		[SerializeField]
		private CButton panelCancel;

		// Token: 0x040073BA RID: 29626
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x040073BB RID: 29627
		private CombatSkillDisplayData _data;
	}
}
