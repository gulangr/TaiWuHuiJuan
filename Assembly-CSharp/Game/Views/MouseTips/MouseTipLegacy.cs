using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using Game.Views.Encyclopedia;
using GameData.Domains.World;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000869 RID: 2153
	public class MouseTipLegacy : MouseTipBase
	{
		// Token: 0x17000C73 RID: 3187
		// (get) Token: 0x060067ED RID: 26605 RVA: 0x002F7A24 File Offset: 0x002F5C24
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060067EE RID: 26606 RVA: 0x002F7A27 File Offset: 0x002F5C27
		protected override void Init(ArgumentBox argsBox)
		{
			this.Element.ForceListenCommand = true;
			this.Refresh(argsBox);
		}

		// Token: 0x060067EF RID: 26607 RVA: 0x002F7A40 File Offset: 0x002F5C40
		public override void Refresh(ArgumentBox argsBox)
		{
			string title;
			argsBox.Get("Title", out title);
			string maxTitle;
			argsBox.Get("MaxTitle", out maxTitle);
			List<Vector2Int> legacyProperties;
			argsBox.Get<List<Vector2Int>>("LegacyProperties", out legacyProperties);
			int point;
			argsBox.Get("TypePoint", out point);
			int totalPoint;
			argsBox.Get("TotalTypePoint", out totalPoint);
			List<IntList> legacyMaxPointAndTimesList;
			argsBox.Get<List<IntList>>("LegacyMaxPointAndTimesList", out legacyMaxPointAndTimesList);
			WorldCreationInfo worldCreationInfo;
			argsBox.Get<WorldCreationInfo>("WorldCreationInfo", out worldCreationInfo);
			int legacyPointBonusFactor;
			argsBox.Get("LegacyPointBonusFactor", out legacyPointBonusFactor);
			ValueTuple<int, int> summary = this.RefreshLegacyProperties(legacyProperties, legacyMaxPointAndTimesList, worldCreationInfo, title, legacyPointBonusFactor);
			this.buildingBg.SetActive(legacyPointBonusFactor != 0);
			bool flag = legacyPointBonusFactor != 0;
			if (flag)
			{
				this.buildingDesc.text = LanguageKey.LK_Legacy_Tip_BuildingBuff.TrFormat(title, CommonUtils.GetColoredValue(legacyPointBonusFactor, true, 0, true, true)).ColorReplace();
			}
			this.textTitle2.text = (this.textTitle.text = title);
			this.textMaxTitle.text = LanguageKey.LK_Legacy_Detail_Max.TrFormat(new object[]
			{
				title,
				(summary.Item1 < summary.Item2) ? "lightgrey" : "brightyellow",
				summary.Item1,
				summary.Item2
			}).ColorReplace();
			this.maxIcon.gameObject.SetActive(summary.Item1 >= summary.Item2);
		}

		// Token: 0x060067F0 RID: 26608 RVA: 0x002F7BB8 File Offset: 0x002F5DB8
		private int GetMaxPoint(short legacyTemplateId, List<IntList> legacyMaxPointList)
		{
			foreach (IntList item in legacyMaxPointList)
			{
				bool flag = item.Items[0] == (int)legacyTemplateId;
				if (flag)
				{
					return item.Items[1];
				}
			}
			throw new Exception(string.Format("Can't find max point for legacyTemplateId: {0}", legacyTemplateId));
		}

		// Token: 0x060067F1 RID: 26609 RVA: 0x002F7C3C File Offset: 0x002F5E3C
		[return: TupleElementNames(new string[]
		{
			"totalValue",
			"totalMaxValue"
		})]
		private ValueTuple<int, int> RefreshLegacyProperties(List<Vector2Int> legacyProperties, List<IntList> legacyMaxPointAndTimesList, WorldCreationInfo worldCreationInfo, string typeText, int legacyPointBonusFactor)
		{
			int totalValue = 0;
			int totalMaxValue = 0;
			this.points.Rebuild<LegacyItem>(legacyProperties.Count, delegate(LegacyItem trans, int i)
			{
				trans.gameObject.SetActive(true);
				int maxPoint = this.GetMaxPoint((short)legacyProperties[i].x, legacyMaxPointAndTimesList);
				int times = legacyMaxPointAndTimesList[i].Items[2];
				trans.RefreshLegacyItem(legacyProperties[i], maxPoint, worldCreationInfo, times, legacyPointBonusFactor);
				totalValue += legacyProperties[i].y;
				totalMaxValue += maxPoint;
			});
			this.RefreshTotalBonus(legacyProperties, worldCreationInfo, typeText);
			this.RefreshWhenAltChange(this._lastAltDown);
			return new ValueTuple<int, int>(totalValue, totalMaxValue);
		}

		// Token: 0x060067F2 RID: 26610 RVA: 0x002F7CDC File Offset: 0x002F5EDC
		private void RefreshTotalBonus(IEnumerable<Vector2Int> legacyProperties, WorldCreationInfo worldCreationInfo, string typeText)
		{
			int creationTypeMaskSet = 0;
			int totalTypeBonus = 0;
			foreach (byte creationType in legacyProperties.SelectMany((Vector2Int t) => LegacyPoint.Instance[t.x].BonusTypes))
			{
				bool flag = (1 << (int)creationType & creationTypeMaskSet) == 0;
				if (flag)
				{
					totalTypeBonus += (int)WorldCreation.Instance[creationType].LegacyPointBonus[MouseTipLegacy.GetWorldCreationPercentValue(worldCreationInfo, creationType)];
					creationTypeMaskSet |= 1 << (int)creationType;
				}
			}
			this.desc1.text = LanguageKey.LK_Legacy_Tip_Desc_Bonus.TrFormat(typeText, string.Format("+{0}%", totalTypeBonus)).ColorReplace();
		}

		// Token: 0x060067F3 RID: 26611 RVA: 0x002F7DAC File Offset: 0x002F5FAC
		internal static int GetWorldCreationPercentValue(WorldCreationInfo worldCreationInfo, byte creationType)
		{
			if (!true)
			{
			}
			int result;
			switch (creationType)
			{
			case 1:
				result = (int)worldCreationInfo.CombatDifficulty;
				goto IL_AE;
			case 2:
				result = (int)worldCreationInfo.ReadingDifficulty;
				goto IL_AE;
			case 3:
				result = (int)worldCreationInfo.BreakoutDifficulty;
				goto IL_AE;
			case 4:
				result = (int)worldCreationInfo.LoopingDifficulty;
				goto IL_AE;
			case 5:
				result = (int)worldCreationInfo.HereticsAmountType;
				goto IL_AE;
			case 6:
				result = (int)worldCreationInfo.BossInvasionSpeedType;
				goto IL_AE;
			case 7:
				result = (int)worldCreationInfo.WorldResourceAmountType;
				goto IL_AE;
			case 11:
				result = (int)worldCreationInfo.EnemyPracticeLevel;
				goto IL_AE;
			case 12:
				result = (int)worldCreationInfo.FavorabilityChange;
				goto IL_AE;
			case 13:
				result = (int)worldCreationInfo.ProfessionUpgrade;
				goto IL_AE;
			case 14:
				result = (int)worldCreationInfo.LootYield;
				goto IL_AE;
			}
			result = -1;
			IL_AE:
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x060067F4 RID: 26612 RVA: 0x002F7E70 File Offset: 0x002F6070
		private void Update()
		{
			bool flag = !this.HasStick;
			if (flag)
			{
				bool altDown = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
				this.moreInfoTips0.text = (altDown ? LanguageKey.LK_Release_Key_Tips : LanguageKey.LK_Hold_Key_Tips).Tr();
				this.moreInfoTips1.text = (altDown ? LanguageKey.LK_Back_To_Simple_Tips : LanguageKey.LK_Show_Detail_Tips).Tr();
				bool flag2 = this._lastAltDown != altDown;
				if (flag2)
				{
					this.RefreshWhenAltChange(altDown);
				}
				this._lastAltDown = altDown;
				bool flag3 = CommonCommandKit.PrimaryInteraction.Check(this.Element, false, false, false, true, false);
				if (flag3)
				{
					ViewEncyclopediaPanel.OpenLink(EncyclopediaTipLink.DefValue.TipLegacy);
				}
			}
		}

		// Token: 0x060067F5 RID: 26613 RVA: 0x002F7F30 File Offset: 0x002F6130
		private void RefreshWhenAltChange(bool altDown)
		{
			for (int i = 0; i < this.points.transform.childCount; i++)
			{
				LegacyItem component = this.points.transform.GetChild(i).GetComponent<LegacyItem>();
				if (component != null)
				{
					component.RefreshWhenAltChange(altDown);
				}
			}
			this.simpleModeDelim.SetActive(!altDown);
		}

		// Token: 0x04004962 RID: 18786
		[SerializeField]
		private CImage maxIcon;

		// Token: 0x04004963 RID: 18787
		[SerializeField]
		private TMP_Text textMaxTitle;

		// Token: 0x04004964 RID: 18788
		[SerializeField]
		private TMP_Text desc1;

		// Token: 0x04004965 RID: 18789
		[SerializeField]
		private TMP_Text moreInfoTips0;

		// Token: 0x04004966 RID: 18790
		[SerializeField]
		private TMP_Text moreInfoTips1;

		// Token: 0x04004967 RID: 18791
		[SerializeField]
		private TMP_Text textTitle;

		// Token: 0x04004968 RID: 18792
		[SerializeField]
		private TMP_Text textTitle2;

		// Token: 0x04004969 RID: 18793
		[SerializeField]
		private TMP_Text buildingDesc;

		// Token: 0x0400496A RID: 18794
		[SerializeField]
		private GameObject simpleModeDelim;

		// Token: 0x0400496B RID: 18795
		[SerializeField]
		private GameObject buildingBg;

		// Token: 0x0400496C RID: 18796
		[SerializeField]
		private TemplatedContainerAssemblyNew points;

		// Token: 0x0400496D RID: 18797
		private bool _lastAltDown = false;
	}
}
