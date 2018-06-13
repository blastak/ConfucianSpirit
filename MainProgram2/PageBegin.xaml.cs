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
	/// Interaction logic for PageBegin.xaml
	/// </summary>
	public partial class PageBegin : Page
	{
		string m_strbase = @"pack://application:,,/";
		public event EventHandler m_evtPageFinish;

		private MediaPlayer m_soundNarration = new MediaPlayer();
		public DispatcherTimer m_timerWaitNarration = new DispatcherTimer();

		private MediaPlayer m_soundBackground = new MediaPlayer();

		public PageBegin()
		{
			InitializeComponent();

			m_soundNarration.Open(new Uri("Media/" + "PageBegin_T포즈.m4a", UriKind.Relative));
			m_soundNarration.Volume = 1;

			m_soundBackground.Open(new Uri("Media/" + "PageBegin_배경음악.mp3", UriKind.Relative));
			m_soundBackground.Volume = 1;
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			// T자세 화면
			m_imgBackground.Source = new BitmapImage(new Uri(m_strbase + "Images/" + "PageBegin_T포즈.png"));

			// 나레이션 시작
			m_soundNarration.Position = TimeSpan.Zero;
			m_soundNarration.Play();

			// 다음 화면 넘어가는 타이머 설정
			m_timerWaitNarration.Interval = TimeSpan.FromSeconds(7); // 나레이션시간4초+대기3초
			m_timerWaitNarration.Tick += new EventHandler(TimerWaitNarration);
			m_timerWaitNarration.Start();
		}

		private void TimerWaitNarration(object sender, EventArgs e)
		{
			m_timerWaitNarration.Stop();

			// 나레이션 종료
			m_soundNarration.Stop();

			// 배경화면과 버튼보이기
			m_imgBackground.Source = new BitmapImage(new Uri(m_strbase + "Images/" + "PageBegin_배경화면.png"));
			m_btnGameStart.Visibility = Visibility.Visible;

			// 배경음악 시작
			m_soundBackground.Position = TimeSpan.Zero;
			m_soundBackground.Play();
		}

		private void m_btnGameStart_Click(object sender, RoutedEventArgs e)
		{
			// 배경음악 종료
			m_soundBackground.Stop();

			// 버튼 감추기
			m_btnGameStart.Visibility = Visibility.Hidden;

			// 페이지 종료 이벤트 발생 시킴
			m_evtPageFinish(sender, e);
		}
	}
}
