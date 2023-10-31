using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfTuringMachine
{
    class TuringMachine
    {
		// delta function
		private Dictionary<Tuple<string, string>, Tuple<string, string, char>> delta;
		// δ(q,a)
		private Tuple<string, string> before;
		//(p,b,L||R||S)
		private Tuple<string, string, char> after;

		private List<string> K;  // q0,q1,...,qn finite set of states
		private List<string> VT; // Σ allowed symbols on the input tape
		private string terminateState;  // terminal state
		private string blank;           // blank symbol
		private string initialState;    // initial state
		private List<string> tape;
		private bool stepByStep;
		private string savedState;
		private string currSymbol;
		private int pos;
		private char currMovement;

		public List<string> GetKSet() { return K; }
		public List<string> GetVT() { return VT; }
		public string Getq0() { return initialState; }
		public string Getqf() { return terminateState; }
		public string GetBlank() { return blank; }
		public int GetPos() { return pos; }
		public string GetState() { return savedState; }
		public Tuple<string, string> GetBefore() { return before; }
		public Tuple<string, string, char> GetAfter()
		{
			Tuple<string, string, char> y;
			StateTrans(before, out y);
			return y;
		}
		public Dictionary<Tuple<string, string>, Tuple<string, string, char>> GetDelta()
		{

			return delta;
		}
		public TuringMachine()
		{
			stepByStep = false;
			execute = false;
			isFirst = true;
			K = new List<string>();
			VT = new List<string>();
			delta = new Dictionary<Tuple<string, string>, Tuple<string, string, char>>();
			terminateState = "";
			blank = "";
			initialState = "";
		}

		public TuringMachine(List<string> Kset, List<string> VTset, string qF, string blankSymbol, string q0, Dictionary<Tuple<string, string>, Tuple<string, string, char>> d)
		{
			stepByStep = false;
			execute = false;
			isFirst = true;
			K = Kset;
			VT = VTset;
			delta = d;
			terminateState = qF;
			blank = blankSymbol;
			initialState = q0;
		}

		public TuringMachine(string filename)
		{
			stepByStep = false;
			execute = false;
			isFirst = true;
			K = new List<string>();
			VT = new List<string>();
			delta = new Dictionary<Tuple<string, string>, Tuple<string, string, char>>();
			readTuringMachine(filename);
		}

		public void switchMode(bool flag)
		{
			stepByStep = flag;
			execute = false;
		}
		private bool execute;
		private bool isFirst;
		public bool ReadString(List<string> str)
		{
			if (str == tape && savedState == terminateState)
				return true;
			if (!CheckInput(str))
				return false;

			int len = str.Count;
			int k = 0;


			if (stepByStep && !isFirst)
			{
				k = pos;
			}
			else
				before = new Tuple<string, string>(initialState, str[0]);
			while (true)
			{
				if (stepByStep) //bool stepByStep
				{
					if (execute)
					{
						execute = false;
						return true;
					}
				}
				//private Tuple<string, string, char> after;
				//private Tuple<string, string> before;
				if (!StateTrans(before, out after))
					return false;
				else
				{

					string currState = after.Item1;
					string writtenSymbol = after.Item2;
					char dir = after.Item3;
					if (!CheckSymbol(writtenSymbol))
						return false;

					if (currState == terminateState)
					{
						savedState = currState;
						pos += movement(dir);

						if (k != -1 && k != -2)
							str[k] = writtenSymbol;
						tape = str;
						return true;
					}

					if (k != -1 && k != -2)
						str[k] = writtenSymbol;

					k += movement(dir);

					if (k == -1)
						before = new Tuple<string, string>(currState, blank);
					else if (k == -2)
						return false;
					else if (k >= str.Count)
					{
						str.Add(blank);
						before = new Tuple<string, string>(currState, str[k]);
					}
					else
						before = new Tuple<string, string>(currState, str[k]);
					savedState = currState;
				}
				tape = str;
				pos = k;
				execute = true;
				isFirst = false;

			}
		}

		private int movement(char dir)
		{
			switch (dir)
			{
				case 'R':
					return 1;
				case 'L':
					return -1;
				default:
					return 0;
			}
		}

		public void readTuringMachine(string filename)
		{
			StreamReader file = new StreamReader(filename);
			if (file == null)
			{
				//Console.WriteLine("Failed to open file.");
				MessageBox.Show("Failed to open file.", "Error",MessageBoxButton.OK,MessageBoxImage.Error);
				return;
			}
			K.Clear();
			VT.Clear();
			delta.Clear();
			blank = null;
			initialState = null;
			terminateState = null;
			string line;
			// 读取 k
			if ((line = file.ReadLine()) != null)
			{
				int pos = line.IndexOf("=");
				if (pos >= 0)
				{
					string kStr = line.Substring(pos + 1); // 提取 k 的字符串部分
					string[] tokens = kStr.Split(',');
					foreach (string token in tokens)
					{
						K.Add(token);
					}
				}
			}

			// 读取 vt
			if ((line = file.ReadLine()) != null)
			{
				int pos = line.IndexOf("=");
				if (pos >= 0)
				{
					string vtStr = line.Substring(pos + 1); // 提取 vt 的字符串部分
					string[] tokens = vtStr.Split(',');
					foreach (string token in tokens)
					{
						VT.Add(token);
					}
				}
			}

			// 读取 blank
			if ((line = file.ReadLine()) != null)
			{
				int pos = line.IndexOf("=");
				if (pos >= 0)
				{
					blank = line.Substring(pos + 1); // 提取 blank 的字符串部分
				}
			}

			// 读取 q0
			if ((line = file.ReadLine()) != null)
			{
				int pos = line.IndexOf("=");
				if (pos >= 0)
				{
					initialState = line.Substring(pos + 1); // 提取 q0 的字符串部分
				}
			}

			// 读取 qf
			if ((line = file.ReadLine()) != null)
			{
				int pos = line.IndexOf("=");
				if (pos >= 0)
				{
					terminateState = line.Substring(pos + 1); // 提取 qf 的字符串部分
				}
			}

			// 读取 delta
			while ((line = file.ReadLine()) != null)
			{
				int pos = line.IndexOf("(");
				if (pos >= 0)
				{
					string deltaStr = line.Substring(pos + 1, line.Length - pos - 2); // 提取 delta 的字符串部分
					string[] tokens = deltaStr.Split(',', ')');
					string state1 = tokens[0];
					string symbol1 = tokens[1];
					string state2 = tokens[2].Substring(tokens[2].IndexOf("(") + 1);
					string symbol2 = tokens[3];
					char movement = tokens[4][0];
					delta.Add(Tuple.Create(state1, symbol1), Tuple.Create(state2, symbol2, movement));
				}
			}

			file.Close();
		}

		public string GetTape()
		{
			string output = "";
			foreach (var symbol in tape)
			{
				output += symbol;
			}
			return output;
		}

		private bool StateTrans(Tuple<string, string> x, out Tuple<string, string, char> y)
		{
			if (!delta.ContainsKey(x))
			{
				y = null;
				return false;
			}
			else
			{
				y = delta[x];
				return true;
			}
		}

		private bool CheckInput(List<string> str)
		{
			foreach (var symbol in str)
			{
				if (symbol != blank && !VT.Contains(symbol))
					return false;
			}
			return true;
		}

		private bool CheckSymbol(string sym)
		{
			return sym == blank || VT.Contains(sym);
		}
	}
}
