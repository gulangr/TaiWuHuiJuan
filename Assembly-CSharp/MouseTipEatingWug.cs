using System;
using System.Collections.Generic;
using System.Linq;
using CharacterDataMonitor;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Item;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000294 RID: 660
public class MouseTipEatingWug : MouseTipBase
{
	// Token: 0x060029F7 RID: 10743 RVA: 0x0013E8A0 File Offset: 0x0013CAA0
	protected override void OnDisable()
	{
		base.OnDisable();
		EatingItemMonitor eatingItemMonitor = this._eatingItemMonitor;
		if (eatingItemMonitor != null)
		{
			eatingItemMonitor.RemoveEatingItemListener(this._eatingItemAction);
		}
		this._eatingItemMonitor = null;
		this._eatingItemAction = null;
	}

	// Token: 0x060029F8 RID: 10744 RVA: 0x0013E8D0 File Offset: 0x0013CAD0
	protected override void Init(ArgumentBox argsBox)
	{
		CanvasGroup canvasGroup = base.GetComponent<CanvasGroup>();
		canvasGroup.alpha = 0f;
		short templateId;
		argsBox.Get("TemplateId", out templateId);
		int charId;
		argsBox.Get("CharId", out charId);
		short eatingTime;
		argsBox.Get("EatingTime", out eatingTime);
		MedicineItem wugConfig = Medicine.Instance.GetItem(templateId);
		bool flag = wugConfig == null;
		if (!flag)
		{
			base.CGet<TextMeshProUGUI>("Title").text = wugConfig.Name;
			base.CGet<TextMeshProUGUI>("Desc").text = wugConfig.Desc;
			base.CGet<TextMeshProUGUI>("WugEffect").text = wugConfig.SpecialEffectDesc;
			TextMeshProUGUI eatingTimeText = base.CGet<TextMeshProUGUI>("EatingTime");
			eatingTimeText.text = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_Eating_Time, eatingTime).ColorReplace();
			eatingTimeText.gameObject.SetActive(eatingTime > 0);
			WugKingItem wugKingConfig = null;
			WugKing.Instance.Iterate(delegate(WugKingItem w)
			{
				bool flag2 = w.GrowingBadWugs.Contains(wugConfig.TemplateId) || w.GrowingGoodWugs.Contains(wugConfig.TemplateId) || w.GrownWug == wugConfig.TemplateId;
				bool result;
				if (flag2)
				{
					wugKingConfig = w;
					result = false;
				}
				else
				{
					result = true;
				}
				return result;
			});
			TextMeshProUGUI wugKingEffect = base.CGet<TextMeshProUGUI>("WugKingEffect");
			TextMeshProUGUI lineText1 = base.CGet<TextMeshProUGUI>("Line1");
			TextMeshProUGUI lineText2 = base.CGet<TextMeshProUGUI>("Line2");
			Func<ValueTuple<ItemKey, short>, bool> <>9__2;
			Action <>9__3;
			this._eatingItemAction = delegate()
			{
				bool flag2;
				WugKingItem wugKingConfig;
				if (wugKingConfig != null && this._eatingItemMonitor != null)
				{
					IEnumerable<ValueTuple<ItemKey, short>> eatingItemList = this._eatingItemMonitor.EatingItemList;
					Func<ValueTuple<ItemKey, short>, bool> predicate;
					if ((predicate = <>9__2) == null)
					{
						predicate = (<>9__2 = ((ValueTuple<ItemKey, short> x) => EatingItems.IsWugKing(x.Item1) && x.Item1.TemplateId == wugKingConfig.WugMedicine));
					}
					flag2 = eatingItemList.Any(predicate);
				}
				else
				{
					flag2 = false;
				}
				bool hasWugKing = flag2;
				wugKingConfig = wugKingConfig;
				bool valueOrDefault = ((wugKingConfig != null) ? new bool?(wugKingConfig.GrowingGoodWugs.Contains(wugConfig.TemplateId)) : null).GetValueOrDefault();
				if (valueOrDefault)
				{
					this.CGet<TextMeshProUGUI>("WugEffect").text = wugConfig.SpecialEffectDesc.SetColor("brightblue");
					lineText1.text = (lineText2.text = "-".SetColor("brightblue"));
					bool flag3 = hasWugKing;
					if (flag3)
					{
						wugKingEffect.text = wugKingConfig.GrowingGoodEffectDesc.SetColor("brightblue");
					}
				}
				WugKingItem wugKingConfig2 = wugKingConfig;
				bool valueOrDefault2 = ((wugKingConfig2 != null) ? new bool?(wugKingConfig2.GrowingBadWugs.Contains(wugConfig.TemplateId)) : null).GetValueOrDefault();
				if (valueOrDefault2)
				{
					this.CGet<TextMeshProUGUI>("WugEffect").text = wugConfig.SpecialEffectDesc.SetColor("brightred");
					lineText1.text = (lineText2.text = "-".SetColor("brightred"));
					bool flag4 = hasWugKing;
					if (flag4)
					{
						wugKingEffect.text = wugKingConfig.GrowingBadEffectDesc.SetColor("brightred");
					}
				}
				WugKingItem wugKingConfig3 = wugKingConfig;
				short? num = (wugKingConfig3 != null) ? new short?(wugKingConfig3.GrownWug) : null;
				int? num2 = (num != null) ? new int?((int)num.GetValueOrDefault()) : null;
				int templateId2 = (int)wugConfig.TemplateId;
				bool isGrown = num2.GetValueOrDefault() == templateId2 & num2 != null;
				bool flag5 = isGrown;
				if (flag5)
				{
					this.CGet<TextMeshProUGUI>("WugEffect").text = wugConfig.SpecialEffectDesc.SetColor("brightred");
					lineText1.text = (lineText2.text = "-".SetColor("brightred"));
					bool flag6 = hasWugKing;
					if (flag6)
					{
						wugKingEffect.text = wugKingConfig.GrownEffectDesc.SetColor("brightred");
					}
				}
				this.CGet<GameObject>("WugKingEffectLayout").SetActive(hasWugKing);
				YieldHelper instance = SingletonObject.getInstance<YieldHelper>();
				uint frame = 1U;
				Action job;
				if ((job = <>9__3) == null)
				{
					job = (<>9__3 = delegate()
					{
						LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform as RectTransform);
						canvasGroup.alpha = 1f;
					});
				}
				instance.DelayFrameDo(frame, job);
			};
			this._eatingItemMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<EatingItemMonitor>(charId, false);
			this._eatingItemMonitor.AddEatingItemListener(this._eatingItemAction);
			bool init = this._eatingItemMonitor.Init;
			if (init)
			{
				this._eatingItemAction();
			}
			TextMeshProUGUI killWugWay = base.CGet<TextMeshProUGUI>("KillWugWay");
			killWugWay.text = TipWugKingEffect.GetKillWugTip(wugConfig);
			TMPTextSpriteHelper helper = killWugWay.GetComponent<TMPTextSpriteHelper>();
			helper.Parse();
		}
	}

	// Token: 0x04001E78 RID: 7800
	private EatingItemMonitor _eatingItemMonitor;

	// Token: 0x04001E79 RID: 7801
	private Action _eatingItemAction;
}
