using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using DG.Tweening;
using FrameWork;
using Game.Views.Map;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.Taiwu.Profession;
using GameData.Serializer;
using GameData.Utilities;
using Map.RenderSystem;
using UnityEngine;

// Token: 0x0200013F RID: 319
public static class ProfessionSkillController
{
	// Token: 0x170001DC RID: 476
	// (get) Token: 0x060010F0 RID: 4336 RVA: 0x00065221 File Offset: 0x00063421
	private static ProfessionData Current
	{
		get
		{
			return SingletonObject.getInstance<ProfessionModel>().GetProfessionData(ProfessionSkillController._professionSkillArg.ProfessionId);
		}
	}

	// Token: 0x060010F2 RID: 4338 RVA: 0x0006633C File Offset: 0x0006453C
	public static void ExecuteSkill(ProfessionSkillArg professionSkillArg, int beggarMoneyCount = 0, ItemDisplayData rebirthCricketItemData = null, Action onPostConfirm = null)
	{
		int skillIndex = SingletonObject.getInstance<ProfessionModel>().GetProfessionData(professionSkillArg.ProfessionId).GetSkillIndex(professionSkillArg.SkillId);
		ProfessionSkillController.ExecuteSkillDirect(professionSkillArg, skillIndex, beggarMoneyCount, rebirthCricketItemData, onPostConfirm);
	}

	// Token: 0x060010F3 RID: 4339 RVA: 0x00066374 File Offset: 0x00064574
	public static void ExecuteSkillDirect(ProfessionSkillArg professionSkillArg, int skillIndex = 0, int beggarMoneyCount = 0, ItemDisplayData rebirthCricketItemData = null, Action onPostConfirm = null)
	{
		ProfessionSkillController.<>c__DisplayClass12_0 CS$<>8__locals1 = new ProfessionSkillController.<>c__DisplayClass12_0();
		CS$<>8__locals1.professionSkillArg = professionSkillArg;
		CS$<>8__locals1.onPostConfirm = onPostConfirm;
		bool skipAnimation = CS$<>8__locals1.professionSkillArg.SkipAnimation;
		if (skipAnimation)
		{
			ExtraDomainMethod.Call.ConfirmExecuteSkill(CS$<>8__locals1.professionSkillArg, true);
			Action onPostConfirm2 = CS$<>8__locals1.onPostConfirm;
			if (onPostConfirm2 != null)
			{
				onPostConfirm2();
			}
		}
		else
		{
			ProfessionSkillController.StartShowSkillAnim(CS$<>8__locals1.professionSkillArg, skillIndex, new Action(CS$<>8__locals1.<ExecuteSkillDirect>g__ConfirmExecuteSkill|0), beggarMoneyCount, CS$<>8__locals1.professionSkillArg.SkillId == 46, rebirthCricketItemData);
		}
	}

	// Token: 0x060010F4 RID: 4340 RVA: 0x000663F8 File Offset: 0x000645F8
	public static void StartShowSkillAnim(ProfessionSkillArg professionSkillArg, int skillIndex, Action callback, int beggarMoneyCount = 0, bool useTravel2 = false, ItemDisplayData rebirthCricketData = null)
	{
		ProfessionSkillController._professionSkillArg = professionSkillArg;
		ProfessionSkillController._skillIndex = skillIndex;
		ProfessionSkillController._callback = callback;
		ProfessionSkillController._useTravel2 = useTravel2;
		ProfessionSkillController._rebirthCricketData = rebirthCricketData;
		ProfessionSkillController._beggarMoneyCount = beggarMoneyCount;
		ProfessionSkillController._mapModel = SingletonObject.getInstance<WorldMapModel>();
		GEvent.OnEvent(UiEvents.DoWorldMapScaleTween, EasyPool.Get<ArgumentBox>().SetObject("tweenType", EWorldmapScaleTweenType.RecordScale));
		SingletonObject.getInstance<YieldHelper>().StartCoroutine(ProfessionSkillController.MainProcess());
	}

	// Token: 0x060010F5 RID: 4341 RVA: 0x0006646B File Offset: 0x0006466B
	[return: TupleElementNames(new string[]
	{
		"invoke",
		"delay"
	})]
	private static IEnumerable<ValueTuple<ProfessionSkillController.OnceProfessionSkillInvoke, float>> GetInvokes(ProfessionSkillController.ProfessionSkillInvokeConfigContainer config, int professionId)
	{
		bool flag = config == null;
		if (flag)
		{
			yield break;
		}
		Time.timeScale = config.TimeScale;
		bool flag2 = config.BeforeEnter != null;
		if (flag2)
		{
			foreach (ProfessionSkillController.OnceProfessionSkillInvoke invoke in config.BeforeEnter)
			{
				yield return new ValueTuple<ProfessionSkillController.OnceProfessionSkillInvoke, float>(invoke, invoke.Delay);
				invoke = null;
			}
			ProfessionSkillController.OnceProfessionSkillInvoke[] array = null;
		}
		bool useStandardEnter = config.UseStandardEnter;
		if (useStandardEnter)
		{
			foreach (ProfessionSkillController.OnceProfessionSkillInvoke invoke2 in ProfessionSkillController.StandardEnter)
			{
				yield return new ValueTuple<ProfessionSkillController.OnceProfessionSkillInvoke, float>(invoke2, invoke2.Delay);
				invoke2 = null;
			}
			ProfessionSkillController.OnceProfessionSkillInvoke[] array2 = null;
		}
		bool flag3 = config.MainInvokes != null;
		if (flag3)
		{
			IEnumerable<ProfessionSkillController.OnceProfessionSkillInvoke> enumerable;
			if (!(config.MainInvokeParser == ProfessionSkillController.DefaultParser))
			{
				enumerable = config.MainInvokeParser(ProfessionSkillController.Current, config.MainInvokes);
			}
			else
			{
				IEnumerable<ProfessionSkillController.OnceProfessionSkillInvoke> mainInvokes = config.MainInvokes;
				enumerable = mainInvokes;
			}
			foreach (ProfessionSkillController.OnceProfessionSkillInvoke invoke3 in enumerable)
			{
				yield return new ValueTuple<ProfessionSkillController.OnceProfessionSkillInvoke, float>(invoke3, invoke3.Delay);
				invoke3 = null;
			}
			IEnumerator<ProfessionSkillController.OnceProfessionSkillInvoke> enumerator = null;
		}
		bool useStandardExit = config.UseStandardExit;
		if (useStandardExit)
		{
			ProfessionSkillController.OnceProfessionSkillInvoke close = ProfessionSkillController.StandardExit[0];
			yield return new ValueTuple<ProfessionSkillController.OnceProfessionSkillInvoke, float>(close, (config.CloseDelay > 0f) ? config.CloseDelay : close.Delay);
			int num;
			for (int i = 1; i < ProfessionSkillController.StandardExit.Length; i = num + 1)
			{
				ProfessionSkillController.OnceProfessionSkillInvoke invoke4 = ProfessionSkillController.StandardExit[i];
				yield return new ValueTuple<ProfessionSkillController.OnceProfessionSkillInvoke, float>(invoke4, invoke4.Delay);
				invoke4 = null;
				num = i;
			}
			close = null;
		}
		bool flag4 = config.AfterExit != null;
		if (flag4)
		{
			foreach (ProfessionSkillController.OnceProfessionSkillInvoke invoke5 in config.AfterExit)
			{
				yield return new ValueTuple<ProfessionSkillController.OnceProfessionSkillInvoke, float>(invoke5, invoke5.Delay);
				invoke5 = null;
			}
			ProfessionSkillController.OnceProfessionSkillInvoke[] array3 = null;
		}
		Time.timeScale = 1f;
		yield break;
		yield break;
	}

	// Token: 0x060010F6 RID: 4342 RVA: 0x00066484 File Offset: 0x00064684
	private static void ExecuteInvoke(ProfessionSkillController.OnceProfessionSkillInvoke invoke)
	{
		switch (invoke.MethodIndex)
		{
		case 0:
			ProfessionSkillController.RecoveryViewToCenterAndNormal();
			break;
		case 1:
			ProfessionSkillController.OpenProfessionSkillUI();
			break;
		case 2:
			ProfessionSkillController.ShowProfessionIllustrationAndSkill();
			break;
		case 3:
			ProfessionSkillController.PlaySkillAnimAndShowEffect();
			break;
		case 4:
			ProfessionSkillController.AdjustMapBlockLightState((int)invoke.Parameters[0], (bool)invoke.Parameters[1]);
			break;
		case 5:
			ProfessionSkillController.ShowPropertyChange();
			break;
		case 6:
			ProfessionSkillController.ShowTeammateUI();
			break;
		case 7:
			ProfessionSkillController.ShowTravelerUI();
			break;
		case 8:
			ProfessionSkillController.PlaySkillSound();
			break;
		case 9:
			ProfessionSkillController.CloseAnimAndEffect();
			break;
		case 10:
			ProfessionSkillController.OnclickSkillCallBack();
			break;
		case 11:
			ProfessionSkillController.InvokeSomeFixMethod();
			break;
		case 12:
			ProfessionSkillController.AdjustMapBlockStateToBeggarSkill3((int)invoke.Parameters[0], (bool)invoke.Parameters[1]);
			break;
		case 13:
			ProfessionSkillController.MoveCameraToRegularMapBlock();
			break;
		case 14:
			ProfessionSkillController.RecoveryViewToOrigin();
			break;
		case 15:
			ProfessionSkillController.RecoveryViewToCenter();
			break;
		case 16:
			ProfessionSkillController.TeleportMoveStart();
			break;
		case 17:
			ProfessionSkillController.TeleportMoveEnd();
			break;
		}
	}

	// Token: 0x060010F7 RID: 4343 RVA: 0x000665D4 File Offset: 0x000647D4
	private static IEnumerator MainProcess()
	{
		int professionId = ProfessionSkillController._professionSkillArg.ProfessionId;
		bool tryReverse = professionId == 11 && ProfessionSkillController._skillIndex == 2 && !ProfessionSkillController._useTravel2;
		ValueTuple<int, int> index = new ValueTuple<int, int>(professionId, tryReverse ? (-ProfessionSkillController._skillIndex) : ProfessionSkillController._skillIndex);
		ProfessionSkillController.ProfessionSkillInvokeConfigContainer config;
		bool flag = ProfessionSkillController.InvokeConfigs.TryGetValue(index, out config) && config != null && ProfessionSkillController.CheckEffectBlocks(config);
		if (flag)
		{
			GEvent.OnEvent(UiEvents.OnClickWorldMapBlock, EasyPool.Get<ArgumentBox>().SetObject("mapBlockData", ProfessionSkillController._mapModel.GetBlockData(ProfessionSkillController._mapModel.CurrentBlockId)));
			GEvent.OnEvent(UiEvents.PlayAnimToHideMainUI, EasyPool.Get<ArgumentBox>().Set("KeepTimePanel", ProfessionSkillController._professionSkillArg.SkillId == 46));
			foreach (ValueTuple<ProfessionSkillController.OnceProfessionSkillInvoke, float> tuple in ProfessionSkillController.GetInvokes(config, professionId))
			{
				bool flag2 = tuple.Item2 > 0f;
				if (flag2)
				{
					yield return new WaitForSeconds(tuple.Item2);
				}
				try
				{
					ProfessionSkillController.ExecuteInvoke(tuple.Item1);
				}
				catch (Exception ex)
				{
					Exception e = ex;
					PredefinedLog.Show(6, professionId, ProfessionSkillController._skillIndex, e.Message + "\n" + e.StackTrace);
					break;
				}
				tuple = default(ValueTuple<ProfessionSkillController.OnceProfessionSkillInvoke, float>);
			}
			IEnumerator<ValueTuple<ProfessionSkillController.OnceProfessionSkillInvoke, float>> enumerator = null;
			GEvent.OnEvent(UiEvents.PlayAnimToShowMainUI, EasyPool.Get<ArgumentBox>().Set("KeepTimePanel", ProfessionSkillController._professionSkillArg.SkillId == 46));
		}
		else
		{
			ProfessionSkillController._callback();
		}
		yield break;
		yield break;
	}

	// Token: 0x060010F8 RID: 4344 RVA: 0x000665DC File Offset: 0x000647DC
	private static void RecoveryViewToOrigin()
	{
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("duration", 0.7f);
		argBox.SetObject("easeMode", Ease.OutQuad);
		argBox.SetObject("tweenType", EWorldmapScaleTweenType.RestoreScale);
		GEvent.OnEvent(UiEvents.DoWorldMapScaleTween, argBox);
	}

	// Token: 0x060010F9 RID: 4345 RVA: 0x00066636 File Offset: 0x00064836
	private static void RecoveryViewToCenter()
	{
		GEvent.OnEvent(UiEvents.WorldMapResetMapCamera, EasyPool.Get<ArgumentBox>().SetObject("easeMode", Ease.OutQuad).Set("isAnim", true));
	}

	// Token: 0x060010FA RID: 4346 RVA: 0x00066668 File Offset: 0x00064868
	private static void TryInvokeEndEvent()
	{
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
		argumentBox.Set("IsExtraordinary", ProfessionSkillController._professionSkillArg.IsExtraordinary);
		argumentBox.Set<ItemKey>("ItemKey", ProfessionSkillController._professionSkillArg.ItemKey);
		GEvent.OnEvent(UiEvents.InvokeProfessionEvent, argumentBox);
	}

	// Token: 0x060010FB RID: 4347 RVA: 0x000666BC File Offset: 0x000648BC
	private static void MoveCameraToRegularMapBlock()
	{
		WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
		Location location = mapModel.LastAnimalDataChangedLocation;
		bool flag = location.IsValid() && mapModel.CurrentAreaId == location.AreaId;
		if (flag)
		{
			GEvent.OnEvent(UiEvents.WorldMapSetCameraToLocation, EasyPool.Get<ArgumentBox>().Set<Location>("location", location).Set("doTween", true).SetObject("tweenCallBack", null).Set("tweenTime", 1f).Set("ease", Ease.OutQuad));
		}
	}

	// Token: 0x060010FC RID: 4348 RVA: 0x0006674C File Offset: 0x0006494C
	private static void RecoveryViewToCenterAndNormal()
	{
		GEvent.OnEvent(UiEvents.WorldMapResetMapCamera, EasyPool.Get<ArgumentBox>().Set("isAnim", true));
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("targetScale", 1f);
		argBox.Set("duration", 0.7f);
		argBox.SetObject("easeMode", Ease.OutQuad);
		argBox.SetObject("tweenType", EWorldmapScaleTweenType.SimpleFloat);
		GEvent.OnEvent(UiEvents.DoWorldMapScaleTween, argBox);
	}

	// Token: 0x060010FD RID: 4349 RVA: 0x000667D4 File Offset: 0x000649D4
	private static void OpenProfessionSkillUI()
	{
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("selectSkillIndex", ProfessionSkillController._skillIndex);
		argBox.Set("ProfessionId", ProfessionSkillController._professionSkillArg.ProfessionId);
		UIElement.ProfessionMask.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.ProfessionMask, true);
	}

	// Token: 0x060010FE RID: 4350 RVA: 0x0006682C File Offset: 0x00064A2C
	private static void ShowProfessionIllustrationAndSkill()
	{
		GEvent.OnEvent(UiEvents.UpdateProfessionIllustrationAndSkill, null);
	}

	// Token: 0x060010FF RID: 4351 RVA: 0x00066840 File Offset: 0x00064A40
	private static void PlaySkillAnimAndShowEffect()
	{
		GEvent.OnEvent(UiEvents.PlayProfessionSkillAnimAndShowEffect, EasyPool.Get<ArgumentBox>().Set("success", ProfessionSkillController._professionSkillArg.IsSuccess).Set("isFirst", ProfessionSkillController._useTravel2).Set("money", ProfessionSkillController._beggarMoneyCount).SetObject("DisplayData", ProfessionSkillController._rebirthCricketData).SetObject("blocks", ProfessionSkillController._professionSkillArg.EffectBlocks));
	}

	// Token: 0x06001100 RID: 4352 RVA: 0x000668B8 File Offset: 0x00064AB8
	private static void AdjustMapBlockLightState(int width, bool isObliqueDirection)
	{
		SingletonObject.getInstance<MapRenderSystem>().AdjustMapBlockAdditionalLightState(width, isObliqueDirection);
	}

	// Token: 0x06001101 RID: 4353 RVA: 0x000668C8 File Offset: 0x00064AC8
	private static void AdjustMapBlockStateToBeggarSkill3(int layer, bool isLight)
	{
		SingletonObject.getInstance<MapRenderSystem>().AdjustMapBlockStateToBeggarSkill3(layer, isLight);
	}

	// Token: 0x06001102 RID: 4354 RVA: 0x000668D8 File Offset: 0x00064AD8
	private static void ShowPropertyChange()
	{
		GEvent.OnEvent(UiEvents.ShowProfessionPropertyChange, null);
	}

	// Token: 0x06001103 RID: 4355 RVA: 0x000668EC File Offset: 0x00064AEC
	private static void PlaySkillSound()
	{
		GEvent.OnEvent(UiEvents.PlayProfessionAudioSound, EasyPool.Get<ArgumentBox>().Set("success", ProfessionSkillController._professionSkillArg.IsSuccess).Set("isFirst", ProfessionSkillController._useTravel2).Set("money", ProfessionSkillController._beggarMoneyCount));
	}

	// Token: 0x06001104 RID: 4356 RVA: 0x00066941 File Offset: 0x00064B41
	private static void CloseAnimAndEffect()
	{
		SingletonObject.getInstance<MapRenderSystem>().ClearMapBlockAdditionalLightState(true);
		GEvent.OnEvent(UiEvents.CloseProfessionMask, null);
	}

	// Token: 0x06001105 RID: 4357 RVA: 0x00066961 File Offset: 0x00064B61
	private static void OnclickSkillCallBack()
	{
		ProfessionSkillController._callback();
	}

	// Token: 0x06001106 RID: 4358 RVA: 0x00066970 File Offset: 0x00064B70
	private static void ShowTeammateUI()
	{
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
		argumentBox.Set("teammateCharacterId", ProfessionSkillController._professionSkillArg.CharId);
		GEvent.OnEvent(UiEvents.ShowProfessionTeammateUI, argumentBox);
	}

	// Token: 0x06001107 RID: 4359 RVA: 0x000669AB File Offset: 0x00064BAB
	private static void ShowTravelerUI()
	{
		UIManager.Instance.MaskUI(UIElement.ProfessionTravelerStation);
	}

	// Token: 0x06001108 RID: 4360 RVA: 0x000669BD File Offset: 0x00064BBD
	private static void InvokeSomeFixMethod()
	{
		MapDomainMethod.AsyncCall.CollectAllResourcesFree(null, delegate(int offset, RawDataPool dataPool)
		{
			List<CollectResourceResult> results = null;
			Serializer.Deserialize(dataPool, offset, ref results);
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
			argumentBox.SetObject("CollectInfo", results);
			argumentBox.Set("CollectType", 2);
			UIElement.CollectResource.SetOnInitArgs(argumentBox);
			UIManager.Instance.ShowUI(UIElement.CollectResource, true);
			EasyPool.Free<ArgumentBox>(argumentBox);
		});
	}

	// Token: 0x06001109 RID: 4361 RVA: 0x000669E8 File Offset: 0x00064BE8
	private static bool CheckEffectBlocks(ProfessionSkillController.ProfessionSkillInvokeConfigContainer config)
	{
		int skillId = ProfessionSkillController._professionSkillArg.SkillId;
		int num = skillId;
		bool result;
		if (num != 15)
		{
			if (num != 71)
			{
				result = true;
			}
			else
			{
				bool flag = ProfessionSkillController._professionSkillArg.EffectBlocks != null;
				if (flag)
				{
					ProfessionSkillController._callback = (Action)Delegate.Combine(ProfessionSkillController._callback, new Action(delegate()
					{
						ExtraDomainMethod.Call.SetDukeSkill3Crickets(ProfessionSkillController._professionSkillArg.EffectBlocks);
					}));
				}
				result = (ProfessionSkillController._professionSkillArg.EffectBlocks != null);
			}
		}
		else
		{
			List<short> effectBlocks = ProfessionSkillController._professionSkillArg.EffectBlocks;
			int effectBlocksCount = (effectBlocks != null) ? effectBlocks.Count : 0;
			config.CloseDelay = (float)effectBlocksCount * 0.2f + 2f;
			result = true;
		}
		return result;
	}

	// Token: 0x0600110A RID: 4362 RVA: 0x00066AA0 File Offset: 0x00064CA0
	private static void TeleportMoveStart()
	{
		AudioManager.Instance.PlaySound("SFX_professionskill_lvren_exit", false, false);
		GEvent.OnEvent(UiEvents.ProfessionTravelerSkillTwoMoveStart, null);
	}

	// Token: 0x0600110B RID: 4363 RVA: 0x00066AC6 File Offset: 0x00064CC6
	private static void TeleportMoveEnd()
	{
		AudioManager.Instance.PlaySound("SFX_professionskill_lvren_enter", false, false);
		GEvent.OnEvent(UiEvents.ProfessionTravelerSkillTwoMoveEnd, null);
	}

	// Token: 0x0600110C RID: 4364 RVA: 0x00066AEC File Offset: 0x00064CEC
	private static IEnumerable<ProfessionSkillController.OnceProfessionSkillInvoke> InvokeParserTravel2(ProfessionData professionData, ProfessionSkillController.OnceProfessionSkillInvoke[] invokes)
	{
		yield return invokes[0];
		int level = CommonUtils.GetProfessionVisionLevel(professionData);
		int viewRange = CommonUtils.GetProfessionVisionLevelAfterSkill(professionData);
		bool flag = level >= 0;
		if (flag)
		{
			yield return invokes[1];
		}
		bool flag2 = level >= 1;
		if (flag2)
		{
			yield return invokes[2];
		}
		else
		{
			yield return new ProfessionSkillController.OnceProfessionSkillInvoke(invokes[2].Delay, -1, null);
		}
		bool flag3 = level >= 2;
		if (flag3)
		{
			yield return invokes[3];
			bool flag4 = viewRange == 4;
			if (flag4)
			{
				yield return invokes[4];
			}
			else
			{
				bool flag5 = viewRange >= 5;
				if (flag5)
				{
					yield return invokes[5];
				}
			}
		}
		else
		{
			yield return new ProfessionSkillController.OnceProfessionSkillInvoke(invokes[3].Delay, -1, null);
		}
		yield break;
	}

	// Token: 0x04000EF4 RID: 3828
	private static ProfessionSkillArg _professionSkillArg;

	// Token: 0x04000EF5 RID: 3829
	private static int _skillIndex;

	// Token: 0x04000EF6 RID: 3830
	private static Action _callback;

	// Token: 0x04000EF7 RID: 3831
	private static bool _useTravel2;

	// Token: 0x04000EF8 RID: 3832
	private static ItemDisplayData _rebirthCricketData;

	// Token: 0x04000EF9 RID: 3833
	private static int _beggarMoneyCount;

	// Token: 0x04000EFA RID: 3834
	private static WorldMapModel _mapModel;

	// Token: 0x04000EFB RID: 3835
	private static readonly ProfessionSkillController.InvokeParser DefaultParser = (ProfessionData _, ProfessionSkillController.OnceProfessionSkillInvoke[] invokes) => invokes;

	// Token: 0x04000EFC RID: 3836
	private static readonly ProfessionSkillController.OnceProfessionSkillInvoke[] StandardEnter = new ProfessionSkillController.OnceProfessionSkillInvoke[]
	{
		new ProfessionSkillController.OnceProfessionSkillInvoke(0.01f, ProfessionSkillController.ProfessionSkillProcess.RecoveryViewToCenterAndNormal.ToInt(), null),
		new ProfessionSkillController.OnceProfessionSkillInvoke(0.01f, ProfessionSkillController.ProfessionSkillProcess.AdjustMapBlockLightState.ToInt(), new List<object>
		{
			0,
			true
		}),
		new ProfessionSkillController.OnceProfessionSkillInvoke(0.01f, ProfessionSkillController.ProfessionSkillProcess.OpenProfessionSkillUI.ToInt(), null),
		new ProfessionSkillController.OnceProfessionSkillInvoke(0.01f, ProfessionSkillController.ProfessionSkillProcess.ShowProfessionIllustrationAndSkill.ToInt(), null),
		new ProfessionSkillController.OnceProfessionSkillInvoke(1f, ProfessionSkillController.ProfessionSkillProcess.PlaySkillAnimAndShowEffect.ToInt(), null),
		new ProfessionSkillController.OnceProfessionSkillInvoke(0f, ProfessionSkillController.ProfessionSkillProcess.PlaySkillSound.ToInt(), null)
	};

	// Token: 0x04000EFD RID: 3837
	private static readonly ProfessionSkillController.OnceProfessionSkillInvoke[] StandardExit = new ProfessionSkillController.OnceProfessionSkillInvoke[]
	{
		new ProfessionSkillController.OnceProfessionSkillInvoke(3f, ProfessionSkillController.ProfessionSkillProcess.CloseAnimAndEffect.ToInt(), null),
		new ProfessionSkillController.OnceProfessionSkillInvoke(0.5f, ProfessionSkillController.ProfessionSkillProcess.OnclickSkillCallBack.ToInt(), null),
		new ProfessionSkillController.OnceProfessionSkillInvoke(0.01f, ProfessionSkillController.ProfessionSkillProcess.RecoveryViewToOrigin.ToInt(), null)
	};

	// Token: 0x04000EFE RID: 3838
	private static readonly ProfessionSkillController.ProfessionSkillInvokeConfigContainer EmptyInvokes = new ProfessionSkillController.ProfessionSkillInvokeConfigContainer();

	// Token: 0x04000EFF RID: 3839
	private static readonly ProfessionSkillController.ProfessionSkillInvokeConfigContainer PropertyChangedInvokes = new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
	{
		MainInvokes = new ProfessionSkillController.OnceProfessionSkillInvoke[]
		{
			new ProfessionSkillController.OnceProfessionSkillInvoke(0f, ProfessionSkillController.ProfessionSkillProcess.ShowPropertyChange.ToInt(), null)
		}
	};

	// Token: 0x04000F00 RID: 3840
	private static readonly Dictionary<ValueTuple<int, int>, ProfessionSkillController.ProfessionSkillInvokeConfigContainer> InvokeConfigs = new Dictionary<ValueTuple<int, int>, ProfessionSkillController.ProfessionSkillInvokeConfigContainer>
	{
		{
			new ValueTuple<int, int>(0, 0),
			ProfessionSkillController.PropertyChangedInvokes
		},
		{
			new ValueTuple<int, int>(0, 1),
			new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
			{
				TimeScale = 1.75f,
				AfterExit = new ProfessionSkillController.OnceProfessionSkillInvoke[]
				{
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.01f, ProfessionSkillController.ProfessionSkillProcess.InvokeSomeFixMethod.ToInt(), null)
				}
			}
		},
		{
			new ValueTuple<int, int>(0, 2),
			null
		},
		{
			new ValueTuple<int, int>(0, 3),
			new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
			{
				TimeScale = 2.5f
			}
		},
		{
			new ValueTuple<int, int>(1, 0),
			new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
			{
				TimeScale = 2f,
				CloseDelay = 1f,
				MainInvokes = new ProfessionSkillController.OnceProfessionSkillInvoke[]
				{
					new ProfessionSkillController.OnceProfessionSkillInvoke(1f, ProfessionSkillController.ProfessionSkillProcess.AdjustMapBlockLightState.ToInt(), new List<object>
					{
						1,
						true
					}),
					new ProfessionSkillController.OnceProfessionSkillInvoke(1f, ProfessionSkillController.ProfessionSkillProcess.AdjustMapBlockLightState.ToInt(), new List<object>
					{
						0,
						true
					})
				}
			}
		},
		{
			new ValueTuple<int, int>(1, 1),
			null
		},
		{
			new ValueTuple<int, int>(1, 2),
			new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
			{
				UseStandardExit = false,
				TimeScale = 2f,
				MainInvokes = new ProfessionSkillController.OnceProfessionSkillInvoke[]
				{
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.5f, ProfessionSkillController.ProfessionSkillProcess.OnclickSkillCallBack.ToInt(), null),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.01f, ProfessionSkillController.ProfessionSkillProcess.RecoveryViewToOrigin.ToInt(), null),
					new ProfessionSkillController.OnceProfessionSkillInvoke(1.5f, ProfessionSkillController.ProfessionSkillProcess.MoveCameraToRegularMapBlock.ToInt(), null),
					new ProfessionSkillController.OnceProfessionSkillInvoke(1.5f, ProfessionSkillController.ProfessionSkillProcess.RecoveryViewToCenter.ToInt(), null),
					new ProfessionSkillController.OnceProfessionSkillInvoke(1f, ProfessionSkillController.ProfessionSkillProcess.CloseAnimAndEffect.ToInt(), null)
				}
			}
		},
		{
			new ValueTuple<int, int>(2, 0),
			null
		},
		{
			new ValueTuple<int, int>(2, 1),
			new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
			{
				TimeScale = 1.75f,
				CloseDelay = 0.8f,
				MainInvokes = new ProfessionSkillController.OnceProfessionSkillInvoke[]
				{
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.5f, ProfessionSkillController.ProfessionSkillProcess.AdjustMapBlockLightState.ToInt(), new List<object>
					{
						1,
						false
					}),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.5f, ProfessionSkillController.ProfessionSkillProcess.AdjustMapBlockLightState.ToInt(), new List<object>
					{
						0,
						true
					}),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.3f, ProfessionSkillController.ProfessionSkillProcess.AdjustMapBlockLightState.ToInt(), new List<object>
					{
						1,
						false
					}),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.5f, ProfessionSkillController.ProfessionSkillProcess.AdjustMapBlockLightState.ToInt(), new List<object>
					{
						0,
						true
					}),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.3f, ProfessionSkillController.ProfessionSkillProcess.AdjustMapBlockLightState.ToInt(), new List<object>
					{
						1,
						false
					}),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.5f, ProfessionSkillController.ProfessionSkillProcess.AdjustMapBlockLightState.ToInt(), new List<object>
					{
						0,
						true
					})
				}
			}
		},
		{
			new ValueTuple<int, int>(2, 2),
			null
		},
		{
			new ValueTuple<int, int>(3, 0),
			null
		},
		{
			new ValueTuple<int, int>(3, 1),
			null
		},
		{
			new ValueTuple<int, int>(3, 2),
			new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
			{
				CloseDelay = 0.5f,
				TimeScale = 1.75f,
				MainInvokes = new ProfessionSkillController.OnceProfessionSkillInvoke[]
				{
					new ProfessionSkillController.OnceProfessionSkillInvoke(1.7f, ProfessionSkillController.ProfessionSkillProcess.AdjustMapBlockLightState.ToInt(), new List<object>
					{
						1,
						false
					}),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.5f, ProfessionSkillController.ProfessionSkillProcess.AdjustMapBlockLightState.ToInt(), new List<object>
					{
						1,
						true
					}),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.8f, ProfessionSkillController.ProfessionSkillProcess.AdjustMapBlockLightState.ToInt(), new List<object>
					{
						0,
						true
					})
				}
			}
		},
		{
			new ValueTuple<int, int>(3, 3),
			new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
			{
				TimeScale = 2f
			}
		},
		{
			new ValueTuple<int, int>(4, 0),
			null
		},
		{
			new ValueTuple<int, int>(4, 1),
			new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
			{
				TimeScale = 2f
			}
		},
		{
			new ValueTuple<int, int>(4, 2),
			new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
			{
				TimeScale = 2f
			}
		},
		{
			new ValueTuple<int, int>(4, 3),
			new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
			{
				TimeScale = 3f
			}
		},
		{
			new ValueTuple<int, int>(5, 0),
			ProfessionSkillController.PropertyChangedInvokes
		},
		{
			new ValueTuple<int, int>(5, 1),
			null
		},
		{
			new ValueTuple<int, int>(5, 2),
			null
		},
		{
			new ValueTuple<int, int>(5, 3),
			ProfessionSkillController.EmptyInvokes
		},
		{
			new ValueTuple<int, int>(6, 0),
			ProfessionSkillController.PropertyChangedInvokes
		},
		{
			new ValueTuple<int, int>(6, 1),
			null
		},
		{
			new ValueTuple<int, int>(6, 2),
			new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
			{
				TimeScale = 2.5f
			}
		},
		{
			new ValueTuple<int, int>(6, 3),
			ProfessionSkillController.EmptyInvokes
		},
		{
			new ValueTuple<int, int>(7, 0),
			null
		},
		{
			new ValueTuple<int, int>(7, 1),
			null
		},
		{
			new ValueTuple<int, int>(7, 2),
			null
		},
		{
			new ValueTuple<int, int>(7, 3),
			new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
			{
				TimeScale = 3f
			}
		},
		{
			new ValueTuple<int, int>(8, 0),
			null
		},
		{
			new ValueTuple<int, int>(8, 1),
			new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
			{
				TimeScale = 2.5f,
				MainInvokes = new ProfessionSkillController.OnceProfessionSkillInvoke[]
				{
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.01f, ProfessionSkillController.ProfessionSkillProcess.ShowTeammateUI.ToInt(), null)
				}
			}
		},
		{
			new ValueTuple<int, int>(8, 2),
			null
		},
		{
			new ValueTuple<int, int>(8, 3),
			new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
			{
				TimeScale = 3f
			}
		},
		{
			new ValueTuple<int, int>(9, 0),
			new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
			{
				TimeScale = 1.75f
			}
		},
		{
			new ValueTuple<int, int>(9, 1),
			new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
			{
				TimeScale = 2f
			}
		},
		{
			new ValueTuple<int, int>(9, 2),
			new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
			{
				TimeScale = 2f,
				CloseDelay = 1f,
				MainInvokes = new ProfessionSkillController.OnceProfessionSkillInvoke[]
				{
					new ProfessionSkillController.OnceProfessionSkillInvoke(1f, ProfessionSkillController.ProfessionSkillProcess.AdjustMapBlockStateToBeggarSkill3.ToInt(), new List<object>
					{
						1,
						true
					}),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.6f, ProfessionSkillController.ProfessionSkillProcess.AdjustMapBlockStateToBeggarSkill3.ToInt(), new List<object>
					{
						2,
						true
					}),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.7f, ProfessionSkillController.ProfessionSkillProcess.AdjustMapBlockStateToBeggarSkill3.ToInt(), new List<object>
					{
						3,
						true
					}),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.5f, ProfessionSkillController.ProfessionSkillProcess.AdjustMapBlockStateToBeggarSkill3.ToInt(), new List<object>
					{
						1,
						false
					}),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0f, ProfessionSkillController.ProfessionSkillProcess.AdjustMapBlockStateToBeggarSkill3.ToInt(), new List<object>
					{
						2,
						false
					}),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0f, ProfessionSkillController.ProfessionSkillProcess.AdjustMapBlockStateToBeggarSkill3.ToInt(), new List<object>
					{
						3,
						false
					})
				}
			}
		},
		{
			new ValueTuple<int, int>(10, 0),
			null
		},
		{
			new ValueTuple<int, int>(10, 1),
			ProfessionSkillController.EmptyInvokes
		},
		{
			new ValueTuple<int, int>(10, 2),
			null
		},
		{
			new ValueTuple<int, int>(11, 0),
			ProfessionSkillController.PropertyChangedInvokes
		},
		{
			new ValueTuple<int, int>(11, 1),
			new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
			{
				TimeScale = 2f,
				CloseDelay = 0.7f,
				MainInvokeParser = new ProfessionSkillController.InvokeParser(ProfessionSkillController.InvokeParserTravel2),
				MainInvokes = new ProfessionSkillController.OnceProfessionSkillInvoke[]
				{
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.5f, ProfessionSkillController.ProfessionSkillProcess.AdjustMapBlockLightState.ToInt(), new List<object>
					{
						0,
						true
					}),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.8f, ProfessionSkillController.ProfessionSkillProcess.AdjustMapBlockLightState.ToInt(), new List<object>
					{
						1,
						false
					}),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.7f, ProfessionSkillController.ProfessionSkillProcess.AdjustMapBlockLightState.ToInt(), new List<object>
					{
						2,
						false
					}),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.7f, ProfessionSkillController.ProfessionSkillProcess.AdjustMapBlockLightState.ToInt(), new List<object>
					{
						3,
						false
					}),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0f, ProfessionSkillController.ProfessionSkillProcess.AdjustMapBlockLightState.ToInt(), new List<object>
					{
						4,
						false
					}),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0f, ProfessionSkillController.ProfessionSkillProcess.AdjustMapBlockLightState.ToInt(), new List<object>
					{
						5,
						false
					})
				}
			}
		},
		{
			new ValueTuple<int, int>(11, 2),
			new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
			{
				TimeScale = 1.5f,
				UseStandardEnter = false,
				UseStandardExit = false,
				MainInvokes = new ProfessionSkillController.OnceProfessionSkillInvoke[]
				{
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.01f, ProfessionSkillController.ProfessionSkillProcess.RecoveryViewToCenterAndNormal.ToInt(), null),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.01f, ProfessionSkillController.ProfessionSkillProcess.OpenProfessionSkillUI.ToInt(), null),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0f, ProfessionSkillController.ProfessionSkillProcess.TeleportMoveStart.ToInt(), null),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.5f, ProfessionSkillController.ProfessionSkillProcess.OnclickSkillCallBack.ToInt(), null),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.5f, ProfessionSkillController.ProfessionSkillProcess.TeleportMoveEnd.ToInt(), null),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.01f, ProfessionSkillController.ProfessionSkillProcess.CloseAnimAndEffect.ToInt(), null),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.01f, ProfessionSkillController.ProfessionSkillProcess.RecoveryViewToOrigin.ToInt(), null)
				}
			}
		},
		{
			new ValueTuple<int, int>(11, -2),
			new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
			{
				TimeScale = 1.5f,
				CloseDelay = 2.5f,
				UseStandardEnter = false,
				UseStandardExit = false,
				MainInvokes = new ProfessionSkillController.OnceProfessionSkillInvoke[]
				{
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.01f, ProfessionSkillController.ProfessionSkillProcess.AdjustMapBlockLightState.ToInt(), new List<object>
					{
						0,
						true
					}),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.01f, ProfessionSkillController.ProfessionSkillProcess.OpenProfessionSkillUI.ToInt(), null),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.5f, ProfessionSkillController.ProfessionSkillProcess.PlaySkillAnimAndShowEffect.ToInt(), null),
					new ProfessionSkillController.OnceProfessionSkillInvoke(2.5f, ProfessionSkillController.ProfessionSkillProcess.CloseAnimAndEffect.ToInt(), null),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.5f, ProfessionSkillController.ProfessionSkillProcess.OnclickSkillCallBack.ToInt(), null)
				}
			}
		},
		{
			new ValueTuple<int, int>(11, 3),
			new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
			{
				UseStandardEnter = false,
				UseStandardExit = false,
				MainInvokes = new ProfessionSkillController.OnceProfessionSkillInvoke[]
				{
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.01f, ProfessionSkillController.ProfessionSkillProcess.RecoveryViewToCenterAndNormal.ToInt(), null),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.01f, ProfessionSkillController.ProfessionSkillProcess.AdjustMapBlockLightState.ToInt(), new List<object>
					{
						0,
						true
					}),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.01f, ProfessionSkillController.ProfessionSkillProcess.OpenProfessionSkillUI.ToInt(), null),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.01f, ProfessionSkillController.ProfessionSkillProcess.ShowProfessionIllustrationAndSkill.ToInt(), null),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0f, ProfessionSkillController.ProfessionSkillProcess.OnclickSkillCallBack.ToInt(), null),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0f, ProfessionSkillController.ProfessionSkillProcess.PlaySkillSound.ToInt(), null),
					new ProfessionSkillController.OnceProfessionSkillInvoke(2f, ProfessionSkillController.ProfessionSkillProcess.CloseAnimAndEffect.ToInt(), null),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.01f, ProfessionSkillController.ProfessionSkillProcess.RecoveryViewToOrigin.ToInt(), null),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.2f, ProfessionSkillController.ProfessionSkillProcess.ShowTravelerUI.ToInt(), null)
				}
			}
		},
		{
			new ValueTuple<int, int>(12, 0),
			ProfessionSkillController.PropertyChangedInvokes
		},
		{
			new ValueTuple<int, int>(12, 1),
			null
		},
		{
			new ValueTuple<int, int>(12, 2),
			new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
			{
				TimeScale = 3f
			}
		},
		{
			new ValueTuple<int, int>(12, 3),
			null
		},
		{
			new ValueTuple<int, int>(13, 0),
			null
		},
		{
			new ValueTuple<int, int>(13, 1),
			ProfessionSkillController.EmptyInvokes
		},
		{
			new ValueTuple<int, int>(13, 2),
			null
		},
		{
			new ValueTuple<int, int>(13, 3),
			new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
			{
				UseStandardExit = false,
				MainInvokes = new ProfessionSkillController.OnceProfessionSkillInvoke[]
				{
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.5f, ProfessionSkillController.ProfessionSkillProcess.CloseAnimAndEffect.ToInt(), null),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.5f, ProfessionSkillController.ProfessionSkillProcess.OnclickSkillCallBack.ToInt(), null),
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.01f, ProfessionSkillController.ProfessionSkillProcess.RecoveryViewToOrigin.ToInt(), null)
				}
			}
		},
		{
			new ValueTuple<int, int>(14, 0),
			ProfessionSkillController.PropertyChangedInvokes
		},
		{
			new ValueTuple<int, int>(14, 1),
			null
		},
		{
			new ValueTuple<int, int>(14, 2),
			null
		},
		{
			new ValueTuple<int, int>(14, 3),
			null
		},
		{
			new ValueTuple<int, int>(15, 0),
			null
		},
		{
			new ValueTuple<int, int>(15, 1),
			new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
			{
				TimeScale = 1.75f
			}
		},
		{
			new ValueTuple<int, int>(15, 2),
			new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
			{
				TimeScale = 3f
			}
		},
		{
			new ValueTuple<int, int>(16, 0),
			null
		},
		{
			new ValueTuple<int, int>(16, 1),
			null
		},
		{
			new ValueTuple<int, int>(16, 2),
			null
		},
		{
			new ValueTuple<int, int>(16, 3),
			new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
			{
				TimeScale = 2f
			}
		},
		{
			new ValueTuple<int, int>(17, 0),
			new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
			{
				TimeScale = 1.75f
			}
		},
		{
			new ValueTuple<int, int>(17, 1),
			null
		},
		{
			new ValueTuple<int, int>(17, 2),
			new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
			{
				MainInvokes = new ProfessionSkillController.OnceProfessionSkillInvoke[]
				{
					new ProfessionSkillController.OnceProfessionSkillInvoke(0.01f, ProfessionSkillController.ProfessionSkillProcess.ShowProfessionIllustrationAndSkill.ToInt(), null)
				}
			}
		},
		{
			new ValueTuple<int, int>(17, 3),
			new ProfessionSkillController.ProfessionSkillInvokeConfigContainer
			{
				TimeScale = 2.5f
			}
		}
	};

	// Token: 0x020011F8 RID: 4600
	public enum ProfessionSkillProcess
	{
		// Token: 0x040098E8 RID: 39144
		RecoveryViewToCenterAndNormal,
		// Token: 0x040098E9 RID: 39145
		OpenProfessionSkillUI,
		// Token: 0x040098EA RID: 39146
		ShowProfessionIllustrationAndSkill,
		// Token: 0x040098EB RID: 39147
		PlaySkillAnimAndShowEffect,
		// Token: 0x040098EC RID: 39148
		AdjustMapBlockLightState,
		// Token: 0x040098ED RID: 39149
		ShowPropertyChange,
		// Token: 0x040098EE RID: 39150
		ShowTeammateUI,
		// Token: 0x040098EF RID: 39151
		ShowTravelerUI,
		// Token: 0x040098F0 RID: 39152
		PlaySkillSound,
		// Token: 0x040098F1 RID: 39153
		CloseAnimAndEffect,
		// Token: 0x040098F2 RID: 39154
		OnclickSkillCallBack,
		// Token: 0x040098F3 RID: 39155
		InvokeSomeFixMethod,
		// Token: 0x040098F4 RID: 39156
		AdjustMapBlockStateToBeggarSkill3,
		// Token: 0x040098F5 RID: 39157
		MoveCameraToRegularMapBlock,
		// Token: 0x040098F6 RID: 39158
		RecoveryViewToOrigin,
		// Token: 0x040098F7 RID: 39159
		RecoveryViewToCenter,
		// Token: 0x040098F8 RID: 39160
		TeleportMoveStart,
		// Token: 0x040098F9 RID: 39161
		TeleportMoveEnd
	}

	// Token: 0x020011F9 RID: 4601
	// (Invoke) Token: 0x0600C467 RID: 50279
	public delegate IEnumerable<ProfessionSkillController.OnceProfessionSkillInvoke> InvokeParser(ProfessionData professionData, ProfessionSkillController.OnceProfessionSkillInvoke[] invokes);

	// Token: 0x020011FA RID: 4602
	public class OnceProfessionSkillInvoke
	{
		// Token: 0x0600C46A RID: 50282 RVA: 0x0057B6DC File Offset: 0x005798DC
		public OnceProfessionSkillInvoke(float delay, int methodIndex, List<object> parameters)
		{
			this.Delay = delay;
			this.MethodIndex = methodIndex;
			this.Parameters = parameters;
		}

		// Token: 0x040098FA RID: 39162
		public float Delay;

		// Token: 0x040098FB RID: 39163
		public int MethodIndex;

		// Token: 0x040098FC RID: 39164
		public List<object> Parameters;
	}

	// Token: 0x020011FB RID: 4603
	public class ProfessionSkillInvokeConfigContainer
	{
		// Token: 0x040098FD RID: 39165
		public bool UseStandardEnter = true;

		// Token: 0x040098FE RID: 39166
		public bool UseStandardExit = true;

		// Token: 0x040098FF RID: 39167
		public float CloseDelay = -1f;

		// Token: 0x04009900 RID: 39168
		public ProfessionSkillController.OnceProfessionSkillInvoke[] BeforeEnter;

		// Token: 0x04009901 RID: 39169
		public ProfessionSkillController.OnceProfessionSkillInvoke[] MainInvokes;

		// Token: 0x04009902 RID: 39170
		public ProfessionSkillController.OnceProfessionSkillInvoke[] AfterExit;

		// Token: 0x04009903 RID: 39171
		public ProfessionSkillController.InvokeParser MainInvokeParser = ProfessionSkillController.DefaultParser;

		// Token: 0x04009904 RID: 39172
		public float TimeScale = 1f;
	}
}
