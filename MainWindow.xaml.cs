using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfTuringMachine
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		DispatcherTimer timer;

		public MainWindow()
		{
			InitializeComponent();
			TimeText.Text = string.Empty;
			timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromSeconds(2);
			timer.Tick += Timer_Tick;

			timer1 = new DispatcherTimer();
			timer1.Interval = TimeSpan.FromSeconds(1);
			timer1.Tick += Timer1_Tick;
			timer1.Start();

			string directoryPath = Directory.GetCurrentDirectory();
			List<string> txtFiles = new List<string>();
			string[] files = Directory.GetFiles(directoryPath, "*.txt"); // 获取目录下所有后缀为.txt的文件路径数组

			txtFiles.AddRange(files); // 将文件路径数组添加到List<string>中
			RemoveModels();
			RemoveTapes();
			DeltaList.Items.Clear();

			foreach (string file in txtFiles)
			{
				AddNewItem(System.IO.Path.GetFileNameWithoutExtension(file));
			}
		}

		private void AddNewItem(string file)
		{
			ModelItem item = new ModelItem();
			item.FileName = file;
			item.MouseDown += new MouseButtonEventHandler(ModelItem_MouseDown);
			item.MouseUp += new MouseButtonEventHandler(ModelItem_MouseUp);
			models.Children.Add(item);
		}

		//删除所有的图灵机
		private void RemoveModels()
		{
			List<UIElement> toRemove = new List<UIElement>();
			foreach (var child in models.Children)
			{
				if (child is ModelItem modelItem)
				{
					toRemove.Add(modelItem);
				}
			}

			foreach (var toDel in toRemove)
			{
				models.Children.Remove(toDel);
			}

		}

		private void RemoveTapes()
		{
			List<UIElement> toRemove = new List<UIElement>();
			foreach (var child in TapePanel.Children)
			{
				if (child is TapeChar tape)
				{
					toRemove.Add(tape);
				}
			}

			foreach (var toDel in toRemove)
			{
				TapePanel.Children.Remove(toDel);
			}

			if (blankSymbol.Text != string.Empty) AppendChar(blankSymbol.Text);
		}

		private void AppendChar(string ch)
		{
			TapeChar tapeChar = new TapeChar();
			tapeChar.CharName = ch;
			TapePanel.Children.Add(tapeChar);
		}

		private void Border_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				this.DragMove();
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void Ellipse_MouseEnter(object sender, MouseEventArgs e)
		{
			if (sender is Ellipse ellipse)
			{
				if (ellipse.Fill == Brushes.Red)
				{
					SolidColorBrush newFill = new SolidColorBrush(Color.FromRgb(206, 128, 132));
					ellipse.Fill = newFill;
				}
				else if (ellipse.Fill == Brushes.Green)
				{
					SolidColorBrush newFill = new SolidColorBrush(Color.FromRgb(104, 210, 104));
					ellipse.Fill = newFill;
				}
			}
			else if (sender is Border border)
			{
				SolidColorBrush newFill = new SolidColorBrush(Color.FromRgb(104, 210, 104));
				MiniElli.Fill = newFill;
			}

		}

		private void Ellipse_MouseLeave(object sender, MouseEventArgs e)
		{
			if (sender is Ellipse ellipse)
			{
				if (ellipse.Name == "MiniElli") ellipse.Fill = Brushes.Green;
				else if (ellipse.Name == "CloseElli") ellipse.Fill = Brushes.Red;
			}
			else if (sender is Border border)
			{

				if (border.Parent is Ellipse ellipse1)
				{
					if (ellipse1.Name == "MiniElli") ellipse1.Fill = Brushes.Green;
				}
			}
		}

		private void AddNew_MouseEnter(object sender, MouseEventArgs e)
		{

		}

		private void AddNew_MouseLeave(object sender, MouseEventArgs e)
		{

		}

		private void AddContent_Click(object sender, RoutedEventArgs e)
		{
			var openFileDialog1 = new OpenFileDialog();
			openFileDialog1.Filter = "Text Files (*.txt)|*.txt";
			bool? res = openFileDialog1.ShowDialog(this);
			if (res == true)
			{
				string path = openFileDialog1.FileName;
				string targetFolderPath = AppDomain.CurrentDomain.BaseDirectory;
				File.Copy(path, System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), System.IO.Path.GetFileName(path)), true);
				string filename = System.IO.Path.GetFileNameWithoutExtension(path);
				// 遍历 ListBox 中的每个项，并将其内容添加到数组中

				foreach (var child in models.Children)
				{
					if (child is ModelItem modelItem)
					{
						if (filename == modelItem.filename) return;
					}
				}

				AddNewItem(filename);
			}
		}

		private void ModelItem_MouseDown(object sender, MouseButtonEventArgs e)
		{
			timer.Start();
			// 获取当前点击的ModelItem
			var clickedItem = sender as ModelItem;
			if (clickedItem == null)
			{
				return;
			}

			// 遍历StackPanel中的所有ModelItem，并将它们的IsActive属性设为true
			foreach (var child in models.Children)
			{
				if (child is ModelItem modelItem)
				{
					modelItem.IsActive = (modelItem == clickedItem);
				}
			}

			List<UIElement> list = new List<UIElement>();
			foreach (var child in models.Children)
			{
				if (child is ModelItem modelItem)
				{
					if (modelItem.Visibility == Visibility.Collapsed) list.Add(modelItem);
				}
			}
			foreach (var toDel in list)
			{
				models.Children.Remove(toDel);
			}
			//update info
			if (clickedItem != null)
			{
				file_name = clickedItem.FileName;
				try
				{
					string filename = clickedItem.FileName;
					filename += ".txt";
					StreamReader file = new StreamReader(filename);
					file_name = filename;
					List<string> K = new List<string>();  // q0,q1,...,qn finite set of states
					List<string> VT = new List<string>(); // Σ allowed symbols on the input tape
					string terminateState;  // terminal state
					string blank;           // blank symbol
					string initialState;    // initial state
					Dictionary<Tuple<string, string>, Tuple<string, string, char>> delta = new Dictionary<Tuple<string, string>, Tuple<string, string, char>>();
					readTuringMachine(filename, K, VT, out blank, out initialState, out terminateState, delta);
					StateSet.Text = string.Join(",", K);
					Symbols.Text = string.Join(",", VT);
					blankSymbol.Text = blank;
					terminalState.Text = terminateState;
					//InitialState.Text = initialState;
					//indicator.Text = initialState;
					//blankLabel.Text = blank;
					RemoveTapes();
					DeltaList.Items.Clear();
					foreach (var record in delta)
					{
						string state1 = record.Key.Item1;
						string symbol1 = record.Key.Item2;
						string state2 = record.Value.Item1;
						string symbol2 = record.Value.Item2;
						char movement = record.Value.Item3;

						string item = $"δ({state1},{symbol1})=({state2},{symbol2},{movement})";
						DeltaList.Items.Add(item);

					}
					file.Close();
					TitleText.Text = clickedItem.FileName;
				}
				catch (FileNotFoundException ex)
				{
					MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				catch (IndexOutOfRangeException)
				{
					MessageBox.Show("此txt文件似乎不合法", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private void ModelItem_MouseUp(object sender, MouseButtonEventArgs e)
		{
			timer.Stop();
			List<UIElement> list = new List<UIElement>();
			foreach (var child in models.Children)
			{
				if (child is ModelItem modelItem)
				{
					if (modelItem.Visibility == Visibility.Collapsed) list.Add(modelItem);
				}
			}
			foreach (var toDel in list)
			{
				models.Children.Remove(toDel);
			}
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			// 停止计时器
			timer.Stop();
			List<UIElement> list = new List<UIElement>();
			foreach (var child in models.Children)
			{
				if (child is ModelItem modelItem)
				{
					if (modelItem.Visibility == Visibility.Collapsed) list.Add(modelItem);
				}
			}
			foreach (var toDel in list)
			{
				models.Children.Remove(toDel);
			}
		}

		private bool IsSelected()
		{
			// 遍历StackPanel中的所有ModelItem，并将它们的IsActive属性设为true
			foreach (var child in models.Children)
			{
				if (child is ModelItem modelItem)
				{
					if (modelItem.IsActive) { return true; }
				}
			}
			return false;
		}

		void readTuringMachine(string filename, List<string> k, List<string> vt, out string blank, out string q0, out string qf, Dictionary<Tuple<string, string>, Tuple<string, string, char>> delta)
		{
			blank = null; q0 = null; qf = null;
			StreamReader file = new StreamReader(filename);
			if (file == null)
			{
				//Console.WriteLine("Failed to open file.");
				return;
			}

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
						k.Add(token);
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
						vt.Add(token);
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
					q0 = line.Substring(pos + 1); // 提取 q0 的字符串部分
				}
			}

			// 读取 qf
			if ((line = file.ReadLine()) != null)
			{
				int pos = line.IndexOf("=");
				if (pos >= 0)
				{
					qf = line.Substring(pos + 1); // 提取 qf 的字符串部分
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
			if (blank == string.Empty || qf == string.Empty || q0 == string.Empty) throw new Exception("此txt似乎不合法");
			if (blank == null || qf == null || q0 == null) throw new Exception("此txt似乎不合法");
			file.Close();
		}


		bool processed = false;
		bool first = true;
		bool IsStep = false;
		bool flag = true;
		int prePos = 0;
		string file_name;
		List<string>? tempStr;
		private TuringMachine TM = new TuringMachine();
		private void ReadTape_Click(object sender, RoutedEventArgs e)
		{
			if (InputTape.Text == string.Empty || !IsSelected())
			{
				RemoveTapes();
				return;
			}
			RemoveTapes();
			TM = new TuringMachine();
			foreach (char token in InputTape.Text)
			{
				AppendChar(token.ToString());
			}
			UIElement item = TapePanel.Children[1];
			if (item is TapeChar t)
			{
				t.StateName = InitialState.Text;
				t.IsActive = true;
			}
			StartExecute.IsEnabled = true;
		}

		private void StartExecute_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				IsStep = true;
				TM.readTuringMachine(file_name);
				string input = InputTape.Text;
				List<string> str = new List<string>();
				foreach (var token in input)
				{
					str.Add(token.ToString());
				}
				if (IsStep)
				{
					TM.switchMode(true);
				}
				else TM.switchMode(false);
				if (first)
				{
					ColorAnimation animation = new ColorAnimation();
					animation.Duration = TimeSpan.FromSeconds(1);
					animation.To = Color.FromArgb(0xdf, 0xeb, 0x26, 0x26);//#EB2626
					MainBorder.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
					first = false;
					tempStr = str;
					prePos = 0;
					ReadTape.IsEnabled = false;
					models.IsEnabled = false;
					TitleText.Text += " 运行中";
				}
				flag = TM.ReadString(tempStr);
				if (!flag)
				{
					MessageBox.Show(this, "Invalid string!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
					TitleText.Text = System.IO.Path.GetFileNameWithoutExtension(file_name);
					models.IsEnabled = true;
					StartExecute.IsEnabled = false;
					ReadTape.IsEnabled = true;
					first = true;
					ColorAnimation animation = new ColorAnimation();
					animation.Duration = TimeSpan.FromSeconds(1.5);
					animation.To = Color.FromArgb(0xdf, 0xef, 0x88, 0xbe);//#EB2626
					MainBorder.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
					return;
				}
				//Tape.Text = TM.GetTape();
				RemoveTapes();
				string currTape = TM.GetTape();
				foreach (char token in currTape)
				{
					AppendChar(token.ToString());
				}
				if (IsStep)
				{
					Tuple<string, string> record = TM.GetBefore();
					string state1 = record.Item1;
					string symbol1 = record.Item2;
					string targetString = $"({state1},{symbol1})";
					for (int i = 0; i < DeltaList.Items.Count; i++)
					{
						if (DeltaList.Items[i].ToString().Contains(targetString))
						{
							DeltaList.SelectedItem = DeltaList.Items[i]; // 选中匹配的项
						}
					}
					TM.switchMode(true);
					UIElement item = TapePanel.Children[prePos + 1];
					if (item is TapeChar t)
					{
						t.IsActive = false;
					}
					int pos = TM.GetPos();
					item = TapePanel.Children[pos + 1];
					if (item is TapeChar t1)
					{
						t1.StateName = TM.GetState();
						t1.IsActive = true;
					}
					prePos = TM.GetPos();
				}
				else TM.switchMode(false);
				if (TM.GetState() == TM.Getqf())
				{
					StartExecute.IsEnabled = false;
					ReadTape.IsEnabled = true;
					first = true;
					tempStr = null;
					TitleText.Text = System.IO.Path.GetFileNameWithoutExtension(file_name);
					models.IsEnabled = true;
					ColorAnimation animation = new ColorAnimation();
					animation.Duration = TimeSpan.FromSeconds(1.5);
					animation.To = Color.FromArgb(0xdf, 0xef, 0x88, 0xbe);//#EB2626
					MainBorder.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
					MessageBox.Show(this, "String accpect!", "Congratulations", MessageBoxButton.OK, MessageBoxImage.Information);
				}
			}
			catch (Exception ex)
			{
				RemoveTapes();
				TM = new TuringMachine();
				first = true;
				tempStr = null;
				MessageBox.Show(this,"Opps! Something went wrong!\n" + ex.Message,"Error!",MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void DeltaList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if(DeltaList.Items.Count > 0) { DeltaFunctionExprText.Text = DeltaList.SelectedItem.ToString(); }
			
		}


		private DispatcherTimer timer1;


		private void Timer1_Tick(object sender, EventArgs e)
		{
			TimeText.Text = DateTime.Now.ToString();
		}

		private void Button_Click_2(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Normal;
		}

		private void CopyRight_MouseDown(object sender, MouseButtonEventArgs e)
		{
			System.Diagnostics.Process.Start("explorer.exe","https://github.com/a-M1NG/WPFUniversalTuringMachine");
		}
	}

	public class TextBoxWidthConverter:IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is double )
			{			
				return (double)value - 160;
			}
			return 350;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
