using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Combat
{
	// Token: 0x02000AF7 RID: 2807
	public class CombatConfigTips : MonoBehaviour
	{
		// Token: 0x06008A41 RID: 35393 RVA: 0x004006FE File Offset: 0x003FE8FE
		private static bool IsAttack(sbyte combatSkillType)
		{
			return combatSkillType != 1 && combatSkillType != 2;
		}

		// Token: 0x06008A42 RID: 35394 RVA: 0x00400710 File Offset: 0x003FE910
		private static string FormatDistance(byte distance)
		{
			return ((int)(distance / 10)).ToString() + "." + ((int)(distance % 10)).ToString();
		}

		// Token: 0x06008A43 RID: 35395 RVA: 0x00400744 File Offset: 0x003FE944
		private static string FormatFiveElements(sbyte fiveElementsType)
		{
			return LocalStringManager.GetFormat(LanguageKey.LK_Quotation_Marks_Fix, LocalStringManager.Get("LK_FiveElements_Type_" + fiveElementsType.ToString()));
		}

		// Token: 0x06008A44 RID: 35396 RVA: 0x00400778 File Offset: 0x003FE978
		private static string FormatCombatSkillType(sbyte combatSkillType)
		{
			return LocalStringManager.GetFormat(LanguageKey.LK_Quotation_Marks_Fix, CombatSkillType.Instance[combatSkillType].Name);
		}

		// Token: 0x17000F40 RID: 3904
		// (get) Token: 0x06008A45 RID: 35397 RVA: 0x004007A4 File Offset: 0x003FE9A4
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x06008A46 RID: 35398 RVA: 0x004007AB File Offset: 0x003FE9AB
		private void Awake()
		{
			this._savedPaddingLeft = this.multiplyLayout.padding.left;
			this._savedPaddingRight = this.multiplyLayout.padding.right;
		}

		// Token: 0x06008A47 RID: 35399 RVA: 0x004007DC File Offset: 0x003FE9DC
		public void SetTips(int enemyCharTemplateId)
		{
			string configTipsText = this.BuildTips(enemyCharTemplateId);
			TextMeshProUGUI configTips = this.multiplyTips;
			configTips.text = configTipsText;
			configTips.GetComponent<TMPTextSpriteHelper>().Parse();
			this.multiplyGo.gameObject.SetActive(!configTips.text.IsNullOrEmpty());
			bool flag = this._tipCount <= 1;
			if (flag)
			{
				this.multiplyLayout.padding.left = 0;
				this.multiplyLayout.padding.right = 0;
				configTips.alignment = TextAlignmentOptions.Center;
			}
			else
			{
				this.multiplyLayout.padding.left = this._savedPaddingLeft;
				this.multiplyLayout.padding.right = this._savedPaddingRight;
				configTips.alignment = TextAlignmentOptions.Left;
			}
		}

		// Token: 0x06008A48 RID: 35400 RVA: 0x004008AC File Offset: 0x003FEAAC
		public void SetConSummateLevel(sbyte selfConsummateLevel, sbyte enemyConsummateLevel)
		{
			this.consummateLevelTips.Set(selfConsummateLevel, enemyConsummateLevel);
		}

		// Token: 0x06008A49 RID: 35401 RVA: 0x004008C0 File Offset: 0x003FEAC0
		private string BuildTips(int enemyCharTemplateId)
		{
			CombatConfigItem config = this.Model.Config;
			this._tipCount = 0;
			this._mainBuilder.Clear();
			string separator = LocalStringManager.Get(LanguageKey.LK_Split_Symbol);
			List<sbyte> list = config.CombatSkillType;
			bool flag = list != null && list.Count > 0;
			if (flag)
			{
				this.AppendTips("<SpName=combat_bifen_icon_juli>" + LocalStringManager.GetFormat(LanguageKey.LK_Combat_Config_Tips_Skill_Attack_Type, string.Join(separator, config.CombatSkillType.Where(new Func<sbyte, bool>(CombatConfigTips.IsAttack)).Select(new Func<sbyte, string>(CombatConfigTips.FormatCombatSkillType)))));
			}
			bool flag2 = config.Sect >= 0;
			if (flag2)
			{
				this.AppendTips("<SpName=combat_bifen_icon_juli>" + LocalStringManager.GetFormat(LanguageKey.LK_Combat_Config_Tips_Skill_Sect, Organization.Instance[config.Sect].Name));
			}
			list = config.FiveElementsOfSkill;
			bool flag3 = list != null && list.Count > 0;
			if (flag3)
			{
				this.AppendTips("<SpName=combat_bifen_icon_juli>" + LocalStringManager.GetFormat(LanguageKey.LK_Combat_Config_Tips_Skill_FiveElements, string.Join(separator, config.FiveElementsOfSkill.Select(new Func<sbyte, string>(CombatConfigTips.FormatFiveElements)))));
			}
			bool flag4 = config.MinDistance > 20 || config.MaxDistance < 120;
			if (flag4)
			{
				this.AppendTips("<SpName=combat_bifen_icon_juli>" + LocalStringManager.GetFormat(LanguageKey.LK_Combat_Config_Tips_Distance, CombatConfigTips.FormatDistance(config.MinDistance), CombatConfigTips.FormatDistance(config.MaxDistance)));
			}
			bool flag5 = !Character.Instance[enemyCharTemplateId].CanDefeat;
			if (flag5)
			{
				this.AppendTips("<SpName=combat_bifen_icon_jibai>" + LocalStringManager.Get(LanguageKey.LK_Combat_Immortal_Enemy_Tips).SetColor("brightred"));
			}
			bool flag6 = config.CaptureRate <= 0;
			if (flag6)
			{
				this.AppendTips("<SpName=combat_bifen_icon_guanya>" + LocalStringManager.Get(LanguageKey.LK_Combat_Config_Tips_No_Capture));
			}
			bool flag7 = !config.SelfCanFlee;
			if (flag7)
			{
				this.AppendTips(("<SpName=combat_bifen_icon_wufataopao>" + LocalStringManager.Get(LanguageKey.LK_Combat_Config_Tips_No_Flee)).SetColor("brightred"));
			}
			bool flag8 = config.LootItemRate <= 0;
			if (flag8)
			{
				this.AppendTips("<SpName=combat_bifen_icon_diaoluo>" + LocalStringManager.Get(LanguageKey.LK_Combat_Config_Tips_No_Loot));
			}
			return this._mainBuilder.ToString();
		}

		// Token: 0x06008A4A RID: 35402 RVA: 0x00400B1C File Offset: 0x003FED1C
		private void AppendTips(string str)
		{
			bool flag = this._tipCount > 0;
			if (flag)
			{
				this._mainBuilder.Append("\n");
			}
			this._mainBuilder.Append(str);
			this._tipCount++;
		}

		// Token: 0x04006A02 RID: 27138
		[SerializeField]
		private GameObject multiplyGo;

		// Token: 0x04006A03 RID: 27139
		[SerializeField]
		private TextMeshProUGUI multiplyTips;

		// Token: 0x04006A04 RID: 27140
		[SerializeField]
		private VerticalLayoutGroup multiplyLayout;

		// Token: 0x04006A05 RID: 27141
		[SerializeField]
		private CombatConsummateTips consummateLevelTips;

		// Token: 0x04006A06 RID: 27142
		private const string Icon1 = "<SpName=combat_bifen_icon_juli>";

		// Token: 0x04006A07 RID: 27143
		private const string Icon2 = "<SpName=combat_bifen_icon_jibai>";

		// Token: 0x04006A08 RID: 27144
		private const string Icon3 = "<SpName=combat_bifen_icon_guanya>";

		// Token: 0x04006A09 RID: 27145
		private const string Icon4 = "<SpName=combat_bifen_icon_wufataopao>";

		// Token: 0x04006A0A RID: 27146
		private const string Icon5 = "<SpName=combat_bifen_icon_diaoluo>";

		// Token: 0x04006A0B RID: 27147
		private readonly StringBuilder _mainBuilder = new StringBuilder();

		// Token: 0x04006A0C RID: 27148
		private int _tipCount;

		// Token: 0x04006A0D RID: 27149
		private int _savedPaddingLeft;

		// Token: 0x04006A0E RID: 27150
		private int _savedPaddingRight;
	}
}
