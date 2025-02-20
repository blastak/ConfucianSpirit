﻿//------------------------------------------------------------------------------
// <copyright file="Player.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace MainProgram2
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Media;
	using System.Windows.Shapes;
	using Microsoft.Kinect;
	using System.Windows.Media.Imaging;

	public class MySkelPlayer
    {
        private const double BoneSize = 0.01;
        private const double HeadSize = 0.075;
        private const double HandSize = 0.03;

        // Keeping track of all bone segments of interest as well as head, hands and feet
        private readonly Dictionary<Bone, BoneData> segments = new Dictionary<Bone, BoneData>();
        private readonly System.Windows.Media.Brush jointsBrush;
        private readonly System.Windows.Media.Brush bonesBrush;
        private readonly int id;
        private static int colorId;
        private Rect playerBounds;
        private System.Windows.Point playerCenter;
        private double playerScale;

        public MySkelPlayer(int skeletonSlot)
        {
            this.id = skeletonSlot;

            // Generate one of 7 colors for player
            int[] mixR = { 1, 1, 1, 0, 1, 0, 0 };
            int[] mixG = { 1, 1, 0, 1, 0, 1, 0 };
            int[] mixB = { 1, 0, 1, 1, 0, 0, 1 };
            byte[] jointCols = { 245, 200 };
            byte[] boneCols = { 235, 160 };

            int i = colorId;
            colorId = (colorId + 1) % mixR.Count();

			//this.jointsBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(jointCols[mixR[i]], jointCols[mixG[i]], jointCols[mixB[i]]));
			//this.bonesBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(boneCols[mixR[i]], boneCols[mixG[i]], boneCols[mixB[i]]));
			this.jointsBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
			this.bonesBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
			this.LastUpdated = DateTime.Now;
        }

        public bool IsAlive { get; set; }

        public DateTime LastUpdated { get; set; }

        public Dictionary<Bone, BoneData> Segments
        {
            get
            {
                return this.segments;
            }
        }

        public int GetId()
        {
            return this.id;
        }

        public void SetBounds(Rect r)
        {
            this.playerBounds = r;
            this.playerCenter.X = (this.playerBounds.Left + this.playerBounds.Right) / 2;
            this.playerCenter.Y = (this.playerBounds.Top + this.playerBounds.Bottom) / 2;
            this.playerScale = Math.Min(this.playerBounds.Width, this.playerBounds.Height / 2);
		}

        public void UpdateBonePosition(Microsoft.Kinect.JointCollection joints, JointType j1, JointType j2)
        {
			var seg = new Segment(
				(joints[j1].Position.X * this.playerScale) + this.playerCenter.X,
				this.playerCenter.Y - (joints[j1].Position.Y * this.playerScale),
				(joints[j2].Position.X * this.playerScale) + this.playerCenter.X,
				this.playerCenter.Y - (joints[j2].Position.Y * this.playerScale))
			{ Radius = Math.Max(3.0, this.playerBounds.Height * BoneSize) / 2 };
            this.UpdateSegmentPosition(j1, j2, seg);
        }

        public void UpdateJointPosition(Microsoft.Kinect.JointCollection joints, JointType j)
        {
            var seg = new Segment(
                (joints[j].Position.X * this.playerScale) + this.playerCenter.X,
                this.playerCenter.Y - (joints[j].Position.Y * this.playerScale))
                { Radius = this.playerBounds.Height * ((j == JointType.Head) ? HeadSize : HandSize) / 2 };
            this.UpdateSegmentPosition(j, j, seg);
        }

		public void UpdateJointPosition2(Microsoft.Kinect.JointCollection joints, JointType j)
		{
			var seg = new Segment(
				(joints[j].Position.X * this.playerScale) + this.playerCenter.X,
				this.playerBounds.Bottom - (joints[j].Position.Y * this.playerScale*2))
			{ Radius = this.playerBounds.Height * ((j == JointType.Head) ? HeadSize : HandSize) / 2 };
			this.UpdateSegmentPosition(j, j, seg);
		}

		public void Draw(UIElementCollection children, int mode)
        {
            if (!this.IsAlive)
            {
                return;
            }

            // Draw all bones first, then circles (head and hands).
            DateTime cur = DateTime.Now;
			if(mode == 0) // 바구니
			{
				foreach (var segment in this.segments)
				{
					Segment seg = segment.Value.GetEstimatedSegment(cur);
					if(segment.Key.Joint1 == JointType.Head)
					{
						var rect = new Rectangle { Width = seg.Radius * 30, Height = seg.Radius * 30 };
						rect.SetValue(Canvas.LeftProperty, seg.X1 - seg.Radius * 15);
						rect.SetValue(Canvas.TopProperty, seg.Y1 - seg.Radius * 5);
						rect.Stretch = Stretch.Fill;
						var abrush = new ImageBrush(); //定义图片画刷
						string uri1 = @"pack://application:,,/" + "Images/" + "바구니.png";
						abrush.ImageSource = new BitmapImage(new Uri(uri1));
						rect.Fill = abrush;//填充
						children.Add(rect);
					}
				}
			}
			else
			{
				foreach (var segment in this.segments)
				{
					Segment seg = segment.Value.GetEstimatedSegment(cur);
					if (!seg.IsCircle())
					{
						var line = new Line
						{
							StrokeThickness = seg.Radius * 2,
							X1 = seg.X1,
							Y1 = seg.Y1,
							X2 = seg.X2,
							Y2 = seg.Y2,
							Stroke = this.bonesBrush,
							StrokeEndLineCap = PenLineCap.Round,
							StrokeStartLineCap = PenLineCap.Round
						};
						children.Add(line);
					}
					if (seg.IsCircle())
					{
						var circle = new Ellipse { Width = seg.Radius * 2, Height = seg.Radius * 2 };
						circle.SetValue(Canvas.LeftProperty, seg.X1 - seg.Radius);
						circle.SetValue(Canvas.TopProperty, seg.Y1 - seg.Radius);
						circle.Stroke = this.jointsBrush;
						circle.StrokeThickness = 1;
						circle.Fill = this.bonesBrush;
						children.Add(circle);

						if (mode == 2 && segment.Key.Joint1 == JointType.Head)
						{
							// 임시 테스트 (여기에 바구니만 와야함, 바구니 캐릭터 말고)
							var rect = new Rectangle { Width = seg.Radius * 20, Height = seg.Radius * 10 };
							rect.SetValue(Canvas.LeftProperty, seg.X1 - rect.Width / 2.0); //왼쪽으로 쉬프트
							rect.SetValue(Canvas.TopProperty, seg.Y1 - rect.Height); // 위로 쉬프트
							rect.Stretch = Stretch.Fill;
							var abrush = new ImageBrush(); //定义图片画刷
							string uri1 = @"pack://application:,,/" + "Images/" + "바구니만.png";
							abrush.ImageSource = new BitmapImage(new Uri(uri1));
							rect.Fill = abrush;//填充
							children.Add(rect);
						}
					}
				}
			}

            //foreach (var segment in this.segments)
            //{
            //    Segment seg = segment.Value.GetEstimatedSegment(cur);
            //    if (seg.IsCircle())
            //    {
            //        var circle = new Ellipse { Width = seg.Radius * 2, Height = seg.Radius * 2 };
            //        circle.SetValue(Canvas.LeftProperty, seg.X1 - seg.Radius);
            //        circle.SetValue(Canvas.TopProperty, seg.Y1 - seg.Radius);
            //        circle.Stroke = this.jointsBrush;
            //        circle.StrokeThickness = 1;
            //        circle.Fill = this.bonesBrush;
            //        children.Add(circle);
            //    }
            //}

            // Remove unused players after 1/2 second.
            if (DateTime.Now.Subtract(this.LastUpdated).TotalMilliseconds > 500)
            {
                this.IsAlive = false;
            }
        }

		public void Draw2(UIElementCollection children)
		{
			if (!this.IsAlive)
			{
				return;
			}

			// Draw all bones first, then circles (head and hands).
			DateTime cur = DateTime.Now;
			foreach (var segment in this.segments)
			{
				Segment seg = segment.Value.GetEstimatedSegment(cur);
				if (seg.IsCircle())
				{
					var rect = new Rectangle { Width = seg.Radius * 30, Height = seg.Radius * 30 };
					rect.SetValue(Canvas.LeftProperty, seg.X1 - seg.Radius * 15);
					rect.SetValue(Canvas.TopProperty, seg.Y1 - seg.Radius * 5);
					rect.Stretch = Stretch.Fill;
					var abrush = new ImageBrush(); //定义图片画刷
					string uri1 = @"pack://application:,,/" + "Images/" + "예효_05_02.png";
					abrush.ImageSource = new BitmapImage(new Uri(uri1));
					rect.Fill = abrush;//填充
					children.Add(rect);

// 					var circle = new Ellipse { Width = seg.Radius * 2, Height = seg.Radius * 2 };
// 					circle.SetValue(Canvas.LeftProperty, seg.X1 - seg.Radius);
// 					circle.SetValue(Canvas.TopProperty, seg.Y1 - seg.Radius);
// 					circle.Stroke = this.jointsBrush;
// 					circle.StrokeThickness = 1;
// 					circle.Fill = this.bonesBrush;
// 					children.Add(circle);
				}
			}

			// Remove unused players after 1/2 second.
			if (DateTime.Now.Subtract(this.LastUpdated).TotalMilliseconds > 500)
			{
				this.IsAlive = false;
			}
		}

		private void UpdateSegmentPosition(JointType j1, JointType j2, Segment seg)
        {
            var bone = new Bone(j1, j2);
            if (this.segments.ContainsKey(bone))
            {
                BoneData data = this.segments[bone];
                data.UpdateSegment(seg);
                this.segments[bone] = data;
            }
            else
            {
                this.segments.Add(bone, new BoneData(seg));
            }
        }
    }
}
