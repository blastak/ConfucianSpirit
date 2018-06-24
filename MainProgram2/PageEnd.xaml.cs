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
	/// Interaction logic for PageEnd.xaml
	/// </summary>
	public partial class PageEnd : Page
	{
		string m_strbase = @"pack://application:,,/";
		public event EventHandler m_evtPageFinish;
		public DispatcherTimer m_timerPageFinish = new DispatcherTimer();
		private MediaPlayer m_startSound = new MediaPlayer();

		public int m_seconds;
		public int m_scores;

		public PageEnd()
		{
			InitializeComponent();
			m_timerPageFinish.Interval = TimeSpan.FromSeconds(12.5); // 시간 고쳐야함
			m_timerPageFinish.Tick += new EventHandler(TimerPageFinish);

			m_startSound.Open(new Uri("Media/" + "PageEnd_배경음악.mp3", UriKind.Relative)); // 속성:빌드시자동복사
			m_startSound.Volume = 1;
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			textBlock_Score.Text = String.Format("{0}", m_scores);
			textBlock_Time.Text = String.Format("{0:mm\\:ss}", TimeSpan.FromSeconds(m_seconds));

			m_startSound.Play();

			m_timerPageFinish.Start();
		}

		private void TimerPageFinish(object sender, EventArgs e)
		{
			m_startSound.Stop();

			m_timerPageFinish.Stop();

			m_evtPageFinish(null, null);
		}
	}
}
