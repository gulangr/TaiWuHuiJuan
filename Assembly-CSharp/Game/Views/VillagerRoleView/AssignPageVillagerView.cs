using System;
using System.Collections.Generic;
using Config;
using DisplayConfig;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Character.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.VillagerRoleView
{
	// Token: 0x02000733 RID: 1843
	public class AssignPageVillagerView : MonoBehaviour
	{
		// Token: 0x06005882 RID: 22658 RVA: 0x00291600 File Offset: 0x0028F800
		private void Awake()
		{
			this.btnRemove.ClearAndAddListener(delegate
			{
				Action onClickRemove = this._onClickRemove;
				if (onClickRemove != null)
				{
					onClickRemove();
				}
			});
			this.btnMain.ClearAndAddListener(delegate
			{
				Action onClickMain = this._onClickMain;
				if (onClickMain != null)
				{
					onClickMain();
				}
			});
			this._dropDownList.Clear();
			this._dropDownList.Add(LocalStringManager.Get(LanguageKey.LK_VillagerRole_Merchant_Random));
			foreach (MerchantTypeItem item in ((IEnumerable<MerchantTypeItem>)MerchantType.Instance))
			{
				bool flag = item.TemplateId >= 7;
				if (!flag)
				{
					this._dropDownList.Add(item.Name);
				}
			}
			this.dropdown.onValueChanged.RemoveAllListeners();
			this.dropdown.ClearOptions();
			this.dropdown.AddOptions(this._dropDownList);
		}

		// Token: 0x06005883 RID: 22659 RVA: 0x002916F0 File Offset: 0x0028F8F0
		public void Set(VillagerCharDisplayData data, Action onClickMain, Action onClickRemove, Action onChangeMerchant, bool isSelected, bool currentModeInteractable)
		{
			this._onClickMain = onClickMain;
			this._onClickRemove = onClickRemove;
			this.avatar.Refresh(data.AvatarRelatedData, data.CharacterTemplateId);
			this.txtName.text = NameCenter.GetMonasticTitleOrDisplayName(ref data.NameData, false, false);
			this.mouseTip.enabled = true;
			this.mouseTip.Type = TipType.CharacterOnMapBlock;
			TooltipInvoker tooltipInvoker = this.mouseTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			this.mouseTip.RuntimeParam.Set("CharId", data.CharacterId);
			ValueTuple<sbyte, short> villagerRolePersonalityTypeAndValue = ViewVillagerRole.GetVillagerRolePersonalityTypeAndValue(data.RoleTemplateId, data);
			sbyte personalityType = villagerRolePersonalityTypeAndValue.Item1;
			short value = villagerRolePersonalityTypeAndValue.Item2;
			string iconName = "ui9_icon_personality_big_" + personalityType.ToString();
			this.prop1.SetActive(true);
			this.iconProp1.SetSprite(iconName, false, null);
			this.txtProp1Name.text = Personality.Instance[(int)personalityType].Name;
			this.txtProp1Value.text = value.ToString();
			ValueTuple<sbyte, short, bool> villagerRoleAttainmentTypeAndValue = ViewVillagerRole.GetVillagerRoleAttainmentTypeAndValue(data.RoleTemplateId, data);
			sbyte skillType = villagerRoleAttainmentTypeAndValue.Item1;
			short skillValue = villagerRoleAttainmentTypeAndValue.Item2;
			bool isLifeSkill = villagerRoleAttainmentTypeAndValue.Item3;
			string iconName2 = (isLifeSkill ? "ui9_icon_craftsmanship_big_2_" : "ui9_back_attainments_combat_") + skillType.ToString();
			this.prop2.SetActive(true);
			this.iconProp2.SetSprite(iconName2, false, null);
			this.txtProp2Name.text = (isLifeSkill ? LifeSkillType.Instance[skillType].Name : CombatSkillType.Instance[skillType].Name);
			this.txtProp2Value.text = skillValue.ToString();
			this.selectedObj.SetActive(isSelected && currentModeInteractable);
			this.btnRemove.gameObject.SetActive(currentModeInteractable);
			this.btnMain.interactable = currentModeInteractable;
			bool flag = data.RoleTemplateId != 3;
			if (flag)
			{
				this.dropdown.gameObject.SetActive(false);
			}
			else
			{
				this.dropdown.gameObject.SetActive(true);
				UnityAction<int> <>9__1;
				TaiwuDomainMethod.AsyncCall.GetMerchantType(null, data.CharacterId, delegate(int offset, RawDataPool dataPool)
				{
					sbyte merchantData = 0;
					Serializer.Deserialize(dataPool, offset, ref merchantData);
					this.dropdown.value = (int)((merchantData == 7) ? 0 : (merchantData + 1));
					this.dropdown.onValueChanged.RemoveAllListeners();
					UnityEvent<int> onValueChanged = this.dropdown.onValueChanged;
					UnityAction<int> call;
					if ((call = <>9__1) == null)
					{
						call = (<>9__1 = delegate(int index)
						{
							sbyte setValue = (sbyte)((index == 0) ? 7 : (index - 1));
							TaiwuDomainMethod.Call.SetMerchantType(data.CharacterId, setValue);
							Action onChangeMerchant2 = onChangeMerchant;
							if (onChangeMerchant2 != null)
							{
								onChangeMerchant2();
							}
						});
					}
					onValueChanged.AddListener(call);
				});
			}
		}

		// Token: 0x04003CDD RID: 15581
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04003CDE RID: 15582
		[SerializeField]
		private CButton btnRemove;

		// Token: 0x04003CDF RID: 15583
		[SerializeField]
		private CButton btnMain;

		// Token: 0x04003CE0 RID: 15584
		[SerializeField]
		private TextMeshProUGUI txtName;

		// Token: 0x04003CE1 RID: 15585
		[SerializeField]
		private GameObject selectedObj;

		// Token: 0x04003CE2 RID: 15586
		[SerializeField]
		private TooltipInvoker mouseTip;

		// Token: 0x04003CE3 RID: 15587
		[Header("属性")]
		[SerializeField]
		private GameObject prop1;

		// Token: 0x04003CE4 RID: 15588
		[SerializeField]
		private CImage iconProp1;

		// Token: 0x04003CE5 RID: 15589
		[SerializeField]
		private TextMeshProUGUI txtProp1Name;

		// Token: 0x04003CE6 RID: 15590
		[SerializeField]
		private TextMeshProUGUI txtProp1Value;

		// Token: 0x04003CE7 RID: 15591
		[SerializeField]
		private GameObject prop2;

		// Token: 0x04003CE8 RID: 15592
		[SerializeField]
		private CImage iconProp2;

		// Token: 0x04003CE9 RID: 15593
		[SerializeField]
		private TextMeshProUGUI txtProp2Name;

		// Token: 0x04003CEA RID: 15594
		[SerializeField]
		private TextMeshProUGUI txtProp2Value;

		// Token: 0x04003CEB RID: 15595
		[SerializeField]
		private CDropdown dropdown;

		// Token: 0x04003CEC RID: 15596
		private Action _onClickRemove;

		// Token: 0x04003CED RID: 15597
		private Action _onClickMain;

		// Token: 0x04003CEE RID: 15598
		private readonly List<string> _dropDownList = new List<string>();
	}
}
