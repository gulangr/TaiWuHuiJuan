using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

// Token: 0x02000415 RID: 1045
public class UI_VillagerSelectMerchantType : UIBase
{
	// Token: 0x06003E4E RID: 15950 RVA: 0x001F4000 File Offset: 0x001F2200
	public override void OnInit(ArgumentBox argsBox)
	{
		RectTransform anchorItem;
		bool flag = argsBox.Get<RectTransform>("AnchorItem", out anchorItem);
		if (flag)
		{
			this._anchorItem = anchorItem;
			this._anchorOriginParent = anchorItem.parent;
			anchorItem.SetParent(base.transform);
		}
		RectTransform root = base.CGet<RectTransform>("Root");
		Vector3 pos;
		bool flag2 = argsBox.Get<Vector3>("Pos", out pos);
		if (flag2)
		{
			root.position = pos;
		}
		argsBox.Get("CharId", out this._charId);
		this._dropDownList.Clear();
		this._dropDownList.Add(LocalStringManager.Get(LanguageKey.LK_VillagerRole_Merchant_Random));
		foreach (MerchantTypeItem item in ((IEnumerable<MerchantTypeItem>)MerchantType.Instance))
		{
			bool flag3 = item.TemplateId >= 7;
			if (!flag3)
			{
				this._dropDownList.Add(item.Name);
			}
		}
		this._dropDown = base.CGet<CDropdownLegacy>("Dropdown");
		this._dropDown.onValueChanged.RemoveAllListeners();
		this._dropDown.ClearOptions();
		this._dropDown.AddOptions(this._dropDownList);
		TaiwuDomainMethod.AsyncCall.GetMerchantType(null, this._charId, delegate(int offset, RawDataPool dataPool)
		{
			sbyte value = 0;
			Serializer.Deserialize(dataPool, offset, ref value);
			bool flag4 = value == 7;
			if (flag4)
			{
				base.CGet<GameObject>("Image").SetActive(false);
				this._dropDown.value = 0;
			}
			else
			{
				base.CGet<GameObject>("Image").GetComponent<CImage>().SetSprite(MerchantType.Instance[value].Icon, false, null);
				base.CGet<GameObject>("Image").SetActive(true);
				this._dropDown.value = (int)(value + 1);
			}
			this._dropDown.onValueChanged.AddListener(delegate(int index)
			{
				sbyte setValue = (sbyte)((index == 0) ? 7 : (index - 1));
				TaiwuDomainMethod.Call.SetMerchantType(this._charId, setValue);
				UIManager.Instance.HideUI(UIElement.VillagerSelectMerchantType);
			});
		});
		UIElement element = this.Element;
		element.OnHide = (Action)Delegate.Combine(element.OnHide, new Action(delegate()
		{
			bool flag4 = this._anchorItem;
			if (flag4)
			{
				this._anchorItem.SetParent(this._anchorOriginParent);
			}
		}));
	}

	// Token: 0x06003E4F RID: 15951 RVA: 0x001F4188 File Offset: 0x001F2388
	protected override void OnClick(Transform btn)
	{
		bool flag = btn.name == "UIMask";
		if (flag)
		{
			this.QuickHide();
		}
	}

	// Token: 0x06003E50 RID: 15952 RVA: 0x001F41B4 File Offset: 0x001F23B4
	private void OnGUI()
	{
		bool flag = this._dropDown && this._dropDown.IsExpanded;
		if (flag)
		{
			Transform trans = this._dropDown.transform.Find("Dropdown List");
			bool flag2 = !trans;
			if (!flag2)
			{
				CToggleObsolete[] toggles = this._dropDown.GetComponentsInChildren<CToggleObsolete>();
				PositionFollower positionFollower = this._dropDown.GetComponentInChildren<PositionFollower>();
				foreach (CToggleObsolete togCell in toggles)
				{
					bool flag3 = !togCell.gameObject.activeSelf;
					if (!flag3)
					{
						togCell.transform.Find("Disable").gameObject.SetActive(togCell.isOn);
						bool flag4 = togCell.isOn && positionFollower;
						if (flag4)
						{
							positionFollower.Target = togCell.transform;
						}
					}
				}
				RectTransform content = trans.GetComponentInChildren<CScrollRectLegacy>().Content;
				int childCount = content.childCount;
				for (int i = 1; i < childCount; i++)
				{
					Transform item = content.GetChild(i);
					Transform image = item.Find("Layout/Image");
					bool flag5 = i == 1;
					if (flag5)
					{
						image.gameObject.SetActive(false);
					}
					else
					{
						CImage component = image.GetComponent<CImage>();
						if (component != null)
						{
							component.SetSprite(MerchantType.Instance[i - 2].Icon, false, null);
						}
						image.gameObject.SetActive(true);
					}
				}
			}
		}
	}

	// Token: 0x04002CF6 RID: 11510
	private CDropdownLegacy _dropDown;

	// Token: 0x04002CF7 RID: 11511
	private Transform _anchorOriginParent;

	// Token: 0x04002CF8 RID: 11512
	private Transform _anchorItem;

	// Token: 0x04002CF9 RID: 11513
	private int _charId;

	// Token: 0x04002CFA RID: 11514
	private readonly List<string> _dropDownList = new List<string>();
}
