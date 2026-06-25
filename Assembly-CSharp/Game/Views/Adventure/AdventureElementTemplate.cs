using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Views.Migrate;
using GameData.Adventure;
using GameData.Domains.Adventure;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Adventure
{
	// Token: 0x02000C64 RID: 3172
	public class AdventureElementTemplate : MonoBehaviour
	{
		// Token: 0x0600A192 RID: 41362 RVA: 0x004B7D64 File Offset: 0x004B5F64
		public bool RefreshDisplay(ViewAdventureRemake.ElementDisplayItem item, AdventureElement element, AdventureBlockIndex taiwuDataCoord, AdventureActionData elementActionData, int remainTime, Action btnAction, AdventureRuntime adventureRuntime, bool selectElementMode)
		{
			this._localCharacterId = -1;
			AdventureElementData elementData = AdventureRemakeModel.Core.GetAdventureElementData(element.CoreId);
			string iconSprite = ViewAdventureRemake.GetElementIcon(elementData, element);
			this.icon.SetSprite(ViewAdventureRemake.GetElementUIIconName(iconSprite), false, null);
			AdventureElementVertexModifier modifier = this.icon.gameObject.GetOrAddComponent<AdventureElementVertexModifier>();
			bool flag = modifier != null;
			if (flag)
			{
				AdventureUnitMicro parentMicro = base.GetComponentInParent<AdventureUnitMicro>();
				bool flag2 = parentMicro != null;
				if (flag2)
				{
					modifier.GridIndex = parentMicro.RenderBlockIndex;
				}
			}
			AdventureParameterValue defineGrade;
			bool haveCharacterDefineGrade = element.TryGetParameter("ConchShipPresetKey_CharacterDefineGrade", out defineGrade);
			bool flag3 = elementData.CharacterId > 0 || haveCharacterDefineGrade;
			if (flag3)
			{
				this.characterName.gameObject.SetActive(false);
				this.elementNameHolder.SetActive(true);
				this.elementName.fontSize = 30f;
				CharacterItem config = Character.Instance[elementData.CharacterId];
				int grade = haveCharacterDefineGrade ? defineGrade.Current : ((int)config.OrganizationInfo.Grade);
				this.identityIcon.SetSprite("ui9_icon_identity_big_" + grade.ToString(), true, null);
				this.elementName.SetText(elementData.Name.SetColor(Colors.Instance.GradeColors[grade]).ColorReplace(), true);
			}
			else
			{
				bool flag4 = adventureRuntime.IsCalledCharacter(element.CharacterId) || adventureRuntime.IsTemporaryCharacter(element.CharacterId);
				if (flag4)
				{
					this.characterName.gameObject.SetActive(true);
					this.elementNameHolder.SetActive(true);
					this.elementName.fontSize = 24f;
					this._localCharacterId = element.CharacterId;
					int characterId = element.CharacterId;
					CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, element.CharacterId, delegate(int offset, RawDataPool pool)
					{
						bool flag8 = characterId != this._localCharacterId;
						if (!flag8)
						{
							CharacterDisplayData characterDisplayData = null;
							Serializer.Deserialize(pool, offset, ref characterDisplayData);
							string displayName = NameCenter.GetMonasticTitleOrDisplayName(characterDisplayData, false);
							this.characterName.SetText(displayName, true);
							sbyte grade2 = characterDisplayData.OrgInfo.Grade;
							this.identityIcon.SetSprite("ui9_icon_identity_small_" + grade2.ToString(), true, null);
							this.elementName.SetText(elementData.Name.SetColor(Colors.Instance.GradeColors[(int)grade2]).ColorReplace(), true);
						}
					});
				}
				else
				{
					this.characterName.gameObject.SetActive(true);
					this.elementNameHolder.SetActive(false);
					this.characterName.SetText(elementData.Name.SetColor("white").ColorReplace(), true);
				}
			}
			this.button.interactable = (element.Index.Equals(taiwuDataCoord) || selectElementMode);
			this.button.ClearAndAddListener(new Action(btnAction.Invoke));
			TooltipInvoker tooltipInvoker = this.mouseTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			this.mouseTip.Type = TipType.Simple;
			this.mouseTip.RuntimeParam.Set("arg0", elementData.Name);
			this.mouseTip.RuntimeParam.Set("arg1", elementData.Desc);
			this.mouseTip.enabled = true;
			this.mouseTip.Refresh(false, -1);
			this.actionProgress.gameObject.SetActive(elementActionData != null);
			bool flag5 = elementActionData != null;
			if (flag5)
			{
				this.actionProgress.SetValue(elementActionData, remainTime);
				this.actionProgress.gameObject.SetActive(true);
			}
			List<AdventureParameterData> buffList = (from e in element.Core.Parameters
			where e != null && !string.IsNullOrEmpty(e.Name) && ((e.Type == EAdventureParameterType.State && element.GetParameter(e.Key).Current > 0) || element.GetParameter(e.Key).Max < 0)
			select e).ToList<AdventureParameterData>();
			this.buffHolder.Rebuild<RectTransform>(buffList.Count, delegate(RectTransform refer, int index)
			{
				AdventureParameterData parameterData = buffList[index];
				AdventureParameterValue parameterValue = element.GetParameter(parameterData.Key);
				AdventureRemakeParameterItem adventureRemakeParameterItem = refer.GetComponent<AdventureRemakeParameterItem>();
				adventureRemakeParameterItem.SetValue(parameterData, parameterValue, false, false);
			});
			float infoHeight = this.nameHodler.rect.size.y;
			float actionHeight = this.actionProgress.gameObject.activeSelf ? this.actionProgress.GetComponent<RectTransform>().rect.height : 0f;
			float minHeight = this.buffHolder.template.GetComponent<LayoutElement>().minHeight;
			float buffHeight = (minHeight + this.buffHolderLayout.spacing) * (float)buffList.Count + (float)((buffList.Count > 0) ? 5 : 0);
			float totalHeight = actionHeight + infoHeight + buffHeight;
			this.actionHolder.gameObject.SetActive(this.actionProgress.gameObject.activeSelf || buffList.Count > 0);
			base.GetComponent<LayoutElement>().preferredHeight = totalHeight;
			bool canInteract = false;
			for (int i = 0; i < elementData.Events.Count; i++)
			{
				AdventureElementEventData elementEvent = elementData.Events[i];
				bool flag6 = elementEvent.TriggerType == EAdventureElementEventTriggerType.ManualInteract;
				if (flag6)
				{
					bool flag7 = !elementEvent.Event.OnlyOnce || !adventureRuntime.IsInvokedOnceElementEvent(element.Id, i);
					if (flag7)
					{
						canInteract = true;
					}
				}
			}
			this.canInteractBg.gameObject.SetActive(canInteract);
			this.canNotInteractBg.gameObject.SetActive(!canInteract);
			this.canInteractInfoBg.gameObject.SetActive(canInteract);
			this.canNotInteractInfoBg.gameObject.SetActive(!canInteract);
			return canInteract;
		}

		// Token: 0x0600A193 RID: 41363 RVA: 0x004B8328 File Offset: 0x004B6528
		public void RefreshExit(ViewAdventureRemake.ElementDisplayItem item, bool isOnBlock)
		{
			string iconName = ViewAdventureRemake.GetElementUIIconName(isOnBlock ? "adventure_element_door_01" : "adventure_element_door_02");
			this.icon.SetSprite(iconName, false, null);
			this.mouseTip.enabled = false;
			this.mouseTip.Refresh(false, -1);
			this.button.interactable = isOnBlock;
			bool interactable = this.button.interactable;
			if (interactable)
			{
				this.button.ClearAndAddListener(delegate
				{
					GEvent.OnEvent(UiEvents.AdventureExitClick, null);
				});
			}
			this.characterName.SetText(LanguageKey.LK_Adventure_ExitFakeElement_Title.Tr(), true);
			this.characterName.gameObject.SetActive(true);
			this.elementNameHolder.gameObject.SetActive(false);
			this.actionHolder.gameObject.SetActive(false);
			this.canInteractBg.gameObject.SetActive(true);
			this.canNotInteractBg.gameObject.SetActive(false);
			this.canInteractInfoBg.gameObject.SetActive(true);
			this.canNotInteractInfoBg.gameObject.SetActive(false);
			float infoHeight = this.nameHodler.rect.size.y;
			base.GetComponent<LayoutElement>().preferredHeight = infoHeight;
			this.buffHolder.Rebuild<RectTransform>(0, null);
		}

		// Token: 0x04007D5D RID: 32093
		[SerializeField]
		private CImage icon;

		// Token: 0x04007D5E RID: 32094
		[SerializeField]
		private TextMeshProUGUI characterName;

		// Token: 0x04007D5F RID: 32095
		[SerializeField]
		private GameObject elementNameHolder;

		// Token: 0x04007D60 RID: 32096
		[SerializeField]
		private CImage identityIcon;

		// Token: 0x04007D61 RID: 32097
		[SerializeField]
		private TextMeshProUGUI elementName;

		// Token: 0x04007D62 RID: 32098
		[SerializeField]
		private AdventureActionProgress actionProgress;

		// Token: 0x04007D63 RID: 32099
		[SerializeField]
		private TemplatedContainerAssemblyNew buffHolder;

		// Token: 0x04007D64 RID: 32100
		[SerializeField]
		private VerticalLayoutGroup buffHolderLayout;

		// Token: 0x04007D65 RID: 32101
		[SerializeField]
		private CButton button;

		// Token: 0x04007D66 RID: 32102
		[SerializeField]
		public PointerTrigger pointerTrigger;

		// Token: 0x04007D67 RID: 32103
		[SerializeField]
		public CImage interactHover;

		// Token: 0x04007D68 RID: 32104
		[SerializeField]
		public CImage canInteractBg;

		// Token: 0x04007D69 RID: 32105
		[SerializeField]
		public CImage canNotInteractBg;

		// Token: 0x04007D6A RID: 32106
		[SerializeField]
		public CImage canInteractInfoBg;

		// Token: 0x04007D6B RID: 32107
		[SerializeField]
		public CImage canNotInteractInfoBg;

		// Token: 0x04007D6C RID: 32108
		[SerializeField]
		public RectTransform nameHodler;

		// Token: 0x04007D6D RID: 32109
		[SerializeField]
		public RectTransform actionHolder;

		// Token: 0x04007D6E RID: 32110
		[SerializeField]
		public TooltipInvoker mouseTip;

		// Token: 0x04007D6F RID: 32111
		private int _localCharacterId;
	}
}
