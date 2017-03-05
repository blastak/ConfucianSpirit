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

using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;

namespace MainProgram
{
	/// <summary>
	/// PageStart.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class PageStart : Page
	{
		public event EventHandler BtnClicked1;
		public event EventHandler BtnClicked2;
		public event EventHandler BtnClicked3;

		private MediaPlayer m_soundBackground = new MediaPlayer();
		private MediaPlayer m_soundNarration = new MediaPlayer();

		public PageStart()
		{
			InitializeComponent();

			this.Loaded += new RoutedEventHandler(PageLoaded);

			m_soundBackground.Open(new Uri("Sounds/" + "게임시작배경음악.mp3", UriKind.Relative)); // 속성:빌드시자동복사
			m_soundBackground.Volume = 0.1;

			m_soundNarration.Open(new Uri("Sounds/" + "게임시작.m4a", UriKind.Relative)); // 속성:빌드시자동복사
			m_soundNarration.Volume = 1;
		}

		private void PageLoaded(object sender, RoutedEventArgs e)
		{
			m_soundBackground.Play();
			m_soundNarration.Play();
		}

		private void buttonStart1_Click(object sender, RoutedEventArgs e)
		{
			m_soundBackground.Stop();
			m_soundNarration.Stop();

			if (BtnClicked1 != null)
				this.BtnClicked1(this, e);
		}

		private void buttonStart2_Click(object sender, RoutedEventArgs e)
		{
			m_soundBackground.Stop();
			m_soundNarration.Stop();

			if (BtnClicked2 != null)
				this.BtnClicked2(this, e);
		}

		private void buttonStart3_Click(object sender, RoutedEventArgs e)
		{
			m_soundBackground.Stop();
			m_soundNarration.Stop();

			if (BtnClicked3 != null)
				this.BtnClicked3(this, e);
		}
	}
}
