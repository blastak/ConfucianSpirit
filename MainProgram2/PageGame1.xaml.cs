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
	/// Interaction logic for PageGame1.xaml
	/// </summary>
	public partial class PageGame1 : Page
	{
		string m_strbase = @"pack://application:,,/";
		public event EventHandler m_evtPageFinish;

		public DispatcherTimer m_timerGameOver = new DispatcherTimer();


		int gravity_factor;

		public PageGame1()
		{
			InitializeComponent();

			m_timerGameOver.Interval = TimeSpan.FromSeconds(5); // 시간을 늘려야 함
			m_timerGameOver.Tick += new EventHandler(TimerGameOver);
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			m_btnHighSpeed.Visibility = Visibility.Hidden;
			m_btnLowSpeed.Visibility = Visibility.Hidden;

			m_videoIntro.Visibility = Visibility.Visible;
			m_btnNext.Visibility = Visibility.Visible;

			m_videoIntro.Position = TimeSpan.Zero;
			m_videoIntro.Play();
		}

		private void m_videoIntro_MediaEnded(object sender, RoutedEventArgs e)
		{
			// 인트로 동영상 반복재생
			m_videoIntro.Position = TimeSpan.Zero;
		}

		private void m_btnNext_Click(object sender, RoutedEventArgs e)
		{
			m_videoIntro.Stop();

			m_videoIntro.Visibility = Visibility.Hidden;
			m_btnNext.Visibility = Visibility.Hidden;

			m_btnHighSpeed.Visibility = Visibility.Visible;
			m_btnLowSpeed.Visibility = Visibility.Visible;

		}

		private void m_btnHighSpeed_Click(object sender, RoutedEventArgs e)
		{
			gravity_factor = 5;

			m_btnHighSpeed.Visibility = Visibility.Hidden;
			m_btnLowSpeed.Visibility = Visibility.Hidden;

			m_timerGameOver.Start();
		}

		private void m_btnLowSpeed_Click(object sender, RoutedEventArgs e)
		{
			gravity_factor = 2;

			m_btnHighSpeed.Visibility = Visibility.Hidden;
			m_btnLowSpeed.Visibility = Visibility.Hidden;

			m_timerGameOver.Start();
		}

		private void TimerGameOver(object sender, EventArgs e)
		{
			m_timerGameOver.Stop();

			m_evtPageFinish(null, null);
		}
	}
}
