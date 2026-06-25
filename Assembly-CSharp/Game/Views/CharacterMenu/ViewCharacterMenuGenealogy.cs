using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Config;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.Character;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.Relation;
using GameData.Domains.Character.Relation.RelationTree;
using GameData.Domains.Extra;
using GameData.Domains.Global;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000BA4 RID: 2980
	public class ViewCharacterMenuGenealogy : UI_CharacterMenuSubPageBase
	{
		// Token: 0x17001003 RID: 4099
		// (get) Token: 0x06009465 RID: 37989 RVA: 0x00452152 File Offset: 0x00450352
		public override LanguageKey TitleKey
		{
			get
			{
				return LanguageKey.LK_CharacterMenu_Title_RelationShip_Title;
			}
		}

		// Token: 0x06009466 RID: 37990 RVA: 0x0045215C File Offset: 0x0045035C
		public override bool CheckState(ECharacterSubToggleBase curSubTogglePage, ECharacterSubPage curSubPage)
		{
			return curSubTogglePage == ECharacterSubToggleBase.RelationshipBase && curSubPage == ECharacterSubPage.Genealogy;
		}

		// Token: 0x06009467 RID: 37991 RVA: 0x00452179 File Offset: 0x00450379
		private void OnEnable()
		{
			GlobalDomainMethod.Call.InvokeGuidingTrigger(86);
		}

		// Token: 0x06009468 RID: 37992 RVA: 0x00452184 File Offset: 0x00450384
		public override void OnInit(ArgumentBox argsBox)
		{
			this.scrollRect.content.GetComponent<MouseWheelScale>().GetComponent<PointerTrigger>().SetBindElement(this.Element);
		}

		// Token: 0x06009469 RID: 37993 RVA: 0x004521A8 File Offset: 0x004503A8
		public override void OnCurrentCharacterChange(int prevCharacterId)
		{
			int selfCharacterId = base.CharacterMenu.CurCharacterId;
			bool flag = selfCharacterId < 0;
			if (!flag)
			{
				base.StopAllCoroutines();
				this._refreshGeneration++;
				int refreshToken = this._refreshGeneration;
				int expectedCharId = selfCharacterId;
				this.localLoadingAnim.SetLoadingState(true);
				CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(this, new List<int>
				{
					selfCharacterId
				}, delegate(int offset, RawDataPool dataPool)
				{
					List<CharacterDisplayData> displays = null;
					Serializer.Deserialize(dataPool, offset, ref displays);
					CharacterDisplayData charData = displays[0];
					CharacterCircle characterCircle = this.charSelf.GetComponentInChildren<CharacterCircle>(true);
					bool flag3 = characterCircle != null;
					if (flag3)
					{
						characterCircle.Set(charData, this.CharacterMenu.IsTaiwu(charData.CharacterId), false);
					}
				});
				bool flag2 = this.IsCurrentSelectDreamBack();
				if (flag2)
				{
					ExtraDomainMethod.AsyncCall.GetDreamBackTaiwuGenealogy(this, delegate(int offset, RawDataPool dataPool)
					{
						bool flag3 = refreshToken != this._refreshGeneration || this.CharacterMenu.CurCharacterId != expectedCharId;
						if (!flag3)
						{
							this.StartCoroutine(this.Prepare(offset, dataPool, refreshToken));
						}
					});
				}
				else
				{
					CharacterDomainMethod.AsyncCall.GetGenealogy(this, selfCharacterId, delegate(int offset, RawDataPool dataPool)
					{
						bool flag3 = refreshToken != this._refreshGeneration || this.CharacterMenu.CurCharacterId != expectedCharId;
						if (!flag3)
						{
							this.StartCoroutine(this.Prepare(offset, dataPool, refreshToken));
						}
					});
				}
			}
		}

		// Token: 0x0600946A RID: 37994 RVA: 0x00452268 File Offset: 0x00450468
		private void Awake()
		{
			Transform switchRoot = this.groupComponentTemplate.parent;
			for (ViewCharacterMenuGenealogy.EGeneration generation = ViewCharacterMenuGenealogy.EGeneration.GrandParents; generation < ViewCharacterMenuGenealogy.EGeneration.Count; generation++)
			{
				int currentIndex = (int)generation;
				Transform child = (currentIndex >= switchRoot.childCount) ? null : switchRoot.GetChild(currentIndex);
				bool flag = child == null;
				if (flag)
				{
					child = Object.Instantiate<GameObject>(this.groupComponentTemplate.gameObject, switchRoot).transform;
					child.SetSiblingIndex(currentIndex);
				}
				CButton button = child.GetComponent<CButton>();
				button.ClearAndAddListener(delegate
				{
					this.SwitchGenealogyFocusGeneration((ViewCharacterMenuGenealogy.EGeneration)currentIndex);
				});
				foreach (TextMeshProUGUI label in button.GetComponent<CButton>().GetComponentsInChildren<TextMeshProUGUI>(true))
				{
					label.text = LocalStringManager.Get("LK_RelationShipGeneration_" + generation.ToString());
				}
			}
			PoolManager.SetSrcObjectWithTurnOff("ViewCharacterMenuGenealogycharTemplate", this.charTemplate);
			this.localLoadingAnim.SetLoadingState(true);
		}

		// Token: 0x0600946B RID: 37995 RVA: 0x00452389 File Offset: 0x00450589
		private void OnDestroy()
		{
			PoolManager.RemoveData("ViewCharacterMenuGenealogycharTemplate");
		}

		// Token: 0x0600946C RID: 37996 RVA: 0x00452398 File Offset: 0x00450598
		private void FixedUpdate()
		{
			this._autoUpdateTime -= Time.deltaTime;
			bool flag = this._autoUpdateTime < 0f;
			if (flag)
			{
				this.OnSetGenealogyScrollOffset(Vector2.zero);
				this._autoUpdateTime = 0.5f;
			}
		}

		// Token: 0x0600946D RID: 37997 RVA: 0x004523E4 File Offset: 0x004505E4
		private RectTransform GetGenerationGroup(ViewCharacterMenuGenealogy.EGeneration generation)
		{
			return this.scrollRect.content.GetChild(generation - ViewCharacterMenuGenealogy.EGeneration.GrandParents + 2).transform as RectTransform;
		}

		// Token: 0x0600946E RID: 37998 RVA: 0x00452418 File Offset: 0x00450618
		private static void SetCanvasGroupVisible(GameObject go, bool visible)
		{
			go.SetActive(true);
			CanvasGroup canvasGroup = go.GetComponent<CanvasGroup>();
			bool flag = canvasGroup == null;
			if (flag)
			{
				canvasGroup = go.AddComponent<CanvasGroup>();
			}
			canvasGroup.alpha = (visible ? 1f : 0f);
			canvasGroup.interactable = visible;
			canvasGroup.blocksRaycasts = visible;
		}

		// Token: 0x0600946F RID: 37999 RVA: 0x0045246D File Offset: 0x0045066D
		private void SetLinkLayersVisible(bool visible)
		{
			ViewCharacterMenuGenealogy.SetCanvasGroupVisible(this.linkBaseRoot.gameObject, visible);
			ViewCharacterMenuGenealogy.SetCanvasGroupVisible(this.linkBaseRootHighlight.gameObject, visible);
		}

		// Token: 0x06009470 RID: 38000 RVA: 0x00452494 File Offset: 0x00450694
		private void SwitchGenealogyFocusGeneration(ViewCharacterMenuGenealogy.EGeneration generation)
		{
			base.StartCoroutine(this.FocusGenerationRoutine(generation));
		}

		// Token: 0x06009471 RID: 38001 RVA: 0x004524A5 File Offset: 0x004506A5
		private IEnumerator FocusGenerationRoutine(ViewCharacterMenuGenealogy.EGeneration generation)
		{
			RectTransform contentRoot = this.scrollRect.content;
			RectTransform groupCore = this.GetGenerationGroup(ViewCharacterMenuGenealogy.EGeneration.Self);
			RectTransform group = this.GetGenerationGroup(generation);
			RectTransform tr = contentRoot.GetComponent<RectTransform>();
			contentRoot.DOKill(false);
			tr.SetPivot(new Vector2(0.5f, 0.5f));
			Vector3 composeScale = contentRoot.localScale;
			float centerMemberX = (group.childCount > 0) ? (-group.GetChild(group.childCount / 2).GetComponent<RectTransform>().anchoredPosition.x) : (tr.anchoredPosition.x / composeScale.x);
			float groupCoreY = groupCore.anchoredPosition.y - group.anchoredPosition.y;
			yield return tr.DOAnchorPos(new Vector2(centerMemberX * composeScale.x, groupCoreY * composeScale.y), this.focusCameraDuration, false).WaitForCompletion();
			this._currentGenealogyGradeIndex = (int)generation;
			yield return null;
			yield break;
		}

		// Token: 0x06009472 RID: 38002 RVA: 0x004524BC File Offset: 0x004506BC
		private bool IsCurrentSelectDreamBack()
		{
			return this.IsDreamBack && base.CharacterMenu.CurrentCharacterIsTaiwu;
		}

		// Token: 0x17001004 RID: 4100
		// (get) Token: 0x06009473 RID: 38003 RVA: 0x004524E4 File Offset: 0x004506E4
		private bool IsDreamBack
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06009474 RID: 38004 RVA: 0x004524E7 File Offset: 0x004506E7
		private IEnumerator Prepare(int offset, RawDataPool dataPool, int refreshToken)
		{
			bool flag = refreshToken != this._refreshGeneration;
			if (flag)
			{
				yield break;
			}
			Genealogy genealogy = new Genealogy();
			Serializer.Deserialize(dataPool, offset, ref genealogy);
			List<int> genealogyCharacterIds = new List<int>();
			Dictionary<ViewCharacterMenuGenealogy.EGeneration, List<GenealogyMaker.GenealogyNode>> genealogyNodes = genealogy.MakeNodes();
			foreach (List<GenealogyMaker.GenealogyNode> generation in genealogyNodes.Values)
			{
				genealogyCharacterIds.AddRange(from n in generation
				select n.CharacterId);
				yield return new WaitForEndOfFrame();
				generation = null;
			}
			Dictionary<ViewCharacterMenuGenealogy.EGeneration, List<GenealogyMaker.GenealogyNode>>.ValueCollection.Enumerator enumerator = default(Dictionary<ViewCharacterMenuGenealogy.EGeneration, List<GenealogyMaker.GenealogyNode>>.ValueCollection.Enumerator);
			bool flag2 = refreshToken != this._refreshGeneration;
			if (flag2)
			{
				yield break;
			}
			genealogyCharacterIds.RemoveAll((int id) => id < 0);
			Dictionary<int, CharacterDisplayDataForRelations> genealogyChars = new Dictionary<int, CharacterDisplayDataForRelations>();
			bool flag3 = this.IsCurrentSelectDreamBack();
			if (flag3)
			{
				ExtraDomainMethod.AsyncCall.GetCharacterDisplayDataListForDreamBackRelations(this, genealogyCharacterIds, delegate(int offset2, RawDataPool dataPool2)
				{
					bool flag4 = refreshToken != this._refreshGeneration;
					if (!flag4)
					{
						List<CharacterDisplayDataForRelations> ret = null;
						Serializer.Deserialize(dataPool2, offset2, ref ret);
						foreach (CharacterDisplayDataForRelations data in ret)
						{
							genealogyChars[data.Main.CharacterId] = data;
						}
						this.Refresh(genealogyChars, genealogyNodes, refreshToken);
					}
				});
			}
			else
			{
				CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataListForRelationsWithRelationType(this, base.CharacterMenu.CurCharacterId, genealogyCharacterIds, delegate(int offset2, RawDataPool dataPool2)
				{
					bool flag4 = refreshToken != this._refreshGeneration;
					if (!flag4)
					{
						List<CharacterDisplayDataForRelations> ret = null;
						Serializer.Deserialize(dataPool2, offset2, ref ret);
						foreach (CharacterDisplayDataForRelations data in ret)
						{
							genealogyChars[data.Main.CharacterId] = data;
						}
						this.Refresh(genealogyChars, genealogyNodes, refreshToken);
					}
				});
			}
			yield break;
			yield break;
		}

		// Token: 0x06009475 RID: 38005 RVA: 0x0045250B File Offset: 0x0045070B
		private IEnumerator RefreshProcess(IDictionary<int, CharacterDisplayDataForRelations> genealogyChars, IDictionary<ViewCharacterMenuGenealogy.EGeneration, List<GenealogyMaker.GenealogyNode>> genealogyNodes, int refreshToken)
		{
			bool flag = refreshToken != this._refreshGeneration;
			if (flag)
			{
				yield break;
			}
			RectTransform contentRoot = this.scrollRect.content;
			this.localLoadingAnim.SetLoadingState(true);
			yield return base.StartCoroutine(this.MakeGenealogy(delegate(RectTransform n)
			{
			}, genealogyChars, genealogyNodes));
			bool flag2 = refreshToken != this._refreshGeneration;
			if (flag2)
			{
				yield break;
			}
			yield return new WaitForEndOfFrame();
			LayoutRebuilder.ForceRebuildLayoutImmediate(contentRoot);
			yield return new WaitForEndOfFrame();
			MouseWheelScale wheelScale = contentRoot.GetComponent<MouseWheelScale>();
			yield return new WaitForEndOfFrame();
			wheelScale.Reset();
			wheelScale.ScaleProcess(0f);
			yield return new WaitForEndOfFrame();
			wheelScale = null;
			this.localLoadingAnim.EnsureContentActiveForLayout();
			this.SetLinkLayersVisible(false);
			yield return new WaitForEndOfFrame();
			Canvas.ForceUpdateCanvases();
			yield return this.FocusGenerationRoutine(ViewCharacterMenuGenealogy.EGeneration.Self);
			bool flag3 = refreshToken != this._refreshGeneration;
			if (flag3)
			{
				yield break;
			}
			yield return base.StartCoroutine(this.BuildRelationLinks(refreshToken));
			bool flag4 = refreshToken != this._refreshGeneration;
			if (flag4)
			{
				yield break;
			}
			this.localLoadingAnim.SetLoadingState(false);
			this.SetLinkLayersVisible(true);
			yield break;
		}

		// Token: 0x06009476 RID: 38006 RVA: 0x0045252F File Offset: 0x0045072F
		private IEnumerator MakeGenealogy(Action<RectTransform> afterMake, IDictionary<int, CharacterDisplayDataForRelations> genealogyChars, IDictionary<ViewCharacterMenuGenealogy.EGeneration, List<GenealogyMaker.GenealogyNode>> genealogyNodes)
		{
			foreach (Transform child in this.linkBaseRoot.transform.Cast<Transform>().ToArray<Transform>())
			{
				Object.Destroy(child.gameObject);
				child = null;
			}
			Transform[] array = null;
			IEnumerator prepareRoutine = GenealogyMaker.SquaredAway(genealogyNodes);
			while (prepareRoutine.MoveNext())
			{
				object obj = prepareRoutine.Current;
				yield return obj;
			}
			prepareRoutine = null;
			int count = (from pair in genealogyNodes
			where pair.Value != null
			select pair).Sum((KeyValuePair<ViewCharacterMenuGenealogy.EGeneration, List<GenealogyMaker.GenealogyNode>> pair) => pair.Value.Count);
			Stopwatch watch = new Stopwatch();
			int splitInterval = 1000 / ((count > 100) ? 30 : 60);
			Dictionary<GenealogyMaker.GenealogyNode, GenealogyCharacterPanel> map = new Dictionary<GenealogyMaker.GenealogyNode, GenealogyCharacterPanel>();
			float xMin = 0f;
			float xMax = 0f;
			RectTransform selfNode = null;
			watch.Start();
			ViewCharacterMenuGenealogy.EGeneration egeneration;
			for (ViewCharacterMenuGenealogy.EGeneration generation = ViewCharacterMenuGenealogy.EGeneration.GrandParents; generation < ViewCharacterMenuGenealogy.EGeneration.Count; generation = egeneration + 1)
			{
				RectTransform group = this.GetGenerationGroup(generation);
				CImage back = group.GetComponent<CImage>();
				List<Transform> destroyList = group.Cast<Transform>().ToList<Transform>();
				foreach (Transform item2 in from item in destroyList
				where item.gameObject != this.charSelf
				select item)
				{
					PoolManager.Destroy("ViewCharacterMenuGenealogycharTemplate", item2.gameObject);
					item2 = null;
				}
				IEnumerator<Transform> enumerator = null;
				destroyList = null;
				back.enabled = false;
				List<GenealogyMaker.GenealogyNode> list;
				bool flag = !genealogyNodes.TryGetValue(generation, out list);
				if (!flag)
				{
					watch.Restart();
					int num2;
					for (int idx = list.Count - 1; idx >= 0; idx = num2 - 1)
					{
						ViewCharacterMenuGenealogy.<>c__DisplayClass36_0 CS$<>8__locals1 = new ViewCharacterMenuGenealogy.<>c__DisplayClass36_0();
						CS$<>8__locals1.<>4__this = this;
						GenealogyMaker.GenealogyNode node = list[idx];
						bool flag2 = !genealogyChars.TryGetValue(node.CharacterId, out CS$<>8__locals1.charData);
						if (!flag2)
						{
							bool isSelf = generation == ViewCharacterMenuGenealogy.EGeneration.Self;
							GameObject charGenealogyObject = isSelf ? this.charSelf : PoolManager.GetObject("ViewCharacterMenuGenealogycharTemplate");
							charGenealogyObject.SetActive(true);
							RectTransform charInfoRect = charGenealogyObject.GetComponent<RectTransform>();
							charInfoRect.SetParent(group, false);
							GenealogyCharacterPanel charPanel = charGenealogyObject.GetComponent<GenealogyCharacterPanel>();
							charPanel.CharacterId = node.CharacterId;
							charPanel.NodeData = node;
							map.Add(node, charPanel);
							CS$<>8__locals1.fullName = NameCenter.GetMonasticTitleOrDisplayName(ref CS$<>8__locals1.charData.Main.NameData, base.CharacterMenu.IsTaiwu(CS$<>8__locals1.charData.Main.CharacterId), false);
							bool flag3 = charPanel == null;
							if (!flag3)
							{
								charPanel.Name = CS$<>8__locals1.fullName;
								bool flag4 = isSelf;
								if (flag4)
								{
									selfNode = charInfoRect;
								}
								bool flag5 = CS$<>8__locals1.charData.Main.CharacterTemplateId == 880;
								if (flag5)
								{
									charPanel.SetBirthAndDeathDate(string.Empty);
								}
								else
								{
									charPanel.SetBirthAndDeathDate(CS$<>8__locals1.charData.Main.BirthDate, CS$<>8__locals1.charData.DeathDate);
								}
								GenealogyCharacterPanel genealogyCharacterPanel = charPanel;
								if (!true)
								{
								}
								GenealogyCharacterPanel.RelationType type;
								switch (generation)
								{
								case ViewCharacterMenuGenealogy.EGeneration.GrandParents:
									type = GenealogyCharacterPanel.RelationType.Blood;
									break;
								case ViewCharacterMenuGenealogy.EGeneration.Parents:
								{
									ushort relationType = node.RelationType;
									if (!true)
									{
									}
									ushort num = relationType;
									GenealogyCharacterPanel.RelationType relationType2;
									if (num != 1)
									{
										if (num != 8)
										{
											if (num != 64)
											{
												relationType2 = GenealogyCharacterPanel.RelationType.Unknown;
											}
											else
											{
												relationType2 = GenealogyCharacterPanel.RelationType.Adoptive;
											}
										}
										else
										{
											relationType2 = GenealogyCharacterPanel.RelationType.Step;
										}
									}
									else
									{
										relationType2 = GenealogyCharacterPanel.RelationType.Blood;
									}
									if (!true)
									{
									}
									type = relationType2;
									break;
								}
								case ViewCharacterMenuGenealogy.EGeneration.Siblings:
								{
									ushort relationType3 = node.RelationType;
									if (!true)
									{
									}
									ushort num = relationType3;
									GenealogyCharacterPanel.RelationType relationType2;
									if (num != 4)
									{
										if (num != 32)
										{
											if (num != 256)
											{
												relationType2 = GenealogyCharacterPanel.RelationType.Unknown;
											}
											else
											{
												relationType2 = GenealogyCharacterPanel.RelationType.Adoptive;
											}
										}
										else
										{
											relationType2 = GenealogyCharacterPanel.RelationType.Step;
										}
									}
									else
									{
										relationType2 = GenealogyCharacterPanel.RelationType.Blood;
									}
									if (!true)
									{
									}
									type = relationType2;
									break;
								}
								case ViewCharacterMenuGenealogy.EGeneration.Self:
									goto IL_66E;
								case ViewCharacterMenuGenealogy.EGeneration.Spouses:
									type = GenealogyCharacterPanel.RelationType.Spouse;
									break;
								case ViewCharacterMenuGenealogy.EGeneration.Children:
								case ViewCharacterMenuGenealogy.EGeneration.GrandChildren:
								{
									ushort relationType4 = node.RelationType;
									if (!true)
									{
									}
									ushort num = relationType4;
									GenealogyCharacterPanel.RelationType relationType2;
									if (num != 2)
									{
										if (num != 16)
										{
											if (num != 128)
											{
												relationType2 = GenealogyCharacterPanel.RelationType.Unknown;
											}
											else
											{
												relationType2 = GenealogyCharacterPanel.RelationType.Adoptive;
											}
										}
										else
										{
											relationType2 = GenealogyCharacterPanel.RelationType.Step;
										}
									}
									else
									{
										relationType2 = GenealogyCharacterPanel.RelationType.Blood;
									}
									if (!true)
									{
									}
									type = relationType2;
									break;
								}
								default:
									goto IL_66E;
								}
								IL_673:
								if (!true)
								{
								}
								genealogyCharacterPanel.SetRelation(type, node.RelationType);
								genealogyCharacterPanel = null;
								TooltipInvoker tips = charGenealogyObject.GetComponent<TooltipInvoker>();
								bool flag6 = tips != null;
								if (flag6)
								{
									tips.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("charId", CS$<>8__locals1.charData.Main.CharacterId).Set("locationShow", true).Set("isDreamBack", this.IsCurrentSelectDreamBack());
								}
								CS$<>8__locals1.charButton = charGenealogyObject.GetComponent<CButton>();
								bool flag7 = CS$<>8__locals1.charButton != null;
								if (flag7)
								{
									ViewCharacterMenuGenealogy.<>c__DisplayClass36_1 CS$<>8__locals2 = new ViewCharacterMenuGenealogy.<>c__DisplayClass36_1();
									CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
									CS$<>8__locals2.CS$<>8__locals1.charButton.interactable = true;
									CS$<>8__locals2.generationForCallback = generation;
									CS$<>8__locals2.CS$<>8__locals1.charButton.ClearAndAddListener(delegate
									{
										bool flag12 = ViewCharacterMenuGenealogy.CanJumpCharacter(CS$<>8__locals2.CS$<>8__locals1.<>4__this.CharacterMenu, CS$<>8__locals2.CS$<>8__locals1.charData, CS$<>8__locals2.CS$<>8__locals1.<>4__this.IsCurrentSelectDreamBack());
										if (flag12)
										{
											CS$<>8__locals2.CS$<>8__locals1.charButton.interactable = false;
											bool flag13 = CS$<>8__locals2.CS$<>8__locals1.charData.LifeState == 1 && CS$<>8__locals2.CS$<>8__locals1.charData.Location.IsValid();
											if (flag13)
											{
												ViewLifeRecords.Show(CS$<>8__locals2.CS$<>8__locals1.charData.CharacterId);
											}
											else
											{
												ViewCharacterMenuGenealogy.JumpCharacterCallback(CS$<>8__locals2.CS$<>8__locals1.<>4__this.CharacterMenu, CS$<>8__locals2.CS$<>8__locals1.charData, CS$<>8__locals2.CS$<>8__locals1.fullName, LocalStringManager.Get("LK_RelationShipGeneration_" + CS$<>8__locals2.generationForCallback.ToString()));
											}
										}
									});
									CS$<>8__locals2 = null;
								}
								bool flag8;
								ViewCharacterMenuGenealogy.UpdateAvatar(CS$<>8__locals1.charData, charPanel.Avatar, out flag8);
								bool flag9 = watch.ElapsedMilliseconds > (long)splitInterval;
								if (flag9)
								{
									yield return new WaitForEndOfFrame();
									watch.Restart();
								}
								int pos = node.GetOffsetFromRoot();
								charInfoRect.name = string.Format("pos: {0}", pos);
								charInfoRect.localScale = Vector3.one;
								charInfoRect.anchoredPosition = new Vector3((this.charTemplate.GetComponent<RectTransform>().rect.width + 112f) * (float)pos, 0f);
								bool flag10 = charInfoRect.anchoredPosition.x < xMin;
								if (flag10)
								{
									xMin = charInfoRect.anchoredPosition.x;
								}
								else
								{
									bool flag11 = charInfoRect.anchoredPosition.x > xMax;
									if (flag11)
									{
										xMax = charInfoRect.anchoredPosition.x;
									}
								}
								CS$<>8__locals1 = null;
								node = null;
								charGenealogyObject = null;
								charInfoRect = null;
								charPanel = null;
								tips = null;
								goto IL_927;
								IL_66E:
								type = GenealogyCharacterPanel.RelationType.Unknown;
								goto IL_673;
							}
							AdaptableLog.TagWarning("MakeGenealogy", string.Format("{0}({1}) failed", CS$<>8__locals1.fullName, CS$<>8__locals1.charData.Main.CharacterId), false);
						}
						IL_927:
						num2 = idx;
					}
					group = null;
					back = null;
					list = null;
				}
				egeneration = generation;
			}
			float layoutWidth = Math.Max(Math.Abs(xMin), Math.Abs(xMax)) * 2f + this.scrollRect.viewport.rect.width;
			for (ViewCharacterMenuGenealogy.EGeneration generation2 = ViewCharacterMenuGenealogy.EGeneration.GrandParents; generation2 < ViewCharacterMenuGenealogy.EGeneration.Count; generation2 = egeneration + 1)
			{
				RectTransform group2 = this.GetGenerationGroup(generation2);
				group2.sizeDelta = group2.sizeDelta.SetX(layoutWidth);
				group2 = null;
				egeneration = generation2;
			}
			float contentWidth = layoutWidth + (float)this.genealogyContentLayoutGroup.padding.left + (float)this.genealogyContentLayoutGroup.padding.right;
			this.genealogyContentLayoutGroup.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, contentWidth);
			this._genealogyPanelMap = map;
			if (afterMake != null)
			{
				afterMake(selfNode);
			}
			yield break;
		}

		// Token: 0x06009477 RID: 38007 RVA: 0x00452553 File Offset: 0x00450753
		private IEnumerator BuildRelationLinks(int refreshToken)
		{
			bool flag = refreshToken != this._refreshGeneration;
			if (flag)
			{
				yield break;
			}
			Dictionary<GenealogyMaker.GenealogyNode, GenealogyCharacterPanel> map = this._genealogyPanelMap;
			bool flag2 = map == null || map.Count == 0;
			if (flag2)
			{
				yield break;
			}
			foreach (Transform child in this.linkBaseRoot.transform.Cast<Transform>().ToArray<Transform>())
			{
				Object.Destroy(child.gameObject);
				child = null;
			}
			Transform[] array = null;
			CommonUtils.PrepareEnoughChildren(this.linkBaseRootHighlight, this.templatedContainerAssembly.gameObject, map.Count, null);
			List<GenealogyCharacterPanel.RelationLinking> links = new List<GenealogyCharacterPanel.RelationLinking>();
			int childIndex = 0;
			foreach (KeyValuePair<GenealogyMaker.GenealogyNode, GenealogyCharacterPanel> pair in map)
			{
				bool flag3 = refreshToken != this._refreshGeneration;
				if (flag3)
				{
					yield break;
				}
				yield return new WaitForEndOfFrame();
				links.Clear();
				foreach (GenealogyMaker.GenealogyNode.GenealogyLink dependency in pair.Key.Dependencies)
				{
					GenealogyCharacterPanel target;
					bool flag4 = !map.TryGetValue(dependency.Node, out target);
					if (!flag4)
					{
						ushort type2 = dependency.Type;
						if (!true)
						{
						}
						ushort num = type2;
						GenealogyCharacterPanel.RelationType relationType;
						if (num <= 16)
						{
							if (num <= 4)
							{
								if (num - 1 > 1 && num != 4)
								{
									goto IL_2B0;
								}
								relationType = GenealogyCharacterPanel.RelationType.Blood;
							}
							else
							{
								if (num != 8 && num != 16)
								{
									goto IL_2B0;
								}
								goto IL_2A1;
							}
						}
						else
						{
							if (num <= 64)
							{
								if (num == 32)
								{
									goto IL_2A1;
								}
								if (num != 64)
								{
									goto IL_2B0;
								}
							}
							else if (num != 128 && num != 256)
							{
								if (num != 1024)
								{
									goto IL_2B0;
								}
								relationType = GenealogyCharacterPanel.RelationType.Spouse;
								goto IL_2B5;
							}
							relationType = GenealogyCharacterPanel.RelationType.Adoptive;
						}
						IL_2B5:
						if (!true)
						{
						}
						GenealogyCharacterPanel.RelationType type = relationType;
						bool flag5 = type == GenealogyCharacterPanel.RelationType.Unknown;
						if (flag5)
						{
							continue;
						}
						links.Add(new GenealogyCharacterPanel.RelationLinking(type, target, false, RelationType.ContainBrotherOrSisterRelations(dependency.Type)));
						target = null;
						dependency = default(GenealogyMaker.GenealogyNode.GenealogyLink);
						continue;
						IL_2A1:
						relationType = GenealogyCharacterPanel.RelationType.Step;
						goto IL_2B5;
						IL_2B0:
						relationType = GenealogyCharacterPanel.RelationType.Unknown;
						goto IL_2B5;
					}
				}
				List<GenealogyMaker.GenealogyNode.GenealogyLink>.Enumerator enumerator2 = default(List<GenealogyMaker.GenealogyNode.GenealogyLink>.Enumerator);
				this.TestPrintAllLinks(pair.Value, links);
				GenealogyCharacterPanel nodeCompToMain = null;
				bool flag6 = pair.Key.NodeToMain != null && pair.Key.CharacterId != base.CharacterMenu.CurCharacterId;
				if (flag6)
				{
					nodeCompToMain = map[pair.Key.NodeToMain];
				}
				bool flag7 = pair.Key.NodeToMain == null;
				if (flag7)
				{
					Debug.Log(pair.Value.TestName + " 没有高亮节点；" + string.Format("RelationType:{0};Generation:{1} depAmount:{2}", pair.Key.RelationType, pair.Key.Generation, pair.Key.Dependencies.Count));
				}
				pair.Value.SetRelationLinking(links, this.linkBaseRoot, this.linkBaseRootHighlight.GetChild(childIndex).GetComponent<TemplatedContainerAssembly>(), base.CharacterMenu.CurCharacterId, pair.Key.Generation, nodeCompToMain);
				int num2 = childIndex;
				childIndex = num2 + 1;
				nodeCompToMain = null;
				pair = default(KeyValuePair<GenealogyMaker.GenealogyNode, GenealogyCharacterPanel>);
			}
			Dictionary<GenealogyMaker.GenealogyNode, GenealogyCharacterPanel>.Enumerator enumerator = default(Dictionary<GenealogyMaker.GenealogyNode, GenealogyCharacterPanel>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06009478 RID: 38008 RVA: 0x0045256C File Offset: 0x0045076C
		private void TestPrintAllLinks(GenealogyCharacterPanel value, List<GenealogyCharacterPanel.RelationLinking> links)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(value.TestName);
			foreach (GenealogyCharacterPanel.RelationLinking item in links)
			{
				sb.AppendLine(item.Target.TestName);
			}
			Debug.Log("test AllLink:" + sb.ToString());
		}

		// Token: 0x06009479 RID: 38009 RVA: 0x004525F4 File Offset: 0x004507F4
		private void OnSetGenealogyScrollOffset(Vector2 _)
		{
			RectTransform switchRoot = this.groupComponentTemplate.parent.GetComponent<RectTransform>();
			float containerHeight = switchRoot.rect.height;
			RectTransform contentRoot = this.scrollRect.content;
			int i = 0;
			int len = switchRoot.childCount;
			while (i < len)
			{
				Transform grade = switchRoot.GetChild(i);
				RectTransform gradeTransform = (RectTransform)grade.transform;
				float scaledGroupHeight = gradeTransform.rect.height * contentRoot.localScale.x;
				gradeTransform.position = gradeTransform.position.SetY(this.GetGenerationGroup((ViewCharacterMenuGenealogy.EGeneration)i).position.y - 3f * gradeTransform.lossyScale.y);
				Rect rect = gradeTransform.rect;
				gradeTransform.anchoredPosition = new Vector2(0f, Mathf.Clamp(gradeTransform.anchoredPosition.y + scaledGroupHeight * 0.5f, -rect.height * 0.5f - containerHeight + (rect.height + 8f) * (float)(len - i), -rect.height * 0.5f - (rect.height + 8f) * (float)i));
				bool flag = Mathf.Abs(gradeTransform.anchoredPosition.y - -containerHeight * 0.5f) < scaledGroupHeight;
				if (flag)
				{
					this._currentGenealogyGradeIndex = i;
				}
				CButton button = grade.GetComponent<CButton>();
				PointerTrigger pointerTrigger = grade.GetComponent<PointerTrigger>();
				GameObject cursor = grade.Find("Hover").gameObject;
				bool current = i == this._currentGenealogyGradeIndex;
				bool flag2 = cursor.activeSelf != current;
				if (flag2)
				{
					bool activeSelf = cursor.activeSelf;
					if (activeSelf)
					{
						button.OnPointerExit(new PointerEventData(EventSystem.current));
					}
				}
				cursor.SetActive(current);
				pointerTrigger.enabled = !current;
				i++;
			}
		}

		// Token: 0x0600947A RID: 38010 RVA: 0x004527EC File Offset: 0x004509EC
		private void Refresh(IDictionary<int, CharacterDisplayDataForRelations> genealogyChars, IDictionary<ViewCharacterMenuGenealogy.EGeneration, List<GenealogyMaker.GenealogyNode>> genealogyNodes, int refreshToken)
		{
			bool flag = refreshToken != this._refreshGeneration;
			if (!flag)
			{
				this.scrollRect.onValueChanged.AddListener(new UnityAction<Vector2>(this.OnSetGenealogyScrollOffset));
				base.StartCoroutine(this.RefreshProcess(genealogyChars, genealogyNodes, refreshToken));
				this.Element.ShowAfterRefresh();
				RectTransform content = this.scrollRect.content;
				bool flag2 = content != null;
				if (flag2)
				{
					Vector2 targetRect = ((RectTransform)content.parent).rect.size;
					Vector3 localScale = content.localScale;
					content.sizeDelta = new Vector2(Mathf.Max(targetRect.x / localScale.x, this._contentRange.width), Mathf.Max(targetRect.y / localScale.y, this._contentRange.height));
					content.localScale = Vector3.one;
					content.localPosition = Vector3.zero;
				}
				this.scrollRect.normalizedPosition = Vector2.one * 0.5f;
			}
		}

		// Token: 0x0600947B RID: 38011 RVA: 0x004528FE File Offset: 0x00450AFE
		internal static bool CanJumpCharacter(ViewCharacterMenu characterMenu, CharacterDisplayDataForRelations charData, bool isDreamBack)
		{
			return ViewCharacterMenuGenealogy.CanLookIn(characterMenu.CurCharacterId, charData) && !isDreamBack;
		}

		// Token: 0x0600947C RID: 38012 RVA: 0x00452918 File Offset: 0x00450B18
		private static bool CanLookIn(int selfCharacterId, CharacterDisplayDataForRelations charData)
		{
			CharacterItem config = Character.Instance.GetItem(charData.Main.CharacterTemplateId);
			bool flag = config != null && !config.CanOpenCharacterMenu;
			return !flag && ((charData.LifeState == 0 && charData.Main.CharacterId != selfCharacterId) || (charData.LifeState == 1 && charData.Location.IsValid()));
		}

		// Token: 0x0600947D RID: 38013 RVA: 0x00452988 File Offset: 0x00450B88
		internal static void JumpCharacterCallback(ViewCharacterMenu characterMenu, CharacterDisplayDataForRelations charData, string fullName, string relationText = null)
		{
			characterMenu.StackView.PushAndActivate(characterMenu, charData.Main.CharacterId, fullName, delegate(Game.Components.Avatar.Avatar avatar)
			{
				CommonUtils.CheckForAvatarExtraInfo(charData.Main.CharacterId, charData.Main.AvatarRelatedData.AvatarData, ref charData.Main.AvatarRelatedData.ClothingDisplayId);
				avatar.Refresh(charData.Main.AvatarRelatedData, charData.Main.CharacterTemplateId);
			}, relationText);
		}

		// Token: 0x0600947E RID: 38014 RVA: 0x004529D0 File Offset: 0x00450BD0
		internal static void UpdateAvatar(CharacterDisplayDataForRelations charData, Game.Components.Avatar.Avatar avatar, out bool isDead)
		{
			isDead = false;
			sbyte lifeState = charData.LifeState;
			sbyte b = lifeState;
			if (b - 1 > 1)
			{
				bool flag = avatar != null;
				if (flag)
				{
					avatar.gameObject.SetActive(true);
					CommonUtils.CheckForAvatarExtraInfo(charData.Main.CharacterId, charData.Main.AvatarRelatedData.AvatarData, ref charData.Main.AvatarRelatedData.ClothingDisplayId);
					avatar.Refresh(charData.Main.AvatarRelatedData, charData.Main.CharacterTemplateId);
				}
			}
			else
			{
				bool flag2 = avatar != null;
				if (flag2)
				{
					avatar.gameObject.SetActive(true);
					avatar.RefreshAsGrave();
				}
				isDead = true;
			}
		}

		// Token: 0x04007241 RID: 29249
		[SerializeField]
		private Transform groupComponentTemplate;

		// Token: 0x04007242 RID: 29250
		[SerializeField]
		private RectTransform linkBaseRoot;

		// Token: 0x04007243 RID: 29251
		[SerializeField]
		private RectTransform linkBaseRootHighlight;

		// Token: 0x04007244 RID: 29252
		[SerializeField]
		private ScrollRect scrollRect;

		// Token: 0x04007245 RID: 29253
		[SerializeField]
		private GameObject charSelf;

		// Token: 0x04007246 RID: 29254
		[SerializeField]
		private GameObject charTemplate;

		// Token: 0x04007247 RID: 29255
		[SerializeField]
		private float focusCameraDuration;

		// Token: 0x04007248 RID: 29256
		[SerializeField]
		private TemplatedContainerAssembly templatedContainerAssembly;

		// Token: 0x04007249 RID: 29257
		[SerializeField]
		private LayoutGroup genealogyContentLayoutGroup;

		// Token: 0x0400724A RID: 29258
		[SerializeField]
		private CharacterMenuLocalLoadingAnim localLoadingAnim;

		// Token: 0x0400724B RID: 29259
		private Rect _contentRange;

		// Token: 0x0400724C RID: 29260
		private int _currentGenealogyGradeIndex;

		// Token: 0x0400724D RID: 29261
		private float _autoUpdateTime;

		// Token: 0x0400724E RID: 29262
		private int _refreshGeneration;

		// Token: 0x0400724F RID: 29263
		private Dictionary<GenealogyMaker.GenealogyNode, GenealogyCharacterPanel> _genealogyPanelMap;

		// Token: 0x04007250 RID: 29264
		private const string CharTemplatePrefabKey = "ViewCharacterMenuGenealogycharTemplate";

		// Token: 0x020021E2 RID: 8674
		public enum EGeneration
		{
			// Token: 0x0400D738 RID: 55096
			GrandParents,
			// Token: 0x0400D739 RID: 55097
			Parents,
			// Token: 0x0400D73A RID: 55098
			Siblings,
			// Token: 0x0400D73B RID: 55099
			Self,
			// Token: 0x0400D73C RID: 55100
			Spouses,
			// Token: 0x0400D73D RID: 55101
			Children,
			// Token: 0x0400D73E RID: 55102
			GrandChildren,
			// Token: 0x0400D73F RID: 55103
			Count
		}
	}
}
