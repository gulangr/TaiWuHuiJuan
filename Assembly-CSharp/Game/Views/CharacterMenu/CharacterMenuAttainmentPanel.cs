using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Coffee.UIExtensions;
using DG.Tweening;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B5F RID: 2911
	public class CharacterMenuAttainmentPanel : MonoBehaviour
	{
		// Token: 0x17000FB1 RID: 4017
		// (get) Token: 0x06008FEE RID: 36846 RVA: 0x0043173D File Offset: 0x0042F93D
		public int currSkillType
		{
			get
			{
				return this.attainmentGroup.GetActiveIndex();
			}
		}

		// Token: 0x17000FB2 RID: 4018
		// (get) Token: 0x06008FEF RID: 36847 RVA: 0x0043174A File Offset: 0x0042F94A
		public int currSkillGradeIndex
		{
			get
			{
				return this.activeSkillLayout.GetActiveIndex();
			}
		}

		// Token: 0x17000FB3 RID: 4019
		// (get) Token: 0x06008FF0 RID: 36848 RVA: 0x00431757 File Offset: 0x0042F957
		private bool _expanded
		{
			get
			{
				return this.rightPanel.activeSelf;
			}
		}

		// Token: 0x06008FF1 RID: 36849 RVA: 0x00431764 File Offset: 0x0042F964
		private void Awake()
		{
			this.attainmentGroup.OnActiveIndexChange += this.AttainmentGroup_OnSkillTogChange;
			this.activeSkillLayout.OnActiveIndexChange += this.ActiveSkillLayout_OnActiveIndexChange;
			this.btnExpand.ClearAndAddListener(delegate
			{
				this.SetExpand(true, false);
			});
			this.btnUnexpand.ClearAndAddListener(delegate
			{
				this.SetExpand(false, false);
				this.activeSkillLayout.DeSelectWithoutNotify();
			});
			this.SetExpand(false, false);
		}

		// Token: 0x06008FF2 RID: 36850 RVA: 0x004317DC File Offset: 0x0042F9DC
		public void SetExpand(bool expand, bool forceSet = false)
		{
			bool flag = !this._expandable && !forceSet;
			if (!flag)
			{
				this.btnExpand.gameObject.SetActive(!expand);
				this.btnUnexpand.gameObject.SetActive(expand);
				this.rightPanel.gameObject.SetActive(expand);
			}
		}

		// Token: 0x06008FF3 RID: 36851 RVA: 0x00431838 File Offset: 0x0042FA38
		public void InitLifeSkill(Action<int> onAttainmentGroupChange, Action<int> onSkillActiveIndexChange, Action<int> onPresetBarActiveIndexChange = null)
		{
			bool inited = this._inited;
			if (!inited)
			{
				this._inited = true;
				this._panelType = CharacterMenuAttainmentPanel.EAttainmentPanelType.LifeSkill;
				this._onPresetBarActiveIndexChange = onPresetBarActiveIndexChange;
				this._onAttainmentGroupChange = onAttainmentGroupChange;
				this._onSkillActiveIndexChange = onSkillActiveIndexChange;
				int totalAmount = 16;
				this.attainmentItems.Clear();
				sbyte i = 0;
				while ((int)i < totalAmount)
				{
					CharacterAttainmentItem togComp = Object.Instantiate<CharacterAttainmentItem>(this.attainmentItemTemplate, this.attainmentGroup.transform);
					this.attainmentItems.Add(togComp);
					this.attainmentGroup.Add(togComp.Toggle);
					togComp.SetLifeSkillType(i);
					i += 1;
				}
				this.attainmentGroup.Init(-1);
				this.activeSkillLayout.Init(-1);
				this.InitActiveSkillSlotPanelType(false);
				this.InitPresetBar();
				this.RefreshBgImagesDisplay();
			}
		}

		// Token: 0x06008FF4 RID: 36852 RVA: 0x0043190C File Offset: 0x0042FB0C
		public void InitCombatSkill(Action<int> onAttainmentGroupChange, Action<int> onSkillActiveIndexChange, Action<int> onPresetBarActiveIndexChange)
		{
			bool inited = this._inited;
			if (!inited)
			{
				this._inited = true;
				this._panelType = CharacterMenuAttainmentPanel.EAttainmentPanelType.CombatSkill;
				this._onPresetBarActiveIndexChange = onPresetBarActiveIndexChange;
				this._onAttainmentGroupChange = onAttainmentGroupChange;
				this._onSkillActiveIndexChange = onSkillActiveIndexChange;
				int totalAmount = 14;
				sbyte i = 0;
				while ((int)i < totalAmount)
				{
					CharacterAttainmentItem togComp = Object.Instantiate<CharacterAttainmentItem>(this.attainmentItemTemplate, this.attainmentGroup.transform);
					this.attainmentItems.Add(togComp);
					this.attainmentGroup.Add(togComp.Toggle);
					togComp.SetCombatSkillType(i);
					i += 1;
				}
				this.attainmentGroup.Init(-1);
				this.activeSkillLayout.Init(-1);
				this.InitActiveSkillSlotPanelType(true);
				this.InitPresetBar();
				this.RefreshBgImagesDisplay();
			}
		}

		// Token: 0x06008FF5 RID: 36853 RVA: 0x004319D4 File Offset: 0x0042FBD4
		private void InitPresetBar()
		{
			this.RefreshPresetBarDisplay();
			bool flag = this.presetBar == null;
			if (!flag)
			{
				this.presetBar.Init(0);
				this.presetBar.OnActiveIndexChange += this.PresetBar_OnActiveIndexChange;
			}
		}

		// Token: 0x06008FF6 RID: 36854 RVA: 0x00431A20 File Offset: 0x0042FC20
		private void InitActiveSkillSlotPanelType(bool isCombatSkill)
		{
			for (int i = 0; i < this.activeSkillLayout.transform.childCount; i++)
			{
				this.activeSkillLayout.transform.GetChild(i).GetComponent<CharacterMenuAttainmentActiveSkill>().SetPanelType(isCombatSkill);
			}
		}

		// Token: 0x06008FF7 RID: 36855 RVA: 0x00431A6C File Offset: 0x0042FC6C
		private void RefreshBgImagesDisplay()
		{
			this.RefreshPanelTypeRawImages(this.bgImages);
			this.RefreshPanelTypeRawImages(this.roleImages);
			this.SwitchUnlockParticleRoot();
		}

		// Token: 0x06008FF8 RID: 36856 RVA: 0x00431A90 File Offset: 0x0042FC90
		private void RefreshPanelTypeRawImages(CRawImage[] images)
		{
			bool flag = images == null || images.Length == 0;
			if (!flag)
			{
				CharacterMenuAttainmentPanel.EAttainmentPanelType panelType = this._panelType;
				if (!true)
				{
				}
				int num;
				if (panelType != CharacterMenuAttainmentPanel.EAttainmentPanelType.CombatSkill)
				{
					if (panelType != CharacterMenuAttainmentPanel.EAttainmentPanelType.LifeSkill)
					{
						num = -1;
					}
					else
					{
						num = 0;
					}
				}
				else
				{
					num = 1;
				}
				if (!true)
				{
				}
				int activeIndex = num;
				for (int i = 0; i < images.Length; i++)
				{
					CRawImage image = images[i];
					bool flag2 = image == null;
					if (!flag2)
					{
						image.gameObject.SetActive(activeIndex >= 0 && i == activeIndex);
					}
				}
			}
		}

		// Token: 0x06008FF9 RID: 36857 RVA: 0x00431B24 File Offset: 0x0042FD24
		private void SwitchUnlockParticleRoot()
		{
			this.EnsureParticleRootBindings();
			CharacterMenuAttainmentPanel.EAttainmentPanelType panelType = this._panelType;
			if (!true)
			{
			}
			int num;
			if (panelType != CharacterMenuAttainmentPanel.EAttainmentPanelType.CombatSkill)
			{
				if (panelType != CharacterMenuAttainmentPanel.EAttainmentPanelType.LifeSkill)
				{
					num = -1;
				}
				else
				{
					num = 0;
				}
			}
			else
			{
				num = 1;
			}
			if (!true)
			{
			}
			int activeIndex = num;
			CharacterMenuAttainmentUnlockParticleCache.ApplyExclusiveParticleRoot(activeIndex);
			bool flag = activeIndex >= 0 && base.gameObject.activeInHierarchy;
			if (flag)
			{
				this.StabilizeUnlockEffectNodesOnActiveRoot();
			}
		}

		// Token: 0x06008FFA RID: 36858 RVA: 0x00431B88 File Offset: 0x0042FD88
		private void EnsureParticleRootBindings()
		{
			bool flag = this.paticles == null || this.paticles.Length < 2;
			if (!flag)
			{
				bool flag2 = this.paticles[0] == null || this.paticles[1] == null;
				if (flag2)
				{
					foreach (UIParticle root in base.GetComponentsInChildren<UIParticle>(true))
					{
						bool flag3 = root == null;
						if (!flag3)
						{
							string objectName = root.gameObject.name;
							bool flag4 = this.paticles[0] == null && objectName == "particle01";
							if (flag4)
							{
								this.paticles[0] = root;
							}
							else
							{
								bool flag5 = this.paticles[1] == null && objectName == "particle02";
								if (flag5)
								{
									this.paticles[1] = root;
								}
							}
						}
					}
				}
				bool flag6 = this.paticles[0] != null;
				if (flag6)
				{
					CharacterMenuAttainmentUnlockParticleCache.RegisterSharedParticleRoot(0, this.paticles[0]);
				}
				bool flag7 = this.paticles[1] != null;
				if (flag7)
				{
					CharacterMenuAttainmentUnlockParticleCache.RegisterSharedParticleRoot(1, this.paticles[1]);
				}
			}
		}

		// Token: 0x06008FFB RID: 36859 RVA: 0x00431CCC File Offset: 0x0042FECC
		private void PresetBar_OnActiveIndexChange(int newIndex, int oldIndex)
		{
			Action<int> onPresetBarActiveIndexChange = this._onPresetBarActiveIndexChange;
			if (onPresetBarActiveIndexChange != null)
			{
				onPresetBarActiveIndexChange(newIndex);
			}
		}

		// Token: 0x06008FFC RID: 36860 RVA: 0x00431CE2 File Offset: 0x0042FEE2
		private void AttainmentGroup_OnSkillTogChange(int newIndex, int oldIndex)
		{
			Action<int> onAttainmentGroupChange = this._onAttainmentGroupChange;
			if (onAttainmentGroupChange != null)
			{
				onAttainmentGroupChange(newIndex);
			}
		}

		// Token: 0x06008FFD RID: 36861 RVA: 0x00431CF8 File Offset: 0x0042FEF8
		private void ActiveSkillLayout_OnActiveIndexChange(int newIndex, int oldIndex)
		{
			Action<int> onSkillActiveIndexChange = this._onSkillActiveIndexChange;
			if (onSkillActiveIndexChange != null)
			{
				onSkillActiveIndexChange(newIndex);
			}
		}

		// Token: 0x06008FFE RID: 36862 RVA: 0x00431D10 File Offset: 0x0042FF10
		private void OnDestroy()
		{
			this.attainmentGroup.OnActiveIndexChange -= this.AttainmentGroup_OnSkillTogChange;
			bool flag = this._unlockEffectCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this._unlockEffectCoroutine);
				this._unlockEffectCoroutine = null;
			}
		}

		// Token: 0x06008FFF RID: 36863 RVA: 0x00431D5C File Offset: 0x0042FF5C
		private void OnEnable()
		{
			bool flag = this.currSkillGradeIndex >= 0 && !this._expanded;
			if (flag)
			{
				this.activeSkillLayout.DeSelectWithoutNotify();
			}
			this.SwitchUnlockParticleRoot();
			this.TryStartPendingUnlockEffects();
		}

		// Token: 0x06009000 RID: 36864 RVA: 0x00431D9D File Offset: 0x0042FF9D
		private void OnDisable()
		{
			this.DeactivateLocalParticleRoots();
		}

		// Token: 0x06009001 RID: 36865 RVA: 0x00431DA8 File Offset: 0x0042FFA8
		private void DeactivateLocalParticleRoots()
		{
			this.EnsureParticleRootBindings();
			bool flag = this.paticles == null;
			if (!flag)
			{
				for (int i = 0; i < this.paticles.Length; i++)
				{
					UIParticle root = this.paticles[i];
					bool flag2 = root != null && root.gameObject.activeSelf;
					if (flag2)
					{
						root.gameObject.SetActive(false);
					}
				}
			}
		}

		// Token: 0x06009002 RID: 36866 RVA: 0x00431E18 File Offset: 0x00430018
		private void TryStartPendingUnlockEffects()
		{
			bool flag = !base.isActiveAndEnabled || this._pendingUnlockEffectRequest == null || this._unlockEffectCoroutine != null;
			if (!flag)
			{
				CharacterMenuAttainmentPanel.PendingUnlockEffectRequest request = this._pendingUnlockEffectRequest.Value;
				this._pendingUnlockEffectRequest = null;
				this._unlockEffectCoroutine = base.StartCoroutine(this.PlayUnlockEffects(request.ActiveSkillDataList, request.SlotUnlocked, request.PendingSlots));
			}
		}

		// Token: 0x06009003 RID: 36867 RVA: 0x00431E8C File Offset: 0x0043008C
		private void StartUnlockEffectsIfNeeded(List<CharacterMenuAttainmentActiveSkillData> activeSkillDataList, bool[] slotUnlocked, List<int> pendingUnlockSlots)
		{
			bool flag = pendingUnlockSlots.Count == 0;
			if (flag)
			{
				this._pendingUnlockEffectRequest = null;
			}
			else
			{
				bool isActiveAndEnabled = base.isActiveAndEnabled;
				if (isActiveAndEnabled)
				{
					this._pendingUnlockEffectRequest = null;
					this._unlockEffectCoroutine = base.StartCoroutine(this.PlayUnlockEffects(activeSkillDataList, slotUnlocked, pendingUnlockSlots));
				}
				else
				{
					this._pendingUnlockEffectRequest = new CharacterMenuAttainmentPanel.PendingUnlockEffectRequest?(new CharacterMenuAttainmentPanel.PendingUnlockEffectRequest
					{
						ActiveSkillDataList = activeSkillDataList,
						SlotUnlocked = slotUnlocked,
						PendingSlots = pendingUnlockSlots
					});
				}
			}
		}

		// Token: 0x06009004 RID: 36868 RVA: 0x00431F10 File Offset: 0x00430110
		public void SetSecondaryTitle(string content = "")
		{
			bool flag = this.secondaryTxt != null && !this.secondaryTxt.gameObject.activeSelf;
			if (flag)
			{
				this.secondaryTxt.gameObject.SetActive(true);
			}
			this.secondaryTxt.text = content;
		}

		// Token: 0x06009005 RID: 36869 RVA: 0x00431F68 File Offset: 0x00430168
		public void SetQualificationAttainmentItem(sbyte type, int valueQualification, int skillLevel, string valueAttainmentText)
		{
			int num = (this._panelType == CharacterMenuAttainmentPanel.EAttainmentPanelType.CombatSkill) ? 14 : 16;
			CharacterAttainmentItem leftItem = this.attainmentGroup.Get((int)type).GetComponent<CharacterAttainmentItem>();
			leftItem.SetQualificationText(valueQualification.SetValueColor());
			leftItem.SetAttainmentText(valueAttainmentText);
		}

		// Token: 0x06009006 RID: 36870 RVA: 0x00431FB0 File Offset: 0x004301B0
		private void SetDataToActiveSkillSlot(int grade, CharacterMenuAttainmentActiveSkillData data, bool isAnim = false)
		{
			bool flag = grade < 0 || grade >= this.activeSkillLayout.transform.childCount;
			if (!flag)
			{
				this.activeSkillLayout.transform.GetChild(grade).GetComponent<CharacterMenuAttainmentActiveSkill>().Set(data, grade, isAnim);
				this.activeSkillLayout.SetInteractable(data != null && data.Locked, grade);
			}
		}

		// Token: 0x06009007 RID: 36871 RVA: 0x0043201C File Offset: 0x0043021C
		private CharacterMenuAttainmentActiveSkill GetActiveSkillSlot(int grade)
		{
			return this.activeSkillLayout.transform.GetChild(grade).GetComponent<CharacterMenuAttainmentActiveSkill>();
		}

		// Token: 0x06009008 RID: 36872 RVA: 0x00432044 File Offset: 0x00430244
		private void HideAllUnlockMasks()
		{
			for (int i = 0; i < this.activeSkillLayout.transform.childCount; i++)
			{
				this.GetActiveSkillSlot(i).HideUnlockMask();
			}
		}

		// Token: 0x06009009 RID: 36873 RVA: 0x0043207E File Offset: 0x0043027E
		private static bool IsUnlockEffectNodeName(string nodeName)
		{
			return CharacterMenuAttainmentUnlockParticleCache.IsUnlockEffectNodeName(nodeName);
		}

		// Token: 0x0600900A RID: 36874 RVA: 0x00432088 File Offset: 0x00430288
		private static float GetUnlockParticlePlayDuration(string nodeName)
		{
			bool flag = !string.IsNullOrEmpty(nodeName) && nodeName.Length == 5 && nodeName.StartsWith("shu0");
			float result;
			if (flag)
			{
				result = 0.3f;
			}
			else
			{
				result = 0.1f;
			}
			return result;
		}

		// Token: 0x0600900B RID: 36875 RVA: 0x004320CA File Offset: 0x004302CA
		private IEnumerator WaitUnlockParticleDuration(string nodeName)
		{
			yield return new WaitForSeconds(CharacterMenuAttainmentPanel.GetUnlockParticlePlayDuration(nodeName));
			yield break;
		}

		// Token: 0x0600900C RID: 36876 RVA: 0x004320E0 File Offset: 0x004302E0
		private static bool IsSlotEligibleForUnlockEffect(int grade, bool[] slotUnlocked)
		{
			return slotUnlocked != null && grade > 0 && grade < slotUnlocked.Length && slotUnlocked[grade];
		}

		// Token: 0x0600900D RID: 36877 RVA: 0x004320F8 File Offset: 0x004302F8
		private int GetParticleRootIndex()
		{
			CharacterMenuAttainmentPanel.EAttainmentPanelType panelType = this._panelType;
			if (!true)
			{
			}
			int result;
			if (panelType != CharacterMenuAttainmentPanel.EAttainmentPanelType.CombatSkill)
			{
				if (panelType != CharacterMenuAttainmentPanel.EAttainmentPanelType.LifeSkill)
				{
					result = -1;
				}
				else
				{
					result = 0;
				}
			}
			else
			{
				result = 1;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600900E RID: 36878 RVA: 0x00432134 File Offset: 0x00430334
		private static int GetSlotGradeFromEffectNodeName(string nodeName)
		{
			bool flag = nodeName == "xishou";
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool flag2 = CharacterMenuAttainmentUnlockParticleCache.IsShuNodeName(nodeName) || CharacterMenuAttainmentUnlockParticleCache.IsXianNodeName(nodeName);
				if (flag2)
				{
					result = int.Parse(nodeName.Substring(4));
				}
				else
				{
					result = -1;
				}
			}
			return result;
		}

		// Token: 0x0600900F RID: 36879 RVA: 0x0043217E File Offset: 0x0043037E
		private bool IsSlotUnlockedForEffect(int grade)
		{
			return this._currentSlotUnlocked != null && grade >= 0 && grade < this._currentSlotUnlocked.Length && this._currentSlotUnlocked[grade];
		}

		// Token: 0x06009010 RID: 36880 RVA: 0x004321A4 File Offset: 0x004303A4
		private void StabilizeUnlockEffectNodesOnActiveRoot()
		{
			UIParticle root = this.GetUnlockParticleRoot();
			bool flag = root == null || !root.gameObject.activeSelf;
			if (!flag)
			{
				HashSet<string> keepActive = this.BuildKeepActiveNodeSet(Array.Empty<string>());
				foreach (Transform node in root.GetComponentsInChildren<Transform>(true))
				{
					bool flag2 = !CharacterMenuAttainmentPanel.IsUnlockEffectNodeName(node.name);
					if (!flag2)
					{
						bool flag3 = CharacterMenuAttainmentUnlockParticleCache.IsShuNodeName(node.name);
						if (flag3)
						{
							bool flag4 = keepActive.Contains(node.name) || CharacterMenuAttainmentPanel.IsShuPersistentlyVisible(this.GetParticleRootIndex(), node.name);
							if (flag4)
							{
								CharacterMenuAttainmentPanel.ShowShuNode(node);
								goto IL_F3;
							}
						}
						ParticleSystem ps = node.GetComponent<ParticleSystem>();
						bool flag5 = ps != null;
						if (flag5)
						{
							ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
						}
						bool activeSelf = node.gameObject.activeSelf;
						if (activeSelf)
						{
							node.gameObject.SetActive(false);
						}
					}
					IL_F3:;
				}
			}
		}

		// Token: 0x06009011 RID: 36881 RVA: 0x004322BC File Offset: 0x004304BC
		private HashSet<string> BuildKeepActiveNodeSet(params string[] extraNodeNames)
		{
			CharacterMenuAttainmentUnlockParticleCache.CollectPlayedShuNodeNames(this.GetParticleRootIndex(), this._playedShuNodeNameBuffer);
			HashSet<string> keepActive = new HashSet<string>(this._playedShuNodeNameBuffer);
			bool flag = extraNodeNames == null;
			HashSet<string> result;
			if (flag)
			{
				result = keepActive;
			}
			else
			{
				for (int i = 0; i < extraNodeNames.Length; i++)
				{
					bool flag2 = !string.IsNullOrEmpty(extraNodeNames[i]);
					if (flag2)
					{
						keepActive.Add(extraNodeNames[i]);
					}
				}
				result = keepActive;
			}
			return result;
		}

		// Token: 0x06009012 RID: 36882 RVA: 0x0043232C File Offset: 0x0043052C
		private void RestoreAllPlayedShuVisuals()
		{
			int particleRootIndex = this.GetParticleRootIndex();
			CharacterMenuAttainmentUnlockParticleCache.CollectPlayedShuNodeNames(particleRootIndex, this._playedShuNodeNameBuffer);
			for (int i = 0; i < this._playedShuNodeNameBuffer.Count; i++)
			{
				ParticleSystem ps = this.FindUnlockSlotParticleSystem(this._playedShuNodeNameBuffer[i]);
				bool flag = ps != null;
				if (flag)
				{
					CharacterMenuAttainmentPanel.ShowShuNode(ps.transform);
				}
			}
		}

		// Token: 0x06009013 RID: 36883 RVA: 0x00432396 File Offset: 0x00430596
		private static bool IsShuPersistentlyVisible(int particleRootIndex, string shuNodeName)
		{
			return CharacterMenuAttainmentUnlockParticleCache.IsShuNodePlayed(particleRootIndex, shuNodeName);
		}

		// Token: 0x06009014 RID: 36884 RVA: 0x004323A0 File Offset: 0x004305A0
		private UIParticle GetUnlockParticleRoot()
		{
			this.EnsureParticleRootBindings();
			CharacterMenuAttainmentPanel.EAttainmentPanelType panelType = this._panelType;
			if (!true)
			{
			}
			int num;
			if (panelType != CharacterMenuAttainmentPanel.EAttainmentPanelType.CombatSkill)
			{
				if (panelType != CharacterMenuAttainmentPanel.EAttainmentPanelType.LifeSkill)
				{
					num = -1;
				}
				else
				{
					num = 0;
				}
			}
			else
			{
				num = 1;
			}
			if (!true)
			{
			}
			int index = num;
			bool flag = index < 0;
			UIParticle result;
			if (flag)
			{
				result = null;
			}
			else
			{
				UIParticle shared = CharacterMenuAttainmentUnlockParticleCache.GetSharedParticleRoot(index);
				bool flag2 = shared != null;
				if (flag2)
				{
					result = shared;
				}
				else
				{
					bool flag3 = this.paticles == null || index >= this.paticles.Length;
					if (flag3)
					{
						result = null;
					}
					else
					{
						result = this.paticles[index];
					}
				}
			}
			return result;
		}

		// Token: 0x06009015 RID: 36885 RVA: 0x00432438 File Offset: 0x00430638
		private ParticleSystem FindUnlockSlotParticleSystem(string nodeName)
		{
			bool flag = string.IsNullOrEmpty(nodeName);
			ParticleSystem result;
			if (flag)
			{
				result = null;
			}
			else
			{
				ParticleSystem cached;
				bool flag2 = this._slotParticleCache.TryGetValue(nodeName, out cached);
				if (flag2)
				{
					result = cached;
				}
				else
				{
					UIParticle root = this.GetUnlockParticleRoot();
					bool flag3 = root == null;
					if (flag3)
					{
						result = null;
					}
					else
					{
						Transform[] transforms = root.GetComponentsInChildren<Transform>(true);
						for (int i = 0; i < transforms.Length; i++)
						{
							bool flag4 = transforms[i].name != nodeName;
							if (!flag4)
							{
								ParticleSystem ps = transforms[i].GetComponent<ParticleSystem>();
								bool flag5 = ps == null;
								if (!flag5)
								{
									this._slotParticleCache[nodeName] = ps;
									return ps;
								}
							}
						}
						result = null;
					}
				}
			}
			return result;
		}

		// Token: 0x06009016 RID: 36886 RVA: 0x004324FC File Offset: 0x004306FC
		private void PrepareUnlockEffectNodesBatch(IReadOnlyList<string> activateNodeNames, params string[] keepActiveNodeNames)
		{
			UIParticle root = this.GetUnlockParticleRoot();
			bool flag = root == null || activateNodeNames == null || activateNodeNames.Count == 0;
			if (!flag)
			{
				HashSet<string> activateSet = new HashSet<string>(activateNodeNames);
				HashSet<string> keepActive = this.BuildKeepActiveNodeSet(keepActiveNodeNames);
				foreach (string nodeName in activateSet)
				{
					keepActive.Add(nodeName);
				}
				int particleRootIndex = this.GetParticleRootIndex();
				foreach (Transform node in root.GetComponentsInChildren<Transform>(true))
				{
					bool flag2 = !CharacterMenuAttainmentPanel.IsUnlockEffectNodeName(node.name);
					if (!flag2)
					{
						bool flag3 = CharacterMenuAttainmentUnlockParticleCache.IsShuNodeName(node.name);
						if (flag3)
						{
							bool flag4 = keepActive.Contains(node.name) || CharacterMenuAttainmentPanel.IsShuPersistentlyVisible(particleRootIndex, node.name);
							if (flag4)
							{
								CharacterMenuAttainmentPanel.ShowShuNode(node);
								goto IL_1BB;
							}
						}
						bool flag5 = activateSet.Contains(node.name);
						if (flag5)
						{
							bool flag6 = !node.gameObject.activeSelf;
							if (flag6)
							{
								node.gameObject.SetActive(true);
							}
							ParticleSystem ps = node.GetComponent<ParticleSystem>();
							bool flag7 = ps == null;
							if (!flag7)
							{
								ParticleSystemRenderer renderer = ps.GetComponent<ParticleSystemRenderer>();
								bool flag8 = renderer != null;
								if (flag8)
								{
									renderer.enabled = true;
								}
							}
						}
						else
						{
							bool flag9 = keepActive.Contains(node.name);
							if (!flag9)
							{
								ParticleSystem inactivePs = node.GetComponent<ParticleSystem>();
								bool flag10 = inactivePs != null;
								if (flag10)
								{
									inactivePs.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
								}
								bool activeSelf = node.gameObject.activeSelf;
								if (activeSelf)
								{
									node.gameObject.SetActive(false);
								}
							}
						}
					}
					IL_1BB:;
				}
			}
		}

		// Token: 0x06009017 RID: 36887 RVA: 0x004326EC File Offset: 0x004308EC
		private void PrepareUnlockEffectNode(string nodeName, params string[] keepActiveNodeNames)
		{
			this.PrepareUnlockEffectNodesBatch(new string[]
			{
				nodeName
			}, keepActiveNodeNames);
		}

		// Token: 0x06009018 RID: 36888 RVA: 0x00432700 File Offset: 0x00430900
		private static void ShowShuNode(Transform node)
		{
			bool flag = !node.gameObject.activeSelf;
			if (flag)
			{
				node.gameObject.SetActive(true);
			}
			ParticleSystem ps = node.GetComponent<ParticleSystem>();
			bool flag2 = ps == null;
			if (!flag2)
			{
				ParticleSystemRenderer renderer = ps.GetComponent<ParticleSystemRenderer>();
				bool flag3 = renderer != null;
				if (flag3)
				{
					renderer.enabled = true;
				}
				bool isPlaying = ps.isPlaying;
				if (isPlaying)
				{
					ps.Stop(false, ParticleSystemStopBehavior.StopEmitting);
				}
			}
		}

		// Token: 0x06009019 RID: 36889 RVA: 0x00432774 File Offset: 0x00430974
		private void ResetTransientUnlockEffectNodes()
		{
			UIParticle root = this.GetUnlockParticleRoot();
			bool flag = root == null;
			if (!flag)
			{
				foreach (Transform node in root.GetComponentsInChildren<Transform>(true))
				{
					string nodeName = node.name;
					bool flag2 = nodeName == "xishou" || CharacterMenuAttainmentUnlockParticleCache.IsXianNodeName(nodeName);
					if (flag2)
					{
						ParticleSystem ps = node.GetComponent<ParticleSystem>();
						bool flag3 = ps != null;
						if (flag3)
						{
							ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
						}
						bool activeSelf = node.gameObject.activeSelf;
						if (activeSelf)
						{
							node.gameObject.SetActive(false);
						}
					}
				}
			}
		}

		// Token: 0x0600901A RID: 36890 RVA: 0x0043282C File Offset: 0x00430A2C
		private void ResetAllUnlockEffectNodes()
		{
			UIParticle root = this.GetUnlockParticleRoot();
			bool flag = root == null;
			if (!flag)
			{
				foreach (Transform node in root.GetComponentsInChildren<Transform>(true))
				{
					bool flag2 = !CharacterMenuAttainmentPanel.IsUnlockEffectNodeName(node.name);
					if (!flag2)
					{
						ParticleSystem ps = node.GetComponent<ParticleSystem>();
						bool flag3 = ps != null;
						if (flag3)
						{
							ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
						}
						bool activeSelf = node.gameObject.activeSelf;
						if (activeSelf)
						{
							node.gameObject.SetActive(false);
						}
					}
				}
			}
		}

		// Token: 0x0600901B RID: 36891 RVA: 0x004328CC File Offset: 0x00430ACC
		private void HideShuNode(Transform node)
		{
			ParticleSystem ps = node.GetComponent<ParticleSystem>();
			bool flag = ps != null;
			if (flag)
			{
				ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			}
			bool activeSelf = node.gameObject.activeSelf;
			if (activeSelf)
			{
				node.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600901C RID: 36892 RVA: 0x00432914 File Offset: 0x00430B14
		private void HideXianNode(Transform node)
		{
			ParticleSystem ps = node.GetComponent<ParticleSystem>();
			bool flag = ps != null;
			if (flag)
			{
				ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			}
			bool activeSelf = node.gameObject.activeSelf;
			if (activeSelf)
			{
				node.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600901D RID: 36893 RVA: 0x0043295C File Offset: 0x00430B5C
		private void SyncUnlockEffectVisualsForSlotState()
		{
			bool flag = this._currentSlotUnlocked == null;
			if (!flag)
			{
				this.SwitchUnlockParticleRoot();
				this.RestoreAllPlayedShuVisuals();
				int particleRootIndex = this.GetParticleRootIndex();
				int grade = 0;
				while (grade < 9 && grade < this._currentSlotUnlocked.Length)
				{
					bool flag2 = grade == 0;
					if (flag2)
					{
						ParticleSystem xishouPs = this.FindUnlockSlotParticleSystem("xishou");
						bool flag3 = xishouPs != null && xishouPs.gameObject.activeSelf;
						if (flag3)
						{
							this.HideXianNode(xishouPs.transform);
						}
					}
					else
					{
						string shuNodeName = CharacterMenuAttainmentUnlockParticleCache.GetShuNodeName(grade);
						string xianNodeName = CharacterMenuAttainmentUnlockParticleCache.GetXianNodeName(grade);
						ParticleSystem shuPs = this.FindUnlockSlotParticleSystem(shuNodeName);
						ParticleSystem xianPs = this.FindUnlockSlotParticleSystem(xianNodeName);
						bool flag4 = xianPs != null && xianPs.gameObject.activeSelf;
						if (flag4)
						{
							this.HideXianNode(xianPs.transform);
						}
						bool flag5 = CharacterMenuAttainmentPanel.IsShuPersistentlyVisible(particleRootIndex, shuNodeName);
						if (!flag5)
						{
							bool flag6 = !this.IsSlotUnlockedForEffect(grade);
							if (flag6)
							{
								bool flag7 = shuPs != null;
								if (flag7)
								{
									this.HideShuNode(shuPs.transform);
								}
							}
						}
					}
					grade++;
				}
			}
		}

		// Token: 0x0600901E RID: 36894 RVA: 0x00432A90 File Offset: 0x00430C90
		private IEnumerator PlayUnlockSlotParticle(string nodeName, bool keepVisibleAfterPlay = false, bool forcePlay = false, bool replayFlash = false, params string[] keepActiveNodeNames)
		{
			int grade = CharacterMenuAttainmentPanel.GetSlotGradeFromEffectNodeName(nodeName);
			int particleRootIndex = this.GetParticleRootIndex();
			bool alreadyPlayed = CharacterMenuAttainmentUnlockParticleCache.IsEffectNodePlayed(particleRootIndex, nodeName, this.debugAlwaysReplayUnlockEffects);
			bool flag = !forcePlay && !replayFlash && alreadyPlayed;
			if (flag)
			{
				bool flag2 = (keepVisibleAfterPlay || CharacterMenuAttainmentUnlockParticleCache.IsShuNodeName(nodeName)) && grade >= 0 && this.IsSlotUnlockedForEffect(grade);
				if (flag2)
				{
					ParticleSystem cachedPs = this.FindUnlockSlotParticleSystem(nodeName);
					bool flag3 = cachedPs != null;
					if (flag3)
					{
						CharacterMenuAttainmentPanel.ShowShuNode(cachedPs.transform);
					}
					cachedPs = null;
				}
				yield break;
			}
			bool flag4 = grade >= 0 && !this.IsSlotUnlockedForEffect(grade);
			if (flag4)
			{
				yield break;
			}
			ParticleSystem ps = this.FindUnlockSlotParticleSystem(nodeName);
			bool flag5 = ps == null;
			if (flag5)
			{
				yield break;
			}
			this.SwitchUnlockParticleRoot();
			this.PrepareUnlockEffectNode(nodeName, keepActiveNodeNames);
			ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			ps.Play(true);
			bool flag6 = !replayFlash && !alreadyPlayed;
			if (flag6)
			{
				CharacterMenuAttainmentUnlockParticleCache.MarkEffectNodePlayed(particleRootIndex, nodeName, this.debugAlwaysReplayUnlockEffects);
			}
			else
			{
				bool flag7 = CharacterMenuAttainmentUnlockParticleCache.IsShuNodeName(nodeName);
				if (flag7)
				{
					CharacterMenuAttainmentUnlockParticleCache.MarkShuNodePlayed(particleRootIndex, nodeName);
				}
			}
			yield return this.WaitUnlockParticleDuration(nodeName);
			bool flag8 = keepVisibleAfterPlay || CharacterMenuAttainmentUnlockParticleCache.IsShuNodeName(nodeName);
			if (flag8)
			{
				ps.Stop(false, ParticleSystemStopBehavior.StopEmitting);
				CharacterMenuAttainmentPanel.ShowShuNode(ps.transform);
			}
			else
			{
				ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
				bool activeSelf = ps.gameObject.activeSelf;
				if (activeSelf)
				{
					ps.gameObject.SetActive(false);
				}
			}
			yield break;
		}

		// Token: 0x0600901F RID: 36895 RVA: 0x00432AC4 File Offset: 0x00430CC4
		private static string GetUnlockSlotParticleNodeName(int slotGrade, bool isXianNode)
		{
			bool flag = slotGrade <= 0;
			string result;
			if (flag)
			{
				result = "xishou";
			}
			else
			{
				result = (isXianNode ? CharacterMenuAttainmentUnlockParticleCache.GetXianNodeName(slotGrade) : CharacterMenuAttainmentUnlockParticleCache.GetShuNodeName(slotGrade));
			}
			return result;
		}

		// Token: 0x06009020 RID: 36896 RVA: 0x00432AFA File Offset: 0x00430CFA
		private IEnumerator PlayUnlockEffects(List<CharacterMenuAttainmentActiveSkillData> activeSkillDataList, bool[] slotUnlocked, List<int> pendingSlots)
		{
			bool flag = pendingSlots == null || pendingSlots.Count == 0;
			if (flag)
			{
				this._unlockEffectCoroutine = null;
				yield break;
			}
			this.SwitchUnlockParticleRoot();
			int num;
			for (int i = 0; i < pendingSlots.Count; i = num + 1)
			{
				int grade = pendingSlots[i];
				this.GetActiveSkillSlot(grade).ShowUnlockMask();
				this.activeSkillLayout.SetInteractable(false, grade);
				num = i;
			}
			for (int j = 0; j < pendingSlots.Count; j = num + 1)
			{
				int grade2 = pendingSlots[j];
				CharacterMenuAttainmentActiveSkill slot = this.GetActiveSkillSlot(grade2);
				CharacterMenuAttainmentActiveSkillData data = (grade2 < activeSkillDataList.Count) ? activeSkillDataList[grade2] : null;
				bool flag2 = grade2 == 0;
				if (flag2)
				{
					yield return this.PlayUnlockSlotParticle("xishou", false, true, false, Array.Empty<string>());
				}
				else
				{
					string shuNode = CharacterMenuAttainmentPanel.GetUnlockSlotParticleNodeName(grade2, false);
					string xianNode = CharacterMenuAttainmentPanel.GetUnlockSlotParticleNodeName(grade2, true);
					yield return this.PlayUnlockSlotParticle(shuNode, true, true, false, Array.Empty<string>());
					slot.HideUnlockMaskOnly();
					yield return this.PlayUnlockSlotParticle(xianNode, false, true, false, new string[]
					{
						shuNode
					});
					shuNode = null;
					xianNode = null;
				}
				this.SetDataToActiveSkillSlot(grade2, data, false);
				slot.HideUnlockMask();
				this.activeSkillLayout.SetInteractable(data != null && data.Locked, grade2);
				slot = null;
				data = null;
				num = j;
			}
			this._unlockEffectCoroutine = null;
			yield break;
		}

		// Token: 0x06009021 RID: 36897 RVA: 0x00432B20 File Offset: 0x00430D20
		private void CollectRevisitFlashSlots(bool[] slotUnlocked, int slotCount)
		{
			this._revisitFlashSlotBuffer.Clear();
			bool flag = !this.flashUnlockEffectOnRevisit || this.debugAlwaysReplayUnlockEffects || slotUnlocked == null;
			if (!flag)
			{
				int particleRootIndex = this.GetParticleRootIndex();
				sbyte grade = 0;
				while ((int)grade < slotCount && (int)grade < slotUnlocked.Length)
				{
					bool flag2 = !slotUnlocked[(int)grade];
					if (!flag2)
					{
						string playedNodeName = (grade == 0) ? "xishou" : CharacterMenuAttainmentUnlockParticleCache.GetShuNodeName((int)grade);
						bool flag3 = !CharacterMenuAttainmentUnlockParticleCache.IsEffectNodePlayed(particleRootIndex, playedNodeName, false);
						if (!flag3)
						{
							this._revisitFlashSlotBuffer.Add((int)grade);
						}
					}
					grade += 1;
				}
			}
		}

		// Token: 0x06009022 RID: 36898 RVA: 0x00432BBC File Offset: 0x00430DBC
		private void StartRevisitFlashEffectsIfNeeded()
		{
			bool flag = this._revisitFlashSlotBuffer.Count == 0;
			if (!flag)
			{
				bool flag2 = !base.isActiveAndEnabled;
				if (!flag2)
				{
					this._unlockEffectCoroutine = base.StartCoroutine(this.PlayRevisitFlashEffects(new List<int>(this._revisitFlashSlotBuffer)));
				}
			}
		}

		// Token: 0x06009023 RID: 36899 RVA: 0x00432C0C File Offset: 0x00430E0C
		private void FireUnlockSlotParticle(string nodeName)
		{
			ParticleSystem ps = this.FindUnlockSlotParticleSystem(nodeName);
			bool flag = ps == null;
			if (!flag)
			{
				ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
				ps.Play(true);
			}
		}

		// Token: 0x06009024 RID: 36900 RVA: 0x00432C40 File Offset: 0x00430E40
		private void FinalizeShuParticleAfterPlay(string nodeName)
		{
			ParticleSystem ps = this.FindUnlockSlotParticleSystem(nodeName);
			bool flag = ps == null;
			if (!flag)
			{
				ps.Stop(false, ParticleSystemStopBehavior.StopEmitting);
				CharacterMenuAttainmentPanel.ShowShuNode(ps.transform);
			}
		}

		// Token: 0x06009025 RID: 36901 RVA: 0x00432C78 File Offset: 0x00430E78
		private void FinalizeTransientParticleAfterPlay(string nodeName)
		{
			ParticleSystem ps = this.FindUnlockSlotParticleSystem(nodeName);
			bool flag = ps == null;
			if (!flag)
			{
				ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
				bool activeSelf = ps.gameObject.activeSelf;
				if (activeSelf)
				{
					ps.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06009026 RID: 36902 RVA: 0x00432CC0 File Offset: 0x00430EC0
		private IEnumerator PlayRevisitFlashEffects(List<int> grades)
		{
			this.SwitchUnlockParticleRoot();
			this._revisitFlashNodeBuffer.Clear();
			List<string> shuNodes = this._revisitFlashNodeBuffer;
			List<string> xianNodes = new List<string>();
			bool hasXishou = false;
			int num;
			for (int i = 0; i < grades.Count; i = num + 1)
			{
				int grade = grades[i];
				bool flag = !this.IsSlotUnlockedForEffect(grade);
				if (!flag)
				{
					bool flag2 = grade == 0;
					if (flag2)
					{
						hasXishou = true;
					}
					else
					{
						shuNodes.Add(CharacterMenuAttainmentPanel.GetUnlockSlotParticleNodeName(grade, false));
						xianNodes.Add(CharacterMenuAttainmentPanel.GetUnlockSlotParticleNodeName(grade, true));
					}
				}
				num = i;
			}
			bool flag3 = !hasXishou && shuNodes.Count == 0;
			if (flag3)
			{
				this._unlockEffectCoroutine = null;
				yield break;
			}
			List<string> phase1Nodes = new List<string>(shuNodes);
			bool flag4 = hasXishou;
			if (flag4)
			{
				phase1Nodes.Add("xishou");
			}
			this.PrepareUnlockEffectNodesBatch(phase1Nodes, shuNodes.ToArray());
			for (int j = 0; j < phase1Nodes.Count; j = num + 1)
			{
				this.FireUnlockSlotParticle(phase1Nodes[j]);
				num = j;
			}
			yield return new WaitForSeconds(0.3f);
			for (int k = 0; k < shuNodes.Count; k = num + 1)
			{
				this.FinalizeShuParticleAfterPlay(shuNodes[k]);
				num = k;
			}
			bool flag5 = hasXishou;
			if (flag5)
			{
				this.FinalizeTransientParticleAfterPlay("xishou");
			}
			bool flag6 = xianNodes.Count == 0;
			if (flag6)
			{
				this._unlockEffectCoroutine = null;
				yield break;
			}
			this.PrepareUnlockEffectNodesBatch(xianNodes, shuNodes.ToArray());
			for (int l = 0; l < xianNodes.Count; l = num + 1)
			{
				this.FireUnlockSlotParticle(xianNodes[l]);
				num = l;
			}
			yield return new WaitForSeconds(0.1f);
			for (int m = 0; m < xianNodes.Count; m = num + 1)
			{
				this.FinalizeTransientParticleAfterPlay(xianNodes[m]);
				num = m;
			}
			this._unlockEffectCoroutine = null;
			yield break;
		}

		// Token: 0x06009027 RID: 36903 RVA: 0x00432CD8 File Offset: 0x00430ED8
		public void SetActiveSkills(List<CharacterMenuAttainmentActiveSkillData> activeSkillDataList, bool autoSelectBookTog, int skillTypeIndex = 0, bool[] slotUnlocked = null)
		{
			this._currentSkillTypeIndex = skillTypeIndex;
			this._currentSlotUnlocked = slotUnlocked;
			bool flag = this._unlockEffectCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this._unlockEffectCoroutine);
				this._unlockEffectCoroutine = null;
			}
			this._pendingUnlockEffectRequest = null;
			this.HideAllUnlockMasks();
			this._slotParticleCache.Clear();
			bool flag2 = this.debugAlwaysReplayUnlockEffects;
			if (flag2)
			{
				CharacterMenuAttainmentUnlockParticleCache.ClearParticleRoot(this.GetParticleRootIndex());
				this.ResetAllUnlockEffectNodes();
			}
			else
			{
				this.ResetTransientUnlockEffectNodes();
			}
			this.SwitchUnlockParticleRoot();
			int slotCount = this.activeSkillLayout.transform.childCount;
			List<int> pendingUnlockSlots = new List<int>();
			sbyte grade = 0;
			while ((int)grade < slotCount)
			{
				CharacterMenuAttainmentActiveSkillData data = ((int)grade < activeSkillDataList.Count) ? activeSkillDataList[(int)grade] : null;
				this.SetDataToActiveSkillSlot((int)grade, data, false);
				this.GetActiveSkillSlot((int)grade).HideUnlockMask();
				grade += 1;
			}
			this.SyncUnlockEffectVisualsForSlotState();
			sbyte grade2 = 0;
			while ((int)grade2 < slotCount)
			{
				bool flag3 = !CharacterMenuAttainmentUnlockParticleCache.ShouldPlaySlotUnlockEffect(this.GetParticleRootIndex(), (int)grade2, slotUnlocked, this.debugAlwaysReplayUnlockEffects);
				if (!flag3)
				{
					pendingUnlockSlots.Add((int)grade2);
				}
				grade2 += 1;
			}
			bool flag4 = pendingUnlockSlots.Count > 0;
			if (flag4)
			{
				CharacterMenuAttainmentUnlockParticleCache.MarkSlotPrimaryNodesPlayed(this.GetParticleRootIndex(), pendingUnlockSlots, this.debugAlwaysReplayUnlockEffects);
				this.StartUnlockEffectsIfNeeded(activeSkillDataList, slotUnlocked, pendingUnlockSlots);
			}
			else
			{
				this.CollectRevisitFlashSlots(slotUnlocked, slotCount);
				this.StartRevisitFlashEffectsIfNeeded();
			}
		}

		// Token: 0x06009028 RID: 36904 RVA: 0x00432E58 File Offset: 0x00431058
		private void ApplyActiveSkillTreeAnimation(List<CharacterMenuAttainmentActiveSkillData> activeSkillDataList, bool[] slotUnlocked, bool startFromHeadAnim)
		{
			CharacterMenuAttainmentPanel.<>c__DisplayClass113_0 CS$<>8__locals1 = new CharacterMenuAttainmentPanel.<>c__DisplayClass113_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.activeSkillDataList = activeSkillDataList;
			bool flag = this._activeSkillTreeSequence != null;
			if (flag)
			{
				this._activeSkillTreeSequence.Kill(false);
				this._activeSkillTreeSequence = null;
			}
			for (int i = 0; i < this._treeTrunkTweeners.Count; i++)
			{
				this._treeTrunkTweeners[i].Kill(false);
				this._treeTrunkTweeners[i] = null;
			}
			this._treeTrunkTweeners.Clear();
			List<int> needLightUpGoldenVein = new List<int>();
			CS$<>8__locals1.needLightUpTreeTrunk = new List<int>();
			sbyte grade4 = 0;
			while ((int)grade4 < CS$<>8__locals1.activeSkillDataList.Count)
			{
				this.SetDataToActiveSkillSlot((int)grade4, null, false);
				bool flag2 = CS$<>8__locals1.activeSkillDataList[(int)grade4] != null;
				if (flag2)
				{
					List<int> list;
					bool flag3 = CharacterMenuAttainmentPanel.Slot2GoldenVeinCorrelation.TryGetValue((int)grade4, out list);
					if (flag3)
					{
						needLightUpGoldenVein.AddRange(list);
					}
					else
					{
						needLightUpGoldenVein.Add((int)grade4);
					}
				}
				foreach (KeyValuePair<int, List<int>> keyValuePair in CharacterMenuAttainmentPanel.Slot2TreeTrunkCorrelation)
				{
					int num;
					List<int> list2;
					keyValuePair.Deconstruct(out num, out list2);
					int treeTrunkIndex = num;
					bool flag4 = CS$<>8__locals1.needLightUpTreeTrunk.Contains(treeTrunkIndex);
					if (!flag4)
					{
						bool isLightUp = CS$<>8__locals1.<ApplyActiveSkillTreeAnimation>g__IsLightUpTreeTrunkParticle|2(treeTrunkIndex);
						bool flag5 = isLightUp;
						if (flag5)
						{
							CS$<>8__locals1.needLightUpTreeTrunk.Add(treeTrunkIndex);
						}
					}
				}
				grade4 += 1;
			}
			needLightUpGoldenVein = (from x in needLightUpGoldenVein.Distinct<int>()
			orderby x
			select x).ToList<int>();
			needLightUpGoldenVein.Remove(0);
			bool flag6 = !startFromHeadAnim;
			if (flag6)
			{
				this._activeSkillTreeSequence = DOTween.Sequence();
				sbyte grade2 = 0;
				while ((int)grade2 < CS$<>8__locals1.activeSkillDataList.Count)
				{
					this.SetDataToActiveSkillSlot((int)grade2, CS$<>8__locals1.activeSkillDataList[(int)grade2], false);
					bool flag7 = grade2 > 0;
					if (flag7)
					{
						this._activeSkillTreeSequence.Join(this.SetPSDissolveAmount(this.goldenVeinParticles[(int)grade2], needLightUpGoldenVein.Contains((int)grade2) ? -0.1f : 1f, 0.5f, true));
					}
					grade2 += 1;
				}
				for (int j = 0; j < this.treeTrunkParticles.Count; j++)
				{
					this._activeSkillTreeSequence.Join(this.SetPSDissolveAmount(this.treeTrunkParticles[j], CS$<>8__locals1.needLightUpTreeTrunk.Contains(j) ? -0.1f : 1f, 0.5f, true));
				}
				this._activeSkillTreeSequence.Play<Sequence>();
			}
			else
			{
				foreach (ParticleSystem ps in this.goldenVeinParticles)
				{
					this.SetPSDissolveAmount(ps, 1f, 0.5f, false);
				}
				foreach (ParticleSystem ps2 in this.treeTrunkParticles)
				{
					this.SetPSDissolveAmount(ps2, 1f, 0.5f, false);
				}
				this._activeSkillTreeSequence = DOTween.Sequence();
				this._activeSkillTreeSequence.AppendCallback(delegate
				{
					CS$<>8__locals1.<>4__this.SetDataToActiveSkillSlot(0, CS$<>8__locals1.activeSkillDataList[0], true);
				});
				for (int k = 0; k < needLightUpGoldenVein.Count; k++)
				{
					int grade = needLightUpGoldenVein[k];
					this._activeSkillTreeSequence.Append(this.SetPSDissolveAmount(this.goldenVeinParticles[grade], -0.1f, 0.5f, true));
					this._activeSkillTreeSequence.AppendCallback(delegate
					{
						CS$<>8__locals1.<>4__this.SetDataToActiveSkillSlot(grade, CS$<>8__locals1.activeSkillDataList[grade], true);
						for (int l = 0; l < CS$<>8__locals1.needLightUpTreeTrunk.Count; l++)
						{
							int treeTrunkIndex2 = CS$<>8__locals1.needLightUpTreeTrunk[l];
							List<int> list3;
							bool flag8 = !CharacterMenuAttainmentPanel.Slot2TreeTrunkCorrelation.TryGetValue(treeTrunkIndex2, out list3);
							if (!flag8)
							{
								int grade3 = grade;
								List<int> list4 = list3;
								bool flag9 = grade3 != list4[list4.Count - 1];
								if (!flag9)
								{
									CS$<>8__locals1.<>4__this._treeTrunkTweeners.Add(CS$<>8__locals1.<>4__this.SetPSDissolveAmount(CS$<>8__locals1.<>4__this.treeTrunkParticles[treeTrunkIndex2], -0.1f, 1f, true));
									CS$<>8__locals1.needLightUpTreeTrunk.Remove(treeTrunkIndex2);
									break;
								}
							}
						}
					});
				}
				this._activeSkillTreeSequence.Play<Sequence>();
			}
		}

		// Token: 0x06009029 RID: 36905 RVA: 0x00433298 File Offset: 0x00431498
		private Tweener SetPSDissolveAmount(ParticleSystem ps, float dissolveAmount = -0.1f, float duration = 0.5f, bool isAnim = false)
		{
			bool flag = ps == null;
			Tweener result;
			if (flag)
			{
				result = null;
			}
			else
			{
				Material mat = ps.GetComponent<ParticleSystemRenderer>().material;
				bool flag2 = !isAnim;
				if (flag2)
				{
					mat.SetFloat(CharacterMenuAttainmentPanel.DissolveProperty, dissolveAmount);
					result = null;
				}
				else
				{
					result = DOTween.To(() => mat.GetFloat(CharacterMenuAttainmentPanel.DissolveProperty), delegate(float x)
					{
						mat.SetFloat(CharacterMenuAttainmentPanel.DissolveProperty, x);
					}, dissolveAmount, duration);
				}
			}
			return result;
		}

		// Token: 0x0600902A RID: 36906 RVA: 0x0043330F File Offset: 0x0043150F
		public void SetIsTaiwuCharacter(bool isTaiwu)
		{
			this._isTaiwuCharacter = isTaiwu;
			this.RefreshPresetBarDisplay();
		}

		// Token: 0x0600902B RID: 36907 RVA: 0x00433320 File Offset: 0x00431520
		private void RefreshPresetBarDisplay()
		{
			bool flag = this.presetBar == null;
			if (!flag)
			{
				this.presetBar.gameObject.SetActive(this._isTaiwuCharacter);
			}
		}

		// Token: 0x0600902C RID: 36908 RVA: 0x00433357 File Offset: 0x00431557
		public void SetLifeSkillInfoArea(sbyte type, short v1, string v2)
		{
			this.infoArea.SetLifeSkillType(type);
			this.infoArea.SetQualification((int)v1);
			this.infoArea.SetAttainmentText(v2);
		}

		// Token: 0x0600902D RID: 36909 RVA: 0x00433381 File Offset: 0x00431581
		public void SetCombatSkillInfoArea(sbyte type, int v1, string v2)
		{
			this.infoArea.SetCombatSkillType(type);
			this.infoArea.SetQualification(v1);
			this.infoArea.SetAttainmentText(v2);
		}

		// Token: 0x0600902E RID: 36910 RVA: 0x004333AB File Offset: 0x004315AB
		public void SetHintText(string v)
		{
			this.hintText.text = v;
		}

		// Token: 0x0600902F RID: 36911 RVA: 0x004333BC File Offset: 0x004315BC
		public void SetExpandable(bool expandable)
		{
			this._expandable = expandable;
			bool flag = !this._expandable && this._expanded;
			if (flag)
			{
				this.SetExpand(false, true);
				this.btnExpand.gameObject.SetActive(false);
			}
			bool flag2 = !expandable;
			if (flag2)
			{
				this.btnExpand.gameObject.SetActive(false);
			}
			bool expandable2 = this._expandable;
			if (expandable2)
			{
				this.SetExpand(this._expanded, false);
			}
		}

		// Token: 0x06009030 RID: 36912 RVA: 0x0043343C File Offset: 0x0043163C
		public void SetAttainment(int index)
		{
			this.attainmentGroup.SetWithoutNotify(index);
			bool flag = this._scrollAttainmentCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this._scrollAttainmentCoroutine);
				this._scrollAttainmentCoroutine = null;
			}
			this._scrollAttainmentCoroutine = base.StartCoroutine(this.ScrollAttainmentTypeIntoViewDeferred(index));
		}

		// Token: 0x06009031 RID: 36913 RVA: 0x0043348D File Offset: 0x0043168D
		private IEnumerator ScrollAttainmentTypeIntoViewDeferred(int index)
		{
			yield return null;
			Canvas.ForceUpdateCanvases();
			this.ScrollAttainmentTypeIntoView(index);
			this._scrollAttainmentCoroutine = null;
			yield break;
		}

		// Token: 0x06009032 RID: 36914 RVA: 0x004334A4 File Offset: 0x004316A4
		private void ScrollAttainmentTypeIntoView(int index)
		{
			CToggle toggle = this.attainmentGroup.Get(index);
			bool flag = toggle == null;
			if (!flag)
			{
				CScrollRect scrollRect = this.attainmentGroup.GetComponentInParent<CScrollRect>();
				bool flag2 = scrollRect == null;
				if (!flag2)
				{
					RectTransform viewport = this.attainmentGroup.transform.parent as RectTransform;
					RectTransform content = scrollRect.Content;
					RectTransform itemRect = toggle.transform as RectTransform;
					bool flag3 = viewport == null || content == null || itemRect == null;
					if (!flag3)
					{
						LayoutRebuilder.ForceRebuildLayoutImmediate(content);
						float scrollMaxY = Mathf.Max(0f, content.rect.height - viewport.rect.height);
						bool flag4 = scrollMaxY <= 0f;
						if (!flag4)
						{
							float itemMinY;
							float itemMaxY;
							CharacterMenuAttainmentPanel.GetRectBoundsInViewportLocalSpace(itemRect, viewport, out itemMinY, out itemMaxY);
							float viewMinY = viewport.rect.min.y;
							float viewMaxY = viewport.rect.max.y;
							float deltaY = 0f;
							float viewHeight = viewMaxY - viewMinY;
							float itemHeight = itemMaxY - itemMinY;
							bool flag5 = itemHeight > viewHeight;
							if (flag5)
							{
								deltaY = viewMaxY - itemMaxY;
							}
							else
							{
								bool flag6 = itemMinY < viewMinY;
								if (flag6)
								{
									deltaY = viewMinY - itemMinY;
								}
								else
								{
									bool flag7 = itemMaxY > viewMaxY;
									if (flag7)
									{
										deltaY = viewMaxY - itemMaxY;
									}
								}
							}
							bool flag8 = Mathf.Abs(deltaY) < 0.5f;
							if (!flag8)
							{
								Vector2 pos = content.anchoredPosition;
								pos.y = Mathf.Clamp(pos.y + deltaY, 0f, scrollMaxY);
								scrollRect.ScrollTo(pos, 0f);
							}
						}
					}
				}
			}
		}

		// Token: 0x06009033 RID: 36915 RVA: 0x00433660 File Offset: 0x00431860
		private static void GetRectBoundsInViewportLocalSpace(RectTransform target, RectTransform viewport, out float minY, out float maxY)
		{
			Vector3[] corners = new Vector3[4];
			target.GetWorldCorners(corners);
			minY = float.MaxValue;
			maxY = float.MinValue;
			for (int i = 0; i < corners.Length; i++)
			{
				float localY = viewport.InverseTransformPoint(corners[i]).y;
				minY = Mathf.Min(minY, localY);
				maxY = Mathf.Max(maxY, localY);
			}
		}

		// Token: 0x06009034 RID: 36916 RVA: 0x004336C8 File Offset: 0x004318C8
		public void RefreshTitles()
		{
			int totalAmount = (this._panelType == CharacterMenuAttainmentPanel.EAttainmentPanelType.LifeSkill) ? 16 : 14;
			sbyte i = 0;
			while ((int)i < totalAmount)
			{
				bool flag = this._panelType == CharacterMenuAttainmentPanel.EAttainmentPanelType.LifeSkill;
				if (flag)
				{
					this.attainmentItems[(int)i].SetLifeSkillTypeTitle(i);
				}
				else
				{
					this.attainmentItems[(int)i].SetCombatSkillTypeTitle(i);
				}
				i += 1;
			}
		}

		// Token: 0x04006EB2 RID: 28338
		[SerializeField]
		private CToggleGroup attainmentGroup;

		// Token: 0x04006EB3 RID: 28339
		[SerializeField]
		private CToggleGroup activeSkillLayout;

		// Token: 0x04006EB4 RID: 28340
		[SerializeField]
		private CharacterAttainmentItem attainmentItemTemplate;

		// Token: 0x04006EB5 RID: 28341
		[SerializeField]
		private CToggleGroup presetBar;

		// Token: 0x04006EB6 RID: 28342
		[SerializeField]
		private CToggle presetToggleTemplate;

		// Token: 0x04006EB7 RID: 28343
		[SerializeField]
		private TextMeshProUGUI secondaryTxt;

		// Token: 0x04006EB8 RID: 28344
		[SerializeField]
		private GameObject rightPanel;

		// Token: 0x04006EB9 RID: 28345
		[SerializeField]
		public CharacterAttainmentItem infoArea;

		// Token: 0x04006EBA RID: 28346
		[SerializeField]
		private TextMeshProUGUI hintText;

		// Token: 0x04006EBB RID: 28347
		[SerializeField]
		private CButton btnExpand;

		// Token: 0x04006EBC RID: 28348
		[SerializeField]
		private CButton btnUnexpand;

		// Token: 0x04006EBD RID: 28349
		[SerializeField]
		private CRawImage[] bgImages;

		// Token: 0x04006EBE RID: 28350
		[SerializeField]
		private CRawImage[] roleImages;

		// Token: 0x04006EBF RID: 28351
		[SerializeField]
		private UIParticle[] paticles;

		// Token: 0x04006EC0 RID: 28352
		[Tooltip("调试：勾选后每次刷新都在已解锁孔位重播解锁动效，忽略播放缓存；未解锁孔位仍不播。")]
		[SerializeField]
		private bool debugAlwaysReplayUnlockEffects;

		// Token: 0x04006EC1 RID: 28353
		[Tooltip("再次进入时：关闭=直接显示已播 shu；开启=所有已解锁 shu/xishou 同步闪播，再同步播全部 xian。")]
		[SerializeField]
		private bool flashUnlockEffectOnRevisit;

		// Token: 0x04006EC2 RID: 28354
		private const int LifeSkillParticleIndex = 0;

		// Token: 0x04006EC3 RID: 28355
		private const int CombatSkillParticleIndex = 1;

		// Token: 0x04006EC4 RID: 28356
		private const string ParticleRoot01ObjectName = "particle01";

		// Token: 0x04006EC5 RID: 28357
		private const string ParticleRoot02ObjectName = "particle02";

		// Token: 0x04006EC6 RID: 28358
		private const float ShuUnlockParticlePlayDuration = 0.3f;

		// Token: 0x04006EC7 RID: 28359
		private const float OtherUnlockParticlePlayDuration = 0.1f;

		// Token: 0x04006EC8 RID: 28360
		private readonly List<string> _playedShuNodeNameBuffer = new List<string>();

		// Token: 0x04006EC9 RID: 28361
		private readonly List<int> _revisitFlashSlotBuffer = new List<int>();

		// Token: 0x04006ECA RID: 28362
		private readonly List<string> _revisitFlashNodeBuffer = new List<string>();

		// Token: 0x04006ECB RID: 28363
		private readonly Dictionary<string, ParticleSystem> _slotParticleCache = new Dictionary<string, ParticleSystem>();

		// Token: 0x04006ECC RID: 28364
		private Coroutine _unlockEffectCoroutine;

		// Token: 0x04006ECD RID: 28365
		private Coroutine _scrollAttainmentCoroutine;

		// Token: 0x04006ECE RID: 28366
		private int _currentSkillTypeIndex;

		// Token: 0x04006ECF RID: 28367
		private bool[] _currentSlotUnlocked;

		// Token: 0x04006ED0 RID: 28368
		private CharacterMenuAttainmentPanel.PendingUnlockEffectRequest? _pendingUnlockEffectRequest;

		// Token: 0x04006ED1 RID: 28369
		private const int LifeSkillBgIndex = 0;

		// Token: 0x04006ED2 RID: 28370
		private const int CombatSkillBgIndex = 1;

		// Token: 0x04006ED3 RID: 28371
		[SerializeField]
		private List<ParticleSystem> goldenVeinParticles;

		// Token: 0x04006ED4 RID: 28372
		[SerializeField]
		private List<ParticleSystem> treeTrunkParticles;

		// Token: 0x04006ED5 RID: 28373
		private bool _expandable = true;

		// Token: 0x04006ED6 RID: 28374
		private bool _isTaiwuCharacter = true;

		// Token: 0x04006ED7 RID: 28375
		private const int PRESET_SLOT = 5;

		// Token: 0x04006ED8 RID: 28376
		private const string PRESET_NAME_KEY_PREFIX = "LK_TraditionalChineseNumber_";

		// Token: 0x04006ED9 RID: 28377
		private const int ACTIVE_SKILL_SLOT = 9;

		// Token: 0x04006EDA RID: 28378
		private Action<int> _onPresetBarActiveIndexChange;

		// Token: 0x04006EDB RID: 28379
		private Action<int> _onAttainmentGroupChange;

		// Token: 0x04006EDC RID: 28380
		private Action<int> _onSkillActiveIndexChange;

		// Token: 0x04006EDD RID: 28381
		private bool _inited = false;

		// Token: 0x04006EDE RID: 28382
		private CharacterMenuAttainmentPanel.EAttainmentPanelType _panelType = CharacterMenuAttainmentPanel.EAttainmentPanelType.None;

		// Token: 0x04006EDF RID: 28383
		private List<CharacterAttainmentItem> attainmentItems = new List<CharacterAttainmentItem>();

		// Token: 0x04006EE0 RID: 28384
		private static readonly int DissolveProperty = Shader.PropertyToID("_rongjie");

		// Token: 0x04006EE1 RID: 28385
		private static readonly Dictionary<int, List<int>> Slot2GoldenVeinCorrelation = new Dictionary<int, List<int>>();

		// Token: 0x04006EE2 RID: 28386
		private static readonly Dictionary<int, List<int>> Slot2TreeTrunkCorrelation = new Dictionary<int, List<int>>
		{
			{
				0,
				new List<int>
				{
					0,
					1,
					2
				}
			},
			{
				1,
				new List<int>
				{
					3,
					4
				}
			},
			{
				2,
				new List<int>
				{
					5,
					6,
					7,
					8
				}
			}
		};

		// Token: 0x04006EE3 RID: 28387
		private Sequence _activeSkillTreeSequence;

		// Token: 0x04006EE4 RID: 28388
		private List<Tweener> _treeTrunkTweeners = new List<Tweener>();

		// Token: 0x0200216E RID: 8558
		private struct PendingUnlockEffectRequest
		{
			// Token: 0x0400D5B5 RID: 54709
			public List<CharacterMenuAttainmentActiveSkillData> ActiveSkillDataList;

			// Token: 0x0400D5B6 RID: 54710
			public bool[] SlotUnlocked;

			// Token: 0x0400D5B7 RID: 54711
			public List<int> PendingSlots;
		}

		// Token: 0x0200216F RID: 8559
		private enum EAttainmentPanelType
		{
			// Token: 0x0400D5B9 RID: 54713
			None,
			// Token: 0x0400D5BA RID: 54714
			CombatSkill,
			// Token: 0x0400D5BB RID: 54715
			LifeSkill
		}
	}
}
