using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfTuringMachine
{
	/// <summary>
	/// TapeChar.xaml 的交互逻辑
	/// </summary>
	public partial class TapeChar : UserControl,INotifyPropertyChanged
	{
		public TapeChar()
		{
			InitializeComponent();
		}
		public event PropertyChangedEventHandler? PropertyChanged;

		public string CharName
		{
			get { return (string)GetValue(CharNameProperty); }
			set { SetValue(CharNameProperty, value); }
		}

		public static readonly DependencyProperty CharNameProperty = DependencyProperty.Register("CharName", typeof(string), typeof(TapeChar));

		public bool _isActive = false;

		public bool IsActive
		{
			get { return (bool)GetValue(IsActiveProperty); }
			set 
			{
				if(value!=_isActive)
				{
					_isActive = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsActive)));
					if (_isActive)
						StateBorder.Visibility = Visibility.Visible;
				}				
				SetValue(IsActiveProperty, value); 
			}
		}

		public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(TapeChar));

		public string StateName
		{
			get { return (string)GetValue(StateNameProperty); }
			set { SetValue(StateNameProperty, value); }
		}

		public static readonly DependencyProperty StateNameProperty = DependencyProperty.Register("StateName", typeof(string), typeof(TapeChar));
	}
}
