using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.GameDataBridge;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F0E RID: 3854
	public class AttributeAndInjuryDynamic : MonoBehaviour
	{
		// Token: 0x17001429 RID: 5161
		// (get) Token: 0x0600B1A4 RID: 45476 RVA: 0x0050ED23 File Offset: 0x0050CF23
		// (set) Token: 0x0600B1A5 RID: 45477 RVA: 0x0050ED2C File Offset: 0x0050CF2C
		public int CharacterId
		{
			get
			{
				return this._characterId;
			}
			set
			{
				bool flag = this._characterId == value;
				if (flag)
				{
					Action onRequestDataCallBack = this.OnRequestDataCallBack;
					if (onRequestDataCallBack != null)
					{
						onRequestDataCallBack();
					}
				}
				else
				{
					this._characterId = value;
					this.attributeDynamic.InitWithListenerId(value);
					this.injuryDynamic.InitWithListenerId(value);
				}
			}
		}

		// Token: 0x1700142A RID: 5162
		// (get) Token: 0x0600B1A6 RID: 45478 RVA: 0x0050ED7D File Offset: 0x0050CF7D
		public Injury Injury
		{
			get
			{
				return this.injuryDynamic.Injury;
			}
		}

		// Token: 0x1700142B RID: 5163
		// (get) Token: 0x0600B1A7 RID: 45479 RVA: 0x0050ED8A File Offset: 0x0050CF8A
		public Attribute Attribute
		{
			get
			{
				return this.attributeDynamic.Attribue;
			}
		}

		// Token: 0x0600B1A8 RID: 45480 RVA: 0x0050ED98 File Offset: 0x0050CF98
		private void Awake()
		{
			this.toggleGroup.Init(this.defaultInjury ? 1 : 0);
			this.toggleGroup.OnActiveIndexChange += this.OnTabChanged;
			this.OnTabChanged(this.toggleGroup.GetActiveIndex(), -1);
		}

		// Token: 0x0600B1A9 RID: 45481 RVA: 0x0050EDE9 File Offset: 0x0050CFE9
		private void OnDestroy()
		{
			this.toggleGroup.OnActiveIndexChange -= this.OnTabChanged;
		}

		// Token: 0x0600B1AA RID: 45482 RVA: 0x0050EE04 File Offset: 0x0050D004
		private void OnEnable()
		{
			this._listenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyGameData));
			this.attributeDynamic.SetExternalListenerId(this._listenerId);
			this.injuryDynamic.SetExternalListenerId(this._listenerId);
			bool flag = this._characterId >= 0;
			if (flag)
			{
				this.attributeDynamic.InitWithListenerId(this._characterId);
				this.injuryDynamic.InitWithListenerId(this._characterId);
			}
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
		}

		// Token: 0x0600B1AB RID: 45483 RVA: 0x0050EE9C File Offset: 0x0050D09C
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
			bool flag = this._listenerId != 0;
			if (flag)
			{
				this.attributeDynamic.UnbindMonitor();
				this.injuryDynamic.UnbindMonitor();
				GameDataBridge.UnregisterListener(this._listenerId);
				this._listenerId = 0;
			}
		}

		// Token: 0x0600B1AC RID: 45484 RVA: 0x0050EEFC File Offset: 0x0050D0FC
		private void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			this.attributeDynamic.OnNotifyGameData(notifications);
			this.injuryDynamic.OnNotifyGameData(notifications);
			Action onRequestDataCallBack = this.OnRequestDataCallBack;
			if (onRequestDataCallBack != null)
			{
				onRequestDataCallBack();
			}
		}

		// Token: 0x0600B1AD RID: 45485 RVA: 0x0050EF2B File Offset: 0x0050D12B
		private void Update()
		{
			this.attributeDynamic.ProcessPendingRequest();
			this.injuryDynamic.ProcessPendingRequest();
		}

		// Token: 0x0600B1AE RID: 45486 RVA: 0x0050EF48 File Offset: 0x0050D148
		private void OnTopUiChanged(ArgumentBox args)
		{
			bool flag = this.CharacterId >= 0;
			if (flag)
			{
				this.Injury.RefreshAllHealBtn(this.CharacterId, false);
			}
		}

		// Token: 0x0600B1AF RID: 45487 RVA: 0x0050EF79 File Offset: 0x0050D179
		public void SwitchToAttribute()
		{
			this.toggleGroup.Set(0, false);
		}

		// Token: 0x0600B1B0 RID: 45488 RVA: 0x0050EF8A File Offset: 0x0050D18A
		public void SwitchToInjury()
		{
			this.toggleGroup.Set(1, false);
		}

		// Token: 0x0600B1B1 RID: 45489 RVA: 0x0050EF9B File Offset: 0x0050D19B
		public void SwitchToAttributeWithoutNotify()
		{
			this.toggleGroup.SetWithoutNotify(0);
			this.RefreshTabVisibility(0);
		}

		// Token: 0x0600B1B2 RID: 45490 RVA: 0x0050EFB3 File Offset: 0x0050D1B3
		public void SwitchToInjuryWithoutNotify()
		{
			this.toggleGroup.SetWithoutNotify(1);
			this.RefreshTabVisibility(1);
		}

		// Token: 0x0600B1B3 RID: 45491 RVA: 0x0050EFCC File Offset: 0x0050D1CC
		public void Refresh()
		{
			int activeIndex = this.toggleGroup.GetActiveIndex();
			bool flag = activeIndex == 0;
			if (flag)
			{
				this.attributeDynamic.Refresh();
			}
			else
			{
				bool flag2 = activeIndex == 1;
				if (flag2)
				{
					this.injuryDynamic.Refresh();
				}
			}
		}

		// Token: 0x0600B1B4 RID: 45492 RVA: 0x0050F010 File Offset: 0x0050D210
		private void OnTabChanged(int newIndex, int oldIndex)
		{
			this.RefreshTabVisibility(newIndex);
		}

		// Token: 0x0600B1B5 RID: 45493 RVA: 0x0050F01C File Offset: 0x0050D21C
		private void RefreshTabVisibility(int activeIndex)
		{
			this.attributeDynamic.gameObject.SetActive(activeIndex == 0);
			this.injuryDynamic.gameObject.SetActive(activeIndex == 1);
			CanvasGroup attributeDynamicCg = this.attributeDynamic.GetComponent<CanvasGroup>();
			bool flag = attributeDynamicCg != null;
			if (flag)
			{
				attributeDynamicCg.alpha = ((activeIndex == 0) ? 1f : 0f);
			}
			CanvasGroup injuryDynamicCg = this.injuryDynamic.GetComponent<CanvasGroup>();
			bool flag2 = injuryDynamicCg != null;
			if (flag2)
			{
				injuryDynamicCg.alpha = ((activeIndex == 1) ? 1f : 0f);
			}
		}

		// Token: 0x0600B1B6 RID: 45494 RVA: 0x0050F0B0 File Offset: 0x0050D2B0
		public void TempSetTableState(int state)
		{
			int activeIndex = this.toggleGroup.GetActiveIndex();
			bool flag = activeIndex == state;
			if (!flag)
			{
				this._storedTabIndex = activeIndex;
				this.toggleGroup.Set(state, false);
			}
		}

		// Token: 0x0600B1B7 RID: 45495 RVA: 0x0050F0EC File Offset: 0x0050D2EC
		public void BackToPrevState()
		{
			int activeIndex = this.toggleGroup.GetActiveIndex();
			bool flag = activeIndex == this._storedTabIndex;
			if (flag)
			{
				this._storedTabIndex = -1;
			}
			bool flag2 = this._storedTabIndex < 0;
			if (!flag2)
			{
				this.toggleGroup.Set(this._storedTabIndex, false);
				this._storedTabIndex = -1;
			}
		}

		// Token: 0x040089A7 RID: 35239
		public const int TabIndexAttribute = 0;

		// Token: 0x040089A8 RID: 35240
		public const int TabIndexInjury = 1;

		// Token: 0x040089A9 RID: 35241
		[SerializeField]
		internal CToggleGroup toggleGroup;

		// Token: 0x040089AA RID: 35242
		[SerializeField]
		private AttributeDynamic attributeDynamic;

		// Token: 0x040089AB RID: 35243
		[SerializeField]
		private InjuryDynamic injuryDynamic;

		// Token: 0x040089AC RID: 35244
		[SerializeField]
		private bool defaultInjury = false;

		// Token: 0x040089AD RID: 35245
		private int _characterId = -1;

		// Token: 0x040089AE RID: 35246
		private int _listenerId;

		// Token: 0x040089AF RID: 35247
		private int _storedTabIndex = -1;

		// Token: 0x040089B0 RID: 35248
		public Action OnRequestDataCallBack;

		// Token: 0x040089B1 RID: 35249
		[NonSerialized]
		public bool AwakeWithInjury = false;
	}
}
