using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using Game.Views.Encyclopedia;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000269 RID: 617
public class MouseTipGeneralLines : MouseTipBase
{
	// Token: 0x17000478 RID: 1144
	// (get) Token: 0x060028D4 RID: 10452 RVA: 0x0012E210 File Offset: 0x0012C410
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060028D5 RID: 10453 RVA: 0x0012E213 File Offset: 0x0012C413
	protected override void Init(ArgumentBox argsBox)
	{
		this.Refresh(argsBox);
	}

	// Token: 0x060028D6 RID: 10454 RVA: 0x0012E21E File Offset: 0x0012C41E
	private new void OnDisable()
	{
	}

	// Token: 0x060028D7 RID: 10455 RVA: 0x0012E224 File Offset: 0x0012C424
	private void Update()
	{
		bool flag = this._linkType > 0 && CommonCommandKit.PrimaryInteraction.Check(this.Element, false, false, false, true, false);
		if (flag)
		{
			ViewEncyclopediaPanel.OpenLink(EncyclopediaTipLink.Instance[this._linkType]);
		}
	}

	// Token: 0x060028D8 RID: 10456 RVA: 0x0012E270 File Offset: 0x0012C470
	public override void Refresh(ArgumentBox argBox)
	{
		base.Refresh();
		this.Clear();
		string title;
		argBox.Get("Title", out title);
		int lineCount;
		argBox.Get("LineCount", out lineCount);
		GameObject encyclopediaHotKey = base.CGet<GameObject>("MouseTipHotKey");
		this._linkType = TooltipManager.GetEncyclopediaLinkId(argBox);
		encyclopediaHotKey.SetActive(this._linkType > 0);
		this.Element.ForceListenCommand = (this._linkType > 0);
		for (int i = 1; i <= lineCount; i++)
		{
			GameObject obj = null;
			GeneralLineData data;
			argBox.Get<GeneralLineData>(string.Format("LineData{0}", i), out data);
			switch (data.Type)
			{
			case 1:
				obj = Object.Instantiate<GameObject>(base.CGet<GameObject>("LineType_1"), base.transform);
				obj.GetComponent<Refers>().CGet<TextMeshProUGUI>("Text").text = data.Args[0].ColorReplace();
				break;
			case 2:
			{
				obj = Object.Instantiate<GameObject>(base.CGet<GameObject>("LineType_2"), base.transform);
				CImage icon = obj.GetComponent<Refers>().CGet<CImage>("Icon");
				icon.SetSprite(data.Args[0], false, null);
				float indent = icon.preferredWidth;
				List<object> extraArgs = data.ExtraArgs;
				bool flag = extraArgs != null && extraArgs.Count > 0;
				if (flag)
				{
					indent = Convert.ToSingle(data.ExtraArgs[0]);
				}
				obj.GetComponent<Refers>().CGet<TextMeshProUGUI>("Text").text = string.Format("<indent={0}>{1}", indent, data.Args[1].ColorReplace());
				break;
			}
			case 3:
			{
				obj = Object.Instantiate<GameObject>(base.CGet<GameObject>("LineType_3"), base.transform);
				TextMeshProUGUI label3 = obj.GetComponent<Refers>().CGet<TextMeshProUGUI>("Text");
				label3.text = data.Args[0].ColorReplace();
				label3.GetComponent<TMPTextSpriteHelper>().Parse();
				break;
			}
			case 4:
				obj = Object.Instantiate<GameObject>(base.CGet<GameObject>("LineType_4"), base.transform);
				obj.GetComponent<LayoutElement>().preferredHeight = data.PreferredHeight;
				break;
			case 5:
			{
				obj = Object.Instantiate<GameObject>(base.CGet<GameObject>("LineType_5"), base.transform);
				List<object> extraArgs2 = data.ExtraArgs;
				float extraLeftOffset = (extraArgs2 != null && extraArgs2.Count > 0) ? Convert.ToSingle(data.ExtraArgs[0]) : 0f;
				Refers refers = obj.GetComponent<Refers>();
				TextMeshProUGUI label4 = refers.CGet<TextMeshProUGUI>("Text");
				label4.text = string.Format("<indent={0}>{1}", 30, data.Args[0].ColorReplace());
				label4.GetComponent<TMPTextSpriteHelper>().Parse();
				RectTransform icon2 = refers.CGet<RectTransform>("Image");
				icon2.anchoredPosition = new Vector2(extraLeftOffset, icon2.anchoredPosition.y);
				obj.GetComponent<HorizontalLayoutGroup>().padding.left = (int)(-22f + extraLeftOffset);
				break;
			}
			case 6:
			{
				obj = Object.Instantiate<GameObject>(base.CGet<GameObject>("LineType_6"), base.transform);
				Refers refers = obj.GetComponent<Refers>();
				TextMeshProUGUI titleLabel = refers.CGet<TextMeshProUGUI>("Title");
				titleLabel.text = data.Args[0].ColorReplace();
				float indent = Convert.ToSingle(data.ExtraArgs[0]);
				refers.CGet<TextMeshProUGUI>("Text").text = string.Format("<indent={0}>{1}", indent, data.Args[1].ColorReplace());
				break;
			}
			case 7:
				obj = Object.Instantiate<GameObject>(base.CGet<GameObject>("LineType_7"), base.transform);
				obj.GetComponent<Refers>().CGet<TextMeshProUGUI>("Text").text = data.Args[0].ColorReplace();
				obj.GetComponent<Refers>().CGet<TextMeshProUGUI>("Text").GetComponent<TMPTextSpriteHelper>().Parse();
				break;
			case 8:
				obj = Object.Instantiate<GameObject>(base.CGet<GameObject>("LineType_8"), base.transform);
				obj.GetComponent<Refers>().CGet<TextMeshProUGUI>("Text1").text = data.Args[0].ColorReplace();
				obj.GetComponent<Refers>().CGet<CImage>("Icon").SetSprite(data.Args[1], false, null);
				obj.GetComponent<Refers>().CGet<TextMeshProUGUI>("Text2").text = data.Args[2].ColorReplace();
				break;
			case 9:
				obj = Object.Instantiate<GameObject>(base.CGet<GameObject>("LineType_9"), base.transform);
				obj.GetComponent<Refers>().CGet<TextMeshProUGUI>("Text1").text = data.Args[0].ColorReplace();
				obj.GetComponent<Refers>().CGet<CImage>("Icon").SetSprite(data.Args[1], false, null);
				obj.GetComponent<Refers>().CGet<TextMeshProUGUI>("Text2").text = data.Args[2].ColorReplace();
				break;
			case 10:
			{
				obj = Object.Instantiate<GameObject>(base.CGet<GameObject>("LineType_10"), base.transform);
				short templateId = Convert.ToInt16(data.ExtraArgs[0]);
				obj.transform.GetChild(0).GetComponent<HotkeyDisplay>().Refresh(templateId);
				break;
			}
			case 11:
			{
				obj = Object.Instantiate<GameObject>(base.CGet<GameObject>("LineType_11"), base.transform);
				Refers refer = obj.GetComponent<Refers>();
				refer.CGet<TextMeshProUGUI>("Text").text = data.Args[0].ColorReplace();
				bool flag2 = data.ExtraArgs != null;
				if (flag2)
				{
					int textLeftSpace = Convert.ToInt32(data.ExtraArgs[0]);
					VerticalLayoutGroup verticalLayout = obj.GetComponent<VerticalLayoutGroup>();
					verticalLayout.padding.left = textLeftSpace;
					bool flag3 = data.Args.Count > 1;
					if (flag3)
					{
						bool needParse = data.Args[1].Equals("true");
						bool flag4 = needParse;
						if (flag4)
						{
							refer.CGet<TMPTextSpriteHelper>("TMPTextSpriteHelper").Parse();
						}
					}
				}
				break;
			}
			}
			bool flag5 = obj == null;
			if (flag5)
			{
				return;
			}
			obj.SetActive(true);
			this._clonedObjects.Add(obj);
		}
		encyclopediaHotKey.transform.SetAsLastSibling();
		base.CGet<TextMeshProUGUI>("Title").text = title;
	}

	// Token: 0x060028D9 RID: 10457 RVA: 0x0012E954 File Offset: 0x0012CB54
	private void Clear()
	{
		foreach (GameObject obj in this._clonedObjects)
		{
			Object.Destroy(obj);
		}
		this._clonedObjects.Clear();
	}

	// Token: 0x04001DC5 RID: 7621
	private readonly List<GameObject> _clonedObjects = new List<GameObject>();

	// Token: 0x04001DC6 RID: 7622
	private int _linkType;
}
