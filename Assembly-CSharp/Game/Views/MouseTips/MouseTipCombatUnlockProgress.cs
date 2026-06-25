using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using Game.Views.MouseTips.CombatBanned;
using GameData.Domains.CombatSkill;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x0200084D RID: 2125
	public class MouseTipCombatUnlockProgress : MouseTipBase
	{
		// Token: 0x17000C69 RID: 3177
		// (get) Token: 0x06006730 RID: 26416 RVA: 0x002F0E0E File Offset: 0x002EF00E
		protected override bool CanStick
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000C6A RID: 3178
		// (get) Token: 0x06006731 RID: 26417 RVA: 0x002F0E11 File Offset: 0x002EF011
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x06006732 RID: 26418 RVA: 0x002F0E18 File Offset: 0x002EF018
		protected override void Init(ArgumentBox argsBox)
		{
			this._usefulSkillList.Clear();
			int unlockPrepareValue;
			argsBox.Get("UnlockPrepareValue", out unlockPrepareValue);
			argsBox.Get("ItemSubType", out this._itemSubType);
			int charId;
			argsBox.Get("CharacterId", out charId);
			bool hasCombatSkill = false;
			this._combatSkillReverseDict.Clear();
			foreach (KeyValuePair<CombatSkillKey, CombatSubProcessorSkill> kv in this.Model.ProcessorSkills)
			{
				bool flag = kv.Key.CharId != charId;
				if (!flag)
				{
					hasCombatSkill = true;
					this._combatSkillReverseDict[kv.Key.SkillTemplateId] = kv.Value.Direction;
				}
			}
			this.SetProgress(unlockPrepareValue);
			bool flag2 = hasCombatSkill;
			if (flag2)
			{
				this.CalcUsefulUnlockCombatSkill();
				UIElement element = this.Element;
				element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
			}
		}

		// Token: 0x06006733 RID: 26419 RVA: 0x002F0F30 File Offset: 0x002EF130
		public override void Refresh(ArgumentBox argBox)
		{
			int unlockPrepareValue;
			argBox.Get("UnlockPrepareValue", out unlockPrepareValue);
			this.SetProgress(unlockPrepareValue);
		}

		// Token: 0x06006734 RID: 26420 RVA: 0x002F0F54 File Offset: 0x002EF154
		private void OnListenerIdReady()
		{
			this.ProcessDisplayData();
		}

		// Token: 0x06006735 RID: 26421 RVA: 0x002F0F5E File Offset: 0x002EF15E
		private void SetProgress(int unlockPrepareValue)
		{
			this.progress.text = this.GetString((float)unlockPrepareValue);
		}

		// Token: 0x06006736 RID: 26422 RVA: 0x002F0F78 File Offset: 0x002EF178
		private void CalcUsefulUnlockCombatSkill()
		{
			int maxUnlockValue = 0;
			foreach (KeyValuePair<short, sbyte> pair in this._combatSkillReverseDict)
			{
				short templateId = pair.Key;
				sbyte direct = pair.Value;
				bool flag = direct < 0;
				if (!flag)
				{
					CombatSkillItem combatSkillItem = CombatSkill.Instance[templateId];
					SpecialEffectItem specialEffectItem = SpecialEffect.Instance[(direct == 0) ? combatSkillItem.DirectEffectID : combatSkillItem.ReverseEffectID];
					bool flag2 = specialEffectItem.AddUnlockValueItemSubType == this._itemSubType;
					if (flag2)
					{
						this._usefulSkillList.Add(specialEffectItem.TemplateId);
						bool flag3 = specialEffectItem.AddUnlockValue > maxUnlockValue;
						if (flag3)
						{
							maxUnlockValue = specialEffectItem.AddUnlockValue;
							this._maxUsefulSpecialEffectTemplateId = specialEffectItem.TemplateId;
						}
					}
				}
			}
		}

		// Token: 0x06006737 RID: 26423 RVA: 0x002F1070 File Offset: 0x002EF270
		private void ProcessDisplayData()
		{
			for (int i = 0; i < this._usefulSkillList.Count; i++)
			{
				bool flag = i >= this.holder.childCount;
				if (flag)
				{
					Object.Instantiate<GameObject>(this.template, this.holder);
				}
				this.FillData(i);
			}
			for (int j = this._usefulSkillList.Count; j < this.holder.childCount; j++)
			{
				this.holder.GetChild(j).gameObject.SetActive(false);
			}
			this.Element.ShowAfterRefresh();
		}

		// Token: 0x06006738 RID: 26424 RVA: 0x002F1114 File Offset: 0x002EF314
		private void FillData(int index)
		{
			CombatBannedItem obj = this.holder.GetChild(index).GetComponent<CombatBannedItem>();
			short specialEffectTemplateId = this._usefulSkillList[index];
			SpecialEffectItem specialEffectItem = SpecialEffect.Instance[specialEffectTemplateId];
			CombatSkillItem config = CombatSkill.Instance[specialEffectItem.SkillTemplateId];
			obj.skillFrame.SetSprite(CombatBannedItem.FramePaths[(int)config.EquipType] + config.Grade.ToString(), false, null);
			obj.skillIcon.SetSprite(config.Icon, false, null);
			obj.nameLabel.text = config.Name.SetColor(Colors.Instance.GradeColors[(int)config.Grade]);
			obj.timeLabel.text = this.GetString((float)(specialEffectItem.AddUnlockValue * 60));
			obj.GetComponent<DisableStyleRoot>().SetStyleEffect(specialEffectTemplateId != this._maxUsefulSpecialEffectTemplateId, false);
			obj.gameObject.SetActive(true);
		}

		// Token: 0x06006739 RID: 26425 RVA: 0x002F120C File Offset: 0x002EF40C
		private string GetString(float value)
		{
			return (value * 10000f / (float)GlobalConfig.Instance.UnlockAttackUnit / 100f).ToString("F2") + "%";
		}

		// Token: 0x040048BA RID: 18618
		[SerializeField]
		private TextMeshProUGUI progress;

		// Token: 0x040048BB RID: 18619
		[SerializeField]
		private Transform holder;

		// Token: 0x040048BC RID: 18620
		[SerializeField]
		private GameObject template;

		// Token: 0x040048BD RID: 18621
		private readonly List<short> _usefulSkillList = new List<short>();

		// Token: 0x040048BE RID: 18622
		private short _maxUsefulSpecialEffectTemplateId;

		// Token: 0x040048BF RID: 18623
		private short _maxValue;

		// Token: 0x040048C0 RID: 18624
		private readonly Dictionary<short, sbyte> _combatSkillReverseDict = new Dictionary<short, sbyte>();

		// Token: 0x040048C1 RID: 18625
		private short _itemSubType;

		// Token: 0x040048C2 RID: 18626
		private const int Factor = 60;
	}
}
