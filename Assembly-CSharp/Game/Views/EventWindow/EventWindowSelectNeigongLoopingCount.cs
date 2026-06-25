using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.CharacterMenu;
using GameData.Domains.CombatSkill;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.EventWindow
{
	// Token: 0x02000A44 RID: 2628
	public class EventWindowSelectNeigongLoopingCount : MonoBehaviour
	{
		// Token: 0x17000E3A RID: 3642
		// (get) Token: 0x060081C2 RID: 33218 RVA: 0x003C6F14 File Offset: 0x003C5114
		// (set) Token: 0x060081C3 RID: 33219 RVA: 0x003C6F1C File Offset: 0x003C511C
		public CombatSkillDisplayData selectedCombatSkill { get; private set; }

		// Token: 0x060081C4 RID: 33220 RVA: 0x003C6F25 File Offset: 0x003C5125
		private void Awake()
		{
			this.btnDeselect.ClearAndAddListener(new Action(this.OnDeselectAll));
			this.btnSelectSkill.ClearAndAddListener(new Action(this.SelectSkill));
		}

		// Token: 0x060081C5 RID: 33221 RVA: 0x003C6F58 File Offset: 0x003C5158
		private void SelectSkill()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CharId", SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
			argBox.Set("ShowCombatSkill", true);
			argBox.Set("ExitByAction", true);
			argBox.Set("CheckEquipRequirePracticeLevel", false);
			argBox.Set("ShowNone", false);
			argBox.SetObject("UnselectableCombatSkillList", new List<short>());
			argBox.SetObject("CombatSkillIdList", this._learnedNeigong);
			argBox.SetObject("CallbackCombatSkill", new Action<CombatSkillDisplayData>(delegate(CombatSkillDisplayData skillData)
			{
				this.Refresh(skillData);
			}));
			argBox.Set("IsShowNeiLiFinish", false);
			UIElement.SelectSkill.SetOnInitArgs(argBox);
			UIManager.Instance.MaskUI(UIElement.SelectSkill);
		}

		// Token: 0x060081C6 RID: 33222 RVA: 0x003C701C File Offset: 0x003C521C
		private void OnDeselectAll()
		{
			this.infomationPage.gameObject.SetActive(false);
			this.selectPage.gameObject.SetActive(true);
			this.selectAmount.gameObject.SetActive(false);
			this.btnDeselect.gameObject.SetActive(false);
			this.txtSelectedAmount.text = "0/1";
		}

		// Token: 0x17000E3B RID: 3643
		// (get) Token: 0x060081C7 RID: 33223 RVA: 0x003C7083 File Offset: 0x003C5283
		public int CurAmount
		{
			get
			{
				return this.selectAmount.CurCount;
			}
		}

		// Token: 0x060081C8 RID: 33224 RVA: 0x003C7090 File Offset: 0x003C5290
		public void Refresh(CombatSkillDisplayData selectedCombatSkill, int maxLoopingCount, int initAmount)
		{
			CombatSkillDomainMethod.AsyncCall.GetLearnedCombatSkillByType(null, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, 0, delegate(int offset, RawDataPool dataPoll)
			{
				Serializer.Deserialize(dataPoll, offset, ref this._learnedNeigong);
			});
			this.btnDeselect.interactable = (selectedCombatSkill != null);
			this.selectedCombatSkill = selectedCombatSkill;
			this.combatSkillItem.Set(selectedCombatSkill);
			this.selectAmount.Rerfresh(maxLoopingCount, 1, initAmount, false, false, 1, delegate(int newValue)
			{
				this.GetLoopPreviewInfomation();
			});
		}

		// Token: 0x060081C9 RID: 33225 RVA: 0x003C70FF File Offset: 0x003C52FF
		private void Refresh(CombatSkillDisplayData selectedCombatSkill)
		{
			this.btnDeselect.interactable = (selectedCombatSkill != null);
			this.selectedCombatSkill = selectedCombatSkill;
			this.combatSkillItem.Set(selectedCombatSkill);
			this.GetLoopPreviewInfomation();
		}

		// Token: 0x060081CA RID: 33226 RVA: 0x003C712E File Offset: 0x003C532E
		private void GetLoopPreviewInfomation()
		{
			CombatSkillDomainMethod.AsyncCall.CalcTaiwuExtraDeltaNeiliAllocationLoops(null, (int)this.selectedCombatSkill.TemplateId, this.CurAmount, delegate(int offset, RawDataPool dataPoll)
			{
				Serializer.Deserialize(dataPoll, offset, ref this._loopInfomation);
				this.loopingInformation.SetData(this.selectedCombatSkill, this._loopInfomation.ReferenceSkillList, this._loopInfomation.ExtraNeiliAllocationProgress.Items, this._loopInfomation.TaiwuQiArtStrategyList, this._loopInfomation.ExtraNeiliTotalRange, this._loopInfomation.ExtraNeiliAllocationTotal, null, false, null, this.CurAmount);
				this.infomationPage.gameObject.SetActive(true);
				this.selectAmount.gameObject.SetActive(true);
				this.selectPage.gameObject.SetActive(false);
				this.btnDeselect.gameObject.SetActive(true);
				this.txtSelectedAmount.text = "1/1";
			});
		}

		// Token: 0x04006333 RID: 25395
		[SerializeField]
		private GameObject selectPage;

		// Token: 0x04006334 RID: 25396
		[SerializeField]
		private GameObject infomationPage;

		// Token: 0x04006335 RID: 25397
		[Header("titleLayout")]
		[SerializeField]
		private TextMeshProUGUI txtSelectedAmount;

		// Token: 0x04006336 RID: 25398
		[SerializeField]
		private CButton btnDeselect;

		// Token: 0x04006337 RID: 25399
		[Header("selectPage")]
		[SerializeField]
		private CButton btnSelectSkill;

		// Token: 0x04006338 RID: 25400
		[Header("infomationPage")]
		[SerializeField]
		private CharacterMenuCombatSkillItem combatSkillItem;

		// Token: 0x04006339 RID: 25401
		[SerializeField]
		private CombatSkillLoopingInformation loopingInformation;

		// Token: 0x0400633A RID: 25402
		[SerializeField]
		private EventWindowSetSelectAmount selectAmount;

		// Token: 0x0400633C RID: 25404
		private CombatSkillNeigongLoopInformation _loopInfomation;

		// Token: 0x0400633D RID: 25405
		private List<short> _learnedNeigong = new List<short>();
	}
}
