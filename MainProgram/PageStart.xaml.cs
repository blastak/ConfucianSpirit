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

		public PageStart()
		{
			InitializeComponent();
		}

		private void buttonStart1_Click(object sender, RoutedEventArgs e)
		{
			//MessageBox.Show("HI1");
			if (BtnClicked1 != null)
				this.BtnClicked1(this, e);
		}

		private void buttonStart2_Click(object sender, RoutedEventArgs e)
		{
			//MessageBox.Show("HI2");
			if (BtnClicked2 != null)
				this.BtnClicked2(this, e);
		}

		private void buttonStart3_Click(object sender, RoutedEventArgs e)
		{
			//MessageBox.Show("HI3");
			if (BtnClicked3 != null)
				this.BtnClicked3(this, e);
		}
	}
}
