using System;
using System.Collections.Generic;
using GameData.Adventure;
using GameData.Adventure.Editor;
using GameData.Utilities;

// Token: 0x0200016B RID: 363
public class AdventureEditorBlackBoard : AdventureBlackBoard<AdventureSnapshot, EAdventureEditType>
{
	// Token: 0x1700023C RID: 572
	// (get) Token: 0x0600143E RID: 5182 RVA: 0x0007E894 File Offset: 0x0007CA94
	// (set) Token: 0x0600143F RID: 5183 RVA: 0x0007E89C File Offset: 0x0007CA9C
	public EBlockViewMode ViewMode { get; private set; }

	// Token: 0x1700023D RID: 573
	// (get) Token: 0x06001440 RID: 5184 RVA: 0x0007E8A5 File Offset: 0x0007CAA5
	// (set) Token: 0x06001441 RID: 5185 RVA: 0x0007E8AD File Offset: 0x0007CAAD
	public int CurrentGroupIndex { get; set; }

	// Token: 0x1700023E RID: 574
	// (get) Token: 0x06001442 RID: 5186 RVA: 0x0007E8B6 File Offset: 0x0007CAB6
	protected override EAdventureEditType TypeAll
	{
		get
		{
			return EAdventureEditType.All;
		}
	}

	// Token: 0x06001443 RID: 5187 RVA: 0x0007E8B9 File Offset: 0x0007CAB9
	protected override void Initialize(AdventureSnapshot snapshot)
	{
		this.ViewMode = EBlockViewMode.Default;
		this.CurrentGroupIndex = 0;
		snapshot.FixBlocks();
		snapshot.Parameters.Add(AdventureSnapshotConstants.ViewTypeDefaultNear);
		snapshot.Parameters.Add(AdventureSnapshotConstants.ViewTypeDefaultFar);
	}

	// Token: 0x06001444 RID: 5188 RVA: 0x0007E8F8 File Offset: 0x0007CAF8
	public void SwitchGroup(int groupIndex)
	{
		bool flag = groupIndex < 0 || groupIndex >= base.Editing.Groups.Count;
		if (!flag)
		{
			bool flag2 = this.CurrentGroupIndex == groupIndex;
			if (!flag2)
			{
				base.Record(ValueChangedCommand.Create<int>(new Action<int>(this.SetCurrentGroupIndexAndReload), this.CurrentGroupIndex, groupIndex));
				this.SetCurrentGroupIndexAndReload(groupIndex);
			}
		}
	}

	// Token: 0x06001445 RID: 5189 RVA: 0x0007E95F File Offset: 0x0007CB5F
	private void SetCurrentGroupIndex(int index)
	{
		this.CurrentGroupIndex = index;
	}

	// Token: 0x06001446 RID: 5190 RVA: 0x0007E96A File Offset: 0x0007CB6A
	private void SetCurrentGroupIndexAndReload(int index)
	{
		this.SetCurrentGroupIndex(index);
		base.LoadAll(EAdventureEditType.Groups);
	}

	// Token: 0x06001447 RID: 5191 RVA: 0x0007E980 File Offset: 0x0007CB80
	public void MoveGroup(int fromIndex, int toIndex)
	{
		List<AdventureGroupSnapshot> groups = base.Editing.Groups;
		bool flag = fromIndex < 0 || fromIndex >= groups.Count || toIndex < 0 || toIndex >= groups.Count;
		if (!flag)
		{
			bool flag2 = fromIndex == toIndex;
			if (!flag2)
			{
				int prevIndex = this.CurrentGroupIndex;
				int newCurrentIndex = this.CalculateMovedGroupIndex(prevIndex, fromIndex, toIndex);
				base.MakeEditComposite(delegate(AdventureSnapshot snapshot)
				{
					AdventureGroupSnapshot item = snapshot.Groups[fromIndex];
					snapshot.Groups.RemoveAt(fromIndex);
					snapshot.Groups.Insert(toIndex, item);
					this.CurrentGroupIndex = newCurrentIndex;
				}, EAdventureEditType.Groups, ValueChangedCommand.Create<int>(new Action<int>(this.SetCurrentGroupIndex), prevIndex, newCurrentIndex));
			}
		}
	}

	// Token: 0x06001448 RID: 5192 RVA: 0x0007EA54 File Offset: 0x0007CC54
	private int CalculateMovedGroupIndex(int currentIndex, int fromIndex, int toIndex)
	{
		bool flag = currentIndex == fromIndex;
		int result;
		if (flag)
		{
			result = toIndex;
		}
		else
		{
			bool flag2 = currentIndex > fromIndex && currentIndex <= toIndex;
			if (flag2)
			{
				result = currentIndex - 1;
			}
			else
			{
				bool flag3 = currentIndex < fromIndex && currentIndex >= toIndex;
				if (flag3)
				{
					result = currentIndex + 1;
				}
				else
				{
					result = currentIndex;
				}
			}
		}
		return result;
	}

	// Token: 0x06001449 RID: 5193 RVA: 0x0007EAA4 File Offset: 0x0007CCA4
	public void ResetGroupIndex()
	{
		bool flag = this.CurrentGroupIndex == 0;
		if (!flag)
		{
			this.SetCurrentGroupIndexAndReload(0);
		}
	}

	// Token: 0x0600144A RID: 5194 RVA: 0x0007EACC File Offset: 0x0007CCCC
	public void CloneGroup(int index)
	{
		List<AdventureGroupSnapshot> groups = base.Editing.Groups;
		bool flag = index < 0 || index >= groups.Count;
		if (!flag)
		{
			int newIndex = index + 1;
			int prevIndex = this.CurrentGroupIndex;
			base.MakeEditComposite(delegate(AdventureSnapshot snapshot)
			{
				AdventureGroupSnapshot sourceGroup = snapshot.Groups[index];
				AdventureGroupSnapshot cloned = new AdventureGroupSnapshot
				{
					Weight = sourceGroup.Weight,
					Comment = sourceGroup.Comment + "_clone"
				};
				foreach (AdventureBlockSnapshot block in sourceGroup.Blocks)
				{
					AdventureBlockSnapshot clonedBlock = new AdventureBlockSnapshot
					{
						Index = block.Index,
						BlockType = block.BlockType,
						Height = block.Height,
						TimeCost = block.TimeCost,
						EntryPriority = block.EntryPriority,
						Icon = block.Icon,
						InCloud = block.InCloud
					};
					clonedBlock.Decorates.ClearAndAddRange(block.Decorates);
					clonedBlock.ElementCoreIds.ClearAndAddRange(block.ElementCoreIds);
					clonedBlock.FakeElementCoreIds.ClearAndAddRange(block.FakeElementCoreIds);
					clonedBlock.GroupIds.ClearAndAddRange(block.GroupIds);
					cloned.Blocks.Add(clonedBlock);
				}
				snapshot.Groups.Insert(newIndex, cloned);
				this.CurrentGroupIndex = newIndex;
			}, EAdventureEditType.Groups, ValueChangedCommand.Create<int>(new Action<int>(this.SetCurrentGroupIndex), prevIndex, newIndex));
		}
	}

	// Token: 0x0600144B RID: 5195 RVA: 0x0007EB60 File Offset: 0x0007CD60
	public void AddGroup()
	{
		int newIndex = base.Editing.Groups.Count;
		int size = base.Editing.Size;
		int prevIndex = this.CurrentGroupIndex;
		base.MakeEditComposite(delegate(AdventureSnapshot snapshot)
		{
			AdventureGroupSnapshot newGroup = new AdventureGroupSnapshot
			{
				Weight = 1U
			};
			foreach (AdventureBlockIndex index in AdventureBlockIndex.GetIndexes(size))
			{
				newGroup.Blocks.Add(new AdventureBlockSnapshot
				{
					Index = index
				});
			}
			snapshot.Groups.Add(newGroup);
			this.CurrentGroupIndex = newIndex;
		}, EAdventureEditType.Groups, ValueChangedCommand.Create<int>(new Action<int>(this.SetCurrentGroupIndex), prevIndex, newIndex));
	}

	// Token: 0x0600144C RID: 5196 RVA: 0x0007EBD8 File Offset: 0x0007CDD8
	public void DeleteGroup(int deleteIndex)
	{
		List<AdventureGroupSnapshot> groups = base.Editing.Groups;
		bool flag = groups.Count <= 1 || deleteIndex < 0 || deleteIndex >= groups.Count;
		if (!flag)
		{
			int targetIndex = (deleteIndex >= groups.Count - 1) ? (groups.Count - 2) : deleteIndex;
			int prevIndex = this.CurrentGroupIndex;
			base.MakeEditComposite(delegate(AdventureSnapshot snapshot)
			{
				snapshot.Groups.RemoveAt(deleteIndex);
				this.CurrentGroupIndex = targetIndex;
			}, EAdventureEditType.Groups, ValueChangedCommand.Create<int>(new Action<int>(this.SetCurrentGroupIndex), prevIndex, targetIndex));
		}
	}

	// Token: 0x1700023F RID: 575
	// (get) Token: 0x0600144D RID: 5197 RVA: 0x0007EC8C File Offset: 0x0007CE8C
	public int GroupCount
	{
		get
		{
			return base.Editing.Groups.Count;
		}
	}

	// Token: 0x17000240 RID: 576
	// (get) Token: 0x0600144E RID: 5198 RVA: 0x0007EC9E File Offset: 0x0007CE9E
	public AdventureGroupSnapshot CurrentGroup
	{
		get
		{
			return (base.Editing.Groups.Count > this.CurrentGroupIndex) ? base.Editing.Groups[this.CurrentGroupIndex] : null;
		}
	}

	// Token: 0x17000241 RID: 577
	// (get) Token: 0x0600144F RID: 5199 RVA: 0x0007ECD1 File Offset: 0x0007CED1
	public IReadOnlyList<AdventureBlockSnapshot> CurrentGroupBlocks
	{
		get
		{
			AdventureGroupSnapshot currentGroup = this.CurrentGroup;
			return (currentGroup != null) ? currentGroup.Blocks : null;
		}
	}

	// Token: 0x06001450 RID: 5200 RVA: 0x0007ECE5 File Offset: 0x0007CEE5
	public void ChangeViewMode(EBlockViewMode mode)
	{
		base.Record(ValueChangedCommand.Create<EBlockViewMode>(new Action<EBlockViewMode>(this.ReloadView), this.ViewMode, mode));
		this.ReloadView(mode);
	}

	// Token: 0x06001451 RID: 5201 RVA: 0x0007ED0F File Offset: 0x0007CF0F
	private void ReloadView(EBlockViewMode mode)
	{
		this.ViewMode = mode;
		base.LoadAll(EAdventureEditType.BlockViewMode);
	}
}
