using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;

namespace Game.Views.MouseTips
{
	// Token: 0x0200082D RID: 2093
	public sealed class CommonTipSimpleRuntime : CommonTipBaseRuntime
	{
		// Token: 0x06006663 RID: 26211 RVA: 0x002EB77E File Offset: 0x002E997E
		public CommonTipSimpleRuntime(CommonTipItem configLine) : base(configLine)
		{
		}

		// Token: 0x06006664 RID: 26212 RVA: 0x002EB7AC File Offset: 0x002E99AC
		public CommonTipSimpleRuntime Set(string key, string value)
		{
			bool flag = string.IsNullOrEmpty(key);
			CommonTipSimpleRuntime result;
			if (flag)
			{
				result = this;
			}
			else
			{
				bool flag2 = value == null;
				bool changed;
				if (flag2)
				{
					changed = this._arguments.Remove(key);
				}
				else
				{
					string oldValue;
					changed = (!this._arguments.TryGetValue(key, out oldValue) || oldValue != value);
					this._arguments[key] = value;
				}
				bool flag3 = changed;
				if (flag3)
				{
					base.RefreshOwner();
				}
				result = this;
			}
			return result;
		}

		// Token: 0x06006665 RID: 26213 RVA: 0x002EB820 File Offset: 0x002E9A20
		public override string GetArgument(string key)
		{
			string value;
			return this._arguments.TryGetValue(key, out value) ? value : null;
		}

		// Token: 0x06006666 RID: 26214 RVA: 0x002EB848 File Offset: 0x002E9A48
		public override bool ShouldShowParagraph(string name)
		{
			return !this._hiddenParagraphs.Contains(name);
		}

		// Token: 0x06006667 RID: 26215 RVA: 0x002EB86C File Offset: 0x002E9A6C
		public override bool ShouldShowAtom(string paragraphName, string name)
		{
			return !this._hiddenAtoms.Contains(new ValueTuple<string, string>(paragraphName, name));
		}

		// Token: 0x06006668 RID: 26216 RVA: 0x002EB894 File Offset: 0x002E9A94
		public CommonTipSimpleRuntime HideParagraph(string name)
		{
			bool flag = string.IsNullOrEmpty(name);
			CommonTipSimpleRuntime result;
			if (flag)
			{
				result = this;
			}
			else
			{
				bool flag2 = this._hiddenParagraphs.Add(name);
				if (flag2)
				{
					base.RefreshOwner();
				}
				result = this;
			}
			return result;
		}

		// Token: 0x06006669 RID: 26217 RVA: 0x002EB8CC File Offset: 0x002E9ACC
		public CommonTipSimpleRuntime ShowParagraph(string name)
		{
			bool flag = string.IsNullOrEmpty(name);
			CommonTipSimpleRuntime result;
			if (flag)
			{
				result = this;
			}
			else
			{
				bool flag2 = this._hiddenParagraphs.Remove(name);
				if (flag2)
				{
					base.RefreshOwner();
				}
				result = this;
			}
			return result;
		}

		// Token: 0x0600666A RID: 26218 RVA: 0x002EB904 File Offset: 0x002E9B04
		public CommonTipSimpleRuntime ShowAllParagraphs()
		{
			bool flag = this._hiddenParagraphs.Count > 0;
			if (flag)
			{
				this._hiddenParagraphs.Clear();
				base.RefreshOwner();
			}
			return this;
		}

		// Token: 0x0600666B RID: 26219 RVA: 0x002EB940 File Offset: 0x002E9B40
		public CommonTipSimpleRuntime HideAtom(string paragraphName, string name)
		{
			bool flag = string.IsNullOrEmpty(paragraphName) || string.IsNullOrEmpty(name);
			CommonTipSimpleRuntime result;
			if (flag)
			{
				result = this;
			}
			else
			{
				bool flag2 = this._hiddenAtoms.Add(new ValueTuple<string, string>(paragraphName, name));
				if (flag2)
				{
					base.RefreshOwner();
				}
				result = this;
			}
			return result;
		}

		// Token: 0x0600666C RID: 26220 RVA: 0x002EB98C File Offset: 0x002E9B8C
		public CommonTipSimpleRuntime ShowAtom(string paragraphName, string name)
		{
			bool flag = string.IsNullOrEmpty(paragraphName) || string.IsNullOrEmpty(name);
			CommonTipSimpleRuntime result;
			if (flag)
			{
				result = this;
			}
			else
			{
				bool flag2 = this._hiddenAtoms.Remove(new ValueTuple<string, string>(paragraphName, name));
				if (flag2)
				{
					base.RefreshOwner();
				}
				result = this;
			}
			return result;
		}

		// Token: 0x0600666D RID: 26221 RVA: 0x002EB9D8 File Offset: 0x002E9BD8
		public CommonTipSimpleRuntime ShowAllAtoms()
		{
			bool flag = this._hiddenAtoms.Count > 0;
			if (flag)
			{
				this._hiddenAtoms.Clear();
				base.RefreshOwner();
			}
			return this;
		}

		// Token: 0x040047B5 RID: 18357
		private readonly Dictionary<string, string> _arguments = new Dictionary<string, string>();

		// Token: 0x040047B6 RID: 18358
		private readonly HashSet<string> _hiddenParagraphs = new HashSet<string>();

		// Token: 0x040047B7 RID: 18359
		[TupleElementNames(new string[]
		{
			"ParagraphName",
			"AtomName"
		})]
		private readonly HashSet<ValueTuple<string, string>> _hiddenAtoms = new HashSet<ValueTuple<string, string>>();
	}
}
