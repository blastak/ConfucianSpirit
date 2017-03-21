using Microsoft.Kinect;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MainProgram
{
	public class MyGameHandUp
	{
		public event EventHandler m_evtGameManager;

		private Canvas m_canvas;
		private Image m_imgBlurMask;
		private Image m_imgOverlayLeft;
		private Image m_imgOverlayRight;
		private Image m_imgUserBody;
		private Image m_imgTFFace;

		private int m_timeRemain;

		private bool m_flgHandLeftUp;
		private bool m_flgHandLeftDown;
		private bool m_flgHandRightUp;
		private bool m_flgHandRightDown;
		private int m_cntOneHand;
		private int m_cntTwoHand;
		private int m_flgLeftRight;

		public MyKinectSensor m_myKinect;

		private MediaPlayer m_startSound = new MediaPlayer();
		private DispatcherTimer m_timerCountdown = new DispatcherTimer();

		private string m_strBackground;
		private string m_strBackgroundMask;
		private string m_strQuestionSound;
		private string m_strLeftOverlay;
		private string m_strRightOverlay;
		private int m_nTruth;
		string m_strbase = @"pack://application:,,/";

		int score;

		public MyGameHandUp()
		{
			m_timerCountdown.Tick += new EventHandler(TimerCountdown);
		}

		public void SetupUI(Canvas canvas, Image blurMask, Image overlayLeft, Image overlayRight, Image userBody, Image tfFace)
		{
			System.Diagnostics.Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

			// binding
			m_canvas = canvas;
			m_imgBlurMask = blurMask;
			m_imgOverlayLeft = overlayLeft;
			m_imgOverlayRight = overlayRight;
			m_imgUserBody = userBody;
			m_imgTFFace = tfFace;
		}

		public void SetupResource(string background, string backgroundMask, string questionSound, string overlayLeft, string overlayRight, int truth)
		{
			m_strBackground = background;
			m_strBackgroundMask = backgroundMask;
			m_strQuestionSound = questionSound;
			m_strLeftOverlay = overlayLeft;
			m_strRightOverlay = overlayRight;
			m_nTruth = truth;
		}

		public void Entrypoint()
		{
			System.Diagnostics.Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

			m_timeRemain = 60;

			// 0. 초기화
			m_imgBlurMask.Visibility = Visibility.Hidden;
			m_imgOverlayLeft.Visibility = Visibility.Hidden;
			m_imgOverlayRight.Visibility = Visibility.Hidden;
			m_imgUserBody.Visibility = Visibility.Hidden;
			m_imgTFFace.Visibility = Visibility.Hidden;

			if (m_myKinect.sensorChooser != null)
			{
				m_myKinect.evtReadySingleSkel += new EventHandler<AllFramesReadyEventArgs>(EventCheckHandOver);
			}

			// 1. 배경 보여주기
			m_canvas.Background = new ImageBrush(new BitmapImage(new Uri(m_strbase + "Images/" + m_strBackground)));

			// 2. 사운드 재생
			m_startSound.Open(new Uri("Sounds/" + m_strQuestionSound, UriKind.Relative)); // 속성:빌드시자동복사
			m_startSound.MediaEnded += new EventHandler(MediaEnd1);
			m_startSound.Volume = 1;
			m_startSound.Play();
		}

		// 3. 사운드 끝날때까지 딜레이
		private void MediaEnd1(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

			m_startSound.MediaEnded -= new EventHandler(MediaEnd1);
			m_startSound.Stop();
			m_startSound.Close();

			// 4. 반투명 마스크 보이기
			m_imgBlurMask.Source = new BitmapImage(new Uri(m_strbase + "Images/" + m_strBackgroundMask));
			m_imgBlurMask.Visibility = Visibility.Visible;

			// 5. 손들기 그림 좌,우에 보이기
			BitmapImage src;
			src = new BitmapImage(new Uri(m_strbase + "Images/" + m_strLeftOverlay));
			m_imgOverlayLeft.Source = src;
			m_imgOverlayLeft.Visibility = Visibility.Visible;

			src = new BitmapImage(new Uri(m_strbase + "Images/" + m_strRightOverlay));
			m_imgOverlayRight.Source = src;
			m_imgOverlayRight.Visibility = Visibility.Visible;

			// 6. 자신의 모습 보이기
			m_imgUserBody.Visibility = Visibility.Visible;

			// 7. 제한시간 시작
			m_timerCountdown.Interval = TimeSpan.FromMilliseconds(1000);
			m_timerCountdown.Start();

			score = 0;
			m_flgHandLeftUp = false;
			m_flgHandRightUp = false;
			m_flgHandLeftDown = true;
			m_flgHandRightDown = true;
			m_cntOneHand = 0;
			m_cntTwoHand = 0;
			m_flgLeftRight = 0;
		}
		

		private void TimerCountdown(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

			m_timeRemain -= 1;
			if (m_timeRemain <= 0)
			{
				m_timerCountdown.Stop();
				ResultGame(false);
			}
			else if (m_cntOneHand > 50 || m_cntTwoHand > 50)
			{
				m_timerCountdown.Stop();
				if (m_nTruth == m_flgLeftRight)
					ResultGame(true);
				else
					ResultGame(false);
			}
		}

		private void ResultGame(bool success)
		{
			System.Diagnostics.Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

			int success2 = 0;
			if (success)
				success2 = 0;
			else
				success2 = 1;

			string[] strImgName = { "Images/중간평가1.png", "Images/중간평가2.png" };
			string[] strSoundName = { "Sounds/중간평가_1(성공).m4a", "Sounds/중간평가_2(다음기회에).m4a" };

			BitmapImage src;
			src = new BitmapImage(new Uri(m_strbase + strImgName[success2]));
			m_imgTFFace.Source = src;
			m_imgTFFace.Visibility = Visibility.Visible;

			m_startSound.Open(new Uri(strSoundName[success2], UriKind.Relative)); // 속성:빌드시자동복사
			m_startSound.MediaEnded += new EventHandler(MediaEnd2);
			m_startSound.Volume = 1;
			m_startSound.Play();

			score = 1 - success2;
		}

		// 3. 사운드 끝날때까지 딜레이
		private void MediaEnd2(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

			m_startSound.MediaEnded -= new EventHandler(MediaEnd2);
			m_startSound.Stop();
			m_startSound.Close();

			if (m_myKinect.sensorChooser != null)
			{
				m_myKinect.evtReadySingleSkel -= new EventHandler<AllFramesReadyEventArgs>(EventCheckHandOver);
			}

			m_evtGameManager(score * 10, null);

			m_imgBlurMask.Visibility = Visibility.Hidden;
			m_imgOverlayLeft.Visibility = Visibility.Hidden;
			m_imgOverlayRight.Visibility = Visibility.Hidden;
			m_imgUserBody.Visibility = Visibility.Hidden;
			m_imgTFFace.Visibility = Visibility.Hidden;
		}

		private void EventCheckHandOver(object sender, AllFramesReadyEventArgs e)
		{
			Skeleton player = (Skeleton)sender;

			float headY = player.Joints[JointType.Head].Position.Y;
			float handLeftY = player.Joints[JointType.HandLeft].Position.Y;
			float handRightY = player.Joints[JointType.HandRight].Position.Y;

			double hipXCanvas = m_myKinect.SkeletonPointToScreen(player.Joints[JointType.HipCenter].Position).X; // 몸 좌우로 가는거 인식할때 사용할 것

			if (player.Joints[JointType.HipCenter].Position.X < 0)
				m_flgLeftRight = 0; // left
			else
				m_flgLeftRight = 1; // right

			if ((handLeftY - headY) > 0.1)
			{
				m_flgHandLeftUp = true;
				m_flgHandLeftDown = false;
			}
			else
			{
				if (m_flgHandLeftUp == true && (handLeftY - headY) < 0)
				{
					m_flgHandLeftUp = false;
					m_flgHandLeftDown = true;
				}
			}

			if ((handRightY - headY) > 0.1)
			{
				m_flgHandRightUp = true;
				m_flgHandRightDown = false;

			}
			else
			{
				if (m_flgHandRightUp == true && (handRightY - headY) < 0)
				{
					m_flgHandRightUp = false;
					m_flgHandRightDown = true;
				}
			}

			if (m_flgHandLeftDown == true && m_flgHandRightDown == true)
			{
				m_cntOneHand = 0;
				m_cntTwoHand = 0;
			}

			if (m_flgHandLeftUp && m_flgHandRightUp)
			{
				m_cntTwoHand += 1;
			}
			else if (m_flgHandLeftUp || m_flgHandRightUp)
			{
				m_cntOneHand += 1;
			}
		}


	}
}
