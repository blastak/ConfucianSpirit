using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using System.Windows.Threading;

namespace MainProgram
{
	/// <summary>
	/// PageStart.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class PageStart : Page
	{
		public event EventHandler BtnClicked1;
		public event EventHandler BtnClicked2;
		public event EventHandler BtnClicked3;

		private MediaPlayer m_soundBackground = new MediaPlayer();
		private MediaPlayer m_soundNarration = new MediaPlayer();

		private MediaPlayer m_soundNarrationTpose = new MediaPlayer();
		string m_strbase = @"pack://application:,,/";
		private DispatcherTimer m_timerCountdown = new DispatcherTimer();
		private int m_timeRemain;

        public Image imgLoading;

        public PageStart()
		{
			InitializeComponent();

			this.Loaded += new RoutedEventHandler(PageLoaded);

			canvasBG.Visibility = Visibility.Hidden;

            m_soundNarrationTpose.Open(new Uri("Sounds/" + "T포즈.m4a", UriKind.Relative)); // 속성:빌드시자동복사
			m_soundNarrationTpose.Volume = 1;
			m_soundNarrationTpose.MediaEnded += new EventHandler(TPoseMusic);
			m_timerCountdown.Tick += new EventHandler(TimerCountdown);

			imgUser.Visibility = Visibility.Hidden;
		}

		private void PageLoaded(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

            buttonStart1.Visibility = Visibility.Hidden;
			buttonStart2.Visibility = Visibility.Hidden;
			buttonStart3.Visibility = Visibility.Hidden;

            imgLoading.Visibility = Visibility.Hidden;

            canvasBG.Background = new ImageBrush(new BitmapImage(new Uri(m_strbase + "Images/" + "T포즈.png")));
			canvasBG.Visibility = Visibility.Visible;

            m_soundNarrationTpose.Position = TimeSpan.Zero;
			m_soundNarrationTpose.Play();
		}

		private void TPoseMusic(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

			m_soundNarrationTpose.Stop();
			
			imgUser.Visibility = Visibility.Visible;

			m_timeRemain = 4;
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
				imgUser.Visibility = Visibility.Hidden;
				PageLoaded2();
			}
		}

		private void PageLoaded2()
        {
            System.Diagnostics.Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

            m_soundBackground.Open(new Uri("Sounds/" + "게임시작배경음악.mp3", UriKind.Relative)); // 속성:빌드시자동복사
            m_soundBackground.Volume = 0.3;
            m_soundBackground.Play();

            m_soundNarration.Open(new Uri("Sounds/" + "게임시작.m4a", UriKind.Relative)); // 속성:빌드시자동복사
            m_soundNarration.Volume = 1;
            m_soundNarration.Play();

            canvasBG.Background = new ImageBrush(new BitmapImage(new Uri(m_strbase + "Images/" + "게임첫화면1.png")));
            buttonStart1.Visibility = Visibility.Visible;
			buttonStart2.Visibility = Visibility.Visible;
			buttonStart3.Visibility = Visibility.Visible;
		}

		private void buttonStart1_Click(object sender, RoutedEventArgs e)
		{
			m_soundBackground.Stop();
			m_soundNarration.Stop();
            m_soundBackground.Close();
            m_soundNarration.Close();

            if (BtnClicked1 != null)
				this.BtnClicked1(this, e);
		}

		private void buttonStart2_Click(object sender, RoutedEventArgs e)
		{
			m_soundBackground.Stop();
			m_soundNarration.Stop();
            m_soundBackground.Close();
            m_soundNarration.Close();

            if (BtnClicked2 != null)
				this.BtnClicked2(this, e);
		}

		private void buttonStart3_Click(object sender, RoutedEventArgs e)
		{
			m_soundBackground.Stop();
			m_soundNarration.Stop();
            m_soundBackground.Close();
            m_soundNarration.Close();

            if (BtnClicked3 != null)
				this.BtnClicked3(this, e);
		}
	}
}
