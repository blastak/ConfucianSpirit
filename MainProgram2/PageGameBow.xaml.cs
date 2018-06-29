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
	/// Interaction logic for PageGameBow.xaml
	/// </summary>
	public partial class PageGameBow : Page
	{
		string m_strbase = @"pack://application:,,/";
		public event EventHandler m_evtPageFinish;
		public event EventHandler m_evtBindHand;
		public event EventHandler m_evtUnBindHand;
		public event EventHandler m_evtBindBGRemoval;
		public event EventHandler m_evtUnBindBGRemoval;
		public event EventHandler m_evtBindSkeletonImage;
		public event EventHandler m_evtUnBindSkeletonImage;

		public DispatcherTimer m_timerPageFinish = new DispatcherTimer();

		private MediaPlayer m_soundBackground = new MediaPlayer();

		public bool m_bMaleOrNot;
		public bool m_bSkip;

		public MyKinectSensor m_myKinect = null;

		public PageGameBow()
		{
			InitializeComponent();
			m_timerPageFinish.Interval = TimeSpan.FromSeconds(1); // 시간 고쳐야함
			m_timerPageFinish.Tick += new EventHandler(TimerPageFinish);

			m_soundBackground.Open(new Uri("Media/" + "PageGameBow_배경음악.mp3", UriKind.Relative));
			m_soundBackground.Volume = 1;
			m_soundBackground.MediaEnded += new EventHandler(BackgroundMusicEnd);
		}

		private void BackgroundMusicEnd(object sender, EventArgs e)
		{
			m_soundBackground.Position = TimeSpan.Zero;
		}


		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			m_imgTop.Visibility = Visibility.Hidden;
			m_btnMale.Visibility = Visibility.Hidden;
			m_btnFemale.Visibility = Visibility.Hidden;
			m_imgTop2.Visibility = Visibility.Hidden;
			m_imgMaleFemale.Visibility = Visibility.Hidden;
			m_imgTop3.Visibility = Visibility.Hidden;
			m_imgSelectedMF.Visibility = Visibility.Hidden;
			m_imgTop4.Visibility = Visibility.Hidden;
			m_imgUserBG.Visibility = Visibility.Hidden;
			m_imgUserSkel.Visibility = Visibility.Hidden;
			m_imgTop5.Visibility = Visibility.Hidden;
			m_imgMiddle1.Visibility = Visibility.Hidden;
			m_imgMiddle2.Visibility = Visibility.Hidden;

			m_imgTop.Visibility = Visibility.Visible;
			m_btnMale.Visibility = Visibility.Visible;
			m_btnFemale.Visibility = Visibility.Visible;

			// 배경음악 시작
			m_soundBackground.Position = TimeSpan.Zero;
			m_soundBackground.Play();

			m_evtBindHand(null, null);
		}

		private void m_btnMale_Click(object sender, RoutedEventArgs e)
		{
			m_bMaleOrNot = true;
			m_imgSelectedMF.Source = new BitmapImage(new Uri(m_strbase + "Images/" + "PageGameBow_04_선택_남자.png"));
			GameStart();
		}

		private void m_btnFemale_Click(object sender, RoutedEventArgs e)
		{
			m_bMaleOrNot = false;
			m_imgSelectedMF.Source = new BitmapImage(new Uri(m_strbase + "Images/" + "PageGameBow_04_선택_여자.png"));
			GameStart();
		}

		int idx;
		int cntTimer;
		int step_bow = -1;
		private void GameStart()
		{
			m_evtUnBindHand(null, null);

			step_bow = -1;
			if (m_myKinect.sensorChooser != null)
			{
				m_myKinect.evtReadySingleSkel += new EventHandler<AllFramesReadyEventArgs>(EventCheckHandOver);
			}

			m_bSkip = false;
			idx = 0;
			cntTimer = 0;
			m_timerPageFinish.Start();
		}

		private void TimerPageFinish(object sender, EventArgs e)
		{
			cntTimer++;

			if (m_bSkip == true)
			{
				idx = 4;
				cntTimer = 0;
				m_bSkip = false;
			}

			if (idx == 0 && cntTimer <= 3)
			{
				m_imgTop.Visibility = Visibility.Hidden;
				m_btnMale.Visibility = Visibility.Hidden;
				m_btnFemale.Visibility = Visibility.Hidden;
				m_imgTop2.Visibility = Visibility.Hidden;
				m_imgMaleFemale.Visibility = Visibility.Hidden;
				m_imgTop3.Visibility = Visibility.Hidden;
				m_imgSelectedMF.Visibility = Visibility.Hidden;
				m_imgTop4.Visibility = Visibility.Hidden;
				m_imgUserBG.Visibility = Visibility.Hidden;
				m_imgUserSkel.Visibility = Visibility.Hidden;
				m_imgTop5.Visibility = Visibility.Hidden;
				m_imgMiddle1.Visibility = Visibility.Hidden;
				m_imgMiddle2.Visibility = Visibility.Hidden;

				m_imgTop2.Visibility = Visibility.Visible; // 남자는 왼손이 위에 여자는 오른손이 위에
				m_imgMaleFemale.Visibility = Visibility.Visible; // 남녀 둘다 나옴

				if (cntTimer == 3)
				{
					idx++;
					cntTimer = 0;
				}
			}
			else if (idx == 1 && cntTimer <= 3)
			{
				m_imgTop.Visibility = Visibility.Hidden;
				m_btnMale.Visibility = Visibility.Hidden;
				m_btnFemale.Visibility = Visibility.Hidden;
				m_imgTop2.Visibility = Visibility.Hidden;
				m_imgMaleFemale.Visibility = Visibility.Hidden;
				m_imgTop3.Visibility = Visibility.Hidden;
				m_imgSelectedMF.Visibility = Visibility.Hidden;
				m_imgTop4.Visibility = Visibility.Hidden;
				m_imgUserBG.Visibility = Visibility.Hidden;
				m_imgUserSkel.Visibility = Visibility.Hidden;
				m_imgTop5.Visibility = Visibility.Hidden;
				m_imgMiddle1.Visibility = Visibility.Hidden;
				m_imgMiddle2.Visibility = Visibility.Hidden;

				m_imgTop3.Visibility = Visibility.Visible; // 팔을 벌리시오
				m_imgSelectedMF.Visibility = Visibility.Visible; // 내가 선택한 성별 그림

				if (cntTimer == 3)
				{
					idx++;
					cntTimer = 0;
				}
			}
			else if (idx == 2)
			{
				m_imgTop.Visibility = Visibility.Hidden;
				m_btnMale.Visibility = Visibility.Hidden;
				m_btnFemale.Visibility = Visibility.Hidden;
				m_imgTop2.Visibility = Visibility.Hidden;
				m_imgMaleFemale.Visibility = Visibility.Hidden;
				m_imgTop3.Visibility = Visibility.Hidden;
				m_imgSelectedMF.Visibility = Visibility.Hidden;
				m_imgTop4.Visibility = Visibility.Hidden;
				m_imgUserBG.Visibility = Visibility.Hidden;
				m_imgUserSkel.Visibility = Visibility.Hidden;
				m_imgTop5.Visibility = Visibility.Hidden;
				m_imgMiddle1.Visibility = Visibility.Hidden;
				m_imgMiddle2.Visibility = Visibility.Hidden;

				m_imgTop4.Visibility = Visibility.Visible;
				m_imgUserBG.Visibility = Visibility.Visible;
				m_imgUserSkel.Visibility = Visibility.Visible;

				m_evtBindBGRemoval(m_imgUserBG, null);
				m_evtBindSkeletonImage(m_imgUserSkel, null);

				idx++;
				cntTimer = 0;

				step_bow = 0;
			}
			else if (idx == 3) // 무한 대기
			{

			}
			else if (idx == 4 && cntTimer <= 5) // 정확히 맞췄을 때만 들어옴
			{
				m_imgTop.Visibility = Visibility.Hidden;
				m_btnMale.Visibility = Visibility.Hidden;
				m_btnFemale.Visibility = Visibility.Hidden;
				m_imgTop2.Visibility = Visibility.Hidden;
				m_imgMaleFemale.Visibility = Visibility.Hidden;
				m_imgTop3.Visibility = Visibility.Hidden;
				m_imgSelectedMF.Visibility = Visibility.Hidden;
				m_imgTop4.Visibility = Visibility.Hidden;
				m_imgUserBG.Visibility = Visibility.Hidden;
				m_imgUserSkel.Visibility = Visibility.Hidden;
				m_imgTop5.Visibility = Visibility.Hidden;
				m_imgMiddle1.Visibility = Visibility.Hidden;
				m_imgMiddle2.Visibility = Visibility.Hidden;

				m_imgTop5.Visibility = Visibility.Visible;
				m_imgMiddle1.Visibility = Visibility.Visible;
				m_imgMiddle2.Visibility = Visibility.Visible;

				if (cntTimer == 5)
				{
					idx++;
					cntTimer = 0;
				}
			}
			else if (idx == 5)
			{
				m_evtUnBindBGRemoval(null, null);
				m_evtUnBindSkeletonImage(null, null);

				step_bow = -1;
				if (m_myKinect.sensorChooser != null)
				{
					m_myKinect.evtReadySingleSkel -= new EventHandler<AllFramesReadyEventArgs>(EventCheckHandOver);
				}

				m_timerPageFinish.Stop();

				// 배경음악 종료
				m_soundBackground.Stop();

				m_evtPageFinish(null, null);

				idx++;
				cntTimer = 0;
			}
		}


		bool isFirstHandRightSameAsMale = false;
		private void EventCheckHandOver(object sender, AllFramesReadyEventArgs e)
		{
			if (step_bow < 0)
				return;

			Skeleton player = (Skeleton)sender;

			float shoulderLeftX = player.Joints[JointType.ShoulderLeft].Position.X;
			float shoulderRightX = player.Joints[JointType.ShoulderRight].Position.X;
			float wristLeftX = player.Joints[JointType.WristLeft].Position.X;
			float wristRightX = player.Joints[JointType.WristRight].Position.X;

			bool isWristLeftOutside = wristLeftX < shoulderLeftX;
			bool isWristRightOutside = wristRightX > shoulderRightX;

			if (isWristLeftOutside == true && isWristRightOutside == true) // 손을 벌림
			{
				step_bow = 0;
				return;
			}

			if (step_bow == 0)
			{
				if (isWristLeftOutside != isWristRightOutside) // 한손만 안으로
				{
					if (isWristRightOutside == false)
						isFirstHandRightSameAsMale = true; // 첫번째 들어온 손이 오른손이면
					else
						isFirstHandRightSameAsMale = false;

					step_bow = 1;
				}
			}
			else if (step_bow == 1)
			{
				if (isWristLeftOutside == isWristRightOutside) // 두손다 안으로
				{
					step_bow = 2;
				}
			}
			else if (step_bow == 2)
			{
				if (isFirstHandRightSameAsMale == m_bMaleOrNot) // 남자선택하고 남자같이 했으면
				{
					idx = 4;
					cntTimer = 0;
					step_bow = -1;
				}
				else // 남자선택하고 여자같이 했거나, 여자선택하고 남자같이 했거나
				{
					idx = 0;
					cntTimer = 0;

					step_bow = 0;
				}
			}
		}
	}
}
