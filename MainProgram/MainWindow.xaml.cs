using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Kinect.Toolkit.Controls;
using System.Windows.Threading;
using System.Collections.Generic;

namespace MainProgram
{
	public enum GameEndInfo { GAME_CLEAR, GAME_OVER };

	/// <summary>
	/// MainWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class MainWindow : Window
	{
		public MyKinectSensor myKinect;

		public DispatcherTimer Timer = new DispatcherTimer(); // 같은 스레드에서 동작
		public string strWorkDir = AppDomain.CurrentDomain.BaseDirectory;

		public PageStart pageStart;
		public PageItem1 pageItem1;
		public PageItem2 pageItem2;
		public PageItem3 pageItem3;
		public PageEnd pageEnd;

		public MainWindow()
		{
			InitializeComponent();

			// 1. Loading 화면 보여주기
			LoadingMask.Visibility = Visibility.Visible;

			pageStart = new PageStart();
			pageStart.BtnClicked1 += new EventHandler(PageStartBtnClicked1);
			pageStart.BtnClicked2 += new EventHandler(PageStartBtnClicked2);
			pageStart.BtnClicked3 += new EventHandler(PageStartBtnClicked3);

			pageEnd = new PageEnd();

			Timer.Interval = TimeSpan.FromSeconds(0.1);
			Timer.Tick += new EventHandler(TimerInit);
			Timer.Start();
		}

		private void TimerInit(object sender, EventArgs e) // 이 함수는 딱 한번만 실행됨, 키넥트 로딩시 blocking 때문에 생성자를 나누어 놓은 것
		{
			Timer.Stop();

			// 2. 키넥트 실행
			myKinect = new MyKinectSensor(sensorChooserUi);

			myKinect.BindBackgroundRemovalImage(pageStart.imgUser);
			frame.Navigate(pageStart);

			ReadyToSelect();

			pageItem1 = new PageItem1(myKinect);
			pageItem1.m_evtPageEnd += new EventHandler(EventEndOfPage);
			pageItem2 = new PageItem2(myKinect);
			pageItem2.m_evtPageEnd += new EventHandler(EventEndOfPage);
			pageItem3 = new PageItem3(myKinect);
			pageItem3.m_evtPageEnd += new EventHandler(EventEndOfPage);

			pageEnd.m_evtPageEnd += new EventHandler(EventRestart);
		}

		void EventEndOfPage(object sender, EventArgs e)
		{
			List<object> a = (List<object>)sender;
			pageEnd.Score = (int)a[0];
			pageEnd.DurationTime = (TimeSpan)a[1];

			frame.Navigate(pageEnd);
		}

		void EventRestart(object sender, EventArgs e)
		{
			ReadyToSelect();
			frame.Navigate(pageStart);
		}
		
		private void ReadyToSelect()
		{
			// 3. Loading 화면 안보이기
			LoadingMask.Visibility = Visibility.Hidden;

			// 4. 손 컨트롤 켜기
			Bind();
		}

		private void Bind()
		{
			// Bind the senUserControl1.xamlsor chooser's current sensor to the KinectRegion
			var regionSensorBinding = new Binding("Kinect") { Source = myKinect.sensorChooser };
			BindingOperations.SetBinding(this.kinectRegion, KinectRegion.KinectSensorProperty, regionSensorBinding);
		}

		private void Unbind()
		{
			BindingOperations.ClearBinding(this.kinectRegion, KinectRegion.KinectSensorProperty);
		}

		public void RequestBind()
		{
			Bind();
		}

		public void RequestUnbind()
		{
			Unbind();
		}

		void PageStartBtnClicked1(object sender, EventArgs e)
		{
			Unbind();
			frame.Navigate(pageItem1);
		}

		void PageStartBtnClicked2(object sender, EventArgs e)
		{
			//Unbind();
			frame.Navigate(pageItem2);
		}

		void PageStartBtnClicked3(object sender, EventArgs e)
		{
			//Unbind();
			frame.Navigate(pageItem3);
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
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			myKinect.Closing();
			//pageItem1.m_game5
			pageItem3.m_game3.runningGameThread = false;
			pageItem1.m_game5.runningGameThread = false;
		}
	}
}
