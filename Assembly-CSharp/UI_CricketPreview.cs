using System;
using System.Collections.Generic;
using Config;
using TMPro;

// Token: 0x02000212 RID: 530
public class UI_CricketPreview : Refers
{
	// Token: 0x060021BA RID: 8634 RVA: 0x000F6A80 File Offset: 0x000F4C80
	private void Awake()
	{
		CDropdownLegacy partDrop = base.CGet<CDropdownLegacy>("Part");
		CDropdownLegacy colorDrop = base.CGet<CDropdownLegacy>("Color");
		List<string> partNameList = new List<string>();
		for (short id = 28; id < 109; id += 1)
		{
			partNameList.Add(CricketParts.Instance[id].Name);
		}
		partDrop.AddOptions(partNameList);
		partDrop.value = 0;
		partDrop.onValueChanged.AddListener(delegate(int value)
		{
			this.UpdateCricket();
		});
		List<string> colorNameList = new List<string>();
		for (short id2 = 0; id2 < 28; id2 += 1)
		{
			colorNameList.Add(CricketParts.Instance[id2].Name);
		}
		short id3 = 109;
		while ((int)id3 < CricketParts.Instance.Count)
		{
			colorNameList.Add(CricketParts.Instance[id3].Name);
			id3 += 1;
		}
		colorDrop.AddOptions(colorNameList);
		colorDrop.value = 0;
		colorDrop.onValueChanged.AddListener(delegate(int value)
		{
			this.UpdateCricket();
		});
		this._cricket = base.CGet<CricketView>("CricketView");
		this.UpdateCricket();
	}

	// Token: 0x060021BB RID: 8635 RVA: 0x000F6BB4 File Offset: 0x000F4DB4
	private void OnDestroy()
	{
		this._cricket.StopLoopSing();
	}

	// Token: 0x060021BC RID: 8636 RVA: 0x000F6BC4 File Offset: 0x000F4DC4
	private void UpdateCricket()
	{
		int colorDropValue = base.CGet<CDropdownLegacy>("Color").value;
		short colorId = (short)((colorDropValue < 28) ? colorDropValue : (colorDropValue - 28 + 109));
		short partId = (short)((colorId >= 109) ? (base.CGet<CDropdownLegacy>("Part").value + 28) : 0);
		this._cricket.SetCricketData(colorId, partId, false, null, false);
		this._cricket.Sing(true, true, true, -1f, null, 0f);
		base.CGet<TextMeshProUGUI>("Name").text = this._cricket.Name;
	}

	// Token: 0x060021BD RID: 8637 RVA: 0x000F6C58 File Offset: 0x000F4E58
	public void OnEnable()
	{
		this._cricket.gameObject.SetActive(true);
		this._cricket.Sing(true, true, true, -1f, null, 0f);
	}

	// Token: 0x060021BE RID: 8638 RVA: 0x000F6C87 File Offset: 0x000F4E87
	public void OnClose()
	{
		this._cricket.StopLoopSing();
		base.gameObject.SetActive(false);
	}

	// Token: 0x04001A13 RID: 6675
	private const short FirstPartId = 28;

	// Token: 0x04001A14 RID: 6676
	private const short FirstColorId = 109;

	// Token: 0x04001A15 RID: 6677
	private CricketView _cricket;
}
