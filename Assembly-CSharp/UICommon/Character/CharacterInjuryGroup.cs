using System;
using System.Runtime.CompilerServices;
using CharacterDataMonitor;
using Config;
using GameData.Domains.Character;
using TMPro;
using UnityEngine;

namespace UICommon.Character
{
	// Token: 0x020005D9 RID: 1497
	public class CharacterInjuryGroup : CharacterUIElement
	{
		// Token: 0x170008EF RID: 2287
		// (get) Token: 0x060046D0 RID: 18128 RVA: 0x00212CF9 File Offset: 0x00210EF9
		private InjuryPoisonMonitor Item
		{
			get
			{
				return this.MonitorDataItem as InjuryPoisonMonitor;
			}
		}

		// Token: 0x060046D1 RID: 18129 RVA: 0x00212D08 File Offset: 0x00210F08
		public CharacterInjuryGroup(Refers[] injuryRefersArray, GameObject mixPoisonTip)
		{
			bool flag = injuryRefersArray == null || injuryRefersArray.Length == 0;
			if (flag)
			{
				throw new Exception("CharacterInjuryGroup must handle at least one injuryRefers");
			}
			this._injuryRefersArray = injuryRefersArray;
			CharacterInjuryGroup.InitNameAndValue(this._injuryRefersArray);
			this._mixPoisonTip = mixPoisonTip;
		}

		// Token: 0x060046D2 RID: 18130 RVA: 0x00212D54 File Offset: 0x00210F54
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<InjuryPoisonMonitor>(charId, this.IsDead);
		}

		// Token: 0x060046D3 RID: 18131 RVA: 0x00212D77 File Offset: 0x00210F77
		internal override void BindEvent()
		{
			this.Item.AddInjuriesListener(new Action(this.FillElement));
		}

		// Token: 0x060046D4 RID: 18132 RVA: 0x00212D93 File Offset: 0x00210F93
		public override void UnbindEvent()
		{
			this.Item.RemoveInjuriesListener(new Action(this.FillElement));
		}

		// Token: 0x060046D5 RID: 18133 RVA: 0x00212DB0 File Offset: 0x00210FB0
		public override void FillElement()
		{
			sbyte sumOuter = 0;
			sbyte sumInner = 0;
			for (sbyte i = 0; i < 7; i += 1)
			{
				ValueTuple<sbyte, sbyte> valueTuple = this.Item.Injuries.Get(i);
				sbyte partOuter = valueTuple.Item1;
				sbyte partInner = valueTuple.Item2;
				CharacterInjuryGroup.FillInjuryByType(i, new ValueTuple<sbyte, sbyte>(partOuter, partInner), this._injuryRefersArray, this.CustomFillElement);
				sumOuter += partOuter;
				sumInner += partInner;
			}
			this.FillFixPoisonTip();
			CharacterInjuryGroup.RefreshSumInjury(this.Item.TemplateId, this._injuryRefersArray, sumOuter, sumInner);
		}

		// Token: 0x060046D6 RID: 18134 RVA: 0x00212E3C File Offset: 0x0021103C
		public override void ResetToEmpty()
		{
			CharacterInjuryGroup.ResetToEmpty(this._injuryRefersArray);
		}

		// Token: 0x060046D7 RID: 18135 RVA: 0x00212E4C File Offset: 0x0021104C
		private void FillFixPoisonTip()
		{
			bool flag = this._mixPoisonTip != null && this.MixPoisonFillElement != null;
			if (flag)
			{
				this.MixPoisonFillElement(this._mixPoisonTip);
			}
		}

		// Token: 0x060046D8 RID: 18136 RVA: 0x00212E8C File Offset: 0x0021108C
		public static void RefreshSumInjury(short templateId, Refers[] injuryRefersArray, sbyte sumOuter, sbyte sumInner)
		{
			bool flag = injuryRefersArray.CheckIndex(7) && null != injuryRefersArray[7];
			if (flag)
			{
				Refers refers = injuryRefersArray[7];
				bool flag2 = null != refers;
				if (flag2)
				{
					CharacterItem characterItem = Character.Instance[templateId];
					refers.CGet<TextMeshProUGUI>("OuterValue").text = (characterItem.OuterInjuryImmunity ? LocalStringManager.Get(LanguageKey.LK_PoisonImmune) : sumOuter.ToString());
					refers.CGet<TextMeshProUGUI>("InnerValue").text = (characterItem.InnerInjuryImmunity ? LocalStringManager.Get(LanguageKey.LK_PoisonImmune) : sumInner.ToString());
				}
			}
		}

		// Token: 0x060046D9 RID: 18137 RVA: 0x00212F2C File Offset: 0x0021112C
		public static void FillInjuryByType(sbyte bodyType, [TupleElementNames(new string[]
		{
			"outer",
			"inner"
		})] ValueTuple<sbyte, sbyte> injury, Refers[] injuryRefersArray, Action<Refers, sbyte, sbyte, sbyte> action)
		{
			bool flag = injuryRefersArray.CheckIndex((int)bodyType) && null != injuryRefersArray[(int)bodyType];
			if (flag)
			{
				Refers refers = injuryRefersArray[(int)bodyType];
				bool flag2 = null != refers;
				if (flag2)
				{
					bool flag3 = action != null;
					if (flag3)
					{
						action(refers, bodyType, injury.Item1, injury.Item2);
					}
					else
					{
						refers.CGet<TextMeshProUGUI>("OuterValue").text = injury.Item1.ToString().SetColor((injury.Item1 <= 0) ? "grey" : "outterinjury");
						refers.CGet<TextMeshProUGUI>("InnerValue").text = injury.Item2.ToString().SetColor((injury.Item2 <= 0) ? "grey" : "innerinjury");
					}
				}
			}
		}

		// Token: 0x060046DA RID: 18138 RVA: 0x00212FFC File Offset: 0x002111FC
		public static void InitNameAndValue(Refers[] injuryRefersArray)
		{
			for (int i = 0; i < 7; i++)
			{
				Refers refer = injuryRefersArray[i];
				bool flag = null != refer;
				if (flag)
				{
					refer.CGet<TextMeshProUGUI>("Name").text = BodyPart.Instance[i].Name;
					refer.CGet<TextMeshProUGUI>("OuterValue").text = string.Empty;
					refer.CGet<TextMeshProUGUI>("InnerValue").text = string.Empty;
				}
			}
		}

		// Token: 0x060046DB RID: 18139 RVA: 0x0021307A File Offset: 0x0021127A
		public static void ResetToEmpty(Refers[] injuryRefersArray)
		{
			Array.ForEach<Refers>(injuryRefersArray, delegate(Refers refers)
			{
				bool flag = null != refers;
				if (flag)
				{
					refers.CGet<TextMeshProUGUI>("OuterValue").text = string.Empty;
					refers.CGet<TextMeshProUGUI>("InnerValue").text = string.Empty;
					bool flag2 = refers.Names.Contains("NewInner");
					if (flag2)
					{
						refers.CGet<GameObject>("NewInner").SetActive(false);
					}
					bool flag3 = refers.Names.Contains("NewOuter");
					if (flag3)
					{
						refers.CGet<GameObject>("NewOuter").SetActive(false);
					}
				}
			});
		}

		// Token: 0x060046DC RID: 18140 RVA: 0x002130A4 File Offset: 0x002112A4
		public static void SetInjuryChange(Refers[] injuryRefersArray, Injuries beforeInjuries, Injuries afterInjuries)
		{
			for (sbyte i = 0; i < 7; i += 1)
			{
				Refers refers = injuryRefersArray[(int)i];
				ValueTuple<sbyte, sbyte> valueTuple = beforeInjuries.Get(i);
				sbyte beforeOuter = valueTuple.Item1;
				sbyte beforeInner = valueTuple.Item2;
				ValueTuple<sbyte, sbyte> valueTuple2 = afterInjuries.Get(i);
				sbyte afterOuter = valueTuple2.Item1;
				sbyte afterInner = valueTuple2.Item2;
				int num = (int)(afterOuter - beforeOuter);
				int num2 = (int)(afterInner - beforeInner);
				int newAddOuterValue = num;
				int newAddInnerValue = num2;
				bool flag = null != refers;
				if (flag)
				{
					refers.CGet<GameObject>("NewInner").SetActive(newAddInnerValue != 0);
					refers.CGet<GameObject>("NewOuter").SetActive(newAddOuterValue != 0);
					string innerValueChange = (newAddInnerValue > 0) ? string.Format("+{0}", newAddInnerValue) : newAddInnerValue.ToString();
					string outerValueChange = (newAddOuterValue > 0) ? string.Format("+{0}", newAddOuterValue) : newAddOuterValue.ToString();
					refers.CGet<TextMeshProUGUI>("InnerValueNew").text = innerValueChange.SetColor((newAddInnerValue > 0) ? "brightred" : "brightblue");
					refers.CGet<TextMeshProUGUI>("OuterValueNew").text = outerValueChange.SetColor((newAddOuterValue > 0) ? "brightred" : "brightblue");
				}
			}
		}

		// Token: 0x04003102 RID: 12546
		public Action<Refers, sbyte, sbyte, sbyte> CustomFillElement;

		// Token: 0x04003103 RID: 12547
		public Action<GameObject> MixPoisonFillElement;

		// Token: 0x04003104 RID: 12548
		private readonly Refers[] _injuryRefersArray;

		// Token: 0x04003105 RID: 12549
		private readonly GameObject _mixPoisonTip;
	}
}
