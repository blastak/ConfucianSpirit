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

using Microsoft.Kinect;

using System.Windows.Threading;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MainProgram
{
	/// <summary>
	/// PageItem1.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class PageItem1 : Page
	{
		public event EventHandler m_evtPageEnd;
		public int m_idxGame;

		public MyGameHandUp m_game1 = new MyGameHandUp();
		public MyGameHandUp m_game2 = new MyGameHandUp();
		public MyGameGestureBow m_game3 = new MyGameGestureBow();
		public MyGameHandUp m_game4 = new MyGameHandUp();
		public MyGameGravityCollect m_game5 = new MyGameGravityCollect();

		private MediaPlayer m_soundBackground = new MediaPlayer();

		private MyKinectSensor m_myKinect;

		private int score;
		private DateTime startTime;

		public PageItem1(MyKinectSensor kinectSensor)
		{
			System.Diagnostics.Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

			InitializeComponent();

			this.Loaded += new RoutedEventHandler(PageLoaded);

			m_myKinect = kinectSensor;

			m_game1.SetupUI(this.canvasBG, this.imgMask, this.imgOverlayLeft, this.imgOverlayRight, this.imgUser, this.imgFace);
			m_game1.SetupResource("예효_01_01(손들기).png", "예효_01_04.png", "예효_01_01(손들기).m4a", "예효_01_02.png", "예효_01_03.png", 0);
			m_game1.m_myKinect = kinectSensor;
			m_game1.m_evtGameManager += new EventHandler(EventGameManager);

			m_game2.SetupUI(this.canvasBG, this.imgMask, this.imgOverlayLeft, this.imgOverlayRight, this.imgUser, this.imgFace);
			m_game2.SetupResource("예효_02_01(손들기).png", "예효_02_04.png", "예효_02_01(손들기).m4a", "예효_02_02.png", "예효_02_03.png", 0);
			m_game2.m_myKinect = kinectSensor;
			m_game2.m_evtGameManager += new EventHandler(EventGameManager);

			m_game3.SetupUI(this.canvasBG, this.imgMask, this.imgUser, this.imgFace);
			m_game3.SetupResource("예효_03_01(인사).png", "예효_03_02.png", "예효_03_01(인사).m4a", "예효_03_02.m4a");
			m_game3.m_myKinect = kinectSensor;
			m_game3.m_evtGameManager += new EventHandler(EventGameManager);

			m_game4.SetupUI(this.canvasBG, this.imgMask, this.imgOverlayLeft, this.imgOverlayRight, this.imgUser, this.imgFace);
			m_game4.SetupResource("예효_04_01(손들기).png", "예효_04_04.png", "예효_04_01(손들기).m4a", "예효_04_02.png", "예효_04_03.png", 1);
			m_game4.m_myKinect = kinectSensor;
			m_game4.m_evtGameManager += new EventHandler(EventGameManager);

			m_game5.SetupUI(this.canvasBG, this.canvasBG2, this.imgUser2, this.imgFace);
			m_game5.SetupResource("예효_05_01(바구니).png", "예효_05_01(바구니).m4a");
			m_game5.m_myKinect = kinectSensor;
			m_game5.m_evtGameManager += new EventHandler(EventGameManager);

			m_soundBackground.Open(new Uri("Sounds/" + "배경음악1_예효.mp3", UriKind.Relative)); // 속성:빌드시자동복사
			m_soundBackground.MediaEnded += new EventHandler(BackgroundMusicEnd);
			m_soundBackground.Volume = 0.1;
		}

		private void PageLoaded(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

			m_myKinect.BindBackgroundRemovalImage(imgUser);

			m_soundBackground.Play();

			score = 0;
			startTime = DateTime.Now;
			
			m_idxGame = 0;
			EventGameManager(null, null);
		}

		private void BackgroundMusicEnd(object sender, EventArgs e)
		{
			m_soundBackground.Position = TimeSpan.Zero;
			m_soundBackground.Play();
		}

		private void EventGameManager(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

			if(sender != null)
			{
				score += (int)sender;
			}

			switch (m_idxGame)
			{
				case 0:
					m_game1.Entrypoint();
					break;
				case 1:
					m_game2.Entrypoint();
					break;
				case 2:
					m_game3.Entrypoint();
					break;
				case 3:
					m_game4.Entrypoint();
					break;
				case 4:
					m_game5.Entrypoint();
					break;
				default:
					m_myKinect.UnbindBackgroundRemovalImage();
					m_soundBackground.Stop();

					double milliseconds = DateTime.Now.Subtract(startTime).TotalMilliseconds;
					TimeSpan timeSpan = TimeSpan.FromMilliseconds(milliseconds);

					List<object> data = new List<object>();
					data.Add(score);
					data.Add(timeSpan);

					m_evtPageEnd(data, null);
					break;
			}

			m_idxGame += 1;
		}
	}
}