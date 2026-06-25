using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Profiling;

// Token: 0x02000028 RID: 40
public class FrameSplitActionQueue
{
	// Token: 0x17000036 RID: 54
	// (get) Token: 0x06000162 RID: 354 RVA: 0x00009A36 File Offset: 0x00007C36
	// (set) Token: 0x06000163 RID: 355 RVA: 0x00009A3E File Offset: 0x00007C3E
	public int MaxActionCountPerFrame { get; set; } = 100;

	// Token: 0x17000037 RID: 55
	// (get) Token: 0x06000164 RID: 356 RVA: 0x00009A47 File Offset: 0x00007C47
	// (set) Token: 0x06000165 RID: 357 RVA: 0x00009A4F File Offset: 0x00007C4F
	public int MaxActionExecuteDurationPerFrameMs { get; set; } = 12;

	// Token: 0x17000038 RID: 56
	// (get) Token: 0x06000166 RID: 358 RVA: 0x00009A58 File Offset: 0x00007C58
	public int Count
	{
		get
		{
			return this._actionQueue.Count;
		}
	}

	// Token: 0x06000167 RID: 359 RVA: 0x00009A68 File Offset: 0x00007C68
	private LinkedListNode<ValueTuple<FrameSplitActionQueue.Handle, Action>> GetNode()
	{
		bool flag = FrameSplitActionQueue._reusableNodes.Count > 0;
		LinkedListNode<ValueTuple<FrameSplitActionQueue.Handle, Action>> result;
		if (flag)
		{
			LinkedListNode<ValueTuple<FrameSplitActionQueue.Handle, Action>> node = FrameSplitActionQueue._reusableNodes.Dequeue();
			result = node;
		}
		else
		{
			result = new LinkedListNode<ValueTuple<FrameSplitActionQueue.Handle, Action>>(default(ValueTuple<FrameSplitActionQueue.Handle, Action>));
		}
		return result;
	}

	// Token: 0x06000168 RID: 360 RVA: 0x00009AAC File Offset: 0x00007CAC
	public FrameSplitActionQueue.Handle Enqueue(Action action)
	{
		LinkedListNode<ValueTuple<FrameSplitActionQueue.Handle, Action>> node = this.GetNode();
		FrameSplitActionQueue.Handle handle = new FrameSplitActionQueue.Handle(this, node);
		node.Value = new ValueTuple<FrameSplitActionQueue.Handle, Action>(handle, action);
		this._actionQueue.AddLast(node);
		return handle;
	}

	// Token: 0x06000169 RID: 361 RVA: 0x00009AEC File Offset: 0x00007CEC
	private void RemoveNode(LinkedListNode<ValueTuple<FrameSplitActionQueue.Handle, Action>> node)
	{
		this._actionQueue.Remove(node);
		bool flag = FrameSplitActionQueue._reusableNodes.Count < 100;
		if (flag)
		{
			FrameSplitActionQueue._reusableNodes.Enqueue(node);
		}
	}

	// Token: 0x0600016A RID: 362 RVA: 0x00009B28 File Offset: 0x00007D28
	public void Update()
	{
		bool flag = this._actionQueue.Count == 0;
		if (!flag)
		{
			Profiler.BeginSample("FrameSplitActionQueue.Update");
			TimeSpan maxDuration = TimeSpan.FromMilliseconds((double)this.MaxActionExecuteDurationPerFrameMs);
			DateTime startTime = DateTime.UtcNow;
			int executedCount = 0;
			while (executedCount < this.MaxActionCountPerFrame)
			{
				LinkedListNode<ValueTuple<FrameSplitActionQueue.Handle, Action>> node = this._actionQueue.First;
				bool flag2 = node == null;
				if (flag2)
				{
					break;
				}
				try
				{
					node.Value.Item2();
				}
				catch (Exception ex)
				{
					GLog.TagError("FrameSplitActionQueue", string.Format("Exception while executing action in FrameSplitActionQueue: {0}", ex), Array.Empty<object>());
				}
				finally
				{
					ValueTuple<FrameSplitActionQueue.Handle, Action> value = node.Value;
					bool isValid = value.Item1.IsValid;
					if (isValid)
					{
						value = node.Value;
						value.Item1.Dispose();
					}
					else
					{
						this.RemoveNode(node);
					}
				}
				executedCount++;
				bool flag3 = DateTime.UtcNow - startTime >= maxDuration;
				if (flag3)
				{
					break;
				}
			}
			Profiler.EndSample();
		}
	}

	// Token: 0x040000B7 RID: 183
	private static Queue<LinkedListNode<ValueTuple<FrameSplitActionQueue.Handle, Action>>> _reusableNodes = new Queue<LinkedListNode<ValueTuple<FrameSplitActionQueue.Handle, Action>>>();

	// Token: 0x040000B8 RID: 184
	private const int MaxReusableNodeCount = 100;

	// Token: 0x040000B9 RID: 185
	[TupleElementNames(new string[]
	{
		"handle",
		"callback"
	})]
	private LinkedList<ValueTuple<FrameSplitActionQueue.Handle, Action>> _actionQueue = new LinkedList<ValueTuple<FrameSplitActionQueue.Handle, Action>>();

	// Token: 0x0200109E RID: 4254
	public struct Handle : IDisposable
	{
		// Token: 0x17001591 RID: 5521
		// (get) Token: 0x0600BFF3 RID: 49139 RVA: 0x0056A568 File Offset: 0x00568768
		// (set) Token: 0x0600BFF4 RID: 49140 RVA: 0x0056A570 File Offset: 0x00568770
		public FrameSplitActionQueue Queue { readonly get; private set; }

		// Token: 0x17001592 RID: 5522
		// (get) Token: 0x0600BFF5 RID: 49141 RVA: 0x0056A579 File Offset: 0x00568779
		public bool IsValid
		{
			get
			{
				return this.Queue != null && this.callbackNode != null;
			}
		}

		// Token: 0x0600BFF6 RID: 49142 RVA: 0x0056A58F File Offset: 0x0056878F
		public Handle(FrameSplitActionQueue queue, LinkedListNode<ValueTuple<FrameSplitActionQueue.Handle, Action>> node)
		{
			this.Queue = queue;
			this.callbackNode = node;
		}

		// Token: 0x0600BFF7 RID: 49143 RVA: 0x0056A5A4 File Offset: 0x005687A4
		public void Dispose()
		{
			bool isValid = this.IsValid;
			if (isValid)
			{
				this.Queue.RemoveNode(this.callbackNode);
			}
			this.callbackNode = null;
			this.Queue = null;
		}

		// Token: 0x0600BFF8 RID: 49144 RVA: 0x0056A5E0 File Offset: 0x005687E0
		public override int GetHashCode()
		{
			return this.IsValid ? this.callbackNode.Value.Item2.GetHashCode() : 0;
		}

		// Token: 0x040093D7 RID: 37847
		[TupleElementNames(new string[]
		{
			null,
			"callback"
		})]
		private LinkedListNode<ValueTuple<FrameSplitActionQueue.Handle, Action>> callbackNode;
	}
}
