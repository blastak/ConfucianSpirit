using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

namespace MainProgram2
{
	/// <summary>
	/// Interaction logic for PageGameBow.xaml
	/// </summary>
	public partial class PageGameBow : Page
	{
		string m_strbase = @"pack://application:,,/";
		public event EventHandler m_evtPageFinish;
		public DispatcherTimer m_timerPageFinish = new DispatcherTimer();

		public PageGameBow()
		{
			InitializeComponent();
			m_timerPageFinish.Interval = TimeSpan.FromSeconds(5); // 시간 고쳐야함
			m_timerPageFinish.Tick += new EventHandler(TimerPageFinish);
		}


		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			m_timerPageFinish.Start();
		}

		private void TimerPageFinish(object sender, EventArgs e)
		{
			m_timerPageFinish.Stop();

			m_evtPageFinish(null, null);
		}
	}
}
