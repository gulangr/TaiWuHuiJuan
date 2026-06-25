using System;
using System.Collections;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameDataExtensions;
using TMPro;
using UnityEngine;

namespace Game.Views.Building
{
	// Token: 0x02000BDF RID: 3039
	public class CricketCollectionJar : MonoBehaviour
	{
		// Token: 0x060098CF RID: 39119 RVA: 0x00472574 File Offset: 0x00470774
		public void Init(ViewCricketCollection parent, int index, Action<int> onClickJarSlot, Action<int> onClickCricketSlot, Action<int, bool> onClickCancel)
		{
			this._index = index;
			this._onClickJarSlot = onClickJarSlot;
			this._onClickCricketSlot = onClickCricketSlot;
			this._onClickCancel = onClickCancel;
			this.jarButton.ClearAndAddListener(new Action(this.OnClickJar));
			this.cricketButton.ClearAndAddListener(new Action(this.OnClickCricket));
			this.jarButtonCancel.ClearAndAddListener(new Action(this.OnClickJarCancel));
			this.cricketButtonCancel.ClearAndAddListener(new Action(this.OnClickCricketCancel));
			this.jarPointerTrigger.EnterEvent.ResetListener(new Action(this.OnEnterJar));
			this.jarPointerTrigger.ExitEvent.ResetListener(new Action(this.OnExitJar));
			this.cricketPointerTrigger.EnterEvent.ResetListener(new Action(this.OnEnterCricket));
			this.cricketPointerTrigger.ExitEvent.ResetListener(new Action(this.OnExitCricket));
			this.cricket.transform.SetParent(parent.cricketHolder);
			this.jarTips.enabled = false;
		}

		// Token: 0x060098D0 RID: 39120 RVA: 0x00472698 File Offset: 0x00470898
		private void RefreshJarTipContent()
		{
			bool flag = !this._canUse;
			if (flag)
			{
				this.jarTips.enabled = true;
				this.jarTips.Type = TipType.SingleDesc;
				TooltipInvoker tooltipInvoker = this.jarTips;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				this.jarTips.RuntimeParam.Set("arg0", LanguageKey.LK_Building_Cannot_Transfer_Warehouse.Tr());
			}
			else
			{
				bool flag2 = !this._hasJar;
				if (flag2)
				{
					this.jarTips.enabled = true;
					this.jarTips.Type = TipType.Simple;
					TooltipInvoker tooltipInvoker = this.jarTips;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					this.jarTips.RuntimeParam.Set("arg0", LanguageKey.LK_CricketCollection_PlaceCricketJar_Tip_Title.Tr());
					this.jarTips.RuntimeParam.Set("arg1", LanguageKey.LK_CricketCollection_PlaceCricketJar_Tip_Desc.Tr());
				}
				else
				{
					bool flag3 = !this._hasCricket;
					if (flag3)
					{
						this.jarTips.enabled = true;
						this.jarTips.Type = TipType.Simple;
						TooltipInvoker tooltipInvoker = this.jarTips;
						if (tooltipInvoker.RuntimeParam == null)
						{
							tooltipInvoker.RuntimeParam = new ArgumentBox();
						}
						this.jarTips.RuntimeParam.Set("arg0", LanguageKey.LK_CricketCollection_PlaceCricket_Tip_Title.Tr());
						this.jarTips.RuntimeParam.Set("arg1", LanguageKey.LK_CricketCollection_PlaceCricket_Tip_Desc.Tr());
					}
					else
					{
						this.jarTips.enabled = false;
					}
				}
			}
		}

		// Token: 0x060098D1 RID: 39121 RVA: 0x00472834 File Offset: 0x00470A34
		public void SetCanUse(bool value)
		{
			this._canUse = value;
			this.RefreshJarTipContent();
			if (value)
			{
				this.cricketPointerTrigger.enabled = true;
				this.jarPointerTrigger.enabled = true;
				this.cricketButton.interactable = true;
				this.jarButton.interactable = true;
				this.cricketTips.enabled = false;
			}
			else
			{
				this.cricketPointerTrigger.enabled = false;
				this.jarPointerTrigger.enabled = false;
				this.cricketButton.interactable = false;
				this.jarButton.interactable = false;
				this.cricketTips.enabled = true;
			}
		}

		// Token: 0x060098D2 RID: 39122 RVA: 0x004728E0 File Offset: 0x00470AE0
		public void SetEmptyJar()
		{
			this.nameBack.SetActive(false);
			this.propertyBack.SetActive(false);
			this.jarIcon.SetSprite("ui9_icon_cricket_collection_jar_empty", false, null);
			this.cricket.SetActive(false);
			this._hasJar = false;
			this.RefreshJarTipContent();
		}

		// Token: 0x060098D3 RID: 39123 RVA: 0x00472938 File Offset: 0x00470B38
		public void SetJar(short templateId)
		{
			MiscItem config = Misc.Instance[templateId];
			this.nameLabel.text = config.Name.SetGradeColor((int)config.Grade);
			this.nameBack.SetActive(true);
			this.propertyBack.SetActive(false);
			this.jarIcon.SetSprite(string.Format("{0}{1}", "ui9_icon_cricket_collection_jar_", config.Grade), false, null);
			this.cricket.SetActive(true);
			this._hasJar = true;
			this.RefreshJarTipContent();
		}

		// Token: 0x060098D4 RID: 39124 RVA: 0x004729CC File Offset: 0x00470BCC
		public void SetEmptyCricket()
		{
			bool flag = this._autoSingCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this._autoSingCoroutine);
				this._autoSingCoroutine = null;
			}
			this.cricketView.StopLoopSing();
			this.recover.SetActive(false);
			this.cricketView.gameObject.SetActive(false);
			this.dead.SetActive(false);
			this._hasCricket = false;
			this._lastCricketKey = ItemKey.Invalid;
			this.RefreshJarTipContent();
		}

		// Token: 0x060098D5 RID: 39125 RVA: 0x00472A50 File Offset: 0x00470C50
		public void SetCricket(ItemDisplayData data, bool isAlive, bool isRecovering)
		{
			this._cricketData = data;
			this.nameLabel.text = data.GetName(false).SetGradeColor((int)new ValueTuple<short, short>(data.CricketColorId, data.CricketPartId).CalcCricketGrade());
			this.RefreshPropertyDisplay();
			this.cricketView.SetCricketData(data.CricketColorId, data.CricketPartId, true, data, false);
			this.recover.SetActive(isRecovering);
			this.cricketView.gameObject.SetActive(true);
			this.dead.SetActive(!isAlive);
			this.cricket.SetActive(true);
			this.nameBack.SetActive(true);
			this.propertyBack.SetActive(true);
			this._hasCricket = true;
			this._isAlive = isAlive;
			if (isAlive)
			{
				this.cricketView.PlayAnimation(ECricketAnim.Idle, true, false);
				this.AutoSing();
			}
			else
			{
				this.cricketView.StopAnimation();
				this.cricketView.StopLoopSing();
				bool flag = this._autoSingCoroutine != null;
				if (flag)
				{
					base.StopCoroutine(this._autoSingCoroutine);
					this._autoSingCoroutine = null;
				}
			}
			this.RefreshJarTipContent();
		}

		// Token: 0x060098D6 RID: 39126 RVA: 0x00472B80 File Offset: 0x00470D80
		public void SetDisplayMode(CricketPropertyDisplayMode mode)
		{
			this._displayMode = mode;
			bool flag = this._hasCricket && this._cricketData != null;
			if (flag)
			{
				this.RefreshPropertyDisplay();
			}
		}

		// Token: 0x060098D7 RID: 39127 RVA: 0x00472BB4 File Offset: 0x00470DB4
		private void RefreshPropertyDisplay()
		{
			CricketPropertyDisplayMode displayMode = this._displayMode;
			CricketPropertyDisplayMode cricketPropertyDisplayMode = displayMode;
			if (cricketPropertyDisplayMode != CricketPropertyDisplayMode.Age)
			{
				if (cricketPropertyDisplayMode != CricketPropertyDisplayMode.Spirit)
				{
					this.propertyNameLabel.text = LanguageKey.LK_Durability.Tr();
					this.propertyValueLabel.text = string.Format("{0}/{1}", this._cricketData.Durability, this._cricketData.MaxDurability);
				}
				else
				{
					this.propertyNameLabel.text = LanguageKey.LK_Cricket_Spirit_Short.Tr();
					this.propertyValueLabel.text = ((this._cricketData.CricketData != null) ? this._cricketData.CricketData.Spirit.ToString() : "-");
				}
			}
			else
			{
				this.propertyNameLabel.text = LanguageKey.LK_SelectItem_Column_CricketAge.Tr();
				this.propertyValueLabel.text = ((this._cricketData.CricketData != null) ? string.Format("{0}/{1}", this._cricketData.CricketData.AgeStr, this._cricketData.CricketData.MaxAge) : "-");
			}
		}

		// Token: 0x060098D8 RID: 39128 RVA: 0x00472CDF File Offset: 0x00470EDF
		public void SetJarSelected()
		{
			this.jarSelected.SetActive(true);
		}

		// Token: 0x060098D9 RID: 39129 RVA: 0x00472CEF File Offset: 0x00470EEF
		public void SetCricketSelected()
		{
			this.cricketSelected.SetActive(true);
		}

		// Token: 0x060098DA RID: 39130 RVA: 0x00472CFF File Offset: 0x00470EFF
		public void SetDeselected()
		{
			this.jarSelected.SetActive(false);
			this.cricketSelected.SetActive(false);
		}

		// Token: 0x060098DB RID: 39131 RVA: 0x00472D1C File Offset: 0x00470F1C
		private void OnClickJar()
		{
			this.SetDeselected();
			this.SetJarSelected();
			this._onClickJarSlot(this._index);
		}

		// Token: 0x060098DC RID: 39132 RVA: 0x00472D3F File Offset: 0x00470F3F
		private void OnClickCricket()
		{
			this.SetDeselected();
			this.SetCricketSelected();
			this._onClickCricketSlot(this._index);
		}

		// Token: 0x060098DD RID: 39133 RVA: 0x00472D62 File Offset: 0x00470F62
		private void OnClickJarCancel()
		{
			this._onClickCancel(this._index, false);
			this.jarButtonCancel.gameObject.SetActive(false);
		}

		// Token: 0x060098DE RID: 39134 RVA: 0x00472D8A File Offset: 0x00470F8A
		private void OnClickCricketCancel()
		{
			this._onClickCancel(this._index, true);
			this.cricketButtonCancel.gameObject.SetActive(false);
		}

		// Token: 0x060098DF RID: 39135 RVA: 0x00472DB4 File Offset: 0x00470FB4
		private void OnEnterJar()
		{
			this.jarHover.SetActive(true);
			bool hasJar = this._hasJar;
			if (hasJar)
			{
				this.jarButtonCancel.gameObject.SetActive(true);
			}
		}

		// Token: 0x060098E0 RID: 39136 RVA: 0x00472DEB File Offset: 0x00470FEB
		private void OnExitJar()
		{
			this.jarHover.SetActive(false);
			this.jarButtonCancel.gameObject.SetActive(false);
		}

		// Token: 0x060098E1 RID: 39137 RVA: 0x00472E10 File Offset: 0x00471010
		private void OnEnterCricket()
		{
			this.cricketHover.SetActive(true);
			bool hasCricket = this._hasCricket;
			if (hasCricket)
			{
				this.cricketButtonCancel.gameObject.SetActive(true);
			}
		}

		// Token: 0x060098E2 RID: 39138 RVA: 0x00472E47 File Offset: 0x00471047
		private void OnExitCricket()
		{
			this.cricketHover.SetActive(false);
			this.cricketButtonCancel.gameObject.SetActive(false);
		}

		// Token: 0x060098E3 RID: 39139 RVA: 0x00472E6C File Offset: 0x0047106C
		private void AutoSing()
		{
			ItemDisplayData cricketData = this._cricketData;
			ItemKey cricketKey = (cricketData != null) ? cricketData.RealKey : ItemKey.Invalid;
			bool flag = !cricketKey.IsValid();
			if (!flag)
			{
				bool flag2 = cricketKey == this._lastCricketKey;
				if (!flag2)
				{
					this._lastCricketKey = cricketKey;
					bool flag3 = this._autoSingCoroutine != null;
					if (flag3)
					{
						base.StopCoroutine(this._autoSingCoroutine);
					}
					this._autoSingCoroutine = base.StartCoroutine(CricketCollectionJar.RandomDelay(delegate
					{
						this.cricketView.Sing(true, true, true, -1f, null, 0f);
					}));
				}
			}
		}

		// Token: 0x060098E4 RID: 39140 RVA: 0x00472EF1 File Offset: 0x004710F1
		private static IEnumerator RandomDelay(Action action)
		{
			float delayTime = Random.Range(0f, 2f);
			yield return new WaitForSeconds(delayTime);
			if (action != null)
			{
				action();
			}
			yield break;
		}

		// Token: 0x040075A3 RID: 30115
		public CButton jarButton;

		// Token: 0x040075A4 RID: 30116
		public PointerTrigger jarPointerTrigger;

		// Token: 0x040075A5 RID: 30117
		public CImage jarIcon;

		// Token: 0x040075A6 RID: 30118
		public GameObject nameBack;

		// Token: 0x040075A7 RID: 30119
		public GameObject propertyBack;

		// Token: 0x040075A8 RID: 30120
		public TextMeshProUGUI nameLabel;

		// Token: 0x040075A9 RID: 30121
		public TextMeshProUGUI propertyNameLabel;

		// Token: 0x040075AA RID: 30122
		public TextMeshProUGUI propertyValueLabel;

		// Token: 0x040075AB RID: 30123
		public GameObject recover;

		// Token: 0x040075AC RID: 30124
		public GameObject jarSelected;

		// Token: 0x040075AD RID: 30125
		public GameObject jarHover;

		// Token: 0x040075AE RID: 30126
		public CButton jarButtonCancel;

		// Token: 0x040075AF RID: 30127
		public GameObject cricket;

		// Token: 0x040075B0 RID: 30128
		public CricketView cricketView;

		// Token: 0x040075B1 RID: 30129
		public CButton cricketButton;

		// Token: 0x040075B2 RID: 30130
		public PointerTrigger cricketPointerTrigger;

		// Token: 0x040075B3 RID: 30131
		public GameObject dead;

		// Token: 0x040075B4 RID: 30132
		public GameObject cricketSelected;

		// Token: 0x040075B5 RID: 30133
		public GameObject cricketHover;

		// Token: 0x040075B6 RID: 30134
		public CButton cricketButtonCancel;

		// Token: 0x040075B7 RID: 30135
		public TooltipInvoker cricketTips;

		// Token: 0x040075B8 RID: 30136
		public TooltipInvoker jarTips;

		// Token: 0x040075B9 RID: 30137
		private int _index;

		// Token: 0x040075BA RID: 30138
		private bool _canUse;

		// Token: 0x040075BB RID: 30139
		private bool _hasJar;

		// Token: 0x040075BC RID: 30140
		private bool _hasCricket;

		// Token: 0x040075BD RID: 30141
		private bool _isAlive;

		// Token: 0x040075BE RID: 30142
		private ItemKey _lastCricketKey = ItemKey.Invalid;

		// Token: 0x040075BF RID: 30143
		private Coroutine _autoSingCoroutine;

		// Token: 0x040075C0 RID: 30144
		private ItemDisplayData _cricketData;

		// Token: 0x040075C1 RID: 30145
		private CricketPropertyDisplayMode _displayMode;

		// Token: 0x040075C2 RID: 30146
		private Action<int> _onClickJarSlot;

		// Token: 0x040075C3 RID: 30147
		private Action<int> _onClickCricketSlot;

		// Token: 0x040075C4 RID: 30148
		private Action<int, bool> _onClickCancel;
	}
}
