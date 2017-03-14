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

namespace MainProgram
{
	/// <summary>
	/// DragImage.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class DragImage : UserControl
	{
		string m_strbase = @"pack://application:,,/";
		private Point originalPosition;
		private Point correctPosition;
		private double correctRadius = 80;

		public DragImage(string strImgName)
		{
			InitializeComponent();

			imgMain.Source = new BitmapImage(new Uri(m_strbase + "Images/" + strImgName));
			imgMain.StretchDirection = StretchDirection.Both;
		}

		public bool CheckPosition(Point _newPoisitionRT)
		{
			double dist = 0;
			dist = Math.Sqrt(Math.Pow((_newPoisitionRT.X - correctPosition.X), 2) + Math.Pow((_newPoisitionRT.Y - correctPosition.Y), 2));

			if (dist < correctRadius)
				return true;
			else
				return false;
		}

		public void GoToOriginalPosition()
		{
			Canvas.SetLeft(this, originalPosition.X);
			Canvas.SetTop(this, originalPosition.Y);
		}

		public void GoToCorrectPosition()
		{
			Canvas.SetLeft(this, correctPosition.X);
			Canvas.SetTop(this, correctPosition.Y);
		}

		public Point CorrectPosition
		{
			get
			{
				return correctPosition;
			}
			set
			{
				correctPosition = value;
			}
		}

		public Point OriginalPosition
		{
			get
			{
				return originalPosition;
			}
			set
			{
				originalPosition = value;
			}
		}

		private Size imgSize;
		public Size ImgSize
		{
			get
			{
				return imgSize;
			}
			set
			{
				imgSize = value;
				this.Width = imgSize.Width;
				this.Height = imgSize.Height;
			}
		}

		public double CorrectRadius
		{
			get
			{
				return correctRadius;
			}
			set
			{
				correctRadius = value;
			}
		}
	}
}
