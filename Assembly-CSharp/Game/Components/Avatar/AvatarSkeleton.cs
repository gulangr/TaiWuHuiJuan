using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Config;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.AvatarSystem.AvatarRes;
using GameData.Utilities;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.Avatar
{
	// Token: 0x02000F83 RID: 3971
	[ExecuteAlways]
	public class AvatarSkeleton : MonoBehaviour
	{
		// Token: 0x17001498 RID: 5272
		// (get) Token: 0x0600B650 RID: 46672 RVA: 0x00530718 File Offset: 0x0052E918
		// (set) Token: 0x0600B651 RID: 46673 RVA: 0x00530720 File Offset: 0x0052E920
		public bool KeepAnimationOnRefresh { get; set; }

		// Token: 0x0600B652 RID: 46674 RVA: 0x0053072C File Offset: 0x0052E92C
		private void RefreshDisplay()
		{
			foreach (SkeletonGraphic skeletonGraphic in this.GetSkeletonGraphics())
			{
				SkeletonData skeletonData;
				bool flag;
				if (skeletonGraphic.skeletonDataAsset != null)
				{
					skeletonData = skeletonGraphic.SkeletonData;
					flag = (skeletonData != null);
				}
				else
				{
					flag = false;
				}
				bool flag2 = flag;
				if (flag2)
				{
					FieldInfo skeletonSlotField = skeletonGraphic.GetType().GetField("separatorSlotNames", BindingFlags.Instance | BindingFlags.NonPublic);
					skeletonGraphic.OnInstructionsPrepared -= this.OnInstructionsPrepared;
					skeletonGraphic.OnInstructionsPrepared += this.OnInstructionsPrepared;
					bool flag3 = skeletonSlotField != null;
					if (flag3)
					{
						List<string> slots = new List<string>();
						HashSet<AvatarSkeleton.PartGroupType> matchedGroups = new HashSet<AvatarSkeleton.PartGroupType>();
						foreach (SlotData slot in skeletonData.Slots)
						{
							foreach (AvatarSkeleton.PartGroupConfiguration group in this.partGroupConfigurations)
							{
								bool flag4 = this.SlotInGroup(group, slot, skeletonGraphic);
								if (flag4)
								{
									slots.Add(slot.Name);
									matchedGroups.Add(group.group);
									break;
								}
							}
						}
						skeletonSlotField.SetValue(skeletonGraphic, slots.ToArray());
						skeletonGraphic.allowMultipleCanvasRenderers = (skeletonGraphic.enableSeparatorSlots = (matchedGroups.Count > 0));
						skeletonGraphic.ReapplySeparatorSlotNames();
						AvatarSkeleton.SyncSubmeshGraphicsWithCanvasRenderers(skeletonGraphic);
						skeletonGraphic.OnBecameVisible();
						skeletonGraphic.LateUpdate();
						skeletonGraphic.MatchRectTransformWithBounds();
					}
				}
				foreach (Slot slot2 in skeletonGraphic.separatorSlots)
				{
					bool flag5 = slot2 == null;
					if (!flag5)
					{
						SlotData slotData = slot2.Data;
						bool flag6 = slotData == null;
						if (!flag6)
						{
							foreach (AvatarSkeleton.PartGroupConfiguration group2 in this.partGroupConfigurations)
							{
								bool flag7 = this.SlotInGroup(group2, slotData, skeletonGraphic);
								if (flag7)
								{
									slot2.SetColor(group2.color);
									break;
								}
							}
						}
					}
				}
				skeletonGraphic.SetAllDirty();
			}
			foreach (AvatarSkeleton.PartGroupConfiguration group3 in this.partGroupConfigurations)
			{
				foreach (Graphic graphic in group3.effectGraphic)
				{
					bool flag8 = graphic == null;
					if (flag8)
					{
						return;
					}
					SkeletonGraphic skeletonGraphic2 = graphic as SkeletonGraphic;
					bool flag9 = skeletonGraphic2 != null && skeletonGraphic2.enableSeparatorSlots;
					if (flag9)
					{
						skeletonGraphic2.color = Color.white;
					}
					else
					{
						graphic.color = group3.color;
					}
					Transform trans = graphic.transform;
					trans.localPosition = trans.localPosition.SetZ(0f);
				}
			}
			this.RefreshClothingCoverDisplay();
			this.ApplySkeletonShadow();
			this._needRefresh = 0;
		}

		// Token: 0x0600B653 RID: 46675 RVA: 0x00530AB0 File Offset: 0x0052ECB0
		private void RefreshClothingCoverDisplay()
		{
			bool flag = !this.clothingCover.gameObject.activeSelf;
			if (!flag)
			{
				bool flag2 = this.clothingCover.skeletonDataAsset == null || this.clothingCover.Skeleton == null;
				if (!flag2)
				{
					this.clothingCover.OnInstructionsPrepared -= this.OnInstructionsPrepared;
					this.clothingCover.OnInstructionsPrepared += this.OnInstructionsPrepared;
					AvatarSkeleton.SyncSubmeshGraphicsWithCanvasRenderers(this.clothingCover);
					foreach (Slot slot in this.clothingCover.Skeleton.Slots)
					{
						foreach (AvatarSkeleton.PartGroupConfiguration group in this.partGroupConfigurations)
						{
							bool flag3 = this.SlotInGroup(group, slot.Data, this.clothingCover);
							if (flag3)
							{
								slot.SetColor(group.color);
								break;
							}
						}
					}
					this.clothingCover.SetAllDirty();
				}
			}
		}

		// Token: 0x0600B654 RID: 46676 RVA: 0x00530BF0 File Offset: 0x0052EDF0
		private void TryLoadClothingCover(string clothingSkeletonDataName, bool keepAnimation = false)
		{
			string baseName = clothingSkeletonDataName;
			bool flag = baseName.EndsWith("_SkeletonData");
			if (flag)
			{
				string text = baseName;
				int length = "_SkeletonData".Length;
				baseName = text.Substring(0, text.Length - length);
			}
			bool flag2 = AvatarSetting.Instance != null && AvatarSetting.Instance.HasClothingCover(baseName);
			if (flag2)
			{
				string coverSkeletonDataName = baseName + "_cover_SkeletonData";
				string coverPath = Path.Combine(AvatarSkeleton.SkeletonPathClothing, coverSkeletonDataName);
				bool flag3 = AvatarSkeleton.TryGiveSkeletonGraphic(this.clothingCover, coverPath, keepAnimation);
				if (flag3)
				{
					this.clothingCover.gameObject.SetActive(true);
					this.SyncClothingCoverAnimation();
				}
				else
				{
					this.HideClothingCover();
				}
			}
			else
			{
				this.HideClothingCover();
			}
		}

		// Token: 0x0600B655 RID: 46677 RVA: 0x00530CAF File Offset: 0x0052EEAF
		private void HideClothingCover()
		{
			this.clothingCover.gameObject.SetActive(false);
		}

		// Token: 0x0600B656 RID: 46678 RVA: 0x00530CC4 File Offset: 0x0052EEC4
		private void SyncClothingCoverAnimation()
		{
			bool flag = !this.clothingCover.gameObject.activeSelf;
			if (!flag)
			{
				AvatarSkeleton.PartGroupConfiguration clothingGroup = this.partGroupConfigurations.FirstOrDefault((AvatarSkeleton.PartGroupConfiguration g) => g.group == AvatarSkeleton.PartGroupType.Clothing);
				bool flag2 = clothingGroup.mainGraphic.Length == 0;
				if (!flag2)
				{
					SkeletonGraphic clothingGraphic = clothingGroup.mainGraphic[0] as SkeletonGraphic;
					bool flag3 = clothingGraphic == null;
					if (!flag3)
					{
						Spine.AnimationState animationState = clothingGraphic.AnimationState;
						TrackEntry clothingTrack = (animationState != null) ? animationState.GetCurrent(0) : null;
						Spine.AnimationState animationState2 = this.clothingCover.AnimationState;
						TrackEntry coverTrack = (animationState2 != null) ? animationState2.GetCurrent(0) : null;
						bool flag4 = clothingTrack == null || coverTrack == null;
						if (!flag4)
						{
							coverTrack.TrackTime = clothingTrack.TrackTime;
						}
					}
				}
			}
		}

		// Token: 0x0600B657 RID: 46679 RVA: 0x00530D98 File Offset: 0x0052EF98
		private bool SlotInGroup(AvatarSkeleton.PartGroupConfiguration groupConfiguration, SlotData slotData, SkeletonGraphic sourceGraphic = null)
		{
			string slotPrefix = groupConfiguration.slotPrefix;
			bool flag = string.IsNullOrEmpty(slotPrefix);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = sourceGraphic != null;
				if (flag2)
				{
					bool flag3 = groupConfiguration.group == AvatarSkeleton.PartGroupType.HairFront || groupConfiguration.group == AvatarSkeleton.PartGroupType.HairBack;
					if (flag3)
					{
						bool flag4 = groupConfiguration.effectGraphic != null && groupConfiguration.effectGraphic.Length != 0;
						if (flag4)
						{
							bool flag5 = Array.IndexOf<Graphic>(groupConfiguration.effectGraphic, sourceGraphic) == -1;
							if (flag5)
							{
								return false;
							}
						}
					}
				}
				string attachmentName = slotData.AttachmentName;
				bool flag6;
				if (attachmentName == null || !attachmentName.StartsWith(slotPrefix))
				{
					string slotName = slotData.Name;
					flag6 = (slotName != null && slotName.StartsWith(slotPrefix));
				}
				else
				{
					flag6 = true;
				}
				result = flag6;
			}
			return result;
		}

		// Token: 0x0600B658 RID: 46680 RVA: 0x00530E53 File Offset: 0x0052F053
		private IEnumerable<SkeletonGraphic> GetSkeletonGraphics()
		{
			HashSet<SkeletonGraphic> seenGraphics = new HashSet<SkeletonGraphic>();
			foreach (AvatarSkeleton.PartGroupConfiguration group in this.partGroupConfigurations)
			{
				foreach (Graphic graphic in group.effectGraphic)
				{
					SkeletonGraphic skeletonGraphic = graphic as SkeletonGraphic;
					bool flag = skeletonGraphic != null && seenGraphics.Add(skeletonGraphic);
					if (flag)
					{
						yield return skeletonGraphic;
					}
					skeletonGraphic = null;
					graphic = null;
				}
				Graphic[] array2 = null;
				group = default(AvatarSkeleton.PartGroupConfiguration);
			}
			AvatarSkeleton.PartGroupConfiguration[] array = null;
			yield break;
		}

		// Token: 0x0600B659 RID: 46681 RVA: 0x00530E64 File Offset: 0x0052F064
		private string GetClothingSkeletonDataName(string prefix, string sex, string size)
		{
			bool flag = string.IsNullOrEmpty(prefix);
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				bool flag2 = prefix.Contains("{0}");
				if (flag2)
				{
					result = string.Format(prefix, sex, size) + "_SkeletonData";
				}
				else
				{
					result = string.Concat(new string[]
					{
						prefix,
						"_",
						sex,
						"_",
						size,
						"_SkeletonData"
					});
				}
			}
			return result;
		}

		// Token: 0x0600B65A RID: 46682 RVA: 0x00530EDC File Offset: 0x0052F0DC
		private static void SyncSubmeshGraphicsWithCanvasRenderers(SkeletonGraphic skeletonGraphic)
		{
			MethodInfo syncSubmeshGraphicsWithCanvasRenderersMethod = AvatarSkeleton.SyncSubmeshGraphicsWithCanvasRenderersMethod;
			if (syncSubmeshGraphicsWithCanvasRenderersMethod != null)
			{
				syncSubmeshGraphicsWithCanvasRenderersMethod.Invoke(skeletonGraphic, null);
			}
		}

		// Token: 0x0600B65B RID: 46683 RVA: 0x00530EF4 File Offset: 0x0052F0F4
		private void OnInstructionsPrepared(SkeletonRendererInstruction instruction)
		{
			SubmeshInstruction[] subs = instruction.submeshInstructions.Items;
			for (int i = 0; i < subs.Length; i++)
			{
				bool flag = !subs[i].material;
				if (flag)
				{
					subs[i].material = this.fixMaterial;
				}
			}
		}

		// Token: 0x0600B65C RID: 46684 RVA: 0x00530F4E File Offset: 0x0052F14E
		private void Awake()
		{
			this.OnEnable();
			base.transform.localScale = Vector3.zero;
		}

		// Token: 0x0600B65D RID: 46685 RVA: 0x00530F6C File Offset: 0x0052F16C
		private void LateUpdate()
		{
			bool flag = this._needRefresh > 0;
			if (flag)
			{
				this._needRefresh--;
				bool flag2 = this._needRefresh == 0;
				if (flag2)
				{
					this.RefreshDisplay();
				}
			}
			base.transform.localScale = this.CalcScaleFromBaseLine();
			this.SyncClothingCoverAnimation();
		}

		// Token: 0x0600B65E RID: 46686 RVA: 0x00530FC4 File Offset: 0x0052F1C4
		private void OnEnable()
		{
			this._needRefresh = 1;
		}

		// Token: 0x0600B65F RID: 46687 RVA: 0x00530FD0 File Offset: 0x0052F1D0
		private void OnDisable()
		{
			foreach (SkeletonGraphic skeleton in this.GetSkeletonGraphics())
			{
				skeleton.OnInstructionsPrepared -= this.OnInstructionsPrepared;
			}
			this.clothingCover.OnInstructionsPrepared -= this.OnInstructionsPrepared;
		}

		// Token: 0x0600B660 RID: 46688 RVA: 0x00531044 File Offset: 0x0052F244
		private Vector3 CalcScaleFromBaseLine()
		{
			Transform transform = base.transform;
			RectTransform parentRect;
			bool flag;
			if (transform is RectTransform)
			{
				parentRect = (transform.parent as RectTransform);
				flag = (parentRect != null);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			Vector3 result;
			if (flag2)
			{
				Vector2 parentSize = parentRect.rect.size;
				result = new Vector3(parentSize.x / this.scaleBaseLine.x, parentSize.y / this.scaleBaseLine.y, 1f);
			}
			else
			{
				result = Vector3.one;
			}
			return result;
		}

		// Token: 0x0600B661 RID: 46689 RVA: 0x005310C8 File Offset: 0x0052F2C8
		private static Vector2 GetElementPosition(float[] positionConfig, AvatarSize size)
		{
			bool flag = positionConfig == null || positionConfig.Length < 2;
			Vector2 result;
			if (flag)
			{
				result = Vector2.zero;
			}
			else
			{
				float x = AvatarManagerUtils.FloatScaleBySize(positionConfig[0], size);
				float y = AvatarManagerUtils.FloatScaleBySize(positionConfig[1], size);
				result = new Vector2(x, y);
			}
			return result;
		}

		// Token: 0x0600B662 RID: 46690 RVA: 0x00531110 File Offset: 0x0052F310
		private void SyncSkeletonHeadRectSize()
		{
			bool flag = this.skeletonHeadImage == null;
			if (!flag)
			{
				RectTransform imageRect = this.skeletonHeadImage.rectTransform;
				float width = imageRect.rect.width;
				float height = imageRect.rect.height;
				bool flag2 = this.skeletonHeadInnerPosition != null && this.skeletonHeadInnerPosition != imageRect;
				if (flag2)
				{
					this.skeletonHeadInnerPosition.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
					this.skeletonHeadInnerPosition.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
				}
				bool flag3 = this.skeletonHeadOuterPosition == null;
				if (!flag3)
				{
					this.skeletonHeadOuterPosition.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
					this.skeletonHeadOuterPosition.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
				}
			}
		}

		// Token: 0x0600B663 RID: 46691 RVA: 0x005311D0 File Offset: 0x0052F3D0
		private void ResetSkeletonHeadOffset()
		{
			this.SetSkeletonHeadOffsetY(0f);
		}

		// Token: 0x0600B664 RID: 46692 RVA: 0x005311DF File Offset: 0x0052F3DF
		private void ApplySkeletonHeadOffset(AvatarHeadItem headConfig)
		{
			this.SetSkeletonHeadOffsetY((float)headConfig.SkeletonHeadBodyOffset);
		}

		// Token: 0x0600B665 RID: 46693 RVA: 0x005311F0 File Offset: 0x0052F3F0
		private void SetSkeletonHeadOffsetY(float yOffset)
		{
			bool flag = this.skeletonHeadInnerPosition == null;
			if (!flag)
			{
				Vector2 anchoredPosition = this.skeletonHeadInnerPosition.anchoredPosition;
				anchoredPosition.y = yOffset;
				this.skeletonHeadInnerPosition.anchoredPosition = anchoredPosition;
			}
		}

		// Token: 0x0600B666 RID: 46694 RVA: 0x00531234 File Offset: 0x0052F434
		private static void LegacyMigrate(CImage legacy, CImage self, Vector3 reversedBaseLineScale)
		{
			AvatarSkeleton.<>c__DisplayClass70_0 CS$<>8__locals1;
			CS$<>8__locals1.self = self;
			CS$<>8__locals1.legacy = legacy;
			CS$<>8__locals1.reversedBaseLineScale = reversedBaseLineScale;
			CS$<>8__locals1.rectLegacy = CS$<>8__locals1.legacy.rectTransform;
			CS$<>8__locals1.rectSelf = CS$<>8__locals1.self.rectTransform;
			AvatarSkeleton.<LegacyMigrate>g__Migration|70_0(ref CS$<>8__locals1);
		}

		// Token: 0x0600B667 RID: 46695 RVA: 0x00531288 File Offset: 0x0052F488
		internal void LegacyUpdateEyes(CImage left, CImage right, CImage leftBall, CImage rightBall)
		{
			Vector3 reversedBaseLineScale = Vector3.one;
			AvatarSkeleton.PartGroupConfiguration groupEyeBase = this.partGroupConfigurations.First((AvatarSkeleton.PartGroupConfiguration g) => g.group == AvatarSkeleton.PartGroupType.EyeBase);
			AvatarSkeleton.PartGroupConfiguration groupEyeball = this.partGroupConfigurations.First((AvatarSkeleton.PartGroupConfiguration g) => g.group == AvatarSkeleton.PartGroupType.Eyeball);
			AvatarSkeleton.LegacyMigrate(left, groupEyeBase.mainGraphic[0] as CImage, reversedBaseLineScale);
			AvatarSkeleton.LegacyMigrate(right, groupEyeBase.mainGraphic[1] as CImage, reversedBaseLineScale);
			AvatarSkeleton.PartGroupConfiguration groupDynamicEyes = this.partGroupConfigurations.FirstOrDefault((AvatarSkeleton.PartGroupConfiguration g) => g.group == AvatarSkeleton.PartGroupType.DynamicEyes);
			bool flag = groupDynamicEyes.group == AvatarSkeleton.PartGroupType.DynamicEyes && groupDynamicEyes.mainGraphic != null;
			if (flag)
			{
				AvatarSkeleton.UpdateDynamicEye(left, 0, groupDynamicEyes, reversedBaseLineScale);
				AvatarSkeleton.UpdateDynamicEye(right, 1, groupDynamicEyes, reversedBaseLineScale);
			}
			AvatarSkeleton.LegacyMigrate(leftBall, groupEyeball.mainGraphic[0] as CImage, reversedBaseLineScale);
			AvatarSkeleton.LegacyMigrate(rightBall, groupEyeball.mainGraphic[1] as CImage, reversedBaseLineScale);
			bool flag2 = this.eyesCenterPosition != null && left != null && right != null;
			if (flag2)
			{
				this.eyesCenterPosition.anchoredPosition = (left.rectTransform.anchoredPosition + right.rectTransform.anchoredPosition) * 0.5f;
			}
		}

		// Token: 0x0600B668 RID: 46696 RVA: 0x00531400 File Offset: 0x0052F600
		private static void UpdateDynamicEye(CImage target, int dynamicIndex, AvatarSkeleton.PartGroupConfiguration groupEyeBase, Vector3 reversedBaseLineScale)
		{
			SkeletonGraphic skeletonGraphic;
			bool flag;
			if (groupEyeBase.mainGraphic.Length > dynamicIndex)
			{
				skeletonGraphic = (groupEyeBase.mainGraphic[dynamicIndex] as SkeletonGraphic);
				flag = (skeletonGraphic != null);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				bool flag3 = target != null;
				if (flag3)
				{
					RectTransform rect = skeletonGraphic.rectTransform;
					float sizeScale = 1f;
					bool flag4 = target.sprite != null && target.sprite.rect.width > 0f;
					if (flag4)
					{
						sizeScale = target.rectTransform.rect.width / target.sprite.rect.width;
					}
					rect.localScale = new Vector3(sizeScale * reversedBaseLineScale.x, sizeScale * reversedBaseLineScale.y, 1f);
				}
			}
		}

		// Token: 0x0600B669 RID: 46697 RVA: 0x005314D8 File Offset: 0x0052F6D8
		internal void LegacyUpdateEyebrows(CImage left, CImage right)
		{
			Vector3 reversedBaseLineScale = Vector3.one;
			AvatarSkeleton.PartGroupConfiguration group = this.partGroupConfigurations.First((AvatarSkeleton.PartGroupConfiguration g) => g.group == AvatarSkeleton.PartGroupType.Eyebrow);
			AvatarSkeleton.LegacyMigrate(left, group.mainGraphic[0] as CImage, reversedBaseLineScale);
			AvatarSkeleton.LegacyMigrate(right, group.mainGraphic[1] as CImage, reversedBaseLineScale);
		}

		// Token: 0x0600B66A RID: 46698 RVA: 0x00531544 File Offset: 0x0052F744
		internal void LegacyUpdateNose(CImage obj)
		{
			Vector3 reversedBaseLineScale = Vector3.one;
			reversedBaseLineScale = new Vector3(1f / reversedBaseLineScale.x, 1f / reversedBaseLineScale.y, 1f);
			AvatarSkeleton.PartGroupConfiguration group = this.partGroupConfigurations.First((AvatarSkeleton.PartGroupConfiguration g) => g.group == AvatarSkeleton.PartGroupType.Nose);
			AvatarSkeleton.LegacyMigrate(obj, group.mainGraphic[0] as CImage, reversedBaseLineScale);
		}

		// Token: 0x0600B66B RID: 46699 RVA: 0x005315BC File Offset: 0x0052F7BC
		internal void LegacyUpdateMouth(CImage obj, CImage obj2)
		{
			Vector3 reversedBaseLineScale = Vector3.one;
			reversedBaseLineScale = new Vector3(1f / reversedBaseLineScale.x, 1f / reversedBaseLineScale.y, 1f);
			AvatarSkeleton.PartGroupConfiguration group = this.partGroupConfigurations.First((AvatarSkeleton.PartGroupConfiguration g) => g.group == AvatarSkeleton.PartGroupType.Mouth);
			AvatarSkeleton.LegacyMigrate(obj, group.mainGraphic[0] as CImage, reversedBaseLineScale);
			AvatarSkeleton.LegacyMigrate(obj2, group.mainGraphic[1] as CImage, reversedBaseLineScale);
		}

		// Token: 0x0600B66C RID: 46700 RVA: 0x0053164C File Offset: 0x0052F84C
		internal void LegacyUpdateWrinkle(CImage obj, CImage obj2, CImage obj3A, CImage obj3B)
		{
			Vector3 reversedBaseLineScale = Vector3.one;
			reversedBaseLineScale = new Vector3(1f / reversedBaseLineScale.x, 1f / reversedBaseLineScale.y, 1f);
			AvatarSkeleton.PartGroupConfiguration group = this.partGroupConfigurations.First((AvatarSkeleton.PartGroupConfiguration g) => g.group == AvatarSkeleton.PartGroupType.Wrinkle);
			AvatarSkeleton.LegacyMigrate(obj, group.mainGraphic[0] as CImage, reversedBaseLineScale);
			AvatarSkeleton.LegacyMigrate(obj2, group.mainGraphic[1] as CImage, reversedBaseLineScale);
			AvatarSkeleton.LegacyMigrate(obj3A, group.mainGraphic[2] as CImage, reversedBaseLineScale);
			AvatarSkeleton.LegacyMigrate(obj3B, group.mainGraphic[3] as CImage, reversedBaseLineScale);
		}

		// Token: 0x0600B66D RID: 46701 RVA: 0x00531704 File Offset: 0x0052F904
		internal void LegacyPassBodyAnchoredPosition(Vector2 anchoredPosition)
		{
			AvatarSkeleton.PartGroupConfiguration group = this.partGroupConfigurations.First((AvatarSkeleton.PartGroupConfiguration g) => g.group == AvatarSkeleton.PartGroupType.Clothing);
			SkeletonGraphic skeletonGraphic = group.mainGraphic[0] as SkeletonGraphic;
			bool flag = skeletonGraphic != null;
			if (flag)
			{
				skeletonGraphic.rectTransform.anchoredPosition = anchoredPosition;
			}
		}

		// Token: 0x0600B66E RID: 46702 RVA: 0x00531764 File Offset: 0x0052F964
		internal void LegacyUpdateBackHairPosition(float[] positionConfig, AvatarSize size)
		{
			bool flag = this.backHairPosition == null;
			if (!flag)
			{
				this.backHairPosition.anchoredPosition = AvatarSkeleton.GetElementPosition(positionConfig, AvatarSize.Big);
			}
		}

		// Token: 0x0600B66F RID: 46703 RVA: 0x00531798 File Offset: 0x0052F998
		internal void LegacyUpdateFrontHairPosition(float[] positionConfig, AvatarSize size)
		{
			bool flag = this.frontHairPosition == null;
			if (!flag)
			{
				this.frontHairPosition.anchoredPosition = AvatarSkeleton.GetElementPosition(positionConfig, AvatarSize.Big);
			}
		}

		// Token: 0x0600B670 RID: 46704 RVA: 0x005317CC File Offset: 0x0052F9CC
		internal void LegacyUpdateFeature1(CImage centerOrLeft, CImage right, CImage leftEye, CImage rightEye)
		{
			Vector3 reversedBaseLineScale = Vector3.one;
			AvatarSkeleton.PartGroupConfiguration group = this.partGroupConfigurations.FirstOrDefault((AvatarSkeleton.PartGroupConfiguration g) => g.group == AvatarSkeleton.PartGroupType.Feature1);
			bool flag = group.group != AvatarSkeleton.PartGroupType.Feature1 || group.mainGraphic == null || group.mainGraphic.Length < 4;
			if (!flag)
			{
				AvatarSkeleton.LegacyMigrate(centerOrLeft, group.mainGraphic[0] as CImage, reversedBaseLineScale);
				AvatarSkeleton.LegacyMigrate(right, group.mainGraphic[1] as CImage, reversedBaseLineScale);
				AvatarSkeleton.LegacyMigrate(leftEye, group.mainGraphic[2] as CImage, reversedBaseLineScale);
				AvatarSkeleton.LegacyMigrate(rightEye, group.mainGraphic[3] as CImage, reversedBaseLineScale);
			}
		}

		// Token: 0x0600B671 RID: 46705 RVA: 0x00531888 File Offset: 0x0052FA88
		internal void LegacyUpdateFeature2(CImage centerOrLeft, CImage right, CImage leftEye, CImage rightEye)
		{
			Vector3 reversedBaseLineScale = Vector3.one;
			AvatarSkeleton.PartGroupConfiguration group = this.partGroupConfigurations.FirstOrDefault((AvatarSkeleton.PartGroupConfiguration g) => g.group == AvatarSkeleton.PartGroupType.Feature2);
			bool flag = group.group != AvatarSkeleton.PartGroupType.Feature2 || group.mainGraphic == null || group.mainGraphic.Length < 4;
			if (!flag)
			{
				AvatarSkeleton.LegacyMigrate(centerOrLeft, group.mainGraphic[0] as CImage, reversedBaseLineScale);
				AvatarSkeleton.LegacyMigrate(right, group.mainGraphic[1] as CImage, reversedBaseLineScale);
				AvatarSkeleton.LegacyMigrate(leftEye, group.mainGraphic[2] as CImage, reversedBaseLineScale);
				AvatarSkeleton.LegacyMigrate(rightEye, group.mainGraphic[3] as CImage, reversedBaseLineScale);
			}
		}

		// Token: 0x0600B672 RID: 46706 RVA: 0x00531944 File Offset: 0x0052FB44
		private static ulong ComputeDataKey(AvatarData data, byte avatarId)
		{
			ulong key = (ulong)avatarId;
			key = (key * 397UL ^ (ulong)((ushort)data.ClothDisplayId));
			key = (key * 397UL ^ (ulong)((ushort)data.FrontHairId));
			key = (key * 397UL ^ (ulong)((ushort)data.BackHairId));
			key = (key * 397UL ^ (ulong)((ushort)data.Beard1Id));
			key = (key * 397UL ^ (ulong)((ushort)data.Beard2Id));
			key = (key * 397UL ^ (ulong)((ushort)data.EyesMainId));
			key = (key * 397UL ^ (ulong)data.HeadId);
			return key * 397UL ^ (ulong)data.ColorSkinId;
		}

		// Token: 0x0600B673 RID: 46707 RVA: 0x005319EC File Offset: 0x0052FBEC
		public bool Refresh(AvatarData data, byte? runtimeAvatarId = null)
		{
			byte avatarId = runtimeAvatarId ?? data.AvatarId;
			bool effectiveKeepAnimation = this.KeepAnimationOnRefresh;
			bool flag = effectiveKeepAnimation;
			if (flag)
			{
				ulong currentKey = AvatarSkeleton.ComputeDataKey(data, avatarId);
				bool flag2 = currentKey != this._lastDataKey;
				if (flag2)
				{
					effectiveKeepAnimation = false;
				}
				this._lastDataKey = currentKey;
			}
			this.ResetSkeletonHeadOffset();
			this.HideClothingCover();
			foreach (SkeletonGraphic skeletonGraphic in this.GetSkeletonGraphics())
			{
				bool allowMultipleCanvasRenderers = skeletonGraphic.allowMultipleCanvasRenderers;
				if (allowMultipleCanvasRenderers)
				{
					skeletonGraphic.enableSeparatorSlots = false;
					skeletonGraphic.allowMultipleCanvasRenderers = false;
					skeletonGraphic.ReapplySeparatorSlotNames();
				}
			}
			bool flag3 = this.clothingCover != null && this.clothingCover.allowMultipleCanvasRenderers;
			if (flag3)
			{
				this.clothingCover.enableSeparatorSlots = false;
				this.clothingCover.allowMultipleCanvasRenderers = false;
				this.clothingCover.ReapplySeparatorSlotNames();
			}
			for (int i = 0; i < this.partGroupConfigurations.Length; i++)
			{
				this.partGroupConfigurations[i].Hidden = false;
			}
			int j = 0;
			Func<AvatarElementsItem, bool> <>9__1;
			while (j < this.partGroupConfigurations.Length)
			{
				bool failed = false;
				AvatarSkeleton.PartGroupConfiguration group = this.partGroupConfigurations[j];
				switch (group.group)
				{
				case AvatarSkeleton.PartGroupType.Skin:
				{
					failed = true;
					Color skinColor = AvatarSkinColors.Instance[data.ColorSkinId].ColorHex.HexStringToColor();
					int headId = (int)((avatarId >= 251 && avatarId <= 254) ? 1 : data.HeadId);
					AvatarHeadItem headConfig = AvatarHead.Instance.FirstOrDefault((AvatarHeadItem item) => item.AvatarId == avatarId && (int)item.HeadId == headId);
					bool flag4 = headConfig != null;
					if (flag4)
					{
						bool flag5 = this.skeletonHeadImage != null;
						if (flag5)
						{
							Sprite sprite = AvatarAtlasAssets.Instance.GetSprite(avatarId, headConfig.NameOrPath, 0);
							bool flag6 = sprite != null;
							if (flag6)
							{
								this.skeletonHeadImage.sprite = sprite;
								this.skeletonHeadImage.SetNativeSize();
								this.SyncSkeletonHeadRectSize();
								failed = false;
							}
						}
						this.ApplySkeletonHeadOffset(headConfig);
					}
					else
					{
						Debug.LogWarning(string.Format("[AvatarSkeleton] AvatarHead config not found for AvatarId: {0}, HeadId: {1}", avatarId, data.HeadId));
					}
					bool flag7 = data.HeadId != byte.MaxValue;
					if (flag7)
					{
						group.color = skinColor;
					}
					else
					{
						group.color = Color.white;
					}
					break;
				}
				case AvatarSkeleton.PartGroupType.Clothing:
				{
					failed = true;
					SkeletonGraphic skeletonGraphic2 = group.mainGraphic[0] as SkeletonGraphic;
					bool flag8 = skeletonGraphic2 != null;
					if (flag8)
					{
						string skeletonName = null;
						bool flag9 = data.ClothDisplayId != 0;
						if (flag9)
						{
							IEnumerable<AvatarElementsItem> instance = AvatarElements.Instance;
							Func<AvatarElementsItem, bool> predicate;
							if ((predicate = <>9__1) == null)
							{
								predicate = (<>9__1 = ((AvatarElementsItem item) => item.AvatarId == avatarId && item.Type == EAvatarElementsType.Cloth && item.ElementId == data.ClothDisplayId));
							}
							AvatarElementsItem clothElement = instance.FirstOrDefault(predicate);
							bool flag10 = clothElement != null && !string.IsNullOrEmpty(clothElement.ClothSkeletonName);
							if (flag10)
							{
								skeletonName = clothElement.ClothSkeletonName + "_SkeletonData";
							}
						}
						bool flag11 = string.IsNullOrEmpty(skeletonName) && data.ClothDisplayId == 0;
						if (flag11)
						{
							string identifierSex = (data.AvatarId % 2 == 0) ? "female" : "male";
							int num = (int)((data.AvatarId + ((data.AvatarId % 2 == 0) ? 0 : 1)) / 2);
							if (!true)
							{
							}
							string text;
							if (num != 1)
							{
								if (num != 2)
								{
									text = "big";
								}
								else
								{
									text = "middle";
								}
							}
							else
							{
								text = "small";
							}
							if (!true)
							{
							}
							string identifierSize = text;
							skeletonName = this.GetClothingSkeletonDataName("qita_yibubiti", identifierSex, identifierSize);
						}
						bool flag12 = !string.IsNullOrEmpty(skeletonName);
						if (flag12)
						{
							bool ok = AvatarSkeleton.TryGiveSkeletonGraphic(skeletonGraphic2, Path.Combine(AvatarSkeleton.SkeletonPathClothing, skeletonName), effectiveKeepAnimation);
							failed = !ok;
							bool flag13 = ok;
							if (flag13)
							{
								skeletonGraphic2.gameObject.SetActive(true);
								foreach (BoneFollowerGraphic bf in base.GetComponentsInChildren<BoneFollowerGraphic>())
								{
									bf.SetBone(bf.boneName);
								}
								this.TryLoadClothingCover(skeletonName, effectiveKeepAnimation);
							}
							else
							{
								this.HideClothingCover();
							}
						}
						else
						{
							this.HideClothingCover();
						}
					}
					group.color = AvatarClothColors.Instance[data.ColorClothId].ColorHex.HexStringToColor();
					break;
				}
				case AvatarSkeleton.PartGroupType.EyeBase:
					break;
				case AvatarSkeleton.PartGroupType.Eyeball:
					group.color = AvatarEyeballColors.Instance[data.ColorEyeballId].ColorHex.HexStringToColor();
					break;
				case AvatarSkeleton.PartGroupType.Eyebrow:
					group.color = AvatarHairColors.Instance[data.ColorEyebrowId].ColorHex.HexStringToColor();
					break;
				case AvatarSkeleton.PartGroupType.Mouth:
					group.color = AvatarLipColors.Instance[data.ColorMouthId].ColorHex.HexStringToColor();
					break;
				case AvatarSkeleton.PartGroupType.HairFront:
					group.color = (this.HairShown[0].ShouldWhite ? Color.white : AvatarHairColors.Instance[data.ColorFrontHairId].ColorHex.HexStringToColor());
					break;
				case AvatarSkeleton.PartGroupType.HairBack:
					group.color = (this.HairShown[1].ShouldWhite ? Color.white : AvatarHairColors.Instance[data.ColorBackHairId].ColorHex.HexStringToColor());
					group.Hidden = this.HairShown[1].Bare;
					break;
				case AvatarSkeleton.PartGroupType.Beard1:
				case AvatarSkeleton.PartGroupType.Beard2:
				{
					failed = true;
					bool flag14 = (avatarId >= 251 && avatarId <= 254) || data.HeadId == byte.MaxValue;
					if (flag14)
					{
						SkeletonGraphic childBeardGraphic = group.mainGraphic[0] as SkeletonGraphic;
						bool flag15 = childBeardGraphic != null;
						if (flag15)
						{
							childBeardGraphic.gameObject.SetActive(false);
							group.Hidden = true;
						}
						failed = false;
					}
					else
					{
						int idx = (group.group == AvatarSkeleton.PartGroupType.Beard1) ? 0 : 1;
						bool shouldWhite = false;
						SkeletonGraphic skeletonGraphic3 = group.mainGraphic[0] as SkeletonGraphic;
						bool flag16 = skeletonGraphic3 != null && this.BeardShown.CheckIndex(idx);
						if (flag16)
						{
							AvatarSkeleton.BeardShownConfiguration info = this.BeardShown[idx];
							bool enable = info.Enable;
							if (enable)
							{
								skeletonGraphic3.gameObject.SetActive(true);
								failed = !AvatarSkeleton.TryGiveSkeletonGraphic(skeletonGraphic3, Path.Combine(Path.Combine("RemakeResources/SpineAnimations/Avatar", string.Format("beard{0}", idx + 1)), string.Format("avatar_{0}_beard{1}_{2}_SkeletonData", avatarId, idx + 1, (group.group == AvatarSkeleton.PartGroupType.Beard1) ? data.Beard1Id : data.Beard2Id)), effectiveKeepAnimation);
								shouldWhite = info.ShouldWhite;
								RectTransform rect = skeletonGraphic3.rectTransform;
								rect.anchoredPosition = info.AnchoredPosition;
								rect.localScale = info.Scale * Vector3.one;
								group.Hidden = false;
							}
							else
							{
								skeletonGraphic3.gameObject.SetActive(false);
								failed = false;
								group.Hidden = true;
							}
						}
						group.color = (shouldWhite ? Color.white : AvatarHairColors.Instance[(group.group == AvatarSkeleton.PartGroupType.Beard1) ? data.ColorBeard1Id : data.ColorBeard2Id].ColorHex.HexStringToColor());
					}
					break;
				}
				case AvatarSkeleton.PartGroupType.Feature1:
					group.color = AvatarFeatureColors.Instance[data.ColorFeature1Id].ColorHex.HexStringToColor();
					break;
				case AvatarSkeleton.PartGroupType.Feature2:
					group.color = AvatarFeatureColors.Instance[data.ColorFeature2Id].ColorHex.HexStringToColor();
					break;
				case AvatarSkeleton.PartGroupType.Nose:
				case AvatarSkeleton.PartGroupType.Wrinkle:
					goto IL_A01;
				case AvatarSkeleton.PartGroupType.DynamicEyes:
				{
					bool flag17 = data.HeadId == byte.MaxValue;
					if (flag17)
					{
						SkeletonGraphic leftEye;
						bool flag18;
						if (group.mainGraphic.Length != 0)
						{
							leftEye = (group.mainGraphic[0] as SkeletonGraphic);
							flag18 = (leftEye != null);
						}
						else
						{
							flag18 = false;
						}
						bool flag19 = flag18;
						if (flag19)
						{
							leftEye.gameObject.SetActive(false);
						}
						SkeletonGraphic rightEye;
						bool flag20;
						if (group.mainGraphic.Length > 1)
						{
							rightEye = (group.mainGraphic[1] as SkeletonGraphic);
							flag20 = (rightEye != null);
						}
						else
						{
							flag20 = false;
						}
						bool flag21 = flag20;
						if (flag21)
						{
							rightEye.gameObject.SetActive(false);
						}
						group.Hidden = true;
					}
					else
					{
						group.color = AvatarSkinColors.Instance[data.ColorSkinId].ColorHex.HexStringToColor();
						AvatarAsset leftEyeAsset = AvatarSkeleton.GetDynamicEyeAsset((int)avatarId, data.EyesMainId, 0);
						AvatarAsset rightEyeAsset = AvatarSkeleton.GetDynamicEyeAsset((int)avatarId, data.EyesMainId, 0);
						this.UpdateSkeletonGraphicEye(group, 0, (int)avatarId, leftEyeAsset, effectiveKeepAnimation);
						this.UpdateSkeletonGraphicEye(group, 1, (int)avatarId, rightEyeAsset, effectiveKeepAnimation);
						AvatarSkeleton.SyncDynamicEyesAnimation(group);
					}
					break;
				}
				case AvatarSkeleton.PartGroupType.ExtraPart:
					break;
				default:
					goto IL_A01;
				}
				IL_A11:
				AvatarSkeleton.PartGroupType group2 = group.group;
				bool flag22 = group2 == AvatarSkeleton.PartGroupType.HairBack || group2 == AvatarSkeleton.PartGroupType.HairFront;
				if (flag22)
				{
					failed = true;
					SkeletonGraphic skeletonGraphic4 = group.mainGraphic[0] as SkeletonGraphic;
					bool flag23 = skeletonGraphic4 != null;
					if (flag23)
					{
						int hairIdx = (group.group == AvatarSkeleton.PartGroupType.HairFront) ? 1 : 2;
						short hairId = (group.group == AvatarSkeleton.PartGroupType.HairFront) ? data.FrontHairId : data.BackHairId;
						bool skipLoad = false;
						bool flag24 = avatarId >= 251 && avatarId <= 254;
						if (flag24)
						{
							AvatarGroup childGroup = AvatarManager.Instance.GetAvatarGroup((int)avatarId);
							bool flag25 = childGroup != null;
							if (flag25)
							{
								List<HairRes> resList = (group.group == AvatarSkeleton.PartGroupType.HairFront) ? childGroup.Hair1Res : childGroup.Hair2Res;
								bool flag26 = resList != null && resList.Count > 0;
								if (flag26)
								{
									hairId = resList[(int)hairId % resList.Count].Id;
								}
								else
								{
									skipLoad = true;
									failed = false;
									skeletonGraphic4.gameObject.SetActive(false);
									group.Hidden = true;
								}
							}
							else
							{
								skipLoad = true;
								failed = false;
								skeletonGraphic4.gameObject.SetActive(false);
								group.Hidden = true;
							}
						}
						bool flag27 = !skipLoad;
						if (flag27)
						{
							bool flag28 = group.group == AvatarSkeleton.PartGroupType.HairFront && this.HairShown[0].Bare;
							if (flag28)
							{
								hairId = 1;
							}
							bool flag29 = group.group == AvatarSkeleton.PartGroupType.HairBack && this.HairShown[1].Bare;
							if (flag29)
							{
								hairId = 1;
							}
							skeletonGraphic4.gameObject.SetActive(true);
							string hairPath = Path.Combine(AvatarSkeleton.SkeletonPathHair, string.Format("avatar_{0}_hair{1}_{2}_SkeletonData", avatarId, hairIdx, hairId));
							failed = !AvatarSkeleton.TryGiveSkeletonGraphic(skeletonGraphic4, hairPath, effectiveKeepAnimation);
						}
					}
				}
				this.partGroupConfigurations[j] = group;
				bool flag30 = failed;
				if (flag30)
				{
					this._needRefresh = 1;
					return false;
				}
				j++;
				continue;
				IL_A01:
				group.color = group.color;
				goto IL_A11;
			}
			this._needRefresh = 1;
			return true;
		}

		// Token: 0x0600B674 RID: 46708 RVA: 0x0053266C File Offset: 0x0053086C
		public void SetNeedRefresh()
		{
			bool flag = this._needRefresh > 0;
			if (!flag)
			{
				this._needRefresh = 1;
			}
		}

		// Token: 0x0600B675 RID: 46709 RVA: 0x00532690 File Offset: 0x00530890
		public void SetShadowStrength(float strengthValue)
		{
			this._shadowStrength = strengthValue;
			this.ApplySkeletonShadow();
		}

		// Token: 0x0600B676 RID: 46710 RVA: 0x005326A4 File Offset: 0x005308A4
		private void ApplySkeletonShadow()
		{
			foreach (SkeletonGraphic sg in base.GetComponentsInChildren<SkeletonGraphic>(false))
			{
				bool flag = sg != null;
				if (flag)
				{
					sg.color = Color.Lerp(sg.color, Color.black, this._shadowStrength);
				}
			}
			foreach (CImage ci in base.GetComponentsInChildren<CImage>(false))
			{
				bool flag2 = ci != null && ci.enabled;
				if (flag2)
				{
					ci.color = Color.Lerp(ci.color, Color.black, this._shadowStrength);
				}
			}
		}

		// Token: 0x0600B677 RID: 46711 RVA: 0x0053275C File Offset: 0x0053095C
		public static bool TryGiveSkeletonGraphic(SkeletonGraphic target, string path, bool keepAnimation = false)
		{
			bool flag = AvatarSetting.Instance == null || !AvatarSetting.Instance.HasSkeletonData(path);
			bool result;
			if (flag)
			{
				AvatarSkeleton.ClearSkeletonGraphic(target);
				result = false;
			}
			else
			{
				SkeletonDataAsset asset = ResLoader.SyncLoad<SkeletonDataAsset>(path);
				bool flag2 = asset;
				if (flag2)
				{
					AvatarSkeleton.SetupSkeletonGraphic(target, asset, keepAnimation);
					result = true;
				}
				else
				{
					AvatarSkeleton.ClearSkeletonGraphic(target);
					AdaptableLog.TagWarning("SyncLoad", path, false);
					result = false;
				}
			}
			return result;
		}

		// Token: 0x0600B678 RID: 46712 RVA: 0x005327D0 File Offset: 0x005309D0
		private static void ClearSkeletonGraphic(SkeletonGraphic target)
		{
			bool flag = target.skeletonDataAsset != null;
			if (flag)
			{
				target.Clear();
				target.skeletonDataAsset = null;
			}
		}

		// Token: 0x0600B679 RID: 46713 RVA: 0x00532800 File Offset: 0x00530A00
		public static void SetupSkeletonGraphic(SkeletonGraphic target, SkeletonDataAsset skeletonDataAsset, bool keepAnimation = false)
		{
			if (skeletonDataAsset.blendModeMaterials == null)
			{
				skeletonDataAsset.blendModeMaterials = new BlendModeMaterials();
			}
			bool isSameAsset = target.skeletonDataAsset == skeletonDataAsset && target.IsValid;
			bool flag = !isSameAsset;
			if (flag)
			{
				target.skeletonDataAsset = skeletonDataAsset;
				target.Initialize(true);
			}
			AvatarSkeleton.SyncSubmeshGraphicsWithCanvasRenderers(target);
			target.SetMaterialDirty();
			bool flag2 = !keepAnimation || !isSameAsset;
			if (flag2)
			{
				Spine.Animation[] animations = skeletonDataAsset.GetSkeletonData(true).Animations.Items;
				bool flag3 = animations.Length != 0 && target.AnimationState != null;
				if (flag3)
				{
					target.AnimationState.SetAnimation(0, animations[0], true);
				}
			}
		}

		// Token: 0x0600B67A RID: 46714 RVA: 0x005328B0 File Offset: 0x00530AB0
		private void UpdateSkeletonGraphicEye(AvatarSkeleton.PartGroupConfiguration group, int graphicIndex, int avatarId, AvatarAsset eyeAsset, bool keepAnimation = false)
		{
			SkeletonGraphic skeletonGraphic;
			bool flag;
			if (group.mainGraphic.Length > graphicIndex)
			{
				skeletonGraphic = (group.mainGraphic[graphicIndex] as SkeletonGraphic);
				flag = (skeletonGraphic != null);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				int eyeResourceId;
				bool flag3 = !AvatarSkeleton.TryGetDynamicEyeResourceId(eyeAsset, out eyeResourceId);
				if (flag3)
				{
					skeletonGraphic.gameObject.SetActive(false);
				}
				else
				{
					string spinePath = Path.Combine("RemakeResources/SpineAnimations/Avatar", "eyes", string.Format("avatar_{0}_eye_{1}_SkeletonData", avatarId, eyeResourceId));
					bool success = AvatarSkeleton.TryGiveSkeletonGraphic(skeletonGraphic, spinePath, keepAnimation);
					skeletonGraphic.gameObject.SetActive(success);
					bool flag4 = success;
					if (flag4)
					{
						skeletonGraphic.color = group.color;
					}
				}
			}
		}

		// Token: 0x0600B67B RID: 46715 RVA: 0x0053295C File Offset: 0x00530B5C
		private static void SyncDynamicEyesAnimation(AvatarSkeleton.PartGroupConfiguration group)
		{
			bool flag = group.mainGraphic.Length < 2;
			if (!flag)
			{
				SkeletonGraphic leftEye = group.mainGraphic[0] as SkeletonGraphic;
				bool flag2 = leftEye == null || !leftEye.gameObject.activeSelf;
				if (!flag2)
				{
					SkeletonGraphic rightEye = group.mainGraphic[1] as SkeletonGraphic;
					bool flag3 = rightEye == null || !rightEye.gameObject.activeSelf;
					if (!flag3)
					{
						Spine.AnimationState animationState = leftEye.AnimationState;
						TrackEntry leftTrack = (animationState != null) ? animationState.GetCurrent(0) : null;
						bool flag4 = leftTrack == null;
						if (!flag4)
						{
							Spine.AnimationState animationState2 = rightEye.AnimationState;
							TrackEntry rightTrack = (animationState2 != null) ? animationState2.GetCurrent(0) : null;
							bool flag5 = rightTrack == null;
							if (!flag5)
							{
								rightTrack.TrackTime = leftTrack.TrackTime;
							}
						}
					}
				}
			}
		}

		// Token: 0x0600B67C RID: 46716 RVA: 0x00532A24 File Offset: 0x00530C24
		private static AvatarAsset GetDynamicEyeAsset(int avatarId, short eyeId, short eyeSubId)
		{
			AvatarGroup group = AvatarManager.Instance.GetAvatarGroup(avatarId);
			return (group != null) ? group.Get(EAvatarElementsType.Eye, new short[]
			{
				eyeId,
				eyeSubId
			}) : null;
		}

		// Token: 0x0600B67D RID: 46717 RVA: 0x00532A60 File Offset: 0x00530C60
		private static bool CanUseDynamicEye(AvatarAsset eyeAsset)
		{
			return ((eyeAsset != null) ? eyeAsset.Config : null) != null && eyeAsset.Config.ParentId == 0;
		}

		// Token: 0x0600B67E RID: 46718 RVA: 0x00532A94 File Offset: 0x00530C94
		private static bool TryGetDynamicEyeResourceId(AvatarAsset eyeAsset, out int eyeResourceId)
		{
			eyeResourceId = 0;
			bool flag = !AvatarSkeleton.CanUseDynamicEye(eyeAsset) || string.IsNullOrEmpty(eyeAsset.Name);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				int markerIndex = eyeAsset.Name.IndexOf("_eye_", StringComparison.Ordinal);
				bool flag2 = markerIndex < 0;
				if (flag2)
				{
					result = false;
				}
				else
				{
					int numberStartIndex = markerIndex + "_eye_".Length;
					int numberEndIndex = eyeAsset.Name.IndexOf('_', numberStartIndex);
					string text;
					if (numberEndIndex < 0)
					{
						string name = eyeAsset.Name;
						int num = numberStartIndex;
						text = name.Substring(num, name.Length - num);
					}
					else
					{
						text = eyeAsset.Name.Substring(numberStartIndex, numberEndIndex - numberStartIndex);
					}
					string numberText = text;
					result = int.TryParse(numberText, out eyeResourceId);
				}
			}
			return result;
		}

		// Token: 0x0600B67F RID: 46719 RVA: 0x00532B44 File Offset: 0x00530D44
		private Transform GetPartByName(string avatarPartName)
		{
			bool flag = string.IsNullOrEmpty(avatarPartName);
			Transform result;
			if (flag)
			{
				result = null;
			}
			else
			{
				foreach (AvatarSkeleton.PartNameMapping mapping in this.partNameMappings)
				{
					bool flag2 = mapping.avatarPartName == avatarPartName;
					if (flag2)
					{
						return mapping.skeletonPart;
					}
				}
				Debug.LogWarning("[AvatarSkeleton] 未找到节点映射: " + avatarPartName);
				result = null;
			}
			return result;
		}

		// Token: 0x0600B680 RID: 46720 RVA: 0x00532BB4 File Offset: 0x00530DB4
		internal void LegacyAddExtraPart(EAvatarExtraPartsType extraPartType, short templateId, byte avatarId)
		{
			bool flag = this.extraPartTemplate == null;
			if (!flag)
			{
				if (this._extraPartMap == null)
				{
					this._extraPartMap = new Dictionary<EAvatarExtraPartsType, PositionFollower>();
				}
				AvatarExtraPartsItem extraPartItem = AvatarExtraParts.Instance.GetItem(templateId);
				bool flag2 = extraPartItem == null;
				if (flag2)
				{
					this.LegacyRemoveExtraPart(extraPartType);
				}
				else
				{
					PositionFollower follower;
					bool flag3 = !this._extraPartMap.TryGetValue(extraPartType, out follower);
					if (flag3)
					{
						GameObject followerObject = Object.Instantiate<GameObject>(this.extraPartTemplate.gameObject, this.extraPartTemplate.parent, false);
						follower = followerObject.GetComponent<PositionFollower>();
						this._extraPartMap.Add(extraPartType, follower);
					}
					Transform layerFollow = this.GetPartByName(extraPartItem.LayerFollow);
					bool flag4 = layerFollow != null;
					if (flag4)
					{
						bool wasInTargetParent = follower.transform.parent == layerFollow.parent;
						int followerIdx = follower.transform.GetSiblingIndex();
						int layerIdx = layerFollow.GetSiblingIndex();
						follower.transform.SetParent(layerFollow.parent, false);
						int targetIdx = layerIdx + (int)extraPartItem.LayerOffset;
						bool flag5 = wasInTargetParent && followerIdx < layerIdx;
						if (flag5)
						{
							targetIdx--;
						}
						follower.transform.SetSiblingIndex(targetIdx);
					}
					CImage followerImage = follower.GetComponent<CImage>();
					followerImage.sprite = AvatarAtlasAssets.Instance.GetSprite(avatarId, extraPartItem.Name, 0);
					bool flag6 = followerImage.sprite == null;
					if (flag6)
					{
						Debug.LogWarning(string.Format("[AvatarSkeleton] ExtraPart sprite not found: avatarId={0}, name={1}", avatarId, extraPartItem.Name));
						follower.gameObject.SetActive(false);
					}
					else
					{
						followerImage.SetNativeSize();
						followerImage.enabled = true;
						follower.transform.localScale = Vector3.one;
						bool flag7 = !string.IsNullOrEmpty(extraPartItem.PositionFollow);
						if (flag7)
						{
							Transform positionTarget = this.GetPartByName(extraPartItem.PositionFollow);
							bool flag8 = positionTarget != null;
							if (flag8)
							{
								follower.Target = AvatarSkeleton.EnsureCenterFollowTransform(positionTarget);
								float x = AvatarManagerUtils.FloatScaleBySize(extraPartItem.PositionOffset[0], AvatarSize.Big);
								float y = AvatarManagerUtils.FloatScaleBySize(extraPartItem.PositionOffset[1], AvatarSize.Big);
								follower.Offset = new Vector3(x, y, 0f);
								follower.Excute();
							}
						}
						bool flag9 = !string.IsNullOrEmpty(extraPartItem.ColorFollow);
						if (flag9)
						{
							Transform colorFollowTrans = this.GetPartByName(extraPartItem.ColorFollow);
							bool flag10 = colorFollowTrans != null;
							if (flag10)
							{
								CImage colorFollowImage = colorFollowTrans.GetComponent<CImage>();
								bool flag11 = colorFollowImage != null;
								if (flag11)
								{
									followerImage.color = colorFollowImage.color;
								}
							}
						}
						bool flag12 = !string.IsNullOrEmpty(extraPartItem.ScaleFollow);
						if (flag12)
						{
							Transform scaleFollowTrans = this.GetPartByName(extraPartItem.ScaleFollow);
							bool flag13 = scaleFollowTrans != null;
							if (flag13)
							{
								follower.transform.localScale = scaleFollowTrans.localScale;
							}
						}
						follower.enabled = true;
						follower.gameObject.SetActive(true);
					}
				}
			}
		}

		// Token: 0x0600B681 RID: 46721 RVA: 0x00532E9C File Offset: 0x0053109C
		internal void LegacyRemoveExtraPart(EAvatarExtraPartsType extraPartType)
		{
			PositionFollower follower;
			bool flag = this._extraPartMap == null || !this._extraPartMap.TryGetValue(extraPartType, out follower);
			if (!flag)
			{
				follower.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600B682 RID: 46722 RVA: 0x00532EDC File Offset: 0x005310DC
		internal void LegacyResetAllExtraParts()
		{
			bool flag = this._extraPartMap == null;
			if (!flag)
			{
				foreach (KeyValuePair<EAvatarExtraPartsType, PositionFollower> keyValuePair in this._extraPartMap)
				{
					EAvatarExtraPartsType eavatarExtraPartsType;
					PositionFollower positionFollower;
					keyValuePair.Deconstruct(out eavatarExtraPartsType, out positionFollower);
					PositionFollower follower = positionFollower;
					CImage image = follower.GetComponent<CImage>();
					image.enabled = false;
					image.sprite = null;
					image.color = Color.white;
					follower.enabled = false;
					follower.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x0600B683 RID: 46723 RVA: 0x00532F88 File Offset: 0x00531188
		private static Transform EnsureCenterFollowTransform(Transform parentTrans)
		{
			bool flag = !parentTrans;
			Transform result;
			if (flag)
			{
				result = parentTrans;
			}
			else
			{
				Transform centerTrans = parentTrans.Find("Center");
				bool flag2 = centerTrans;
				if (flag2)
				{
					result = centerTrans;
				}
				else
				{
					centerTrans = new GameObject("Center").transform;
					centerTrans.SetParent(parentTrans, false);
					RectTransform centerRectTrans = centerTrans.GetComponent<RectTransform>();
					bool flag3 = null == centerRectTrans;
					if (flag3)
					{
						centerRectTrans = centerTrans.gameObject.AddComponent<RectTransform>();
					}
					centerRectTrans.anchorMin = (centerRectTrans.anchorMax = Vector2.one * 0.5f);
					centerRectTrans.pivot = Vector2.one * 0.5f;
					centerRectTrans.anchoredPosition = Vector2.zero;
					result = centerRectTrans.transform;
				}
			}
			return result;
		}

		// Token: 0x0600B684 RID: 46724 RVA: 0x00533050 File Offset: 0x00531250
		internal void TrySetXiangshuInfection(AvatarData data)
		{
			bool flag = data == null || data.XiangshuInfectionStyle < 0;
			if (flag)
			{
				bool flag2 = this.xiangshuInfectionSkeleton != null;
				if (flag2)
				{
					this.xiangshuInfectionSkeleton.gameObject.SetActive(false);
				}
			}
			else
			{
				sbyte bodyType = data.GetBodyType();
				sbyte infectionType = data.XiangshuInfectionStyle;
				if (!true)
				{
				}
				string text2;
				if (infectionType != 0)
				{
					if (!true)
					{
					}
					string text;
					if (bodyType != 0)
					{
						if (bodyType != 1)
						{
							text = this.xiangshuCompletelyInfectionFatSkeletonPath;
						}
						else
						{
							text = this.xiangshuCompletelyInfectionNormalSkeletonPath;
						}
					}
					else
					{
						text = this.xiangshuCompletelyInfectionThinSkeletonPath;
					}
					if (!true)
					{
					}
					text2 = text;
				}
				else
				{
					if (!true)
					{
					}
					string text;
					if (bodyType != 0)
					{
						if (bodyType != 1)
						{
							text = this.xiangshuPartlyInfectionFatSkeletonPath;
						}
						else
						{
							text = this.xiangshuPartlyInfectionNormalSkeletonPath;
						}
					}
					else
					{
						text = this.xiangshuPartlyInfectionThinSkeletonPath;
					}
					if (!true)
					{
					}
					text2 = text;
				}
				if (!true)
				{
				}
				string selectedPath = text2;
				bool flag3 = string.IsNullOrEmpty(selectedPath);
				if (flag3)
				{
					this.xiangshuInfectionSkeleton.gameObject.SetActive(false);
				}
				else
				{
					bool isSamePath = this._currentXiangshuInfectionPath == selectedPath;
					bool flag4 = isSamePath && this.xiangshuInfectionSkeleton.gameObject.activeSelf;
					if (!flag4)
					{
						SkeletonDataAsset asset = ResLoader.SyncLoad<SkeletonDataAsset>(selectedPath);
						bool flag5 = asset;
						if (flag5)
						{
							bool flag6 = !isSamePath;
							if (flag6)
							{
								this.xiangshuInfectionSkeleton.skeletonDataAsset = asset;
								this.xiangshuInfectionSkeleton.Initialize(true);
								this.xiangshuInfectionSkeleton.AnimationState.SetAnimation(0, "animation", true);
							}
							this.xiangshuInfectionSkeleton.gameObject.SetActive(true);
							this._currentXiangshuInfectionPath = selectedPath;
						}
						else
						{
							this.xiangshuInfectionSkeleton.gameObject.SetActive(false);
						}
					}
				}
			}
		}

		// Token: 0x0600B685 RID: 46725 RVA: 0x00533200 File Offset: 0x00531400
		internal void TrySetHuanxinFace(AvatarData data)
		{
			bool flag = data == null || data.HuanxinFaceStyle < 0;
			if (flag)
			{
				bool flag2 = this.huanxinFaceSkeleton != null;
				if (flag2)
				{
					this.huanxinFaceSkeleton.gameObject.SetActive(false);
				}
			}
			else
			{
				sbyte huanxinFaceStyle = data.HuanxinFaceStyle;
				if (!true)
				{
				}
				string text;
				switch (huanxinFaceStyle)
				{
				case 0:
					text = this.huanxinBlueOpenEyesSkeletonPath;
					break;
				case 1:
					text = this.huanxinBlueCloseEyesSkeletonPath;
					break;
				case 2:
					text = this.huanxinWhiteOpenEyesSkeletonPath;
					break;
				case 3:
					text = this.huanxinWhiteCloseEyesSkeletonPath;
					break;
				case 4:
					text = this.huanxinRedOpenEyesSkeletonPath;
					break;
				default:
					text = this.huanxinRedCloseEyesSkeletonPath;
					break;
				}
				if (!true)
				{
				}
				string selectedPath = text;
				bool flag3 = !string.IsNullOrEmpty(selectedPath);
				if (flag3)
				{
					SkeletonDataAsset asset = ResLoader.SyncLoad<SkeletonDataAsset>(selectedPath);
					bool flag4 = asset;
					if (flag4)
					{
						this.huanxinFaceSkeleton.skeletonDataAsset = asset;
						this.huanxinFaceSkeleton.Initialize(true);
						this.huanxinFaceSkeleton.gameObject.SetActive(true);
						this.huanxinFaceSkeleton.AnimationState.SetAnimation(0, "animation", true);
						return;
					}
				}
				this.huanxinFaceSkeleton.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600B686 RID: 46726 RVA: 0x00533330 File Offset: 0x00531530
		internal void SetHuanxinFaceOffset(Vector2 offset)
		{
			bool flag = this.huanxinFaceSkeleton != null;
			if (flag)
			{
				this.huanxinFaceSkeleton.rectTransform.anchoredPosition = offset;
			}
		}

		// Token: 0x0600B687 RID: 46727 RVA: 0x00533360 File Offset: 0x00531560
		public void RefreshAsSkeleton(AvatarData data, byte avatarId)
		{
			this.Refresh(data, new byte?(avatarId));
			this.HideFaceParts();
		}

		// Token: 0x0600B688 RID: 46728 RVA: 0x00533378 File Offset: 0x00531578
		private void HideFaceParts()
		{
			for (int i = 0; i < this.partGroupConfigurations.Length; i++)
			{
				AvatarSkeleton.PartGroupConfiguration group = this.partGroupConfigurations[i];
				AvatarSkeleton.PartGroupType group2 = group.group;
				AvatarSkeleton.PartGroupType partGroupType = group2;
				if (partGroupType - AvatarSkeleton.PartGroupType.EyeBase <= 3 || partGroupType - AvatarSkeleton.PartGroupType.Beard1 <= 6)
				{
					this.partGroupConfigurations[i].Hidden = true;
					bool flag = group.mainGraphic != null;
					if (flag)
					{
						foreach (Graphic graphic in group.mainGraphic)
						{
							CImage image = graphic as CImage;
							bool flag2 = image != null;
							if (flag2)
							{
								image.enabled = false;
								image.sprite = null;
							}
							else
							{
								SkeletonGraphic skeletonGraphic = graphic as SkeletonGraphic;
								bool flag3 = skeletonGraphic != null;
								if (flag3)
								{
									skeletonGraphic.gameObject.SetActive(false);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600B68B RID: 46731 RVA: 0x005334E4 File Offset: 0x005316E4
		[CompilerGenerated]
		internal static void <LegacyMigrate>g__Migration|70_0(ref AvatarSkeleton.<>c__DisplayClass70_0 A_0)
		{
			A_0.self.color = A_0.legacy.color;
			A_0.self.sprite = A_0.legacy.sprite;
			A_0.self.enabled = (A_0.legacy.enabled && A_0.legacy.gameObject.activeSelf);
			A_0.rectSelf.pivot = A_0.rectLegacy.pivot;
			A_0.rectSelf.anchoredPosition = new Vector2(A_0.rectLegacy.anchoredPosition.x * A_0.reversedBaseLineScale.x, A_0.rectLegacy.anchoredPosition.y * A_0.reversedBaseLineScale.y);
			A_0.rectSelf.rotation = A_0.rectLegacy.rotation;
			A_0.rectSelf.sizeDelta = A_0.rectLegacy.sizeDelta;
			A_0.rectSelf.localScale = new Vector3(A_0.rectLegacy.localScale.x * A_0.reversedBaseLineScale.x, A_0.rectLegacy.localScale.y * A_0.reversedBaseLineScale.y, 1f);
		}

		// Token: 0x04008D76 RID: 36214
		[SerializeField]
		private Vector2 scaleBaseLine;

		// Token: 0x04008D77 RID: 36215
		[SerializeField]
		private RectTransform eyesCenterPosition;

		// Token: 0x04008D78 RID: 36216
		[SerializeField]
		private RectTransform skeletonHeadOuterPosition;

		// Token: 0x04008D79 RID: 36217
		[SerializeField]
		private RectTransform skeletonHeadInnerPosition;

		// Token: 0x04008D7A RID: 36218
		[SerializeField]
		private CImage skeletonHeadImage;

		// Token: 0x04008D7B RID: 36219
		[SerializeField]
		private RectTransform frontHairPosition;

		// Token: 0x04008D7C RID: 36220
		[SerializeField]
		private RectTransform backHairPosition;

		// Token: 0x04008D7D RID: 36221
		[SerializeField]
		private SkeletonGraphic clothingCover;

		// Token: 0x04008D7E RID: 36222
		private const string AvatarClothEmptyPrefix = "qita_yibubiti";

		// Token: 0x04008D7F RID: 36223
		private const string SkeletonPathBase = "RemakeResources/SpineAnimations/Avatar";

		// Token: 0x04008D80 RID: 36224
		private static readonly string SkeletonPathHair = Path.Combine("RemakeResources/SpineAnimations/Avatar", "hair");

		// Token: 0x04008D81 RID: 36225
		private static readonly string SkeletonPathClothing = Path.Combine("RemakeResources/SpineAnimations/Avatar", "clothing");

		// Token: 0x04008D82 RID: 36226
		private static readonly MethodInfo SyncSubmeshGraphicsWithCanvasRenderersMethod = typeof(SkeletonGraphic).GetMethod("SyncSubmeshGraphicsWithCanvasRenderers", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04008D83 RID: 36227
		private const int ChildAvatarIdMin = 251;

		// Token: 0x04008D84 RID: 36228
		private const int ChildAvatarIdMax = 254;

		// Token: 0x04008D85 RID: 36229
		private const int ChildSkinIndexOffset = 245;

		// Token: 0x04008D86 RID: 36230
		[SerializeField]
		private AvatarSkeleton.PartGroupConfiguration[] partGroupConfigurations = Array.Empty<AvatarSkeleton.PartGroupConfiguration>();

		// Token: 0x04008D87 RID: 36231
		[SerializeField]
		private UnityEngine.Material fixMaterial;

		// Token: 0x04008D88 RID: 36232
		[Header("ExtraPart Settings")]
		[SerializeField]
		private RectTransform extraPartTemplate;

		// Token: 0x04008D89 RID: 36233
		[SerializeField]
		private AvatarSkeleton.PartNameMapping[] partNameMappings = Array.Empty<AvatarSkeleton.PartNameMapping>();

		// Token: 0x04008D8A RID: 36234
		[Header("XiangshuInfection")]
		[SerializeField]
		private SkeletonGraphic xiangshuInfectionSkeleton;

		// Token: 0x04008D8B RID: 36235
		[SerializeField]
		private string xiangshuPartlyInfectionThinSkeletonPath;

		// Token: 0x04008D8C RID: 36236
		[SerializeField]
		private string xiangshuPartlyInfectionNormalSkeletonPath;

		// Token: 0x04008D8D RID: 36237
		[SerializeField]
		private string xiangshuPartlyInfectionFatSkeletonPath;

		// Token: 0x04008D8E RID: 36238
		[SerializeField]
		private string xiangshuCompletelyInfectionThinSkeletonPath;

		// Token: 0x04008D8F RID: 36239
		[SerializeField]
		private string xiangshuCompletelyInfectionNormalSkeletonPath;

		// Token: 0x04008D90 RID: 36240
		[SerializeField]
		private string xiangshuCompletelyInfectionFatSkeletonPath;

		// Token: 0x04008D91 RID: 36241
		[Header("HuanxinFace")]
		[SerializeField]
		private SkeletonGraphic huanxinFaceSkeleton;

		// Token: 0x04008D92 RID: 36242
		[SerializeField]
		private string huanxinBlueOpenEyesSkeletonPath;

		// Token: 0x04008D93 RID: 36243
		[SerializeField]
		private string huanxinBlueCloseEyesSkeletonPath;

		// Token: 0x04008D94 RID: 36244
		[SerializeField]
		private string huanxinWhiteOpenEyesSkeletonPath;

		// Token: 0x04008D95 RID: 36245
		[SerializeField]
		private string huanxinWhiteCloseEyesSkeletonPath;

		// Token: 0x04008D96 RID: 36246
		[SerializeField]
		private string huanxinRedOpenEyesSkeletonPath;

		// Token: 0x04008D97 RID: 36247
		[SerializeField]
		private string huanxinRedCloseEyesSkeletonPath;

		// Token: 0x04008D99 RID: 36249
		private ulong _lastDataKey;

		// Token: 0x04008D9A RID: 36250
		private string _currentXiangshuInfectionPath;

		// Token: 0x04008D9B RID: 36251
		private int _needRefresh;

		// Token: 0x04008D9C RID: 36252
		private float _shadowStrength;

		// Token: 0x04008D9D RID: 36253
		internal AvatarSkeleton.BeardShownConfiguration[] BeardShown;

		// Token: 0x04008D9E RID: 36254
		internal AvatarSkeleton.HairShownConfiguration[] HairShown = new AvatarSkeleton.HairShownConfiguration[2];

		// Token: 0x04008D9F RID: 36255
		private Dictionary<EAvatarExtraPartsType, PositionFollower> _extraPartMap;

		// Token: 0x020025C1 RID: 9665
		internal enum PartGroupType
		{
			// Token: 0x0400E8E9 RID: 59625
			Skin,
			// Token: 0x0400E8EA RID: 59626
			Clothing,
			// Token: 0x0400E8EB RID: 59627
			EyeBase,
			// Token: 0x0400E8EC RID: 59628
			Eyeball,
			// Token: 0x0400E8ED RID: 59629
			Eyebrow,
			// Token: 0x0400E8EE RID: 59630
			Mouth,
			// Token: 0x0400E8EF RID: 59631
			HairFront,
			// Token: 0x0400E8F0 RID: 59632
			HairBack,
			// Token: 0x0400E8F1 RID: 59633
			Beard1,
			// Token: 0x0400E8F2 RID: 59634
			Beard2,
			// Token: 0x0400E8F3 RID: 59635
			Feature1,
			// Token: 0x0400E8F4 RID: 59636
			Feature2,
			// Token: 0x0400E8F5 RID: 59637
			Nose,
			// Token: 0x0400E8F6 RID: 59638
			Wrinkle,
			// Token: 0x0400E8F7 RID: 59639
			DynamicEyes,
			// Token: 0x0400E8F8 RID: 59640
			ExtraPart
		}

		// Token: 0x020025C2 RID: 9666
		[Serializable]
		internal struct PartGroupConfiguration
		{
			// Token: 0x0400E8F9 RID: 59641
			[SerializeField]
			internal AvatarSkeleton.PartGroupType group;

			// Token: 0x0400E8FA RID: 59642
			[SerializeField]
			internal string slotPrefix;

			// Token: 0x0400E8FB RID: 59643
			[SerializeField]
			internal Color color;

			// Token: 0x0400E8FC RID: 59644
			[SerializeField]
			internal Graphic[] effectGraphic;

			// Token: 0x0400E8FD RID: 59645
			[SerializeField]
			internal Graphic[] mainGraphic;

			// Token: 0x0400E8FE RID: 59646
			internal bool Hidden;
		}

		// Token: 0x020025C3 RID: 9667
		internal struct BeardShownConfiguration
		{
			// Token: 0x0400E8FF RID: 59647
			internal bool Enable;

			// Token: 0x0400E900 RID: 59648
			internal bool ShouldWhite;

			// Token: 0x0400E901 RID: 59649
			internal Vector2 AnchoredPosition;

			// Token: 0x0400E902 RID: 59650
			internal float Scale;
		}

		// Token: 0x020025C4 RID: 9668
		internal struct HairShownConfiguration
		{
			// Token: 0x0400E903 RID: 59651
			internal bool Bare;

			// Token: 0x0400E904 RID: 59652
			internal bool ShouldWhite;
		}

		// Token: 0x020025C5 RID: 9669
		[Serializable]
		internal struct PartNameMapping
		{
			// Token: 0x0400E905 RID: 59653
			[SerializeField]
			internal string avatarPartName;

			// Token: 0x0400E906 RID: 59654
			[SerializeField]
			internal Transform skeletonPart;
		}
	}
}
