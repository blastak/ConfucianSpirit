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
	/// Interaction logic for PageGameBow.xaml
	/// </summary>
	public partial class PageGameBow : Page
	{
		string m_strbase = @"pack://application:,,/";
		public event EventHandler m_evtPageFinish;
		public event EventHandler m_evtBindHand;
		public event EventHandler m_evtUnBindHand;
		public event EventHandler m_evtBindBGRemoval;
		public event EventHandler m_evtUnBindBGRemoval;
		public event EventHandler m_evtBindSkeletonImage;
		public event EventHandler m_evtUnBindSkeletonImage;

		public DispatcherTimer m_timerPageFinish = new DispatcherTimer();

		public bool m_bMaleOrNot;
		public bool m_bSkip;

		public PageGameBow()
		{
			InitializeComponent();
			m_timerPageFinish.Interval = TimeSpan.FromSeconds(1); // 시간 고쳐야함
			m_timerPageFinish.Tick += new EventHandler(TimerPageFinish);
		}


		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			m_imgTop.Visibility = Visibility.Hidden;
			m_btnMale.Visibility = Visibility.Hidden;
			m_btnFemale.Visibility = Visibility.Hidden;
			m_imgTop2.Visibility = Visibility.Hidden;
			m_imgMaleFemale.Visibility = Visibility.Hidden;
			m_imgTop3.Visibility = Visibility.Hidden;
			m_imgSelectedMF.Visibility = Visibility.Hidden;
			m_imgTop4.Visibility = Visibility.Hidden;
			m_imgUserBG.Visibility = Visibility.Hidden;
			m_imgUserSkel.Visibility = Visibility.Hidden;
			m_imgTop5.Visibility = Visibility.Hidden;
			m_imgMiddle1.Visibility = Visibility.Hidden;
			m_imgMiddle2.Visibility = Visibility.Hidden;

			m_imgTop.Visibility = Visibility.Visible;
			m_btnMale.Visibility = Visibility.Visible;
			m_btnFemale.Visibility = Visibility.Visible;

			m_evtBindHand(null, null);
		}

		private void m_btnMale_Click(object sender, RoutedEventArgs e)
		{
			m_bMaleOrNot = true;
			m_imgSelectedMF.Source = new BitmapImage(new Uri(m_strbase + "Images/" + "PageGameBow_04_선택_남자.png"));
			GameStart();
		}

		private void m_btnFemale_Click(object sender, RoutedEventArgs e)
		{
			m_bMaleOrNot = false;
			m_imgSelectedMF.Source = new BitmapImage(new Uri(m_strbase + "Images/" + "PageGameBow_04_선택_여자.png"));
			GameStart();
		}

		int idx;
		int cntTimer;
		private void GameStart()
		{
			m_evtUnBindHand(null, null);

			m_imgTop.Visibility = Visibility.Hidden;
			m_btnMale.Visibility = Visibility.Hidden;
			m_btnFemale.Visibility = Visibility.Hidden;
			m_imgTop2.Visibility = Visibility.Hidden;
			m_imgMaleFemale.Visibility = Visibility.Hidden;
			m_imgTop3.Visibility = Visibility.Hidden;
			m_imgSelectedMF.Visibility = Visibility.Hidden;
			m_imgTop4.Visibility = Visibility.Hidden;
			m_imgUserBG.Visibility = Visibility.Hidden;
			m_imgUserSkel.Visibility = Visibility.Hidden;
			m_imgTop5.Visibility = Visibility.Hidden;
			m_imgMiddle1.Visibility = Visibility.Hidden;
			m_imgMiddle2.Visibility = Visibility.Hidden;

			m_imgTop2.Visibility = Visibility.Visible;
			m_imgMaleFemale.Visibility = Visibility.Visible;

			m_bSkip = false;
			idx = 0;
			cntTimer = 0;
			m_timerPageFinish.Start();
		}

		private void TimerPageFinish(object sender, EventArgs e)
		{
			cntTimer++;

			if (idx == 0 && cntTimer > 3)
			{
				m_imgTop.Visibility = Visibility.Hidden;
				m_btnMale.Visibility = Visibility.Hidden;
				m_btnFemale.Visibility = Visibility.Hidden;
				m_imgTop2.Visibility = Visibility.Hidden;
				m_imgMaleFemale.Visibility = Visibility.Hidden;
				m_imgTop3.Visibility = Visibility.Hidden;
				m_imgSelectedMF.Visibility = Visibility.Hidden;
				m_imgTop4.Visibility = Visibility.Hidden;
				m_imgUserBG.Visibility = Visibility.Hidden;
				m_imgUserSkel.Visibility = Visibility.Hidden;
				m_imgTop5.Visibility = Visibility.Hidden;
				m_imgMiddle1.Visibility = Visibility.Hidden;
				m_imgMiddle2.Visibility = Visibility.Hidden;

				m_imgTop3.Visibility = Visibility.Visible;
				m_imgSelectedMF.Visibility = Visibility.Visible;

				idx++;
				cntTimer = 0;
			}
			else if (idx == 1 && cntTimer > 3)
			{
				m_imgTop.Visibility = Visibility.Hidden;
				m_btnMale.Visibility = Visibility.Hidden;
				m_btnFemale.Visibility = Visibility.Hidden;
				m_imgTop2.Visibility = Visibility.Hidden;
				m_imgMaleFemale.Visibility = Visibility.Hidden;
				m_imgTop3.Visibility = Visibility.Hidden;
				m_imgSelectedMF.Visibility = Visibility.Hidden;
				m_imgTop4.Visibility = Visibility.Hidden;
				m_imgUserBG.Visibility = Visibility.Hidden;
				m_imgUserSkel.Visibility = Visibility.Hidden;
				m_imgTop5.Visibility = Visibility.Hidden;
				m_imgMiddle1.Visibility = Visibility.Hidden;
				m_imgMiddle2.Visibility = Visibility.Hidden;

				m_imgTop4.Visibility = Visibility.Visible;
				m_imgUserBG.Visibility = Visibility.Visible;
				m_imgUserSkel.Visibility = Visibility.Visible;

				m_evtBindBGRemoval(m_imgUserBG, null);
				m_evtBindSkeletonImage(m_imgUserSkel, null);

				idx++;
				cntTimer = 0;
			}
			else if (idx == 2 && cntTimer > 10)
			{
				m_imgTop.Visibility = Visibility.Hidden;
				m_btnMale.Visibility = Visibility.Hidden;
				m_btnFemale.Visibility = Visibility.Hidden;
				m_imgTop2.Visibility = Visibility.Hidden;
				m_imgMaleFemale.Visibility = Visibility.Hidden;
				m_imgTop3.Visibility = Visibility.Hidden;
				m_imgSelectedMF.Visibility = Visibility.Hidden;
				m_imgTop4.Visibility = Visibility.Hidden;
				m_imgUserBG.Visibility = Visibility.Hidden;
				m_imgUserSkel.Visibility = Visibility.Hidden;
				m_imgTop5.Visibility = Visibility.Hidden;
				m_imgMiddle1.Visibility = Visibility.Hidden;
				m_imgMiddle2.Visibility = Visibility.Hidden;

				m_imgTop5.Visibility = Visibility.Visible;
				m_imgMiddle1.Visibility = Visibility.Visible;
				m_imgMiddle2.Visibility = Visibility.Visible;

				m_evtUnBindBGRemoval(null, null);
				m_evtUnBindSkeletonImage(null, null);

				idx++;
				cntTimer = 0;
			}
			else if ((idx == 3 && cntTimer > 5) || m_bSkip == true)
			{
				m_timerPageFinish.Stop();

				m_evtPageFinish(null, null);

				idx++;
				cntTimer = 0;
			}
		}
	}
}
