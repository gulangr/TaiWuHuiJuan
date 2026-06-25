using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.Building.BuildingAreaQuickActionMenu;
using Game.Views.CharacterMenu;
using Game.Views.SectInteract.Fulong;
using Game.Views.VillagerRoleView;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Extra;
using GameData.Domains.Organization;
using GameData.Domains.Taiwu;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Bottom
{
	// Token: 0x02000C30 RID: 3120
	public class CustomButton : MonoBehaviour
	{
		// Token: 0x170010C0 RID: 4288
		// (get) Token: 0x06009E88 RID: 40584 RVA: 0x004A21B5 File Offset: 0x004A03B5
		// (set) Token: 0x06009E89 RID: 40585 RVA: 0x004A21C7 File Offset: 0x004A03C7
		public static int[] Settings
		{
			get
			{
				return CustomButton.SettingsCache.Take(6).ToArray<int>();
			}
			set
			{
				TaiwuDomainMethod.Call.SetActiveShortCut(CustomButton.SettingsCache = value.ToList<int>());
			}
		}

		// Token: 0x170010C1 RID: 4289
		// (get) Token: 0x06009E8A RID: 40586 RVA: 0x004A21DB File Offset: 0x004A03DB
		public bool IsConfig
		{
			get
			{
				return this._parent == null;
			}
		}

		// Token: 0x170010C2 RID: 4290
		// (get) Token: 0x06009E8B RID: 40587 RVA: 0x004A21E6 File Offset: 0x004A03E6
		// (set) Token: 0x06009E8C RID: 40588 RVA: 0x004A21F0 File Offset: 0x004A03F0
		public int TemplateId
		{
			get
			{
				return this.templateId;
			}
			set
			{
				this.templateId = value;
				SpriteState state = this.button.spriteState;
				MainUiCustomButtonItem cfg = Config.MainUiCustomButton.Instance[this.templateId];
				this.buttonName.text = cfg.Name;
				this.img.SetSprite(cfg.IconDisable, false, null);
				state.disabledSprite = this.img.sprite;
				this.img.SetSprite(cfg.IconPressed, false, null);
				state.pressedSprite = null;
				this.hover.sprite = this.img.sprite;
				this.img.SetSprite(cfg.IconHighLight, false, null);
				state.highlightedSprite = this.img.sprite;
				this.img.SetSprite(cfg.IconNormal, false, null);
				state.selectedSprite = this.img.sprite;
				this.button.spriteState = state;
				base.gameObject.SetActive(true);
			}
		}

		// Token: 0x06009E8D RID: 40589 RVA: 0x004A22F8 File Offset: 0x004A04F8
		private void Awake()
		{
			this.button.onClick.ResetListener(new Action(this.Click));
			this.remove.onClick.ResetListener(new Action(this.Click));
			GEvent.Add(UiEvents.NotifyBottomCustomButtonChange, new GEvent.Callback(this.RefreshButton));
		}

		// Token: 0x06009E8E RID: 40590 RVA: 0x004A235C File Offset: 0x004A055C
		private void OnDestroy()
		{
			GEvent.Remove(UiEvents.NotifyBottomCustomButtonChange, new GEvent.Callback(this.RefreshButton));
		}

		// Token: 0x06009E8F RID: 40591 RVA: 0x004A237B File Offset: 0x004A057B
		public void Init(IAsyncMethodRequestHandler parent)
		{
			this._parent = parent;
		}

		// Token: 0x06009E90 RID: 40592 RVA: 0x004A2384 File Offset: 0x004A0584
		private void OnEnable()
		{
			this.RefreshButton(null);
		}

		// Token: 0x06009E91 RID: 40593 RVA: 0x004A2390 File Offset: 0x004A0590
		private void RefreshButton(ArgumentBox _)
		{
			bool isConfig = this.IsConfig;
			if (isConfig)
			{
				this.hover.gameObject.SetActive(ViewBottom.TempShortCutsSettings.Contains(this.templateId));
			}
			else
			{
				this.hover.gameObject.SetActive(false);
				bool flag = CustomButton.Settings.CheckIndex(this.index);
				if (flag)
				{
					this.TemplateId = CustomButton.Settings[this.index];
				}
				else
				{
					base.gameObject.SetActive(false);
				}
			}
			this.RefreshInteractable();
		}

		// Token: 0x06009E92 RID: 40594 RVA: 0x004A2420 File Offset: 0x004A0620
		private void Click()
		{
			bool isConfig = this.IsConfig;
			if (isConfig)
			{
				int[] settings = ViewBottom.TempShortCutsSettings;
				ViewBottom.TempShortCutsSettings = (settings.Contains(this.templateId) ? (from x in settings
				where x != this.templateId
				select x).ToArray<int>() : settings.Append(this.templateId).ToArray<int>());
				GEvent.OnEvent(UiEvents.NotifyBottomCustomButtonChange, null);
			}
			else
			{
				switch (this.templateId)
				{
				case 0:
					UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", SingletonObject.getInstance<BasicGameData>().TaiwuCharId).Set("CanOperate", true).SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.CharacterBase, ECharacterSubPage.Team)));
					UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
					GEvent.OnEvent(UiEvents.OnNeedOpenCharacterMenuSubPage, EasyPool.Get<ArgumentBox>().SetObject("TargetSubPageIndex", ECharacterSubToggleBase.CharacterBase));
					break;
				case 1:
					UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", SingletonObject.getInstance<BasicGameData>().TaiwuCharId).Set("CanOperate", true).SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.EquipmentBase, ECharacterSubPage.None)));
					UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
					break;
				case 2:
					UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", SingletonObject.getInstance<BasicGameData>().TaiwuCharId).Set("CanOperate", true).SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.ItemBase, ECharacterSubPage.None)));
					UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
					break;
				case 3:
					UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", SingletonObject.getInstance<BasicGameData>().TaiwuCharId).Set("CanOperate", true).SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.CharacterBase, ECharacterSubPage.Prison)));
					UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
					break;
				case 4:
					UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", SingletonObject.getInstance<BasicGameData>().TaiwuCharId).Set("CanOperate", true).SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.AttainmentBase, ECharacterSubPage.Attainment)));
					UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
					break;
				case 5:
					UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", SingletonObject.getInstance<BasicGameData>().TaiwuCharId).Set("CanOperate", true).SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.PracticeBase, ECharacterSubPage.None)));
					UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
					break;
				case 6:
					UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", SingletonObject.getInstance<BasicGameData>().TaiwuCharId).Set("CanOperate", true).SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.NeiliBase, ECharacterSubPage.None)));
					UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
					break;
				case 7:
					UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", SingletonObject.getInstance<BasicGameData>().TaiwuCharId).Set("CanOperate", true).SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.EquipCombatSkillBase, ECharacterSubPage.None)));
					UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
					break;
				case 8:
					UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", SingletonObject.getInstance<BasicGameData>().TaiwuCharId).Set("CanOperate", true).SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.InformationBase, ECharacterSubPage.Information)));
					UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
					break;
				case 9:
					UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", SingletonObject.getInstance<BasicGameData>().TaiwuCharId).Set("CanOperate", true).SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.InformationBase, ECharacterSubPage.Secret)));
					UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
					break;
				case 10:
					UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", SingletonObject.getInstance<BasicGameData>().TaiwuCharId).Set("CanOperate", true).SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.StoryBase, ECharacterSubPage.None)));
					UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
					break;
				case 11:
				{
					ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
					HashSet<int> patientList = EasyPool.Get<HashSet<int>>();
					HashSet<int> doctorList = EasyPool.Get<HashSet<int>>();
					BasicGameData data = SingletonObject.getInstance<BasicGameData>();
					CharacterMonitorModel monitor = SingletonObject.getInstance<CharacterMonitorModel>();
					patientList.Add(data.TaiwuCharId);
					patientList.UnionWith(monitor.GetTaiwuTeamCharIds());
					doctorList.UnionWith(patientList);
					patientList.UnionWith(monitor.GetTaiwuSpecialGroup());
					doctorList.UnionWith(monitor.GetTaiwuGearMateGroup());
					argBox.SetObject("DoctorList", new List<int>(doctorList));
					argBox.SetObject("PatientList", new List<int>(patientList));
					argBox.Set("NeedPay", false);
					CharacterDomainMethod.AsyncCall.GetSomeoneKidnapCharacters(null, data.TaiwuCharId, delegate(int offset, RawDataPool dataPool)
					{
						KidnappedCharacterList kidnappedCharacterList = null;
						Serializer.Deserialize(dataPool, offset, ref kidnappedCharacterList);
						bool flag = kidnappedCharacterList != null;
						if (flag)
						{
							for (int i = 0; i < kidnappedCharacterList.GetCount(); i++)
							{
								patientList.Add(kidnappedCharacterList.Get(i).CharId);
							}
						}
						UIElement.Heal.SetOnInitArgs(argBox);
						UIManager.Instance.ShowUI(UIElement.Heal, true);
					});
					break;
				}
				case 12:
					BuildingActionUtils.ShowCricketCollection();
					break;
				case 13:
					BuildingActionUtils.ShowStoneHouse();
					break;
				case 14:
					BuildingActionUtils.ShowJiaoPool(null);
					break;
				case 15:
					BuildingActionUtils.ShowTeaHorseCaravan();
					break;
				case 16:
					BuildingActionUtils.ShowSamsaraPlatform();
					break;
				case 17:
					BuildingActionUtils.ShowChickenCoop();
					break;
				case 18:
					UIElement.VillagerRole.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("EnterType", ViewVillagerRole.EnterType.Normal).Set("EnterPage", ViewVillagerRole.EVillagerRolePage.RoleAssign));
					UIManager.Instance.ShowUI(UIElement.VillagerRole, true);
					break;
				case 19:
					UIElement.VillagerRole.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("EnterType", ViewVillagerRole.EnterType.Normal).Set("EnterPage", ViewVillagerRole.EVillagerRolePage.ChickenAssign));
					UIManager.Instance.ShowUI(UIElement.VillagerRole, true);
					break;
				case 20:
					UIManager.Instance.ShowUI(UIElement.JieQingInteract, true);
					break;
				case 21:
					TaiwuEventDomainMethod.Call.CloseUI("WuxianWugFairy", false, -1);
					break;
				case 22:
					UIManager.Instance.ShowUI(UIElement.ThreeVitals, true);
					break;
				case 23:
				{
					MusicPlayerModel player = SingletonObject.getInstance<MusicPlayerModel>();
					bool isEnabled = player.IsEnabled;
					if (isEnabled)
					{
						player.DisableMusicPlayer();
					}
					else
					{
						player.EnableMusicPlayer();
					}
					break;
				}
				case 24:
					ViewChickenMap.Open(null);
					break;
				}
			}
		}

		// Token: 0x06009E93 RID: 40595 RVA: 0x004A2B3C File Offset: 0x004A0D3C
		public void RefreshToggleVisibility(Action<bool> callback)
		{
			switch (this.templateId)
			{
			case 0:
			case 1:
			case 2:
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
			{
				this.button.transform.parent.gameObject.SetActive(true);
				Action<bool> callback2 = callback;
				if (callback2 != null)
				{
					callback2(true);
				}
				break;
			}
			case 3:
			{
				bool active = SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(19);
				this.button.transform.parent.gameObject.SetActive(active);
				Action<bool> callback3 = callback;
				if (callback3 != null)
				{
					callback3(active);
				}
				break;
			}
			case 12:
			{
				BuildingBlockData buildingBlockData;
				bool active = SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10) && SingletonObject.getInstance<BuildingModel>().GetBuilding(44, out buildingBlockData);
				this.button.transform.parent.gameObject.SetActive(active);
				Action<bool> callback4 = callback;
				if (callback4 != null)
				{
					callback4(active);
				}
				break;
			}
			case 13:
			{
				bool active = SingletonObject.getInstance<BuildingModel>().CanOperateStoneRoom;
				this.button.transform.parent.gameObject.SetActive(active);
				Action<bool> callback5 = callback;
				if (callback5 != null)
				{
					callback5(active);
				}
				break;
			}
			case 14:
				ExtraDomainMethod.AsyncCall.GetIsJiaoPoolOpen(this._parent, delegate(int offset, RawDataPool dataPool)
				{
					bool isOpen = false;
					Serializer.Deserialize(dataPool, offset, ref isOpen);
					bool flag2 = this.button;
					if (flag2)
					{
						this.button.transform.parent.gameObject.SetActive(isOpen);
					}
					Action<bool> callback11 = callback;
					if (callback11 != null)
					{
						callback11(isOpen);
					}
				});
				break;
			case 15:
			{
				BuildingBlockData buildingBlockData;
				bool active = SingletonObject.getInstance<BuildingModel>().GetBuilding(51, out buildingBlockData);
				this.button.transform.parent.gameObject.SetActive(active);
				Action<bool> callback6 = callback;
				if (callback6 != null)
				{
					callback6(active);
				}
				break;
			}
			case 16:
			{
				BuildingBlockData buildingBlockData;
				bool active = SingletonObject.getInstance<BuildingModel>().GetBuilding(50, out buildingBlockData);
				this.button.transform.parent.gameObject.SetActive(active);
				Action<bool> callback7 = callback;
				if (callback7 != null)
				{
					callback7(active);
				}
				break;
			}
			case 17:
			{
				BuildingBlockData buildingBlockData;
				bool active = SingletonObject.getInstance<BuildingModel>().GetBuilding(49, out buildingBlockData);
				this.button.transform.parent.gameObject.SetActive(active);
				Action<bool> callback8 = callback;
				if (callback8 != null)
				{
					callback8(active);
				}
				break;
			}
			case 18:
			{
				bool active = SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10);
				this.button.transform.parent.gameObject.SetActive(active);
				Action<bool> callback9 = callback;
				if (callback9 != null)
				{
					callback9(active);
				}
				break;
			}
			case 19:
			{
				bool flag = SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(11);
				if (flag)
				{
					OrganizationDomainMethod.AsyncCall.GetSectFunctionStatus(null, 14, SectFunctionStatuses.SectFunctionStatusType.SpecialInteractionUnlocked, delegate(int offset, RawDataPool pool)
					{
						bool unlock = false;
						Serializer.Deserialize(pool, offset, ref unlock);
						bool flag2 = this.button;
						if (flag2)
						{
							this.button.transform.parent.gameObject.SetActive(unlock);
						}
						Action<bool> callback11 = callback;
						if (callback11 != null)
						{
							callback11(unlock);
						}
					});
				}
				else
				{
					this.button.transform.parent.gameObject.SetActive(false);
					Action<bool> callback10 = callback;
					if (callback10 != null)
					{
						callback10(false);
					}
				}
				break;
			}
			case 20:
				this.button.transform.parent.gameObject.SetActive(false);
				TaiwuDomainMethod.AsyncCall.HasSectItem(this._parent, 13, delegate(int offset, RawDataPool pool)
				{
					bool interactable = false;
					Serializer.Deserialize(pool, offset, ref interactable);
					bool flag2 = this.button;
					if (flag2)
					{
						this.button.transform.parent.gameObject.SetActive(interactable);
					}
					Action<bool> callback11 = callback;
					if (callback11 != null)
					{
						callback11(interactable);
					}
				});
				break;
			case 21:
				this.button.transform.parent.gameObject.SetActive(false);
				TaiwuDomainMethod.AsyncCall.HasSectItem(this._parent, 12, delegate(int offset, RawDataPool pool)
				{
					bool interactable = false;
					Serializer.Deserialize(pool, offset, ref interactable);
					bool flag2 = this.button;
					if (flag2)
					{
						this.button.transform.parent.gameObject.SetActive(interactable);
					}
					Action<bool> callback11 = callback;
					if (callback11 != null)
					{
						callback11(interactable);
					}
				});
				break;
			case 22:
				this.button.transform.parent.gameObject.SetActive(false);
				TaiwuDomainMethod.AsyncCall.HasSectItem(this._parent, 5, delegate(int offset, RawDataPool pool)
				{
					bool interactable = false;
					Serializer.Deserialize(pool, offset, ref interactable);
					bool flag2 = this.button;
					if (flag2)
					{
						this.button.transform.parent.gameObject.SetActive(interactable);
					}
					Action<bool> callback11 = callback;
					if (callback11 != null)
					{
						callback11(interactable);
					}
				});
				break;
			case 23:
				this.button.transform.parent.gameObject.SetActive(false);
				TaiwuDomainMethod.AsyncCall.HasSectItem(this._parent, 13, delegate(int offset, RawDataPool pool)
				{
					bool interactable = false;
					Serializer.Deserialize(pool, offset, ref interactable);
					interactable &= SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(28);
					bool flag2 = this.button;
					if (flag2)
					{
						this.button.transform.parent.gameObject.SetActive(interactable);
					}
					Action<bool> callback11 = callback;
					if (callback11 != null)
					{
						callback11(interactable);
					}
				});
				break;
			case 24:
				this.button.transform.parent.gameObject.SetActive(false);
				TaiwuDomainMethod.AsyncCall.HasSectItem(this._parent, 14, false, delegate(int offset, RawDataPool pool)
				{
					bool interactable = false;
					Serializer.Deserialize(pool, offset, ref interactable);
					interactable &= (ViewChickenMap.IsChickenMapInteractable && !ViewChickenMap.AllChickenInTaiwuVillage);
					bool flag2 = this.button;
					if (flag2)
					{
						this.button.transform.parent.gameObject.SetActive(interactable);
					}
					Action<bool> callback11 = callback;
					if (callback11 != null)
					{
						callback11(interactable);
					}
				});
				break;
			default:
				this.button.transform.parent.gameObject.SetActive(false);
				break;
			}
		}

		// Token: 0x06009E94 RID: 40596 RVA: 0x004A2FA4 File Offset: 0x004A11A4
		private void RefreshInteractable()
		{
			bool isConfig = this.IsConfig;
			if (isConfig)
			{
				int[] settings = ViewBottom.TempShortCutsSettings;
				this.button.interactable = (settings.Length < 6 || settings.Contains(this.templateId));
			}
			else
			{
				bool flag = this.templateId >= 0 && Config.MainUiCustomButton.Instance[this.templateId].Category == 2 && ViewBottom.ForceDisable(SingletonObject.getInstance<AdventureRemakeModel>());
				if (flag)
				{
					this.button.interactable = false;
				}
				else
				{
					TutorialChapterModel tutorialModel = SingletonObject.getInstance<TutorialChapterModel>();
					bool flag2 = tutorialModel.InGuiding && this.templateId >= 0;
					if (flag2)
					{
						MainUiCustomButtonItem template = Config.MainUiCustomButton.Instance[this.templateId];
						bool flag3 = template.TutorialFunctionType < 0 || !tutorialModel.GetFunctionStatus(template.TutorialFunctionType);
						if (flag3)
						{
							this.button.interactable = false;
							return;
						}
					}
					switch (this.templateId)
					{
					case 0:
						this.button.interactable = true;
						break;
					case 1:
						this.button.interactable = true;
						break;
					case 2:
						this.button.interactable = true;
						break;
					case 3:
						this.button.interactable = SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(19);
						break;
					case 4:
						this.button.interactable = true;
						break;
					case 5:
						this.button.interactable = true;
						break;
					case 6:
						this.button.interactable = true;
						break;
					case 7:
						this.button.interactable = true;
						break;
					case 8:
						this.button.interactable = true;
						break;
					case 9:
						this.button.interactable = true;
						break;
					case 10:
						this.button.interactable = true;
						break;
					case 11:
						this.button.interactable = true;
						break;
					case 12:
					{
						BuildingBlockData buildingBlockData;
						this.button.interactable = (SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10) && SingletonObject.getInstance<BuildingModel>().GetBuilding(44, out buildingBlockData));
						break;
					}
					case 13:
						this.button.interactable = SingletonObject.getInstance<BuildingModel>().CanOperateStoneRoom;
						break;
					case 14:
						ExtraDomainMethod.AsyncCall.GetIsJiaoPoolOpen(this._parent, delegate(int offset, RawDataPool dataPool)
						{
							bool isOpen = false;
							Serializer.Deserialize(dataPool, offset, ref isOpen);
							bool flag5 = this.button;
							if (flag5)
							{
								this.button.interactable = isOpen;
							}
						});
						break;
					case 15:
					{
						BuildingBlockData buildingBlockData;
						this.button.interactable = SingletonObject.getInstance<BuildingModel>().GetBuilding(51, out buildingBlockData);
						break;
					}
					case 16:
					{
						BuildingBlockData buildingBlockData;
						this.button.interactable = SingletonObject.getInstance<BuildingModel>().GetBuilding(50, out buildingBlockData);
						break;
					}
					case 17:
					{
						BuildingBlockData buildingBlockData;
						this.button.interactable = SingletonObject.getInstance<BuildingModel>().GetBuilding(49, out buildingBlockData);
						break;
					}
					case 18:
						this.button.interactable = SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10);
						break;
					case 19:
					{
						bool flag4 = SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(11);
						if (flag4)
						{
							OrganizationDomainMethod.AsyncCall.GetSectFunctionStatus(null, 14, SectFunctionStatuses.SectFunctionStatusType.SpecialInteractionUnlocked, delegate(int offset, RawDataPool pool)
							{
								bool unlock = false;
								Serializer.Deserialize(pool, offset, ref unlock);
								bool flag5 = this.button;
								if (flag5)
								{
									this.button.interactable = unlock;
								}
							});
						}
						else
						{
							this.button.interactable = false;
						}
						break;
					}
					case 20:
						this.button.interactable = false;
						TaiwuDomainMethod.AsyncCall.HasSectItem(this._parent, 13, delegate(int offset, RawDataPool pool)
						{
							bool interactable = false;
							Serializer.Deserialize(pool, offset, ref interactable);
							bool flag5 = this.button;
							if (flag5)
							{
								this.button.interactable = interactable;
							}
						});
						break;
					case 21:
						this.button.interactable = false;
						TaiwuDomainMethod.AsyncCall.HasSectItem(this._parent, 12, delegate(int offset, RawDataPool pool)
						{
							bool interactable = false;
							Serializer.Deserialize(pool, offset, ref interactable);
							bool flag5 = this.button;
							if (flag5)
							{
								this.button.interactable = interactable;
							}
						});
						break;
					case 22:
						this.button.interactable = false;
						TaiwuDomainMethod.AsyncCall.HasSectItem(this._parent, 5, delegate(int offset, RawDataPool pool)
						{
							bool interactable = false;
							Serializer.Deserialize(pool, offset, ref interactable);
							bool flag5 = this.button;
							if (flag5)
							{
								this.button.interactable = interactable;
							}
						});
						break;
					case 23:
						this.button.interactable = SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(28);
						TaiwuDomainMethod.AsyncCall.HasSectItem(this._parent, 13, delegate(int offset, RawDataPool pool)
						{
							bool interactable = false;
							Serializer.Deserialize(pool, offset, ref interactable);
							bool flag5 = this.button;
							if (flag5)
							{
								this.button.interactable = interactable;
							}
						});
						break;
					case 24:
						this.button.interactable = (ViewChickenMap.IsChickenMapInteractable && !ViewChickenMap.AllChickenInTaiwuVillage);
						TaiwuDomainMethod.AsyncCall.HasSectItem(this._parent, 14, true, delegate(int offset, RawDataPool pool)
						{
							bool interactable = false;
							Serializer.Deserialize(pool, offset, ref interactable);
							bool flag5 = this.button;
							if (flag5)
							{
								this.button.interactable = interactable;
							}
						});
						break;
					}
				}
			}
		}

		// Token: 0x04007AAE RID: 31406
		public static List<int> SettingsCache = new List<int>();

		// Token: 0x04007AAF RID: 31407
		public const int MaxFixedButtonCount = 6;

		// Token: 0x04007AB0 RID: 31408
		[SerializeField]
		private bool isSelected;

		// Token: 0x04007AB1 RID: 31409
		[SerializeField]
		private CButton button;

		// Token: 0x04007AB2 RID: 31410
		[SerializeField]
		private CButton remove;

		// Token: 0x04007AB3 RID: 31411
		[SerializeField]
		private CImage img;

		// Token: 0x04007AB4 RID: 31412
		[SerializeField]
		private CImage hover;

		// Token: 0x04007AB5 RID: 31413
		[SerializeField]
		private TMP_Text buttonName;

		// Token: 0x04007AB6 RID: 31414
		[SerializeField]
		private int templateId;

		// Token: 0x04007AB7 RID: 31415
		[SerializeField]
		private int index = -1;

		// Token: 0x04007AB8 RID: 31416
		private IAsyncMethodRequestHandler _parent;
	}
}
