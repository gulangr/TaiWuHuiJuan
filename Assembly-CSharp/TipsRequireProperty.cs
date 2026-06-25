using System;
using Config;
using TMPro;

// Token: 0x020002E9 RID: 745
public class TipsRequireProperty : Refers
{
	// Token: 0x06002BF0 RID: 11248 RVA: 0x00157E00 File Offset: 0x00156000
	public void SetData(short type, int currValue, int requireValue, string requireValueColorName = "", bool needGray = false)
	{
		CharacterPropertyDisplayItem configData = CharacterPropertyDisplay.Instance[CharacterPropertyReferenced.Instance[type].DisplayType];
		TextMeshProUGUI currValueBlue = base.CGet<TextMeshProUGUI>("CurrValueBlue");
		TextMeshProUGUI currValueRed = base.CGet<TextMeshProUGUI>("CurrValueRed");
		TextMeshProUGUI currValueText = (currValue < 0) ? null : ((currValue >= requireValue) ? currValueBlue : currValueRed);
		base.CGet<CImage>("PropertyIcon").SetSprite(string.IsNullOrEmpty(configData.TipsBigIcon) ? configData.TipsIcon : configData.TipsBigIcon, false, null);
		base.CGet<TextMeshProUGUI>("PropertyName").text = configData.ShortName;
		string requireValueText = (currValue >= 0) ? string.Format("/{0}", requireValue) : string.Format("-/{0}", requireValue);
		bool flag = !requireValueColorName.IsNullOrEmpty();
		if (flag)
		{
			requireValueText = requireValueText.SetColor(requireValueColorName);
		}
		base.CGet<TextMeshProUGUI>("RequireValue").text = requireValueText;
		bool flag2 = currValueText != null;
		if (flag2)
		{
			currValueText.text = CommonUtils.GetDisplayStringForNum(currValue, 10000);
		}
		currValueBlue.gameObject.SetActive(currValueText == currValueBlue);
		currValueRed.gameObject.SetActive(currValueText == currValueRed);
		DisableStyleRoot disable;
		bool flag3 = base.TryGetComponent<DisableStyleRoot>(out disable);
		if (flag3)
		{
			disable.SetStyleEffect(needGray, false);
		}
	}

	// Token: 0x04001FE8 RID: 8168
	private const int ProficiencyTypeId = 110;
}
