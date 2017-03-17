using Microsoft.Kinect.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;

namespace MainProgram
{
	class MyGameDragAndDrop
	{
		public event EventHandler m_evtGameManager;

		private Canvas m_canvas;
		private Image m_imgTFFace;

		private string m_strBackground;
		private string m_strQuestionSound;

		private int m_timeRemain;
		private DispatcherTimer m_timerCountdown = new DispatcherTimer();
		private MediaPlayer m_startSound = new MediaPlayer();

		int score;


		private enum GripState
		{
			Released,
			Gripped
		}
		private GripState lastGripState;
		private long lastTimeStamp;
		private long lastGripTimeStamp;
		private long lastGripReleaseTimeStamp;

		UIElement currentItem;
		private Point gripPoint;

		public DragImage dragImg1;
		public DragImage dragImg2;
		string m_strbase = @"pack://application:,,/";

		int truePos = 0;

		public MyGameDragAndDrop(int idx)
		{
			m_timerCountdown.Tick += new EventHandler(TimerCountdown);

			if (idx == 1)
			{
				dragImg1 = new DragImage("신충_03_02.png");
				dragImg2 = new DragImage("신충_03_03.png");
				truePos = 1;
			}
			else if (idx == 2)
			{
				dragImg1 = new DragImage("신충_04_02.png");
				dragImg2 = new DragImage("신충_04_03.png");
				truePos = 2;
			}
			else if (idx == 3)
			{
				dragImg1 = new DragImage("신충_05_02.png");
				dragImg2 = new DragImage("신충_05_03.png");
				truePos = 1;
			}
			else if (idx == 4)
			{
				dragImg1 = new DragImage("신충_06_02.png");
				dragImg2 = new DragImage("신충_06_03.png");
				truePos = 2;
			}
		}

		public void SetupUI(Canvas canvas, Image tfFace)
		{
			m_canvas = canvas;
			m_imgTFFace = tfFace;
		}

		public void SetupResource(string background, string questionSound)
		{
			m_strBackground = background;
			m_strQuestionSound = questionSound;
		}

		public void Entrypoint()
		{
			double aW = m_canvas.ActualWidth;
			double aH = m_canvas.ActualHeight;

			m_canvas.Children.Add(dragImg1);
			m_canvas.Children.Add(dragImg2);

			dragImg1.ImgSize = new Size(aW * 0.25, aH * 0.2);
			dragImg1.OriginalPosition = new Point(aW * 0.72, aH * 0.2);
			dragImg1.CorrectRadius = aW * 0.2;

			dragImg2.ImgSize = new Size(aW * 0.25, aH * 0.2);
			dragImg2.OriginalPosition = new Point(aW * 0.72, aH * 0.6);
			dragImg2.CorrectRadius = aW * 0.2;

			if(truePos == 1)
			{
				dragImg1.CorrectPosition = new Point(aW * 0.23, aH * 0.80);
				dragImg2.CorrectPosition = new Point(aW * -1, aH * -1);
			}
			else
			{
				dragImg1.CorrectPosition = new Point(aW * -1, aH * -1);
				dragImg2.CorrectPosition = new Point(aW * 0.23, aH * 0.80);
			}

			dragImg1.Visibility = Visibility.Visible;
			dragImg2.Visibility = Visibility.Visible;

			this.lastGripState = GripState.Released;
			dragImg1.GoToOriginalPosition();
			dragImg2.GoToOriginalPosition();


			m_timeRemain = 30;
			score = 0;

			// 1. 배경 보여주기
			m_canvas.Background = new ImageBrush(new BitmapImage(new Uri(m_strbase + "Images/" + m_strBackground)));

			// 2. 사운드 재생
			if(m_strQuestionSound != "")
			{
				m_startSound.Open(new Uri("Sounds/" + m_strQuestionSound, UriKind.Relative)); // 속성:빌드시자동복사
				m_startSound.MediaEnded += new EventHandler(MediaEnd1);
				m_startSound.Volume = 1;
				m_startSound.Play();
			}
			else
			{
				// 7. 제한시간 시작
				m_timerCountdown.Interval = TimeSpan.FromMilliseconds(1000);
				m_timerCountdown.Start();
			}

			this.lastGripState = GripState.Released;
			KinectRegion.AddHandPointerGripHandler(dragImg1, this.OnHandPointerGrip);
			KinectRegion.AddHandPointerGripHandler(dragImg2, this.OnHandPointerGrip);
			KinectRegion.AddHandPointerMoveHandler(m_canvas, this.OnHandPointerMove);
			KinectRegion.AddHandPointerGripReleaseHandler(m_canvas, this.OnHandPointerGripRelease);
			KinectRegion.AddQueryInteractionStatusHandler(m_canvas, this.OnQueryInteractionStatus);

			// 			runningGameThread = true;
			// 			var myTimerThread = new Thread(this.TimerThread);
			// 			myTimerThread.SetApartmentState(ApartmentState.STA);
			// 			myTimerThread.Start();


		}

		// 3. 사운드 끝날때까지 딜레이
		private void MediaEnd1(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

			m_startSound.MediaEnded -= new EventHandler(MediaEnd1);
			m_startSound.Stop();
			m_startSound.Close();

			// 7. 제한시간 시작
			m_timerCountdown.Interval = TimeSpan.FromMilliseconds(1000);
			m_timerCountdown.Start();
		}

		private void TimerCountdown(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

			m_timeRemain -= 1;
			if(score >= 4)
			{
				m_timerCountdown.Stop();
				ResultGame(true);
			}
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
			string[] strSoundName = { "Sounds/중간평가_1(성공).m4a", "Sounds/중간평가_2(다음기회에).m4a" };

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

			m_evtGameManager(score, null);

			m_canvas.Children.Remove(dragImg1);
			m_canvas.Children.Remove(dragImg2);

			m_imgTFFace.Visibility = Visibility.Hidden;

			dragImg1.Visibility = Visibility.Hidden;
			dragImg2.Visibility = Visibility.Hidden;

			try
			{
				KinectRegion.RemoveHandPointerGripHandler(dragImg1, this.OnHandPointerGrip);
				KinectRegion.RemoveHandPointerGripHandler(dragImg2, this.OnHandPointerGrip);
				KinectRegion.RemoveHandPointerMoveHandler(m_canvas, this.OnHandPointerMove);
				KinectRegion.RemoveHandPointerGripReleaseHandler(m_canvas, this.OnHandPointerGripRelease);
				KinectRegion.RemoveQueryInteractionStatusHandler(m_canvas, this.OnQueryInteractionStatus);
			}
			catch(Exception)
			{

			}
		}

		private void OnHandPointerMove(object sender, HandPointerEventArgs kinectHandPointerEventArgs)
		{
			if (m_canvas.Equals(kinectHandPointerEventArgs.HandPointer.Captured))
			{
				kinectHandPointerEventArgs.Handled = true;

				var currentPosition = kinectHandPointerEventArgs.HandPointer.GetPosition(m_canvas);

				//this.sampleTracker.AddSample(currentPosition.X, currentPosition.Y, kinectHandPointerEventArgs.HandPointer.TimestampOfLastUpdate);
				this.lastTimeStamp = kinectHandPointerEventArgs.HandPointer.TimestampOfLastUpdate;
				if (this.lastGripState == GripState.Released)
				{
					return;
				}

				if (!kinectHandPointerEventArgs.HandPointer.IsInteractive)
				{
					this.lastGripState = GripState.Released;
					return;
				}

				//move item
				if (currentItem != null)
				{
					Canvas.SetLeft(currentItem, currentPosition.X - ((DragImage)currentItem).Width / 2);
					Canvas.SetTop(currentItem, currentPosition.Y - ((DragImage)currentItem).Height / 2);
				}
			}
		}

		private void OnHandPointerGrip(object sender, HandPointerEventArgs kinectHandPointerEventArgs)
		{
			if (kinectHandPointerEventArgs.HandPointer.IsPrimaryUser && kinectHandPointerEventArgs.HandPointer.IsPrimaryHandOfUser && kinectHandPointerEventArgs.HandPointer.IsInteractive)
			{
				this.HandleHandPointerGrip(kinectHandPointerEventArgs.HandPointer);
				kinectHandPointerEventArgs.Handled = true;

				currentItem = (UIElement)sender;
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

			this.lastGripState = GripState.Gripped;
			this.lastGripTimeStamp = handPointer.TimestampOfLastUpdate;
			this.gripPoint = handPointer.GetPosition(m_canvas);
		}

		private void OnHandPointerGripRelease(object sender, HandPointerEventArgs kinectHandPointerEventArgs)
		{
			this.lastGripReleaseTimeStamp = kinectHandPointerEventArgs.HandPointer.TimestampOfLastUpdate;

			if (m_canvas.Equals(kinectHandPointerEventArgs.HandPointer.Captured))
			{
				//kinectHandPointerEventArgs.Handled = true;
				this.lastGripState = GripState.Released;

				kinectHandPointerEventArgs.HandPointer.Capture(null);

				Point currentPosition = kinectHandPointerEventArgs.HandPointer.GetPosition(m_canvas);
				currentPosition.X = currentPosition.X - ((DragImage)currentItem).Width / 2;
				currentPosition.Y = currentPosition.Y - ((DragImage)currentItem).Height / 2;

				if ((currentItem as DragImage).CheckPosition(currentPosition))
				{
					//show message and fix position
					(currentItem as DragImage).GoToCorrectPosition();

					score += 1;

					KinectRegion.RemoveHandPointerGripHandler(currentItem, this.OnHandPointerGrip);
				}
				else
				{
					//labelResult.Content = "틀렸어요!";

					(currentItem as DragImage).GoToOriginalPosition();
				}

				currentItem = null;
			}
		}

		private void OnQueryInteractionStatus(object sender, QueryInteractionStatusEventArgs queryInteractionStatusEventArgs)
		{
			if (m_canvas.Equals(queryInteractionStatusEventArgs.HandPointer.Captured))
			{
				queryInteractionStatusEventArgs.IsInGripInteraction = this.lastGripState == GripState.Gripped;
				queryInteractionStatusEventArgs.Handled = true;
			}
		}
	}
}
