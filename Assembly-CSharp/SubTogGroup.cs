using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000360 RID: 864
[RequireComponent(typeof(CToggleGroupObsolete))]
public class SubTogGroup : MonoBehaviour
{
	// Token: 0x17000573 RID: 1395
	// (get) Token: 0x06003234 RID: 12852 RVA: 0x0018C2B3 File Offset: 0x0018A4B3
	public CToggleGroupObsolete ToggleGroup
	{
		get
		{
			return this._togGroup;
		}
	}

	// Token: 0x06003235 RID: 12853 RVA: 0x0018C2BB File Offset: 0x0018A4BB
	private void Awake()
	{
		this._togGroup = base.GetComponent<CToggleGroupObsolete>();
		this.Init(null, -1);
	}

	// Token: 0x06003236 RID: 12854 RVA: 0x0018C2D4 File Offset: 0x0018A4D4
	private void OnDisable()
	{
		bool flag = !this.IsToggleStruct;
		if (flag)
		{
			foreach (CToggleObsolete tog in this._togGroup.GetAll())
			{
				tog.GetComponent<CImage>().SetSprite(this.BackSprite[0], false, null);
			}
		}
	}

	// Token: 0x06003237 RID: 12855 RVA: 0x0018C350 File Offset: 0x0018A550
	public void Init(UnityAction<bool, int> toggleEvent = null, int targetKey = -1)
	{
		bool flag = !this.IsToggleStruct;
		if (flag)
		{
			using (List<CToggleObsolete>.Enumerator enumerator = this._togGroup.GetAll().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CToggleObsolete tog = enumerator.Current;
					PointerTrigger pointerTrigger = tog.gameObject.GetComponent<PointerTrigger>();
					bool flag2 = null == pointerTrigger;
					if (flag2)
					{
						pointerTrigger = tog.gameObject.AddComponent<PointerTrigger>();
					}
					tog.transition = Selectable.Transition.None;
					tog.onValueChanged.RemoveAllListeners();
					tog.onValueChanged.AddListener(delegate(bool on)
					{
						tog.transform.GetChild(1).gameObject.SetActive(on);
					});
					tog.transform.GetChild(1).gameObject.SetActive(tog.isOn);
					pointerTrigger.Toggle = tog;
					pointerTrigger.EnterEvent = new UnityEvent();
					pointerTrigger.ExitEvent = new UnityEvent();
					pointerTrigger.EnterEvent.AddListener(delegate()
					{
						tog.GetComponent<CImage>().SetSprite(this.BackSprite[1], false, null);
					});
					pointerTrigger.ExitEvent.AddListener(delegate()
					{
						tog.GetComponent<CImage>().SetSprite(this.BackSprite[0], false, null);
					});
				}
			}
		}
		else
		{
			bool flag3 = toggleEvent != null;
			if (flag3)
			{
				using (List<CToggleObsolete>.Enumerator enumerator2 = this._togGroup.GetAll().GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CToggleObsolete tog = enumerator2.Current;
						tog.onValueChanged.RemoveAllListeners();
						tog.onValueChanged.AddListener(delegate(bool isOn)
						{
							toggleEvent(isOn, tog.Key);
						});
					}
				}
			}
		}
		this._togGroup.InitPreOnToggle(targetKey);
	}

	// Token: 0x06003238 RID: 12856 RVA: 0x0018C598 File Offset: 0x0018A798
	public void UpdateGroup(List<SubTogGroup.TogContent> toggleContentList, UnityAction<bool, int> toggleEvent = null, int targetKey = -1)
	{
		bool flag = null == this._togGroup;
		if (flag)
		{
			this._togGroup = base.GetComponent<CToggleGroupObsolete>();
		}
		this._togGroup.Clear();
		CToggleObsolete[] togglesCore = base.transform.GetComponentsInTopChildren(true);
		for (int i = 0; i < toggleContentList.Count; i++)
		{
			SubTogGroup.TogContent toggleContent = toggleContentList[i];
			bool flag2 = togglesCore.CheckIndex(i);
			CToggleObsolete itemToggle;
			if (flag2)
			{
				itemToggle = togglesCore[i];
			}
			else
			{
				itemToggle = Object.Instantiate<CToggleObsolete>(this.TogglePrefab, base.transform, false);
			}
			itemToggle.gameObject.SetActive(true);
			itemToggle.onValueChanged.RemoveAllListeners();
			itemToggle.isOn = false;
			TextMeshProUGUI[] labels = itemToggle.GetComponentsInChildren<TextMeshProUGUI>(true);
			Array.ForEach<TextMeshProUGUI>(labels, delegate(TextMeshProUGUI e)
			{
				e.text = toggleContent.Content;
			});
			itemToggle.Key = i;
			this._togGroup.Add(itemToggle);
			TooltipInvoker tip = itemToggle.GetComponent<TooltipInvoker>();
			bool flag3 = tip;
			if (flag3)
			{
				bool flag4 = !toggleContent.TipTitle.IsNullOrEmpty();
				if (flag4)
				{
					tip.enabled = true;
					tip.Type = TipType.Simple;
					bool flag5 = tip.PresetParam == null || tip.PresetParam.Length < 2;
					if (flag5)
					{
						tip.PresetParam = new string[2];
					}
					tip.PresetParam[0] = toggleContent.TipTitle;
					tip.PresetParam[1] = toggleContent.TipContent;
				}
				else
				{
					tip.enabled = false;
				}
			}
		}
		for (int j = toggleContentList.Count; j < togglesCore.Length; j++)
		{
			togglesCore[j].gameObject.SetActive(false);
			togglesCore[j].onValueChanged.RemoveAllListeners();
		}
		this.Init(toggleEvent, targetKey);
	}

	// Token: 0x040024BE RID: 9406
	public CToggleObsolete TogglePrefab;

	// Token: 0x040024BF RID: 9407
	public string[] BackSprite = new string[]
	{
		"charactermenu3_01_mh_erji_1",
		"charactermenu3_01_mh_erji_3"
	};

	// Token: 0x040024C0 RID: 9408
	public bool IsToggleStruct = false;

	// Token: 0x040024C1 RID: 9409
	private CToggleGroupObsolete _togGroup;

	// Token: 0x02001720 RID: 5920
	public struct TogContent
	{
		// Token: 0x0400AA80 RID: 43648
		public string Content;

		// Token: 0x0400AA81 RID: 43649
		public string TipTitle;

		// Token: 0x0400AA82 RID: 43650
		public string TipContent;
	}
}
