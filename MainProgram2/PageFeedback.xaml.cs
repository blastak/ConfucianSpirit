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

		public bool m_bGoodOrBad;

		public PageFeedback()
		{
			InitializeComponent();
		}
		
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			if(m_bGoodOrBad == true)
			{
				m_imgGoodOrBad.Source = new BitmapImage(new Uri(m_strbase + "Images/" + "PageFeedback_01_잘했을때.png"));
			}
			else
			{
				m_imgGoodOrBad.Source = new BitmapImage(new Uri(m_strbase + "Images/" + "PageFeedback_02_못했을때.png"));
			}

			m_imgGoodOrBad.Visibility = Visibility.Visible;

			m_evtBindHand(null, null);
		}

		private void m_btnNext_Click(object sender, RoutedEventArgs e)
		{
			m_evtUnBindHand(null, null);

			m_evtPageFinish(null, null);
		}
	}
}
