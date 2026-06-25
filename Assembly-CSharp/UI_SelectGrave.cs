using System;
using System.Collections.Generic;
using FrameWork;
using Game.Components.Avatar;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UICommon.Character;

// Token: 0x020003A2 RID: 930
public class UI_SelectGrave : UIBase
{
	// Token: 0x060037ED RID: 14317 RVA: 0x001C2130 File Offset: 0x001C0330
	public override void OnInit(ArgumentBox argsBox)
	{
		this.NeedDataListenerId = true;
		argsBox.Get<Action<int>>("CallBack", out this._onSelected);
		argsBox.Get<List<int>>("GraveList", out this._graveIdList);
		bool showNone;
		bool flag = argsBox.Get("ShowNone", out showNone) && showNone;
		if (flag)
		{
			this._canSelectCharIdList.Add(-1);
			this._selectGraveId = -1;
		}
		string title;
		bool flag2 = argsBox.Get("Title", out title);
		if (flag2)
		{
			base.CGet<PopupWindow>("PopupWindowBase").SetTitle(title);
		}
		else
		{
			base.CGet<PopupWindow>("PopupWindowBase").SetTitle(LocalStringManager.Get(LanguageKey.LK_SelectGrave_Title));
		}
		this.Element.OnListenerIdReady = delegate()
		{
			this._graveList = new List<GraveDisplayData>();
			CharacterDomainMethod.Call.GetGraveDisplayDataList(this.Element.GameDataListenerId, this._graveIdList);
			this._charScroll = base.CGet<InfinityScrollLegacy>("CharacterScroll");
			this._charScroll.OnItemRender = new Action<int, Refers>(this.OnRenderChar);
			this._charScroll.OnItemHide = new Action<Refers>(this.OnItemHide);
			this._content = base.CGet<CToggleGroupObsolete>("Content");
			this._content.InitPreOnToggle(-1);
			this._charScroll.SetTogGroup(this._content, false, false);
			base.CGet<PopupWindow>("PopupWindowBase").OnConfirmClick = new Action(this.OnConfirm);
			base.CGet<PopupWindow>("PopupWindowBase").OnCancelClick = new Action(this.OnCancel);
		};
	}

	// Token: 0x060037EE RID: 14318 RVA: 0x001C21EC File Offset: 0x001C03EC
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 1)
			{
				bool flag = notification.DomainId == 4;
				if (flag)
				{
					bool flag2 = notification.MethodId == 51;
					if (flag2)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._graveList);
						for (int i = 0; i < this._graveList.Count; i++)
						{
							bool flag3 = this._graveList[i].Id > 0;
							if (flag3)
							{
								this._canSelectCharIdList.Add(this._graveList[i].Id);
							}
						}
						this._canSelectCharIdList.Sort();
						this._charScroll.UpdateData(this._canSelectCharIdList.Count);
						this.Element.ShowAfterRefresh();
					}
				}
			}
		}
	}

	// Token: 0x060037EF RID: 14319 RVA: 0x001C2334 File Offset: 0x001C0534
	private void OnDisable()
	{
		this._onSelected = null;
		this._canSelectCharIdList.Clear();
	}

	// Token: 0x060037F0 RID: 14320 RVA: 0x001C234A File Offset: 0x001C054A
	private void OnConfirm()
	{
		Action<int> onSelected = this._onSelected;
		if (onSelected != null)
		{
			onSelected(this._selectGraveId);
		}
		UIManager.Instance.HideUI(this.Element);
		this._onSelected = null;
	}

	// Token: 0x060037F1 RID: 14321 RVA: 0x001C237D File Offset: 0x001C057D
	private void OnCancel()
	{
		AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
		base.QuickHide();
		this._onSelected = null;
	}

	// Token: 0x060037F2 RID: 14322 RVA: 0x001C23A0 File Offset: 0x001C05A0
	private void OnRenderChar(int index, Refers refers)
	{
		int charId = this._canSelectCharIdList.CheckIndex(index) ? this._canSelectCharIdList[index] : -1;
		TooltipInvoker mouseTip = refers.CGet<TooltipInvoker>("MouseTip");
		mouseTip.enabled = (charId >= 0);
		refers.CGet<CImage>("None").gameObject.SetActive(charId < 0);
		Avatar avatar = refers.CGet<Avatar>("Avatar");
		avatar.gameObject.SetActive(false);
		bool flag = charId < 0;
		if (flag)
		{
			refers.CGet<TextMeshProUGUI>("Name").text = LocalStringManager.Get(LanguageKey.LK_None);
			refers.CGet<CImage>("GraveIcon").gameObject.SetActive(false);
		}
		else
		{
			GraveDisplayData grave = this._graveList.Find((GraveDisplayData g) => g.Id == charId);
			bool flag2 = grave != null;
			if (flag2)
			{
				refers.CGet<TextMeshProUGUI>("Name").text = NameCenter.GetDisplayName(ref grave.NameData, false);
				refers.CGet<CImage>("GraveIcon").gameObject.SetActive(true);
				refers.CGet<CImage>("GraveIcon").SetSprite("NPCFace_tomb", false, null);
			}
		}
		bool enabled = mouseTip.enabled;
		if (enabled)
		{
			bool flag3 = mouseTip.RuntimeParam == null;
			if (flag3)
			{
				mouseTip.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			mouseTip.Type = TipType.Character;
			mouseTip.RuntimeParam.Set("charId", charId);
		}
		CToggleObsolete toggle = refers.GetComponent<CToggleObsolete>();
		toggle.onValueChanged.RemoveAllListeners();
		toggle.interactable = !toggle.isOn;
		toggle.onValueChanged.AddListener(delegate(bool isOn)
		{
			if (isOn)
			{
				this._selectGraveId = charId;
				refers.CGet<CImage>("HoverLight").enabled = false;
			}
			else
			{
				refers.CGet<CImage>("HoverLight").enabled = true;
			}
		});
		bool flag4 = this._content.GetActive() == null;
		if (flag4)
		{
			toggle.isOn = true;
			this._content.NotifyToggle(toggle, true, false);
		}
	}

	// Token: 0x060037F3 RID: 14323 RVA: 0x001C25DC File Offset: 0x001C07DC
	private void OnItemHide(Refers refers)
	{
		List<CharacterUIElement> elementList = refers.UserObject as List<CharacterUIElement>;
		if (elementList != null)
		{
			elementList.ForEach(delegate(CharacterUIElement e)
			{
				e.CharacterId = -1;
			});
		}
	}

	// Token: 0x04002879 RID: 10361
	private Action<int> _onSelected;

	// Token: 0x0400287A RID: 10362
	private InfinityScrollLegacy _charScroll;

	// Token: 0x0400287B RID: 10363
	private List<int> _canSelectCharIdList = new List<int>();

	// Token: 0x0400287C RID: 10364
	private int _selectGraveId;

	// Token: 0x0400287D RID: 10365
	private List<int> _graveIdList;

	// Token: 0x0400287E RID: 10366
	private List<GraveDisplayData> _graveList;

	// Token: 0x0400287F RID: 10367
	private CToggleGroupObsolete _content;
}
