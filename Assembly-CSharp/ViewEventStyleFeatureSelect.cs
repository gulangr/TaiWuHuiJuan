using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using CharacterDataMonitor;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using GameData.Domains.Map;
using GameData.Domains.TaiwuEvent;
using TMPro;
using UnityEngine;

// Token: 0x02000407 RID: 1031
public class ViewEventStyleFeatureSelect : UIBase
{
	// Token: 0x06003D54 RID: 15700 RVA: 0x001ED108 File Offset: 0x001EB308
	private void Awake()
	{
		this.btnConfirm.ClearAndAddListener(new Action(this.OnConfirm));
		this.btnCancel.ClearAndAddListener(delegate
		{
			bool flag = !this._confirmNotHide;
			if (flag)
			{
				this.QuickHide();
			}
			Action cancelAction = this._cancelAction;
			if (cancelAction != null)
			{
				cancelAction();
			}
		});
		this._featureScroll.OnItemRender += delegate(int i, GameObject obj)
		{
			short featureId = this._featureList[i];
			Feature item = obj.GetComponent<Feature>();
			item.Set(featureId, -1, false, -1);
			Refers refers = obj.GetComponent<Refers>();
			CButton button;
			refers.CTryGet<CButton>("Btn", out button);
			GameObject selected;
			refers.CTryGet<GameObject>("Selected", out selected);
			bool flag = featureId == this._selectFeatureId;
			if (flag)
			{
				selected.gameObject.SetActive(true);
				this._selectedObj = selected;
			}
			else
			{
				selected.gameObject.SetActive(false);
			}
			button.ClearAndAddListener(delegate
			{
				bool flag2 = this._selectedObj != null;
				if (flag2)
				{
					this._selectedObj.SetActive(false);
				}
				this._selectedObj = selected;
				this._selectFeatureId = featureId;
				selected.SetActive(true);
				this.SetSelectedDisplay(true);
			});
		};
	}

	// Token: 0x06003D55 RID: 15701 RVA: 0x001ED160 File Offset: 0x001EB360
	private string HandleContentTag(string src)
	{
		Regex taiwuNameRegex = new Regex("<Character key=RoleTaiwu str=Name/>");
		bool flag = !string.IsNullOrEmpty(src) && taiwuNameRegex.IsMatch(src);
		if (flag)
		{
			src = taiwuNameRegex.Replace(src, (Match match) => SingletonObject.getInstance<BasicGameData>().TaiwuMonasticTitleOrDisplayName);
		}
		return src;
	}

	// Token: 0x06003D56 RID: 15702 RVA: 0x001ED1C0 File Offset: 0x001EB3C0
	public override void OnInit(ArgumentBox argsBox)
	{
		this._desc = null;
		argsBox.Get<List<short>>("featureList", out this._featureList);
		argsBox.Get("desc", out this._desc);
		argsBox.Get("texName", out this._texName);
		argsBox.Get("confirmNotHide", out this._confirmNotHide);
		argsBox.Get<Action<short>>("callBack", out this._confirmAction);
		argsBox.Get<Action>("cancelAction", out this._cancelAction);
		EventTextureManager eventTextureManager = SingletonObject.getInstance<EventTextureManager>();
		bool flag = !argsBox.Get("canExit", out this._canExit);
		if (flag)
		{
			this._canExit = true;
		}
		bool flag2 = !argsBox.Get("showExit", out this._showExit);
		if (flag2)
		{
			this._showExit = false;
		}
		bool flag3 = string.IsNullOrEmpty(this._texName);
		if (flag3)
		{
			WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
			MapBlockData blockData = worldMapModel.GetBlockData(worldMapModel.CurrentBlockId);
			this._texName = blockData.GetConfig().EventBack;
		}
		string texturePath;
		bool find = eventTextureManager.GetEventBackPath(this._texName, out texturePath);
		bool flag4 = find;
		if (flag4)
		{
			ResLoader.LoadModOrGameResource<Texture2D>(texturePath, delegate(Texture2D texture)
			{
				this.eventTexture.texture = texture;
				this.eventTexture.enabled = true;
			}, null);
		}
		else
		{
			Debug.LogWarning("TexName not find :" + this._texName);
		}
		int taiwuId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		this._featureMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<FeatureMonitor>(taiwuId, false);
		bool init = this._featureMonitor.Init;
		if (init)
		{
			this.<OnInit>g__Init|24_1();
		}
		else
		{
			this._featureMonitor.AddFeatureListener(new Action(this.<OnInit>g__Init|24_1));
		}
		bool flag5 = string.IsNullOrEmpty(this._desc);
		if (flag5)
		{
			this.noContent.SetActive(true);
			this.eventContent.gameObject.SetActive(false);
		}
		else
		{
			this.noContent.SetActive(false);
			this.eventContent.gameObject.SetActive(true);
			this._desc = this.HandleContentTag(this._desc);
			this.eventContent.text = this._desc;
		}
		this.btnCancel.interactable = this._canExit;
		this.btnCancel.gameObject.SetActive(this._showExit);
	}

	// Token: 0x06003D57 RID: 15703 RVA: 0x001ED400 File Offset: 0x001EB600
	private void SetSelectedDisplay(bool flag)
	{
		if (flag)
		{
			this.titleValue.text = "1/1";
		}
		else
		{
			this.titleValue.text = "0/1";
		}
		this.btnConfirm.interactable = flag;
	}

	// Token: 0x06003D58 RID: 15704 RVA: 0x001ED444 File Offset: 0x001EB644
	public override void QuickHide()
	{
		bool flag = !this._canExit;
		if (!flag)
		{
			Action cancelAction = this._cancelAction;
			if (cancelAction != null)
			{
				cancelAction();
			}
			bool flag2 = !this._confirmNotHide;
			if (flag2)
			{
				base.QuickHide();
			}
		}
	}

	// Token: 0x06003D59 RID: 15705 RVA: 0x001ED487 File Offset: 0x001EB687
	private void OnEnable()
	{
		GEvent.Add(UiEvents.HideEventStyleFeatureSelectPage, new GEvent.Callback(this.HideEventStyleFeatureSelectPage));
	}

	// Token: 0x06003D5A RID: 15706 RVA: 0x001ED4A3 File Offset: 0x001EB6A3
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.HideEventStyleFeatureSelectPage, new GEvent.Callback(this.HideEventStyleFeatureSelectPage));
	}

	// Token: 0x06003D5B RID: 15707 RVA: 0x001ED4BF File Offset: 0x001EB6BF
	private void HideEventStyleFeatureSelectPage(ArgumentBox argbox)
	{
		TaiwuEventDomainMethod.Call.TriggerListener("SelectCharacterFeatureFinish", true);
		base.QuickHide();
	}

	// Token: 0x06003D5C RID: 15708 RVA: 0x001ED4D8 File Offset: 0x001EB6D8
	public void Update()
	{
		bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
		if (flag)
		{
			this.OnConfirm();
		}
		bool flag2 = this.btnCancel.gameObject.activeSelf && this.btnCancel.interactable && (CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false) || CommonCommandKit.RightMouse.Check(this.Element, false, false, false, true, false));
		if (flag2)
		{
			this.QuickHide();
		}
	}

	// Token: 0x06003D5D RID: 15709 RVA: 0x001ED564 File Offset: 0x001EB764
	private void OnConfirm()
	{
		bool flag = !this._confirmNotHide;
		if (flag)
		{
			base.QuickHide();
		}
		short id = this._selectFeatureId;
		Action<short> confirmAction = this._confirmAction;
		if (confirmAction != null)
		{
			confirmAction(id);
		}
	}

	// Token: 0x06003D62 RID: 15714 RVA: 0x001ED6E8 File Offset: 0x001EB8E8
	[CompilerGenerated]
	private void <OnInit>g__Init|24_1()
	{
		this._selectFeatureId = -1;
		for (int i = this._featureList.Count - 1; i >= 0; i--)
		{
			bool flag = this._featureMonitor.FeatureIds.Contains(this._featureList[i]);
			if (flag)
			{
				this._selectFeatureId = this._featureList[i];
				break;
			}
		}
		bool flag2 = this._selectFeatureId == -1;
		if (flag2)
		{
			this._selectFeatureId = this._featureList[0];
		}
		this.<OnInit>g__InitScroll|24_2();
		this._featureMonitor.RemoveFeatureListener(new Action(this.<OnInit>g__Init|24_1));
		this.Element.ShowAfterRefresh();
	}

	// Token: 0x06003D63 RID: 15715 RVA: 0x001ED7A0 File Offset: 0x001EB9A0
	[CompilerGenerated]
	private void <OnInit>g__InitScroll|24_2()
	{
		bool useSmallItem = LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN;
		this._featureScroll.SetDataCount(0);
		this._featureScroll.UpdateStyle(this._featureScroll.Direction, useSmallItem ? this.smallColumnCount : this.largeColumnCount, this._featureScroll.gap, this._featureScroll.padding, useSmallItem ? this.featureTemplate.gameObject : this.featureLargeTemplate.gameObject);
		this._featureScroll.UpdateData(this._featureList.Count);
	}

	// Token: 0x04002C1F RID: 11295
	[SerializeField]
	private TextMeshProUGUI eventContent;

	// Token: 0x04002C20 RID: 11296
	[SerializeField]
	private GameObject noContent;

	// Token: 0x04002C21 RID: 11297
	[SerializeField]
	private CRawImage eventTexture;

	// Token: 0x04002C22 RID: 11298
	[SerializeField]
	private CButton btnConfirm;

	// Token: 0x04002C23 RID: 11299
	[SerializeField]
	private CButton btnCancel;

	// Token: 0x04002C24 RID: 11300
	[SerializeField]
	private Feature featureTemplate;

	// Token: 0x04002C25 RID: 11301
	[SerializeField]
	private Feature featureLargeTemplate;

	// Token: 0x04002C26 RID: 11302
	[SerializeField]
	private InfinityScroll _featureScroll;

	// Token: 0x04002C27 RID: 11303
	[SerializeField]
	private TextMeshProUGUI titleValue;

	// Token: 0x04002C28 RID: 11304
	[SerializeField]
	private int smallColumnCount = 4;

	// Token: 0x04002C29 RID: 11305
	[SerializeField]
	private int largeColumnCount = 2;

	// Token: 0x04002C2A RID: 11306
	private List<short> _featureList;

	// Token: 0x04002C2B RID: 11307
	private string _desc;

	// Token: 0x04002C2C RID: 11308
	private string _texName;

	// Token: 0x04002C2D RID: 11309
	private bool _confirmNotHide;

	// Token: 0x04002C2E RID: 11310
	private Action<short> _confirmAction;

	// Token: 0x04002C2F RID: 11311
	private Action _cancelAction;

	// Token: 0x04002C30 RID: 11312
	private GameObject _selectedObj;

	// Token: 0x04002C31 RID: 11313
	private short _selectFeatureId = -1;

	// Token: 0x04002C32 RID: 11314
	private FeatureMonitor _featureMonitor;

	// Token: 0x04002C33 RID: 11315
	private bool _canExit = true;

	// Token: 0x04002C34 RID: 11316
	private bool _showExit = false;
}
