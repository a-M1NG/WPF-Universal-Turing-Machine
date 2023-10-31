using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
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
	/// ModelItem.xaml 的交互逻辑
	/// </summary>
	public partial class ModelItem : UserControl, INotifyPropertyChanged
	{
		public ModelItem()
		{
			InitializeComponent();
			timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromSeconds(2);
			timer.Tick += Timer_Tick;
		}

		public string filename = "";

		public string FileName
		{
			get { return (string)GetValue(FileNameProperty); }
			set 
			{
				if (value != filename)
				{
					filename = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FileName)));
				}
				SetValue(FileNameProperty, value); 
			}
		}

		public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string),typeof(ModelItem));

		public bool _isActive = false;

		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				if (_isActive != value)
				{
					_isActive = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsActive)));
					if (!IsActive)
					{
						ColorAnimation animation = new ColorAnimation();
						SolidColorBrush newFill = new SolidColorBrush(Color.FromRgb(0x60, 0x8d, 0xff));//#608dff
						animation.To = Color.FromRgb(0x60, 0x8d, 0xff);
						animation.Duration = TimeSpan.FromSeconds(0.2);
						ItemBorder.Background.BeginAnimation(SolidColorBrush.ColorProperty,animation);
					}
				}
				SetValue(IsActiveProperty, value);
			}
		}

		public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(ModelItem));

		public event PropertyChangedEventHandler? PropertyChanged;

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		DispatcherTimer timer;

		private void Timer_Tick(object sender, EventArgs e)
		{
			// 停止计时器
			timer.Stop();
			// 创建一个DoubleAnimation
			DoubleAnimation da = new DoubleAnimation();
			DoubleAnimation animation = new DoubleAnimation();
			animation.To = 0;
			animation.Duration = TimeSpan.FromSeconds(1);
			da.From = 1; // 开始值
			da.To = 0; // 结束值
			da.Duration = new Duration(TimeSpan.FromSeconds(1)); // 持续时间
																 // 当动画完成时，隐藏控件
			da.Completed += (s, _) => this.Visibility = Visibility.Collapsed;
			// 开始动画
			ItemBorder.BeginAnimation(Border.HeightProperty,animation);
			//ItemBorder.BeginAnimation(Border.WidthProperty, animation);
			this.BeginAnimation(UIElement.OpacityProperty, da);
		}

		private void Border_MouseDown(object sender, MouseButtonEventArgs e)
		{
			timer.Start();
		}

		private void Border_MouseUp(object sender, MouseButtonEventArgs e)
		{
			timer.Stop();
		}

		private void Border_MouseLeave(object sender, MouseEventArgs e)
		{
			//if (sender is Border border)
			//{
			//	SolidColorBrush newFill = new SolidColorBrush(Color.FromRgb(0x60, 0x8d, 0xff));//#608dff
			//	if (this.IsActive)
			//	{
			//		border.Background = newFill;
			//	}
			//	else
			//		border.Background = Brushes.Transparent;
			//}
		}

		private void Border_MouseEnter(object sender, MouseEventArgs e)
		{
			//if (sender is Border border)
			//{
			//	SolidColorBrush newFill = new SolidColorBrush(Color.FromRgb(0x7b, 0xae, 0xff));//#608dff
			//	border.Background = newFill;
			//}
		}
	}

	public class BooleanToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			SolidColorBrush newFill = new SolidColorBrush(Color.FromRgb(0x60, 0x8d, 0xff));//#608dff
			if (value is bool isActive && isActive)
			{
				return (Color)ColorConverter.ConvertFromString(parameter.ToString());
			}

			return newFill;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
