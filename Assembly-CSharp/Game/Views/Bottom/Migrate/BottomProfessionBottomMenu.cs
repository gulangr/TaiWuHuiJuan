using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Profession;
using GameData.Domains.Taiwu.Profession.SkillsData;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Bottom.Migrate
{
	// Token: 0x02000C5C RID: 3164
	public class BottomProfessionBottomMenu : MonoBehaviour
	{
		// Token: 0x170010EB RID: 4331
		// (get) Token: 0x0600A14D RID: 41293 RVA: 0x004B6083 File Offset: 0x004B4283
		private ProfessionModel Model
		{
			get
			{
				return SingletonObject.getInstance<ProfessionModel>();
			}
		}

		// Token: 0x0600A14E RID: 41294 RVA: 0x004B608A File Offset: 0x004B428A
		private void OnEnable()
		{
			this.RefreshByEvent(null);
			GEvent.Add(UiEvents.OnProfessionDataChange, new GEvent.Callback(this.RefreshByEvent));
		}

		// Token: 0x0600A14F RID: 41295 RVA: 0x004B60B1 File Offset: 0x004B42B1
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.OnProfessionDataChange, new GEvent.Callback(this.RefreshByEvent));
		}

		// Token: 0x0600A150 RID: 41296 RVA: 0x004B60D0 File Offset: 0x004B42D0
		private void Update()
		{
			bool flag = !base.gameObject.activeSelf;
			if (!flag)
			{
				PolygonRaycastGraphic polygonRaycast = base.GetComponent<PolygonRaycastGraphic>();
				bool flag2 = polygonRaycast == null;
				if (!flag2)
				{
					bool isMouseInPolygon = polygonRaycast.Raycast(Input.mousePosition, UIManager.Instance.UiCamera);
					bool flag3 = !isMouseInPolygon;
					if (flag3)
					{
						base.gameObject.SetActive(false);
					}
				}
			}
		}

		// Token: 0x0600A151 RID: 41297 RVA: 0x004B613C File Offset: 0x004B433C
		public void Init(IAsyncMethodRequestHandler handler)
		{
			this._handler = handler;
			bool flag = !this._inited;
			if (flag)
			{
				this.InitSlotRefers();
			}
			this._inited = true;
		}

		// Token: 0x0600A152 RID: 41298 RVA: 0x004B616E File Offset: 0x004B436E
		public void RefreshByEvent(ArgumentBox argbox)
		{
			TaiwuDomainMethod.AsyncCall.RequestTaiwuResourceDisplayData(null, delegate(int offset, RawDataPool pool)
			{
				TaiwuResourceDisplayData data = new TaiwuResourceDisplayData();
				Serializer.Deserialize(pool, offset, ref data);
				this._resources = new ResourceInts(data.Resources);
				this._exp = data.Exp;
				this.Refresh();
			});
		}

		// Token: 0x0600A153 RID: 41299 RVA: 0x004B6184 File Offset: 0x004B4384
		public void Refresh()
		{
			this.RefreshExtraSkill();
			for (int i = 0; i < this._equipSkillItems.Count; i++)
			{
				List<bottomBtnProfessionSkill> subList = this._equipSkillItems[i];
				for (int j = 0; j < subList.Count; j++)
				{
					bottomBtnProfessionSkill skillItem = subList[j];
					int skillId = this.Model.GetProfessionSkillFromSlot(i, j);
					bool isEmpty = skillId == -1;
					ValueTuple<ProfessionData, int> valueTuple = this.Model.FindProfessionDataBySkillId(skillId);
					ProfessionData professionData = valueTuple.Item1;
					int skillIndex = valueTuple.Item2;
					GameObject content = skillItem.content;
					GameObject empty = skillItem.empty;
					TooltipInvoker tip = skillItem.GetComponent<TooltipInvoker>();
					content.SetActive(!isEmpty);
					empty.SetActive(isEmpty);
					tip.enabled = !isEmpty;
					bool flag = !isEmpty;
					if (flag)
					{
						int professionId = professionData.TemplateId;
						ProfessionItem professionConfig = Profession.Instance[professionId];
						ProfessionSkillItem professionSkillConfig = ProfessionSkill.Instance[skillId];
						bool isUnlocked = professionData.IsSkillUnlocked(skillIndex);
						ProfessionSkillItem config = ProfessionSkill.Instance[skillId];
						GameObject unlocked = skillItem.unlocked;
						GameObject locked = skillItem.locked;
						CImage icon = skillItem.icon;
						GameObject coolDown = skillItem.coolDown;
						TextMeshProUGUI label = skillItem.label;
						TextMeshProUGUI cdLabel = skillItem.cdLabel;
						CImage hover = skillItem.hover;
						int x = (int)professionSkillConfig.TriggerType;
						sbyte b = unlocked ? professionSkillConfig.Level : 0;
						skillItem.RefreshSkillType(config.Type);
						skillItem.SetSelected(false);
						icon.SetSprite(config.Icon, false, null);
						locked.SetActive(!isUnlocked);
						unlocked.SetActive(isUnlocked);
						label.text = config.Name;
						bool isCoolDown = professionData.IsSkillCooldown(SingletonObject.getInstance<BasicGameData>().CurrDate, skillIndex);
						coolDown.SetActive(isCoolDown);
						int coolDownTime = professionData.SkillOffCooldownDates[i] - SingletonObject.getInstance<BasicGameData>().CurrDate;
						cdLabel.text = Mathf.Max(0, coolDownTime).ToString();
						bool isActive = config.TriggerType == EProfessionSkillTriggerType.Active;
						bool flag2 = skillIndex == 3 && professionConfig.ExtraProfessionSkill > 0;
						skillItem.numbers.SetActive(false);
						TeaTasterSkillsData skillsData;
						bool flag3;
						if (skillId == 66)
						{
							skillsData = (professionData.SkillsData as TeaTasterSkillsData);
							flag3 = (skillsData != null);
						}
						else
						{
							flag3 = false;
						}
						bool flag4 = flag3;
						if (flag4)
						{
							this.RefreshNumbers(skillItem, skillsData.ActionPointGained / 10, GlobalConfig.Instance.ProfessionSkillRecoverActionPointLimit / 10);
						}
						CButton button = skillItem.GetComponent<CButton>();
						this.RefreshButton(button, professionId, skillIndex, professionConfig, professionData, icon, isCoolDown, isActive);
						BottomProfessionBottomMenu.RefreshPointerTrigger(skillItem, button);
						this.RefreshTips(tip, isUnlocked, skillId, professionData, professionId, skillIndex);
					}
					else
					{
						CButton button2 = skillItem.GetComponent<CButton>();
						button2.onClick.RemoveAllListeners();
						skillItem.HideHover();
						PointerTrigger pointerTrigger = skillItem.GetComponent<PointerTrigger>();
						pointerTrigger.enabled = false;
					}
				}
			}
		}

		// Token: 0x0600A154 RID: 41300 RVA: 0x004B6478 File Offset: 0x004B4678
		public static void ExtraProfessionSkillSelectFeature()
		{
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, delegate(int offset, RawDataPool dataPool)
			{
				CharacterDisplayData data = null;
				Serializer.Deserialize(dataPool, offset, ref data);
				string taiwuName = NameCenter.GetNameByDisplayData(data, true, false);
				string content = LocalStringManager.GetFormat(LanguageKey.Event_ExtraProfessionSkill_SelectFeature, taiwuName);
				List<short> featureList = new List<short>
				{
					252,
					253,
					254,
					255,
					256,
					257,
					258,
					259,
					260,
					261
				};
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.SetObject("featureList", featureList);
				argBox.Set("texPath", "Profession/tex_profession_activate");
				argBox.Set("desc", content);
				argBox.SetObject("callBack", new Action<short>(BottomProfessionBottomMenu.ExecuteExtraProfessionSkillSetFeature));
				UIElement.EventStyleFeatureSelect.SetOnInitArgs(argBox);
				UIManager.Instance.MaskUI(UIElement.EventStyleFeatureSelect);
			});
		}

		// Token: 0x0600A155 RID: 41301 RVA: 0x004B64AC File Offset: 0x004B46AC
		private static void ExecuteExtraProfessionSkillSetFeature(short id)
		{
			bool flag = id < 0;
			if (!flag)
			{
				DialogCmd cmd = new DialogCmd
				{
					Title = LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_Confirm_Title),
					Content = string.Concat(new string[]
					{
						LocalStringManager.GetFormat(LanguageKey.LK_ProfessionSkill_Confirm_NoCost, LocalStringManager.GetFormat(LanguageKey.LK_ExtraProfessionSkill_Title, Array.Empty<object>())),
						"\n",
						LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_Confirm_CoolDown),
						" <SpName=mousetip_shijie> 1",
						LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_Confirm_Confirm)
					}),
					Type = 1,
					Yes = delegate()
					{
						SingletonObject.getInstance<ProfessionModel>().SetLastTimeCastExtraSkill();
						TaiwuEventDomainMethod.Call.CloseUI("UI_ExtraProfessionSkillConfirmed", false, (int)id);
					},
					No = null
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
		}

		// Token: 0x0600A156 RID: 41302 RVA: 0x004B6594 File Offset: 0x004B4794
		private void RefreshExtraSkill()
		{
			bool flag = !this.Model.IsExtraProfessionSkillUnlocked;
			if (flag)
			{
				this.extraSkill.gameObject.SetActive(false);
			}
			else
			{
				bool isReady = this.Model.IsExtraProfessionSkillReady();
				this.extraSkill.Refresh(isReady, new Action(BottomProfessionBottomMenu.ExtraProfessionSkillSelectFeature));
				this.extraSkill.RefreshTip();
			}
		}

		// Token: 0x0600A157 RID: 41303 RVA: 0x004B65FC File Offset: 0x004B47FC
		private void RefreshNumbers(bottomBtnProfessionSkill refers, int valueLeft, int valueRight)
		{
			bool flag = valueLeft >= 10;
			if (flag)
			{
				refers.numLeft1.GetComponent<CImage>().SetSprite(string.Format("sp_number_0_{0}", valueLeft / 10), false, null);
				refers.numLeft2.GetComponent<CImage>().SetSprite(string.Format("sp_number_0_{0}", valueLeft % 10), false, null);
				refers.numLeft1.SetActive(true);
				refers.numLeft2.SetActive(true);
			}
			else
			{
				refers.numLeft1.GetComponent<CImage>().SetSprite(string.Format("sp_number_0_{0}", valueLeft), false, null);
				refers.numLeft1.SetActive(true);
				refers.numLeft2.SetActive(false);
			}
			bool flag2 = valueRight >= 10;
			if (flag2)
			{
				refers.numRight1.GetComponent<CImage>().SetSprite(string.Format("sp_number_0_{0}", valueRight / 10), false, null);
				refers.numRight2.GetComponent<CImage>().SetSprite(string.Format("sp_number_0_{0}", valueRight % 10), false, null);
				refers.numRight1.SetActive(true);
				refers.numRight2.SetActive(true);
			}
			else
			{
				refers.numRight1.GetComponent<CImage>().SetSprite(string.Format("sp_number_0_{0}", valueRight), false, null);
				refers.numRight1.SetActive(true);
				refers.numRight2.SetActive(false);
			}
			refers.numbers.SetActive(true);
		}

		// Token: 0x0600A158 RID: 41304 RVA: 0x004B6784 File Offset: 0x004B4984
		private void RefreshTips(TooltipInvoker tip, bool isUnlocked, int skillId, ProfessionData professionData, int professionId, int skillIndex)
		{
			bool flag = !isUnlocked;
			if (flag)
			{
				bool flag2 = tip.RuntimeParam != null;
				if (flag2)
				{
					EasyPool.Free<ArgumentBox>(tip.RuntimeParam);
					tip.RuntimeParam = null;
				}
				tip.Type = TipType.SingleDesc;
				bool flag3 = tip.PresetParam == null || tip.PresetParam.Length == 0;
				if (flag3)
				{
					tip.PresetParam = new string[1];
				}
				tip.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_Seniority_NotMeet);
			}
			else
			{
				tip.Type = TipType.ProfessionSkill;
				ArgumentBox tipRuntimeParam = EasyPool.Get<ArgumentBox>();
				tipRuntimeParam.Set("ProfessionSkillId", skillId);
				tipRuntimeParam.SetObject("ProfessionData", professionData);
				tipRuntimeParam.Set("ProfessionId", professionId);
				tipRuntimeParam.Set("SkillIndex", skillIndex);
				tipRuntimeParam.Set("Exp", this._exp);
				tipRuntimeParam.SetObject("Resources", this._resources);
				tip.RuntimeParam = tipRuntimeParam;
			}
		}

		// Token: 0x0600A159 RID: 41305 RVA: 0x004B6877 File Offset: 0x004B4A77
		public void UpdateExp(int exp)
		{
			this._exp = exp;
		}

		// Token: 0x0600A15A RID: 41306 RVA: 0x004B6881 File Offset: 0x004B4A81
		public void UpdateResources(ResourceInts resourceInts)
		{
			this._resources = resourceInts;
		}

		// Token: 0x0600A15B RID: 41307 RVA: 0x004B688C File Offset: 0x004B4A8C
		private void RefreshButton(CButton button, int professionId, int skillIndex, ProfessionItem professionConfig, ProfessionData professionData, CImage icon, bool isCoolDown, bool isActive)
		{
			ProfessionSkillItem skillCfg = professionData.GetSkillConfig(skillIndex);
			bool flag = skillCfg.TemplateId == 3;
			if (flag)
			{
				button.ClearAndAddListener(delegate
				{
					ProfessionSkillArg professionSkillArg = new ProfessionSkillArg
					{
						ProfessionId = professionId,
						SkillId = skillCfg.TemplateId,
						IsSuccess = true
					};
					ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
					argumentBox.Clear();
					argumentBox.SetObject("ProfessionSkillArg", professionSkillArg);
					argumentBox.Set("PreConirmTimeCost", GlobalConfig.Instance.SavageSkill3_OpenItemSelectTimeCost);
					UIElement.ProfessionSkillPreConfirm.SetOnInitArgs(argumentBox);
					UIManager.Instance.ShowUI(UIElement.ProfessionSkillPreConfirm, true);
				});
			}
			else
			{
				button.ClearAndAddListener(delegate
				{
					ExtraDomainMethod.Call.ExecuteActiveProfessionSkill(professionId, skillIndex);
					this.gameObject.SetActive(false);
				});
			}
			bool flag2 = skillIndex == 3 && professionConfig.ExtraProfessionSkill > 0;
			bool canUse = UIManager.Instance.IsFocusElement(UIElement.StateMainWorld);
			ProfessionSkillStateHelper.OnGetResult <>9__3;
			ExtraDomainMethod.AsyncCall.CanExecuteProfessionSkill(this._handler, professionId, skillIndex, delegate(int offset, RawDataPool dataPool)
			{
				bool canExecute = false;
				Serializer.Deserialize(dataPool, offset, ref canExecute);
				button.interactable = (canExecute & canUse);
				IAsyncMethodRequestHandler handler = this._handler;
				int templateId = skillCfg.TemplateId;
				ResourceInts resources = this._resources;
				int exp = this._exp;
				ProfessionSkillStateHelper.OnGetResult onGetResult;
				if ((onGetResult = <>9__3) == null)
				{
					onGetResult = (<>9__3 = delegate(ProfessionSkillStateHelper.Result result)
					{
						bool needGray = isCoolDown || (isActive && !button.interactable) || (skillCfg.Type != EProfessionSkillType.Passive && !result.CanUse);
						icon.SetColor(needGray ? Color.gray : Color.white);
					});
				}
				ProfessionSkillStateHelper.AsyncGetSkillUseState(handler, templateId, resources, exp, onGetResult);
			});
		}

		// Token: 0x0600A15C RID: 41308 RVA: 0x004B6980 File Offset: 0x004B4B80
		private static void RefreshPointerTrigger(bottomBtnProfessionSkill skillItem, CButton button)
		{
			PointerTrigger pointerTrigger = skillItem.GetComponent<PointerTrigger>();
			pointerTrigger.enabled = true;
			pointerTrigger.EnterEvent.RemoveAllListeners();
			pointerTrigger.ExitEvent.RemoveAllListeners();
			pointerTrigger.EnterEvent.AddListener(delegate()
			{
				bool interactable = button.interactable;
				if (interactable)
				{
					skillItem.ShowHover();
				}
			});
			pointerTrigger.ExitEvent.AddListener(new UnityAction(skillItem.HideHover));
		}

		// Token: 0x0600A15D RID: 41309 RVA: 0x004B6A04 File Offset: 0x004B4C04
		private void InitSlotRefers()
		{
			this._equipSkillItems = new List<List<bottomBtnProfessionSkill>>();
			int levelCount = 4;
			bottomBtnProfessionSkill[] allChildRefers = this.skillSlots.GetComponentsInChildren<bottomBtnProfessionSkill>();
			int i;
			int num;
			for (i = 0; i < levelCount; i = num + 1)
			{
				List<bottomBtnProfessionSkill> subList = new List<bottomBtnProfessionSkill>();
				int slotCount = levelCount - i;
				int j;
				for (j = 0; j < slotCount; j = num + 1)
				{
					bottomBtnProfessionSkill item = allChildRefers.Find((bottomBtnProfessionSkill r) => r.gameObject.name == string.Format("Slot_{0}_{1}", i, j));
					subList.Add(item);
					num = j;
				}
				this._equipSkillItems.Add(subList);
				num = i;
			}
		}

		// Token: 0x04007D19 RID: 32025
		private List<List<bottomBtnProfessionSkill>> _equipSkillItems;

		// Token: 0x04007D1A RID: 32026
		private bool _inited = false;

		// Token: 0x04007D1B RID: 32027
		private IAsyncMethodRequestHandler _handler;

		// Token: 0x04007D1C RID: 32028
		private ResourceInts _resources;

		// Token: 0x04007D1D RID: 32029
		private int _exp;

		// Token: 0x04007D1E RID: 32030
		public RectTransform skillSlots;

		// Token: 0x04007D1F RID: 32031
		public GameObject extraSkillBg;

		// Token: 0x04007D20 RID: 32032
		public BottomProfessionExtraSkill extraSkill;
	}
}
