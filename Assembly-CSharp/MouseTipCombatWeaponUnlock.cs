using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using GameData.Domains.Combat;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200028A RID: 650
public class MouseTipCombatWeaponUnlock : MouseTipBase
{
	// Token: 0x1700048D RID: 1165
	// (get) Token: 0x060029C5 RID: 10693 RVA: 0x0013C88F File Offset: 0x0013AA8F
	protected override bool CanStick
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060029C6 RID: 10694 RVA: 0x0013C894 File Offset: 0x0013AA94
	protected override void Init(ArgumentBox argsBox)
	{
		argsBox.Get("WeaponTemplateId", out this._weaponTemplateId);
		argsBox.Get("WeaponIndex", out this._weaponIndex);
		argsBox.Get("IsAlly", out this._isAlly);
		this.SetUnlockDesc();
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
	}

	// Token: 0x060029C7 RID: 10695 RVA: 0x0013C906 File Offset: 0x0013AB06
	private void OnListenerIdReady()
	{
		CombatDomainMethod.AsyncCall.GetUnlockSimulateResult(this, this._weaponIndex, this._isAlly, delegate(int offset, RawDataPool dataPool)
		{
			Serializer.Deserialize(dataPool, offset, ref this._unlockSimulateResult);
			GameObject tipGameObject = base.CGet<GameObject>("Tip");
			GameObject title2GameObject = base.CGet<GameObject>("Title2");
			GameObject gameObject = tipGameObject;
			UnlockSimulateResult unlockSimulateResult = this._unlockSimulateResult;
			gameObject.SetActive(unlockSimulateResult != null && unlockSimulateResult.AllBlocked);
			bool flag = this._unlockSimulateResult == null || this._unlockSimulateResult.AllRawCreateEffects.Count<int>() == 0;
			if (flag)
			{
				title2GameObject.SetActive(false);
			}
			else
			{
				title2GameObject.SetActive(true);
				this.ProcessDisplayData();
			}
		});
	}

	// Token: 0x060029C8 RID: 10696 RVA: 0x0013C928 File Offset: 0x0013AB28
	private void SetUnlockDesc()
	{
		WeaponItem weaponItem = Weapon.Instance[this._weaponTemplateId];
		bool flag = weaponItem.UnlockEffect >= 0;
		if (flag)
		{
			WeaponUnlockEffectItem weaponUnlockEffectItem = WeaponUnlockEffect.Instance[weaponItem.UnlockEffect];
			base.CGet<TextMeshProUGUI>("Desc").text = weaponUnlockEffectItem.Desc;
		}
		else
		{
			base.CGet<TextMeshProUGUI>("Desc").gameObject.SetActive(false);
		}
	}

	// Token: 0x060029C9 RID: 10697 RVA: 0x0013C99C File Offset: 0x0013AB9C
	private void Awake()
	{
		for (int i = 1; i <= 5; i++)
		{
			this._rows.Add(base.CGet<GameObject>(string.Format("Row{0}", i)));
		}
		for (int j = 0; j < 5; j++)
		{
			this._rows[j].transform.GetChild(0).gameObject.SetActive(false);
			this._rows[j].transform.GetChild(1).gameObject.SetActive(false);
			this._rows[j].SetActive(false);
		}
	}

	// Token: 0x060029CA RID: 10698 RVA: 0x0013CA4C File Offset: 0x0013AC4C
	protected override void OnDisable()
	{
		base.OnDisable();
		for (int i = 0; i < 5; i++)
		{
			this._rows[i].transform.GetChild(0).gameObject.SetActive(false);
			this._rows[i].transform.GetChild(1).gameObject.SetActive(false);
			this._rows[i].SetActive(false);
		}
	}

	// Token: 0x060029CB RID: 10699 RVA: 0x0013CACC File Offset: 0x0013ACCC
	private void ProcessDisplayData()
	{
		base.CGet<TextMeshProUGUI>("Desc").rectTransform.SetWidth((this._unlockSimulateResult.AllRawCreateEffects.Count<int>() <= 5) ? this._weight1 : this._weight2);
		bool flag = this._unlockSimulateResult.AllRawCreateEffects.Count<int>() <= 5;
		if (flag)
		{
			int count = 0;
			foreach (int specialEffectTemplateId in this._unlockSimulateResult.AllRawCreateEffects)
			{
				GameObject obj = this._rows[count].transform.GetChild(0).gameObject;
				this._rows[count].SetActive(true);
				count++;
				obj.SetActive(true);
				this.FillData(obj.GetComponent<Refers>(), (short)specialEffectTemplateId);
			}
		}
		else
		{
			int count2 = 0;
			bool isFirstChild = true;
			foreach (int specialEffectTemplateId2 in this._unlockSimulateResult.AllRawCreateEffects)
			{
				int index = isFirstChild ? 0 : 1;
				GameObject obj2 = this._rows[count2].transform.GetChild(index).gameObject;
				this._rows[count2].SetActive(true);
				obj2.SetActive(true);
				this.FillData(obj2.GetComponent<Refers>(), (short)specialEffectTemplateId2);
				bool flag2 = !isFirstChild;
				if (flag2)
				{
					count2++;
				}
				isFirstChild = !isFirstChild;
				bool flag3 = count2 >= this._rows.Count;
				if (flag3)
				{
					break;
				}
			}
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(base.GetComponent<RectTransform>());
		this.Element.ShowAfterRefresh();
	}

	// Token: 0x060029CC RID: 10700 RVA: 0x0013CCBC File Offset: 0x0013AEBC
	private void FillData(Refers refers, short specialEffectTemplateId)
	{
		SpecialEffectItem specialEffectItem = SpecialEffect.Instance[specialEffectTemplateId];
		CombatSkillItem combatSkillItem = CombatSkill.Instance[specialEffectItem.SkillTemplateId];
		CImage icon = refers.CGet<CImage>("Icon");
		CImage direct = refers.CGet<CImage>("Direct");
		icon.SetSprite(combatSkillItem.Icon, false, null);
		direct.SetSprite((combatSkillItem.DirectEffectID == (int)specialEffectTemplateId) ? "mousetip_zhengni_0" : "mousetip_zhengni_1", false, null);
		DisableStyleRoot disableStyleRoot = refers.CGet<DisableStyleRoot>("DisableStyleRoot");
		UnlockSimulateResult unlockSimulateResult = this._unlockSimulateResult;
		bool flag = unlockSimulateResult != null && unlockSimulateResult.BlockedRawCreateEffects != null && this._unlockSimulateResult.BlockedRawCreateEffects.Contains((int)specialEffectTemplateId);
		if (flag)
		{
			disableStyleRoot.SetStyleEffect(true, false);
		}
		else
		{
			disableStyleRoot.SetStyleEffect(false, false);
		}
		icon.SetColor(Colors.Instance.FiveElementsColors[(int)combatSkillItem.FiveElements]);
		sbyte grade = combatSkillItem.Grade;
		refers.CGet<CImage>("GradeBack").SetSprite(ItemView.GetGradeIcon(grade), false, null);
		refers.CGet<TextMeshProUGUI>("Grade").text = ItemView.GetGradeText(grade);
		refers.CGet<TextMeshProUGUI>("Description").text = CommonUtils.GetSpecialEffectDescSpecifyIndex((int)specialEffectItem.TemplateId, 1, -1).SetColor((combatSkillItem.DirectEffectID == (int)specialEffectTemplateId) ? "brightblue" : "brightred");
		refers.CGet<TextMeshProUGUI>("CombatSkillName").text = combatSkillItem.Name.SetColor(Colors.Instance.GradeColors[(int)grade]);
	}

	// Token: 0x04001E5C RID: 7772
	private int _weaponIndex;

	// Token: 0x04001E5D RID: 7773
	private short _weaponTemplateId;

	// Token: 0x04001E5E RID: 7774
	private bool _isAlly;

	// Token: 0x04001E5F RID: 7775
	private UnlockSimulateResult _unlockSimulateResult = new UnlockSimulateResult();

	// Token: 0x04001E60 RID: 7776
	private readonly List<GameObject> _rows = new List<GameObject>();

	// Token: 0x04001E61 RID: 7777
	private float _weight1 = 637f;

	// Token: 0x04001E62 RID: 7778
	private float _weight2 = 1283f;

	// Token: 0x04001E63 RID: 7779
	private const int RowCount = 5;
}
