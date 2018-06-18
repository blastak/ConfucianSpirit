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
	/// Interaction logic for PageGame3.xaml
	/// </summary>
	public partial class PageGame3 : Page
	{
		string m_strbase = @"pack://application:,,/";
		public event EventHandler m_evtPageFinish;
		public event EventHandler m_evtBindHand;
		public event EventHandler m_evtUnBindHand;

		public DispatcherTimer m_timerPageFinish = new DispatcherTimer();

		private MediaPlayer m_soundBackground = new MediaPlayer();

		private int m_cntRemainSecond;
		public bool m_bSkip;

		public PageGame3()
		{
			InitializeComponent();

			m_soundBackground.Open(new Uri("Media/" + "PageGame3_배경음악.mp3", UriKind.Relative));
			m_soundBackground.Volume = 1;

			m_timerPageFinish.Interval = TimeSpan.FromSeconds(1);
			m_timerPageFinish.Tick += new EventHandler(TimerPageFinish);
		}
		
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			m_labelRemainSecond.Visibility = Visibility.Hidden;
			m_videoIntro.Visibility = Visibility.Visible;
			m_btnNext.Visibility = Visibility.Visible;

			m_videoIntro.Position = TimeSpan.Zero;
			m_videoIntro.Play();

			// 배경음악 시작
			m_soundBackground.Position = TimeSpan.Zero;
			m_soundBackground.Play();

			// kinect control on
			m_evtBindHand(null, null);
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

			// kinect control off
			m_evtUnBindHand(null, null);

			m_video1.Position = TimeSpan.Zero;
			m_video2.Position = TimeSpan.Zero;
			m_video1.Play();
			m_video2.Play();

			m_bSkip = false;
			m_cntRemainSecond = 60;
			m_labelRemainSecond.Content = m_cntRemainSecond;
			m_labelRemainSecond.Visibility = Visibility.Visible;
			m_timerPageFinish.Start();
		}

		private void TimerPageFinish(object sender, EventArgs e)
		{
			m_labelRemainSecond.Content = m_cntRemainSecond;

			if (m_cntRemainSecond < 0)
			{
				// 타이머 종료
				m_timerPageFinish.Stop();

				// 배경음악 종료
				m_soundBackground.Stop();

				// 페이지 종료
				m_evtPageFinish(null, null);
			}

			m_cntRemainSecond--;

			if (m_bSkip == true)
				m_cntRemainSecond = -1;
		}
	}
}
