using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using Game.Components.Character;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Combat;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000AF1 RID: 2801
	public class CombatResultStatusPage : MonoBehaviour
	{
		// Token: 0x060089AB RID: 35243 RVA: 0x003FBDD2 File Offset: 0x003F9FD2
		public void Init(CombatResultDisplayData displayData)
		{
			this._displayData = displayData;
			this.RefreshInjuryDisplay();
			this.RefreshProficiencies();
		}

		// Token: 0x060089AC RID: 35244 RVA: 0x003FBDEC File Offset: 0x003F9FEC
		private void RefreshInjuryDisplay()
		{
			bool flag = this.beforeInjury != null;
			if (flag)
			{
				CharacterInjuryDisplayData data = this.CreateDisplayDataFromSnapshot(this._displayData.SnapshotBeforeCombat);
				this.beforeInjury.Set(data, false);
			}
			bool flag2 = this.afterInjury != null;
			if (flag2)
			{
				CharacterInjuryDisplayData data2 = this.CreateDisplayDataFromSnapshot(this._displayData.SnapshotAfterCombat);
				this.afterInjury.Set(data2, false);
			}
		}

		// Token: 0x060089AD RID: 35245 RVA: 0x003FBE60 File Offset: 0x003FA060
		private CharacterInjuryDisplayData CreateDisplayDataFromSnapshot(CombatResultSnapshot snapshot)
		{
			CharacterInjuryDisplayData data = new CharacterInjuryDisplayData();
			data.Injuries = snapshot.Injuries;
			data.Poisons = snapshot.Poisons;
			data.PoisonResists = snapshot.PoisonResists;
			data.DisorderOfQi = snapshot.DisorderOfQi;
			data.ChangeOfQiDisorder = snapshot.ChangeOfQiDisorder;
			data.Health = snapshot.Health;
			data.LeftMaxHealth = snapshot.LeftMaxHealth;
			data.HealthRecovery = snapshot.HealthRecovery;
			data.EatingItems = snapshot.EatingItemList;
			data.CanEatingMaxCount = snapshot.CanEatingMaxCount;
			data.EatingItemDisplayDataArray = new ItemDisplayData[9];
			for (int i = 0; i < data.EatingItemDisplayDataArray.Length; i++)
			{
				ItemKey itemKey = data.EatingItems.GetItem(i);
				bool flag = EatingItems.IsValid(itemKey);
				if (flag)
				{
					ItemDisplayData itemData = new ItemDisplayData();
					itemData.Key = itemKey;
					itemData.Amount = 0;
					data.EatingItemDisplayDataArray[i] = itemData;
				}
			}
			data.MainAttributeRecoveries = snapshot.MainAttribute;
			data.CurMainAttributes = default(MainAttributes);
			data.MaxMainAttributes = default(MainAttributes);
			data.IsImmune = new bool[6];
			data.IsBornImmune = new bool[6];
			data.AllBodyPartExists = new List<bool>();
			for (int j = 0; j < 7; j++)
			{
				data.AllBodyPartExists.Add(true);
			}
			data.CharacterId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			data.TemplateId = snapshot.TemplateId;
			return data;
		}

		// Token: 0x060089AE RID: 35246 RVA: 0x003FBFDC File Offset: 0x003FA1DC
		private void RefreshProficiencies()
		{
			Dictionary<short, int> delta = this._displayData.ChangedProficienciesDelta;
			int count = (delta != null) ? delta.Count : 0;
			this.proficiencyArea.SetActive(count > 0);
			bool flag = count == 0;
			if (!flag)
			{
				CommonUtils.PrepareEnoughChildren(this.proficiencyContainer, this.proficiencyItemPrefab, count, null);
				List<short> keys = delta.Keys.ToList<short>();
				for (int i = 0; i < count; i++)
				{
					Transform child = this.proficiencyContainer.GetChild(i);
					short skillId = keys[i];
					int value = delta[skillId];
					CombatSkillItem config = CombatSkill.Instance[skillId];
					Transform transform = child.Find("Icon");
					CImage icon = (transform != null) ? transform.GetComponent<CImage>() : null;
					Transform transform2 = child.Find("Name");
					TextMeshProUGUI nameLabel = (transform2 != null) ? transform2.GetComponent<TextMeshProUGUI>() : null;
					Transform transform3 = child.Find("Value");
					TextMeshProUGUI valueLabel = (transform3 != null) ? transform3.GetComponent<TextMeshProUGUI>() : null;
					bool flag2 = icon != null;
					if (flag2)
					{
						icon.SetSprite(string.Format("{0}{1}", "ui9_icon_attainments_small_0_", config.Type), false, null);
					}
					bool flag3 = nameLabel != null;
					if (flag3)
					{
						nameLabel.text = config.Name.SetGradeColor((int)config.Grade);
					}
					bool flag4 = valueLabel != null;
					if (flag4)
					{
						valueLabel.text = CommonUtils.GetAddReduceString(value);
					}
					child.gameObject.SetActive(true);
				}
				for (int j = count; j < this.proficiencyContainer.childCount; j++)
				{
					this.proficiencyContainer.GetChild(j).gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x04006991 RID: 27025
		[Header("伤病状态")]
		[SerializeField]
		private Injury beforeInjury;

		// Token: 0x04006992 RID: 27026
		[SerializeField]
		private Injury afterInjury;

		// Token: 0x04006993 RID: 27027
		[Header("实战发挥")]
		[SerializeField]
		private GameObject proficiencyArea;

		// Token: 0x04006994 RID: 27028
		[SerializeField]
		private Transform proficiencyContainer;

		// Token: 0x04006995 RID: 27029
		[SerializeField]
		private GameObject proficiencyItemPrefab;

		// Token: 0x04006996 RID: 27030
		private CombatResultDisplayData _displayData;
	}
}
