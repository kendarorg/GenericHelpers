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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GenericHelpers
{
	public static class ReflectionUtils
	{
		public static bool IsSystemType(Type type)
		{
			if (type == typeof(object)) return false;
			if (type.IsPrimitive) return true;
			if (type == typeof(string)) return true;
			if (type == typeof(DateTime)) return true;
			if (type == typeof(TimeSpan)) return true;
			return false;
		}
		private const BindingFlags DynamicBindingFlags = BindingFlags.Public | BindingFlags.Instance;

		public static Dictionary<string, object> ObjectToDictionary(object data)
		{
			var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

			if (data == null) return dict;
			var stri = data.ToString();
			if (string.IsNullOrWhiteSpace(stri)) return dict;

			try
			{
				foreach (var property in data.GetType().GetProperties(DynamicBindingFlags))
				{
					if (property.CanRead)
					{
						dict.Add(property.Name, property.GetValue(data, null));
					}
				}
			}
			catch (Exception)
			{

			}
			return dict;
		}
	}
}
