using System;
using System.Text;
using Config;
using FrameWork;
using Game.Views.CharacterMenu;
using Game.Views.MouseTips.Item.Common;
using GameData.Domains.Building;
using GameData.Domains.Combat;
using GameData.Domains.Item.Display;
using GameData.Domains.Story;
using GameData.Domains.Story.MainStory;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips.Item
{
	// Token: 0x020008A8 RID: 2216
	public class TooltipMisc : TooltipItemBase
	{
		// Token: 0x17000C94 RID: 3220
		// (get) Token: 0x06006A06 RID: 27142 RVA: 0x0030E810 File Offset: 0x0030CA10
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06006A07 RID: 27143 RVA: 0x0030E814 File Offset: 0x0030CA14
		protected override void Init(ArgumentBox argsBox)
		{
			argsBox.Get<ItemDisplayData>("ItemData", out this._itemData);
			argsBox.Get("TemplateDataOnly", out this._templateDataOnly);
			this._itemKey = this._itemData.RealKey;
			this.configData = Misc.Instance[this._itemKey.TemplateId];
			bool isSwordFragment = GameData.Domains.Combat.SharedConstValue.SwordFragment2BossId.ContainsKey(this._itemKey.TemplateId);
			this._xiangshuAvatarId = ((!this._templateDataOnly && isSwordFragment) ? Convert.ToSByte((int)(this._itemKey.TemplateId - 229)) : -1);
			this._canCastBossSkill = (!this._templateDataOnly && isSwordFragment && this._itemData.SpecialArg >= 0);
			base.Init(argsBox);
			this.DisableDetail = !this._canCastBossSkill;
			base.PostInit();
			bool flag = this._xiangshuAvatarId >= 0;
			if (flag)
			{
				StoryDomainMethod.AsyncCall.GetDivineFlameDisplayData(this, new AsyncMethodCallbackDelegate(this.OnGetTipData));
			}
			else
			{
				this._isDivineFlameDataUnlocked = false;
				this._isDivineFlameDataGood = false;
				this.Refresh();
			}
		}

		// Token: 0x06006A08 RID: 27144 RVA: 0x0030E938 File Offset: 0x0030CB38
		private void OnGetTipData(int offset, RawDataPool dataPool)
		{
			DivineFlameData divineFlameData = null;
			Serializer.Deserialize(dataPool, offset, ref divineFlameData);
			this._isDivineFlameDataUnlocked = divineFlameData.IsUnlocked;
			this._isDivineFlameDataGood = SingletonObject.getInstance<BasicGameData>().IsXiangshuAvatarTaskStatusGood((int)this._xiangshuAvatarId);
			this.Refresh();
		}

		// Token: 0x06006A09 RID: 27145 RVA: 0x0030E97B File Offset: 0x0030CB7B
		public override void SetNewData(ArgumentBox argsBox)
		{
			this.Init(argsBox);
			this.Refresh();
		}

		// Token: 0x06006A0A RID: 27146 RVA: 0x0030E98D File Offset: 0x0030CB8D
		public override void Refresh()
		{
			base.Refresh();
			this.RefreshEatArea();
			this.RefreshCricketJarArea();
			this.RefreshCombat();
			this.RefreshDivineFlame();
			UIElement element = this.Element;
			if (element != null)
			{
				element.ShowAfterRefresh();
			}
		}

		// Token: 0x06006A0B RID: 27147 RVA: 0x0030E9C8 File Offset: 0x0030CBC8
		private void RefreshEatArea()
		{
			string title = LanguageKey.LK_ItemTips_Recover_Neili.Tr();
			string content = this.configData.Neili.ToString().SetColor("brightblue");
			this.propertyNeili.Set(string.Empty, title, content, true);
			this.propertyNeili.gameObject.SetActive(this.configData.Neili > 0);
			string icon = "ui9_icon_max_neili_big";
			string title2 = LanguageKey.LK_UnlockSkillSlot_MaxNeili.Tr();
			string content2 = string.Format("+{0}", this.configData.MaxNeili).SetColor("brightblue");
			this.propertyMaxNeili.Set(icon, title2, content2, true);
			this.propertyMaxNeili.gameObject.SetActive(this.configData.MaxNeili > 0);
			int type = this.configData.FiveElementTransfer.First;
			string icon2 = "ui9_icon_five_elements_" + type.ToString();
			LanguageKey typeNameKey = LanguageKey.LK_FiveElements_Type_0 + type;
			string title3 = LanguageKey.LK_Tooltip_Misc_Transfer.TrFormat(typeNameKey.Tr(), LanguageKey.LK_Neili.Tr());
			string content3 = string.Format("+{0}%", this.configData.FiveElementTransfer.Second).SetColor("brightblue");
			this.propertyFiveElementTransfer.Set(icon2, title3, content3, true);
			this.propertyFiveElementTransfer.gameObject.SetActive(this.configData.FiveElementTransfer.Second > 0);
			bool isShow = this.propertyNeili.gameObject.activeSelf || this.propertyMaxNeili.gameObject.activeSelf || this.propertyFiveElementTransfer.gameObject.activeSelf;
			this.rootEatArea.SetActive(isShow);
		}

		// Token: 0x06006A0C RID: 27148 RVA: 0x0030EB94 File Offset: 0x0030CD94
		private void RefreshCricketJarArea()
		{
			bool isCricketJar = this.configData.ItemSubType == 1201;
			this.rootCricketJarArea.SetActive(isCricketJar);
			bool flag = !isCricketJar;
			if (!flag)
			{
				this.propertyDurability.SetValue("+1".SetColor("brightblue"));
				this.textDuration.SetText(SharedMethods.CalcCricketRegenTime(this.configData.Grade).ToString(), true);
				this.propertyHeal.SetValue(string.Format("{0}%", this.configData.CricketHealInjuryOdds));
			}
		}

		// Token: 0x06006A0D RID: 27149 RVA: 0x0030EC34 File Offset: 0x0030CE34
		private void RefreshCombat()
		{
			bool isShowCost = this.configData.ConsumedFeatureMedals >= 0;
			this.propertyCost.gameObject.SetActive(isShowCost);
			bool flag = isShowCost;
			if (flag)
			{
				this.propertyCost.SetValue(this.configData.ConsumedFeatureMedals.ToString().SetColor("brightyellow"));
			}
			bool isTianSuiBaoLuItem = CommonUtils.IsTianSuiBaoLuItem(this._itemKey.ItemType, this._itemKey.TemplateId);
			bool isShowCombat = isShowCost || this._canCastBossSkill || isTianSuiBaoLuItem;
			this.rootCombat.SetActive(isShowCombat);
			this.RefreshTianSuiBaoLuEffect(isTianSuiBaoLuItem);
			this.RefreshCastBossSkill(this._canCastBossSkill);
		}

		// Token: 0x06006A0E RID: 27150 RVA: 0x0030ECE0 File Offset: 0x0030CEE0
		private void RefreshTianSuiBaoLuEffect(bool isShow)
		{
			this.propertyTianSuiBaoLu.gameObject.SetActive(isShow);
			bool flag = !isShow;
			if (!flag)
			{
				short templateId = this._itemKey.TemplateId;
				if (!true)
				{
				}
				LanguageKey languageKey;
				switch (templateId)
				{
				case 254:
					languageKey = LanguageKey.LK_CombatUseEffect_ShenJianSpiritWords;
					break;
				case 255:
					languageKey = LanguageKey.LK_CombatUseEffect_HuanSheSpiritWords;
					break;
				case 256:
					languageKey = LanguageKey.LK_CombatUseEffect_FuCangSpiritWords;
					break;
				case 257:
					languageKey = LanguageKey.LK_CombatUseEffect_YinShenSpiritWords;
					break;
				case 258:
					languageKey = LanguageKey.LK_CombatUseEffect_QuLiuWuSpiritWords;
					break;
				case 259:
					languageKey = LanguageKey.LK_CombatUseEffect_ShenJianMysteryWords;
					break;
				case 260:
					languageKey = LanguageKey.LK_CombatUseEffect_HuanSheMysteryWords;
					break;
				case 261:
					languageKey = LanguageKey.LK_CombatUseEffect_FuCangMysteryWords;
					break;
				case 262:
					languageKey = LanguageKey.LK_CombatUseEffect_YinShenMysteryWords;
					break;
				case 263:
					languageKey = LanguageKey.LK_CombatUseEffect_QuLiuWuMysteryWords;
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				if (!true)
				{
				}
				LanguageKey key = languageKey;
				this.propertyTianSuiBaoLu.SetValue(key.Tr());
			}
		}

		// Token: 0x06006A0F RID: 27151 RVA: 0x0030EDC4 File Offset: 0x0030CFC4
		private void RefreshCastBossSkill(bool isShow)
		{
			this.propertyCastBossSkill.gameObject.SetActive(isShow);
			this.propertyBossSkillTrick.gameObject.SetActive(isShow);
			this.propertyInfectionTips.gameObject.SetActive(isShow);
			bool flag = !isShow;
			if (!flag)
			{
				CombatSkillItem skillConfig = CombatSkill.Instance[this._itemData.SpecialArg];
				StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
				strBuilder.Clear();
				for (int i = 0; i < skillConfig.TrickCost.Count; i++)
				{
					bool flag2 = i > 0;
					if (flag2)
					{
						strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Split_Symbol));
					}
					strBuilder.Append(Config.TrickType.Instance[skillConfig.TrickCost[i].TrickType].Name);
				}
				string content = strBuilder.ToString();
				EasyPool.Free<StringBuilder>(strBuilder);
				string title = LanguageKey.UI_Cast_Boss_Skill_Trick_Tips.Tr();
				this.propertyBossSkillTrick.Set(string.Empty, title, content, true);
				string icon = "ui9_mousetip_icon_big_swordtombcombat_{0}".GetFormat(this._xiangshuAvatarId);
				string title2 = LanguageKey.UI_Cast_Boss_Skill_Tips.Tr();
				string content2 = skillConfig.Name.SetGradeColor((int)skillConfig.Grade);
				this.propertyCastBossSkill.Set(icon, title2, content2, true);
				string title3 = skillConfig.Name;
				string content3 = skillConfig.Desc.SetGradeColor((int)skillConfig.Grade);
				this.propertyBossSkill.Set(string.Empty, title3, content3, true);
				this.propertyInfectionTips.SetValue(LanguageKey.UI_Cast_Boss_Skill_Infection_Tips.Tr());
			}
		}

		// Token: 0x06006A10 RID: 27152 RVA: 0x0030EF64 File Offset: 0x0030D164
		private void RefreshDivineFlame()
		{
			this.rootDivineFlame.SetActive(this._isDivineFlameDataUnlocked);
			bool flag = !this._isDivineFlameDataUnlocked;
			if (!flag)
			{
				string title = ViewCharacterMenuItems.GetDivineFlameTitle(this._xiangshuAvatarId, true);
				string content = ViewCharacterMenuItems.GetDivineFlameSelectTargetTip(this._xiangshuAvatarId, true, false);
				this.propertyDivineFlameGood.Set(string.Empty, title, content, true);
				DisableStyleRoot styleRoot = this.propertyDivineFlameGood.StyleRoot;
				if (styleRoot != null)
				{
					styleRoot.SetInteractable(this._isDivineFlameDataGood);
				}
				string title2 = ViewCharacterMenuItems.GetDivineFlameTitle(this._xiangshuAvatarId, false);
				string content2 = ViewCharacterMenuItems.GetDivineFlameSelectTargetTip(this._xiangshuAvatarId, false, false);
				this.propertyDivineFlameBad.Set(string.Empty, title2, content2, true);
				DisableStyleRoot styleRoot2 = this.propertyDivineFlameBad.StyleRoot;
				if (styleRoot2 != null)
				{
					styleRoot2.SetInteractable(!this._isDivineFlameDataGood);
				}
				bool isGood = SingletonObject.getInstance<BasicGameData>().IsXiangshuAvatarTaskStatusGood((int)this._xiangshuAvatarId);
				string texture = "ui9_mousetip_img_swordtomb_{0}_{1}".GetFormat(isGood ? 0 : 1, this._xiangshuAvatarId);
				this.imageDivineFlameBack.SetTexture(texture);
			}
		}

		// Token: 0x06006A11 RID: 27153 RVA: 0x0030F07C File Offset: 0x0030D27C
		protected override void InitItemDisableFunctionList()
		{
			base.InitItemDisableFunctionList();
			bool flag = !this.configData.Repairable;
			if (flag)
			{
				this._disableFunctionList.Add(ItemFunction.Repairable);
			}
			bool flag2 = !this.configData.Transferable;
			if (flag2)
			{
				this._disableFunctionList.Add(ItemFunction.Transferable);
			}
			bool flag3 = !this.configData.Poisonable;
			if (flag3)
			{
				this._disableFunctionList.Add(ItemFunction.Poisonable);
			}
			bool flag4 = !this.configData.Refinable;
			if (flag4)
			{
				this._disableFunctionList.Add(ItemFunction.Refinable);
			}
		}

		// Token: 0x04004C6A RID: 19562
		[Header("服食效果")]
		[SerializeField]
		private GameObject rootEatArea;

		// Token: 0x04004C6B RID: 19563
		[SerializeField]
		private TooltipItemProperty propertyNeili;

		// Token: 0x04004C6C RID: 19564
		[SerializeField]
		private TooltipItemProperty propertyMaxNeili;

		// Token: 0x04004C6D RID: 19565
		[SerializeField]
		private TooltipItemProperty propertyFiveElementTransfer;

		// Token: 0x04004C6E RID: 19566
		[Header("物品效果")]
		[SerializeField]
		private GameObject rootCricketJarArea;

		// Token: 0x04004C6F RID: 19567
		[SerializeField]
		private TooltipItemProperty propertyDurability;

		// Token: 0x04004C70 RID: 19568
		[SerializeField]
		private TextMeshProUGUI textDuration;

		// Token: 0x04004C71 RID: 19569
		[SerializeField]
		private TooltipItemProperty propertyHeal;

		// Token: 0x04004C72 RID: 19570
		[Header("战斗使用")]
		[SerializeField]
		private GameObject rootCombat;

		// Token: 0x04004C73 RID: 19571
		[SerializeField]
		private TooltipItemProperty propertyCost;

		// Token: 0x04004C74 RID: 19572
		[SerializeField]
		private TooltipItemProperty propertyTianSuiBaoLu;

		// Token: 0x04004C75 RID: 19573
		[SerializeField]
		private TooltipItemProperty propertyCastBossSkill;

		// Token: 0x04004C76 RID: 19574
		[SerializeField]
		private TooltipItemProperty propertyBossSkillTrick;

		// Token: 0x04004C77 RID: 19575
		[Header("神剑效果")]
		[SerializeField]
		private GameObject rootDivineFlame;

		// Token: 0x04004C78 RID: 19576
		[SerializeField]
		private TooltipItemProperty propertyDivineFlameGood;

		// Token: 0x04004C79 RID: 19577
		[SerializeField]
		private TooltipItemProperty propertyDivineFlameBad;

		// Token: 0x04004C7A RID: 19578
		[SerializeField]
		private CRawImage imageDivineFlameBack;

		// Token: 0x04004C7B RID: 19579
		[Header("详细")]
		[SerializeField]
		private TooltipItemProperty propertyInfectionTips;

		// Token: 0x04004C7C RID: 19580
		[SerializeField]
		private TooltipItemProperty propertyBossSkill;

		// Token: 0x04004C7D RID: 19581
		private MiscItem configData;

		// Token: 0x04004C7E RID: 19582
		private bool _canCastBossSkill;

		// Token: 0x04004C7F RID: 19583
		private bool _isDivineFlameDataUnlocked;

		// Token: 0x04004C80 RID: 19584
		private bool _isDivineFlameDataGood;

		// Token: 0x04004C81 RID: 19585
		private sbyte _xiangshuAvatarId = -1;
	}
}
