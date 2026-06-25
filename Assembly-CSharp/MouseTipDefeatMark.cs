using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using Game.Views.Combat;
using GameData.Domains.Combat;
using GameData.Utilities;
using TMPro;

// Token: 0x0200028D RID: 653
public class MouseTipDefeatMark : MouseTipBase
{
	// Token: 0x060029D4 RID: 10708 RVA: 0x0013D434 File Offset: 0x0013B634
	public override bool CanShowWithArgumentBox(ArgumentBox argumentBox)
	{
		int markKeyInt;
		List<DefeatMarkKey> markKeyList;
		bool flag = argumentBox.Get("MarkKey", out markKeyInt) && argumentBox.Get<List<DefeatMarkKey>>("MarkKeyList", out markKeyList);
		return flag && ((DefeatMarkKey)markKeyInt).Valid && markKeyList != null && markKeyList.Count > 0;
	}

	// Token: 0x060029D5 RID: 10709 RVA: 0x0013D494 File Offset: 0x0013B694
	protected override void Init(ArgumentBox argsBox)
	{
		int markKeyInt;
		argsBox.Get("MarkKey", out markKeyInt);
		this._markKey = (DefeatMarkKey)markKeyInt;
		argsBox.Get<List<DefeatMarkKey>>("MarkKeyList", out this._markKeyList);
		HeavyOrBreakInjuryData heavyOrBreakInjuryData;
		argsBox.Get<HeavyOrBreakInjuryData>("HeavyOrBreakData", out heavyOrBreakInjuryData);
		CombatHeavyOrBreakLayout combatHeavyOrBreakLayout = base.CGet<CombatHeavyOrBreakLayout>("HeavyOrBreakLayout");
		HeavyOrBreakInjuryData data = heavyOrBreakInjuryData;
		EMarkType type = this._markKey.Type;
		combatHeavyOrBreakLayout.Set(data, (int)((type == EMarkType.Outer || type == EMarkType.Inner) ? this._markKey.BodyPart : -1));
		bool condition;
		if (this._markKey.Valid)
		{
			List<DefeatMarkKey> markKeyList = this._markKeyList;
			condition = (markKeyList != null && markKeyList.Count > 0);
		}
		else
		{
			condition = false;
		}
		Tester.Assert(condition, "");
		this.UpdateMarks();
	}

	// Token: 0x060029D6 RID: 10710 RVA: 0x0013D548 File Offset: 0x0013B748
	private void UpdateMarks()
	{
		base.CGet<TextMeshProUGUI>("MarkType").text = CombatConstants.ParseMarkType(this._markKey);
		base.CGet<TextMeshProUGUI>("MarkName").text = CombatConstants.ParseMarkName(this._markKey);
		base.CGet<TextMeshProUGUI>("MarkCount").text = this.GetMarkCountText();
		base.CGet<CImage>("MarkIcon").SetSprite(CombatConstants.ParseMarkIcon(this._markKey), false, null);
	}

	// Token: 0x060029D7 RID: 10711 RVA: 0x0013D5C4 File Offset: 0x0013B7C4
	private string GetMarkCountText()
	{
		int count = this._markKeyList.Count(new Func<DefeatMarkKey, bool>(this.GroupEquals));
		bool flag = !this._markKey.HasOld;
		string result;
		if (flag)
		{
			result = count.ToString();
		}
		else
		{
			int oldMark = this._markKeyList.Count(new Func<DefeatMarkKey, bool>(this.GroupEqualsAndOld));
			int newMark = count - oldMark;
			LanguageKey languageId = (this._markKey.Type == EMarkType.Mind) ? LanguageKey.LK_Combat_MarkCount_Mind : LanguageKey.LK_Combat_MarkCount_Old;
			result = LocalStringManager.GetFormat(languageId, oldMark, newMark);
		}
		return result;
	}

	// Token: 0x060029D8 RID: 10712 RVA: 0x0013D658 File Offset: 0x0013B858
	private bool GroupEquals(DefeatMarkKey markKey)
	{
		return this._markKey.GroupEquals(markKey);
	}

	// Token: 0x060029D9 RID: 10713 RVA: 0x0013D666 File Offset: 0x0013B866
	private bool GroupEqualsAndOld(DefeatMarkKey markKey)
	{
		return markKey.Old && this.GroupEquals(markKey);
	}

	// Token: 0x04001E68 RID: 7784
	private DefeatMarkKey _markKey;

	// Token: 0x04001E69 RID: 7785
	private List<DefeatMarkKey> _markKeyList;
}
