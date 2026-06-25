using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Combat;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;
using GameDataExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002E2 RID: 738
public class MouseTipWeapon : MouseTipItem
{
	// Token: 0x170004D4 RID: 1236
	// (get) Token: 0x06002BA7 RID: 11175 RVA: 0x00154E2A File Offset: 0x0015302A
	protected override bool CanStick
	{
		get
		{
			bool result;
			if (UIManager.Instance.CheckPopupElementIsInTop(UIElement.CharacterMenuEquip))
			{
				ItemDisplayData itemData = this._itemData;
				result = (itemData != null && itemData.UsingType == ItemDisplayData.ItemUsingType.Equiped);
			}
			else
			{
				result = true;
			}
			return result;
		}
	}

	// Token: 0x06002BA8 RID: 11176 RVA: 0x00154E5C File Offset: 0x0015305C
	protected override void Init(ArgumentBox argsBox)
	{
		argsBox.Get<ItemDisplayData>("ItemData", out this._itemData);
		argsBox.Get<ItemKey>("ItemKey", out this._itemKey);
		argsBox.Get("IsInCompareUI", out this._isInCompareUI);
		argsBox.Get("TemplateDataOnly", out this._templateDataOnly);
		argsBox.Get("GetNewItemDisplayData", out this._GetNewItemDisplayData);
		bool flag = !argsBox.Get("CharId", out this._charId);
		if (flag)
		{
			this._charId = -1;
		}
		base.Init(argsBox);
		bool flag2 = this.Element == null;
		if (flag2)
		{
			this.OnListenerIdReady();
		}
		else
		{
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
		}
		base.PostInit();
		GlobalDomainMethod.Call.InvokeGuidingTrigger(263);
	}

	// Token: 0x06002BA9 RID: 11177 RVA: 0x00154F3C File Offset: 0x0015313C
	private void OnListenerIdReady()
	{
		bool getNewItemDisplayData = this._GetNewItemDisplayData;
		if (getNewItemDisplayData)
		{
			ItemKey requestKey;
			int requestCharId;
			if (this._itemData != null)
			{
				ItemKey key = this._itemData.Key;
				int num = this._itemData.OwnerCharId;
				requestKey = key;
				requestCharId = num;
			}
			else
			{
				ItemKey itemKey = this._itemKey;
				int num = this._charId;
				requestKey = itemKey;
				requestCharId = num;
			}
			ItemDomainMethod.AsyncCall.GetItemDisplayData(this, requestKey, requestCharId, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this._itemData);
				this.ShowData();
			});
		}
		else
		{
			this.ShowData();
		}
	}

	// Token: 0x06002BAA RID: 11178 RVA: 0x00154FB0 File Offset: 0x001531B0
	private unsafe void ShowData()
	{
		bool flag = this._itemData.Key.ItemType != 0;
		if (!flag)
		{
			this._itemKey = this._itemData.Key;
			WeaponItem configData = Weapon.Instance[this._itemData.Key.TemplateId];
			TextMeshProUGUI currDurabilityYellow = base.CGet<TextMeshProUGUI>("CurrDurabilityYellow");
			TextMeshProUGUI currDurabilityRed = base.CGet<TextMeshProUGUI>("CurrDurabilityRed");
			StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
			base.CGet<TextMeshProUGUI>("Name").text = configData.Name;
			base.CGet<CImage>("GradeBack").SetSprite(ItemView.GetGradeIcon(configData.Grade), false, null);
			base.CGet<TextMeshProUGUI>("GradeName").text = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", configData.Grade));
			base.CGet<TextMeshProUGUI>("Grade").text = (LocalStringManager.Get(string.Format("LK_Num_{0}", (int)(9 - configData.Grade))) + LocalStringManager.Get(LanguageKey.LK_Item_Grade)).SetColor(Colors.Instance.GradeColors[(int)configData.Grade]);
			base.CGet<TextMeshProUGUI>("Value").text = (this._templateDataOnly ? configData.BaseValue.ToString() : this._itemData.Value.ToString());
			base.CGet<GameObject>("Durability").SetActive(!this._templateDataOnly);
			base.CGet<GameObject>("Material").SetActive(!this._templateDataOnly);
			base.CGet<CImage>("ItemIcon").SetSprite(configData.Icon, false, null);
			base.SetItemDesc(configData.Desc, this._itemData.LoveTokenDataItem);
			bool hasHalfDurability = this._itemData.Durability > this._itemData.MaxDurability / 2;
			strBuilder.Clear();
			bool flag2 = configData.ResourceType >= 0;
			if (flag2)
			{
				strBuilder.Append(Config.ResourceType.Instance[configData.ResourceType].Name);
			}
			strBuilder.Append(LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", configData.ItemSubType)));
			base.CGet<TextMeshProUGUI>("SubType").text = strBuilder.ToString();
			currDurabilityYellow.gameObject.SetActive(hasHalfDurability);
			currDurabilityRed.gameObject.SetActive(!hasHalfDurability);
			(hasHalfDurability ? currDurabilityYellow : currDurabilityRed).text = this._itemData.Durability.ToString();
			base.CGet<TextMeshProUGUI>("MaxDurability").text = string.Format("/{0}", this._itemData.MaxDurability);
			base.CGet<TextMeshProUGUI>("Weight").text = NumberFormatUtils.FormatItemWeight(this._itemData.Weight);
			base.CGet<TextMeshProUGUI>("EquipAttack").text = string.Format("{0:f2}", (float)this._itemData.EquipmentAttack / 100f);
			base.CGet<TextMeshProUGUI>("EquipDefend").text = string.Format("{0:f2}", (float)this._itemData.EquipmentDefense / 100f);
			this.InitItemDisableFunctionList(this._itemData);
			base.RefreshDisableFunction();
			RectTransform hitTypeHolder = base.CGet<RectTransform>("HitTypeHolder");
			bool hasAnyHit = false;
			for (sbyte hitType = 0; hitType < 4; hitType += 1)
			{
				Refers hitRefers = hitTypeHolder.GetChild((int)hitType).GetComponent<Refers>();
				int hitValue = (int)(*(ref this._itemData.HitAvoidFactor.Items.FixedElementField + (IntPtr)hitType * 2));
				hitRefers.gameObject.SetActive(hitValue != 0);
				bool flag3 = hitValue != 0;
				if (flag3)
				{
					hasAnyHit = true;
					hitRefers.CGet<TextMeshProUGUI>("AddValue").text = ((hitValue > 0) ? string.Format("{0}%", 100 + hitValue) : "");
					hitRefers.CGet<TextMeshProUGUI>("ReduceValue").text = ((hitValue > 0) ? "" : string.Format("{0}%", 100 + hitValue));
				}
			}
			base.CGet<GameObject>("HitTypes").SetActive(hasAnyHit);
			base.RefreshPoisons(configData.InnatePoisons, this._itemData);
			base.RefreshAttachedEffect();
			base.CGet<GameObject>("SkillEffects").SetActive(false);
			base.RefreshRequirement(this._itemData.Requirements);
			this.UpdateUsePowerRelatedInfo();
			EasyPool.Free<StringBuilder>(strBuilder);
			this.NeedWaitData = this.NeedRefresh;
			bool needRefresh = this.NeedRefresh;
			if (needRefresh)
			{
				this.Refresh();
			}
			bool templateDataOnly = this._templateDataOnly;
			if (templateDataOnly)
			{
				this.SetWeaponTricks(Weapon.Instance[this._itemData.Key.TemplateId].Tricks);
			}
			else
			{
				ItemDomainMethod.AsyncCall.GetWeaponTricks(this, this._itemKey, new AsyncMethodCallbackDelegate(this.OnGetWeaponTricks));
			}
			bool flag4 = this._charId >= 0;
			if (flag4)
			{
				ItemDomainMethod.AsyncCall.GetWeaponPrepareFrame(this, this._charId, this._itemKey, new AsyncMethodCallbackDelegate(this.OnGetWeaponPrepareFrame));
				ItemDomainMethod.AsyncCall.GetWeaponAttackRange(this, this._charId, this._itemKey, new AsyncMethodCallbackDelegate(this.OnGetWeaponAttackRange));
			}
			else
			{
				this.SetPrepareFrame(CFormula.CalcAttackStartupOrRecoveryFrame(100, configData.BaseStartupFrames));
				this.SetAttackRange((int)configData.MinDistance, (int)configData.MaxDistance);
			}
			base.RefreshHoldCount();
			base.RefreshHotkeyDisplayLockItem();
			base.ForceRebuildLayout(2U, null);
			bool flag5 = this.Element != null;
			if (flag5)
			{
				this.Element.ShowAfterRefresh();
			}
		}
	}

	// Token: 0x06002BAB RID: 11179 RVA: 0x00155560 File Offset: 0x00153760
	protected override void InitItemDisableFunctionList(ItemDisplayData itemDisplayData)
	{
		base.InitItemDisableFunctionList(itemDisplayData);
		WeaponItem configData = Weapon.Instance[itemDisplayData.Key.TemplateId];
		bool flag = !configData.Repairable;
		if (flag)
		{
			this._disableFunctionList.Add(MouseTipItem.ItemFunction.Repairable);
		}
		bool flag2 = !configData.Transferable;
		if (flag2)
		{
			this._disableFunctionList.Add(MouseTipItem.ItemFunction.Transferable);
		}
		bool flag3 = !configData.Poisonable;
		if (flag3)
		{
			this._disableFunctionList.Add(MouseTipItem.ItemFunction.Poisonable);
		}
		bool flag4 = !configData.Refinable;
		if (flag4)
		{
			this._disableFunctionList.Add(MouseTipItem.ItemFunction.Refinable);
		}
		bool flag5 = !configData.CanChangeTrick;
		if (flag5)
		{
			this._disableFunctionList.Add(MouseTipItem.ItemFunction.CanChangeTrick);
		}
	}

	// Token: 0x06002BAC RID: 11180 RVA: 0x00155614 File Offset: 0x00153814
	public override void Refresh()
	{
		bool flag = this._charId >= 0;
		if (flag)
		{
			CharacterDomainMethod.AsyncCall.GetItemPowerInfo(this, this._charId, this._itemKey, new AsyncMethodCallbackDelegate(this.OnGetPowerInfo));
		}
		bool exist = UIElement.Combat.Exist;
		if (exist)
		{
			CombatDomainMethod.AsyncCall.GetWeaponInnerRatio(this, this._itemKey, new AsyncMethodCallbackDelegate(this.OnGetWeaponInnerRatio));
			CombatDomainMethod.AsyncCall.GetWeaponEffects(this, this._itemKey, new AsyncMethodCallbackDelegate(this.OnGetWeaponEffects));
		}
	}

	// Token: 0x06002BAD RID: 11181 RVA: 0x00155694 File Offset: 0x00153894
	private void OnGetPowerInfo(int offset, RawDataPool dataPool)
	{
		bool flag = null == this;
		if (!flag)
		{
			bool flag2 = this._itemData == null;
			if (!flag2)
			{
				ItemPowerInfo usePowerInfo = default(ItemPowerInfo);
				Serializer.Deserialize(dataPool, offset, ref usePowerInfo);
				this._itemData.PowerInfo = usePowerInfo;
				this.UpdateUsePowerRelatedInfo();
			}
		}
	}

	// Token: 0x06002BAE RID: 11182 RVA: 0x001556E4 File Offset: 0x001538E4
	private void OnGetWeaponInnerRatio(int offset, RawDataPool dataPool)
	{
		bool flag = null == this;
		if (!flag)
		{
			bool flag2 = this._itemData == null;
			if (!flag2)
			{
				sbyte innerRatio = 0;
				Serializer.Deserialize(dataPool, offset, ref innerRatio);
				int totalPenetrate = (int)(this._itemData.PenetrationInfo.Item1 * this._itemData.PowerInfo.Power / 100);
				int innerPenetrate = totalPenetrate * (int)innerRatio / 100;
				int outerPenetrate = totalPenetrate - innerPenetrate;
				this.SetPenetrate(outerPenetrate, innerPenetrate);
			}
		}
	}

	// Token: 0x06002BAF RID: 11183 RVA: 0x00155758 File Offset: 0x00153958
	private void OnGetWeaponEffects(int offset, RawDataPool dataPool)
	{
		bool flag = this == null;
		if (!flag)
		{
			WeaponEffectDisplayData[] effectKeys = null;
			Serializer.Deserialize(dataPool, offset, ref effectKeys);
			List<WeaponEffectDisplayData> keyList = effectKeys.FindAll((WeaponEffectDisplayData key) => key.EffectKey.SkillId >= 0);
			base.CGet<GameObject>("SkillEffects").SetActive(keyList.Count > 0);
			bool flag2 = keyList.Count > 0;
			if (flag2)
			{
				RectTransform effectHolder = base.CGet<RectTransform>("SkillEffectHolder");
				GameObject effectPrefab = effectHolder.GetChild(0).gameObject;
				StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
				for (int i = 0; i < keyList.Count; i++)
				{
					WeaponEffectDisplayData effectKey = keyList[i];
					string desc = CommonUtils.GetSpecialEffectDescSpecifyIndex(effectKey.EffectDescription, 0, 1);
					int colonIndex = desc.IndexOf(LocalStringManager.Get(LanguageKey.LK_Colon_Symbol), StringComparison.Ordinal);
					string descPart = desc.Substring(0, colonIndex + 1).SetColor("male");
					string descPart2 = desc.Substring(colonIndex + 1);
					GameObject effectObj = (i < effectHolder.childCount) ? effectHolder.GetChild(i).gameObject : Object.Instantiate<GameObject>(effectPrefab, effectHolder, false);
					effectObj.GetComponent<TextMeshProUGUI>().text = descPart + descPart2;
					LayoutRebuilder.ForceRebuildLayoutImmediate(effectObj.GetComponent<RectTransform>());
				}
				for (int j = keyList.Count; j < effectHolder.childCount; j++)
				{
					effectHolder.GetChild(j).gameObject.SetActive(false);
				}
				EasyPool.Free<StringBuilder>(strBuilder);
			}
			base.RefreshAttachedEffectLayout();
		}
	}

	// Token: 0x06002BB0 RID: 11184 RVA: 0x001558FC File Offset: 0x00153AFC
	private void OnGetWeaponTricks(int offset, RawDataPool dataPool)
	{
		bool flag = this == null;
		if (!flag)
		{
			List<sbyte> tricks = null;
			Serializer.Deserialize(dataPool, offset, ref tricks);
			this.SetWeaponTricks(tricks);
		}
	}

	// Token: 0x06002BB1 RID: 11185 RVA: 0x0015592C File Offset: 0x00153B2C
	public void SetWeaponTricks(List<sbyte> tricks)
	{
		StringBuilder builder = EasyPool.Get<StringBuilder>();
		builder.Append(LocalStringManager.Get(LanguageKey.LK_WeaponTrick));
		builder.Append(LocalStringManager.Get(LanguageKey.LK_Colon_Symbol));
		builder.Append(string.Join(LocalStringManager.Get(LanguageKey.LK_Dot_Symbol), from x in tricks
		select Config.TrickType.Instance[x].Name).SetColor("pinkyellow"));
		base.CGet<TextMeshProUGUI>("Trick").text = builder.ToString();
		EasyPool.Free<StringBuilder>(builder);
	}

	// Token: 0x06002BB2 RID: 11186 RVA: 0x001559C8 File Offset: 0x00153BC8
	private void OnGetWeaponPrepareFrame(int offset, RawDataPool pool)
	{
		bool flag = this == null;
		if (!flag)
		{
			int prepareFrame = 0;
			Serializer.Deserialize(pool, offset, ref prepareFrame);
			this.SetPrepareFrame(prepareFrame);
		}
	}

	// Token: 0x06002BB3 RID: 11187 RVA: 0x001559F8 File Offset: 0x00153BF8
	private void OnGetWeaponAttackRange(int offset, RawDataPool dataPool)
	{
		bool flag = this == null;
		if (!flag)
		{
			ValueTuple<int, int> attackRange = default(ValueTuple<int, int>);
			Serializer.Deserialize(dataPool, offset, ref attackRange);
			this.SetAttackRange(attackRange.Item1, attackRange.Item2);
		}
	}

	// Token: 0x06002BB4 RID: 11188 RVA: 0x00155A38 File Offset: 0x00153C38
	private void UpdateUsePowerRelatedInfo()
	{
		WeaponItem configData = Weapon.Instance[this._itemData.Key.TemplateId];
		bool flag = configData == null;
		if (!flag)
		{
			StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
			short power = this._itemData.PowerInfo.Power;
			bool shouldShowPower = this._itemData.ShouldShowPower();
			string powerStr = shouldShowPower ? (this._itemData.PowerInfo.Power.ToString() ?? "") : "-";
			this.SetDurabilityPosition(shouldShowPower);
			base.CGet<TextMeshProUGUI>("Power").text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_WeaponAndArmor_Power, powerStr);
			base.CGet<TextMeshProUGUI>("ChangeTrick").text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTipWeapon_ChangeTrick, ((int)(configData.ChangeTrickPercent * power / 100)).ToString()).ColorReplace();
			base.CGet<TextMeshProUGUI>("PursueRate").text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTipWeapon_PursueRate, ((int)(configData.PursueAttackFactor * power / 100)).ToString()).ColorReplace();
			int totalPenetrate = (int)(this._itemData.PenetrationInfo.Item1 * power / 100);
			int innerPenetrate = totalPenetrate * (int)configData.DefaultInnerRatio / 100;
			int outerPenetrate = totalPenetrate - innerPenetrate;
			this.SetPenetrate(outerPenetrate, innerPenetrate);
			base.CGet<TextMeshProUGUI>("CurrAndMaxPower").text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_WeaponAndArmor_RequirementsPower, shouldShowPower ? this._itemData.PowerInfo.RequirementsPower.ToString() : "-", this._itemData.PowerInfo.MaxPower.ToString());
			EasyPool.Free<StringBuilder>(strBuilder);
		}
	}

	// Token: 0x06002BB5 RID: 11189 RVA: 0x00155BD8 File Offset: 0x00153DD8
	private void SetDurabilityPosition(bool shouldShowPower)
	{
		base.CGet<RectTransform>("PowerSpace").gameObject.SetActive(shouldShowPower);
		base.CGet<RectTransform>("PowerAndRange").gameObject.SetActive(shouldShowPower);
		Transform durability = base.CGet<GameObject>("Durability").transform;
		durability.SetParent(base.CGet<RectTransform>(shouldShowPower ? "DurabilityHolderWithPower" : "DurabilityHolderNoPower"));
		(durability as RectTransform).anchoredPosition = Vector2.zero;
	}

	// Token: 0x06002BB6 RID: 11190 RVA: 0x00155C52 File Offset: 0x00153E52
	private void Update()
	{
		base.UpdateCompare();
	}

	// Token: 0x06002BB7 RID: 11191 RVA: 0x00155C5C File Offset: 0x00153E5C
	public override void SetNewData(ArgumentBox argsBox)
	{
		this.Init(argsBox);
	}

	// Token: 0x06002BB8 RID: 11192 RVA: 0x00155C68 File Offset: 0x00153E68
	private void SetPrepareFrame(int prepareFrame)
	{
		float seconds = (float)prepareFrame / 60f;
		base.CGet<TextMeshProUGUI>("PrepareFrame").text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTipWeapon_PrepareFrame, seconds.ToString("F2")).ColorReplace();
	}

	// Token: 0x06002BB9 RID: 11193 RVA: 0x00155CAC File Offset: 0x00153EAC
	private void SetAttackRange(int min, int max)
	{
		string minStr = ((float)min / 10f).ToString("F1");
		string maxStr = ((float)max / 10f).ToString("F1");
		base.CGet<TextMeshProUGUI>("AttackRange").text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTipWeapon_AttackRange, minStr + "-" + maxStr).ColorReplace();
	}

	// Token: 0x06002BBA RID: 11194 RVA: 0x00155D14 File Offset: 0x00153F14
	private void SetPenetrate(int outer, int inner)
	{
		TextMeshProUGUI penetrate = base.CGet<TextMeshProUGUI>("Penetrate");
		penetrate.text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTipWeapon_Penetrate, outer.ToString(), inner.ToString()).ColorReplace();
		penetrate.GetComponent<TMPTextSpriteHelper>().Parse();
	}

	// Token: 0x04001FD9 RID: 8153
	private bool _templateDataOnly;

	// Token: 0x04001FDA RID: 8154
	private bool _GetNewItemDisplayData;
}
