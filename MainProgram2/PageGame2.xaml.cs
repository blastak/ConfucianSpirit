using Microsoft.Kinect.Toolkit.Controls;
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
	/// Interaction logic for PageGame2.xaml
	/// </summary>
	public partial class PageGame2 : Page
	{
		string m_strbase = @"pack://application:,,/";
		public event EventHandler m_evtPageFinish;
		public event EventHandler m_evtBindHand;
		public event EventHandler m_evtUnBindHand;
		
		public DispatcherTimer m_timerPageFinish = new DispatcherTimer();
		public DragImage m_dragImg1 = null;
		public DragImage m_dragImg2 = null;
		public DragImage m_dragImg3 = null;
		public DragImage m_dragImg4 = null;

		private MediaPlayer m_soundIntroBackground = new MediaPlayer();

		private MediaPlayer m_soundVideo1Background = new MediaPlayer();
		private MediaPlayer m_soundVideo2Background = new MediaPlayer();
		private MediaPlayer m_soundVideo3Background = new MediaPlayer();
		private MediaPlayer m_soundVideo4Background = new MediaPlayer();

		private MediaPlayer m_soundBackground = new MediaPlayer();

		private enum GripState
		{
			Released,
			Gripped
		}
		private GripState m_lastGripState;
		private long m_lastTimeStamp;
		private long m_lastGripTimeStamp;
		private long m_lastGripReleaseTimeStamp;
		UIElement m_currentItem;
		private Point m_gripPoint;

		public int m_nScore;
		public int m_cntRemainSecond;
		public bool m_bSkip;


		public PageGame2()
		{
			InitializeComponent();

			m_soundIntroBackground.Open(new Uri("Media/" + "PageGame공통_인트로_배경음악.mp3", UriKind.Relative));
			m_soundIntroBackground.Volume = 1;

			m_soundBackground.Open(new Uri("Media/" + "PageGame2_배경음악.mp3", UriKind.Relative));
			m_soundBackground.Volume = 1;

			m_soundVideo1Background.Open(new Uri("Media/" + "PageGame2_보기1_배경음악.mp3", UriKind.Relative));
			m_soundVideo1Background.Volume = 1;
			m_soundVideo2Background.Open(new Uri("Media/" + "PageGame2_보기2_배경음악.mp3", UriKind.Relative));
			m_soundVideo2Background.Volume = 1;
			m_soundVideo3Background.Open(new Uri("Media/" + "PageGame2_보기3_배경음악.mp3", UriKind.Relative));
			m_soundVideo3Background.Volume = 1;
			m_soundVideo4Background.Open(new Uri("Media/" + "PageGame2_보기4_배경음악.mp3", UriKind.Relative));
			m_soundVideo4Background.Volume = 1;

			m_timerPageFinish.Interval = TimeSpan.FromSeconds(1);
			m_timerPageFinish.Tick += new EventHandler(TimerPageFinish);
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			m_labelRemainSecond.Visibility = Visibility.Hidden;
			m_canvas.Visibility = Visibility.Hidden;

			m_video1.Visibility = Visibility.Hidden;
			m_video2.Visibility = Visibility.Hidden;
			m_video3.Visibility = Visibility.Hidden;
			m_video4.Visibility = Visibility.Hidden;

			m_imgBlank1.Visibility = Visibility.Hidden;
			m_imgBlank2.Visibility = Visibility.Hidden;
			m_imgBlank3.Visibility = Visibility.Hidden;
			m_imgBlank4.Visibility = Visibility.Hidden;


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

		private void m_btnNext_Click(object sender, RoutedEventArgs e)
		{
			m_videoIntro.Stop();

			m_videoIntro.Visibility = Visibility.Hidden;
			m_btnNext.Visibility = Visibility.Hidden;

			m_video1.Visibility = Visibility.Visible;
			m_video2.Visibility = Visibility.Visible;
			m_video3.Visibility = Visibility.Visible;
			m_video4.Visibility = Visibility.Visible;

#if DEBUG
			m_video1.SpeedRatio = 20;
			m_video2.SpeedRatio = 20;
			m_video3.SpeedRatio = 20;
			m_video4.SpeedRatio = 20;
#endif
			// 인트로 배경음악 종료
			m_soundIntroBackground.Stop();

			m_soundVideo1Background.Position = TimeSpan.Zero;
			m_soundVideo2Background.Position = TimeSpan.Zero;
			m_soundVideo3Background.Position = TimeSpan.Zero;
			m_soundVideo4Background.Position = TimeSpan.Zero;

			m_video1.Position = TimeSpan.Zero;
			m_video2.Position = TimeSpan.Zero;
			m_video3.Position = TimeSpan.Zero;
			m_video4.Position = TimeSpan.Zero;

			m_video1.Play();
			m_soundVideo1Background.Play();
		}

		private void m_video1_MediaEnded(object sender, RoutedEventArgs e)
		{
			m_soundVideo1Background.Stop();

			m_video2.Play();
			m_soundVideo2Background.Play();
		}

		private void m_video2_MediaEnded(object sender, RoutedEventArgs e)
		{
			m_soundVideo2Background.Stop();

			m_video3.Play();
			m_soundVideo3Background.Play();
		}

		private void m_video3_MediaEnded(object sender, RoutedEventArgs e)
		{
			m_soundVideo3Background.Stop();

			m_video4.Play();
			m_soundVideo4Background.Play();
		}

		private void m_video4_MediaEnded(object sender, RoutedEventArgs e)
		{
			m_soundVideo4Background.Stop();

			GameStart();
		}

		int m_numRandom = 0;
		private void GameStart()
		{
			double aW = m_canvas.ActualWidth;
			double aH = m_canvas.ActualHeight;

			Size allImgSize = new Size(aW * 0.22, aH * 0.19);
			string[] dragImages = new string[4];
			Point[] posOriginal = new Point[4];
			Point[] posCorrect = new Point[4];
			dragImages[0] = "PageGame2_03_보기1.png";
			dragImages[1] = "PageGame2_03_보기2.png";
			dragImages[2] = "PageGame2_03_보기3.png";
			dragImages[3] = "PageGame2_03_보기4.png";

			posOriginal[0] = new Point(aW * ((1680 - allImgSize.Width / 2.0) / 1920.0), aH * ((195 - allImgSize.Height / 2.0) / 1080.0));
			posOriginal[1] = new Point(aW * ((1680 - allImgSize.Width / 2.0) / 1920.0), aH * ((430 - allImgSize.Height / 2.0) / 1080.0));
			posOriginal[2] = new Point(aW * ((1680 - allImgSize.Width / 2.0) / 1920.0), aH * ((665 - allImgSize.Height / 2.0) / 1080.0));
			posOriginal[3] = new Point(aW * ((1680 - allImgSize.Width / 2.0) / 1920.0), aH * ((900 - allImgSize.Height / 2.0) / 1080.0));

			posCorrect[0] = new Point(aW * ((360 - allImgSize.Width / 2.0) / 1920.0), aH * ((494 - allImgSize.Height / 2.0) / 1080.0));
			posCorrect[1] = new Point(aW * ((1078 - allImgSize.Width / 2.0) / 1920.0), aH * ((494 - allImgSize.Height / 2.0) / 1080.0));
			posCorrect[2] = new Point(aW * ((360 - allImgSize.Width / 2.0) / 1920.0), aH * ((998 - allImgSize.Height / 2.0) / 1080.0));
			posCorrect[3] = new Point(aW * ((1078 - allImgSize.Width / 2.0) / 1920.0), aH * ((998 - allImgSize.Height / 2.0) / 1080.0));

			m_numRandom = RandomNumber(1, 4 + 1);
			int[] randIdx = new int[4];
			if (m_numRandom == 1)
			{
				randIdx[0] = 1; randIdx[1] = 3; randIdx[2] = 2; randIdx[3] = 4;
			}
			else if (m_numRandom == 2)
			{
				randIdx[0] = 2; randIdx[1] = 1; randIdx[2] = 4; randIdx[3] = 3;
			}
			else if (m_numRandom == 3)
			{
				randIdx[0] = 3; randIdx[1] = 4; randIdx[2] = 1; randIdx[3] = 2;
			}
			else if (m_numRandom == 4)
			{
				randIdx[0] = 4; randIdx[1] = 2; randIdx[2] = 3; randIdx[3] = 1;
			}

			m_dragImg1 = new DragImage(dragImages[0]);
			m_dragImg2 = new DragImage(dragImages[1]);
			m_dragImg3 = new DragImage(dragImages[2]);
			m_dragImg4 = new DragImage(dragImages[3]);

			m_canvas.Children.Add(m_dragImg1);
			m_canvas.Children.Add(m_dragImg2);
			m_canvas.Children.Add(m_dragImg3);
			m_canvas.Children.Add(m_dragImg4);

			m_dragImg1.ImgSize = allImgSize;
			m_dragImg1.OriginalPosition = posOriginal[randIdx[0] - 1];
			m_dragImg1.CorrectPosition = posCorrect[0];
			m_dragImg1.CorrectRadius = aW * 0.2;

			m_dragImg2.ImgSize = allImgSize;
			m_dragImg2.OriginalPosition = posOriginal[randIdx[1] - 1];
			m_dragImg2.CorrectPosition = posCorrect[1];
			m_dragImg2.CorrectRadius = aW * 0.2;

			m_dragImg3.ImgSize = allImgSize;
			m_dragImg3.OriginalPosition = posOriginal[randIdx[2] - 1];
			m_dragImg3.CorrectPosition = posCorrect[2];
			m_dragImg3.CorrectRadius = aW * 0.2;

			m_dragImg4.ImgSize = allImgSize;
			m_dragImg4.OriginalPosition = posOriginal[randIdx[3] - 1];
			m_dragImg4.CorrectPosition = posCorrect[3];
			m_dragImg4.CorrectRadius = aW * 0.2;

			m_dragImg1.GoToOriginalPosition();
			m_dragImg2.GoToOriginalPosition();
			m_dragImg3.GoToOriginalPosition();
			m_dragImg4.GoToOriginalPosition();

			this.m_lastGripState = GripState.Released;
			KinectRegion.AddHandPointerGripHandler(m_dragImg1, this.OnHandPointerGrip);
			KinectRegion.AddHandPointerGripHandler(m_dragImg2, this.OnHandPointerGrip);
			KinectRegion.AddHandPointerGripHandler(m_dragImg3, this.OnHandPointerGrip);
			KinectRegion.AddHandPointerGripHandler(m_dragImg4, this.OnHandPointerGrip);
			KinectRegion.AddHandPointerMoveHandler(m_canvas, this.OnHandPointerMove);
			KinectRegion.AddHandPointerGripReleaseHandler(m_canvas, this.OnHandPointerGripRelease);
			KinectRegion.AddQueryInteractionStatusHandler(m_canvas, this.OnQueryInteractionStatus);

			m_dragImg1.Visibility = Visibility.Visible;
			m_dragImg2.Visibility = Visibility.Visible;
			m_dragImg3.Visibility = Visibility.Visible;
			m_dragImg4.Visibility = Visibility.Visible;
			m_canvas.Visibility = Visibility.Visible;

			m_imgBlank1.Visibility = Visibility.Visible;
			m_imgBlank2.Visibility = Visibility.Visible;
			m_imgBlank3.Visibility = Visibility.Visible;
			m_imgBlank4.Visibility = Visibility.Visible;

			// 배경음악 시작
			m_soundBackground.Position = TimeSpan.Zero;
			m_soundBackground.Play();

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
			if (m_cntRemainSecond < 0 || m_bSkip == true || m_nScore >= 40)
			{
				// 타이머 종료
				m_timerPageFinish.Stop();

				KinectRegion.RemoveHandPointerGripHandler(m_dragImg1, this.OnHandPointerGrip);
				KinectRegion.RemoveHandPointerGripHandler(m_dragImg2, this.OnHandPointerGrip);
				KinectRegion.RemoveHandPointerGripHandler(m_dragImg3, this.OnHandPointerGrip);
				KinectRegion.RemoveHandPointerGripHandler(m_dragImg4, this.OnHandPointerGrip);
				KinectRegion.RemoveHandPointerMoveHandler(m_canvas, this.OnHandPointerMove);
				KinectRegion.RemoveHandPointerGripReleaseHandler(m_canvas, this.OnHandPointerGripRelease);
				KinectRegion.RemoveQueryInteractionStatusHandler(m_canvas, this.OnQueryInteractionStatus);

				m_video1.Stop();
				m_video2.Stop();
				m_video3.Stop();
				m_video4.Stop();

				m_canvas.Children.Clear();
				m_dragImg1 = null;
				m_dragImg2 = null;
				m_dragImg3 = null;
				m_dragImg4 = null;

				// kinect control off
				m_evtUnBindHand(null, null);

				// 배경음악 종료
				m_soundBackground.Stop();

				// 페이지 종료
				if (m_nScore >= 40)
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

		private void OnHandPointerMove(object sender, HandPointerEventArgs kinectHandPointerEventArgs)
		{
			if (m_canvas.Equals(kinectHandPointerEventArgs.HandPointer.Captured))
			{
				kinectHandPointerEventArgs.Handled = true;

				var currentPosition = kinectHandPointerEventArgs.HandPointer.GetPosition(m_canvas);

				//this.sampleTracker.AddSample(currentPosition.X, currentPosition.Y, kinectHandPointerEventArgs.HandPointer.TimestampOfLastUpdate);
				this.m_lastTimeStamp = kinectHandPointerEventArgs.HandPointer.TimestampOfLastUpdate;
				if (this.m_lastGripState == GripState.Released)
				{
					return;
				}

				if (!kinectHandPointerEventArgs.HandPointer.IsInteractive)
				{
					this.m_lastGripState = GripState.Released;
					return;
				}

				//move item
				if (m_currentItem != null)
				{
					Canvas.SetLeft(m_currentItem, currentPosition.X - ((DragImage)m_currentItem).Width / 2);
					Canvas.SetTop(m_currentItem, currentPosition.Y - ((DragImage)m_currentItem).Height / 2);
				}
			}
		}

		private void OnHandPointerGrip(object sender, HandPointerEventArgs kinectHandPointerEventArgs)
		{
			if (kinectHandPointerEventArgs.HandPointer.IsPrimaryUser && kinectHandPointerEventArgs.HandPointer.IsPrimaryHandOfUser && kinectHandPointerEventArgs.HandPointer.IsInteractive)
			{
				this.HandleHandPointerGrip(kinectHandPointerEventArgs.HandPointer);
				kinectHandPointerEventArgs.Handled = true;

				m_currentItem = (UIElement)sender;
			}
		}

		private void HandleHandPointerGrip(HandPointer handPointer)
		{
			if (handPointer == null)
			{
				return;
			}

			if (handPointer.Captured == null)
			{
				// Only capture hand pointer if it isn't already captured
				handPointer.Capture(m_canvas);
			}
			else
			{
				// Some other control has capture, ignore grip
				return;
			}

			this.m_lastGripState = GripState.Gripped;
			this.m_lastGripTimeStamp = handPointer.TimestampOfLastUpdate;
			this.m_gripPoint = handPointer.GetPosition(m_canvas);
		}

		private void OnHandPointerGripRelease(object sender, HandPointerEventArgs kinectHandPointerEventArgs)
		{
			this.m_lastGripReleaseTimeStamp = kinectHandPointerEventArgs.HandPointer.TimestampOfLastUpdate;

			if (m_canvas.Equals(kinectHandPointerEventArgs.HandPointer.Captured))
			{
				//kinectHandPointerEventArgs.Handled = true;
				this.m_lastGripState = GripState.Released;

				kinectHandPointerEventArgs.HandPointer.Capture(null);

				Point currentPosition = kinectHandPointerEventArgs.HandPointer.GetPosition(m_canvas);
				currentPosition.X = currentPosition.X - ((DragImage)m_currentItem).Width / 2;
				currentPosition.Y = currentPosition.Y - ((DragImage)m_currentItem).Height / 2;

				if ((m_currentItem as DragImage).CheckPosition(currentPosition))
				{
					//show message and fix position
					(m_currentItem as DragImage).GoToCorrectPosition();

					m_nScore += 10;

					KinectRegion.RemoveHandPointerGripHandler(m_currentItem, this.OnHandPointerGrip);
				}
				else
				{
					//labelResult.Content = "틀렸어요!";

					(m_currentItem as DragImage).GoToOriginalPosition();
				}

				m_currentItem = null;
			}
		}

		private void OnQueryInteractionStatus(object sender, QueryInteractionStatusEventArgs queryInteractionStatusEventArgs)
		{
			if (m_canvas.Equals(queryInteractionStatusEventArgs.HandPointer.Captured))
			{
				queryInteractionStatusEventArgs.IsInGripInteraction = this.m_lastGripState == GripState.Gripped;
				queryInteractionStatusEventArgs.Handled = true;
			}
		}

		private void m_video1_Loaded(object sender, RoutedEventArgs e)
		{
			m_video1.Play();
			m_video1.Pause();
		}

		private void m_video2_Loaded(object sender, RoutedEventArgs e)
		{
			m_video2.Play();
			m_video2.Pause();
		}

		private void m_video3_Loaded(object sender, RoutedEventArgs e)
		{
			m_video3.Play();
			m_video3.Pause();
		}

		private void m_video4_Loaded(object sender, RoutedEventArgs e)
		{
			m_video4.Play();
			m_video4.Pause();
		}

		private int RandomNumber(int min, int max)
		{
			Random random = new Random(DateTime.Now.Millisecond);
			return random.Next(min, max);
		}
	}
}
