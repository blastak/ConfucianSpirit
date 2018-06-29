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
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static MyKinectSensor m_myKinect = null;
		public DispatcherTimer m_timerLaunchKinect = new DispatcherTimer();
		public DispatcherTimer m_timerGoToPageBegin = new DispatcherTimer();

		public PageBegin m_pageBegin = new PageBegin();
		public PageGameBow m_pageGameBow = new PageGameBow();
		public PageGame1 m_pageGame1 = new PageGame1();
		public PageGame2 m_pageGame2 = new PageGame2();
		public PageGame3 m_pageGame3 = new PageGame3();
		public PageFeedback m_pageFeedback = new PageFeedback();
		public PageEnd m_pageEnd = new PageEnd();

		int m_numRandom;

		public MainWindow()
		{
			InitializeComponent();

			m_pageBegin.m_evtPageFinish += new EventHandler(GoToNextGame);
			m_pageFeedback.m_evtPageFinish += new EventHandler(GoToNextGame);
			m_pageGameBow.m_evtPageFinish += new EventHandler(GoToNextGame);
			m_pageGame1.m_evtPageFinish += new EventHandler(GoToFeedback);
			m_pageGame2.m_evtPageFinish += new EventHandler(GoToFeedback);
			m_pageGame3.m_evtPageFinish += new EventHandler(GoToFeedback);
			m_pageEnd.m_evtPageFinish += new EventHandler(GoToBegin);

			m_pageBegin.m_evtBindHand += new EventHandler(EventBindHand);
			m_pageBegin.m_evtUnBindHand += new EventHandler(EventUnBindHand);
			m_pageBegin.m_evtBindBGRemoval += new EventHandler(EventBindBGRemoval);
			m_pageBegin.m_evtUnBindBGRemoval += new EventHandler(EventUnBindBGRemoval);

			m_pageGameBow.m_evtBindHand += new EventHandler(EventBindHand);
			m_pageGameBow.m_evtUnBindHand += new EventHandler(EventUnBindHand);
			m_pageGameBow.m_evtBindBGRemoval += new EventHandler(EventBindBGRemoval);
			m_pageGameBow.m_evtUnBindBGRemoval += new EventHandler(EventUnBindBGRemoval);
			m_pageGameBow.m_evtBindSkeletonImage += new EventHandler(EventBindSkeletonImage);
			m_pageGameBow.m_evtUnBindSkeletonImage += new EventHandler(EventUnBindSkeletonImage);

			m_pageGame1.m_evtBindHand += new EventHandler(EventBindHand);
			m_pageGame1.m_evtUnBindHand += new EventHandler(EventUnBindHand);

			m_pageGame2.m_evtBindHand += new EventHandler(EventBindHand);
			m_pageGame2.m_evtUnBindHand += new EventHandler(EventUnBindHand);

			m_pageGame3.m_evtBindHand += new EventHandler(EventBindHand);
			m_pageGame3.m_evtUnBindHand += new EventHandler(EventUnBindHand);
			m_pageGame3.m_evtBindSkeletonImage += new EventHandler(EventBindSkeletonImage);
			m_pageGame3.m_evtUnBindSkeletonImage += new EventHandler(EventUnBindSkeletonImage);

			m_pageFeedback.m_evtBindHand += new EventHandler(EventBindHand);
			m_pageFeedback.m_evtUnBindHand += new EventHandler(EventUnBindHand);

			m_timerLaunchKinect.Interval = TimeSpan.FromSeconds(0.1);
			m_timerLaunchKinect.Tick += new EventHandler(TimerLaunchKinect);
			m_timerLaunchKinect.Start();
		}

		~MainWindow()
		{
			GC.SuppressFinalize(this);
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Escape:
					Close();
					break;
				case Key.Return:
					m_pageGameBow.m_bSkip = true;
					m_pageGame1.m_bSkip = true;
					m_pageGame2.m_bSkip = true;
					m_pageGame3.m_bSkip = true;
					break;
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (m_myKinect != null)
			{
				m_pageGame1.m_gameGravity.m_runningGameThread = false;
				m_myKinect.Closing();
			}
		}

		private void TimerLaunchKinect(object sender, EventArgs e)
		{
			m_timerLaunchKinect.Stop();

			m_myKinect = new MyKinectSensor(m_sensorChooserUi); // 키넥트 실행. 로딩시 blocking 때문에 timer를 이용하였음

			m_timerGoToPageBegin.Interval = TimeSpan.FromSeconds(2); // 최소 2초간 로고 보이기
			m_timerGoToPageBegin.Tick += new EventHandler(TimerGoToPageBegin);
			m_timerGoToPageBegin.Start();
		}

		private void TimerGoToPageBegin(object sender, EventArgs e)
		{
			m_timerGoToPageBegin.Stop();
			m_image.Visibility = Visibility.Hidden;

			m_pageGame1.m_myKinect = m_myKinect;

			GoToBegin(null, null);
		}

		private void GoToBegin(object sender, EventArgs e)
		{
			m_frame.Navigate(m_pageBegin);

			m_numRandom = RandomNumber(1, 6);
#if DEBUG
			m_numRandom = 1;
			idxGame = 1;
#endif
		}

		int idxGame = 0;
		private void GoToNextGame(object sender, EventArgs e)
		{
			idxGame++;

			if(idxGame == 1)
			{
				m_frame.Navigate(m_pageGameBow);
			}
			else if(idxGame == 5)
			{
				m_pageEnd.m_scores = m_pageGame1.m_nScore + m_pageGame2.m_nScore + m_pageGame3.m_nScore;
				m_pageEnd.m_seconds = (60 - m_pageGame1.m_cntRemainSecond + 60 - m_pageGame2.m_cntRemainSecond + 60 - m_pageGame3.m_cntRemainSecond);
				m_frame.Navigate(m_pageEnd);
				idxGame = 0;
			}
			else
			{
				if(m_numRandom == 1) // 1,2,3
				{
					if(idxGame == 2)
						m_frame.Navigate(m_pageGame1);
					else if (idxGame == 3)
						m_frame.Navigate(m_pageGame2);
					else if (idxGame == 4)
						m_frame.Navigate(m_pageGame3);
				}
				else if (m_numRandom == 2) // 1,3,2
				{
					if (idxGame == 2)
						m_frame.Navigate(m_pageGame1);
					else if (idxGame == 3)
						m_frame.Navigate(m_pageGame3);
					else if (idxGame == 4)
						m_frame.Navigate(m_pageGame2);
				}
				else if (m_numRandom == 3) // 2,3,1
				{
					if (idxGame == 2)
						m_frame.Navigate(m_pageGame2);
					else if (idxGame == 3)
						m_frame.Navigate(m_pageGame3);
					else if (idxGame == 4)
						m_frame.Navigate(m_pageGame1);
				}
				else if (m_numRandom == 4) // 2,1,3
				{
					if (idxGame == 2)
						m_frame.Navigate(m_pageGame2);
					else if (idxGame == 3)
						m_frame.Navigate(m_pageGame1);
					else if (idxGame == 4)
						m_frame.Navigate(m_pageGame3);
				}
				else if (m_numRandom == 5) // 3,1,2
				{
					if (idxGame == 2)
						m_frame.Navigate(m_pageGame3);
					else if (idxGame == 3)
						m_frame.Navigate(m_pageGame1);
					else if (idxGame == 4)
						m_frame.Navigate(m_pageGame2);
				}
				else if (m_numRandom == 6) // 3,2,1
				{
					if (idxGame == 2)
						m_frame.Navigate(m_pageGame3);
					else if (idxGame == 3)
						m_frame.Navigate(m_pageGame2);
					else if (idxGame == 4)
						m_frame.Navigate(m_pageGame1);
				}
			}
		}

		private void GoToFeedback(object sender, EventArgs e)
		{
			m_pageFeedback.m_bGoodOrBad = (bool)sender;
			m_frame.Navigate(m_pageFeedback);
		}

		private int RandomNumber(int min, int max)
		{
			Random random = new Random(DateTime.Now.Millisecond);
			return random.Next(min, max);
		}

		private void EventBindHand(object sender, EventArgs e)
		{
			// Bind the senUserControl1.xamlsor chooser's current sensor to the KinectRegion
			var regionSensorBinding = new Binding("Kinect") { Source = m_myKinect.sensorChooser };
			BindingOperations.SetBinding(this.m_kinectRegion, KinectRegion.KinectSensorProperty, regionSensorBinding);
		}

		private void EventUnBindHand(object sender, EventArgs e)
		{
			BindingOperations.ClearBinding(this.m_kinectRegion, KinectRegion.KinectSensorProperty);
		}

		private void EventBindBGRemoval(object sender, EventArgs e)
		{
			m_myKinect.BindBackgroundRemovalImage((Image)sender);
		}

		private void EventUnBindBGRemoval(object sender, EventArgs e)
		{
			m_myKinect.UnBindBackgroundRemovalImage();
		}

		private void EventBindSkeletonImage(object sender, EventArgs e)
		{
			m_myKinect.BindSkeletonImage((Image)sender);
		}

		private void EventUnBindSkeletonImage(object sender, EventArgs e)
		{
			m_myKinect.UnBindSkeletonImage();
		}
	}
}
