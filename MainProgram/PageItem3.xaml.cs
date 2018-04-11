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
	/// PageItem3.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class PageItem3 : Page
	{
		public event EventHandler m_evtPageEnd;
		public int m_idxGame;

		public MyGame3SecondStop m_game1 = new MyGame3SecondStop();
		public MyGameHandUp m_game2 = new MyGameHandUp();
		public MyGameGravityTouch m_game3 = new MyGameGravityTouch();
		public MyGame3SecondStop m_game4 = new MyGame3SecondStop();

		private MediaPlayer m_soundBackground = new MediaPlayer();

		private MyKinectSensor m_myKinect;

		private int score;
		private DateTime startTime;

		public PageItem3(MyKinectSensor kinectSensor)
		{
			//System.Diagnostics.Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

			InitializeComponent();

			this.Loaded += new RoutedEventHandler(PageLoaded);

			m_myKinect = kinectSensor;

			Point[] tempPoint1 = { new Point(393 / 1612.0, 487 / 1185.0), new Point(1241 / 1612.0, 476 / 1185.0), new Point(335 / 1612.0, 970 / 1185.0), new Point(1185 / 1612.0, 948 / 1185.0) };
			m_game1.SetupUI(canvasBG, imgFace);
			m_game1.SetupResource("경성_01_01(3초간정지).png", "경성_01_01(3초간정지).m4a", tempPoint1, 3, 200 / 1185.0);
			m_game1.m_evtGameManager += new EventHandler(EventGameManager);

			m_game2.SetupUI(this.canvasBG, this.imgMask, this.imgOverlayLeft, this.imgOverlayRight, this.imgUser, this.imgFace);
			m_game2.SetupResource("경성_02_01(손들기).png", "경성_02_04.png", "경성_02_01(손들기).m4a", "경성_02_02.png", "경성_02_03.png", 1);
			m_game2.m_myKinect = kinectSensor;
			m_game2.m_evtGameManager += new EventHandler(EventGameManager);

			m_game3.SetupUI(this.canvasBG, this.canvasBG2, this.imgUser2, this.imgFace, this.imgIcon);
			m_game3.SetupResource("경성_03_01(터트리기).png", "경성_03_01(터트리기).m4a");
			m_game3.m_myKinect = kinectSensor;
			m_game3.m_evtGameManager += new EventHandler(EventGameManager);

			m_game4.SetupUI(canvasBG, imgFace);
			m_game4.SetupResource("경성_04_01(3초간정지).png", "경성_04_01(3초간정지).m4a", tempPoint1, 1, 200 / 1185.0);
			m_game4.m_evtGameManager += new EventHandler(EventGameManager);

			m_soundBackground.Open(new Uri("Sounds/" + "배경음악3_경성.mp3", UriKind.Relative)); // 속성:빌드시자동복사
			m_soundBackground.MediaEnded += new EventHandler(BackgroundMusicEnd);
			m_soundBackground.Volume = 0.5;
		}

		private void PageLoaded(object sender, EventArgs e)
		{
			//System.Diagnostics.Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

			m_myKinect.BindBackgroundRemovalImage(imgUser);
			//m_myKinect.BindBackgroundRemovalImage(imgUser2);

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
			//System.Diagnostics.Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

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
