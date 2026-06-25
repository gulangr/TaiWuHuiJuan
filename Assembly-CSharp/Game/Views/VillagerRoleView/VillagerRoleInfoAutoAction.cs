using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Config;
using FrameWork.UISystem.UIElements;
using Game.Components.Switch;
using GameData.Domains.Extra;
using GameData.Domains.Taiwu;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.VillagerRoleView
{
	// Token: 0x02000738 RID: 1848
	public class VillagerRoleInfoAutoAction : MonoBehaviour
	{
		// Token: 0x0600595D RID: 22877 RVA: 0x00296FEA File Offset: 0x002951EA
		public void OnInit()
		{
			this._lastRole = -1;
			this.autoActionMaterialStorageTypeDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnStorageTypeToggleChange));
		}

		// Token: 0x0600595E RID: 22878 RVA: 0x00297014 File Offset: 0x00295214
		public void Refresh(VillagerRoleItem roleConfig, ViewVillagerRole parent)
		{
			this._config = roleConfig;
			this._parent = parent;
			this.InitMaterialStorageTypeToggleGroup(roleConfig);
			this._autoAcionCofigs.Clear();
			foreach (VillagerRoleAutoActionItem autoActionConfig in ((IEnumerable<VillagerRoleAutoActionItem>)VillagerRoleAutoAction.Instance))
			{
				bool flag = autoActionConfig.VillagerRole != this._config.TemplateId;
				if (!flag)
				{
					this._autoAcionCofigs.Add(autoActionConfig);
				}
			}
			this._autoAcionCofigs.Sort(new Comparison<VillagerRoleAutoActionItem>(this.CompareAutoActionConfig));
			bool flag2 = this._lastRole != roleConfig.TemplateId;
			if (flag2)
			{
				this.RefreshAll(this._autoAcionCofigs);
			}
			else
			{
				this.RefreshSwitchesOnly(this._autoAcionCofigs);
			}
			this._lastRole = roleConfig.TemplateId;
		}

		// Token: 0x0600595F RID: 22879 RVA: 0x00297104 File Offset: 0x00295304
		private void RefreshSwitchesOnly(List<VillagerRoleAutoActionItem> autoActionConfigs)
		{
			for (int i = 0; i < autoActionConfigs.Count; i++)
			{
				VillagerRoleAutoActionItem autoActionConfig = autoActionConfigs[i];
				Refers refers = this.autoActionRoot.GetChild(i).GetComponent<Refers>();
				this.RefreshAutoActionItemSwitch(refers, autoActionConfig);
			}
		}

		// Token: 0x06005960 RID: 22880 RVA: 0x0029714C File Offset: 0x0029534C
		private void RefreshAll(List<VillagerRoleAutoActionItem> autoActionConfigs)
		{
			this._chickenBgCheckItems.Clear();
			CommonUtils.PrepareEnoughChildren(this.autoActionRoot, this.autoActionItemTemplate.gameObject, autoActionConfigs.Count, null);
			for (int i = 0; i < autoActionConfigs.Count; i++)
			{
				VillagerRoleAutoActionItem autoActionConfig = autoActionConfigs[i];
				Refers refers = this.autoActionRoot.GetChild(i).GetComponent<Refers>();
				this.RefreshAutoActionItem(refers, autoActionConfig, i);
			}
		}

		// Token: 0x06005961 RID: 22881 RVA: 0x002971CC File Offset: 0x002953CC
		private int CompareAutoActionConfig(VillagerRoleAutoActionItem x, VillagerRoleAutoActionItem y)
		{
			int unlockCompare = x.UnlockByChicken.CompareTo(y.UnlockByChicken);
			bool flag = unlockCompare != 0;
			int result;
			if (flag)
			{
				result = unlockCompare;
			}
			else
			{
				result = x.TemplateId.CompareTo(y.TemplateId);
			}
			return result;
		}

		// Token: 0x06005962 RID: 22882 RVA: 0x00297210 File Offset: 0x00295410
		private void RefreshAutoActionItem(Refers refers, VillagerRoleAutoActionItem autoActionConfig, int i)
		{
			bool unlockByChicken = autoActionConfig.UnlockByChicken;
			bool isExtraEffectUnlocked = this._parent.IsExtraEffectUnlocked((int)this._config.TemplateId);
			bool useChickenStyle = unlockByChicken && isExtraEffectUnlocked;
			bool fulongSpecialInteractUnlocked = this._parent.IsFulongSpecialInteractOpen;
			TextMeshProUGUI descName = refers.CGet<TextMeshProUGUI>("Name");
			TextMeshProUGUI desc2 = refers.CGet<TextMeshProUGUI>("Desc2");
			CImage picture = refers.CGet<CImage>("Pic");
			GameObject hint = refers.CGet<GameObject>("Hint");
			picture.SetSprite(autoActionConfig.Illustration, false, delegate
			{
				picture.gameObject.SetActive(true);
			});
			hint.SetActive(false);
			GameObject actionToggleGo = refers.CGet<GameObject>("ActionToggle");
			SwitchToggleSmall actionToggle = actionToggleGo.GetComponent<SwitchToggleSmall>();
			actionToggle.gameObject.SetActive(!unlockByChicken || isExtraEffectUnlocked);
			CanvasGroup descCanvasGroup = desc2.GetComponent<CanvasGroup>();
			descName.text = autoActionConfig.DescName.ColorReplace();
			desc2.text = autoActionConfig.DescContent.ColorReplace();
			descCanvasGroup.alpha = 0f;
			actionToggle.isOn = this._parent.IsAutoActionEnabled(this._config.TemplateId, autoActionConfig.TemplateId);
			actionToggle.onValueChanged.RemoveAllListeners();
			actionToggle.onValueChanged.AddListener(delegate(bool isOn)
			{
				this.OnActionToggleChanged(actionToggle, isOn, autoActionConfig);
			});
			refers.gameObject.SetActive(!unlockByChicken || fulongSpecialInteractUnlocked);
			bool needGray = unlockByChicken && !isExtraEffectUnlocked;
			DisableStyleRoot disableStyleRoot = refers.GetComponent<DisableStyleRoot>();
			disableStyleRoot.SetStyleEffect(needGray, false);
			bool flag = needGray;
			if (flag)
			{
				string originalText = desc2.text;
				Regex regex = new Regex("<color=[^>]+>|</color>");
				desc2.text = regex.Replace(originalText, "");
				desc2.color = disableStyleRoot.EffectTextColor;
				desc2.overrideColorTags = false;
			}
			else
			{
				desc2.color = Colors.Instance["lightgrey"];
			}
			SingletonObject.getInstance<YieldHelper>().StartCoroutine(this.DelayShowDesc(descCanvasGroup));
			desc2.GetComponent<TMPTextSpriteHelper>().Parse();
		}

		// Token: 0x06005963 RID: 22883 RVA: 0x00297458 File Offset: 0x00295658
		private void RefreshAutoActionItemSwitch(Refers refers, VillagerRoleAutoActionItem autoActionConfig)
		{
			GameObject actionToggleGo = refers.CGet<GameObject>("ActionToggle");
			SwitchToggleSmall actionToggle = actionToggleGo.GetComponent<SwitchToggleSmall>();
			actionToggle.isOn = this._parent.IsAutoActionEnabled(this._config.TemplateId, autoActionConfig.TemplateId);
		}

		// Token: 0x06005964 RID: 22884 RVA: 0x0029749C File Offset: 0x0029569C
		private IEnumerator DelayShowDesc(CanvasGroup descCanvasGroup)
		{
			yield return null;
			yield return null;
			yield return null;
			descCanvasGroup.alpha = 1f;
			yield break;
		}

		// Token: 0x06005965 RID: 22885 RVA: 0x002974B2 File Offset: 0x002956B2
		private void OnActionToggleChanged(SwitchToggleSmall toggle, bool isOn, VillagerRoleAutoActionItem autoActionConfig)
		{
			this._parent.SetAutoActionEnabled(this._config.TemplateId, autoActionConfig.TemplateId, isOn);
		}

		// Token: 0x06005966 RID: 22886 RVA: 0x002974D4 File Offset: 0x002956D4
		private void InitMaterialStorageTypeToggleGroup(VillagerRoleItem roleConfig)
		{
			this.autoActionMaterialStorageTypeDropdown.transform.parent.gameObject.SetActive(roleConfig.TemplateId == 0);
			ItemSourceType target = (ItemSourceType)this._parent.FarmerAutoCollectStorageType;
			bool flag = target != ItemSourceType.Warehouse && target != ItemSourceType.Treasury && target != ItemSourceType.Stock;
			if (flag)
			{
				target = ItemSourceType.Warehouse;
			}
			this.autoActionMaterialStorageTypeDropdown.ClearOptions();
			this.autoActionMaterialStorageTypeDropdown.AddOptions(new List<CDropdown.OptionData>
			{
				new CDropdown.OptionData(LanguageKey.LK_Warehouse.Tr()),
				new CDropdown.OptionData(LanguageKey.LK_Treasury.Tr()),
				new CDropdown.OptionData(LanguageKey.LK_StockStorage.Tr())
			});
			foreach (KeyValuePair<ItemSourceType, int> keyValuePair in this._itemSourceTypeToFarmerAutoCollectStorageType)
			{
				ItemSourceType itemSourceType;
				int num;
				keyValuePair.Deconstruct(out itemSourceType, out num);
				ItemSourceType type = itemSourceType;
				int index = num;
				bool flag2 = target == type;
				if (flag2)
				{
					this.autoActionMaterialStorageTypeDropdown.SetValueWithoutNotify(index);
					break;
				}
			}
		}

		// Token: 0x06005967 RID: 22887 RVA: 0x00297600 File Offset: 0x00295800
		private void OnStorageTypeToggleChange(int arg0)
		{
			foreach (KeyValuePair<ItemSourceType, int> keyValuePair in this._itemSourceTypeToFarmerAutoCollectStorageType)
			{
				ItemSourceType itemSourceType;
				int num;
				keyValuePair.Deconstruct(out itemSourceType, out num);
				ItemSourceType type = itemSourceType;
				int index = num;
				bool flag = index == arg0;
				if (flag)
				{
					ExtraDomainMethod.Call.SetFarmerAutoCollectStorageType((sbyte)type);
					break;
				}
			}
		}

		// Token: 0x06005968 RID: 22888 RVA: 0x00297678 File Offset: 0x00295878
		public void RefreshByChickUnlock()
		{
			bool flag = this._config == null || this._parent == null;
			if (!flag)
			{
				this.RefreshAll(this._autoAcionCofigs);
			}
		}

		// Token: 0x04003D77 RID: 15735
		private VillagerRoleItem _config;

		// Token: 0x04003D78 RID: 15736
		private ViewVillagerRole _parent;

		// Token: 0x04003D79 RID: 15737
		private Dictionary<ItemSourceType, int> _itemSourceTypeToFarmerAutoCollectStorageType = new Dictionary<ItemSourceType, int>
		{
			{
				ItemSourceType.Warehouse,
				0
			},
			{
				ItemSourceType.Treasury,
				1
			},
			{
				ItemSourceType.Stock,
				2
			}
		};

		// Token: 0x04003D7A RID: 15738
		private readonly List<VillagerRoleInfoAutoAction.ChickenBgCheckItem> _chickenBgCheckItems = new List<VillagerRoleInfoAutoAction.ChickenBgCheckItem>();

		// Token: 0x04003D7B RID: 15739
		private readonly List<VillagerRoleAutoActionItem> _autoAcionCofigs = new List<VillagerRoleAutoActionItem>();

		// Token: 0x04003D7C RID: 15740
		private short _lastRole = -1;

		// Token: 0x04003D7D RID: 15741
		[SerializeField]
		private RectTransform autoActionRoot;

		// Token: 0x04003D7E RID: 15742
		[SerializeField]
		private Refers autoActionItemTemplate;

		// Token: 0x04003D7F RID: 15743
		[SerializeField]
		private CDropdown autoActionMaterialStorageTypeDropdown;

		// Token: 0x02001C08 RID: 7176
		private struct ChickenBgCheckItem
		{
			// Token: 0x0400BF59 RID: 48985
			public RectTransform ToCheckRect;

			// Token: 0x0400BF5A RID: 48986
			public CImage ChickenBg;
		}
	}
}
