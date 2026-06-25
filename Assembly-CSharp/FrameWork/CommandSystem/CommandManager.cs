using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameData.Utilities;
using UnityEngine;

namespace FrameWork.CommandSystem
{
	// Token: 0x0200105A RID: 4186
	public class CommandManager : MonoBehaviour, ISingletonInit, IDisposable
	{
		// Token: 0x0600BEA7 RID: 48807 RVA: 0x005660D8 File Offset: 0x005642D8
		public static void AddCommandShowUI(EPriority priority, UIElement element, ArgumentBox argBox = null)
		{
			bool flag = argBox == null;
			if (flag)
			{
				CommandManager.AddCommand<CommandShowUI, UIElement>(priority, element);
			}
			else
			{
				CommandManager.AddCommand<CommandShowUIWithArgs, UIElement, ArgumentBox>(priority, element, argBox);
			}
		}

		// Token: 0x0600BEA8 RID: 48808 RVA: 0x00566104 File Offset: 0x00564304
		public static void AddCommandStackUI(EPriority priority, UIElement element, ArgumentBox argBox = null)
		{
			bool flag = argBox == null;
			if (flag)
			{
				CommandManager.AddCommand<CommandStackUI, UIElement>(priority, element);
			}
			else
			{
				CommandManager.AddCommand<CommandStackUIWithArgs, UIElement, ArgumentBox>(priority, element, argBox);
			}
		}

		// Token: 0x0600BEA9 RID: 48809 RVA: 0x00566130 File Offset: 0x00564330
		public static void AddCommandMethodCall(EPriority priority, ushort domain, ushort method, CallMethodRespHandler onResp, CallMethodSkipHandler onSkip = null)
		{
			ArgumentBox requestArgs = EasyPool.Get<ArgumentBox>();
			requestArgs.Set("domain", domain);
			requestArgs.Set("method", method);
			bool flag = onResp != null;
			if (flag)
			{
				requestArgs.SetObject("onResp", onResp);
			}
			bool flag2 = onSkip != null;
			if (flag2)
			{
				requestArgs.SetObject("onSkip", onSkip);
			}
			CommandManager.AddCommand<CommandCallMethod>(priority, requestArgs, true);
		}

		// Token: 0x0600BEAA RID: 48810 RVA: 0x00566194 File Offset: 0x00564394
		public static void AddCommandMethodCall<T>(EPriority priority, ushort domain, ushort method, T arg0, CallMethodRespHandler onResp, CallMethodSkipHandler onSkip = null)
		{
			ArgumentBox requestArgs = EasyPool.Get<ArgumentBox>();
			requestArgs.Set("domain", domain);
			requestArgs.Set("method", method);
			requestArgs.SetObject("arg0", arg0);
			bool flag = onResp != null;
			if (flag)
			{
				requestArgs.SetObject("onResp", onResp);
			}
			bool flag2 = onSkip != null;
			if (flag2)
			{
				requestArgs.SetObject("onSkip", onSkip);
			}
			CommandManager.AddCommand<CommandCallMethod<T>>(priority, requestArgs, true);
		}

		// Token: 0x0600BEAB RID: 48811 RVA: 0x0056620C File Offset: 0x0056440C
		public static void AddCommandMethodCall<T, T1>(EPriority priority, ushort domain, ushort method, T arg0, T1 arg1, CallMethodRespHandler onResp, CallMethodSkipHandler onSkip = null)
		{
			ArgumentBox requestArgs = EasyPool.Get<ArgumentBox>();
			requestArgs.Set("domain", domain);
			requestArgs.Set("method", method);
			requestArgs.SetObject("arg0", arg0);
			requestArgs.SetObject("arg1", arg1);
			bool flag = onResp != null;
			if (flag)
			{
				requestArgs.SetObject("onResp", onResp);
			}
			bool flag2 = onSkip != null;
			if (flag2)
			{
				requestArgs.SetObject("onSkip", onSkip);
			}
			CommandManager.AddCommand<CommandCallMethod<T, T1>>(priority, requestArgs, true);
		}

		// Token: 0x0600BEAC RID: 48812 RVA: 0x00566298 File Offset: 0x00564498
		public static void AddCommandMethodCall<T, T1, T2>(EPriority priority, ushort domain, ushort method, T arg0, T1 arg1, T2 arg2, CallMethodRespHandler onResp, CallMethodSkipHandler onSkip = null)
		{
			ArgumentBox requestArgs = EasyPool.Get<ArgumentBox>();
			requestArgs.Set("domain", domain);
			requestArgs.Set("method", method);
			requestArgs.SetObject("arg0", arg0);
			requestArgs.SetObject("arg1", arg1);
			requestArgs.SetObject("arg2", arg2);
			bool flag = onResp != null;
			if (flag)
			{
				requestArgs.SetObject("onResp", onResp);
			}
			bool flag2 = onSkip != null;
			if (flag2)
			{
				requestArgs.SetObject("onSkip", onSkip);
			}
			CommandManager.AddCommand<CommandCallMethod<T, T1, T2>>(priority, requestArgs, true);
		}

		// Token: 0x0600BEAD RID: 48813 RVA: 0x00566335 File Offset: 0x00564535
		public static void AddCommandAuto(EPriority priority, CmdExecuteDelegate execute, CmdGetDoneDelegate getDone)
		{
			CommandManager.AddCommand<CommandAuto, CmdExecuteDelegate, CmdGetDoneDelegate>(priority, execute, getDone);
		}

		// Token: 0x0600BEAE RID: 48814 RVA: 0x00566341 File Offset: 0x00564541
		public static void AddCommandWaitUntil(EPriority priority, CmdGetDoneDelegate getDone)
		{
			CommandManager.AddCommand<CommandWaitUntil, CmdGetDoneDelegate>(priority, getDone);
		}

		// Token: 0x0600BEAF RID: 48815 RVA: 0x0056634C File Offset: 0x0056454C
		public static void AddCommandWaitEvent(EPriority priority, Enum waitEvent, float timeout = 5f, ArgumentBox extraArgs = null, WaitEventExtraHandler extraHandler = null, WaitEventSimpleHandler simpleHandler = null, WaitEventCompleteHandler completeHandler = null)
		{
			ArgumentBox waitArgs = EasyPool.Get<ArgumentBox>();
			waitArgs.Set("event", waitEvent);
			waitArgs.Set("timeout", timeout);
			bool flag = extraArgs != null;
			if (flag)
			{
				waitArgs.SetObject("extraArgs", extraArgs);
			}
			bool flag2 = extraHandler != null;
			if (flag2)
			{
				waitArgs.SetObject("extraHandler", extraHandler);
			}
			bool flag3 = simpleHandler != null;
			if (flag3)
			{
				waitArgs.SetObject("simpleHandler", simpleHandler);
			}
			bool flag4 = completeHandler != null;
			if (flag4)
			{
				waitArgs.SetObject("completeHandler", completeHandler);
			}
			CommandManager.AddCommand<CommandWaitEvent>(priority, waitArgs, true);
		}

		// Token: 0x0600BEB0 RID: 48816 RVA: 0x005663E0 File Offset: 0x005645E0
		public static void AddCommand<TCommand>(EPriority priority, ArgumentBox argBox, bool autoFree = true) where TCommand : class, ICommand, ICollectable<ArgumentBox>, new()
		{
			CommandManager.AddCommand<TCommand, ArgumentBox>(priority, argBox);
			if (autoFree)
			{
				EasyPool.Free<ArgumentBox>(argBox);
			}
		}

		// Token: 0x0600BEB1 RID: 48817 RVA: 0x00566404 File Offset: 0x00564604
		public static void AddCommand<TCommand>(EPriority priority) where TCommand : class, ICommand, ICollectable, new()
		{
			TCommand command = EasyPool.Get<TCommand>();
			command.Pooled = true;
			command.Reset();
			CommandManager.AddCommand(command, priority);
		}

		// Token: 0x0600BEB2 RID: 48818 RVA: 0x00566440 File Offset: 0x00564640
		public static void AddCommand<TCommand, T1>(EPriority priority, T1 arg) where TCommand : class, ICommand, ICollectable<T1>, new()
		{
			TCommand command = EasyPool.Get<TCommand>();
			command.Pooled = true;
			command.Reset(arg);
			CommandManager.AddCommand(command, priority);
		}

		// Token: 0x0600BEB3 RID: 48819 RVA: 0x0056647C File Offset: 0x0056467C
		public static void AddCommand<TCommand, T1, T2>(EPriority priority, T1 arg1, T2 arg2) where TCommand : class, ICommand, ICollectable<T1, T2>, new()
		{
			TCommand command = EasyPool.Get<TCommand>();
			command.Pooled = true;
			command.Reset(arg1, arg2);
			CommandManager.AddCommand(command, priority);
		}

		// Token: 0x0600BEB4 RID: 48820 RVA: 0x005664B8 File Offset: 0x005646B8
		public static void AddCommand<TCommand, T1, T2, T3>(EPriority priority, T1 arg1, T2 arg2, T3 arg3) where TCommand : class, ICommand, ICollectable<T1, T2, T3>, new()
		{
			TCommand command = EasyPool.Get<TCommand>();
			command.Pooled = true;
			command.Reset(arg1, arg2, arg3);
			CommandManager.AddCommand(command, priority);
		}

		// Token: 0x17001578 RID: 5496
		// (get) Token: 0x0600BEB5 RID: 48821 RVA: 0x005664F5 File Offset: 0x005646F5
		public static bool IsRunning
		{
			get
			{
				return SingletonObject.getInstance<CommandManager>().InternalIsRunning;
			}
		}

		// Token: 0x0600BEB6 RID: 48822 RVA: 0x00566501 File Offset: 0x00564701
		public static void AddCommand(ICommand command, EPriority priority)
		{
			SingletonObject.getInstance<CommandManager>().InternalAddCommand(command, priority);
		}

		// Token: 0x0600BEB7 RID: 48823 RVA: 0x00566511 File Offset: 0x00564711
		public static void RemoveCommand(ICommand command)
		{
			SingletonObject.getInstance<CommandManager>().InternalRemoveCommand(command);
		}

		// Token: 0x0600BEB8 RID: 48824 RVA: 0x00566520 File Offset: 0x00564720
		public static void RemoveCommand(EPriority priority)
		{
			SingletonObject.getInstance<CommandManager>().InternalRemoveCommand(priority);
		}

		// Token: 0x0600BEB9 RID: 48825 RVA: 0x00566530 File Offset: 0x00564730
		private CommandManager.PriorityKey KeyByNow(EPriority priority)
		{
			CommandManager.PriorityKey result = default(CommandManager.PriorityKey);
			result.Priority = priority;
			ulong nextCommandId = this._nextCommandId;
			this._nextCommandId = nextCommandId + 1UL;
			result.Id = nextCommandId;
			return result;
		}

		// Token: 0x17001579 RID: 5497
		// (get) Token: 0x0600BEBA RID: 48826 RVA: 0x0056656C File Offset: 0x0056476C
		private bool InternalIsRunning
		{
			get
			{
				return this._runningCommand != null;
			}
		}

		// Token: 0x0600BEBB RID: 48827 RVA: 0x00566578 File Offset: 0x00564778
		private void InternalAddCommand(ICommand command, EPriority priority)
		{
			bool flag = this._runningCommand == null || this._runningCommand.Done;
			if (flag)
			{
				this._waitingCommands.Add(this.KeyByNow(priority), command);
			}
			else
			{
				this.FinishCommand(command);
			}
		}

		// Token: 0x0600BEBC RID: 48828 RVA: 0x005665C0 File Offset: 0x005647C0
		private void InternalRemoveCommand(ICommand command)
		{
			int index = this._waitingCommands.IndexOfValue(command);
			bool flag = index >= 0 && index < this._waitingCommands.Count;
			if (flag)
			{
				this._waitingCommands.RemoveAt(index);
			}
		}

		// Token: 0x0600BEBD RID: 48829 RVA: 0x00566604 File Offset: 0x00564804
		private void InternalRemoveCommand(EPriority priority)
		{
			List<CommandManager.PriorityKey> keys = EasyPool.Get<List<CommandManager.PriorityKey>>();
			keys.Clear();
			foreach (CommandManager.PriorityKey key in this._waitingCommands.Keys)
			{
				bool flag = key.Priority == priority;
				if (flag)
				{
					keys.Add(key);
				}
			}
			foreach (CommandManager.PriorityKey key2 in keys)
			{
				this._waitingCommands.Remove(key2);
			}
			EasyPool.Free<List<CommandManager.PriorityKey>>(keys);
		}

		// Token: 0x0600BEBE RID: 48830 RVA: 0x005666C4 File Offset: 0x005648C4
		private void FinishCommand(ICommand command)
		{
			ICollectable collectable = command as ICollectable;
			bool flag = collectable != null;
			if (flag)
			{
				try
				{
					collectable.Collect();
				}
				catch (Exception e)
				{
					AdaptableLog.TagWarning("Command", string.Format("Some exceptions occurred while collect the command, this exception has been ignored, details: \ncommand={0} \nexception={1} \ntrace=\n{2}", command, e.Message, e.StackTrace), false);
				}
				bool pooled = collectable.Pooled;
				if (pooled)
				{
					collectable.Pooled = false;
					EasyPool.Free<object>(command);
				}
			}
		}

		// Token: 0x0600BEBF RID: 48831 RVA: 0x00566748 File Offset: 0x00564948
		private void FinishCommand(int index)
		{
			bool flag = index >= this._waitingCommands.Count;
			if (!flag)
			{
				for (int i = index; i < this._waitingCommands.Count; i++)
				{
					this.FinishCommand(this._waitingCommands.Values[i]);
				}
			}
		}

		// Token: 0x0600BEC0 RID: 48832 RVA: 0x005667A0 File Offset: 0x005649A0
		public void Execute()
		{
			bool flag = this._runningCommand != null || this._waitingCommands == null || this._waitingCommands.Count == 0;
			if (!flag)
			{
				for (int i = 0; i < this._waitingCommands.Count; i++)
				{
					ICommand command = this._waitingCommands.Values[i];
					bool blocking = false;
					try
					{
						blocking = command.Execute();
					}
					catch (Exception e)
					{
						AdaptableLog.TagError("Command", string.Format("Some exceptions occurred while executing the command, this command has been skipped, details: \npriority={0} \ncommand={1} \nexception={2} \ntrace=\n{3}", new object[]
						{
							this._waitingCommands.Keys[i],
							command,
							e.Message,
							e.StackTrace
						}));
					}
					bool flag2 = blocking;
					if (flag2)
					{
						this._runningCommand = command;
						this.FinishCommand(i + 1);
						break;
					}
				}
				this._waitingCommands.Clear();
			}
		}

		// Token: 0x0600BEC1 RID: 48833 RVA: 0x005668A4 File Offset: 0x00564AA4
		private void LateUpdate()
		{
			bool flag = this._runningCommand != null && this._runningCommand.Done;
			if (flag)
			{
				this.FinishCommand(this._runningCommand);
				this._runningCommand = null;
			}
			bool flag2 = this._runningCommand == null;
			if (flag2)
			{
				this.Execute();
			}
		}

		// Token: 0x0600BEC2 RID: 48834 RVA: 0x005668F6 File Offset: 0x00564AF6
		public void Dispose()
		{
			this._runningCommand = null;
			this._waitingCommands.Clear();
		}

		// Token: 0x0600BEC3 RID: 48835 RVA: 0x0056690C File Offset: 0x00564B0C
		public void Init()
		{
			this._waitingCommands = new SortedList<CommandManager.PriorityKey, ICommand>();
		}

		// Token: 0x0600BEC4 RID: 48836 RVA: 0x0056691C File Offset: 0x00564B1C
		[Conditional("CONCHSHIP_DEV")]
		private void LogSkipCommand(int index)
		{
			bool flag = index >= this._waitingCommands.Count;
			if (!flag)
			{
				for (int i = index; i < this._waitingCommands.Count; i++)
				{
					AdaptableLog.TagInfo("Command", string.Format("Skipped command {0}.", this._waitingCommands.Values[i]));
				}
				AdaptableLog.TagInfo("Command", string.Format("Total skipped {0} commands.", this._waitingCommands.Count - index));
			}
		}

		// Token: 0x0600BEC5 RID: 48837 RVA: 0x005669A8 File Offset: 0x00564BA8
		[Conditional("CONCHSHIP_DEV")]
		private void LogSkipCommand(ICommand command, EPriority priority)
		{
			AdaptableLog.TagInfo("Command", string.Format("Skipped command {0} in running, priority = {1}", command, priority));
		}

		// Token: 0x04009256 RID: 37462
		private ICommand _runningCommand;

		// Token: 0x04009257 RID: 37463
		private SortedList<CommandManager.PriorityKey, ICommand> _waitingCommands;

		// Token: 0x04009258 RID: 37464
		private ulong _nextCommandId;

		// Token: 0x02002686 RID: 9862
		private struct PriorityKey : IComparable<CommandManager.PriorityKey>, IEquatable<CommandManager.PriorityKey>
		{
			// Token: 0x06011C19 RID: 72729 RVA: 0x00689484 File Offset: 0x00687684
			public override bool Equals(object obj)
			{
				bool result;
				if (obj is CommandManager.PriorityKey)
				{
					CommandManager.PriorityKey pk = (CommandManager.PriorityKey)obj;
					result = this.Equals(pk);
				}
				else
				{
					result = false;
				}
				return result;
			}

			// Token: 0x06011C1A RID: 72730 RVA: 0x006894B0 File Offset: 0x006876B0
			public bool Equals(CommandManager.PriorityKey other)
			{
				return this.Priority == other.Priority && this.Id == other.Id;
			}

			// Token: 0x06011C1B RID: 72731 RVA: 0x006894E4 File Offset: 0x006876E4
			public override int GetHashCode()
			{
				return (int)(this.Priority * (EPriority)397 ^ (EPriority)this.Id.GetHashCode());
			}

			// Token: 0x06011C1C RID: 72732 RVA: 0x00689510 File Offset: 0x00687710
			public int CompareTo(CommandManager.PriorityKey other)
			{
				return (this.Priority != other.Priority) ? this.Priority.CompareTo(other.Priority) : this.Id.CompareTo(other.Id);
			}

			// Token: 0x0400EB00 RID: 60160
			public EPriority Priority;

			// Token: 0x0400EB01 RID: 60161
			public ulong Id;
		}
	}
}
