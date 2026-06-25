using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Game.Views.Adventure;
using GameData.Adventure;
using GameData.Adventure.Editor;
using TMPro;
using UnityEngine;

// Token: 0x02000186 RID: 390
public class AdventureEditorVolume : MonoBehaviour, IAdventureEditorBlackBoardElement, IAdventureBlackBoardElement<EAdventureEditType>
{
	// Token: 0x17000271 RID: 625
	// (get) Token: 0x06001607 RID: 5639 RVA: 0x000889D1 File Offset: 0x00086BD1
	// (set) Token: 0x06001608 RID: 5640 RVA: 0x000889D9 File Offset: 0x00086BD9
	[TupleElementNames(new string[]
	{
		"X",
		"Y"
	})]
	public ValueTuple<int, int> VolumeCoord { [return: TupleElementNames(new string[]
	{
		"X",
		"Y"
	})] get; [param: TupleElementNames(new string[]
	{
		"X",
		"Y"
	})] private set; }

	// Token: 0x06001609 RID: 5641 RVA: 0x000889E2 File Offset: 0x00086BE2
	private void OnEnable()
	{
		AdventureEditorKit.BlackBoard.Register(this);
	}

	// Token: 0x0600160A RID: 5642 RVA: 0x000889F1 File Offset: 0x00086BF1
	private void OnDisable()
	{
		AdventureEditorKit.BlackBoard.Unregister(this);
	}

	// Token: 0x17000272 RID: 626
	// (get) Token: 0x0600160B RID: 5643 RVA: 0x00088A00 File Offset: 0x00086C00
	bool IAdventureBlackBoardElement<EAdventureEditType>.LoadOnRegister
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600160C RID: 5644 RVA: 0x00088A04 File Offset: 0x00086C04
	void IAdventureBlackBoardElement<EAdventureEditType>.Load(EAdventureEditType editType)
	{
		bool flag = editType.Contains(EAdventureEditType.BlockProperties) || editType.Contains(EAdventureEditType.Groups);
		if (flag)
		{
			this.CalculateAndApplyHeight();
		}
		bool flag2 = editType.Contains(EAdventureEditType.All) || editType.Contains(EAdventureEditType.BlockViewMode);
		if (flag2)
		{
			this.RefreshLightingGridIndex(new AdventureBlockIndex(this.VolumeCoord.Item1, this.VolumeCoord.Item2, AdventureBlockIndex.CenterI));
		}
	}

	// Token: 0x0600160D RID: 5645 RVA: 0x00088A70 File Offset: 0x00086C70
	public void Set(int x, int y)
	{
		this.VolumeCoord = new ValueTuple<int, int>(x, y);
		bool flag = this.coordText != null;
		if (flag)
		{
			this.coordText.text = string.Format("({0},{1})", x, y);
		}
		bool needActive = !base.gameObject.activeSelf;
		bool flag2 = needActive;
		if (flag2)
		{
			base.gameObject.SetActive(true);
		}
		this.CalculateAndApplyHeight();
		this.RefreshLightingGridIndex(new AdventureBlockIndex(x, y, AdventureBlockIndex.CenterI));
	}

	// Token: 0x0600160E RID: 5646 RVA: 0x00088AFC File Offset: 0x00086CFC
	public void SetVolumeRealFullHeight(float value)
	{
		bool flag = this.volumeController != null;
		if (flag)
		{
			this.volumeController.VolumeRealFullHeight = value;
		}
	}

	// Token: 0x0600160F RID: 5647 RVA: 0x00088B28 File Offset: 0x00086D28
	public void RefreshLightingGridIndex(AdventureBlockIndex index)
	{
		bool flag = this.volumeController != null;
		if (flag)
		{
			this.volumeController.SetGridIndex(index);
		}
	}

	// Token: 0x06001610 RID: 5648 RVA: 0x00088B54 File Offset: 0x00086D54
	private void CalculateAndApplyHeight()
	{
		IReadOnlyList<AdventureBlockSnapshot> currentBlocks = AdventureEditorKit.BlackBoard.CurrentGroupBlocks;
		bool flag = currentBlocks == null;
		if (!flag)
		{
			float[] heights = AdventureHeightCalculator.ExtractHeights<AdventureBlockSnapshot>(currentBlocks, this.VolumeCoord.Item1, this.VolumeCoord.Item2, (AdventureBlockSnapshot b) => b.Index, (AdventureBlockSnapshot b) => b.Height);
			AdventureHeightCalculator.HeightResult heightResult = AdventureHeightCalculator.Calculate(heights);
			bool flag2 = this.volumeController != null;
			if (flag2)
			{
				this.volumeController.VolumeHeight = heightResult.VolumeHeight;
			}
		}
	}

	// Token: 0x04001208 RID: 4616
	[SerializeField]
	private BlockVolumeController volumeController;

	// Token: 0x04001209 RID: 4617
	[SerializeField]
	private TextMeshProUGUI coordText;
}
