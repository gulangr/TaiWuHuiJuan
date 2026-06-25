using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CharacterDataMonitor;
using Config;
using GameData.Domains.Character;
using GameData.Domains.Item;
using TMPro;

// Token: 0x020002EA RID: 746
public class TipWugKingEffect : Refers
{
	// Token: 0x06002BF2 RID: 11250 RVA: 0x00157F57 File Offset: 0x00156157
	private void OnDisable()
	{
		EatingItemMonitor eatingItemMonitor = this._eatingItemMonitor;
		if (eatingItemMonitor != null)
		{
			eatingItemMonitor.RemoveEatingItemListener(this._eatingItemAction);
		}
		this._eatingItemMonitor = null;
		this._eatingItemAction = null;
	}

	// Token: 0x06002BF3 RID: 11251 RVA: 0x00157F80 File Offset: 0x00156180
	public void Refresh(ItemKey itemKey, int charId, bool isEating)
	{
		bool isWugKing = EatingItems.IsWugKing(itemKey);
		base.gameObject.SetActive(isWugKing);
		bool flag = !isWugKing;
		if (!flag)
		{
			MedicineItem configData = Medicine.Instance[itemKey.TemplateId];
			WugKingItem wugKingConfig = WugKing.Instance.First((WugKingItem w) => w.WugMedicine == configData.TemplateId);
			string title = configData.Name + LocalStringManager.Get(LanguageKey.LK_Colon_Symbol);
			this.SetWugKingEffect("EffectLayout", title, configData.SpecialEffectDesc);
			short growingGoodWugId = wugKingConfig.GrowingGoodWugs.First<short>();
			MedicineItem growingGoodWugConfig = Medicine.Instance[growingGoodWugId];
			string growingGoodWugTitle = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_WugKing_Effect_GrowingGood, growingGoodWugConfig.Name);
			this.SetWugKingEffect("GrowingGoodEffectLayout", growingGoodWugTitle, wugKingConfig.GrowingGoodEffectDesc);
			short growingBadWugId = wugKingConfig.GrowingBadWugs.First<short>();
			MedicineItem growingBadWugConfig = Medicine.Instance[growingBadWugId];
			string growingBadWugTitle = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_WugKing_Effect_GrowingBad, growingBadWugConfig.Name);
			this.SetWugKingEffect("GrowingBadEffectLayout", growingBadWugTitle, wugKingConfig.GrowingBadEffectDesc);
			MedicineItem grownWugConfig = Medicine.Instance[wugKingConfig.GrownWug];
			string grownWugTitle = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_WugKing_Effect_Grown, grownWugConfig.Name);
			this.SetWugKingEffect("GrownEffectLayout", grownWugTitle, wugKingConfig.GrownEffectDesc);
			bool flag2 = charId >= 0 && isEating;
			if (flag2)
			{
				Func<ValueTuple<ItemKey, short>, bool> <>9__3;
				Func<ValueTuple<ItemKey, short>, bool> <>9__4;
				Func<ValueTuple<ItemKey, short>, bool> <>9__5;
				this._eatingItemAction = delegate()
				{
					bool hasWugKing = this._eatingItemMonitor.EatingItemList.Any((ValueTuple<ItemKey, short> x) => EatingItems.IsWugKing(x.Item1));
					bool flag3;
					if (hasWugKing)
					{
						IEnumerable<ValueTuple<ItemKey, short>> eatingItemList = this._eatingItemMonitor.EatingItemList;
						Func<ValueTuple<ItemKey, short>, bool> predicate;
						if ((predicate = <>9__3) == null)
						{
							predicate = (<>9__3 = ((ValueTuple<ItemKey, short> x) => EatingItems.IsWug(x.Item1) && wugKingConfig.GrowingGoodWugs.Contains(x.Item1.TemplateId)));
						}
						flag3 = eatingItemList.Any(predicate);
					}
					else
					{
						flag3 = false;
					}
					bool hasGrowingGood = flag3;
					this.SetWugKingEffectEnabled("GrowingGoodEffectLayout", hasGrowingGood);
					bool flag4;
					if (hasWugKing)
					{
						IEnumerable<ValueTuple<ItemKey, short>> eatingItemList2 = this._eatingItemMonitor.EatingItemList;
						Func<ValueTuple<ItemKey, short>, bool> predicate2;
						if ((predicate2 = <>9__4) == null)
						{
							predicate2 = (<>9__4 = ((ValueTuple<ItemKey, short> x) => EatingItems.IsWug(x.Item1) && wugKingConfig.GrowingBadWugs.Contains(x.Item1.TemplateId)));
						}
						flag4 = eatingItemList2.Any(predicate2);
					}
					else
					{
						flag4 = false;
					}
					bool hasGrowingBad = flag4;
					this.SetWugKingEffectEnabled("GrowingBadEffectLayout", hasGrowingBad);
					bool flag5;
					if (hasWugKing)
					{
						IEnumerable<ValueTuple<ItemKey, short>> eatingItemList3 = this._eatingItemMonitor.EatingItemList;
						Func<ValueTuple<ItemKey, short>, bool> predicate3;
						if ((predicate3 = <>9__5) == null)
						{
							predicate3 = (<>9__5 = ((ValueTuple<ItemKey, short> x) => EatingItems.IsWug(x.Item1) && wugKingConfig.GrownWug == x.Item1.TemplateId));
						}
						flag5 = eatingItemList3.Any(predicate3);
					}
					else
					{
						flag5 = false;
					}
					bool hasGrown = flag5;
					this.SetWugKingEffectEnabled("GrownEffectLayout", hasGrown);
					this.SetWugKingEffectEnabled("EffectLayout", hasWugKing && !hasGrowingGood && !hasGrowingBad && !hasGrown);
				};
				this._eatingItemMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<EatingItemMonitor>(charId, false);
				this._eatingItemMonitor.AddEatingItemListener(this._eatingItemAction);
				bool init = this._eatingItemMonitor.Init;
				if (init)
				{
					this._eatingItemAction();
				}
			}
			else
			{
				this.SetWugKingEffectEnabled("GrowingGoodEffectLayout", false);
				this.SetWugKingEffectEnabled("GrowingBadEffectLayout", false);
				this.SetWugKingEffectEnabled("GrownEffectLayout", false);
				this.SetWugKingEffectEnabled("EffectLayout", true);
			}
		}
	}

	// Token: 0x06002BF4 RID: 11252 RVA: 0x00158198 File Offset: 0x00156398
	private void SetWugKingEffect(string refers, string title, string content)
	{
		Refers effectLayout = base.CGet<Refers>(refers);
		effectLayout.CGet<TextMeshProUGUI>("Title").text = title;
		effectLayout.CGet<TextMeshProUGUI>("Content").text = content.ColorReplace();
		TextMeshProUGUI line = effectLayout.CGet<TextMeshProUGUI>("Line");
		bool flag = line != null;
		if (flag)
		{
			Regex colorReg = new Regex("<color=[^>]+>(.*?)");
			Match match = colorReg.Match(content);
			bool success = match.Success;
			if (success)
			{
				string lineStr = match.Groups[0].Value + line.text + "</color>";
				line.text = lineStr.ColorReplace();
			}
		}
	}

	// Token: 0x06002BF5 RID: 11253 RVA: 0x00158248 File Offset: 0x00156448
	private void SetWugKingEffectEnabled(string refers, bool enabled)
	{
		Refers effectLayout = base.CGet<Refers>(refers);
		DisableStyleRoot root = effectLayout.CGet<DisableStyleRoot>("Disable");
		root.SetStyleEffect(!enabled, false);
	}

	// Token: 0x06002BF6 RID: 11254 RVA: 0x00158278 File Offset: 0x00156478
	public static string GetKillWugTip(MedicineItem wugMedicineConfig)
	{
		sbyte killWugPoisonType = -1;
		bool flag = wugMedicineConfig.ItemSubType == 802 && wugMedicineConfig.WugGrowthType != 5;
		if (flag)
		{
			Medicine.Instance.Iterate(delegate(MedicineItem m)
			{
				bool flag3 = m.ItemSubType == 801 && m.DetoxWugType == wugMedicineConfig.WugType;
				bool result2;
				if (flag3)
				{
					killWugPoisonType = m.PoisonType;
					result2 = false;
				}
				else
				{
					result2 = true;
				}
				return result2;
			});
		}
		bool flag2 = killWugPoisonType < 0;
		string result;
		if (flag2)
		{
			result = LocalStringManager.Get(LanguageKey.LK_ItemTips_Wug_Cannot_Kill);
		}
		else
		{
			PoisonItem poisonConfig = Poison.Instance[killWugPoisonType];
			result = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_Wug_Can_Kill, "<SpName=" + poisonConfig.TipsIcon + ">" + poisonConfig.Name.SetColor(poisonConfig.FontColor));
		}
		return result;
	}

	// Token: 0x04001FE9 RID: 8169
	private EatingItemMonitor _eatingItemMonitor;

	// Token: 0x04001FEA RID: 8170
	private Action _eatingItemAction;
}
