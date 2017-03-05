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
using System.Windows.Threading;

namespace MainProgram
{
	/// <summary>
	/// PageEnd.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class PageEnd : Page
	{
		string m_strbase = @"pack://application:,,/";
		private MediaPlayer m_startSound = new MediaPlayer();
		public DispatcherTimer Timer = new DispatcherTimer(); // 같은 스레드에서 동작
		public event EventHandler m_evtPageEnd;

		public PageEnd()
		{
			InitializeComponent();
			this.Loaded += new RoutedEventHandler(PageLoaded);

			Timer.Tick += new EventHandler(Timer_Tick);

			m_startSound.Open(new Uri("Sounds/" + "평가1_배경음악.mp3", UriKind.Relative)); // 속성:빌드시자동복사
			m_startSound.Volume = 1;
		}

		private void PageLoaded(object sender, EventArgs e)
		{
			m_startSound.Play();

			Timer.Interval = TimeSpan.FromMilliseconds(12500);
			Timer.Start();

			textBlock_Time.Text = String.Format("{0:mm\\:ss}", durationTime);
			textBlock_Score.Text = String.Format("{0}", score);
		}

		private TimeSpan durationTime;
		public TimeSpan DurationTime
		{
			get
			{
				return durationTime;
			}
			set
			{
				durationTime = value;
			}
		}

		private int score;
		public int Score
		{
			get
			{
				return score;
			}
			set
			{
				score = value;
			}
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			Timer.Stop();
			m_startSound.Stop();

			m_evtPageEnd(null, null);
		}
	}
}
