// ===========================================================
// Copyright (c) 2014-2015, Enrico Da Ros/kendar.org
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// ===========================================================


using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace GenericHelpers
{
	public class CommandLineParser
	{
		private static Dictionary<string, string> _kvps;

		private static readonly object _lockObject = new object();
		private readonly Dictionary<string, List<string>> _commandLineValues;
		private readonly Action _exitBehaviour;
		private readonly char _separator;
		private readonly string _helpMessage;

		public CommandLineParser(string[] args, string helpMessage, Action exitBehaviour = null, char separator = ';', params string[] allowMultiple)
		{
			InitializeEnvironmentVariables();

			_helpMessage = helpMessage;
			_exitBehaviour = exitBehaviour;
			_separator = separator;
			var allowMultiple1 = new HashSet<string>(allowMultiple, StringComparer.InvariantCultureIgnoreCase);
			_commandLineValues = new Dictionary<string, List<string>>();
			for (int index = 0; index < args.Length; index++)
			{
				var item = args[index];
				if (item.StartsWith("-"))
				{
					var itemIndex = item.Substring(1).ToLowerInvariant();
					if (index < (args.Length - 1))
					{
						var nextItem = args[index + 1];
						if (!nextItem.StartsWith("-"))
						{
							if (!_commandLineValues.ContainsKey(itemIndex))
							{
								_commandLineValues[itemIndex] = new List<string>();
							}
							else if (!allowMultiple1.Contains(itemIndex))
							{
								throw new Exception(string.Format("Multiple key not allowed for '{0}'", item.Substring(1)));
							}
							_commandLineValues[itemIndex].Add(nextItem);
							continue;
						}
					}
					_commandLineValues.Add(itemIndex, new List<string> { string.Empty });
				}
			}
			if (IsSet("help") || IsSet("h"))
			{
				ShowHelp();
			}
		}

		public string Help
		{
			get { return _helpMessage; }
		}

		public bool IsMultiple(string index)
		{
			index = index.ToLowerInvariant();
			return _commandLineValues[index].Count > 1;
		}

		public IEnumerable<string> GetMultiple(string index)
		{
			index = index.ToLowerInvariant();
			if (!IsSet(index)) yield break;
			foreach (var item in _commandLineValues[index])
			{
				yield return item;
			}
		}

		public string this[string index]
		{
			get
			{
				index = index.ToLowerInvariant();
				if (IsSet(index))
				{
					if (!IsMultiple(index))
					{
						return string.Join(_separator.ToString(CultureInfo.InvariantCulture), _commandLineValues[index]);
					}
					return _commandLineValues[index][0];
				}
				return null;
			}
			set
			{
				index = index.ToLowerInvariant();
				if (!_commandLineValues.ContainsKey(index))
				{
					_commandLineValues.Add(index, new List<string>());
				}
				if (string.IsNullOrWhiteSpace(value))
				{
					_commandLineValues[index].Add(string.Empty);
					return;
				}
				_commandLineValues[index].Clear();
				var val = value.Split(new[] { _separator });
				foreach (var commandLineValue in val)
				{
					_commandLineValues[index].Add(commandLineValue);
				}
			}
		}

		private static void LoadEnvironmentVariables(EnvironmentVariableTarget target, bool none = false)
		{
			IDictionary environmentVariables = none ? Environment.GetEnvironmentVariables() : Environment.GetEnvironmentVariables(target);

			foreach (DictionaryEntry de in environmentVariables)
			{
				var lowerKey = ((string)de.Key).ToLowerInvariant();
				if (!_kvps.ContainsKey(lowerKey))
				{
					_kvps.Add(lowerKey, (string)de.Value);
				}
			}
		}

		public static string GetEnv(string envVar)
		{
			envVar = envVar.ToLowerInvariant();
			InitializeEnvironmentVariables();
			if (_kvps.ContainsKey(envVar))
			{
				return _kvps[envVar];
			}
			return null;
		}

		public static void SetEnv(string envVar, string val)
		{
			envVar = envVar.ToLowerInvariant();
			InitializeEnvironmentVariables();
			if (_kvps.ContainsKey(envVar))
			{
				_kvps[envVar] = val;
			}
			else
			{
				_kvps.Add(envVar, val);
			}
		}

		private static void InitializeEnvironmentVariables()
		{
			if (_kvps == null)
			{
				lock (_lockObject)
				{
					_kvps = new Dictionary<string, string>();
					LoadEnvironmentVariables(EnvironmentVariableTarget.Process, true);
					LoadEnvironmentVariables(EnvironmentVariableTarget.Process);
					LoadEnvironmentVariables(EnvironmentVariableTarget.User);
					LoadEnvironmentVariables(EnvironmentVariableTarget.Machine);
				}
			}
		}

		public string GetOrDefault(string index, string defaultValue)
		{
			if (Has(index)) return this[index];
			return defaultValue;
		}

		public bool IsSet(string index)
		{
			index = index.ToLowerInvariant();
			return _commandLineValues.ContainsKey(index) && _commandLineValues[index].Count > 0;
		}

		public bool Has(params string[] vals)
		{
			foreach (var item in vals)
			{
				var index = item.ToLowerInvariant();
				if (!IsSet(index)) return false;
			}
			return true;
		}

		public bool HasAllOrNone(params string[] vals)
		{
			int setted = 0;
			foreach (var item in vals)
			{
				var index = item.ToLowerInvariant();
				if (IsSet(index)) setted++;
			}
			if (setted == 0 || setted == vals.Length) return true;
			return false;
		}

		public bool HasOneAndOnlyOne(params string[] vals)
		{
			bool setted = false;
			foreach (var item in vals)
			{
				var index = item.ToLowerInvariant();
				if (IsSet(index))
				{
					if (setted)
					{
						return false;
					}
					setted = true;
				}
			}
			return setted;
		}

		public void ShowHelp()
		{
			Console.WriteLine(_helpMessage);
			if (_exitBehaviour != null) _exitBehaviour();
		}
	}
}
