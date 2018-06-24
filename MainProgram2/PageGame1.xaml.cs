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

		private MediaPlayer m_soundBackground = new MediaPlayer();

		//public MyGameGravityTouch m_game3 = new MyGameGravityTouch();
		//public MyGameGravityCollect m_game5 = new MyGameGravityCollect();

		private int m_gravity_factor = 2;

		public int m_nScore;
		public int m_cntRemainSecond;
		public bool m_bSkip;

		public PageGame1()
		{
			InitializeComponent();

			m_soundBackground.Open(new Uri("Media/" + "PageGame1_배경음악.mp3", UriKind.Relative));
			m_soundBackground.Volume = 1;
			m_soundBackground.MediaEnded += new EventHandler(BackgroundMusicEnd);

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

		private void BackgroundMusicEnd(object sender, EventArgs e)
		{
			m_soundBackground.Position = TimeSpan.Zero;
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			m_labelRemainSecond.Visibility = Visibility.Hidden;
			m_labelScore.Visibility = Visibility.Hidden;
			m_btnHighSpeed.Visibility = Visibility.Hidden;
			m_btnLowSpeed.Visibility = Visibility.Hidden;
			m_imgTop.Visibility = Visibility.Hidden;

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

			m_imgTop.Visibility = Visibility.Visible;
			m_btnHighSpeed.Visibility = Visibility.Visible;
			m_btnLowSpeed.Visibility = Visibility.Visible;

		}

		private void m_btnHighSpeed_Click(object sender, RoutedEventArgs e)
		{
			m_gravity_factor = 5;

			GameStart();
		}

		private void m_btnLowSpeed_Click(object sender, RoutedEventArgs e)
		{
			m_gravity_factor = 2;

			GameStart();
		}

		string[] strBigCircles = { "PageGame1_04_깨끗이씻기_글포함.png", "PageGame1_04_부모님께반말하기_글포함.png", "PageGame1_04_선생님께인사_글포함.png", "PageGame1_04_손님한테인사 안하기_글포함.png", "PageGame1_04_스스로일어나기_글포함.png", "PageGame1_04_식전인사하기_글포함.png", "PageGame1_04_위험한장소가기_글포함.png", "PageGame1_04_인사거절하기_글포함.png", "PageGame1_04_주머니에손넣고인사_글포함.png", "PageGame1_04_집안청소_글포함.png", "PageGame1_04_형제와싸움_글포함.png", "PageGame1_04_하교후인사하기_글포함.png" };
		int idxBigCircle = 0;
		private void GameStart()
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

				m_bSkip = false;
				m_cntRemainSecond = 60;
				m_labelRemainSecond.Content = m_cntRemainSecond;
				m_labelRemainSecond.Visibility = Visibility.Visible;
				m_timerPageFinish.Start();
			}
			else
			{
				m_imgBigCircle.Source = new BitmapImage(new Uri(m_strbase + "Images/" + strBigCircles[idxBigCircle]));
			}
		}
		
		private void TimerPageFinish(object sender, EventArgs e)
		{
			m_labelRemainSecond.Content = m_cntRemainSecond;
			if (m_cntRemainSecond < 0 || m_bSkip == true)
			{
				// 타이머 종료
				m_timerPageFinish.Stop();

				// 배경음악 종료
				m_soundBackground.Stop();

				// 페이지 종료
				if(m_nScore >= 60)
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
