using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Coffee.UIExtensions;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Views.Building;
using GameData.Domains.Building;
using GameData.Domains.Organization;
using GameData.Domains.Organization.Display;
using GameData.Serializer;
using GameData.Utilities;
using Spine.Unity;
using UnityEngine;

namespace Game.Components.Building
{
	// Token: 0x02000F66 RID: 3942
	public class SettlementRoadAnimation : MonoBehaviour
	{
		// Token: 0x0600B45F RID: 46175 RVA: 0x00520FAD File Offset: 0x0051F1AD
		public void SetBuildingArea(ViewBuildingArea buildingArea)
		{
			this._buildingArea = buildingArea;
			this.villagerInit = false;
		}

		// Token: 0x0600B460 RID: 46176 RVA: 0x00520FC0 File Offset: 0x0051F1C0
		public void TryInit(short settlementId, Dictionary<Vector2Int, Vector2Int> roadCrossPos)
		{
			this._roadCrossPos = roadCrossPos;
			try
			{
				this.InitChickens();
				this.InitVillagerAnimation(settlementId);
				this._buildingArea.ShowAfterRefresh();
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		// Token: 0x0600B461 RID: 46177 RVA: 0x00521010 File Offset: 0x0051F210
		private void InitVillagerAnimation(short settlementId)
		{
			SettlementPopulationDisplayData displayData = new SettlementPopulationDisplayData();
			bool flag = !SingletonObject.getInstance<GlobalSettings>().VillagerAnimation;
			if (flag)
			{
				this.InitVillagerAnimation(displayData);
			}
			else
			{
				OrganizationDomainMethod.AsyncCall.GetSettlementPopulationDisplayData(null, settlementId, delegate(int offset2, RawDataPool dataPool2)
				{
					Serializer.Deserialize(dataPool2, offset2, ref displayData);
					this.InitVillagerAnimation(displayData);
				});
			}
		}

		// Token: 0x0600B462 RID: 46178 RVA: 0x00521070 File Offset: 0x0051F270
		public void InitVillagerAnimation(SettlementPopulationDisplayData data)
		{
			List<Vector2Int> posKeyList = EasyPool.Get<List<Vector2Int>>();
			List<Vector2Int> neighborList = EasyPool.Get<List<Vector2Int>>();
			posKeyList.Clear();
			posKeyList.AddRange(from key in this._roadCrossPos.Keys
			where Mathf.Abs(key.x) <= 100 && Mathf.Abs(key.y) <= 100
			select key);
			int totalCount = data.ManCount + data.WomanCount + data.BoyCount + data.GirlCount;
			for (int index = 0; index < totalCount; index++)
			{
				bool flag = index < this.villagerDataList.Count;
				SettlementRoadAnimation.VillagerData villagerData;
				if (flag)
				{
					villagerData = this.villagerDataList[index];
				}
				else
				{
					villagerData = new SettlementRoadAnimation.VillagerData();
					villagerData.GameObject = this.VillagerObjectPool.GetObject();
					this.villagerDataList.Add(villagerData);
					this.moveInRoadDataList.Clear();
				}
				villagerData.Cross1 = posKeyList[Random.Range(0, posKeyList.Count)];
				this._buildingArea.GetNeighborCross(villagerData.Cross1, neighborList);
				bool flag2 = neighborList.Count > 0;
				if (flag2)
				{
					villagerData.Cross2 = neighborList.GetOrDefault(Random.Range(0, neighborList.Count));
				}
				bool adult = index < data.ManCount + data.WomanCount;
				sbyte gender = (index < data.ManCount || (index >= data.ManCount + data.WomanCount && index < data.ManCount + data.WomanCount + data.BoyCount)) ? 1 : 0;
				bool flag3 = adult && gender == 1;
				SkeletonDataAsset skeletonGraphicRes;
				if (flag3)
				{
					skeletonGraphicRes = this.villagerSkeletonDataAssets[0];
				}
				else
				{
					bool flag4 = adult && gender == 0;
					if (flag4)
					{
						skeletonGraphicRes = this.villagerSkeletonDataAssets[1];
					}
					else
					{
						bool flag5 = !adult && gender == 1;
						if (flag5)
						{
							skeletonGraphicRes = this.villagerSkeletonDataAssets[2];
						}
						else
						{
							skeletonGraphicRes = this.villagerSkeletonDataAssets[3];
						}
					}
				}
				SkeletonAnimation skeletonGraphic = villagerData.SkeletonComponent;
				skeletonGraphic.skeletonDataAsset = skeletonGraphicRes;
				skeletonGraphic.Initialize(true, false);
				villagerData.Gender = gender;
				villagerData.Adult = adult;
				villagerData.LinerValue = (float)Random.Range(0, 10) / 10f;
				villagerData.LinerSpeed = this.GetVillagerMoveSpeed(villagerData);
				Vector2Int pos = this._roadCrossPos[villagerData.Cross1];
				Vector2Int pos2 = this._roadCrossPos[villagerData.Cross2];
				Vector2Int pos3 = new Vector2Int((int)((float)pos.x + (float)(pos2.x - pos.x) * Random.Range(0.1f, 1f)), (int)((float)pos.y + (float)(pos2.y - pos.y) * Random.Range(0.1f, 1f)));
				RectTransform villagerTrans = villagerData.GameObject.GetComponent<RectTransform>();
				villagerTrans.SetParent(this.animationHolder, false);
				SettlementRoadAnimation.SetSpinePosition(villagerTrans, new Vector2((float)pos3.x, (float)pos3.y));
				villagerData.GameObject.SetActive(true);
				this.UpdateVillagerSkeleton(villagerData);
			}
			EasyPool.Free<List<Vector2Int>>(posKeyList);
			EasyPool.Free<List<Vector2Int>>(neighborList);
			for (int i = this.villagerDataList.Count - 1; i >= totalCount; i--)
			{
				this.VillagerObjectPool.DestroyObject(this.villagerDataList[i].GameObject);
				this.villagerDataList.RemoveAt(i);
				this.moveInRoadDataList.Clear();
			}
			this.villagerInit = true;
		}

		// Token: 0x0600B463 RID: 46179 RVA: 0x005213FC File Offset: 0x0051F5FC
		private float GetVillagerMoveSpeed(SettlementRoadAnimation.VillagerData villagerData)
		{
			bool flag = !villagerData.Adult;
			float result2;
			if (flag)
			{
				result2 = Random.Range(2f, 3f);
			}
			else
			{
				bool flag2 = villagerData.Gender == 1;
				float wSlow;
				float wMedium;
				float wFast;
				if (flag2)
				{
					wSlow = 2f;
					wMedium = 3f;
					wFast = 5f;
				}
				else
				{
					wSlow = 5f;
					wMedium = 3f;
					wFast = 2f;
				}
				float totalWeight = wSlow + wMedium + wFast;
				float randomValue = Random.value * totalWeight;
				float result = 4f;
				bool flag3 = randomValue < wSlow;
				if (flag3)
				{
					result = 2f;
				}
				else
				{
					bool flag4 = randomValue < wSlow + wMedium;
					if (flag4)
					{
						result = 3f;
					}
				}
				result2 = Random.Range(result - 1f, result + 1f);
			}
			return result2;
		}

		// Token: 0x0600B464 RID: 46180 RVA: 0x005214C8 File Offset: 0x0051F6C8
		private void UpdateVillagerSkeleton(SettlementRoadAnimation.VillagerData villager)
		{
			SkeletonAnimation skeletonGraphic = villager.SkeletonComponent;
			this.UpdateVillagerAnimation(skeletonGraphic, false);
			Vector2Int direction = SettlementRoadAnimation.GetMoveDirection(villager.Cross1, villager.Cross2);
			string skinName = this.GetVillagerSkinName(direction);
			skeletonGraphic.Skeleton.SetSkin(skinName);
			string slotName = this.GetVillagerSlotName(skinName);
			skeletonGraphic.Skeleton.SetAttachment(slotName, this.GetVillagerAttachmentName(villager, skinName));
			villager.GameObject.transform.localScale = this.GetVillagerScale(direction);
		}

		// Token: 0x0600B465 RID: 46181 RVA: 0x00521544 File Offset: 0x0051F744
		private void UpdateVillagerAnimation(IAnimationStateComponent comp, bool stop)
		{
			string animationName = "walk";
			comp.AnimationState.SetAnimation(0, animationName, true);
		}

		// Token: 0x0600B466 RID: 46182 RVA: 0x00521568 File Offset: 0x0051F768
		private string GetVillagerSkinName(Vector2Int direction)
		{
			bool flag = direction == Vector2Int.up;
			string result;
			if (flag)
			{
				result = "back";
			}
			else
			{
				bool flag2 = direction == Vector2Int.down;
				if (flag2)
				{
					result = "front";
				}
				else
				{
					result = "side";
				}
			}
			return result;
		}

		// Token: 0x0600B467 RID: 46183 RVA: 0x005215B0 File Offset: 0x0051F7B0
		private string GetVillagerAttachmentName(SettlementRoadAnimation.VillagerData villagerData, string skinName)
		{
			bool flag = villagerData.Gender == 1 && villagerData.Adult;
			string prefix;
			if (flag)
			{
				prefix = "man";
			}
			else
			{
				bool flag2 = villagerData.Gender == 0 && villagerData.Adult;
				if (flag2)
				{
					prefix = "woman";
				}
				else
				{
					bool flag3 = villagerData.Gender == 1 && !villagerData.Adult;
					if (flag3)
					{
						prefix = "boy";
					}
					else
					{
						prefix = "girl";
					}
				}
			}
			return string.Format("{0}_{1}_{2}_body", prefix, Random.Range(1, 2), skinName);
		}

		// Token: 0x0600B468 RID: 46184 RVA: 0x00521644 File Offset: 0x0051F844
		private string GetVillagerSlotName(string skinName)
		{
			return skinName + "_body";
		}

		// Token: 0x0600B469 RID: 46185 RVA: 0x00521664 File Offset: 0x0051F864
		private Vector3 GetVillagerScale(Vector2Int direction)
		{
			bool flag = direction == Vector2Int.right;
			Vector3 result;
			if (flag)
			{
				result = new Vector3(-1f, 1f, 1f) * this.peopleScale;
			}
			else
			{
				result = Vector3.one * this.peopleScale;
			}
			return result;
		}

		// Token: 0x0600B46A RID: 46186 RVA: 0x005216B8 File Offset: 0x0051F8B8
		private void Update()
		{
			bool flag = UIManager.Instance.IsFocusElement(UIElement.BuildingArea) || UIManager.Instance.IsFocusElement(UIElement.BuildingQuickActionMenu);
			if (flag)
			{
				bool flag2 = this.villagerInit && this.ChickenInit;
				if (flag2)
				{
					this.UpdateMove();
					this.CheckChickenHold();
					this.UpdateSibling();
				}
			}
		}

		// Token: 0x0600B46B RID: 46187 RVA: 0x0052171B File Offset: 0x0051F91B
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void SetSpinePosition(RectTransform trans, Vector3 postion)
		{
			postion.z = postion.y / 600f;
			trans.anchoredPosition3D = postion;
		}

		// Token: 0x0600B46C RID: 46188 RVA: 0x0052173C File Offset: 0x0051F93C
		private void UpdateMove()
		{
			List<Vector2Int> neighborList = EasyPool.Get<List<Vector2Int>>();
			for (int i = 0; i < this.moveInRoadDataList.Count; i++)
			{
				SettlementRoadAnimation.MoveInRoadData moveInRoadData = this.moveInRoadDataList[i];
				bool pointerEntered = moveInRoadData.PointerEntered;
				if (!pointerEntered)
				{
					bool flag = moveInRoadData.LinerValue >= 1f;
					if (flag)
					{
						moveInRoadData.LinerValue = 0f;
						Vector2Int fromNode = moveInRoadData.Cross1;
						moveInRoadData.Cross1 = moveInRoadData.Cross2;
						this._buildingArea.GetNeighborCross(moveInRoadData.Cross1, neighborList);
						bool flag2 = neighborList.Count > 1;
						if (flag2)
						{
							neighborList.Remove(fromNode);
						}
						bool flag3 = neighborList.Count > 0;
						if (flag3)
						{
							moveInRoadData.Cross2 = neighborList[Random.Range(0, neighborList.Count)];
							float roadWidthHalf = 15f;
							moveInRoadData.TargetLaneOffset = ((Random.value > 0.5f) ? 1f : -1f) * roadWidthHalf;
						}
						moveInRoadData.Direction = SettlementRoadAnimation.GetMoveDirection(moveInRoadData.Cross1, moveInRoadData.Cross2);
						SettlementRoadAnimation.VillagerData data = moveInRoadData as SettlementRoadAnimation.VillagerData;
						bool flag4 = data != null;
						if (flag4)
						{
							this.UpdateVillagerSkeleton(data);
						}
						else
						{
							this.UpdateChickenSkeleton((SettlementRoadAnimation.ChickenData)moveInRoadData);
						}
					}
					else
					{
						bool flag5 = moveInRoadData.HoldMoveUntilTime > Time.time;
						if (!flag5)
						{
							bool holding = moveInRoadData.Holding;
							if (holding)
							{
								moveInRoadData.Holding = false;
								SettlementRoadAnimation.VillagerData data2 = moveInRoadData as SettlementRoadAnimation.VillagerData;
								bool flag6 = data2 != null;
								if (flag6)
								{
									moveInRoadData.LinerSpeed = this.GetVillagerMoveSpeed(data2);
									this.UpdateVillagerSkeleton(data2);
								}
								else
								{
									moveInRoadData.LinerSpeed = (float)this.GetChickenMoveSpeed();
									this.UpdateChickenSkeleton((SettlementRoadAnimation.ChickenData)moveInRoadData);
								}
							}
							moveInRoadData.LinerValue += moveInRoadData.LinerSpeed * 0.0005f;
							bool flag7 = moveInRoadData.LinerValue > 1f;
							if (flag7)
							{
								moveInRoadData.LinerValue = 1f;
							}
							RectTransform rectTransform = moveInRoadData.GameObject.GetComponent<RectTransform>();
							Vector2 pos = rectTransform.anchoredPosition;
							float currentT = moveInRoadData.LinerValue;
							Vector2Int startPos;
							Vector2Int endPos;
							bool flag8 = this._roadCrossPos.TryGetValue(moveInRoadData.Cross1, out startPos) && this._roadCrossPos.TryGetValue(moveInRoadData.Cross2, out endPos);
							if (flag8)
							{
								pos.x = Mathf.Lerp((float)startPos.x, (float)endPos.x, currentT);
								pos.y = Mathf.Lerp((float)startPos.y, (float)endPos.y, currentT);
								bool flag9 = moveInRoadData.TargetLaneOffset != 0f;
								float finalOffset;
								if (flag9)
								{
									float blendFactor = 1f;
									bool flag10 = currentT > 0.75f;
									if (flag10)
									{
										float tailT = Mathf.InverseLerp(0.75f, 1f, currentT);
										blendFactor = 1f - tailT;
									}
									else
									{
										bool flag11 = currentT < 0.25f;
										if (flag11)
										{
											float headT = Mathf.InverseLerp(0f, 0.25f, currentT);
											blendFactor = headT;
										}
									}
									float blendedTarget = moveInRoadData.TargetLaneOffset * blendFactor;
									float smoothSpeed = moveInRoadData.OffsetSmoothSpeed * Time.deltaTime;
									smoothSpeed = Mathf.Min(smoothSpeed, 1f);
									moveInRoadData.CurrentLaneOffset = Mathf.Lerp(moveInRoadData.CurrentLaneOffset, blendedTarget, smoothSpeed);
									finalOffset = moveInRoadData.CurrentLaneOffset;
								}
								else
								{
									moveInRoadData.CurrentLaneOffset = Mathf.Lerp(moveInRoadData.CurrentLaneOffset, 0f, 10f * Time.deltaTime);
									finalOffset = moveInRoadData.CurrentLaneOffset;
								}
								bool flag12 = Mathf.Abs(finalOffset) > 0.01f;
								if (flag12)
								{
									Vector2 direction = endPos - startPos;
									bool flag13 = direction.sqrMagnitude > 0.0001f;
									if (flag13)
									{
										Vector2 normal = new Vector2(direction.y, -direction.x);
										normal.Normalize();
										pos += normal * finalOffset;
									}
								}
							}
							else
							{
								moveInRoadData.Cross1 = this.GetNearestCross(moveInRoadData);
								moveInRoadData.Cross2 = moveInRoadData.Cross1;
								moveInRoadData.LinerValue = 1f;
							}
							SettlementRoadAnimation.SetSpinePosition(rectTransform, pos);
						}
					}
				}
			}
			EasyPool.Free<List<Vector2Int>>(neighborList);
		}

		// Token: 0x0600B46D RID: 46189 RVA: 0x00521B68 File Offset: 0x0051FD68
		private Vector2Int GetNearestCross(SettlementRoadAnimation.MoveInRoadData data)
		{
			Vector2Int crossKey = data.Cross1;
			Vector2Int nearestCross = new Vector2Int(-1, -1);
			int minDistance = int.MaxValue;
			foreach (KeyValuePair<Vector2Int, Vector2Int> roadCross in this._roadCrossPos)
			{
				int distance = Mathf.RoundToInt(Vector2Int.Distance(roadCross.Key, crossKey));
				bool flag = distance < minDistance;
				if (flag)
				{
					minDistance = distance;
					nearestCross = roadCross.Key;
				}
			}
			return nearestCross;
		}

		// Token: 0x0600B46E RID: 46190 RVA: 0x00521C04 File Offset: 0x0051FE04
		private void UpdateSibling()
		{
			bool flag = this.moveInRoadDataList.Count <= 0;
			if (flag)
			{
				this.moveInRoadDataList.AddRange(this.villagerDataList);
				this.moveInRoadDataList.AddRange(this._chickenDataList);
			}
		}

		// Token: 0x0600B46F RID: 46191 RVA: 0x00521C50 File Offset: 0x0051FE50
		private void InitChickens()
		{
			List<GameData.Domains.Building.Chicken> chickenList = this.ChickenList;
			int chickenCount = (chickenList != null) ? chickenList.Count : 0;
			bool flag = chickenCount >= 0;
			if (flag)
			{
				List<Vector2Int> posKeyList = EasyPool.Get<List<Vector2Int>>();
				List<Vector2Int> neighborList = EasyPool.Get<List<Vector2Int>>();
				posKeyList.Clear();
				posKeyList.AddRange(from key in this._roadCrossPos.Keys
				where Mathf.Abs(key.x) <= 100 && Mathf.Abs(key.y) <= 100
				select key);
				for (int i = 0; i < chickenCount; i++)
				{
					bool flag2 = i < this._chickenDataList.Count;
					SettlementRoadAnimation.ChickenData chicken;
					if (flag2)
					{
						chicken = this._chickenDataList[i];
					}
					else
					{
						chicken = new SettlementRoadAnimation.ChickenData
						{
							GameObject = this.ChickenObjectPool.GetObject()
						};
						this._chickenDataList.Add(chicken);
						this.moveInRoadDataList.Clear();
					}
					chicken.Cross1 = posKeyList[Random.Range(0, posKeyList.Count)];
					this._buildingArea.GetNeighborCross(chicken.Cross1, neighborList);
					bool flag3 = neighborList.Count > 0;
					if (flag3)
					{
						chicken.Cross2 = neighborList.GetOrDefault(Random.Range(0, neighborList.Count));
					}
					chicken.Id = this.ChickenList[i].Id;
					chicken.TemplateId = this.ChickenList[i].TemplateId;
					chicken.LinerValue = (float)Random.Range(0, 10) / 10f;
					chicken.LinerSpeed = (float)this.GetChickenMoveSpeed();
					Vector2Int pos = this._roadCrossPos[chicken.Cross1];
					Vector2Int pos2 = this._roadCrossPos[chicken.Cross2];
					Vector2Int chickenPos = new Vector2Int((int)((float)pos.x + (float)(pos2.x - pos.x) * Random.Range(0.1f, 1f)), (int)((float)pos.y + (float)(pos2.y - pos.y) * Random.Range(0.1f, 1f)));
					RectTransform chickenTrans = chicken.GameObject.GetComponent<RectTransform>();
					chickenTrans.SetParent(this.animationHolder, false);
					SettlementRoadAnimation.SetSpinePosition(chickenTrans, new Vector2((float)chickenPos.x, (float)chickenPos.y));
					chicken.PointerEntered = false;
					chicken.GameObject.SetActive(true);
					this._chickenDataList[i] = chicken;
					this.UpdateChickenSkeleton(chicken);
					PointerTrigger pointerTrigger = chicken.GameObject.GetComponent<PointerTrigger>();
					pointerTrigger.EnterEvent.ResetListener(delegate()
					{
						chicken.PointerEntered = true;
					});
					pointerTrigger.ExitEvent.ResetListener(delegate()
					{
						GameObject uigameObject = chicken.UIGameObject;
						PointerTrigger uiTrigger = (uigameObject != null) ? uigameObject.GetComponent<PointerTrigger>() : null;
						chicken.PointerEntered = (uiTrigger != null && uiTrigger.AtEnter);
					});
					TooltipInvoker tooltipInvoker = chicken.GameObject.GetComponent<Refers>().CGet<TooltipInvoker>("Tip");
					tooltipInvoker.Type = TipType.Chicken;
					tooltipInvoker.triggerByChildRaycast = true;
					TooltipInvoker tooltipInvoker2 = tooltipInvoker;
					if (tooltipInvoker2.RuntimeParam == null)
					{
						tooltipInvoker2.RuntimeParam = EasyPool.Get<ArgumentBox>();
					}
					tooltipInvoker.RuntimeParam.Set("ChickenId", chicken.Id);
				}
				EasyPool.Free<List<Vector2Int>>(posKeyList);
				EasyPool.Free<List<Vector2Int>>(neighborList);
				for (int j = this._chickenDataList.Count - 1; j >= chickenCount; j--)
				{
					this.ChickenObjectPool.DestroyObject(this._chickenDataList[j].GameObject);
					this._chickenDataList.RemoveAt(j);
					this.moveInRoadDataList.Clear();
				}
			}
			this.ChickenInit = true;
			this.InitChickenUi();
		}

		// Token: 0x0600B470 RID: 46192 RVA: 0x0052206A File Offset: 0x0052026A
		private void InitChickenUi()
		{
			this.chickenUiHolder.Rebuild<RectTransform>(this._chickenDataList.Count, delegate(RectTransform rect, int index)
			{
				SettlementRoadAnimation.ChickenData chickenData = this._chickenDataList[index];
				chickenData.UIGameObject = rect.gameObject;
				PositionFollower positionFollower = rect.GetComponent<PositionFollower>();
				positionFollower.Target = chickenData.GameObject.transform;
				this.SetFulongChickenFeatherUi(chickenData);
			});
		}

		// Token: 0x0600B471 RID: 46193 RVA: 0x00522090 File Offset: 0x00520290
		private void UpdateChickenSkeleton(SettlementRoadAnimation.ChickenData chicken)
		{
			SkeletonAnimation skeletonGraphic = chicken.SkeletonComponent;
			this.UpdateChickenAnimation(skeletonGraphic, false);
			Vector2Int direction = SettlementRoadAnimation.GetMoveDirection(chicken.Cross1, chicken.Cross2);
			string skinName = this.GetChickenSkinName(direction);
			skeletonGraphic.Skeleton.SetSkin(skinName);
			skeletonGraphic.Skeleton.SetAttachment(this.GetChickenSlotName(skinName), this.GetChickenAttachmentName(chicken, skinName));
			chicken.GameObject.transform.localScale = this.GetChickenScale(direction);
		}

		// Token: 0x0600B472 RID: 46194 RVA: 0x00522108 File Offset: 0x00520308
		private void UpdateChickenAnimation(SettlementRoadAnimation.ChickenData chicken, bool stop)
		{
			this.UpdateChickenAnimation(chicken.SkeletonComponent, stop);
		}

		// Token: 0x0600B473 RID: 46195 RVA: 0x0052211C File Offset: 0x0052031C
		private void UpdateChickenAnimation(IAnimationStateComponent comp, bool stop)
		{
			string animationName = "walk";
			if (stop)
			{
				animationName = ((Random.Range(0, 2) == 0) ? "eat" : "jump");
			}
			comp.AnimationState.SetAnimation(0, animationName, true);
		}

		// Token: 0x0600B474 RID: 46196 RVA: 0x0052215C File Offset: 0x0052035C
		public static Vector2Int GetMoveDirection(Vector2Int start, Vector2Int end)
		{
			bool flag = end.x > start.x;
			Vector2Int result;
			if (flag)
			{
				result = Vector2Int.right;
			}
			else
			{
				bool flag2 = end.x < start.x;
				if (flag2)
				{
					result = Vector2Int.left;
				}
				else
				{
					bool flag3 = end.y > start.y;
					if (flag3)
					{
						result = Vector2Int.down;
					}
					else
					{
						result = Vector2Int.up;
					}
				}
			}
			return result;
		}

		// Token: 0x0600B475 RID: 46197 RVA: 0x005221C8 File Offset: 0x005203C8
		private string GetChickenSkinName(Vector2Int direction)
		{
			bool flag = direction == Vector2Int.up;
			string result;
			if (flag)
			{
				result = "back";
			}
			else
			{
				bool flag2 = direction == Vector2Int.down;
				if (flag2)
				{
					result = "front";
				}
				else
				{
					result = "side";
				}
			}
			return result;
		}

		// Token: 0x0600B476 RID: 46198 RVA: 0x00522210 File Offset: 0x00520410
		private string GetChickenSlotName(string skinName)
		{
			return skinName + "_body";
		}

		// Token: 0x0600B477 RID: 46199 RVA: 0x00522230 File Offset: 0x00520430
		private string GetChickenAttachmentName(SettlementRoadAnimation.ChickenData chickenData, string skinName)
		{
			bool hairLess = this.ChickenHairless(chickenData);
			ChickenItem config = Config.Chicken.Instance[chickenData.TemplateId];
			string attachmentName = string.Format("chicken_{0}_", config.ChickenColor.ToInt()) + skinName;
			bool flag = hairLess;
			if (flag)
			{
				attachmentName += "_hairless";
			}
			return attachmentName;
		}

		// Token: 0x0600B478 RID: 46200 RVA: 0x00522298 File Offset: 0x00520498
		private Vector3 GetChickenScale(Vector2Int direction)
		{
			bool flag = direction == Vector2Int.right;
			Vector3 result;
			if (flag)
			{
				result = new Vector3(-1f, 1f, 1f) * this.chickenScale;
			}
			else
			{
				result = Vector3.one * this.chickenScale;
			}
			return result;
		}

		// Token: 0x17001476 RID: 5238
		// (get) Token: 0x0600B479 RID: 46201 RVA: 0x005222EB File Offset: 0x005204EB
		private TaskModel TaskModel
		{
			get
			{
				return SingletonObject.getInstance<TaskModel>();
			}
		}

		// Token: 0x0600B47A RID: 46202 RVA: 0x005222F4 File Offset: 0x005204F4
		public void CalcChickenChange()
		{
			bool chickenInit = this.ChickenInit;
			if (chickenInit)
			{
				this._sectFulongLoseFeatherChickensChange = this.SectFulongLoseFeatherChickensNew.Except(this.SectFulongLoseFeatherChickensOld).ToList<int>();
			}
		}

		// Token: 0x0600B47B RID: 46203 RVA: 0x00522328 File Offset: 0x00520528
		private void UpdateAllChickens()
		{
			foreach (SettlementRoadAnimation.ChickenData chickenData in this._chickenDataList)
			{
				this.SetFulongChickenFeatherUi(chickenData);
				this.UpdateChickenSkeleton(chickenData);
			}
		}

		// Token: 0x0600B47C RID: 46204 RVA: 0x0052238C File Offset: 0x0052058C
		private void UpdateKingChange()
		{
			foreach (SettlementRoadAnimation.ChickenData chickenData in this._chickenDataList)
			{
				bool flag = chickenData.TemplateId == 63;
				if (flag)
				{
					this.SetFulongChickenFeatherUi(chickenData);
				}
			}
		}

		// Token: 0x0600B47D RID: 46205 RVA: 0x005223F4 File Offset: 0x005205F4
		private SettlementRoadAnimation.ChickenData GetChickenDataById(int chickenId)
		{
			for (int i = 0; i < this._chickenDataList.Count; i++)
			{
				SettlementRoadAnimation.ChickenData chicken = this._chickenDataList[i];
				bool flag = chicken.Id == chickenId;
				if (flag)
				{
					return chicken;
				}
			}
			return null;
		}

		// Token: 0x0600B47E RID: 46206 RVA: 0x00522444 File Offset: 0x00520644
		public void PlayChickenLoseFeatureEffect()
		{
			bool flag = !UIManager.Instance.IsFocusElement(UIElement.BuildingArea) || !this.TaskModel.IsTaskInProgress(303);
			if (!flag)
			{
				SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.3f, delegate
				{
					for (int i = 0; i < this._sectFulongLoseFeatherChickensChange.Count; i++)
					{
						int chickenId = this._sectFulongLoseFeatherChickensChange[i];
						SettlementRoadAnimation.ChickenData chickenData = this.GetChickenDataById(chickenId);
						string effectName = this.GetLoseFeatureEffectName(chickenData);
						this.PlayChickenFeatureEffect(chickenData, effectName);
						this.UpdateChickenSkeleton(chickenData);
						this.SetFulongChickenFeatherUi(chickenData);
					}
					this._sectFulongLoseFeatherChickensChange.Clear();
				});
			}
		}

		// Token: 0x0600B47F RID: 46207 RVA: 0x0052249C File Offset: 0x0052069C
		public void UpdateChickenFeather()
		{
			bool flag = !UIManager.Instance.IsFocusElement(UIElement.BuildingArea);
			if (!flag)
			{
				this.UpdateKingChange();
				this.UpdateAllChickens();
				bool flag2 = this._inTaskFulongUpgradeClothing && !this.TaskModel.IsTaskInProgress(406);
				if (flag2)
				{
					foreach (SettlementRoadAnimation.ChickenData chickenData in this._chickenDataList)
					{
						this.PlayChickenFeatureEffect(chickenData, "eff_chicken_zhangmao_tongyong");
					}
				}
				this._inTaskFulongUpgradeClothing = this.TaskModel.IsTaskInProgress(406);
			}
		}

		// Token: 0x0600B480 RID: 46208 RVA: 0x00522560 File Offset: 0x00520760
		private string GetLoseFeatureEffectName(SettlementRoadAnimation.ChickenData chickenData)
		{
			ChickenItem config = Config.Chicken.Instance[chickenData.TemplateId];
			return string.Format("eff_chicken_bamao_{0}", config.ChickenColor.ToInt());
		}

		// Token: 0x0600B481 RID: 46209 RVA: 0x005225A4 File Offset: 0x005207A4
		private void PlayChickenFeatureEffect(SettlementRoadAnimation.ChickenData chickenData, string effectName)
		{
			UIParticle particleHolder = chickenData.GameObject.GetComponentInChildren<UIParticle>();
			bool have = false;
			for (int i = 0; i < particleHolder.transform.childCount; i++)
			{
				Transform child = particleHolder.transform.GetChild(i);
				bool flag = child.name.Contains(effectName);
				if (flag)
				{
					have = true;
				}
			}
			bool flag2 = have;
			if (flag2)
			{
				SettlementRoadAnimation.<PlayChickenFeatureEffect>g__PlayEffect|66_0(particleHolder, effectName);
			}
			else
			{
				string pathName = "RemakeResources/Particle/UIEffectPrefabs/BuildingChickenHouse/" + effectName;
				ResLoader.Load<GameObject>(pathName, delegate(GameObject effect)
				{
					GameObject effectGo = Object.Instantiate<GameObject>(effect, chickenData.GameObject.transform);
					effectGo.transform.SetParent(particleHolder.transform);
					SettlementRoadAnimation.<PlayChickenFeatureEffect>g__PlayEffect|66_0(particleHolder, effectName);
				}, null, false);
			}
		}

		// Token: 0x0600B482 RID: 46210 RVA: 0x00522674 File Offset: 0x00520874
		private void SetFulongChickenFeatherUi(SettlementRoadAnimation.ChickenData chickenData)
		{
			GameObject chickenUi = chickenData.UIGameObject;
			CButton btn = chickenUi.GetComponent<CButton>();
			SkeletonAnimation pluck = chickenUi.GetComponentInChildren<SkeletonAnimation>();
			btn.gameObject.SetActive(this.ChickenPluckActive(chickenData));
			btn.onClick.RemoveAllListeners();
			bool activeSelf = pluck.gameObject.activeSelf;
			if (activeSelf)
			{
				pluck.AnimationState.SetAnimation(0, "idle_" + this.GetPluckAnimationState(), true);
				btn.ClearAndAddListener(delegate
				{
					this._buildingArea.OnChickenClick(chickenData.Id);
				});
				PointerTrigger pointerTrigger = chickenUi.GetComponent<PointerTrigger>();
				pointerTrigger.EnterEvent.ResetListener(delegate()
				{
					pluck.AnimationState.SetAnimation(0, "move_" + this.GetPluckAnimationState(), true);
					chickenData.PointerEntered = true;
				});
				pointerTrigger.ExitEvent.ResetListener(delegate()
				{
					pluck.AnimationState.SetAnimation(0, "idle_" + this.GetPluckAnimationState(), true);
					PointerTrigger chickenPointerTrigger = chickenData.GameObject.GetComponent<PointerTrigger>();
					chickenData.PointerEntered = chickenPointerTrigger.AtEnter;
				});
				TooltipInvoker tooltipInvoker = chickenUi.GetComponent<TooltipInvoker>();
				tooltipInvoker.Type = TipType.Chicken;
				tooltipInvoker.triggerByChildRaycast = true;
				TooltipInvoker tooltipInvoker2 = tooltipInvoker;
				if (tooltipInvoker2.RuntimeParam == null)
				{
					tooltipInvoker2.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				tooltipInvoker.RuntimeParam.Set("ChickenId", chickenData.Id);
			}
		}

		// Token: 0x0600B483 RID: 46211 RVA: 0x005227B8 File Offset: 0x005209B8
		private bool ChickenHairless(SettlementRoadAnimation.ChickenData chickenData)
		{
			return (this.TaskModel.IsTaskInProgress(303) && this.SectFulongLoseFeatherChickensOld.Contains(chickenData.Id)) || this.TaskModel.IsTaskInProgress(404) || this.TaskModel.IsTaskInProgress(405) || this.TaskModel.IsTaskInProgress(406);
		}

		// Token: 0x0600B484 RID: 46212 RVA: 0x00522828 File Offset: 0x00520A28
		private bool ChickenPluckActive(SettlementRoadAnimation.ChickenData chickenData)
		{
			bool condition = this.TaskModel.IsTaskInProgress(303) && !this.SectFulongLoseFeatherChickensOld.Contains(chickenData.Id);
			bool condition2 = chickenData.TemplateId == 63 && (this.TaskModel.IsTaskInProgress(404) || this.TaskModel.IsTaskInProgress(406));
			bool condition3 = this.CanPluckFeatherChickenIds.Contains(chickenData.Id);
			return condition || condition2 || condition3;
		}

		// Token: 0x0600B485 RID: 46213 RVA: 0x005228B4 File Offset: 0x00520AB4
		private string GetPluckAnimationState()
		{
			bool flag = this.TaskModel.IsTaskInProgress(406);
			string result;
			if (flag)
			{
				result = "2";
			}
			else
			{
				result = "1";
			}
			return result;
		}

		// Token: 0x0600B486 RID: 46214 RVA: 0x005228E8 File Offset: 0x00520AE8
		private void CheckChickenHold()
		{
			for (int i = 0; i < this._chickenDataList.Count; i++)
			{
				SettlementRoadAnimation.ChickenData chicken = this._chickenDataList[i];
				bool flag = Time.time - chicken.HoldMoveUntilTime > this._minChickenMoveDuration;
				if (flag)
				{
					bool flag2 = Time.time - chicken.CheckHoldTime > 3f;
					if (flag2)
					{
						bool flag3 = GameApp.RandomRange(0, 100) < (int)this._probabilityOfChickenHoldMove;
						if (flag3)
						{
							chicken.CheckHoldTime = Time.time;
							bool flag4 = this.CanChickenHold(chicken);
							if (flag4)
							{
								chicken.HoldMoveUntilTime = Time.time + GameApp.RandomRange(this._chickenHoldMoveRange.x, this._chickenHoldMoveRange.y);
								chicken.Holding = true;
								this.UpdateChickenAnimation(chicken, true);
								this._chickenDataList.ForEach(delegate(SettlementRoadAnimation.ChickenData e)
								{
									e.CheckHoldTime = Time.time;
								});
								break;
							}
						}
					}
				}
			}
		}

		// Token: 0x0600B487 RID: 46215 RVA: 0x005229F8 File Offset: 0x00520BF8
		private sbyte GetChickenMoveSpeed()
		{
			return (sbyte)Random.Range(this._chickenMoveSpeedRange.x, this._chickenMoveSpeedRange.y);
		}

		// Token: 0x0600B488 RID: 46216 RVA: 0x00522A28 File Offset: 0x00520C28
		private bool CanChickenHold(SettlementRoadAnimation.ChickenData target)
		{
			for (int i = 0; i < this._chickenDataList.Count; i++)
			{
				SettlementRoadAnimation.ChickenData chicken = this._chickenDataList[i];
				bool flag = chicken == target;
				if (!flag)
				{
					bool flag2 = SettlementRoadAnimation.<CanChickenHold>g__IsAtSameLine|73_0(target.Cross1, target.Cross2, chicken.Cross1, chicken.Cross2);
					if (flag2)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x0600B48C RID: 46220 RVA: 0x00522C0C File Offset: 0x00520E0C
		[CompilerGenerated]
		internal static void <PlayChickenFeatureEffect>g__PlayEffect|66_0(UIParticle particleHolder, string effectName)
		{
			particleHolder.RefreshParticles();
			for (int i = 0; i < particleHolder.transform.childCount; i++)
			{
				Transform child = particleHolder.transform.GetChild(i);
				bool flag = !child.name.Contains(effectName);
				if (!flag)
				{
					ParticleSystem particle = child.GetComponent<ParticleSystem>();
					bool flag2 = particle != null;
					if (flag2)
					{
						particle.Play();
					}
				}
			}
		}

		// Token: 0x0600B48D RID: 46221 RVA: 0x00522C7C File Offset: 0x00520E7C
		[CompilerGenerated]
		internal static bool <CanChickenHold>g__IsAtSameLine|73_0(Vector2Int posA, Vector2Int posB, Vector2Int posC, Vector2Int posD)
		{
			return (posA == posC && posB == posD) || (posA == posD && posB == posC);
		}

		// Token: 0x04008C93 RID: 35987
		[SerializeField]
		public RectTransform animationHolder;

		// Token: 0x04008C94 RID: 35988
		[SerializeField]
		public GameObject chickenTemplate;

		// Token: 0x04008C95 RID: 35989
		[SerializeField]
		public GameObject villagerTemplate;

		// Token: 0x04008C96 RID: 35990
		[SerializeField]
		public TemplatedContainerAssemblyNew chickenUiHolder;

		// Token: 0x04008C97 RID: 35991
		[SerializeField]
		private SkeletonDataAsset[] villagerSkeletonDataAssets;

		// Token: 0x04008C98 RID: 35992
		[SerializeField]
		public float chickenScale = 2.5f;

		// Token: 0x04008C99 RID: 35993
		[SerializeField]
		public float peopleScale = 1f;

		// Token: 0x04008C9A RID: 35994
		public PoolItem VillagerObjectPool;

		// Token: 0x04008C9B RID: 35995
		private bool villagerInit;

		// Token: 0x04008C9C RID: 35996
		private ViewBuildingArea _buildingArea;

		// Token: 0x04008C9D RID: 35997
		private Dictionary<Vector2Int, Vector2Int> _roadCrossPos;

		// Token: 0x04008C9E RID: 35998
		private readonly List<SettlementRoadAnimation.VillagerData> villagerDataList = new List<SettlementRoadAnimation.VillagerData>();

		// Token: 0x04008C9F RID: 35999
		private readonly List<SettlementRoadAnimation.MoveInRoadData> moveInRoadDataList = new List<SettlementRoadAnimation.MoveInRoadData>();

		// Token: 0x04008CA0 RID: 36000
		[NonSerialized]
		public bool HaveChickenKing;

		// Token: 0x04008CA1 RID: 36001
		private readonly sbyte _probabilityOfChickenHoldMove = 30;

		// Token: 0x04008CA2 RID: 36002
		private readonly float _minChickenMoveDuration = 6f;

		// Token: 0x04008CA3 RID: 36003
		private readonly Vector2 _chickenHoldMoveRange = new Vector2(3f, 7.5f);

		// Token: 0x04008CA4 RID: 36004
		private Vector2Int _chickenMoveSpeedRange = new Vector2Int(5, 10);

		// Token: 0x04008CA5 RID: 36005
		[NonSerialized]
		public bool ChickenInit;

		// Token: 0x04008CA6 RID: 36006
		private readonly List<SettlementRoadAnimation.ChickenData> _chickenDataList = new List<SettlementRoadAnimation.ChickenData>();

		// Token: 0x04008CA7 RID: 36007
		public readonly HashSet<int> CanPluckFeatherChickenIds = new HashSet<int>();

		// Token: 0x04008CA8 RID: 36008
		public List<GameData.Domains.Building.Chicken> ChickenList = new List<GameData.Domains.Building.Chicken>();

		// Token: 0x04008CA9 RID: 36009
		public PoolItem ChickenObjectPool;

		// Token: 0x04008CAA RID: 36010
		[NonSerialized]
		public List<int> SectFulongLoseFeatherChickensOld = new List<int>();

		// Token: 0x04008CAB RID: 36011
		[NonSerialized]
		public List<int> SectFulongLoseFeatherChickensNew = new List<int>();

		// Token: 0x04008CAC RID: 36012
		[NonSerialized]
		private List<int> _sectFulongLoseFeatherChickensChange = new List<int>();

		// Token: 0x04008CAD RID: 36013
		[NonSerialized]
		private bool _inTaskFulongUpgradeClothing;

		// Token: 0x0200259C RID: 9628
		public class MoveInRoadData
		{
			// Token: 0x06010C40 RID: 68672 RVA: 0x0066F7D8 File Offset: 0x0066D9D8
			public Vector2 GetPosition()
			{
				RectTransform villagerObjTrans = this.GameObject.GetComponent<RectTransform>();
				return villagerObjTrans.anchoredPosition;
			}

			// Token: 0x0400E876 RID: 59510
			public GameObject GameObject;

			// Token: 0x0400E877 RID: 59511
			public Vector2Int Cross1;

			// Token: 0x0400E878 RID: 59512
			public Vector2Int Cross2;

			// Token: 0x0400E879 RID: 59513
			public bool PointerEntered;

			// Token: 0x0400E87A RID: 59514
			public bool Holding;

			// Token: 0x0400E87B RID: 59515
			public Vector2Int Direction;

			// Token: 0x0400E87C RID: 59516
			public float LinerValue;

			// Token: 0x0400E87D RID: 59517
			public float LinerSpeed;

			// Token: 0x0400E87E RID: 59518
			public float HoldMoveUntilTime;

			// Token: 0x0400E87F RID: 59519
			public float CheckHoldTime;

			// Token: 0x0400E880 RID: 59520
			public float TargetLaneOffset = 0f;

			// Token: 0x0400E881 RID: 59521
			public float CurrentLaneOffset = 0f;

			// Token: 0x0400E882 RID: 59522
			public float OffsetSmoothSpeed = 10f;
		}

		// Token: 0x0200259D RID: 9629
		private abstract class SkeletionAnimMoveInRoadData : SettlementRoadAnimation.MoveInRoadData
		{
			// Token: 0x17001B95 RID: 7061
			// (get) Token: 0x06010C42 RID: 68674 RVA: 0x0066F828 File Offset: 0x0066DA28
			public SkeletonAnimation SkeletonComponent
			{
				get
				{
					bool flag = this._skeletonComp == null;
					if (flag)
					{
						this._skeletonComp = this.GameObject.GetComponentInChildren<SkeletonAnimation>();
					}
					return this._skeletonComp;
				}
			}

			// Token: 0x0400E883 RID: 59523
			private SkeletonAnimation _skeletonComp;
		}

		// Token: 0x0200259E RID: 9630
		private class ChickenData : SettlementRoadAnimation.SkeletionAnimMoveInRoadData
		{
			// Token: 0x0400E884 RID: 59524
			public GameObject UIGameObject;

			// Token: 0x0400E885 RID: 59525
			public int Id;

			// Token: 0x0400E886 RID: 59526
			public short TemplateId;
		}

		// Token: 0x0200259F RID: 9631
		private class VillagerData : SettlementRoadAnimation.SkeletionAnimMoveInRoadData
		{
			// Token: 0x0400E887 RID: 59527
			public sbyte Gender;

			// Token: 0x0400E888 RID: 59528
			public bool Adult;
		}
	}
}
