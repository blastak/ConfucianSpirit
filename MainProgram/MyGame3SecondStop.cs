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
	public class MyGame3SecondStop
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

		Point[] m_truePoints;
		int m_trueIdx;
		double m_radius;

		int m_idxRange;
		DateTime m_startTime;

		double m_aW;
		double m_aH;

		string m_strbase = @"pack://application:,,/";
		
		public MyGame3SecondStop()
		{
			m_timerCountdown.Tick += new EventHandler(TimerCountdown);
		}

		public void SetupUI(Canvas canvas, Image tfFace)
		{
			m_canvas = canvas;
			m_imgTFFace = tfFace;
		}

		public void SetupResource(string background, string questionSound, Point[] truePoints, int trueIdx, double radius)
		{
			m_strBackground = background;
			m_strQuestionSound = questionSound;
			m_trueIdx = trueIdx;
			m_truePoints = truePoints;
			m_radius = radius;
		}

		public void Entrypoint()
		{
			m_aW = m_canvas.ActualWidth;
			m_aH = m_canvas.ActualHeight;
			m_timeRemain = 60;
			score = 0;

			// 1. 배경 보여주기
			m_canvas.Background = new ImageBrush(new BitmapImage(new Uri(m_strbase + "Images/" + m_strBackground)));

			// 2. 사운드 재생
			if (m_strQuestionSound != "")
			{
				m_startSound.Open(new Uri("Sounds/" + m_strQuestionSound, UriKind.Relative)); // 속성:빌드시자동복사
				m_startSound.MediaEnded += new EventHandler(MediaEnd1);
				m_startSound.Volume = 1;
				//m_startSound.Position = TimeSpan.FromSeconds(45);
				m_startSound.Play();
			}
			else
			{
				// 7. 제한시간 시작
				m_timerCountdown.Interval = TimeSpan.FromMilliseconds(1000);
				m_timerCountdown.Start();
			}
		}

		// 3. 사운드 끝날때까지 딜레이
		private void MediaEnd1(object sender, EventArgs e)
		{
			//System.Diagnostics.Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

			m_startSound.MediaEnded -= new EventHandler(MediaEnd1);
			m_startSound.Stop();
			m_startSound.Close();

			// 7. 제한시간 시작
			m_timerCountdown.Interval = TimeSpan.FromMilliseconds(1000);
			m_timerCountdown.Start();
			

			KinectRegion.AddHandPointerMoveHandler(m_canvas, this.OnHandPointerMove);
			KinectRegion.AddHandPointerEnterHandler(m_canvas, this.OnHandPointerEnter);
			KinectRegion.AddHandPointerLeaveHandler(m_canvas, this.OnHandPointerLeave);
			
			m_idxRange = -1;
			m_startTime = DateTime.MinValue;
		}

		private void TimerCountdown(object sender, EventArgs e)
		{
			//System.Diagnostics.Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

			m_timeRemain -= 1;
			if (score >= 1)
			{
				m_timerCountdown.Stop();
				if (m_idxRange != m_trueIdx)
					ResultGame(false);
				else
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
			//System.Diagnostics.Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

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
			//System.Diagnostics.Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

			m_startSound.MediaEnded -= new EventHandler(MediaEnd2);
			m_startSound.Stop();
			m_startSound.Close();

			m_evtGameManager(score * 12, null);
			
			m_imgTFFace.Visibility = Visibility.Hidden;
			
			try
			{
				KinectRegion.RemoveHandPointerMoveHandler(m_canvas, this.OnHandPointerMove);
				KinectRegion.RemoveHandPointerEnterHandler(m_canvas, this.OnHandPointerEnter);
				KinectRegion.RemoveHandPointerLeaveHandler(m_canvas, this.OnHandPointerLeave);
			}
			catch (Exception)
			{

			}
		}

		private void OnHandPointerEnter(object sender, HandPointerEventArgs kinectHandPointerEventArgs)
		{
			//isHandEnter = true;
		}

		private void OnHandPointerLeave(object sender, HandPointerEventArgs kinectHandPointerEventArgs)
		{
			//isHandEnter = false;

			m_idxRange = -1;
			m_startTime = DateTime.MinValue;
		}

		private double SquaredDistance(double x1, double y1, double x2, double y2)
		{
			return Math.Sqrt(((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1)));
		}

		private double PointDistance(Point A, Point B)
		{
			return SquaredDistance(A.X, A.Y, B.X, B.Y);
		}

		private void OnHandPointerMove(object sender, HandPointerEventArgs kinectHandPointerEventArgs)
		{
// 			if(!isHandEnter) // 화면 밖으로 나갔는데 한 자리에 정지한걸로 인식하지 않기 위하여 처리해줘야함
// 			{
// 				return;
// 			}

			Point currPoint = kinectHandPointerEventArgs.HandPointer.GetPosition(m_canvas);
			currPoint.X = currPoint.X / m_aW;
			currPoint.Y = currPoint.Y / m_aH;

			if (m_idxRange != -1)
			{
				if (PointDistance(m_truePoints[m_idxRange], currPoint) < m_radius)
				{
					double milliseconds = DateTime.Now.Subtract(m_startTime).TotalMilliseconds;
					//TimeSpan timeSpan = TimeSpan.FromMilliseconds(milliseconds);

					if(milliseconds > 3000)
					{
						score = 1;
					}

					return;
				}
				else
				{
					score = 0;
					m_idxRange = -1;
					m_startTime = DateTime.MinValue;
				}
			}

			for (int i = 0; i < m_truePoints.Length; i++)
			{
				if(PointDistance(m_truePoints[i],currPoint) < m_radius)
				{
					m_startTime = DateTime.Now;
					m_idxRange = i;
					break;
				}
			}

			//if (m_canvas.Equals(kinectHandPointerEventArgs.HandPointer.Captured))
			//{
			//	kinectHandPointerEventArgs.Handled = true;

			//	var currentPosition = kinectHandPointerEventArgs.HandPointer.GetPosition(m_canvas);

			//	//this.sampleTracker.AddSample(currentPosition.X, currentPosition.Y, kinectHandPointerEventArgs.HandPointer.TimestampOfLastUpdate);
			//	this.lastTimeStamp = kinectHandPointerEventArgs.HandPointer.TimestampOfLastUpdate;
			//	if (this.lastGripState == GripState.Released)
			//	{
			//		return;
			//	}

			//	if (!kinectHandPointerEventArgs.HandPointer.IsInteractive)
			//	{
			//		this.lastGripState = GripState.Released;
			//		return;
			//	}

			//	//move item
			//	if (currentItem != null)
			//	{
			//		Canvas.SetLeft(currentItem, currentPosition.X - ((DragImage)currentItem).Width / 2);
			//		Canvas.SetTop(currentItem, currentPosition.Y - ((DragImage)currentItem).Height / 2);
			//	}
			//}
		}
	}
}
