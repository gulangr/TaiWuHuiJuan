using System;
using Config;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Bottom
{
	// Token: 0x02000C3C RID: 3132
	public class MapElementDisplayRulePanel : MonoBehaviour
	{
		// Token: 0x170010C9 RID: 4297
		// (get) Token: 0x06009F13 RID: 40723 RVA: 0x004A5BDA File Offset: 0x004A3DDA
		private GlobalSettings GlobalSettings
		{
			get
			{
				return SingletonObject.getInstance<GlobalSettings>();
			}
		}

		// Token: 0x06009F14 RID: 40724 RVA: 0x004A5BE4 File Offset: 0x004A3DE4
		private void Awake()
		{
			this.toggleCharacterCount.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleCharacterCountValueChanged));
			this.toggleCharacterAvatar.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleCharacterAvatarValueChanged));
			this.toggleMapElement.onValueChanged.AddListener(new UnityAction<bool>(this.OnTogglMapElementValueChanged));
			this.toggleMapInteract.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleMapInteractValueChanged));
			this.InitToggleTip(this.characterCount, this.textCharacterCount, 0);
			this.InitToggleTip(this.characterAvatar, this.textCharacterAvatar, 1);
			this.InitToggleTip(this.mapElement, this.textMapElement, 2);
			this.InitToggleTip(this.mapInteract, this.textMapInteract, 3);
		}

		// Token: 0x06009F15 RID: 40725 RVA: 0x004A5CB8 File Offset: 0x004A3EB8
		private void OnEnable()
		{
			this.toggleCharacterCount.SetIsOnWithoutNotify(this.GlobalSettings.GetMapElementDisplayRuleGroupState(0));
			this.toggleCharacterAvatar.SetIsOnWithoutNotify(this.GlobalSettings.GetMapElementDisplayRuleGroupState(1));
			this.toggleMapElement.SetIsOnWithoutNotify(this.GlobalSettings.GetMapElementDisplayRuleGroupState(2));
			this.toggleMapInteract.SetIsOnWithoutNotify(this.GlobalSettings.GetMapElementDisplayRuleGroupState(3));
		}

		// Token: 0x06009F16 RID: 40726 RVA: 0x004A5D28 File Offset: 0x004A3F28
		private void InitToggleTip(GameObject go, TextMeshProUGUI txt, short templateId)
		{
			MapElementDisplayRuleGroupItem groupConfig = MapElementDisplayRuleGroup.Instance[templateId];
			TooltipInvoker tip = go.GetOrAddComponent<TooltipInvoker>();
			tip.Type = TipType.Simple;
			tip.PresetParam = new string[]
			{
				groupConfig.Name,
				groupConfig.Desc
			};
			tip.triggerByChildRaycast = true;
			txt.text = groupConfig.Name;
		}

		// Token: 0x06009F17 RID: 40727 RVA: 0x004A5D82 File Offset: 0x004A3F82
		private void OnToggleCharacterCountValueChanged(bool isOn)
		{
			this.GlobalSettings.SetMapElementDisplayRuleGroupState(0, isOn);
			this.GlobalSettings.SaveSettings();
			GEvent.OnEvent(UiEvents.OnForceRefreshAllMapBlock, null);
		}

		// Token: 0x06009F18 RID: 40728 RVA: 0x004A5DAD File Offset: 0x004A3FAD
		private void OnToggleCharacterAvatarValueChanged(bool isOn)
		{
			this.GlobalSettings.SetMapElementDisplayRuleGroupState(1, isOn);
			this.GlobalSettings.SaveSettings();
			GEvent.OnEvent(UiEvents.OnForceRefreshAllMapBlock, null);
		}

		// Token: 0x06009F19 RID: 40729 RVA: 0x004A5DD8 File Offset: 0x004A3FD8
		private void OnTogglMapElementValueChanged(bool isOn)
		{
			this.GlobalSettings.SetMapElementDisplayRuleGroupState(2, isOn);
			this.GlobalSettings.SaveSettings();
			GEvent.OnEvent(UiEvents.OnForceRefreshAllMapBlock, null);
		}

		// Token: 0x06009F1A RID: 40730 RVA: 0x004A5E03 File Offset: 0x004A4003
		private void OnToggleMapInteractValueChanged(bool isOn)
		{
			this.GlobalSettings.SetMapElementDisplayRuleGroupState(3, isOn);
			this.GlobalSettings.SaveSettings();
			GEvent.OnEvent(UiEvents.OnForceRefreshAllMapBlock, null);
		}

		// Token: 0x06009F1B RID: 40731 RVA: 0x004A5E30 File Offset: 0x004A4030
		public void ToggleAll()
		{
			bool newCharacterCount = !this.toggleCharacterCount.isOn;
			bool newCharacterAvatar = !this.toggleCharacterAvatar.isOn;
			bool newMapElement = !this.toggleMapElement.isOn;
			bool newMapInteract = !this.toggleMapInteract.isOn;
			this.GlobalSettings.SetMapElementDisplayRuleGroupState(0, newCharacterCount);
			this.GlobalSettings.SetMapElementDisplayRuleGroupState(1, newCharacterAvatar);
			this.GlobalSettings.SetMapElementDisplayRuleGroupState(2, newMapElement);
			this.GlobalSettings.SetMapElementDisplayRuleGroupState(3, newMapInteract);
			this.GlobalSettings.SaveSettings();
			GEvent.OnEvent(UiEvents.OnForceRefreshAllMapBlock, null);
			this.toggleCharacterCount.SetIsOnWithoutNotify(newCharacterCount);
			this.toggleCharacterAvatar.SetIsOnWithoutNotify(newCharacterAvatar);
			this.toggleMapElement.SetIsOnWithoutNotify(newMapElement);
			this.toggleMapInteract.SetIsOnWithoutNotify(newMapInteract);
		}

		// Token: 0x04007B03 RID: 31491
		[SerializeField]
		private GameObject characterCount;

		// Token: 0x04007B04 RID: 31492
		[SerializeField]
		private GameObject characterAvatar;

		// Token: 0x04007B05 RID: 31493
		[SerializeField]
		private GameObject mapElement;

		// Token: 0x04007B06 RID: 31494
		[SerializeField]
		private GameObject mapInteract;

		// Token: 0x04007B07 RID: 31495
		[SerializeField]
		private TextMeshProUGUI textCharacterCount;

		// Token: 0x04007B08 RID: 31496
		[SerializeField]
		private TextMeshProUGUI textCharacterAvatar;

		// Token: 0x04007B09 RID: 31497
		[SerializeField]
		private TextMeshProUGUI textMapElement;

		// Token: 0x04007B0A RID: 31498
		[SerializeField]
		private TextMeshProUGUI textMapInteract;

		// Token: 0x04007B0B RID: 31499
		[SerializeField]
		private CToggle toggleCharacterCount;

		// Token: 0x04007B0C RID: 31500
		[SerializeField]
		private CToggle toggleCharacterAvatar;

		// Token: 0x04007B0D RID: 31501
		[SerializeField]
		private CToggle toggleMapElement;

		// Token: 0x04007B0E RID: 31502
		[SerializeField]
		private CToggle toggleMapInteract;
	}
}
