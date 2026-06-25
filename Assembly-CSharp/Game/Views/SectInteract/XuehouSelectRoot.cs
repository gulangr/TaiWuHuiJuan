using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Views.SectInteract.Xuehou;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.SectInteract
{
	// Token: 0x020009BA RID: 2490
	public class XuehouSelectRoot : MonoBehaviour
	{
		// Token: 0x17000D6D RID: 3437
		// (get) Token: 0x060078A2 RID: 30882 RVA: 0x00381E9E File Offset: 0x0038009E
		private ViewJixi ViewJixi
		{
			get
			{
				return UIElement.Jixi.UiBaseAs<ViewJixi>();
			}
		}

		// Token: 0x060078A3 RID: 30883 RVA: 0x00381EAC File Offset: 0x003800AC
		public void Init(Action<int, int> onActiveIndexChange, Action seacrchButtonCallBack, Action changeCharacterButtonCallBack)
		{
			this._inChangeButton = false;
			this.neiliToggleGroup.Init(-1);
			this.neiliToggleGroup.OnActiveIndexChange += onActiveIndexChange;
			this.searchButton.ClearAndAddListener(seacrchButtonCallBack);
			this.changeCharacterButton.ClearAndAddListener(changeCharacterButtonCallBack);
			this.lightCircle.SetActive(false);
			PointerTrigger pointTrigger = base.gameObject.GetComponent<PointerTrigger>();
			PointerTrigger pointerTrigger = pointTrigger;
			if (pointerTrigger.EnterEvent == null)
			{
				pointerTrigger.EnterEvent = new UnityEvent();
			}
			pointerTrigger = pointTrigger;
			if (pointerTrigger.ExitEvent == null)
			{
				pointerTrigger.ExitEvent = new UnityEvent();
			}
			pointTrigger.EnterEvent.AddListener(delegate()
			{
				this._inChangeButton = true;
				this.lightCircle.SetActive(true);
				this.changeHover.SetActive(!this.noCharacter.gameObject.activeSelf);
			});
			pointTrigger.ExitEvent.AddListener(delegate()
			{
				this._inChangeButton = false;
				this.lightCircle.SetActive(false);
				this.changeHover.SetActive(false);
			});
			PointerTrigger searchPointTrigger = this.searchButton.GetComponent<PointerTrigger>();
			pointerTrigger = searchPointTrigger;
			if (pointerTrigger.EnterEvent == null)
			{
				pointerTrigger.EnterEvent = new UnityEvent();
			}
			searchPointTrigger.EnterEvent.AddListener(delegate()
			{
				this.lightCircle.SetActive(false);
			});
			pointerTrigger = searchPointTrigger;
			if (pointerTrigger.ExitEvent == null)
			{
				pointerTrigger.ExitEvent = new UnityEvent();
			}
			searchPointTrigger.ExitEvent.AddListener(delegate()
			{
				bool inChangeButton = this._inChangeButton;
				if (inChangeButton)
				{
					this.lightCircle.SetActive(true);
					this.changeHover.SetActive(!this.noCharacter.gameObject.activeSelf);
				}
				else
				{
					this.lightCircle.SetActive(false);
					this.changeHover.SetActive(false);
				}
			});
		}

		// Token: 0x060078A4 RID: 30884 RVA: 0x00381FD0 File Offset: 0x003801D0
		public unsafe void Set(CharacterDisplayData characterDisplayData, NeiliAllocation baseNeiliAllocation, NeiliAllocation extraNeiliAllocation, int targetIndex, IntList currentTargetNeiliAllocProgressDrained, int fixedNeiliProgressPerAllocation)
		{
			bool flowControl = this.Set(characterDisplayData, targetIndex, false);
			bool flag = !flowControl;
			if (!flag)
			{
				for (int i = 0; i < 4; i++)
				{
					Transform toggle = this.neiliToggleGroup.Get(i).transform;
					string text = string.Format("{0}\n+{1}", *(ref baseNeiliAllocation.Items.FixedElementField + (IntPtr)i * 2), (ref extraNeiliAllocation.Items.FixedElementField + (IntPtr)i * 2).ToString().SetColor("brightblue"));
					toggle.GetChild(1).GetComponent<TextMeshProUGUI>().text = text.ColorReplace();
				}
				string name = NameCenter.GetNameByDisplayData(characterDisplayData, false, false);
				int total = currentTargetNeiliAllocProgressDrained.Items.Sum((int v) => v / fixedNeiliProgressPerAllocation);
				string curNeiliAllocationTypeName = LocalStringManager.Get(string.Format("LK_CombatSkill_EquipType_{0}", targetIndex + 1));
				TooltipInvoker tooltipInvoker = this.mouseTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				this.mouseTip.RuntimeParam.Set("Title", LanguageKey.LK_Jixi_NeiliProgressDrained.Tr()).Set("LineCount", 6).SetObject("LineData1", new GeneralLineData(7, new List<string>
				{
					(targetIndex >= 0) ? LanguageKey.LK_Jixi_NeiliProgressDrained_Content.TrFormat(name, curNeiliAllocationTypeName.SetColor(XuehouSelectRoot.NeiliAllocationFontColor[(targetIndex == -1) ? 0 : targetIndex]), total.ToString().SetColor("lightyellow")) : LanguageKey.LK_Jixi_NeiliProgressDrained_Disable.Tr()
				}, null)).SetObject("LineData2", new GeneralLineData(4, null, null));
				for (int j = 0; j < 4; j++)
				{
					string neiliAllocationTypeName = LocalStringManager.Get(string.Format("LK_CombatSkill_EquipType_{0}", j + 1));
					this.mouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", j + 3), new GeneralLineData(5, new List<string>
					{
						string.Concat(new string[]
						{
							"<SpName=ui9_icon_mousetip_kungfu_",
							(j + 1).ToString(),
							">",
							neiliAllocationTypeName,
							"：",
							(currentTargetNeiliAllocProgressDrained.Items[j] / fixedNeiliProgressPerAllocation).ToString().SetColor("lightyellow")
						})
					}, null));
				}
				this.mouseTip.Refresh(false, -1);
			}
		}

		// Token: 0x060078A5 RID: 30885 RVA: 0x00382278 File Offset: 0x00380478
		public void Set(CharacterDisplayData characterDisplayData, sbyte[] fiveElements, int targetIndex, int[] taiwuTransformFiveElementsTotal)
		{
			bool flowControl = this.Set(characterDisplayData, targetIndex, true);
			bool flag = !flowControl;
			if (!flag)
			{
				for (int i = 0; i < 5; i++)
				{
					this.neiliToggleGroup.SetInteractable(fiveElements[i] > 0, i);
					Transform toggle = this.neiliToggleGroup.Get(i).transform;
					bool flag2 = fiveElements[i] <= 0 && this.neiliToggleGroup.GetActiveIndex() == i;
					if (flag2)
					{
						this.neiliToggleGroup.DeSelect(false);
					}
					toggle.GetChild(1).GetComponent<TextMeshProUGUI>().text = string.Format("{0}%", fiveElements[i]).ToString();
				}
				string name = NameCenter.GetNameByDisplayData(characterDisplayData, false, false);
				string currFiveElementName = LocalStringManager.Get(string.Format("LK_FiveElements_Type_{0}", targetIndex));
				int total = taiwuTransformFiveElementsTotal.Sum();
				TooltipInvoker tooltipInvoker = this.mouseTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				this.mouseTip.RuntimeParam.Set("Title", LanguageKey.LK_Jixi_NeiliProgressTrans.Tr()).Set("LineCount", 7).SetObject("LineData1", new GeneralLineData(7, new List<string>
				{
					(targetIndex >= 0) ? LanguageKey.LK_Jixi_NeiliProgressTrans_Content.TrFormat(name, currFiveElementName.SetColor(Colors.Instance.FiveElementsColors[targetIndex]), total.ToString().SetColor("lightyellow")) : LanguageKey.LK_Jixi_NeiliProgressTrans_Disable.Tr()
				}, null)).SetObject("LineData2", new GeneralLineData(4, null, null));
				for (int j = 0; j < 5; j++)
				{
					sbyte displayIndex = XuehouSelectRoot.DisplayFiveElementsOrder[j];
					string fiveElementsTypeName = LocalStringManager.Get(string.Format("LK_FiveElements_Type_{0}", displayIndex));
					this.mouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", j + 3), new GeneralLineData(5, new List<string>
					{
						string.Concat(new string[]
						{
							"<SpName=ui9_icon_mousetip_elements_",
							displayIndex.ToString(),
							">",
							fiveElementsTypeName,
							"：",
							taiwuTransformFiveElementsTotal[(int)displayIndex].ToString().SetColor("lightyellow"),
							"%"
						})
					}, null));
				}
				this.mouseTip.Refresh(false, -1);
			}
		}

		// Token: 0x060078A6 RID: 30886 RVA: 0x00382504 File Offset: 0x00380704
		private bool Set(CharacterDisplayData characterDisplayData, int targetIndex, bool isTaiwu)
		{
			this.noCharacter.gameObject.SetActive(characterDisplayData.CharacterId < 0);
			this.avatar.gameObject.SetActive(characterDisplayData.CharacterId >= 0);
			this.neiliToggleGroup.gameObject.SetActive(characterDisplayData.CharacterId >= 0);
			this.searchButton.gameObject.SetActive(characterDisplayData.CharacterId >= 0);
			this.characterName.transform.parent.gameObject.SetActive(characterDisplayData.CharacterId >= 0);
			this.ViewJixi.ChangeConnectEffStatus(isTaiwu, characterDisplayData.CharacterId >= 0 && targetIndex >= 0, targetIndex);
			this.mouseTip.enabled = (characterDisplayData.CharacterId >= 0);
			bool flag = characterDisplayData.CharacterId < 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this.avatar.Refresh(characterDisplayData, true);
				this.characterName.text = NameCenter.GetNameByDisplayData(characterDisplayData, false, false);
				bool flag2 = targetIndex != -1;
				if (flag2)
				{
					this.neiliToggleGroup.SetWithoutNotify(targetIndex);
				}
				bool flag3 = targetIndex == -1;
				if (flag3)
				{
					this.neiliToggleGroup.DeSelectWithoutNotify();
				}
				result = true;
			}
			return result;
		}

		// Token: 0x04005B4F RID: 23375
		[SerializeField]
		private TextMeshProUGUI characterName;

		// Token: 0x04005B50 RID: 23376
		[SerializeField]
		private RectTransform noCharacter;

		// Token: 0x04005B51 RID: 23377
		[SerializeField]
		private CButton searchButton;

		// Token: 0x04005B52 RID: 23378
		[SerializeField]
		private CButton changeCharacterButton;

		// Token: 0x04005B53 RID: 23379
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04005B54 RID: 23380
		[SerializeField]
		private CToggleGroup neiliToggleGroup;

		// Token: 0x04005B55 RID: 23381
		[SerializeField]
		private TooltipInvoker mouseTip;

		// Token: 0x04005B56 RID: 23382
		[SerializeField]
		private GameObject lightCircle;

		// Token: 0x04005B57 RID: 23383
		[SerializeField]
		private GameObject changeHover;

		// Token: 0x04005B58 RID: 23384
		public static readonly string[] NeiliAllocationFontColor = new string[]
		{
			"attack",
			"agile",
			"defense",
			"assist"
		};

		// Token: 0x04005B59 RID: 23385
		private static readonly sbyte[] DisplayFiveElementsOrder = new sbyte[]
		{
			0,
			2,
			1,
			3,
			4
		};

		// Token: 0x04005B5A RID: 23386
		private bool _inChangeButton;
	}
}
