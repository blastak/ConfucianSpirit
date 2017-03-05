using Microsoft.Kinect;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MainProgram
{
	class MyGameGravityCollect
	{
		public event EventHandler m_evtGameManager;

		private Canvas m_canvas;
		private Image m_imgUserBody;
		private Image m_imgTFFace;

		private int m_timeRemain;

		public MyKinectSensor m_myKinect;

		private MediaPlayer m_startSound = new MediaPlayer();
		private DispatcherTimer m_timerCountdown = new DispatcherTimer();

		private string m_strBackground;
		private string m_strQuestionSound;

		string m_strbase = @"pack://application:,,/";

		public MyGameGravityCollect()
		{
			m_timerCountdown.Tick += new EventHandler(TimerCountdown);

		}

		public void SetupUI(Canvas canvas, Image userBody, Image tfFace)
		{
			System.Diagnostics.Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

			// binding
			m_canvas = canvas;
			m_imgUserBody = userBody;
			m_imgTFFace = tfFace;
		}

		public void SetupResource(string background, string questionSound)
		{
			m_strBackground = background;
			m_strQuestionSound = questionSound;
		}

		public void Entrypoint()
		{
			System.Diagnostics.Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

			m_timeRemain = 2;

			// 0. 초기화
			m_imgUserBody.Visibility = Visibility.Hidden;
			m_imgTFFace.Visibility = Visibility.Hidden;

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

			// 6. 자신의 모습 보이기
			m_imgUserBody.Visibility = Visibility.Visible;

			// 7. 제한시간 시작
			m_timerCountdown.Interval = TimeSpan.FromMilliseconds(1000);
			m_timerCountdown.Start();
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
// 			else if (m_cntOneHand > 50)
// 			{
// 				m_timerCountdown.Stop();
// 				ResultGame(m_nTruth == 0);
// 			}
// 			else if (m_cntTwoHand > 50)
// 			{
// 				m_timerCountdown.Stop();
// 				ResultGame(m_nTruth == 1);
// 			}
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
			string[] strSoundName = { "Sounds/중간평가_1(성공).mp3", "Sounds/중간평가_2(다음기회에).mp3" };

			BitmapImage src;
			src = new BitmapImage(new Uri(m_strbase + strImgName[success2]));
			m_imgTFFace.Source = src;
			m_imgTFFace.Visibility = Visibility.Visible;

			m_startSound.Open(new Uri(strSoundName[success2], UriKind.Relative)); // 속성:빌드시자동복사
			m_startSound.MediaEnded += new EventHandler(MediaEnd2);
			m_startSound.Volume = 1;
			m_startSound.Play();
		}

		// 3. 사운드 끝날때까지 딜레이
		private void MediaEnd2(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

			m_startSound.MediaEnded -= new EventHandler(MediaEnd2);
			m_startSound.Stop();
			m_startSound.Close();

			m_evtGameManager(null, null);

			m_imgUserBody.Visibility = Visibility.Hidden;
			m_imgTFFace.Visibility = Visibility.Hidden;
		}
	}
}
