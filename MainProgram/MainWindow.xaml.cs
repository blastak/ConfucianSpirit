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

using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using System.Windows.Threading;

namespace MainProgram
{
	
	/// <summary>
	/// MainWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class MainWindow : Window
	{
		private KinectSensorChooser sensorChooser;

		public DispatcherTimer Timer = new DispatcherTimer(); // 같은 스레드에서 동작
		string strWorkDir = AppDomain.CurrentDomain.BaseDirectory;

		PageStart pageStart;

		public MainWindow()
		{
			InitializeComponent();

			this.sensorChooser = new KinectSensorChooser();

			// 1. Loading 화면 보여주기
			LoadingMask.Visibility = Visibility.Visible;

			pageStart = new PageStart();
			pageStart.BtnClicked1 += new EventHandler(PageStartBtnClicked1);
			pageStart.BtnClicked2 += new EventHandler(PageStartBtnClicked2);
			pageStart.BtnClicked3 += new EventHandler(PageStartBtnClicked3);

			//frame.Navigate(pageStart);

			Timer.Interval = TimeSpan.FromSeconds(0.1);
			Timer.Tick += new EventHandler(TimerInit);
			Timer.Start();
		}

		private void TimerInit(object sender, EventArgs e)
		{
			Timer.Stop();

			// 2. 키넥트 실행
			InitKinect();

			// 3. Loading 화면 안보이기
			LoadingMask.Visibility = Visibility.Hidden;

			// 4. 버튼 보이기
// 			buttonStart1.Visibility = Visibility.Visible;
// 			buttonStart2.Visibility = Visibility.Visible;
// 			buttonStart3.Visibility = Visibility.Visible;
		}


		private void InitKinect()
		{
			// initialize the sensor chooser and UI
			this.sensorChooserUi.KinectSensorChooser = this.sensorChooser;
			this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
			this.sensorChooser.Start();

			Bind();
		}

		private void Bind()
		{
			// Bind the senUserControl1.xamlsor chooser's current sensor to the KinectRegion
			var regionSensorBinding = new Binding("Kinect") { Source = this.sensorChooser };
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

		private static void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs args)
		{
			if (args.OldSensor != null)
			{
				try
				{
					args.OldSensor.DepthStream.Range = DepthRange.Default;
					args.OldSensor.SkeletonStream.EnableTrackingInNearRange = false;
					args.OldSensor.DepthStream.Disable();
					args.OldSensor.SkeletonStream.Disable();
				}
				catch (InvalidOperationException)
				{
					// KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
					// E.g.: sensor might be abruptly unplugged.
				}
			}

			if (args.NewSensor != null)
			{
				try
				{
					args.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
					args.NewSensor.SkeletonStream.Enable();

// 					try
// 					{
// 						args.NewSensor.DepthStream.Range = DepthRange.Near;
// 						args.NewSensor.SkeletonStream.EnableTrackingInNearRange = true;
// 					}
// 					catch (InvalidOperationException)
// 					{
// 						// Non Kinect for Windows devices do not support Near mode, so reset back to default mode.
// 						args.NewSensor.DepthStream.Range = DepthRange.Default;
// 						args.NewSensor.SkeletonStream.EnableTrackingInNearRange = false;
// 					}
					args.NewSensor.DepthStream.Range = DepthRange.Default;
					args.NewSensor.SkeletonStream.EnableTrackingInNearRange = false;
				}
				catch (InvalidOperationException)
				{
					// KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
					// E.g.: sensor might be abruptly unplugged.
				}
			}
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
			this.sensorChooser.Stop();
		}
	}
}
