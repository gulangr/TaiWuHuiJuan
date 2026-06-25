using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.CommandSystem;
using FrameWork.UISystem.UIElements;
using Game.Views.Map;
using Game.Views.Select;
using GameData.Domains.Building;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Map;
using GameData.Domains.Taiwu;
using GameData.Domains.World;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Views.Bottom
{
	// Token: 0x02000C2D RID: 3117
	[RequireComponent(typeof(CButton), typeof(CImage))]
	public class BlockButton : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x06009E5F RID: 40543 RVA: 0x004A120C File Offset: 0x0049F40C
		public static void SelectGraveImpl(IAsyncMethodRequestHandler parent, int charId, Location location, HashSet<int> graveSet, AsyncMethodCallbackDelegate refresh)
		{
			BlockButton.<>c__DisplayClass2_0 CS$<>8__locals1 = new BlockButton.<>c__DisplayClass2_0();
			CS$<>8__locals1.location = location;
			CS$<>8__locals1.parent = parent;
			CS$<>8__locals1.charId = charId;
			CS$<>8__locals1.refresh = refresh;
			SelectCharacterConfigHelper.ShowSelctGraveCharacter(graveSet.ToList<int>(), new SelectCharacterCallback(CS$<>8__locals1.<SelectGraveImpl>g__OnSelectChar|2), delegate
			{
				base.<SelectGraveImpl>g__OnSelectGrave|0(-1);
			});
		}

		// Token: 0x06009E60 RID: 40544 RVA: 0x004A1264 File Offset: 0x0049F464
		public static void LockAssignImpl(IAsyncMethodRequestHandler parent, int charId, Action refresh = null)
		{
			BuildingDomainMethod.AsyncCall.RequestUnlockedWorkingVillagers(parent, delegate(int offset, RawDataPool pool)
			{
				List<int> unlockedWorkingList = new List<int>();
				Serializer.Deserialize(pool, offset, ref unlockedWorkingList);
				BuildingDomainMethod.Call.SetUnlockedWorkingVillagers(charId, !unlockedWorkingList.Contains(charId));
				Action refresh2 = refresh;
				if (refresh2 != null)
				{
					refresh2();
				}
			});
		}

		// Token: 0x06009E61 RID: 40545 RVA: 0x004A1298 File Offset: 0x0049F498
		public static void QuickAssignImpl(IAsyncMethodRequestHandler parent, AsyncMethodCallbackDelegate refresh, Location location, bool farmerFirst = false)
		{
			TaiwuDomainMethod.AsyncCall.GetVillagersForWork(parent, true, farmerFirst, delegate(int offset, RawDataPool dataPool)
			{
				List<int> charIdList = new List<int>();
				Serializer.Deserialize(dataPool, offset, ref charIdList);
				bool flag = charIdList.Count > 0;
				if (flag)
				{
					BlockButton.Template.SelectLocationWorker(parent, location, charIdList.First<int>(), refresh);
				}
			});
		}

		// Token: 0x170010BB RID: 4283
		// (get) Token: 0x06009E62 RID: 40546 RVA: 0x004A12DA File Offset: 0x0049F4DA
		public int ConsumeTime
		{
			get
			{
				int result;
				if (!this.button.interactable)
				{
					result = -1;
				}
				else
				{
					BlockButtonItem blockButtonItem = BlockButton.Instance[this.templateId];
					result = (int)((blockButtonItem != null) ? blockButtonItem.TimeConsume : -1);
				}
				return result;
			}
		}

		// Token: 0x170010BC RID: 4284
		// (get) Token: 0x06009E63 RID: 40547 RVA: 0x004A1308 File Offset: 0x0049F508
		private int ConsumeTimeRaw
		{
			get
			{
				BlockButtonItem blockButtonItem = BlockButton.Instance[this.templateId];
				return (int)((blockButtonItem != null) ? blockButtonItem.TimeConsume : -1);
			}
		}

		// Token: 0x170010BD RID: 4285
		// (get) Token: 0x06009E64 RID: 40548 RVA: 0x004A1326 File Offset: 0x0049F526
		public string ConsumeTimeDesc
		{
			get
			{
				BlockButtonItem blockButtonItem = BlockButton.Instance[this.templateId];
				return ((blockButtonItem != null) ? blockButtonItem.TimeConsumeDesc : null) ?? string.Empty;
			}
		}

		// Token: 0x06009E65 RID: 40549 RVA: 0x004A134D File Offset: 0x0049F54D
		public void Awake()
		{
			this.button.onClick.ResetListener(new Action(this.OnClick));
		}

		// Token: 0x06009E66 RID: 40550 RVA: 0x004A136D File Offset: 0x0049F56D
		public void OnPointerEnter(PointerEventData eventData)
		{
			IBlockButtonParent parent = this._parent;
			if (parent != null)
			{
				parent.OnChildEnter(this);
			}
		}

		// Token: 0x06009E67 RID: 40551 RVA: 0x004A1383 File Offset: 0x0049F583
		public void OnPointerExit(PointerEventData eventData)
		{
			IBlockButtonParent parent = this._parent;
			if (parent != null)
			{
				parent.OnChildExit(this);
			}
		}

		// Token: 0x06009E68 RID: 40552 RVA: 0x004A1398 File Offset: 0x0049F598
		public virtual void Init(IBlockButtonParent parent, byte buttonTemplateId)
		{
			this._parent = parent;
			this.templateId = buttonTemplateId;
			BlockButton.Template.InitData(this);
			this.Name = this.Name.ColorReplace();
			this.Simple = this.Simple.ColorReplace();
			this.Complex = this.Complex.ColorReplace();
		}

		// Token: 0x06009E69 RID: 40553 RVA: 0x004A13F0 File Offset: 0x0049F5F0
		public void SetText(CImage btnImage, TMP_Text btnName, TMP_Text btnSummary, TMP_Text btnDescription)
		{
			btnImage.sprite = this.image.sprite;
			btnImage.enabled = true;
			btnName.text = this.Name;
			btnSummary.text = this.Simple;
			btnDescription.text = this.Complex;
		}

		// Token: 0x06009E6A RID: 40554 RVA: 0x004A1440 File Offset: 0x0049F640
		public void OnClick()
		{
			ViewWorldMap worldMap = UIElement.WorldMap.UiBaseAs<ViewWorldMap>();
			bool flag = UIElement.WorldMap.Exist && (worldMap == null || worldMap.IsMoving);
			if (!flag)
			{
				bool flag2 = this.templateId <= 7;
				if (flag2)
				{
					this._parent.MuteOnDisable();
				}
				bool flag3 = worldMap.PlayerAtBlock != this._parent.BlockData && this.ConsumeTime >= 0;
				if (flag3)
				{
					this._parent.MoveToBlock(this);
				}
				else
				{
					BlockButton.Template.OnClick(this);
				}
			}
		}

		// Token: 0x06009E6B RID: 40555 RVA: 0x004A14D8 File Offset: 0x0049F6D8
		public IEnumerator MoveToBlock(ViewWorldMap worldMap, Location location, Action<BlockButton> onComplete = null)
		{
			this._parent.Hide(this);
			WorldMapModel model = SingletonObject.getInstance<WorldMapModel>();
			while (model.PlayerAtBlock.GetLocation() != location)
			{
				while (worldMap.IsDoingMove)
				{
					yield return null;
				}
				yield return null;
				yield return null;
				model.FindWay(location);
				IReadOnlyList<Location> movePath = model.MovePath;
				bool flag = movePath == null || movePath.Count <= 0;
				if (flag)
				{
					break;
				}
				worldMap.MoveToBlock(model.MovePath[0]);
			}
			if (onComplete != null)
			{
				onComplete(this);
			}
			yield break;
		}

		// Token: 0x04007A78 RID: 31352
		protected IBlockButtonParent _parent;

		// Token: 0x04007A79 RID: 31353
		[SerializeField]
		internal byte templateId;

		// Token: 0x04007A7A RID: 31354
		[SerializeField]
		protected TMP_Text buttonText;

		// Token: 0x04007A7B RID: 31355
		[SerializeField]
		protected CButton button;

		// Token: 0x04007A7C RID: 31356
		[SerializeField]
		protected CImage image;

		// Token: 0x04007A7D RID: 31357
		[SerializeField]
		protected CImage lockImage;

		// Token: 0x04007A7E RID: 31358
		public bool IsUnlocked;

		// Token: 0x04007A7F RID: 31359
		public string Name;

		// Token: 0x04007A80 RID: 31360
		public string Simple;

		// Token: 0x04007A81 RID: 31361
		public string Complex;

		// Token: 0x04007A82 RID: 31362
		public string PriorDisplayText;

		// Token: 0x04007A83 RID: 31363
		private bool _isDreamBackUnlockBuildingButtonClicked;

		// Token: 0x04007A84 RID: 31364
		private bool _isSecretInformationButtonClicked;

		// Token: 0x0200234C RID: 9036
		public static class Template
		{
			// Token: 0x060102FC RID: 66300 RVA: 0x0065351C File Offset: 0x0065171C
			public unsafe static void InitData(BlockButton button)
			{
				switch (button.templateId)
				{
				case 0:
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
				{
					TutorialChapterModel tutorialChapterModel = SingletonObject.getInstance<TutorialChapterModel>();
					bool flag = SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(5) || (tutorialChapterModel.InGuiding && SingletonObject.getInstance<WorldMapModel>().CurrentBlockId == button._parent.BlockData.BlockId && tutorialChapterModel.GetFunctionStatus(17));
					if (flag)
					{
						button.image.enabled = true;
						button.lockImage.enabled = false;
						button.buttonText.text = string.Format("{0}/{1}", *(ref button._parent.BlockData.CurrResources.Items.FixedElementField + (IntPtr)button.templateId * 2), *(ref button._parent.BlockData.MaxResources.Items.FixedElementField + (IntPtr)button.templateId * 2));
						button.Name = BlockButton.Instance[button.templateId].Name;
						button.Simple = BlockButton.Instance[button.templateId].Summary;
						bool flag2 = button._parent.MoveCost < 0;
						if (flag2)
						{
							button.button.interactable = false;
							button.Complex = LanguageKey.LK_BlockOperation_Cannot_Move.Tr().SetColor("brightred");
						}
						else
						{
							bool hasResource = button._parent.BlockData.CanCollectResource((sbyte)button.templateId) && *button._parent.BlockData.CurrResources[(int)button.templateId] > 0;
							bool hasTime = SingletonObject.getInstance<TimeManager>().IsActionPointEnough(button.ConsumeTimeRaw + button._parent.MoveCost);
							button.Complex = ((!hasResource) ? LanguageKey.LK_Resource_Cannot_Collect_No_Resource_Tips.Tr().SetColor("brightred") : ((!hasTime) ? LanguageKey.LK_Resource_Cannot_Collect_No_Time_Tips.Tr().SetColor("brightred") : LanguageKey.LK_Collect_Resource_Tip_Desc.Tr()));
							button.button.interactable = (hasResource && hasTime);
						}
					}
					else
					{
						button.Name = LanguageKey.LK_Not_Unlock.Tr().SetColor("brightred");
						button.buttonText.text = (button.Simple = (button.Complex = ""));
						button.button.interactable = (button.image.enabled = false);
						button.lockImage.enabled = true;
					}
					break;
				}
				case 6:
				case 7:
				{
					bool flag3 = SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10) && !SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
					if (flag3)
					{
						button.button.interactable = (button.image.enabled = true);
						button.lockImage.enabled = false;
						button.Name = BlockButton.Instance[button.templateId].Name;
						button.buttonText.text = button.Name;
						button.Simple = BlockButton.Instance[button.templateId].Summary;
						Location location2Compare = button._parent.BlockData.GetLocation();
						bool flag4 = button.button.interactable = (button._parent.MoveCost >= 0 && SingletonObject.getInstance<TimeManager>().IsActionPointEnough(button.ConsumeTimeRaw + button._parent.MoveCost));
						if (flag4)
						{
							ExtraDomainMethod.AsyncCall.FindTreasureExpect(button._parent, location2Compare, delegate(int offsetData, RawDataPool poolData)
							{
								try
								{
									TreasureExpectResult expectResult = default(TreasureExpectResult);
									Serializer.Deserialize(poolData, offsetData, ref expectResult);
									bool flag17 = expectResult.Location != location2Compare;
									if (flag17)
									{
										Debug.LogWarning(string.Format("FindTreasureExpect returns {0} which is not original location {1}", expectResult.Location, location2Compare));
										button.Complex = "";
									}
									else
									{
										int chance = expectResult.Chance;
										if (!true)
										{
										}
										LanguageKey languageKey;
										if (chance <= 66)
										{
											if (chance <= 33)
											{
												languageKey = LanguageKey.LK_Treasure_Expect_Tips_Rate_0_33;
											}
											else
											{
												languageKey = LanguageKey.LK_Treasure_Expect_Tips_Rate_33_66;
											}
										}
										else
										{
											languageKey = LanguageKey.LK_Treasure_Expect_Tips_Rate_66_100;
										}
										if (!true)
										{
										}
										LanguageKey rate = languageKey;
										sbyte maxGrade = expectResult.MaxGrade;
										if (!true)
										{
										}
										if (maxGrade < 6)
										{
											if (maxGrade < 3)
											{
												languageKey = LanguageKey.LK_Treasure_Expect_Tips_Grade_Low;
											}
											else
											{
												languageKey = LanguageKey.LK_Treasure_Expect_Tips_Grade_Middle;
											}
										}
										else
										{
											languageKey = LanguageKey.LK_Treasure_Expect_Tips_Grade_High;
										}
										if (!true)
										{
										}
										LanguageKey grade = languageKey;
										string desc = LanguageKey.LK_Treasure_Expect_Tips_Title.Tr().SetColor("GradeColor_3") + "\n" + ((expectResult.Chance == 0) ? (expectResult.AnyMaterial ? LanguageKey.LK_Treasure_Expect_Tips_Material.Tr() : LanguageKey.LK_Treasure_Expect_Tips_None.Tr()) : LanguageKey.LK_Treasure_Expect_Tips_Content.TrFormat(rate.Tr(), grade.Tr()));
										bool flag18 = expectResult.Chance > 0 || expectResult.AnyMaterial;
										if (flag18)
										{
											bool flag19 = expectResult.Chance > 0 && expectResult.AnyMaterial;
											if (flag19)
											{
												desc = desc + "\n" + LanguageKey.LK_Treasure_Expect_Tips_Material.Tr();
											}
										}
										button.Complex = desc.ColorReplace();
									}
								}
								catch (Exception e)
								{
									Debug.LogError(e);
									button.Complex = "";
								}
							});
						}
						else
						{
							button.Complex = ((button._parent.MoveCost < 0) ? LanguageKey.LK_BlockOperation_Cannot_Move : LanguageKey.LK_Treasure_Cannot_Find_No_Time_Tips).Tr().SetColor("brightred");
							button.button.interactable = false;
						}
					}
					else
					{
						button.Name = LanguageKey.LK_Not_Unlock.Tr().SetColor("brightred");
						button.buttonText.text = (button.Simple = (button.Complex = ""));
						button.button.interactable = (button.image.enabled = false);
						button.lockImage.enabled = true;
					}
					break;
				}
				case 8:
				case 9:
				{
					BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
					bool flag5 = !SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10);
					if (flag5)
					{
						button.Name = LanguageKey.LK_Not_Unlock.Tr().SetColor("brightred");
						button.buttonText.text = (button.Simple = (button.Complex = ""));
						button.button.interactable = (button.image.enabled = false);
						button.lockImage.enabled = true;
					}
					else
					{
						button.button.interactable = (button.image.enabled = true);
						button.lockImage.enabled = false;
						bool isMarked = buildingModel.CheckBlockIsMarked(button._parent.BlockData.GetLocation());
						bool flag6 = button.templateId == 9 ^ isMarked;
						if (flag6)
						{
							button.gameObject.SetActive(false);
						}
						else
						{
							button.gameObject.SetActive(true);
							button.Name = BlockButton.Instance[button.templateId].Name;
							button.buttonText.text = button.Name;
							button.Simple = BlockButton.Instance[button.templateId].Summary;
							button.Complex = BlockButton.Instance[button.templateId].Desc;
						}
					}
					break;
				}
				case 10:
				{
					bool flag7 = !SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10);
					if (flag7)
					{
						button.Name = LanguageKey.LK_Not_Unlock.Tr().SetColor("brightred");
						button.buttonText.text = (button.Simple = (button.Complex = ""));
						button.button.interactable = (button.image.enabled = false);
						button.lockImage.enabled = true;
					}
					else
					{
						button.button.interactable = (button.image.enabled = true);
						button.lockImage.enabled = false;
						bool flag8 = button._parent.CharId != -1;
						if (flag8)
						{
							button.gameObject.SetActive(false);
						}
						else
						{
							button.gameObject.SetActive(true);
							button.Name = BlockButton.Instance[button.templateId].Name;
							button.buttonText.text = button.Name;
							button.Simple = BlockButton.Instance[button.templateId].Summary;
							button.Complex = BlockButton.Instance[button.templateId].Desc;
						}
					}
					break;
				}
				case 11:
				case 12:
				case 14:
				case 15:
				case 16:
				case 17:
				{
					bool flag9 = !SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10);
					if (flag9)
					{
						button.gameObject.SetActive(false);
					}
					else
					{
						button.button.interactable = (button.image.enabled = true);
						button.lockImage.enabled = false;
						bool flag10 = button._parent.CharId == -1;
						if (flag10)
						{
							button.gameObject.SetActive(false);
						}
						else
						{
							button.gameObject.SetActive(true);
							button.Name = BlockButton.Instance[button.templateId].Name;
							bool flag11 = button.buttonText != null && button.templateId != 11;
							if (flag11)
							{
								button.buttonText.text = button.Name;
							}
							button.Simple = BlockButton.Instance[button.templateId].Summary;
							button.Complex = BlockButton.Instance[button.templateId].Desc;
						}
						bool flag12;
						if (button.templateId == 17)
						{
							Selectable button2 = button.button;
							HashSet<int> graveSet = button._parent.BlockData.GraveSet;
							flag12 = !(button2.interactable = (graveSet != null && graveSet.Count > 0));
						}
						else
						{
							flag12 = false;
						}
						bool flag13 = flag12;
						if (flag13)
						{
							button.Complex = LanguageKey.LK_VillagerWork_Tips_NoGrave.Tr();
						}
					}
					break;
				}
				case 13:
				{
					bool flag14 = !SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10);
					if (flag14)
					{
						bool flag15 = button.buttonText != null;
						if (flag15)
						{
							button.buttonText.text = "";
						}
						button.button.interactable = (button.image.enabled = false);
						button.lockImage.enabled = true;
					}
					else
					{
						button.button.interactable = (button.image.enabled = true);
						button.lockImage.enabled = false;
						button.gameObject.SetActive(true);
						button.Name = BlockButton.Instance[button.templateId].Name;
						bool flag16 = button.buttonText != null;
						if (flag16)
						{
							button.buttonText.text = button.Name;
						}
						button.Simple = BlockButton.Instance[button.templateId].Summary;
						button.Complex = BlockButton.Instance[button.templateId].Desc;
					}
					break;
				}
				default:
					Debug.LogError("unknown type: " + button.templateId.ToString());
					break;
				}
			}

			// Token: 0x060102FD RID: 66301 RVA: 0x00654270 File Offset: 0x00652470
			public unsafe static void TimeCollect(BlockButton button, sbyte selectedResourceType)
			{
				button._parent.DisableMove = true;
				bool collectResourceIsMax = *(ref button._parent.BlockData.CurrResources.Items.FixedElementField + (IntPtr)selectedResourceType * 2) >= *(ref button._parent.BlockData.MaxResources.Items.FixedElementField + (IntPtr)selectedResourceType * 2);
				Action <>9__1;
				MapDomainMethod.AsyncCall.CollectResource(button._parent, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, selectedResourceType, delegate(int offset, RawDataPool dataPool)
				{
					button._parent.Hide(button);
					CollectResourceResult result = default(CollectResourceResult);
					Serializer.Deserialize(dataPool, offset, ref result);
					ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
					argBox.Set("CollectResourceIsMax", collectResourceIsMax);
					argBox.Set("CollectType", 0);
					argBox.SetObject("CollectInfo", new List<CollectResourceResult>
					{
						result
					});
					UIElement.CollectResource.SetOnInitArgs(argBox);
					UIElement collectResource = UIElement.CollectResource;
					Delegate onHide = collectResource.OnHide;
					Action b;
					if ((b = <>9__1) == null)
					{
						b = (<>9__1 = delegate()
						{
							button._parent.DisableMove = false;
							button._parent.OnChildExit(button);
							WorldDomainMethod.Call.AdvanceDaysInMonth(1);
						});
					}
					collectResource.OnHide = (Action)Delegate.Combine(onHide, b);
					UIManager.Instance.ShowUI(UIElement.CollectResource, true);
				});
			}

			// Token: 0x060102FE RID: 66302 RVA: 0x0065431C File Offset: 0x0065251C
			public static void AddMark(IBlockButtonParent parent)
			{
				BuildingDomainMethod.AsyncCall.AddLocationMark(parent, parent.BlockData.GetLocation(), delegate(int _, RawDataPool _)
				{
					parent.Refresh();
				});
			}

			// Token: 0x060102FF RID: 66303 RVA: 0x00654360 File Offset: 0x00652560
			public static void RemoveMark(IBlockButtonParent parent, MapBlockData mapBlockData)
			{
				BuildingDomainMethod.AsyncCall.RemoveLocationMark(parent, parent.BlockData.GetLocation(), delegate(int _, RawDataPool _)
				{
					parent.Refresh();
				});
			}

			// Token: 0x06010300 RID: 66304 RVA: 0x006543A4 File Offset: 0x006525A4
			public static void SelectLocationWorker(IAsyncMethodRequestHandler parent, Location location, int charId, AsyncMethodCallbackDelegate refresh)
			{
				bool flag = charId == -1;
				if (flag)
				{
					TaiwuDomainMethod.Call.StopVillagerWorkOptional(-1, location.AreaId, location.BlockId, 13, true);
					TaiwuDomainMethod.AsyncCall.StopVillagerWorkOptional(parent, location.AreaId, location.BlockId, 12, true, refresh);
				}
				else
				{
					TaiwuDomainMethod.AsyncCall.SetVillagerIdleWork(parent, charId, location.AreaId, location.BlockId, refresh);
				}
			}

			// Token: 0x06010301 RID: 66305 RVA: 0x00654400 File Offset: 0x00652600
			public static void OpenSelectWindow(IAsyncMethodRequestHandler parent, int charId, Location location, Action refresh)
			{
				AsyncMethodCallbackDelegate <>9__2;
				SelectCharacterCallback <>9__1;
				TaiwuDomainMethod.AsyncCall.GetVillagersAvailableForWorkDisplayData(parent, false, delegate(int offset, RawDataPool dataPool)
				{
					List<VillagerSelectCharacterDisplayData> displayDataList = new List<VillagerSelectCharacterDisplayData>();
					Serializer.Deserialize(dataPool, offset, ref displayDataList);
					List<ISelectCharacterData> dataList = VillagerSelectCharacterSelectionHelper.CreateDataList(displayDataList);
					CommonSelectCharacterConfig config = VillagerSelectCharacterSelectionHelper.CreateVillagerSingleSlotConfig(BlockButton.Instance[10].Name, charId, ESelectCharacterSubPage.Villager);
					UIElement selectChar = UIElement.SelectChar;
					ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().SetObject("SelectCharacterConfig", config).SetObject("SelectCharacterDataList", dataList);
					string key = "SelectCharacterCallback";
					SelectCharacterCallback arg;
					if ((arg = <>9__1) == null)
					{
						arg = (<>9__1 = delegate(List<int> selectedIds)
						{
							int charId2 = (selectedIds != null && selectedIds.Count > 0) ? selectedIds[0] : -1;
							bool flag = charId2 != -1;
							if (flag)
							{
								BuildingDomainMethod.Call.SetUnlockedWorkingVillagers(charId2, false);
							}
							bool flag2 = charId2 != -1;
							if (flag2)
							{
								BuildingDomainMethod.Call.SetUnlockedWorkingVillagers(charId2, true);
							}
							IAsyncMethodRequestHandler parent2 = parent;
							Location location2 = location;
							int charId3 = charId2;
							AsyncMethodCallbackDelegate refresh2;
							if ((refresh2 = <>9__2) == null)
							{
								refresh2 = (<>9__2 = delegate(int _, RawDataPool _)
								{
									Action refresh3 = refresh;
									if (refresh3 != null)
									{
										refresh3();
									}
								});
							}
							BlockButton.Template.SelectLocationWorker(parent2, location2, charId3, refresh2);
						});
					}
					selectChar.SetOnInitArgs(argumentBox.SetObject(key, arg));
					UIManager.Instance.MaskUI(UIElement.SelectChar);
				});
			}

			// Token: 0x06010302 RID: 66306 RVA: 0x0065444C File Offset: 0x0065264C
			public static void OnClick(BlockButton button)
			{
				BlockButton.Template.<>c__DisplayClass6_0 CS$<>8__locals1 = new BlockButton.Template.<>c__DisplayClass6_0();
				CS$<>8__locals1.button = button;
				switch (CS$<>8__locals1.button.templateId)
				{
				case 0:
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
					BlockButton.Template.TimeCollect(CS$<>8__locals1.button, (sbyte)CS$<>8__locals1.button.templateId);
					break;
				case 6:
					CommandManager.AddCommandShowUI(EPriority.ShowUINormal, UIElement.CollectResource, EasyPool.Get<ArgumentBox>().Set("IsDigSeries", false).Set("CollectType", 4));
					break;
				case 7:
					CommandManager.AddCommandShowUI(EPriority.ShowUINormal, UIElement.CollectResource, EasyPool.Get<ArgumentBox>().Set("IsDigSeries", true).Set("CollectType", 4));
					break;
				case 8:
				case 9:
				{
					Location location = CS$<>8__locals1.button._parent.BlockData.GetLocation();
					BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
					bool hasWorker = buildingModel.CheckBlockHasWork(location, -1);
					bool flag = hasWorker || buildingModel.CheckBlockIsMarked(location);
					if (flag)
					{
						bool flag2 = hasWorker;
						if (flag2)
						{
							UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new DialogCmd
							{
								Title = LanguageKey.LK_Mark_Cancel_Tip_Title.Tr(),
								Content = LanguageKey.LK_Mark_Cancel_Tip_Desc2.Tr(),
								Type = 1,
								Yes = delegate()
								{
									BlockButton.Template.RemoveMark(CS$<>8__locals1.button._parent, CS$<>8__locals1.button._parent.BlockData);
									SingletonObject.getInstance<WorldMapModel>().RequestStopVillagerWorkOnMap(location, true);
								},
								No = null
							}));
							UIManager.Instance.MaskUI(UIElement.Dialog);
						}
						else
						{
							BlockButton.Template.RemoveMark(CS$<>8__locals1.button._parent, CS$<>8__locals1.button._parent.BlockData);
						}
					}
					else
					{
						BlockButton.Template.AddMark(CS$<>8__locals1.button._parent);
					}
					break;
				}
				case 10:
				case 11:
					BlockButton.Template.OpenSelectWindow(CS$<>8__locals1.button._parent, CS$<>8__locals1.button._parent.CharId, CS$<>8__locals1.button._parent.BlockData.GetLocation(), new Action(CS$<>8__locals1.button._parent.Refresh));
					break;
				case 12:
					BlockButton.Template.SelectLocationWorker(CS$<>8__locals1.button._parent, CS$<>8__locals1.button._parent.BlockData.GetLocation(), -1, delegate(int _, RawDataPool _)
					{
						CS$<>8__locals1.button._parent.Refresh();
					});
					CS$<>8__locals1.button._parent.OnChildExit(CS$<>8__locals1.button);
					break;
				case 13:
					BlockButton.QuickAssignImpl(CS$<>8__locals1.button._parent, delegate(int _, RawDataPool _)
					{
						CS$<>8__locals1.button._parent.Refresh();
					}, CS$<>8__locals1.button._parent.BlockData.GetLocation(), false);
					break;
				case 14:
					BlockButton.LockAssignImpl(CS$<>8__locals1.button._parent, CS$<>8__locals1.button._parent.CharId, new Action(CS$<>8__locals1.button._parent.Refresh));
					break;
				case 15:
					CS$<>8__locals1.button._parent.ExtraViewOpened = true;
					UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", CS$<>8__locals1.button._parent.CharId));
					UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
					break;
				case 16:
					TaiwuDomainMethod.AsyncCall.SetVillagerIdleWork(CS$<>8__locals1.button._parent, CS$<>8__locals1.button._parent.CharId, CS$<>8__locals1.button._parent.BlockData.AreaId, CS$<>8__locals1.button._parent.BlockData.BlockId, delegate(int _, RawDataPool _)
					{
						CS$<>8__locals1.button._parent.Refresh();
					});
					break;
				case 17:
				{
					HashSet<int> graveSet;
					bool flag3;
					if (CS$<>8__locals1.button._parent.CharId != -1)
					{
						graveSet = CS$<>8__locals1.button._parent.BlockData.GraveSet;
						flag3 = (graveSet != null && graveSet.Count > 0);
					}
					else
					{
						flag3 = false;
					}
					bool flag4 = flag3;
					if (flag4)
					{
						CS$<>8__locals1.button._parent.ExtraViewOpened = true;
						BlockButton.SelectGraveImpl(CS$<>8__locals1.button._parent, CS$<>8__locals1.button._parent.CharId, CS$<>8__locals1.button._parent.BlockData.GetLocation(), graveSet, delegate(int _, RawDataPool _)
						{
							CS$<>8__locals1.button._parent.Refresh();
						});
					}
					break;
				}
				default:
					Debug.LogError("unknown type: " + CS$<>8__locals1.button.templateId.ToString());
					CS$<>8__locals1.button._parent.Hide(CS$<>8__locals1.button);
					break;
				}
			}
		}
	}
}
