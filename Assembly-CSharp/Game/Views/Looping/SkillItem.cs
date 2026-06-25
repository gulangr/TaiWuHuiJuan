using System;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.CombatSkill;
using TMPro;
using UnityEngine;

namespace Game.Views.Looping
{
	// Token: 0x02000980 RID: 2432
	public class SkillItem : MonoBehaviour
	{
		// Token: 0x17000D2A RID: 3370
		// (get) Token: 0x060074B1 RID: 29873 RVA: 0x00365895 File Offset: 0x00363A95
		private ViewLooping Parent
		{
			get
			{
				return UIElement.Looping.UiBaseAs<ViewLooping>();
			}
		}

		// Token: 0x060074B2 RID: 29874 RVA: 0x003658A4 File Offset: 0x00363AA4
		private void Awake()
		{
			this.emptyBtn.ClearAndAddListener(delegate
			{
				this.Parent.SetSelectedReferenceSkillSlotIndex(this.Index);
			});
			this.skillBtn.ClearAndAddListener(delegate
			{
				this.Parent.SetSelectedReferenceSkillSlotIndex(this.Index);
			});
			this.deleteBtn.ClearAndAddListener(delegate
			{
				this.Parent.RemoveReferenceSkill(this.Index);
			});
			this.emptyPointerTrigger.EnterEvent.ResetListener(delegate()
			{
				this.back.sprite = this.backSprites[1];
			});
			this.emptyPointerTrigger.ExitEvent.ResetListener(delegate()
			{
				this.back.sprite = this.backSprites[0];
			});
			this.skillPointerTrigger.EnterEvent.ResetListener(delegate()
			{
				this.back.sprite = this.backSprites[1];
			});
			this.skillPointerTrigger.ExitEvent.ResetListener(delegate()
			{
				this.back.sprite = this.backSprites[0];
			});
		}

		// Token: 0x060074B3 RID: 29875 RVA: 0x00365970 File Offset: 0x00363B70
		public void Set(CombatSkillDisplayDataCharacterMenuListItem combatSkillDisplayData, bool isunLock, int requireValue, int bonusPercent, bool showSlotSelected)
		{
			this._combatSkillDisplayData = combatSkillDisplayData;
			bool flag = this.selImg != null;
			if (flag)
			{
				this.selImg.SetActive(showSlotSelected);
			}
			this.lockRoot.gameObject.SetActive(!isunLock);
			this.emptyRoot.gameObject.SetActive(isunLock && this._combatSkillDisplayData == null);
			this.skillRoot.gameObject.SetActive(isunLock && this._combatSkillDisplayData != null);
			this.requireValue.text = requireValue.ToString();
			bool flag2 = this._combatSkillDisplayData != null;
			if (flag2)
			{
				CombatSkillItem config = CombatSkill.Instance[combatSkillDisplayData.TemplateId];
				Color gradeColor = Colors.Instance.GradeColors[(int)config.Grade];
				this.skillName.text = config.Name.SetColor(gradeColor);
				this.icon.SetSprite(config.Icon, false, null);
				this.icon.SetColor(CommonUtils.GetFiveElementColor((int)config.FiveElements));
				this.frame.SetSprite(SkillItem.FramePaths[(int)config.EquipType] + config.Grade.ToString(), false, null);
				this.fiveElementsFrame.gameObject.SetActive(false);
				this.neiliBonus.transform.parent.gameObject.SetActive(bonusPercent > 0);
				this.neiliAllocationBonus.transform.parent.gameObject.SetActive(bonusPercent > 0);
				bool flag3 = bonusPercent > 0;
				if (flag3)
				{
					string text = string.Format("+{0}%", bonusPercent);
					this.neiliBonus.text = text;
					this.neiliAllocationBonus.text = text;
				}
				this.eventRate.text = string.Format("+{0}%", config.QiArtStrategyGenerateProbability);
				TooltipInvoker tip = this.frame.GetComponent<TooltipInvoker>();
				tip.Type = TipType.CombatSkill;
				tip.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("CombatSkillId", this._combatSkillDisplayData.TemplateId).Set("CharId", SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
				for (int i = 0; i < this.strategies.Length; i++)
				{
					bool flag4 = i >= config.PossibleQiArtStrategyList.Count;
					if (flag4)
					{
						this.strategies[i].gameObject.SetActive(false);
					}
					else
					{
						this.strategies[i].gameObject.SetActive(true);
						sbyte strategyTid = config.PossibleQiArtStrategyList[i];
						QiArtStrategyItem strategyConfig = QiArtStrategy.Instance[strategyTid];
						this.strategies[i].SetSprite(strategyConfig.Icon, false, null);
						TooltipInvoker strategyTip = this.strategies[i].GetComponent<TooltipInvoker>();
						strategyTip.Type = TipType.Simple;
						strategyTip.PresetParam[0] = strategyConfig.Name;
						strategyTip.PresetParam[1] = strategyConfig.Desc;
					}
				}
			}
		}

		// Token: 0x04005741 RID: 22337
		[SerializeField]
		private RectTransform lockRoot;

		// Token: 0x04005742 RID: 22338
		[SerializeField]
		private RectTransform emptyRoot;

		// Token: 0x04005743 RID: 22339
		[SerializeField]
		private RectTransform skillRoot;

		// Token: 0x04005744 RID: 22340
		[SerializeField]
		private TextMeshProUGUI requireValue;

		// Token: 0x04005745 RID: 22341
		[SerializeField]
		private CButton emptyBtn;

		// Token: 0x04005746 RID: 22342
		[SerializeField]
		private CButton skillBtn;

		// Token: 0x04005747 RID: 22343
		[SerializeField]
		private CButton deleteBtn;

		// Token: 0x04005748 RID: 22344
		[SerializeField]
		private CImage icon;

		// Token: 0x04005749 RID: 22345
		[SerializeField]
		private CImage frame;

		// Token: 0x0400574A RID: 22346
		[SerializeField]
		private GameObject selImg;

		// Token: 0x0400574B RID: 22347
		[SerializeField]
		private TextMeshProUGUI skillName;

		// Token: 0x0400574C RID: 22348
		[SerializeField]
		private TextMeshProUGUI neiliBonus;

		// Token: 0x0400574D RID: 22349
		[SerializeField]
		private TextMeshProUGUI neiliAllocationBonus;

		// Token: 0x0400574E RID: 22350
		[SerializeField]
		private TextMeshProUGUI eventRate;

		// Token: 0x0400574F RID: 22351
		[SerializeField]
		private CImage[] strategies;

		// Token: 0x04005750 RID: 22352
		[SerializeField]
		private CImage fiveElementsFrame;

		// Token: 0x04005751 RID: 22353
		[SerializeField]
		private PointerTrigger emptyPointerTrigger;

		// Token: 0x04005752 RID: 22354
		[SerializeField]
		private PointerTrigger skillPointerTrigger;

		// Token: 0x04005753 RID: 22355
		[SerializeField]
		private Sprite[] backSprites;

		// Token: 0x04005754 RID: 22356
		[SerializeField]
		private CImage back;

		// Token: 0x04005755 RID: 22357
		private CombatSkillDisplayDataCharacterMenuListItem _combatSkillDisplayData;

		// Token: 0x04005756 RID: 22358
		public int Index;

		// Token: 0x04005757 RID: 22359
		private static readonly string[] FramePaths = new string[]
		{
			"ui9_icon_combat_skill_type_neigong_",
			"ui9_icon_combat_skill_type_attack_",
			"ui9_icon_combat_skill_type_agile_",
			"ui9_icon_combat_skill_type_defense_",
			"ui9_icon_combat_skill_type_assist_"
		};
	}
}
