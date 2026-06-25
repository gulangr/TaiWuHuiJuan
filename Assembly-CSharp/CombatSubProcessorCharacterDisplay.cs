using System;
using GameData.Serializer;
using GameData.Utilities;

// Token: 0x0200011B RID: 283
public class CombatSubProcessorCharacterDisplay : CombatSubProcessor, ICombatNotifySubProcessor
{
	// Token: 0x1700011E RID: 286
	// (get) Token: 0x06000A64 RID: 2660 RVA: 0x00043B25 File Offset: 0x00041D25
	public int CharacterId { get; }

	// Token: 0x1700011F RID: 287
	// (get) Token: 0x06000A65 RID: 2661 RVA: 0x00043B2D File Offset: 0x00041D2D
	ulong ICombatNotifySubProcessor.SubId0
	{
		get
		{
			return (ulong)((long)this.CharacterId);
		}
	}

	// Token: 0x06000A66 RID: 2662 RVA: 0x00043B36 File Offset: 0x00041D36
	public CombatSubProcessorCharacterDisplay(int characterId)
	{
		this.CharacterId = characterId;
		base.Setup();
	}

	// Token: 0x06000A67 RID: 2663 RVA: 0x00043B4E File Offset: 0x00041D4E
	[CombatNotifyData(8, 10, 104U)]
	private void HandlerDataVisible(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.Visible);
		OnCharacterDataChangedEvent onCharacterVisibleChanged = CombatSubProcessor.Model.OnCharacterVisibleChanged;
		if (onCharacterVisibleChanged != null)
		{
			onCharacterVisibleChanged(this.CharacterId);
		}
	}

	// Token: 0x06000A68 RID: 2664 RVA: 0x00043B7B File Offset: 0x00041D7B
	[CombatNotifyData(8, 10, 9U)]
	private void HandlerDataCurrentPosition(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.CurrentPosition);
	}

	// Token: 0x06000A69 RID: 2665 RVA: 0x00043B8C File Offset: 0x00041D8C
	[CombatNotifyData(8, 10, 10U)]
	private void HandlerDataDisplayPosition(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.DisplayPosition);
	}

	// Token: 0x06000A6A RID: 2666 RVA: 0x00043B9D File Offset: 0x00041D9D
	[CombatNotifyData(8, 10, 83U)]
	private void HandlerDataAnimationToLoop(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.AnimationToLoop);
	}

	// Token: 0x06000A6B RID: 2667 RVA: 0x00043BAE File Offset: 0x00041DAE
	[CombatNotifyData(8, 10, 84U)]
	private void HandlerDataAnimationToPlayOnce(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.AnimationToPlayOnce);
		OnCharacterDataChangedEvent onAnimationToPlayOnceChanged = CombatSubProcessor.Model.OnAnimationToPlayOnceChanged;
		if (onAnimationToPlayOnceChanged != null)
		{
			onAnimationToPlayOnceChanged(this.CharacterId);
		}
	}

	// Token: 0x06000A6C RID: 2668 RVA: 0x00043BDB File Offset: 0x00041DDB
	[CombatNotifyData(8, 10, 89U)]
	private void HandlerDataAnimationTimeScale(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.AnimationTimeScale);
	}

	// Token: 0x06000A6D RID: 2669 RVA: 0x00043BEC File Offset: 0x00041DEC
	[CombatNotifyData(8, 10, 85U)]
	private void HandlerDataParticleToPlay(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.ParticleToPlay);
		OnCharacterDataChangedEvent onParticleToPlayChanged = CombatSubProcessor.Model.OnParticleToPlayChanged;
		if (onParticleToPlayChanged != null)
		{
			onParticleToPlayChanged(this.CharacterId);
		}
	}

	// Token: 0x06000A6E RID: 2670 RVA: 0x00043C19 File Offset: 0x00041E19
	[CombatNotifyData(8, 10, 86U)]
	private void HandlerDataParticleToLoop(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.ParticleToLoop);
		OnCharacterDataChangedEvent onParticleToLoopChanged = CombatSubProcessor.Model.OnParticleToLoopChanged;
		if (onParticleToLoopChanged != null)
		{
			onParticleToLoopChanged(this.CharacterId);
		}
	}

	// Token: 0x06000A6F RID: 2671 RVA: 0x00043C46 File Offset: 0x00041E46
	[CombatNotifyData(8, 10, 116U)]
	private void HandlerDataParticleToLoopByCombatSkill(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.ParticleToLoopByCombatSkill);
		OnCharacterDataChangedEvent onParticleToLoopByCombatSkillChanged = CombatSubProcessor.Model.OnParticleToLoopByCombatSkillChanged;
		if (onParticleToLoopByCombatSkillChanged != null)
		{
			onParticleToLoopByCombatSkillChanged(this.CharacterId);
		}
	}

	// Token: 0x06000A70 RID: 2672 RVA: 0x00043C73 File Offset: 0x00041E73
	[CombatNotifyData(8, 10, 91U)]
	private void HandlerDataAttackSoundToPlay(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.AttackSoundToPlay);
	}

	// Token: 0x06000A71 RID: 2673 RVA: 0x00043C84 File Offset: 0x00041E84
	[CombatNotifyData(8, 10, 93U)]
	private void HandlerDataHitSoundToPlay(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.HitSoundToPlay);
	}

	// Token: 0x06000A72 RID: 2674 RVA: 0x00043C95 File Offset: 0x00041E95
	[CombatNotifyData(8, 10, 95U)]
	private void HandlerDataWhooshSoundToPlay(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.WhooshSoundToPlay);
	}

	// Token: 0x06000A73 RID: 2675 RVA: 0x00043CA6 File Offset: 0x00041EA6
	[CombatNotifyData(8, 10, 96U)]
	private void HandlerDataShockSoundToPlay(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.ShockSoundToPlay);
	}

	// Token: 0x06000A74 RID: 2676 RVA: 0x00043CB7 File Offset: 0x00041EB7
	[CombatNotifyData(8, 10, 97U)]
	private void HandlerDataStepSoundToPlay(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.StepSoundToPlay);
	}

	// Token: 0x06000A75 RID: 2677 RVA: 0x00043CC8 File Offset: 0x00041EC8
	[CombatNotifyData(8, 10, 94U)]
	private void HandlerDataArmorHitSoundToPlay(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.ArmorHitSoundToPlay);
	}

	// Token: 0x06000A76 RID: 2678 RVA: 0x00043CD9 File Offset: 0x00041ED9
	[CombatNotifyData(8, 10, 92U)]
	private void HandlerDataSkillSoundToPlay(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.SkillSoundToPlay);
	}

	// Token: 0x06000A77 RID: 2679 RVA: 0x00043CEA File Offset: 0x00041EEA
	[CombatNotifyData(8, 10, 98U)]
	private void HandlerDataDieSoundToPlay(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.DieSoundToPlay);
	}

	// Token: 0x06000A78 RID: 2680 RVA: 0x00043CFB File Offset: 0x00041EFB
	[CombatNotifyData(8, 10, 99U)]
	private void HandlerDataSoundToLoop(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.SoundToLoop);
		OnCharacterDataChangedEvent onSoundToLoopChanged = CombatSubProcessor.Model.OnSoundToLoopChanged;
		if (onSoundToLoopChanged != null)
		{
			onSoundToLoopChanged(this.CharacterId);
		}
	}

	// Token: 0x04000D52 RID: 3410
	private const ushort CombatCharacterDict = 10;

	// Token: 0x04000D53 RID: 3411
	public bool Visible;

	// Token: 0x04000D54 RID: 3412
	public int CurrentPosition;

	// Token: 0x04000D55 RID: 3413
	public int DisplayPosition;

	// Token: 0x04000D56 RID: 3414
	public string AnimationToLoop;

	// Token: 0x04000D57 RID: 3415
	public string AnimationToPlayOnce;

	// Token: 0x04000D58 RID: 3416
	public float AnimationTimeScale;

	// Token: 0x04000D59 RID: 3417
	public string ParticleToPlay;

	// Token: 0x04000D5A RID: 3418
	public string ParticleToLoop;

	// Token: 0x04000D5B RID: 3419
	public string ParticleToLoopByCombatSkill;

	// Token: 0x04000D5C RID: 3420
	public string AttackSoundToPlay;

	// Token: 0x04000D5D RID: 3421
	public string HitSoundToPlay;

	// Token: 0x04000D5E RID: 3422
	public string WhooshSoundToPlay;

	// Token: 0x04000D5F RID: 3423
	public string ShockSoundToPlay;

	// Token: 0x04000D60 RID: 3424
	public string StepSoundToPlay;

	// Token: 0x04000D61 RID: 3425
	public string ArmorHitSoundToPlay;

	// Token: 0x04000D62 RID: 3426
	public string SkillSoundToPlay;

	// Token: 0x04000D63 RID: 3427
	public string DieSoundToPlay;

	// Token: 0x04000D64 RID: 3428
	public string SoundToLoop;
}
