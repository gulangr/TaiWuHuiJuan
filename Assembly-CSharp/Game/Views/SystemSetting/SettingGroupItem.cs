using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Combat;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.SystemSetting
{
	// Token: 0x02000773 RID: 1907
	public class SettingGroupItem : MonoBehaviour
	{
		// Token: 0x17000ACC RID: 2764
		// (get) Token: 0x06005BF7 RID: 23543 RVA: 0x002AABA7 File Offset: 0x002A8DA7
		public ESettingSubCategory SubCategory
		{
			get
			{
				return this._subCategory;
			}
		}

		// Token: 0x06005BF8 RID: 23544 RVA: 0x002AABB0 File Offset: 0x002A8DB0
		private void Awake()
		{
			bool flag = this.resetButton != null;
			if (flag)
			{
				this.resetButton.onClick.AddListener(new UnityAction(this.OnResetButtonClick));
			}
		}

		// Token: 0x06005BF9 RID: 23545 RVA: 0x002AABF0 File Offset: 0x002A8DF0
		private void OnDestroy()
		{
			bool flag = this.resetButton != null;
			if (flag)
			{
				this.resetButton.onClick.RemoveListener(new UnityAction(this.OnResetButtonClick));
			}
		}

		// Token: 0x06005BFA RID: 23546 RVA: 0x002AAC30 File Offset: 0x002A8E30
		private void LateUpdate()
		{
			bool flag = !base.enabled;
			if (!flag)
			{
				TooltipInvoker mouseDisplayer = this.resetButton.GetComponent<TooltipInvoker>();
				mouseDisplayer.PresetParam[0] = ((this._subCategory == ESettingSubCategory.RegionStory && GameApp.Instance.GetCurrentGameStateName() != EGameState.InGame) ? LanguageKey.LK_SystemSetting_BaseSettings_NeedGameStart.Tr() : LanguageKey.LK_SystemSetting_ResetBtn_Tips.Tr());
				bool flag2 = !mouseDisplayer.Showing && this._hasShowTips;
				if (flag2)
				{
					mouseDisplayer.HideTips();
					this._hasShowTips = false;
				}
				else
				{
					bool flag3 = mouseDisplayer.Showing && !this._hasShowTips;
					if (flag3)
					{
						mouseDisplayer.ShowTips();
						this._hasShowTips = true;
					}
				}
			}
		}

		// Token: 0x06005BFB RID: 23547 RVA: 0x002AACE0 File Offset: 0x002A8EE0
		public void Set(ESettingSubCategory subCategory, LanguageKey displayName, List<ISettingItemInfo> infos, Dictionary<SettingUIType, SettingItemBase> prefabs)
		{
			this._subCategory = subCategory;
			this._infos = infos;
			base.gameObject.name = string.Format("Group_{0}", subCategory);
			this.title.text = displayName.Tr();
			this.UpdateResetButtonVisibility();
			bool flag = infos == null || infos.Count == 0;
			if (!flag)
			{
				this._keyToItemMap.Clear();
				this._keyToInfoMap.Clear();
				CommonUtils.PrepareEnoughChildren(this.settingItemFrameTemp.parent, this.settingItemFrameTemp.gameObject, infos.Count, null);
				this._items.Clear();
				for (int i = 0; i < infos.Count; i++)
				{
					SettingGroupItem.<>c__DisplayClass15_0 CS$<>8__locals1 = new SettingGroupItem.<>c__DisplayClass15_0();
					CS$<>8__locals1.<>4__this = this;
					CS$<>8__locals1.info = infos[i];
					SettingItemBase prefab = this.GetPrefab(CS$<>8__locals1.info.Attribute.UIType, prefabs);
					bool flag2 = prefab == null;
					if (flag2)
					{
						Debug.LogWarning(string.Format("[SettingGroupItem] Prefab not found for UIType: {0}", CS$<>8__locals1.info.Attribute.UIType));
					}
					else
					{
						Transform parent = this.settingItemFrameTemp.parent.GetChild(i);
						SettingItemBase item = (parent.childCount > 1) ? parent.GetChild(1).gameObject.GetComponent<SettingItemBase>() : Object.Instantiate<SettingItemBase>(prefab, parent);
						item.gameObject.SetActive(true);
						item.gameObject.name = CS$<>8__locals1.info.Attribute.LanguageKey.ToString();
						item.Initialize(CS$<>8__locals1.info);
						this._items.Add(item);
						bool flag3 = CS$<>8__locals1.info.Attribute.Key > ESettingKey.None;
						if (flag3)
						{
							this._keyToItemMap[CS$<>8__locals1.info.Attribute.Key] = item;
							this._keyToInfoMap[CS$<>8__locals1.info.Attribute.Key] = CS$<>8__locals1.info;
						}
						item.OnChanged -= CS$<>8__locals1.<Set>g__OnChanged|2;
						item.OnChanged += CS$<>8__locals1.<Set>g__OnChanged|2;
						CS$<>8__locals1.frame = parent.GetComponent<CImage>();
						CS$<>8__locals1.frame.SetAlpha(0f);
						PointerTrigger pointerTrigger = parent.GetComponent<PointerTrigger>();
						pointerTrigger.EnterEvent.RemoveAllListeners();
						pointerTrigger.EnterEvent.AddListener(delegate()
						{
							CS$<>8__locals1.frame.SetAlpha(1f);
							ViewSystemSetting.RefreshRightTips(CS$<>8__locals1.info.Attribute.LanguageKey, CS$<>8__locals1.info.Attribute.TipLanguageKey, CS$<>8__locals1.info.Attribute);
						});
						pointerTrigger.ExitEvent.RemoveAllListeners();
						pointerTrigger.ExitEvent.AddListener(delegate()
						{
							CS$<>8__locals1.frame.SetAlpha(0f);
						});
					}
				}
				this.UpdateAllDependencyStates();
			}
		}

		// Token: 0x06005BFC RID: 23548 RVA: 0x002AAFB4 File Offset: 0x002A91B4
		private void OnSettingItemValueChanged(ESettingKey changedKey)
		{
			bool flag = changedKey > ESettingKey.None;
			if (flag)
			{
				this.UpdateDependentItems(changedKey);
			}
		}

		// Token: 0x06005BFD RID: 23549 RVA: 0x002AAFD4 File Offset: 0x002A91D4
		private void UpdateAllDependencyStates()
		{
			int i = 0;
			while (i < this._items.Count && i < this._infos.Count)
			{
				this.UpdateItemInteractableState(this._items[i], this._infos[i]);
				i++;
			}
		}

		// Token: 0x06005BFE RID: 23550 RVA: 0x002AB030 File Offset: 0x002A9230
		private void UpdateDependentItems(ESettingKey dependencyKey)
		{
			int i = 0;
			while (i < this._items.Count && i < this._infos.Count)
			{
				ISettingItemInfo info = this._infos[i];
				bool flag = info.Attribute.DependsOn == dependencyKey;
				if (flag)
				{
					this.UpdateItemInteractableState(this._items[i], info);
				}
				i++;
			}
		}

		// Token: 0x06005BFF RID: 23551 RVA: 0x002AB0A0 File Offset: 0x002A92A0
		private void UpdateItemInteractableState(SettingItemBase item, ISettingItemInfo info)
		{
			ESettingKey dependsOn = info.Attribute.DependsOn;
			bool flag = dependsOn == ESettingKey.None;
			if (flag)
			{
				item.SetInteractable(true);
			}
			else
			{
				bool dependencyValue = this.GetDependencyValue(dependsOn);
				bool flag2 = info.Attribute.Key == ESettingKey.SaveMoveTarget;
				if (flag2)
				{
					item.SetInteractable(!dependencyValue);
				}
				else
				{
					item.SetInteractable(dependencyValue);
				}
			}
		}

		// Token: 0x06005C00 RID: 23552 RVA: 0x002AB100 File Offset: 0x002A9300
		private bool GetDependencyValue(ESettingKey key)
		{
			ISettingItemInfo info;
			bool flag = this._keyToInfoMap.TryGetValue(key, out info);
			bool result;
			if (flag)
			{
				result = info.GetValueAsBool();
			}
			else
			{
				result = this.GetDependencyValueFromGlobalSettings(key);
			}
			return result;
		}

		// Token: 0x06005C01 RID: 23553 RVA: 0x002AB138 File Offset: 0x002A9338
		private bool GetDependencyValueFromGlobalSettings(ESettingKey key)
		{
			GlobalSettings settings = SingletonObject.getInstance<GlobalSettings>();
			AiOptions aiOptions = SystemSettingMapping.AiOptionsRef;
			switch (key)
			{
			case ESettingKey.EnableAutoTriggerNormalMapPickup:
				return settings.EnableAutoTriggerNormalMapPickup;
			case ESettingKey.AutoAttack:
				return aiOptions != null && aiOptions.AutoAttack;
			case ESettingKey.AutoMove:
				return aiOptions != null && aiOptions.AutoMove;
			case ESettingKey.AutoHealInjury:
				return ((aiOptions != null) ? aiOptions.AutoUseOtherAction : null) != null && aiOptions.AutoUseOtherAction.Length != 0 && aiOptions.AutoUseOtherAction[0];
			case ESettingKey.AutoHealPoison:
				return ((aiOptions != null) ? aiOptions.AutoUseOtherAction : null) != null && aiOptions.AutoUseOtherAction.Length > 1 && aiOptions.AutoUseOtherAction[1];
			case ESettingKey.AutoFlee:
				return ((aiOptions != null) ? aiOptions.AutoUseOtherAction : null) != null && aiOptions.AutoUseOtherAction.Length > 2 && aiOptions.AutoUseOtherAction[2];
			case ESettingKey.AutoCastAttack:
				return ((aiOptions != null) ? aiOptions.AutoCastSkill : null) != null && aiOptions.AutoCastSkill.Length != 0 && aiOptions.AutoCastSkill[0];
			case ESettingKey.AutoCastAgile:
				return ((aiOptions != null) ? aiOptions.AutoCastSkill : null) != null && aiOptions.AutoCastSkill.Length > 1 && aiOptions.AutoCastSkill[1];
			case ESettingKey.AutoCastDefense:
				return ((aiOptions != null) ? aiOptions.AutoCastSkill : null) != null && aiOptions.AutoCastSkill.Length > 2 && aiOptions.AutoCastSkill[2];
			}
			return true;
		}

		// Token: 0x06005C02 RID: 23554 RVA: 0x002AB2BC File Offset: 0x002A94BC
		private SettingItemBase GetPrefab(SettingUIType uiType, Dictionary<SettingUIType, SettingItemBase> prefabs)
		{
			SettingItemBase prefab;
			return prefabs.TryGetValue(uiType, out prefab) ? prefab : null;
		}

		// Token: 0x06005C03 RID: 23555 RVA: 0x002AB2E0 File Offset: 0x002A94E0
		private void UpdateResetButtonVisibility()
		{
			bool flag = this.resetButton == null;
			if (!flag)
			{
				this.resetButton.gameObject.SetActive(ViewSystemSetting.CanResetCategory(this._subCategory));
				this.resetButton.interactable = (this._subCategory != ESettingSubCategory.RegionStory || GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame);
			}
		}

		// Token: 0x06005C04 RID: 23556 RVA: 0x002AB341 File Offset: 0x002A9541
		private void OnResetButtonClick()
		{
			ViewSystemSetting.ResetCategory(this._subCategory, true, this.title.text, delegate
			{
				this.RefreshItems();
			});
		}

		// Token: 0x06005C05 RID: 23557 RVA: 0x002AB368 File Offset: 0x002A9568
		public void RefreshItems()
		{
			int i = 0;
			while (i < this._items.Count && i < this._infos.Count)
			{
				this._infos[i].RefreshValueTo(this._items[i]);
				i++;
			}
			this.UpdateAllDependencyStates();
		}

		// Token: 0x06005C06 RID: 23558 RVA: 0x002AB3C8 File Offset: 0x002A95C8
		public void SetGroupTitle(bool isShow, string groupTitle)
		{
			TextMeshProUGUI textMeshProUGUI = this.groupTitleLabel;
			if (textMeshProUGUI != null)
			{
				textMeshProUGUI.transform.parent.gameObject.SetActive(isShow);
			}
			bool flag = isShow && this.groupTitleLabel;
			if (flag)
			{
				this.groupTitleLabel.text = groupTitle;
			}
		}

		// Token: 0x04003FB5 RID: 16309
		[SerializeField]
		private TextMeshProUGUI title;

		// Token: 0x04003FB6 RID: 16310
		[SerializeField]
		private CButton resetButton;

		// Token: 0x04003FB7 RID: 16311
		[SerializeField]
		private RectTransform settingItemFrameTemp;

		// Token: 0x04003FB8 RID: 16312
		[SerializeField]
		private TextMeshProUGUI groupTitleLabel;

		// Token: 0x04003FB9 RID: 16313
		private readonly List<SettingItemBase> _items = new List<SettingItemBase>();

		// Token: 0x04003FBA RID: 16314
		private List<ISettingItemInfo> _infos;

		// Token: 0x04003FBB RID: 16315
		private ESettingSubCategory _subCategory;

		// Token: 0x04003FBC RID: 16316
		private readonly Dictionary<ESettingKey, SettingItemBase> _keyToItemMap = new Dictionary<ESettingKey, SettingItemBase>();

		// Token: 0x04003FBD RID: 16317
		private readonly Dictionary<ESettingKey, ISettingItemInfo> _keyToInfoMap = new Dictionary<ESettingKey, ISettingItemInfo>();

		// Token: 0x04003FBE RID: 16318
		private bool _hasShowTips;
	}
}
