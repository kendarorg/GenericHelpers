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
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenericHelpers.Test
{
	/// <summary>
	/// Summary description for AssembliesManagerTest
	/// </summary>
	[TestClass]
	public class AssembliesManagerTest
	{

		[TestMethod]
		public void ParsingAsimpleType()
		{
			const string tp = "System.String";
			var tb = AssembliesManager.ParseType(tp);
			Assert.IsNotNull(tb);
			Assert.AreEqual(tp, tb.ToString());
		}

		[TestMethod]
		public void LoadingAsimpleType()
		{
			const string tp = "System.String";
			var tb = AssembliesManager.LoadType(tp);
			Assert.AreEqual(typeof(string), tb);
		}

		[TestMethod]
		public void ParsingAsimpleGeneric()
		{
			const string tp = "System.Collections.Generic.IEnumerable<System.String>";
			var tb = AssembliesManager.ParseType(tp);
			Assert.IsNotNull(tb);
			Assert.AreEqual(tp, tb.ToString());
		}

		[TestMethod]
		public void LoadingAsimpleGeneric()
		{
			const string tp = "System.Collections.Generic.IEnumerable<System.String>";
			var tb = AssembliesManager.LoadType(tp);
			Assert.AreEqual(typeof(IEnumerable<string>), tb);
		}


		[TestMethod]
		public void ParsingAgenericWithChildren()
		{
			const string tp = "System.Collections.Generic.Dictionary<System.String,System.Int32>";
			var tb = AssembliesManager.ParseType(tp);
			Assert.IsNotNull(tb);
			Assert.AreEqual(tp, tb.ToString());
		}

		[TestMethod]
		public void LoadingAgenericWithChildren()
		{
			const string tp = "System.Collections.Generic.Dictionary<System.String,System.Int32>";
			var tb = AssembliesManager.LoadType(tp);
			Assert.AreEqual(typeof(Dictionary<string, int>), tb);
		}

		[TestMethod]
		public void ParsingAgenericWithComplex()
		{
			const string tp = "System.Collections.Generic.Dictionary<System.String,System.Collections.Generic.List<System.Int32>>";
			var tb = AssembliesManager.ParseType(tp);
			Assert.IsNotNull(tb);
			Assert.AreEqual(tp, tb.ToString());
		}

		[TestMethod]
		public void LoadingAgenericWithComplex()
		{
			const string tp = "System.Collections.Generic.Dictionary<System.String,System.Collections.Generic.List<System.Int32>>";
			var tb = AssembliesManager.LoadType(tp);
			Assert.AreEqual(typeof(Dictionary<String, List<Int32>>), tb);
		}

		[TestMethod]
		public void ParsingAgenericWithComplexAndWhiteSpaces()
		{
			const string tp = "Dictionary <string ,List<int >> ";
			const string expect = "Dictionary<string,List<int>>";
			var tb = AssembliesManager.ParseType(tp);
			Assert.IsNotNull(tb);
			Assert.AreEqual(expect, tb.ToString());
		}

		[Ignore]
		[TestMethod]
		public void ShouldBeAbleToLoadTypesGivenTheirParent()
		{

		}

		[Ignore]
		[TestMethod]
		public void ShouldBeAbleToLoadTypesGivenTheirAttribute()
		{

		}
	}
}
