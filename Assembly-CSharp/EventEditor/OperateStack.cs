using System;
using System.Collections.Generic;
using FrameWork;

namespace EventEditor
{
	// Token: 0x0200063A RID: 1594
	public class OperateStack
	{
		// Token: 0x17000960 RID: 2400
		// (get) Token: 0x06004B51 RID: 19281 RVA: 0x00236AAF File Offset: 0x00234CAF
		public bool CanRedo
		{
			get
			{
				return this._undoStack.Count > 0;
			}
		}

		// Token: 0x17000961 RID: 2401
		// (get) Token: 0x06004B52 RID: 19282 RVA: 0x00236ABF File Offset: 0x00234CBF
		public bool CanUndo
		{
			get
			{
				return this._doneStack.Count > 0;
			}
		}

		// Token: 0x06004B53 RID: 19283 RVA: 0x00236ACF File Offset: 0x00234CCF
		public void Clear()
		{
			this._doneStack.Clear();
			this._undoStack.Clear();
		}

		// Token: 0x06004B54 RID: 19284 RVA: 0x00236AEA File Offset: 0x00234CEA
		public OperateStack(byte maxSize)
		{
			this._maxStepSize = maxSize;
			this._doneStack = new Stack<OperateCommand>();
			this._undoStack = new Stack<OperateCommand>();
		}

		// Token: 0x06004B55 RID: 19285 RVA: 0x00236B14 File Offset: 0x00234D14
		public void Execute(OperateCommand cmd, bool doIt = true)
		{
			bool flag = cmd == null;
			if (!flag)
			{
				if (doIt)
				{
					Action @do = cmd.Do;
					if (@do != null)
					{
						@do();
					}
				}
				bool flag2 = this._doneStack.Count >= (int)this._maxStepSize;
				if (flag2)
				{
					OperateCommand[] cmdArray = this._doneStack.ToArray();
					this._doneStack.Clear();
					for (int i = cmdArray.Length / 2; i >= 0; i--)
					{
						this._doneStack.Push(cmdArray[i]);
					}
				}
				this._doneStack.Push(cmd);
				this._undoStack.Clear();
			}
		}

		// Token: 0x06004B56 RID: 19286 RVA: 0x00236BC4 File Offset: 0x00234DC4
		public void Undo()
		{
			bool flag = this._doneStack.Count > 0;
			if (flag)
			{
				OperateCommand cmd = this._doneStack.Pop();
				Action undo = cmd.Undo;
				if (undo != null)
				{
					undo();
				}
				this._undoStack.Push(cmd);
			}
		}

		// Token: 0x06004B57 RID: 19287 RVA: 0x00236C14 File Offset: 0x00234E14
		public void Redo()
		{
			bool flag = this._undoStack.Count > 0;
			if (flag)
			{
				OperateCommand cmd = this._undoStack.Pop();
				Action @do = cmd.Do;
				if (@do != null)
				{
					@do();
				}
				this._doneStack.Push(cmd);
			}
		}

		// Token: 0x06004B58 RID: 19288 RVA: 0x00236C64 File Offset: 0x00234E64
		public void RemoveOperations(Predicate<OperateCommand> removeMatch)
		{
			Stack<OperateCommand> cacheStack = EasyPool.Get<Stack<OperateCommand>>();
			foreach (object obj in this._doneStack)
			{
				OperateCommand cmd = obj as OperateCommand;
				bool flag = !removeMatch(cmd);
				if (flag)
				{
					cacheStack.Push(cmd);
				}
			}
			this._doneStack.Clear();
			foreach (object obj2 in cacheStack)
			{
				OperateCommand cmd2 = obj2 as OperateCommand;
				this._doneStack.Push(cmd2);
			}
			cacheStack.Clear();
			foreach (object obj3 in this._undoStack)
			{
				OperateCommand cmd3 = obj3 as OperateCommand;
				bool flag2 = !removeMatch(cmd3);
				if (flag2)
				{
					cacheStack.Push(cmd3);
				}
			}
			this._undoStack.Clear();
			foreach (object obj4 in cacheStack)
			{
				OperateCommand cmd4 = obj4 as OperateCommand;
				this._undoStack.Push(cmd4);
			}
			EasyPool.Free<Stack<OperateCommand>>(cacheStack);
		}

		// Token: 0x04003455 RID: 13397
		private Stack<OperateCommand> _doneStack;

		// Token: 0x04003456 RID: 13398
		private Stack<OperateCommand> _undoStack;

		// Token: 0x04003457 RID: 13399
		private byte _maxStepSize;
	}
}
