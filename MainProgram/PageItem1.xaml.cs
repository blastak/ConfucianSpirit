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
using HrkimKinectSensor;

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
		public event EventHandler m_evGameManager;
		public int m_idxGame;

		private readonly MediaPlayer m_startSound = new MediaPlayer();

		public MyKinectSensor m_myKinect;
		private DrawingGroup m_drawingGroup;
		private DrawingImage m_imageSource;

		private Skeleton[] m_frameSkeletons;

		private bool m_flgHandLeftOver;
		private bool m_flgHandLeftRelease;
		private bool m_flgHandRightOver;
		private bool m_flgHandRightRelease;
		private int m_cntOneHand;
		private int m_cntTwoHand;

		private DispatcherTimer m_timerCountdown = new DispatcherTimer();
		private int m_timeRemain;

        private string[] m_strBackground = { "01_예효_01(손들기).png", "02_예효_01(손들기).png", "03_예효_01(손들기).png" };
        private string[] m_strQuestionSound = { "01_예효_01(손들기).mp3", "02_예효_01(손들기).mp3", "03_예효_01(손들기).mp3" };
        private string[] m_strLeftOverlay = { "01_예효_02.png", "02_예효_02.png", "03_예효_02.png" };
        private string[] m_strRightOverlay = { "01_예효_03.png", "02_예효_03.png", "03_예효_03.png" };
        private int[] m_nTruth = { 0, 0, 1 };

        string m_strbase = @"pack://application:,,/";

		public PageItem1(MyKinectSensor kinectSensor)
		{
			System.Diagnostics.Debug.WriteLine("PageItem1");
			InitializeComponent();

			this.m_myKinect = kinectSensor;

			this.m_drawingGroup = new DrawingGroup();
			this.m_imageSource = new DrawingImage(this.m_drawingGroup);
			imgSkeleton.Source = this.m_imageSource;

			m_timerCountdown.Tick += new EventHandler(TimerCountdown);

			m_evGameManager += new EventHandler(EventGameManager);

			this.Loaded += new RoutedEventHandler(PageLoaded);

		}

		private void PageLoaded(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("PageLoaded");
			m_idxGame = 0;
			m_evGameManager(this, null);
		}

		private void EventGameManager(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("EventGameManager");
			switch (m_idxGame)
			{
				case 0:
					Entrypoint();
					break;
                case 1:
                    Entrypoint();
                    break;
                case 2:
                    Entrypoint();
                    break;
                default:
                    MessageBox.Show("end\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n of\n\n\n\n\n\n\n\n\n\n\n\n\n story");
					////////////////////////////////////// 페이지 끝남 넣기
					break;
			}
		}

		private void Entrypoint()
		{
			System.Diagnostics.Debug.WriteLine("Entrypoint");
			// 0. 초기화
			imgMask.Visibility = Visibility.Hidden;
			imgOverlayLeft.Visibility = Visibility.Hidden;
			imgOverlayRight.Visibility = Visibility.Hidden;
			imgSkeleton.Visibility = Visibility.Hidden;
			imgFace.Visibility = Visibility.Hidden;
			m_timeRemain = 10;
			m_flgHandLeftOver = false;
			m_flgHandRightOver = false;
			m_flgHandLeftRelease = true;
			m_flgHandRightRelease = true;
			m_cntOneHand = 0;
			m_cntTwoHand = 0;

            if (m_myKinect.sensorChooser != null)
            {
                m_myKinect.ReadSkeleton += new EventHandler<AllFramesReadyEventArgs>(MySkeletonFrameReady);
            }

            // 1. 배경 보여주기
            canvasBG.Background = new ImageBrush(new BitmapImage(new Uri(m_strbase + "Images/" + m_strBackground[m_idxGame])));

			// 2. 사운드 재생
			m_startSound.Open(new Uri("Sounds/" + m_strQuestionSound[m_idxGame], UriKind.Relative)); // 속성:빌드시자동복사
			m_startSound.MediaEnded += new EventHandler(MediaEnd1);
			m_startSound.Volume = 1;
			m_startSound.Play();
        }

		// 3. 사운드 끝날때까지 딜레이
		private void MediaEnd1(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("MediaEnd1");
			m_startSound.MediaEnded -= new EventHandler(MediaEnd1);
			m_startSound.Stop();
			m_startSound.Close();

			// 4. 반투명 마스크 보이기
			imgMask.Visibility = Visibility.Visible;

			// 5. 손들기 그림 좌,우에 보이기
			BitmapImage src;
			src = new BitmapImage(new Uri(m_strbase + "Images/" + m_strLeftOverlay[m_idxGame]));
			imgOverlayLeft.Source = src;
			imgOverlayLeft.Visibility = Visibility.Visible;

			src = new BitmapImage(new Uri(m_strbase + "Images/" + m_strRightOverlay[m_idxGame]));
			imgOverlayRight.Source = src;
			imgOverlayRight.Visibility = Visibility.Visible;

			// 6. 자신의 모습 보이기
			imgSkeleton.Visibility = Visibility.Visible;

			// 7. 제한시간 시작
			m_timerCountdown.Interval = TimeSpan.FromMilliseconds(1000);
			m_timerCountdown.Start();
		}


		

		private void TimerCountdown(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("TimerCountdown");
			m_timeRemain -= 1;
			if(m_timeRemain <= 0)
			{
				m_timerCountdown.Stop();
                ResultGame(false);
			}
            else if (m_cntOneHand > 50)
            {
                m_timerCountdown.Stop();
                if (m_nTruth[m_idxGame] == 0)
                    ResultGame(true);
                else
                    ResultGame(false);
            }
            else if (m_cntTwoHand > 50)
			{
				m_timerCountdown.Stop();
                if (m_nTruth[m_idxGame] == 1)
                    ResultGame(true);
                else
                    ResultGame(false);
            }
		}
        
        private void ResultGame(bool success)
        {
			System.Diagnostics.Debug.WriteLine("ResultGame");
			if (success)
            {
                BitmapImage src;
                src = new BitmapImage(new Uri(m_strbase + "Images/중간평가1.png"));
                imgFace.Source = src;
                imgFace.Visibility = Visibility.Visible;

                m_startSound.Open(new Uri("Sounds/중간평가_1(성공).mp3", UriKind.Relative)); // 속성:빌드시자동복사
                m_startSound.MediaEnded += new EventHandler(MediaEnd2);
                m_startSound.Volume = 1;
				m_startSound.Play();
            }
            else
            {
                BitmapImage src;
                src = new BitmapImage(new Uri(m_strbase + "Images/중간평가2.png"));
                imgFace.Source = src;
                imgFace.Visibility = Visibility.Visible;

                m_startSound.Open(new Uri("Sounds/중간평가_2(다음기회에).mp3", UriKind.Relative)); // 속성:빌드시자동복사
                m_startSound.MediaEnded += new EventHandler(MediaEnd2);
                m_startSound.Volume = 1;
				m_startSound.Play();
            }
        }

		// 3. 사운드 끝날때까지 딜레이
		private void MediaEnd2(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("MediaEnd2");
			m_startSound.MediaEnded -= new EventHandler(MediaEnd2);
            m_startSound.Stop();
            m_startSound.Close();

			if (m_myKinect.sensorChooser != null)
			{
				m_myKinect.ReadSkeleton -= new EventHandler<AllFramesReadyEventArgs>(MySkeletonFrameReady);
			}

			m_idxGame += 1;
			m_evGameManager(this, null);
		}


		private void MySkeletonFrameReady(object sender, AllFramesReadyEventArgs e)
		{
			using (DrawingContext dc = this.m_drawingGroup.Open())
			{
				// Draw a transparent background to set the render size
				//dc.DrawRectangle(null, new Pen(Brushes.Blue,10), new Rect(0.0, 0.0, 640, 480));
				//dc.DrawRectangle(null, null, new Rect(0.0, 0.0, 640, 480));
				dc.DrawRectangle(null, new Pen(Brushes.Blue, 1), new Rect(0.0, 0.0, 640, 480));


				m_frameSkeletons = m_myKinect.skeletons;

				if (m_frameSkeletons.Length != 0)
				{
					Single distMin = Single.MaxValue;
					Skeleton player = new Skeleton();

					foreach (Skeleton skel in m_frameSkeletons)
					{
						// 화면밖으로 벗어났을 때 빨간선
						//RenderClippedEdges(skel, dc);

						if (null == skel)
						{
							continue;
						}

						if (skel.TrackingState != SkeletonTrackingState.Tracked)
						{
							continue;
						}

						Single dist = skel.Position.Z;
						if (dist < distMin && dist >= 0.5)
						{
							player = skel;
							distMin = dist;
						}
					}

					if (player.TrackingState == SkeletonTrackingState.Tracked)
					{
						this.DrawBonesAndJoints(player, dc);
					}
					else if (player.TrackingState == SkeletonTrackingState.PositionOnly)
					{
						dc.DrawEllipse(
						Brushes.Blue,
						null,
						this.SkeletonPointToScreen(player.Position),
						10, 10);
					}

					CheckHandOver(player);
				}

				// prevent drawing outside of our render area
				this.m_drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, 640, 480));
			}
		}

		private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
		{
			// Convert point to depth space.  
			// We are not using depth directly, but we do want the points in our 640x480 output resolution.
			DepthImagePoint depthPoint = m_myKinect.sensorChooser.Kinect.CoordinateMapper.MapSkeletonPointToDepthPoint(
				skelpoint, m_myKinect.sensorChooser.Kinect.DepthStream.Format);
			return new Point(depthPoint.X, depthPoint.Y);



			//return new Point(skelpoint.X*1280.0, skelpoint.Y*720.0);

			//ColorImagePoint a = m_myKinect.sensorChooser.Kinect.CoordinateMapper.MapSkeletonPointToColorPoint(skelpoint, ColorImageFormat.RgbResolution1280x960Fps12);
			//return new Point(a.X, a.Y);
		}

		private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext)
		{
			// Render Torso
			this.DrawBone(skeleton, drawingContext, JointType.Head, JointType.ShoulderCenter);
			this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft);
			this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight);
			this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine);
			this.DrawBone(skeleton, drawingContext, JointType.Spine, JointType.HipCenter);
			this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft);
			this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipRight);

			// Left Arm
			this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft);
			this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft);
			this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft);

			// Right Arm
			this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight);
			this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight);
			this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight);

			// Left Leg
			this.DrawBone(skeleton, drawingContext, JointType.HipLeft, JointType.KneeLeft);
			this.DrawBone(skeleton, drawingContext, JointType.KneeLeft, JointType.AnkleLeft);
			this.DrawBone(skeleton, drawingContext, JointType.AnkleLeft, JointType.FootLeft);

			// Right Leg
			this.DrawBone(skeleton, drawingContext, JointType.HipRight, JointType.KneeRight);
			this.DrawBone(skeleton, drawingContext, JointType.KneeRight, JointType.AnkleRight);
			this.DrawBone(skeleton, drawingContext, JointType.AnkleRight, JointType.FootRight);

			// Render Joints
			foreach (Joint joint in skeleton.Joints)
			{
				Brush drawBrush = null;

				if (joint.TrackingState == JointTrackingState.Tracked)
				{
					drawBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));
				}
				else if (joint.TrackingState == JointTrackingState.Inferred)
				{
					drawBrush = Brushes.Yellow;
				}

				if (drawBrush != null)
				{
					if (joint.JointType == JointType.Head)
						drawingContext.DrawEllipse(Brushes.WhiteSmoke, new Pen(drawBrush, 3), this.SkeletonPointToScreen(joint.Position), 35, 45);
					else
						drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(joint.Position), 3, 3);
				}
			}
		}

		private void DrawBone(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1)
		{
			Joint joint0 = skeleton.Joints[jointType0];
			Joint joint1 = skeleton.Joints[jointType1];

			// If we can't find either of these joints, exit
			if (joint0.TrackingState == JointTrackingState.NotTracked ||
				joint1.TrackingState == JointTrackingState.NotTracked)
			{
				return;
			}

			// Don't draw if both points are inferred
			if (joint0.TrackingState == JointTrackingState.Inferred &&
				joint1.TrackingState == JointTrackingState.Inferred)
			{
				return;
			}

			// We assume all drawn bones are inferred unless BOTH joints are tracked
			Pen drawPen;
			if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
			{
				drawPen = new Pen(Brushes.Green, 6); // bone color
			}
			else
			{
				drawPen = new Pen(Brushes.Gray, 1); // bone color (non-tracked)
			}

			drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
		}

		private void CheckHandOver(Skeleton skeleton)
		{
			Single headY = skeleton.Joints[JointType.Head].Position.Y;
			Single handLeftY = skeleton.Joints[JointType.HandLeft].Position.Y;
			Single handRightY = skeleton.Joints[JointType.HandRight].Position.Y;
			double hipXCanvas = SkeletonPointToScreen(skeleton.Joints[JointType.HipCenter].Position).X;

			if ((handLeftY - headY) > 0.1)
			{
				m_flgHandLeftOver = true;
                m_flgHandLeftRelease = false;
            }
            else
			{
				if (m_flgHandLeftOver == true && (handLeftY - headY) < 0)
				{
					m_flgHandLeftOver = false;
					m_flgHandLeftRelease = true;
				}
			}

			if ((handRightY - headY) > 0.1)
			{
				m_flgHandRightOver = true;
                m_flgHandRightRelease = false;

            }
            else
			{
				if (m_flgHandRightOver == true && (handRightY - headY) < 0)
				{
					m_flgHandRightOver = false;
					m_flgHandRightRelease = true;
				}
			}

			if (m_flgHandLeftRelease == true && m_flgHandRightRelease == true)
			{
				m_cntOneHand = 0;
				m_cntTwoHand = 0;
			}

			if (m_flgHandLeftOver && m_flgHandRightOver)
			{
				m_cntTwoHand += 1;
			}
			else if (m_flgHandLeftOver || m_flgHandRightOver)
			{
				m_cntOneHand += 1;
			}
		}
	}




	/*



	//ImageSourceConverter imgsc = new ImageSourceConverter();
	//imgOverlayLeft.SetValue(Image.SourceProperty, imgsc.ConvertFromString(""));



	// 6. 스켈레톤 보이기

	// 7. 인식할 때까지 대기
	Delay(1000);

	// 8. 인식이 되면 사운드 재생
	startSound.Open(new Uri("Sounds/성공.mp3", UriKind.Relative)); // 속성:빌드시자동복사
	startSound.Volume = 1;
	startSound.Play();

	// 9. 다음 과정 넘어가기
	


	 		void TimerThread(object ms)
	 		{
	 			startSound.Dispatcher.Invoke(
	 		  System.Windows.Threading.DispatcherPriority.SystemIdle,
	 		  new Action(
	 			delegate ()
	 			{
	 				startSound.Open(new Uri("Sounds/01_예효_01(손들기).mp3", UriKind.Relative)); // 속성:빌드시자동복사
	 				startSound.Volume = 1;
	 				startSound.Play();
	 				startSound.Stop();
	 				startSound.Close();
	 			}
	 		));
	 		}


	 
	 		private static DateTime Delay(int MS)
	 		{
	 			DateTime ThisMoment = DateTime.Now;
	 			TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
	 			DateTime AfterWards = ThisMoment.Add(duration);
	 
	 			while (AfterWards >= ThisMoment)
	 			{
	 				//Application.DoEvents();
	 				ThisMoment = DateTime.Now;
	 			}
	 
	 			return DateTime.Now;
	 		}
	*/
}