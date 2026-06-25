using System;
using Config;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Combat;
using GameData.Domains.Item;
using TMPro;
using UnityEngine;

// Token: 0x02000153 RID: 339
public class CombatWeaponInnerRatio : MonoBehaviour, ICombatComponent
{
	// Token: 0x17000218 RID: 536
	// (get) Token: 0x060012BE RID: 4798 RVA: 0x00072713 File Offset: 0x00070913
	private CombatModel Model
	{
		get
		{
			return SingletonObject.getInstance<CombatModel>();
		}
	}

	// Token: 0x060012BF RID: 4799 RVA: 0x0007271C File Offset: 0x0007091C
	private void Awake()
	{
		this.innerRatioSlider.interactable = (this.canOperate && this.Model.CanOperateSelfCharacter);
		this.handleArea.SetActive(this.canOperate);
		bool flag = !this.canOperate;
		if (!flag)
		{
			this.innerRatioSlider.onValueChanged.AddListener(delegate(float value)
			{
				bool autoSettingWeaponInnerRatioSlider = this._autoSettingWeaponInnerRatioSlider;
				if (!autoSettingWeaponInnerRatioSlider)
				{
					bool flag2 = this.ally;
					if (flag2)
					{
						this.OnInnerRatioSliderValueChanged(value);
					}
				}
			});
		}
	}

	// Token: 0x060012C0 RID: 4800 RVA: 0x0007278C File Offset: 0x0007098C
	public void Setup()
	{
		this.innerRatioSlider.interactable = true;
		CombatModel model = this.Model;
		model.OnWeaponsChanged = (OnDataChangedEvent)Delegate.Combine(model.OnWeaponsChanged, new OnDataChangedEvent(this.OnWeaponsChanged));
		CombatModel model2 = this.Model;
		model2.OnUsingWeaponIndexChanged = (OnDataChangedEvent)Delegate.Combine(model2.OnUsingWeaponIndexChanged, new OnDataChangedEvent(this.OnUsingWeaponIndexChanged));
		CombatModel model3 = this.Model;
		model3.OnWeaponInnerRatioChanged = (OnWeaponDataChangedEvent)Delegate.Combine(model3.OnWeaponInnerRatioChanged, new OnWeaponDataChangedEvent(this.OnWeaponInnerRatioChanged));
		this.Model.AddEvent(ECombatEvents.OnWeaponInnerRatioChanged, new OnCombatEvent(this.OnWeaponExpectedInnerRatioChanged));
		this.Model.AddEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
		this.Model.AddEvent(ECombatEvents.CombatEnd, new OnCombatEvent(this.OnCombatEnd));
	}

	// Token: 0x060012C1 RID: 4801 RVA: 0x00072868 File Offset: 0x00070A68
	public void Close()
	{
		CombatModel model = this.Model;
		model.OnWeaponsChanged = (OnDataChangedEvent)Delegate.Remove(model.OnWeaponsChanged, new OnDataChangedEvent(this.OnWeaponsChanged));
		CombatModel model2 = this.Model;
		model2.OnUsingWeaponIndexChanged = (OnDataChangedEvent)Delegate.Remove(model2.OnUsingWeaponIndexChanged, new OnDataChangedEvent(this.OnUsingWeaponIndexChanged));
		CombatModel model3 = this.Model;
		model3.OnWeaponInnerRatioChanged = (OnWeaponDataChangedEvent)Delegate.Combine(model3.OnWeaponInnerRatioChanged, new OnWeaponDataChangedEvent(this.OnWeaponInnerRatioChanged));
		this.Model.RemoveEvent(ECombatEvents.OnWeaponInnerRatioChanged, new OnCombatEvent(this.OnWeaponExpectedInnerRatioChanged));
		this.Model.RemoveEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
		this.Model.RemoveEvent(ECombatEvents.CombatEnd, new OnCombatEvent(this.OnCombatEnd));
	}

	// Token: 0x060012C2 RID: 4802 RVA: 0x00072938 File Offset: 0x00070B38
	private void Set(sbyte weaponInnerRatio, short characterInnerRatio, short weaponTemplateId)
	{
		WeaponItem weaponConfig = Weapon.Instance[weaponTemplateId];
		int baseRatio = (int)weaponConfig.DefaultInnerRatio;
		int changeRange = (int)((short)weaponConfig.InnerRatioAdjustRange * characterInnerRatio / 100);
		Vector2Int innerRatioRange = new Vector2Int(Math.Max(baseRatio - changeRange, 0), Math.Min(baseRatio + changeRange, 100));
		this.leftRange.fillAmount = (float)(100 - innerRatioRange.y) / 100f;
		this.rightRange.fillAmount = (float)innerRatioRange.x / 100f;
		this.outerRatioLabel.text = string.Format("{0}%", (int)(100 - weaponInnerRatio));
		this.innerRatioLabel.text = string.Format("{0}%", weaponInnerRatio);
		this.leftBar.fillAmount = (float)(100 - weaponInnerRatio) / 100f;
		this.rightBar.fillAmount = (float)weaponInnerRatio / 100f;
		this.currentRatio.anchoredPosition = new Vector2(245f * (float)(100 - weaponInnerRatio) / 100f, 0f);
	}

	// Token: 0x060012C3 RID: 4803 RVA: 0x00072A43 File Offset: 0x00070C43
	private void SetExpectedInnerRatio(float ratio)
	{
		this._autoSettingWeaponInnerRatioSlider = true;
		this.innerRatioSlider.value = ratio;
		this._autoSettingWeaponInnerRatioSlider = false;
	}

	// Token: 0x060012C4 RID: 4804 RVA: 0x00072A61 File Offset: 0x00070C61
	private void OnWeaponsChanged(bool isAlly)
	{
		this.UpdateCurrentInnerRatio(isAlly);
	}

	// Token: 0x060012C5 RID: 4805 RVA: 0x00072A6C File Offset: 0x00070C6C
	private void OnUsingWeaponIndexChanged(bool isAlly)
	{
		this.UpdateCurrentInnerRatio(isAlly);
		if (isAlly)
		{
			this.UpdateExpectedInnerRatio(true);
		}
	}

	// Token: 0x060012C6 RID: 4806 RVA: 0x00072A90 File Offset: 0x00070C90
	private void OnWeaponInnerRatioChanged(ItemKey itemKey)
	{
		CombatSubProcessorCharacter selfCharacter = this.Model.SelfCharacter;
		int index = (selfCharacter != null) ? selfCharacter.Weapons.IndexOf(itemKey) : -1;
		bool isAlly = index >= 0;
		bool flag = index < 0;
		if (flag)
		{
			CombatSubProcessorCharacter enemyCharacter = this.Model.EnemyCharacter;
			index = ((enemyCharacter != null) ? enemyCharacter.Weapons.IndexOf(itemKey) : -1);
		}
		bool flag2 = index < 0;
		if (!flag2)
		{
			this.UpdateCurrentInnerRatio(isAlly);
		}
	}

	// Token: 0x060012C7 RID: 4807 RVA: 0x00072AFC File Offset: 0x00070CFC
	private void OnWeaponExpectedInnerRatioChanged()
	{
		bool taiwuInCombat = this.Model.TaiwuInCombat;
		if (taiwuInCombat)
		{
			this.UpdateExpectedInnerRatio(true);
		}
	}

	// Token: 0x060012C8 RID: 4808 RVA: 0x00072B24 File Offset: 0x00070D24
	private void OnChangeChar()
	{
		int currCharId = this.Model.ChangingToCharId;
		bool flag = this.Model.CharIsAlly(currCharId) != this.ally;
		if (!flag)
		{
			this.UpdateCurrentInnerRatio(currCharId);
			this.UpdateExpectedInnerRatio(currCharId);
		}
	}

	// Token: 0x060012C9 RID: 4809 RVA: 0x00072B6B File Offset: 0x00070D6B
	private void OnCombatEnd()
	{
		this.innerRatioSlider.interactable = false;
	}

	// Token: 0x060012CA RID: 4810 RVA: 0x00072B7C File Offset: 0x00070D7C
	private void UpdateCurrentInnerRatio(bool isAlly)
	{
		bool flag = isAlly != this.ally;
		if (!flag)
		{
			int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
			this.UpdateCurrentInnerRatio(charId);
		}
	}

	// Token: 0x060012CB RID: 4811 RVA: 0x00072BC0 File Offset: 0x00070DC0
	private void UpdateCurrentInnerRatio(int charId)
	{
		CombatSubProcessorCharacter processor;
		bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
		if (!flag)
		{
			int weaponIndex = processor.UsingWeaponIndex;
			ItemKey[] weaponList = processor.Weapons;
			bool flag2 = !weaponList.CheckIndex(weaponIndex);
			if (!flag2)
			{
				ItemKey weaponKey = weaponList[weaponIndex];
				CombatSubProcessorWeapon weaponProcessor;
				bool flag3 = !this.Model.ProcessorWeapons.TryGetValue(weaponKey, out weaponProcessor);
				if (!flag3)
				{
					this.Set(weaponProcessor.InnerRatio, processor.InnerRatio, weaponKey.TemplateId);
				}
			}
		}
	}

	// Token: 0x060012CC RID: 4812 RVA: 0x00072C4C File Offset: 0x00070E4C
	private void UpdateExpectedInnerRatio(bool isAlly)
	{
		bool flag = isAlly != this.ally;
		if (!flag)
		{
			int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
			this.UpdateExpectedInnerRatio(charId);
		}
	}

	// Token: 0x060012CD RID: 4813 RVA: 0x00072C90 File Offset: 0x00070E90
	private void UpdateExpectedInnerRatio(int charId)
	{
		CombatSubProcessorCharacter processor;
		bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
		if (!flag)
		{
			int usingWeaponIndex = processor.UsingWeaponIndex;
			bool flag2 = usingWeaponIndex < 0 || usingWeaponIndex >= 7;
			if (!flag2)
			{
				this.SetExpectedInnerRatio((float)(100 - this.Model.GetWeaponExpectRatio(charId, processor.UsingWeaponIndex)));
			}
		}
	}

	// Token: 0x060012CE RID: 4814 RVA: 0x00072CF4 File Offset: 0x00070EF4
	private void OnInnerRatioSliderValueChanged(float value)
	{
		bool flag = this.Model.SelfCharacter == null;
		if (!flag)
		{
			CombatDomainMethod.Call.ChangeTaiwuWeaponInnerRatio(this.Model.SelfCharacter.UsingWeaponIndex, (sbyte)(100f - value));
		}
	}

	// Token: 0x04000FE9 RID: 4073
	[SerializeField]
	private TextMeshProUGUI innerRatioLabel;

	// Token: 0x04000FEA RID: 4074
	[SerializeField]
	private TextMeshProUGUI outerRatioLabel;

	// Token: 0x04000FEB RID: 4075
	[SerializeField]
	private GameObject handleArea;

	// Token: 0x04000FEC RID: 4076
	[SerializeField]
	private CSlider innerRatioSlider;

	// Token: 0x04000FED RID: 4077
	[SerializeField]
	private CImage leftRange;

	// Token: 0x04000FEE RID: 4078
	[SerializeField]
	private CImage rightRange;

	// Token: 0x04000FEF RID: 4079
	[SerializeField]
	private CImage leftBar;

	// Token: 0x04000FF0 RID: 4080
	[SerializeField]
	private CImage rightBar;

	// Token: 0x04000FF1 RID: 4081
	[SerializeField]
	private RectTransform currentRatio;

	// Token: 0x04000FF2 RID: 4082
	[Tooltip("是否可以操作，不可操作时，Handle会隐藏")]
	[SerializeField]
	private bool canOperate;

	// Token: 0x04000FF3 RID: 4083
	[SerializeField]
	private bool ally;

	// Token: 0x04000FF4 RID: 4084
	private bool _autoSettingWeaponInnerRatioSlider;
}
