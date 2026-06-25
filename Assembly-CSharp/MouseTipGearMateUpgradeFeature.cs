using System;
using System.Collections.Generic;
using FrameWork;
using TMPro;

// Token: 0x020002A1 RID: 673
public class MouseTipGearMateUpgradeFeature : MouseTipBase
{
	// Token: 0x06002A2C RID: 10796 RVA: 0x00141CD4 File Offset: 0x0013FED4
	protected override void Init(ArgumentBox argsBox)
	{
		this.InitRefers();
		for (int i = 0; i < this._descList.Count; i++)
		{
			string desc;
			bool flag = argsBox.Get(string.Format("Desc{0}", i), out desc);
			if (flag)
			{
				this._descList[i].text = desc;
			}
		}
		string value;
		bool flag2 = argsBox.Get("Value", out value);
		if (flag2)
		{
			this._value.text = value;
		}
	}

	// Token: 0x06002A2D RID: 10797 RVA: 0x00141D54 File Offset: 0x0013FF54
	private void InitRefers()
	{
		this._descList = base.CGetList<TextMeshProUGUI>("Desc");
		this._value = base.CGet<TextMeshProUGUI>("Value");
		this._desc0 = base.CGet<TextMeshProUGUI>("Desc0");
	}

	// Token: 0x04001E99 RID: 7833
	private List<TextMeshProUGUI> _descList;

	// Token: 0x04001E9A RID: 7834
	private TextMeshProUGUI _value;

	// Token: 0x04001E9B RID: 7835
	private TextMeshProUGUI _desc0;
}
