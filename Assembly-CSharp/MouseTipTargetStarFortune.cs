using System;
using Config;
using FrameWork;
using GameData.Domains.Extra;
using GameData.Domains.World;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020002DD RID: 733
public class MouseTipTargetStarFortune : MouseTipBase
{
	// Token: 0x170004CE RID: 1230
	// (get) Token: 0x06002B86 RID: 11142 RVA: 0x00153732 File Offset: 0x00151932
	private TextMeshProUGUI Title
	{
		get
		{
			return base.CGet<TextMeshProUGUI>("Title");
		}
	}

	// Token: 0x170004CF RID: 1231
	// (get) Token: 0x06002B87 RID: 11143 RVA: 0x0015373F File Offset: 0x0015193F
	private TextMeshProUGUI Desc
	{
		get
		{
			return base.CGet<TextMeshProUGUI>("Desc");
		}
	}

	// Token: 0x170004D0 RID: 1232
	// (get) Token: 0x06002B88 RID: 11144 RVA: 0x0015374C File Offset: 0x0015194C
	private TextMeshProUGUI TxtStarFortune
	{
		get
		{
			return base.CGet<TextMeshProUGUI>("TxtStarFortune");
		}
	}

	// Token: 0x170004D1 RID: 1233
	// (get) Token: 0x06002B89 RID: 11145 RVA: 0x00153759 File Offset: 0x00151959
	private Refers WorldInfoTemplate
	{
		get
		{
			return base.CGet<Refers>("WorldInfoTemplate");
		}
	}

	// Token: 0x170004D2 RID: 1234
	// (get) Token: 0x06002B8A RID: 11146 RVA: 0x00153766 File Offset: 0x00151966
	private RectTransform WorldInfoLayout
	{
		get
		{
			return base.CGet<RectTransform>("WorldInfoLayout");
		}
	}

	// Token: 0x06002B8B RID: 11147 RVA: 0x00153773 File Offset: 0x00151973
	private void Awake()
	{
		this.Title.text = LocalStringManager.Get(LanguageKey.LK_SectMainStory_JieQing_MurderMap);
	}

	// Token: 0x06002B8C RID: 11148 RVA: 0x0015378C File Offset: 0x0015198C
	protected override void Init(ArgumentBox argsBox)
	{
		argsBox.Get("charId", out this._charId);
		ExtraDomainMethod.AsyncCall.GetCharacterExtraLegacyPointWorth(null, this._charId, delegate(int offset, RawDataPool dataPool)
		{
			int realWorth = 0;
			Serializer.Deserialize(dataPool, offset, ref realWorth);
			this.SetDesc(realWorth);
			this.SetStarFortune(realWorth);
		});
		WorldDomainMethod.AsyncCall.GetWorldCreationInfo(null, delegate(int offset, RawDataPool dataPool)
		{
			Serializer.Deserialize(dataPool, offset, ref this.worldCreationInfo);
			this.SetWorldCreationInfo(this.worldCreationInfo);
		});
	}

	// Token: 0x06002B8D RID: 11149 RVA: 0x001537D8 File Offset: 0x001519D8
	private void SetWorldCreationInfo(WorldCreationInfo worldCreationInfo)
	{
		CommonUtils.PrepareEnoughChildren(this.WorldInfoLayout, this.WorldInfoTemplate.gameObject, 3, null);
		sbyte i = 0;
		while ((int)i < this.WorldInfoLayout.childCount)
		{
			Refers refers = this.WorldInfoLayout.GetChild((int)i).GetComponent<Refers>();
			int level = worldCreationInfo.GetGroupLevel(i);
			WorldCreationGroupItem groupCfg = WorldCreationGroup.Instance[i];
			string dot = LocalStringManager.Get(LanguageKey.LK_Dot_Symbol);
			string colon = LocalStringManager.Get(LanguageKey.LK_Colon_Symbol);
			string levelText = LocalStringManager.Get(string.Format("LK_WorldCreation_GroupLevel_{0}", level));
			Color color = WorldDetailSettingGroup.GetLevelColor(level);
			int sum = worldCreationInfo.GetGroupLegacyBonusSum(i);
			refers.CGet<TextMeshProUGUI>("WorldDetailDesc").text = (groupCfg.Name + dot + levelText + colon).SetColor(color);
			refers.CGet<TextMeshProUGUI>("WorldDetailValue").text = string.Format("+{0}%", sum);
			i += 1;
		}
	}

	// Token: 0x06002B8E RID: 11150 RVA: 0x001538E1 File Offset: 0x00151AE1
	private void SetStarFortune(int realWorth)
	{
		this.TxtStarFortune.text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_StarFortuneAmount, realWorth).ColorReplace();
	}

	// Token: 0x06002B8F RID: 11151 RVA: 0x00153905 File Offset: 0x00151B05
	private void SetDesc(int realWorth)
	{
		this.Desc.text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_MurderSignDesc, realWorth);
	}

	// Token: 0x04001FC5 RID: 8133
	private int _charId;

	// Token: 0x04001FC6 RID: 8134
	private WorldCreationInfo worldCreationInfo;
}
