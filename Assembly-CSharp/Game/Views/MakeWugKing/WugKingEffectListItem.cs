using System;
using System.Collections.Generic;
using System.Linq;
using CharacterDataMonitor;
using Config;
using GameData.Domains.Character;
using GameData.Domains.Item;
using TMPro;

namespace Game.Views.MakeWugKing
{
	// Token: 0x0200094B RID: 2379
	public class WugKingEffectListItem : Refers
	{
		// Token: 0x0600707A RID: 28794 RVA: 0x00341645 File Offset: 0x0033F845
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

		// Token: 0x0600707B RID: 28795 RVA: 0x00341670 File Offset: 0x0033F870
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

		// Token: 0x0600707C RID: 28796 RVA: 0x00341888 File Offset: 0x0033FA88
		private void SetWugKingEffect(string refers, string title, string content)
		{
			Refers effectLayout = base.CGet<Refers>(refers);
			TextMeshProUGUI contentTxt = effectLayout.CGet<TextMeshProUGUI>("Content");
			contentTxt.text = LocalStringManager.Get(LanguageKey.LK_Dot_Symbol) + title + content;
			contentTxt.text = contentTxt.text.ColorReplace();
		}

		// Token: 0x0600707D RID: 28797 RVA: 0x003418D4 File Offset: 0x0033FAD4
		private void SetWugKingEffectEnabled(string refers, bool enabled)
		{
		}

		// Token: 0x0600707E RID: 28798 RVA: 0x003418E4 File Offset: 0x0033FAE4
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

		// Token: 0x04005363 RID: 21347
		private EatingItemMonitor _eatingItemMonitor;

		// Token: 0x04005364 RID: 21348
		private Action _eatingItemAction;
	}
}
