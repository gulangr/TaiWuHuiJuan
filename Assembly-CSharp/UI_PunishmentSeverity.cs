using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x02000398 RID: 920
public class UI_PunishmentSeverity : UIBase
{
	// Token: 0x0600374F RID: 14159 RVA: 0x001BD194 File Offset: 0x001BB394
	public override void OnInit(ArgumentBox argsBox)
	{
	}

	// Token: 0x06003750 RID: 14160 RVA: 0x001BD197 File Offset: 0x001BB397
	public void Awake()
	{
		this._content = base.CGet<GameObject>("Content");
	}

	// Token: 0x06003751 RID: 14161 RVA: 0x001BD1AB File Offset: 0x001BB3AB
	private void OnEnable()
	{
		this.SetData();
	}

	// Token: 0x06003752 RID: 14162 RVA: 0x001BD1B5 File Offset: 0x001BB3B5
	private void OnDisable()
	{
		this.Clear();
	}

	// Token: 0x06003753 RID: 14163 RVA: 0x001BD1BF File Offset: 0x001BB3BF
	public override void QuickHide()
	{
		UIManager.Instance.HideUI(this.Element);
	}

	// Token: 0x06003754 RID: 14164 RVA: 0x001BD1D4 File Offset: 0x001BB3D4
	protected override void OnClick(Transform btn)
	{
		bool flag = btn.name == "ButtonConfirm";
		if (flag)
		{
			this.QuickHide();
		}
	}

	// Token: 0x06003755 RID: 14165 RVA: 0x001BD200 File Offset: 0x001BB400
	private void SetData()
	{
		this.Clear();
		foreach (PunishmentSeverityItem punishment in ((IEnumerable<PunishmentSeverityItem>)PunishmentSeverity.Instance))
		{
			GameObject obj = Object.Instantiate<GameObject>(this.punishmentTemplate, this._content.transform);
			Refers refers = obj.GetComponent<Refers>();
			refers.CGet<TextMeshProUGUI>("CrimeName").text = punishment.Name.SetColor(punishment.NameColor);
			refers.CGet<TextMeshProUGUI>("CrimeDesc").text = punishment.PunishmentDesc;
			refers.CGet<TextMeshProUGUI>("CrimeLevel").text = ((punishment.PrisonTime == 0) ? "/" : punishment.PrisonTime.ToString());
			refers.CGet<GameObject>("Icon").SetActive(punishment.PrisonTime != 0);
			obj.SetActive(true);
			this._objList.Add(obj);
		}
	}

	// Token: 0x06003756 RID: 14166 RVA: 0x001BD308 File Offset: 0x001BB508
	private void Clear()
	{
		foreach (GameObject obj in this._objList)
		{
			Object.Destroy(obj);
		}
		this._objList.Clear();
	}

	// Token: 0x04002808 RID: 10248
	public GameObject punishmentTemplate;

	// Token: 0x04002809 RID: 10249
	private GameObject _content;

	// Token: 0x0400280A RID: 10250
	private readonly List<GameObject> _objList = new List<GameObject>();
}
