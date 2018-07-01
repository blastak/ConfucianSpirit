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
	/// Interaction logic for PageFeedback.xaml
	/// </summary>
	public partial class PageFeedback : Page
	{
		string m_strbase = @"pack://application:,,/";
		public event EventHandler m_evtPageFinish;
		public event EventHandler m_evtBindHand;
		public event EventHandler m_evtUnBindHand;
		public event EventHandler m_evtBindSkeletonImage;
		public event EventHandler m_evtUnBindSkeletonImage;

		public bool m_bGoodOrBad;

		private MediaPlayer m_soundGoodBackground = new MediaPlayer();
		private MediaPlayer m_soundBadBackground = new MediaPlayer();
		private bool m_bOnce = true;

		public MyKinectSensor m_myKinect = null;

		public PageFeedback()
		{
			InitializeComponent();
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			if (m_bOnce)
			{
				m_bOnce = false;

				m_soundGoodBackground.Open(new Uri("Media/" + "PageFeedback_Good.wav", UriKind.Relative));
				m_soundGoodBackground.Volume = 1;

				m_soundBadBackground.Open(new Uri("Media/" + "PageFeedback_Bad.mp3", UriKind.Relative));
				m_soundBadBackground.Volume = 1;
			}

			if (m_bGoodOrBad == true)
			{
				m_imgGoodOrBad.Source = new BitmapImage(new Uri(m_strbase + "Images/" + "PageFeedback_01_잘했을때.png"));
				m_soundGoodBackground.Position = TimeSpan.Zero;
				m_soundGoodBackground.Play();

				double aw = m_imgGoodOrBad.ActualWidth;
				double ah = m_imgGoodOrBad.ActualHeight;
				m_myKinect.m_faceOnlyPoint = new Point(640.0 * (558.0 / 1920.0), 480.0 * (168.0 / 1080.0));
				m_myKinect.m_faceOnlyScale = 3;
			}
			else
			{
				m_imgGoodOrBad.Source = new BitmapImage(new Uri(m_strbase + "Images/" + "PageFeedback_02_못했을때.png"));
				m_soundBadBackground.Position = TimeSpan.Zero;
				m_soundBadBackground.Play();

				double aw = m_imgGoodOrBad.ActualWidth;
				double ah = m_imgGoodOrBad.ActualHeight;
				m_myKinect.m_faceOnlyPoint = new Point(640.0 * (452.0 / 1920.0), 480.0 * (493.0/ 1080.0));
				m_myKinect.m_faceOnlyScale = 3;
			}

			m_myKinect.m_faceOnlyMode = true;
			m_evtBindSkeletonImage(m_imgSkeleton, null);

			m_imgGoodOrBad.Visibility = Visibility.Visible;

			m_evtBindHand(null, null);
		}

		private void m_btnNext_Click(object sender, RoutedEventArgs e)
		{
			m_soundGoodBackground.Stop();
			m_soundBadBackground.Stop();

			m_myKinect.m_faceOnlyMode = false;
			m_evtUnBindSkeletonImage(null, null);

			m_evtUnBindHand(null, null);

			m_evtPageFinish(null, null);
		}
	}
}
