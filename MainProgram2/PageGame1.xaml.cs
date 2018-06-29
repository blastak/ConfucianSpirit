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
		public event EventHandler m_evtBindHand;
		public event EventHandler m_evtUnBindHand;

		public DispatcherTimer m_timerPageFinish = new DispatcherTimer();
		public DispatcherTimer m_timerBigCircle = new DispatcherTimer();

		private MediaPlayer m_soundIntroBackground = new MediaPlayer();
		private MediaPlayer m_soundBackground1 = new MediaPlayer();
		private MediaPlayer m_soundBackground2 = new MediaPlayer();
		private MediaPlayer m_soundBackground3 = new MediaPlayer();

		public MyGameGravity m_gameGravity = new MyGameGravity();

		public MyKinectSensor m_myKinect = null;

		private int m_gravity_factor = 2;

		public int m_nScore;
		public int m_cntRemainSecond;
		public bool m_bSkip;

		public PageGame1()
		{
			InitializeComponent();

			m_soundIntroBackground.Open(new Uri("Media/" + "PageGame공통_인트로_배경음악.mp3", UriKind.Relative));
			m_soundIntroBackground.Volume = 1;

			m_soundBackground1.Open(new Uri("Media/" + "PageGame1_배경음악_1.mp3", UriKind.Relative));
			m_soundBackground1.Volume = 1;
			m_soundBackground2.Open(new Uri("Media/" + "PageGame1_배경음악_2.mp3", UriKind.Relative));
			m_soundBackground2.Volume = 1;
			m_soundBackground3.Open(new Uri("Media/" + "PageGame1_배경음악_3.wav", UriKind.Relative));
			m_soundBackground3.Volume = 1;

			m_timerPageFinish.Interval = TimeSpan.FromSeconds(1);
			m_timerPageFinish.Tick += new EventHandler(TimerPageFinish);

			m_timerBigCircle.Interval = TimeSpan.FromSeconds(2);
			m_timerBigCircle.Tick += new EventHandler(TimerBigCircle);

			//m_game3.SetupUI(this.canvasBG, this.canvasBG2, this.imgUser2, this.imgFace, this.imgIcon);
			//m_game3.SetupResource("경성_03_01(터트리기).png", "경성_03_01(터트리기).m4a");
			//m_game3.m_myKinect = kinectSensor;
			//m_game3.m_evtGameManager += new EventHandler(EventGameManager);
			//
			//m_game5.SetupUI(this.canvasBG, this.canvasBG2, this.imgUser2, this.imgFace, this.imgIcon);
			//m_game5.SetupResource("예효_05_01(바구니).png", "예효_05_01(바구니).m4a");
			//m_game5.m_myKinect = kinectSensor;
			//m_game5.m_evtGameManager += new EventHandler(EventGameManager);
		}
		

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			if(m_myKinect != null && m_gameGravity.m_myKinect == null)
			{
				m_gameGravity.m_myKinect = m_myKinect;
			}

			m_labelRemainSecond.Visibility = Visibility.Hidden;
			m_labelScore.Visibility = Visibility.Hidden;
			m_btnHighSpeed.Visibility = Visibility.Hidden;
			m_btnLowSpeed.Visibility = Visibility.Hidden;
			m_imgTop.Visibility = Visibility.Hidden;

			m_videoIntro.Visibility = Visibility.Visible;
			m_btnNext.Visibility = Visibility.Visible;

			m_videoIntro.Position = TimeSpan.Zero;
			m_videoIntro.Play();

			// 인트로 배경음악 시작
			m_soundIntroBackground.Position = TimeSpan.Zero;
			m_soundIntroBackground.Play();

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

			m_imgTop.Visibility = Visibility.Visible;
			m_btnHighSpeed.Visibility = Visibility.Visible;
			m_btnLowSpeed.Visibility = Visibility.Visible;

		}

		private void m_btnHighSpeed_Click(object sender, RoutedEventArgs e)
		{
			m_gravity_factor = 3;

			CallBigCircle();
		}

		private void m_btnLowSpeed_Click(object sender, RoutedEventArgs e)
		{
			m_gravity_factor = 1;

			CallBigCircle();
		}

		string[] strBigCircles = { "PageGame1_04_깨끗이씻기_글포함.png", "PageGame1_04_부모님께반말하기_글포함.png", "PageGame1_04_선생님께인사_글포함.png", "PageGame1_04_손님한테인사 안하기_글포함.png", "PageGame1_04_스스로일어나기_글포함.png", "PageGame1_04_식전인사하기_글포함.png", "PageGame1_04_위험한장소가기_글포함.png", "PageGame1_04_인사거절하기_글포함.png", "PageGame1_04_주머니에손넣고인사_글포함.png", "PageGame1_04_집안청소_글포함.png", "PageGame1_04_형제와싸움_글포함.png", "PageGame1_04_하교후인사하기_글포함.png" };
		int idxBigCircle = 0;
		private void CallBigCircle()
		{
			// kinect control off
			m_evtUnBindHand(null, null);

			m_imgTop.Visibility = Visibility.Hidden;
			m_btnHighSpeed.Visibility = Visibility.Hidden;
			m_btnLowSpeed.Visibility = Visibility.Hidden;
			
			idxBigCircle = 0;
			m_imgBigCircle.Source = new BitmapImage(new Uri(m_strbase + "Images/" + strBigCircles[idxBigCircle]));
			m_imgBigCircle.Visibility = Visibility.Visible;

			m_bSkip = false;
			m_timerBigCircle.Start();
		}

		private void TimerBigCircle(object sender, EventArgs e)
		{
			idxBigCircle++;

			if (idxBigCircle == strBigCircles.Length || m_bSkip == true)
			{
				m_timerBigCircle.Stop();
				m_imgBigCircle.Visibility = Visibility.Hidden;

				m_nScore = 0;
				m_labelScore.Content = m_nScore;
				m_labelScore.Visibility = Visibility.Visible;

				// 인트로 배경음악 종료
				m_soundIntroBackground.Stop();

				GravityGameStart();
			}
			else
			{
				m_imgBigCircle.Source = new BitmapImage(new Uri(m_strbase + "Images/" + strBigCircles[idxBigCircle]));
			}
		}

		private void GravityGameStart()
		{
			m_canvasSkel.Visibility = Visibility.Visible;
			m_gameGravity.SetCanvas(m_canvasSkel);

			m_gameGravity.SetGameSpeed(m_gravity_factor);
			m_gameGravity.SetGameMode(0);

			m_gameGravity.GameStart();

			// 배경음악1 시작
			m_soundBackground1.Position = TimeSpan.Zero;
			m_soundBackground1.Play();

			m_bSkip = false;
			m_cntRemainSecond = 60;
			m_labelRemainSecond.Content = m_cntRemainSecond;
			m_labelRemainSecond.Visibility = Visibility.Visible;
			m_timerPageFinish.Start();
		}
		
		private void TimerPageFinish(object sender, EventArgs e)
		{
			m_nScore = m_gameGravity.GetGameResult();

			if (m_nScore >= 20 && m_gameGravity.m_mode == 0)
			{
				// 배경음악1 종료
				m_soundBackground1.Stop();
				// 배경음악2 시작
				m_soundBackground2.Position = TimeSpan.Zero;
				m_soundBackground2.Play();

				m_gameGravity.SetGameMode(1);
			}
			else if (m_nScore >= 40 && m_gameGravity.m_mode == 1)
			{
				// 배경음악2 종료
				m_soundBackground2.Stop();
				// 배경음악3 시작
				m_soundBackground3.Position = TimeSpan.Zero;
				m_soundBackground3.Play();

				m_gameGravity.SetGameMode(2);
			}

			m_labelScore.Content = m_nScore;
			m_labelRemainSecond.Content = m_cntRemainSecond;
			if (m_cntRemainSecond < 0 || m_bSkip == true || m_nScore >= 60)
			{
				// 타이머 종료
				m_timerPageFinish.Stop();

				// 게임 종료
				m_gameGravity.GameEnd();

				// 배경음악3 종료
				m_soundBackground3.Stop();

				// 페이지 종료
				if (m_nScore >= 60)
				{
					m_evtPageFinish(true, null);
				}
				else
				{
					m_evtPageFinish(false, null);
				}
			}
			else
			{
				m_cntRemainSecond--;
			}
		}
	}
}
