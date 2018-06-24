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

			m_soundBackground.Open(new Uri("Media/" + "PageGame2_배경음악.mp3", UriKind.Relative));
			m_soundBackground.Volume = 1;
			m_soundBackground.MediaEnded += new EventHandler(BackgroundMusicEnd);

			m_timerPageFinish.Interval = TimeSpan.FromSeconds(1);
			m_timerPageFinish.Tick += new EventHandler(TimerPageFinish);
		}

		private void BackgroundMusicEnd(object sender, EventArgs e)
		{
			m_soundBackground.Position = TimeSpan.Zero;
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

			// 배경음악 시작
			m_soundBackground.Position = TimeSpan.Zero;
			m_soundBackground.Play();

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

			m_video1.Position = TimeSpan.Zero;
			m_video2.Position = TimeSpan.Zero;
			m_video3.Position = TimeSpan.Zero;
			m_video4.Position = TimeSpan.Zero;

			m_video1.Play();
		}

		private void m_video1_MediaEnded(object sender, RoutedEventArgs e)
		{
			m_video2.Play();
		}

		private void m_video2_MediaEnded(object sender, RoutedEventArgs e)
		{
			m_video3.Play();
		}

		private void m_video3_MediaEnded(object sender, RoutedEventArgs e)
		{
			m_video4.Play();
		}

		private void m_video4_MediaEnded(object sender, RoutedEventArgs e)
		{
			GameStart();
		}

		private void GameStart()
		{
			m_dragImg1 = new DragImage("PageGame2_03_보기1.png");
			m_dragImg2 = new DragImage("PageGame2_03_보기2.png");
			m_dragImg3 = new DragImage("PageGame2_03_보기3.png");
			m_dragImg4 = new DragImage("PageGame2_03_보기4.png");

			m_canvas.Children.Add(m_dragImg1);
			m_canvas.Children.Add(m_dragImg2);
			m_canvas.Children.Add(m_dragImg3);
			m_canvas.Children.Add(m_dragImg4);

			double aW = m_canvas.ActualWidth;
			double aH = m_canvas.ActualHeight;


			m_dragImg1.ImgSize = new Size(aW * 0.2, aH * 0.1);
			m_dragImg1.OriginalPosition = new Point(aW * 0.775, aH * 0.2);
			m_dragImg1.CorrectPosition = new Point(aW * 0.0875, aH * 0.407);
			m_dragImg1.CorrectRadius = aW * 0.2;

			m_dragImg2.ImgSize = new Size(aW * 0.2, aH * 0.1);
			m_dragImg2.OriginalPosition = new Point(aW * 0.775, aH * 0.4);
			m_dragImg2.CorrectPosition = new Point(aW * 0.46328125, aH * 0.407);
			m_dragImg2.CorrectRadius = aW * 0.2;

			m_dragImg3.ImgSize = new Size(aW * 0.2, aH * 0.1);
			m_dragImg3.OriginalPosition = new Point(aW * 0.775, aH * 0.6);
			m_dragImg3.CorrectPosition = new Point(aW * 0.0875, aH * 0.87361);
			m_dragImg3.CorrectRadius = aW * 0.2;

			m_dragImg4.ImgSize = new Size(aW * 0.2, aH * 0.1);
			m_dragImg4.OriginalPosition = new Point(aW * 0.775, aH * 0.8);
			m_dragImg4.CorrectPosition = new Point(aW * 0.46328125, aH * 0.87361);
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
	}
}
