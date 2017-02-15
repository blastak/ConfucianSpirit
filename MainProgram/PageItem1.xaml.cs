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

		private readonly MediaPlayer startSound = new MediaPlayer();

		string strbase = @"pack://application:,,/";

		public PageItem1()
		{
			InitializeComponent();

			this.Loaded += new RoutedEventHandler(PageLoaded);
		}

		private void PageLoaded(object sender, EventArgs e)
		{
			Entrypoint();
		}

		private void Entrypoint()
		{
			// 1. 배경 보여주기
			canvasBG.Background = new ImageBrush(new BitmapImage(new Uri(strbase + @"Images/01_예효_01(손들기).png")));

			// 2. 사운드 재생
			startSound.Open(new Uri("Sounds/01_예효_01(손들기).mp3", UriKind.Relative)); // 속성:빌드시자동복사
			startSound.MediaEnded += new EventHandler(MediaEnd1);
			startSound.Volume = 1;
			startSound.Play();
		}

		// 3. 사운드 끝날때까지 딜레이 x
		private void MediaEnd1(object sender, EventArgs e)
		{
			startSound.MediaEnded -= new EventHandler(MediaEnd1);
			startSound.Stop();
			startSound.Close();

			// 4. 반투명 마스크 보이기
			imgMask.Visibility = Visibility.Visible;

			// 5. 손들기 그림 좌,우에 보이기
			BitmapImage src = new BitmapImage(new Uri("Images/01_예효_02.png", UriKind.Relative));
			imgOverlayLeft.Source = src;

			src = new BitmapImage(new Uri("Images/01_예효_03.png", UriKind.Relative));
			imgOverlayRight.Source = src;
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
		*/


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
		

		private void TimerCountdown(object sender, EventArgs e)
		{

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
	}
}