using Microsoft.Kinect;
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
		public event EventHandler m_evtBindSkeletonImage;
		public event EventHandler m_evtUnBindSkeletonImage;

		public DispatcherTimer m_timerPageFinish = new DispatcherTimer();
		public DispatcherTimer m_timerPageFinish2 = new DispatcherTimer();

		private MediaPlayer m_soundIntroBackground = new MediaPlayer();
		private MediaPlayer m_soundBackground = new MediaPlayer();

		public int m_nScore;
		public int m_cntRemainSecond;
		public bool m_bSkip;

		public MyKinectSensor m_myKinect = null;

		public PageGame3()
		{
			InitializeComponent();

			m_soundIntroBackground.Open(new Uri("Media/" + "PageGame공통_인트로_배경음악.mp3", UriKind.Relative));
			m_soundIntroBackground.Volume = 1;

			m_soundBackground.Open(new Uri("Media/" + "PageGame3_배경음악.mp3", UriKind.Relative));
			m_soundBackground.Volume = 1;

			m_timerPageFinish.Interval = TimeSpan.FromSeconds(1);
			m_timerPageFinish.Tick += new EventHandler(TimerPageFinish);

			m_timerPageFinish2.Interval = TimeSpan.FromSeconds(3); // 답 제출 후 3초 대기용
			m_timerPageFinish2.Tick += new EventHandler(TimerPageFinish2);
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			m_labelRemainSecond.Visibility = Visibility.Hidden;
			m_videoLeft.Visibility = Visibility.Hidden;
			m_videoRight.Visibility = Visibility.Hidden;
			m_btnVideoLeft.Visibility = Visibility.Hidden;
			m_btnVideoRight.Visibility = Visibility.Hidden;

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

		int m_numRandom = 0;
		private void m_btnNext_Click(object sender, RoutedEventArgs e)
		{
			m_videoIntro.Stop();

			m_videoIntro.Visibility = Visibility.Hidden;
			m_btnNext.Visibility = Visibility.Hidden;

			m_numRandom = RandomNumber(1, 2 + 1);
			if (m_numRandom == 1)
			{
				m_videoLeft.Source = new Uri("Media/" + "PageGame3_보기1.mp4", UriKind.Relative);
				m_videoRight.Source = new Uri("Media/" + "PageGame3_보기2.mp4", UriKind.Relative);
			}
			else
			{
				m_videoLeft.Source = new Uri("Media/" + "PageGame3_보기2.mp4", UriKind.Relative);
				m_videoRight.Source = new Uri("Media/" + "PageGame3_보기1.mp4", UriKind.Relative);
			}

			m_videoLeft.Visibility = Visibility.Visible;
			m_videoRight.Visibility = Visibility.Visible;
			m_btnVideoLeft.Visibility = Visibility.Visible;
			m_btnVideoRight.Visibility = Visibility.Visible;

			m_videoLeft.Position = TimeSpan.Zero;
			m_videoRight.Position = TimeSpan.Zero;
			m_videoLeft.Play();
			m_videoRight.Play();

			// 인트로 배경음악 종료
			m_soundIntroBackground.Stop();

			// 배경음악 시작
			m_soundBackground.Position = TimeSpan.Zero;
			m_soundBackground.Play();

			// kinect skeleton image on
			m_imgSkeleton.Visibility = Visibility.Visible;
			m_evtBindSkeletonImage(m_imgSkeleton, null);

			if (m_myKinect.sensorChooser != null)
			{
				m_myKinect.evtReadySingleSkel += new EventHandler<AllFramesReadyEventArgs>(EventCheckHandOver);
			}

			m_bSkip = false;
			m_nScore = 0;
			m_cntRemainSecond = 60;
			m_labelRemainSecond.Content = m_cntRemainSecond;
			m_labelRemainSecond.Visibility = Visibility.Visible;
			m_timerPageFinish.Start();
		}

		private void TimerPageFinish(object sender, EventArgs e)
		{
			m_labelRemainSecond.Content = m_cntRemainSecond;

			if (m_cntRemainSecond < 0 || m_bSkip == true)
			{
				// 타이머 종료
				m_timerPageFinish.Stop();

				// 동영상 종료
				m_videoLeft.Stop();
				m_videoRight.Stop();

				// kinect control off
				m_evtUnBindHand(null, null);

				if (m_myKinect.sensorChooser != null)
				{
					m_myKinect.evtReadySingleSkel -= new EventHandler<AllFramesReadyEventArgs>(EventCheckHandOver);
				}
				
				// 얼굴 교체
				if(m_nScore >= 10) // 맞았을때
				{
					m_myKinect.m_skelFaceMode = 2;
				}
				else // 틀렸을때
				{
					m_myKinect.m_skelFaceBrush = new ImageBrush(new BitmapImage(new Uri(m_strbase + "Images/" + "PageGame3_03_틀렸을때.png")));
					m_myKinect.m_skelFaceMode = 1;
				}

				m_timerPageFinish2.Start();
			}
			else
			{
				m_cntRemainSecond--;
			}
		}

		private void TimerPageFinish2(object sender, EventArgs e)
		{
			m_timerPageFinish2.Stop();

			// kinect skeleton image off
			m_imgSkeleton.Visibility = Visibility.Hidden;
			m_myKinect.m_skelFaceMode = 0;
			m_evtUnBindSkeletonImage(null, null);

			// 배경음악 종료
			m_soundBackground.Stop();

			// 페이지 종료
			if (m_nScore >= 10)
			{
				m_evtPageFinish(true, null);
			}
			else
			{
				m_evtPageFinish(false, null);
			}

		}


		private void m_btnVideoLeft_Click(object sender, RoutedEventArgs e)
		{
			if (playerPos == -1) // 왼쪽
			{
				if (m_numRandom == 1) // random 1은 왼쪽이 정답
				{
					m_nScore = 10;
					m_bSkip = true;
				}
				else
				{
					m_nScore = 0;
					m_bSkip = true;
				}
			}
		}

		private void m_btnVideoRight_Click(object sender, RoutedEventArgs e)
		{
			if (playerPos == 1) // 오른쪽
			{
				if (m_numRandom == 2) // random 2은 오른쪽이 정답
				{
					m_nScore = 10;
					m_bSkip = true;
				}
				else
				{
					m_nScore = 0;
					m_bSkip = true;
				}
			}
		}

		private int RandomNumber(int min, int max)
		{
			Random random = new Random(DateTime.Now.Millisecond);
			return random.Next(min, max);
		}

		int playerPos = 0; // -1은 왼쪽, 0은 가운데, 1은 오른쪽
		private void EventCheckHandOver(object sender, AllFramesReadyEventArgs e)
		{
			Skeleton player = (Skeleton)sender;

			int cntLeft = 0;
			int cntRight = 0;
			for (int i = 0; i < 20; i++)
			{
				if (player.Joints[(JointType)i].Position.X < 0) // 왼쪽에 있을 경우
				{
					cntLeft++;
				}
				else
				{
					cntRight++;
				}
			}

			if (cntLeft == 20)
			{
				playerPos = -1;
			}
			else if (cntRight == 20)
			{
				playerPos = 1;
			}
			else
			{
				playerPos = 0;
			}
		}
	}
}
