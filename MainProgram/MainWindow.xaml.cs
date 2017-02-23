using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Kinect.Toolkit.Controls;
using System.Windows.Threading;

using HrkimKinectSensor;

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

		public MainWindow()
		{
			InitializeComponent();

			// 1. Loading 화면 보여주기
			LoadingMask.Visibility = Visibility.Visible;

			pageStart = new PageStart();
			pageStart.BtnClicked1 += new EventHandler(PageStartBtnClicked1);
			pageStart.BtnClicked2 += new EventHandler(PageStartBtnClicked2);
			pageStart.BtnClicked3 += new EventHandler(PageStartBtnClicked3);
			frame.Navigate(pageStart);
			
			Timer.Interval = TimeSpan.FromSeconds(0.1);
			Timer.Tick += new EventHandler(TimerInit);
			Timer.Start();
		}

		private void TimerInit(object sender, EventArgs e)
		{
			Timer.Stop();

			// 2. 키넥트 실행
			myKinect = new MyKinectSensor(sensorChooserUi);

			// 3. Loading 화면 안보이기
			LoadingMask.Visibility = Visibility.Hidden;

			// 4. 손 컨트롤 켜기
			Bind();

			// 
			pageItem1 = new PageItem1(myKinect);

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

		void PageStartBtnClicked1(object sender, EventArgs e)
		{
			//frame.Navigate(introPage);
			Unbind();
			frame.Navigate(pageItem1);
		}

		void PageStartBtnClicked2(object sender, EventArgs e)
		{
			//frame.Navigate(introPage);
		}

		void PageStartBtnClicked3(object sender, EventArgs e)
		{
			//frame.Navigate(introPage);
			Bind();
		}

		public void ShowingImg(string strFileName)
		{
			try
			{
				BitmapImage bitmap = new BitmapImage();
				bitmap.BeginInit();
				bitmap.UriSource = new Uri(strWorkDir + strFileName);
				bitmap.EndInit();
				//BackgroundImg.Source = bitmap;
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}
		public void ShowingImg(BitmapImage bitmap)
		{
			try
			{
				//BackgroundImg.Source = bitmap;
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
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
		}
	}
}
