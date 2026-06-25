using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FrameWork;
using Game.Components.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x0200083E RID: 2110
	public class MouseTipCharacterOnMapBlock : MouseTipBase
	{
		// Token: 0x17000C5C RID: 3164
		// (set) Token: 0x060066D1 RID: 26321 RVA: 0x002EE3DC File Offset: 0x002EC5DC
		private State State
		{
			set
			{
				this._state = value;
				switch (value)
				{
				case State.Simple:
					this.alt.Refresh(EHotKeyDisplayType.Detail);
					this.shift.Refresh(EHotKeyDisplayType.Interaction);
					this.alt.gameObject.SetActive(true);
					this.shift.gameObject.SetActive(this._canShowInteraction);
					break;
				case State.Detail:
					this.shift.gameObject.SetActive(false);
					this.alt.Refresh(EHotKeyDisplayType.CancelDetail);
					this.alt.gameObject.SetActive(true);
					break;
				case State.Interaction:
					this.alt.gameObject.SetActive(false);
					this.shift.Refresh(EHotKeyDisplayType.CancelInteraction);
					this.shift.gameObject.SetActive(this._canShowInteraction);
					break;
				}
			}
		}

		// Token: 0x060066D2 RID: 26322 RVA: 0x002EE4C4 File Offset: 0x002EC6C4
		public override bool CanShowWithArgumentBox(ArgumentBox argumentBox)
		{
			CharacterDisplayDataForMapBlock data;
			bool flag;
			if (argumentBox != null && argumentBox.Get<CharacterDisplayDataForMapBlock>("Data", out data))
			{
				byte? b = (data != null) ? new byte?(data.CreatingType) : null;
				int? num = (b != null) ? new int?((int)b.GetValueOrDefault()) : null;
				int num2 = 1;
				flag = (num.GetValueOrDefault() == num2 & num != null);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			int charId;
			return flag2 || (argumentBox != null && argumentBox.Get("CharId", out charId) && charId >= 0);
		}

		// Token: 0x060066D3 RID: 26323 RVA: 0x002EE568 File Offset: 0x002EC768
		protected override void Init(ArgumentBox argsBox)
		{
			this.Element.ForceListenCommand = true;
			this.canvasGroup.alpha = 0f;
			RectTransform mainRect = this.mainPanel.RectTransform;
			RectTransform itemRect = this.itemPanel.RectTransform;
			bool currIsLeft = mainRect.anchoredPosition.x < itemRect.anchoredPosition.x;
			bool flag;
			bool isMapBlockCharList = argsBox.Get("IsMapBlockCharList", out flag);
			bool isMainRole;
			bool isEvent = argsBox.Get("IsEvent", out isMainRole);
			this._baseTipFixedPosX = this.sizeDelta;
			this.TipFixedPosX = this.simpleDelta;
			CharacterDisplayDataForMapBlock data;
			bool flag2 = argsBox.Get<CharacterDisplayDataForMapBlock>("Data", out data);
			if (flag2)
			{
				this.NeedDataListenerId = false;
				this.OnDataReady(data);
			}
			else
			{
				bool showLeft = !isEvent || isMainRole;
				bool flag3 = currIsLeft ^ showLeft;
				if (flag3)
				{
					MouseTipCharacterOnMapBlock.<Init>g__RefreshPosition|21_0(mainRect, showLeft);
					MouseTipCharacterOnMapBlock.<Init>g__RefreshPosition|21_0(itemRect, showLeft);
					MouseTipCharacterOnMapBlock.<Init>g__RefreshPosition|21_0(this.attributePanel.RectTransform, showLeft);
					MouseTipCharacterOnMapBlock.<Init>g__RefreshPosition|21_0(this.interactionPanel.RectTransform, showLeft);
				}
				int charId;
				argsBox.Get("CharId", out charId);
				this.NeedDataListenerId = true;
				UIElement element = this.Element;
				AsyncMethodCallbackDelegate <>9__2;
				element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
				{
					IAsyncMethodRequestHandler <>4__this = this;
					int charId = charId;
					AsyncMethodCallbackDelegate callback;
					if ((callback = <>9__2) == null)
					{
						callback = (<>9__2 = delegate(int offset, RawDataPool pool)
						{
							CharacterDisplayDataForMapBlock displayData = null;
							Serializer.Deserialize(pool, offset, ref displayData);
							bool flag4 = displayData != null;
							if (flag4)
							{
								this.OnDataReady(displayData);
							}
						});
					}
					CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForMapBlock(<>4__this, charId, callback);
				}));
			}
		}

		// Token: 0x060066D4 RID: 26324 RVA: 0x002EE6C0 File Offset: 0x002EC8C0
		private void OnDataReady(CharacterDisplayDataForMapBlock data)
		{
			bool flag = data.CreatingType != 1;
			if (!flag)
			{
				bool flag2 = data.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId || data.CreatingType != 1 || data.FavorabilityToTaiwu == short.MinValue;
				if (flag2)
				{
					this._canShowInteraction = false;
					this._noInteractionReason = 0;
				}
				else
				{
					Dictionary<short, bool> visibleCharacterInteractionEventOptionDict = data.VisibleCharacterInteractionEventOptionDict;
					this._canShowInteraction = (visibleCharacterInteractionEventOptionDict != null && visibleCharacterInteractionEventOptionDict.Count > 0);
					this._noInteractionReason = data.NoInteractionReason;
				}
				this.State = State.Simple;
				this.mainPanel.Set(data);
				this.interactionPanel.Set(data);
				this.attributePanel.Set(data);
				this.itemPanel.Set(data);
				this.RefreshPageState();
				this.Element.ShowAfterRefresh();
				this.canvasGroup.alpha = 1f;
			}
		}

		// Token: 0x060066D5 RID: 26325 RVA: 0x002EE7B0 File Offset: 0x002EC9B0
		private void Update()
		{
			bool flag = this.Element == null || this._state == State.Hidden;
			if (!flag)
			{
				switch (this._state)
				{
				case State.Simple:
				case State.Interaction:
					this.TipFixedPosX = this.simpleDelta;
					break;
				case State.Detail:
					this.TipFixedPosX = this.sizeDelta;
					break;
				}
				bool flag2 = this._state != State.Interaction;
				if (flag2)
				{
					State state = CommonCommandKit.Alt.Check(this.Element, true, false, false, true, false) ? State.Detail : State.Simple;
					bool flag3 = this._state != state;
					if (flag3)
					{
						this.State = state;
						this.RefreshPageState();
					}
				}
				bool flag4 = this._canShowInteraction && this._state != State.Detail;
				if (flag4)
				{
					State state2 = CommonCommandKit.Shift.Check(this.Element, true, false, false, true, false) ? State.Interaction : State.Simple;
					bool flag5 = this._state != state2;
					if (flag5)
					{
						this.State = state2;
						this.RefreshPageState();
					}
				}
			}
		}

		// Token: 0x060066D6 RID: 26326 RVA: 0x002EE8D4 File Offset: 0x002ECAD4
		private void RefreshPageState()
		{
			RectTransform rectTransform = base.RectTransform;
			Vector2 v = base.RectTransform.sizeDelta;
			State state = this._state;
			rectTransform.sizeDelta = v.SetY((state == State.Simple || state == State.Interaction) ? this.floatingY : this.fixedY);
			bool showDetail = this._state == State.Detail;
			bool showInteraction = this._state == State.Interaction;
			bool showRelationship = showDetail || showInteraction;
			this.attributePanel.gameObject.SetActive(showDetail);
			this.itemPanel.gameObject.SetActive(showDetail);
			this.interactionPanel.gameObject.SetActive(showInteraction);
		}

		// Token: 0x060066D8 RID: 26328 RVA: 0x002EE9A8 File Offset: 0x002ECBA8
		[CompilerGenerated]
		internal static void <Init>g__RefreshPosition|21_0(RectTransform rectTransform, bool showLeft)
		{
			Vector2 position = rectTransform.anchoredPosition;
			position.x *= -1f;
			Vector2 pivot = rectTransform.pivot;
			pivot.x = (float)(showLeft ? 0 : 1);
			rectTransform.pivot = (rectTransform.anchorMax = (rectTransform.anchorMin = pivot));
			rectTransform.anchoredPosition = position;
		}

		// Token: 0x0400483E RID: 18494
		[SerializeField]
		private MainPanel mainPanel;

		// Token: 0x0400483F RID: 18495
		[SerializeField]
		private AttributePanel attributePanel;

		// Token: 0x04004840 RID: 18496
		[SerializeField]
		private ItemPanel itemPanel;

		// Token: 0x04004841 RID: 18497
		[SerializeField]
		private InteractionPanel interactionPanel;

		// Token: 0x04004842 RID: 18498
		[SerializeField]
		private CanvasGroup canvasGroup;

		// Token: 0x04004843 RID: 18499
		[SerializeField]
		private Game.Components.Common.HotkeyDisplay alt;

		// Token: 0x04004844 RID: 18500
		[SerializeField]
		private Game.Components.Common.HotkeyDisplay shift;

		// Token: 0x04004845 RID: 18501
		private Refers _basicInfoLayout;

		// Token: 0x04004846 RID: 18502
		private Refers _itemPanel;

		// Token: 0x04004847 RID: 18503
		private readonly List<short> _showFeatureList = new List<short>();

		// Token: 0x04004848 RID: 18504
		private bool _canShowInteraction;

		// Token: 0x04004849 RID: 18505
		private int _noInteractionReason;

		// Token: 0x0400484A RID: 18506
		private float _baseTipFixedPosX;

		// Token: 0x0400484B RID: 18507
		[SerializeField]
		private float fixedY = 1430f;

		// Token: 0x0400484C RID: 18508
		[SerializeField]
		private float floatingY = 1275f;

		// Token: 0x0400484D RID: 18509
		private State _state;

		// Token: 0x0400484E RID: 18510
		[SerializeField]
		private float sizeDelta = 218f;

		// Token: 0x0400484F RID: 18511
		[SerializeField]
		private float simpleDelta = 320f;
	}
}
