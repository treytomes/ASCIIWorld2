using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCIIWorld
{
	public static class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			//ScriptTest();
			//return;

			var settings = new AppSettings();

			Console.WriteLine("Launching game window.");
			new ASCIIWorldGameWindow().Run(settings.UpdatesPerSecond, settings.FramesPerSecond);
		}

		private static void ScriptTest()
		{
			try
			{
				//Console.WriteLine(MoonSharp1());
				//Console.WriteLine(MoonSharp2());
				//Console.WriteLine(MoonSharp3());
				//Console.WriteLine(MoonSharp4());
				//Console.WriteLine(MoonSharp5());
				//Console.WriteLine(MoonSharp6());
				//MoonSharp7();
				//MoonSharp8();
				//CallbackTest();
				//EnumerableTest();
				//TableTest1();
				//TableTest2();
				//TableTestReverse();
				//TableTestReverseSafer();
				//TableTestReverseWithTable();
				//CoroutineTest2();
				//CoroutineTest();
				//PreemptiveCoroutineTest();
				OOPTest();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private static void OOPTest()
		{
			var script = new Script();
			script.Options.DebugPrint = s => { Console.WriteLine(s); };
			script.Options.DebugInput = s => { return Console.ReadLine(); };

			//			script.DoString(@"
			//Account = {
			//	balance = 0
			//}
			//function Account.withdraw(self, v)
			//	self.balance = self.balance - v
			//end

			//a = Account
			//b = Account
			//Account = nil

			//a.withdraw(a, 10.0)
			//b.withdraw(b, 12.5)

			//print(a)
			//print(b)

			//");

			//			script.DoString(@"
			//Account = {
			//	balance = 0
			//}
			//function Account:withdraw(v)
			//	self.balance = self.balance - v
			//end

			//a = {balance = 0, withdraw = Account.withdraw }
			//b = {balance = 0, withdraw = Account.withdraw }

			//a:withdraw(10.0)
			//b:withdraw(12.5)

			//print(a)
			//print(b)

			//");

			//			script.DoString(@"
			//Account = {
			//	balance = 0,
			//	withdraw = function (self, v)
			//		self.balance = self.balance - v
			//	end
			//}

			//function Account:deposit(v)
			//	self.balance = self.balance + v
			//end

			//a = {balance = 0, withdraw = Account.withdraw, deposit = Account.deposit }
			//b = {balance = 0, withdraw = Account.withdraw, deposit = Account.deposit }

			//a:withdraw(10.0)
			//a.deposit(a, 1)

			//b:withdraw(12.5)

			//print(a)
			//print(b)

			//");

			//			script.DoString(@"
			//Account = {
			//	balance = 0,
			//	withdraw = function (self, v)
			//		self.balance = self.balance - v
			//	end,
			//	deposit = function (self, v)
			//		self.balance = self.balance + v
			//	end
			//}

			//function Account:new (o)
			//	o = o or {} -- create object if user does not provide one
			//	setmetatable(o, self) -- if 'o' doesn't have the function, it will look at 'self' for the function.
			//	self.__index = self
			//	return o
			//end

			//a = Account:new() --{balance = 0}
			//a:deposit(100)

			//b = {balance = 0, withdraw = Account.withdraw, deposit = Account.deposit }

			//a:withdraw(10.0)
			//a.deposit(a, 1)

			//b:withdraw(12.5)

			//print(a)
			//print(b)

			//");

			script.DoString(@"
Account = { balance = 0 }
function Account:new (o)
	o = o or {} -- create object if user does not provide one
	setmetatable(o, self) -- if 'o' doesn't have the function, it will look at 'self' for the function.
	self.__index = self -- __index is a special variable
	return o
end
function Account:withdraw(v)
	if v > self.balance then error""insufficient funds"" end
	self.balance = self.balance - v
end
function Account:deposit(v)
	self.balance = self.balance + v
end

a = Account:new() --{balance = 0}
a:deposit(100)

a:withdraw(10.0)
a:deposit(1)

SpecialAccount = Account:new()
function SpecialAccount:withdraw (v)
	if v - self.balance >= self:getLimit() then
		error'insufficient funds'
	end
	self.balance = self.balance - v
end
function SpecialAccount: getLimit()
	return self.limit or 0
end

s = SpecialAccount:new{limit=1000.00}
function s:getLimit () -- give the 's' instance a special override method
		return self.balance * 0.10
end
s:deposit(100.00)

print(a)
");
			/*

			*/
			Console.WriteLine("a={0}", script.Globals.Get("a").Table.Get("balance").Number);
			Console.WriteLine("s={0}", script.Globals.Get("s").Table.Get("balance").Number);
		}

		// See http://www.moonsharp.org/ for more information!

		/*
			How can I redirect the output of the print function ?
				script.Options.DebugPrint = s => { Console.WriteLine(s); }

			How can I redirect the input to the Lua program ?
				script.Options.DebugInput = () => { return Console.ReadLine(); }

			How can I redirect the IO streams of a Lua program ?
				IoModule.SetDefaultFile(script, IoModule.DefaultFiles.In, myInputStream);
				IoModule.SetDefaultFile(script, IoModule.DefaultFiles.Out, myInputStream);
				IoModule.SetDefaultFile(script, IoModule.DefaultFiles.Err, myInputStream);
		*/

		private static void PreemptiveCoroutineTest()
		{

			// This will force the function to yield to the main program every 10 instructions!

			string code = @"
	function fib(n)
		if (n == 0 or n == 1) then
			return 1;
		else
			return fib(n - 1) + fib(n - 2);
		end
	end
	";

			// Load the code and get the returned function
			Script script = new Script(CoreModules.None);
			script.DoString(code);

			// get the function
			DynValue function = script.Globals.Get("fib");

			// Create the coroutine in C#
			DynValue coroutine = script.CreateCoroutine(function);

			// Set the automatic yield counter every 10 instructions. 
			// 10 is likely too small! Use a much bigger value in your code to avoid interrupting too often!
			coroutine.Coroutine.AutoYieldCounter = 10; // 1000 is usually a good starting point.

			int cycles = 0;
			DynValue result = null;

			// Cycle until we get that the coroutine has returned something useful and not an automatic yield..
			for (result = coroutine.Coroutine.Resume(8);
				result.Type == DataType.YieldRequest;
				result = coroutine.Coroutine.Resume())
			{
				cycles += 1;
			}

			// Check the values of the operation
			Console.WriteLine(DataType.Number == result.Type);
			Console.WriteLine(34 == result.Number);
		}

		private static void CoroutineTest()
		{
			string code = @"
				return function()
					local x = 0
					while true do
						x = x + 1
						coroutine.yield(x)
					end
				end
				";

			// Load the code and get the returned function
			Script script = new Script();
			DynValue function = script.DoString(code);

			// Create the coroutine in C#
			DynValue coroutine = script.CreateCoroutine(function);

			// Resume the coroutine forever and ever..
			while (true)
			{
				DynValue x = coroutine.Coroutine.Resume();
				Console.WriteLine("{0}", x);
			}
		}

		private static void CoroutineTest2()
		{
			string code = @"
	return function()
		local x = 0
		while true do
			x = x + 1
			coroutine.yield(x)
			if (x > 5) then
				return 7
			end
		end
	end
	";

			// Load the code and get the returned function
			Script script = new Script();
			DynValue function = script.DoString(code);

			// Create the coroutine in C#
			DynValue coroutine = script.CreateCoroutine(function);

			// Loop the coroutine 
			string ret = "";

			foreach (DynValue x in coroutine.Coroutine.AsTypedEnumerable())
			{
				ret = ret + x.ToString();
			}

			Console.WriteLine(ret);
			//Assert.AreEqual("1234567", ret);
		}

		class IndexerTestClass
		{
			Dictionary<int, int> mymap = new Dictionary<int, int>();

			public int this[int idx]
			{
				get { return mymap[idx]; }
				set { mymap[idx] = value; }
			}

			public int this[int idx1, int idx2, int idx3]
			{
				get { int idx = (idx1 + idx2) * idx3; return mymap[idx]; }
				set { int idx = (idx1 + idx2) * idx3; mymap[idx] = value; }
			}

			/*
			Can be accessed in a Lua script via:

-- sets the value of an indexer
o[5] = 19; 		

-- use the value of an indexer
x = 5 + o[5]; 	

-- sets the value of an indexer using multiple indices (not standard Lua!)
o[1,2,3] = 19; 		

-- use the value of an indexer using multiple indices (not standard Lua!)
x = 5 + o[1,2,3];

			*/
		}


		[MoonSharpUserData]
		class MyClassStatic
		{
			// Overloaded methods can be used in a script.  It'll probably use the right one...

			public static double calcHypotenuse(double a, double b)
			{
				return Math.Sqrt(a * a + b * b);
			}
		}

		private static double MyClassStaticThroughPlaceholder()
		{
			string scriptCode = @"    
				return obj.calcHypotenuse(3, 4);
				-- or return obj:calcHypotenuse(3, 4); -- this is a static way of accessing the object.
			";

			// Automatically register all MoonSharpUserData types
			UserData.RegisterAssembly();

			Script script = new Script();

			script.Globals["obj"] = typeof(MyClassStatic);
			// or you can assign obj to UserData.CreateStatic<MyClassStatic>();

			DynValue res = script.DoString(scriptCode);

			return res.Number;
		}


		private static double MyClassStaticThroughInstance()
		{
			string scriptCode = @"    
				return obj.calcHypotenuse(3, 4);
			";

			// Automatically register all MoonSharpUserData types
			UserData.RegisterAssembly();

			Script script = new Script();

			script.Globals["obj"] = new MyClassStatic();

			DynValue res = script.DoString(scriptCode);

			return res.Number;
		}

		class MyClass2
		{
			public double CalcHypotenuse(double a, double b)
			{
				return Math.Sqrt(a * a + b * b);
			}
		}

		static double CallMyClass2()
		{
			string scriptCode = @"    
				return obj.calcHypotenuse(3, 4);
			";

			// Register just MyClass, explicitely.
			UserData.RegisterType<MyClass2>();

			Script script = new Script();

			// create a userdata, again, explicitely.
			DynValue obj = UserData.Create(new MyClass2());

			script.Globals.Set("obj", obj);

			DynValue res = script.DoString(scriptCode);

			return res.Number;
		}


		[MoonSharpUserData]
		class MyClass
		{
			public double calcHypotenuse(double a, double b)
			{
				return Math.Sqrt(a * a + b * b);
			}
		}

		private static double CallMyClass1()
		{
			string scriptCode = @"    
				return obj.calcHypotenuse(3, 4);
			";

			// Automatically register all MoonSharpUserData types in the current assembly
			UserData.RegisterAssembly();

			Script script = new Script();

			// Pass an instance of MyClass to the script in a global
			script.Globals["obj"] = new MyClass();

			DynValue res = script.DoString(scriptCode);

			return res.Number;
		}

		private static void CustomConverters()
		{
			// This will cause all StringBuilder objects to be upper-cased before being shoved into Lua.
			Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion<StringBuilder>(
				(script, sb) => DynValue.NewString(sb.ToString().ToUpper()));

			// This will cause all Tables to be processed into integer lists on their way to the CLR.
			/*Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Table, typeof(IList<int>),
				v => new List<int>() { ... });*/
		}

		private static double MoonSharp1()
		{
			string script = @"    
				-- defines a factorial function
				function fact (n)
					if (n == 0) then
						return 1
					else
						return n*fact(n - 1)
					end
				end

				return fact(5)";

			DynValue res = Script.RunString(script);
			return res.Number;
		}

		private static double MoonSharp2()
		{
			string scriptCode = @"    
				-- defines a factorial function
				function fact (n)
					if (n == 0) then
						return 1
					else
						return n*fact(n - 1)
					end
				end

				return fact(5)";

			var script = new Script();
			var result = script.DoString(scriptCode);
			return result.Number;
		}

		private static double MoonSharp3()
		{
			string scriptCode = @"    
				-- defines a factorial function
				function fact (n)
					if (n == 0) then
						return 1
					else
						return n*fact(n - 1)
					end
				end

				return fact(mynumber)";

			var script = new Script();
			script.Globals["mynumber"] = 7;
			var result = script.DoString(scriptCode);
			return result.Number;
		}

		private static double MoonSharp4()
		{
			string scriptCode = @"    
				-- defines a factorial function
				function fact (n)
					if (n == 0) then
						return 1
					else
						return n*fact(n - 1)
					end
				end";

			var script = new Script();
			script.DoString(scriptCode);
			var result = script.Call(script.Globals["fact"], 4);
			return result.Number;
		}

		private static double MoonSharp5()
		{
			string scriptCode = @"    
				-- defines a factorial function
				function fact (n)
					if (n == 0) then
						return 1
					else
						return n*fact(n - 1)
					end
				end";

			var script = new Script();
			script.DoString(scriptCode);

			var luaFactFunction = script.Globals.Get("fact");
			var result = script.Call(luaFactFunction, 4);
			return result.Number;
		}

		private static double MoonSharp6()
		{
			string scriptCode = @"    
				-- defines a factorial function
				function fact (n)
					if (n == 0) then
						return 1
					else
						return n*fact(n - 1)
					end
				end";

			var script = new Script();
			script.DoString(scriptCode);

			var luaFactFunction = script.Globals.Get("fact");
			var result = script.Call(luaFactFunction, DynValue.NewNumber(4));
			return result.Number;
		}

		private static void MoonSharp7()
		{
			// Create a new number
			var v1 = DynValue.NewNumber(1);
			// and a new string
			var v2 = DynValue.NewString("ciao");
			// and another string using the automagic converters
			var v3 = DynValue.FromObject(new Script(), "hello");

			// This prints Number - String - String
			Console.WriteLine("{0} - {1} - {2}", v1.Type, v2.Type, v3.Type);
			// This prints Number - String - Some garbage number you shouldn't rely on to be 0
			Console.WriteLine("{0} - {1} - {2}", v1.Number, v2.String, v3.Number);
		}

		private static void MoonSharp8()
		{
			var ret = Script.RunString("return true, 'ciao', 2*3");

			// prints "Tuple"
			Console.WriteLine("{0}", ret.Type);

			// Prints:
			//   Boolean = true
			//   String = "ciao"
			//   Number = 6
			for (var i = 0; i < ret.Tuple.Length; i++)
			{
				Console.WriteLine("{0} = {1}", ret.Tuple[i].Type, ret.Tuple[i]);
			}
		}

		// Used by CallbackTest.
		private static int Mul(int a, int b)
		{
			return a * b;
		}

		private static void CallbackTest()
		{
			var scriptCode = @"    
				-- defines a factorial function
				function fact (n)
					if (n == 0) then
						return 1
					else
						return Mul(n, fact(n - 1));
					end
				end";

			var script = new Script();
			script.Globals["Mul"] = (Func<int, int, int>)Mul;
			script.DoString(scriptCode);

			var res = script.Call(script.Globals["fact"], 4);

			Console.WriteLine(res.Number);
		}

		// Used by EnumerableTest.
		private static IEnumerable<int> GetNumbers()
		{
			for (int i = 1; i <= 10; i++)
			{
				yield return i;
			}
		}

		private static void EnumerableTest()
		{
			var scriptCode = @"    
				total = 0;
        
				for i in getNumbers() do
					total = total + i;
				end

				return total;
			";

			var script = new Script();
			script.Globals["getNumbers"] = (Func<IEnumerable<int>>)GetNumbers;

			var res = script.DoString(scriptCode);

			Console.WriteLine(res.Number);
		}

		// Used by TableTest1.
		private static List<int> GetNumberList()
		{
			var lst = new List<int>();

			for (var i = 1; i <= 10; i++)
			{
				lst.Add(i);
			}

			return lst;
		}

		private static void TableTest1()
		{
			// The tbl variable is 1-indexed.  This is how Lua works.
			var scriptCode = @"    
				total = 0;

				tbl = getNumbers()
        
				for _, i in ipairs(tbl) do
					total = total + i;
				end

				return total;
			";

			var script = new Script();
			script.Globals["getNumbers"] = (Func<List<int>>)GetNumberList;

			var res = script.DoString(scriptCode);
			Console.WriteLine(res.Number);
		}

		// Used by TableTest2.
		private static Table GetNumberTable(Script script)
		{
			var tbl = new Table(script);

			for (int i = 1; i <= 10; i++)
			{
				tbl[i] = i * 2;
			}

			return tbl;
		}

		private static void TableTest2()
		{
			var scriptCode = @"    
				total = 0;

				tbl = getNumbers()
        
				for _, i in ipairs(tbl) do -- _ is the item number, starting at 1.
					total = total + i;
				end

				return total;
			";

			var script = new Script();
			script.Globals["getNumbers"] = (Func<Script, Table>)(GetNumberTable);

			var res = script.DoString(scriptCode);
			Console.WriteLine(res.Number);
		}

		private static void TableTestReverse()
		{
			string scriptCode = @"    
				return dosum { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 } -- call dosum with the given table
			";

			Script script = new Script();

			script.Globals["dosum"] = (Func<List<int>, int>)(l => l.Sum()); // the table becomes a List<T>!

			DynValue res = script.DoString(scriptCode);

			Console.WriteLine(res.Number);
		}

		private static void TableTestReverseSafer()
		{
			string scriptCode = @"    
				return dosum { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }
			";

			Script script = new Script();

			// Safer to assume that the Table is a list of objects, than to implicitely cast to ints.
			script.Globals["dosum"] = (Func<List<object>, double>)(l => l.OfType<double>().Sum());

			DynValue res = script.DoString(scriptCode);

			Console.WriteLine(res.Number);
		}

		// Used by TableTestReverseWithTable.
		private static double Sum(Table t)
		{
			var nums = from v in t.Values
					   where v.Type == DataType.Number
					   select v.Number;

			return nums.Sum();
		}

		private static void TableTestReverseWithTable()
		{
			string scriptCode = @"    
				return dosum { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }
			";

			Script script = new Script();

			script.Globals["dosum"] = (Func<Table, double>)Sum;

			DynValue res = script.DoString(scriptCode);

			Console.WriteLine(res.Number);
		}
	}
}
