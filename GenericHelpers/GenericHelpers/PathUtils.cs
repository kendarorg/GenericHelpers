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


using System.IO;
using System.Text;
using Microsoft.Win32;

namespace GenericHelpers
{
	public static class PathUtils
	{
		public static string GetExtension(string path)
		{
			var res = Path.GetExtension(path);
			if (res == null) return res;
			return res.Trim('.');
		}

		private readonly static string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
		private readonly static byte[] _preamble = Encoding.UTF8.GetPreamble();

		public static string RemoveBom(string result, byte[] data)
		{
			if (data.Length > _preamble.Length)
			{
				if (data[0] == _preamble[0] && data[1] == _preamble[1] && data[2] == _preamble[2])
				{
					return result.Remove(0, _byteOrderMarkUtf8.Length);
				}
			}
			return result;
		}

		public static string RemoveBom(string result)
		{
			if (result.Length < _byteOrderMarkUtf8.Length) return result;

			var data = Encoding.UTF8.GetBytes(result.Substring(0, _byteOrderMarkUtf8.Length));
			if (data[0] == _preamble[0] && data[1] == _preamble[1] && data[2] == _preamble[2])
			{
				return result.Remove(0, _byteOrderMarkUtf8.Length);
			}
			return result;
		}

		public static int HasBom(byte[] data)
		{
			if (data.Length < _preamble.Length) return 0;
			if (data[0] == _preamble[0] && data[1] == _preamble[1] && data[2] == _preamble[2])
			{
				return _preamble.Length;
			}
			return 0;
		}
	}
}
