using Microsoft.Kinect.Toolkit.Controls;
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

namespace MainProgram
{
	/// <summary>
	/// PageItem2.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class PageItem2 : Page
	{
		public event EventHandler m_evtPageEnd;
		public int m_idxGame;
		
		private MyGameHandUp m_game1 = new MyGameHandUp();
		private MyGameHandUp m_game2 = new MyGameHandUp();
		private MyGameDragAndDrop m_game3 = new MyGameDragAndDrop(1);
		private MyGameDragAndDrop m_game4 = new MyGameDragAndDrop(2);
		private MyGameDragAndDrop m_game5 = new MyGameDragAndDrop(3);
		private MyGameDragAndDrop m_game6 = new MyGameDragAndDrop(4);
		private MyGameHandUp m_game7 = new MyGameHandUp();
		private MyGame3SecondStop m_game8 = new MyGame3SecondStop();
		private MyGameHandUp m_game9 = new MyGameHandUp();

		private MediaPlayer m_soundBackground = new MediaPlayer();

		private MyKinectSensor m_myKinect;

		private int score;
		private DateTime startTime;

		public PageItem2(MyKinectSensor kinectSensor)
		{
			System.Diagnostics.Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

			InitializeComponent();

			this.Loaded += new RoutedEventHandler(PageLoaded);

			m_myKinect = kinectSensor;

			m_game1.SetupUI(this.canvasBG, this.imgMask, this.imgOverlayLeft, this.imgOverlayRight, this.imgUser, this.imgFace);
			m_game1.SetupResource("신충_01_01(손들기).png", "신충_01_04.png", "신충_01_01(손들기).m4a", "신충_01_02.png", "신충_01_03.png", 1);
			m_game1.m_myKinect = kinectSensor;
			m_game1.m_evtGameManager += new EventHandler(EventGameManager);

			m_game2.SetupUI(this.canvasBG, this.imgMask, this.imgOverlayLeft, this.imgOverlayRight, this.imgUser, this.imgFace);
			m_game2.SetupResource("신충_02_01(손들기).png", "신충_02_04.png", "신충_02_01(손들기).m4a", "신충_02_02.png", "신충_02_03.png", 0);
			m_game2.m_myKinect = kinectSensor;
			m_game2.m_evtGameManager += new EventHandler(EventGameManager);

			m_game3.SetupUI(this.canvasBG, this.imgFace);
			m_game3.SetupResource("신충_03_01(드래그).png", "신충_03_01(드래그).m4a");
			m_game3.m_evtGameManager += new EventHandler(EventGameManager);

			m_game4.SetupUI(this.canvasBG, this.imgFace);
			m_game4.SetupResource("신충_04_01(드래그).png", "신충_04_01(드래그).m4a");
			m_game4.m_evtGameManager += new EventHandler(EventGameManager);

			m_game5.SetupUI(this.canvasBG, this.imgFace);
			m_game5.SetupResource("신충_05_01(드래그).png", "신충_05_01(드래그).m4a");
			m_game5.m_evtGameManager += new EventHandler(EventGameManager);

			m_game6.SetupUI(this.canvasBG, this.imgFace);
			m_game6.SetupResource("신충_06_01(드래그).png", "신충_06_01(드래그).m4a");
			m_game6.m_evtGameManager += new EventHandler(EventGameManager);

			m_game7.SetupUI(this.canvasBG, this.imgMask, this.imgOverlayLeft, this.imgOverlayRight, this.imgUser, this.imgFace);
			m_game7.SetupResource("신충_07_01(손들기).png", "신충_07_04.png", "신충_07_01(손들기).m4a", "신충_07_02.png", "신충_07_03.png", 1);
			m_game7.m_myKinect = kinectSensor;
			m_game7.m_evtGameManager += new EventHandler(EventGameManager);

			m_game9.SetupUI(this.canvasBG, this.imgMask, this.imgOverlayLeft, this.imgOverlayRight, this.imgUser, this.imgFace);
			m_game9.SetupResource("신충_09_01(손들기).png", "신충_09_04.png", "신충_09_01(손들기).m4a", "신충_09_02.png", "신충_09_03.png", 0);
			m_game9.m_myKinect = kinectSensor;
			m_game9.m_evtGameManager += new EventHandler(EventGameManager);

			m_soundBackground.Open(new Uri("Sounds/" + "배경음악2_신충.mp3", UriKind.Relative)); // 속성:빌드시자동복사
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

			if (sender != null)
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
				case 5:
					m_game6.Entrypoint();
					break;
				case 6:
					m_game7.Entrypoint();
					break;
				case 7:
					//m_game8.Entrypoint();
					//break;
				//case 8:
					m_game9.Entrypoint();
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
