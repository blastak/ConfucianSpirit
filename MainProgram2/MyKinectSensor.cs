using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.BackgroundRemoval;
using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace MainProgram2
{
	public class MyKinectSensor
	{
		private const DepthImageFormat DepthFormat = DepthImageFormat.Resolution640x480Fps30;
		private const ColorImageFormat ColorFormat = ColorImageFormat.RgbResolution640x480Fps30;

		public KinectSensorChooser sensorChooser;
		public Skeleton[] skeletons;
		public int currentlyTrackedSkeletonId;

		public event EventHandler<AllFramesReadyEventArgs> evtReadySingleSkel;

		private WriteableBitmap foregroundBitmap;
		private BackgroundRemovedColorStream backgroundRemovedColorStream;

		private Image imgBackgroundRemoval = null;

		private DrawingGroup m_drawingGroup = null;
		private DrawingImage m_imageSource = null;

		private int m_idPlayer = -1;

		public MyKinectSensor()
		{
			this.sensorChooser = new KinectSensorChooser();
			this.sensorChooser.KinectChanged += this.SensorChooserOnKinectChanged;
			this.sensorChooser.Start();
		}

		public MyKinectSensor(KinectSensorChooserUI ui)
		{
			this.sensorChooser = new KinectSensorChooser();
			this.sensorChooser.KinectChanged += this.SensorChooserOnKinectChanged;
			ui.KinectSensorChooser = this.sensorChooser;
			this.sensorChooser.Start();
		}

		public void BindBackgroundRemovalImage(Image input_image)
		{
			imgBackgroundRemoval = input_image;
		}

		public void UnbindBackgroundRemovalImage()
		{
			imgBackgroundRemoval = null;
		}
		
		public void Closing()
		{
			if (null != this.backgroundRemovedColorStream)
			{
				this.backgroundRemovedColorStream.Dispose();
				this.backgroundRemovedColorStream = null;
			}

			if (this.sensorChooser != null)
			{
				this.sensorChooser.Stop();
				this.sensorChooser = null;
			}
		}

		private void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs args)
		{
			if (args.OldSensor != null)
			{
				try
				{
					args.OldSensor.AllFramesReady -= this.SensorAllFramesReady;
					args.OldSensor.DepthStream.Disable();
					args.OldSensor.ColorStream.Disable();
					args.OldSensor.SkeletonStream.Disable();

					// Create the background removal stream to process the data and remove background, and initialize it.
					if (null != this.backgroundRemovedColorStream)
					{
						this.backgroundRemovedColorStream.BackgroundRemovedFrameReady -= this.BackgroundRemovedFrameReadyHandler;
						this.backgroundRemovedColorStream.Dispose();
						this.backgroundRemovedColorStream = null;
					}
				}
				catch (InvalidOperationException)
				{
					// KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
					// E.g.: sensor might be abruptly unplugged.
				}
			}

			if (args.NewSensor != null)
			{
				try
				{
					// S.D.C. 배치가 가장 로딩시간이 적음
					args.NewSensor.SkeletonStream.Enable();
					args.NewSensor.DepthStream.Enable(DepthFormat);
					args.NewSensor.ColorStream.Enable(ColorFormat);

					this.backgroundRemovedColorStream = new BackgroundRemovedColorStream(args.NewSensor);
					this.backgroundRemovedColorStream.Enable(ColorFormat, DepthFormat);

					// Allocate space to put the depth, color, and skeleton data we'll receive
					if (null == this.skeletons)
					{
						this.skeletons = new Skeleton[args.NewSensor.SkeletonStream.FrameSkeletonArrayLength];
					}

					// Add an event handler to be called when the background removed color frame is ready, so that we can
					// composite the image and output to the app
					this.backgroundRemovedColorStream.BackgroundRemovedFrameReady += this.BackgroundRemovedFrameReadyHandler;

					// Add an event handler to be called whenever there is new depth frame data
					args.NewSensor.AllFramesReady += this.SensorAllFramesReady;
				}
				catch (InvalidOperationException e)
				{
					MessageBox.Show(e.Message);
					// KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
					// E.g.: sensor might be abruptly unplugged.
				}
			}
		}

		private void SensorAllFramesReady(object sender, AllFramesReadyEventArgs e)
		{
			// in the middle of shutting down, or lingering events from previous sensor, do nothing here.
			if (null == this.sensorChooser || null == this.sensorChooser.Kinect || this.sensorChooser.Kinect != sender)
			{
				return;
			}

			try
			{
				using (var skeletonFrame = e.OpenSkeletonFrame())
				{
					if (null != skeletonFrame)
					{
						skeletonFrame.CopySkeletonDataTo(this.skeletons);
						this.backgroundRemovedColorStream.ProcessSkeleton(this.skeletons, skeletonFrame.Timestamp);
					}
				}

				using (var depthFrame = e.OpenDepthImageFrame())
				{
					if (null != depthFrame)
					{
						this.backgroundRemovedColorStream.ProcessDepth(depthFrame.GetRawPixelData(), depthFrame.Timestamp);
					}
				}

				using (var colorFrame = e.OpenColorImageFrame())
				{
					if (null != colorFrame)
					{
						this.backgroundRemovedColorStream.ProcessColor(colorFrame.GetRawPixelData(), colorFrame.Timestamp);
					}
				}

				this.ChooseSkeleton();

				if (evtReadySingleSkel != null && m_idPlayer != -1)
				{
					object hi;
					hi = skeletons[m_idPlayer];
					evtReadySingleSkel(hi, e);
				}
			}
			catch (InvalidOperationException)
			{
				// Ignore the exception. 
			}
		}

		private void ChooseSkeleton()
		{
			var isTrackedSkeltonVisible = false;
			var nearestDistance = float.MaxValue;
			var nearestSkeleton = 0;

			int idx = -1;

			foreach (var skel in this.skeletons)
			{
				idx += 1;

				if (null == skel)
				{
					continue;
				}

				if (skel.TrackingState != SkeletonTrackingState.Tracked)
				{
					continue;
				}

				if (skel.TrackingId == this.currentlyTrackedSkeletonId)
				{
					isTrackedSkeltonVisible = true;
					break;
				}

				if (skel.Position.Z < nearestDistance)
				{
					nearestDistance = skel.Position.Z;
					nearestSkeleton = skel.TrackingId;
				}
			}

			if (!isTrackedSkeltonVisible && nearestSkeleton != 0)
			{
				this.backgroundRemovedColorStream.SetTrackedPlayer(nearestSkeleton);
				this.currentlyTrackedSkeletonId = nearestSkeleton;
			}

			// hr 추가
			if (isTrackedSkeltonVisible)
				m_idPlayer = idx;
			else
				m_idPlayer = -1;
		}

		private void BackgroundRemovedFrameReadyHandler(object sender, BackgroundRemovedColorFrameReadyEventArgs e)
		{
			using (var backgroundRemovedFrame = e.OpenBackgroundRemovedColorFrame())
			{
				if (backgroundRemovedFrame != null)
				{
					if (null == this.foregroundBitmap || this.foregroundBitmap.PixelWidth != backgroundRemovedFrame.Width
						|| this.foregroundBitmap.PixelHeight != backgroundRemovedFrame.Height)
					{
						this.foregroundBitmap = new WriteableBitmap(backgroundRemovedFrame.Width, backgroundRemovedFrame.Height, 96.0, 96.0, PixelFormats.Bgra32, null);
					}

					// Set the image we display to point to the bitmap where we'll put the image data
					if (imgBackgroundRemoval != null)
					{
						imgBackgroundRemoval.Source = this.foregroundBitmap;
					}

					// Write the pixel data into our bitmap
					this.foregroundBitmap.WritePixels(
						new Int32Rect(0, 0, this.foregroundBitmap.PixelWidth, this.foregroundBitmap.PixelHeight),
						backgroundRemovedFrame.GetRawPixelData(),
						this.foregroundBitmap.PixelWidth * sizeof(int),
						0);
				}
			}
		}

		private void MySkeletonFrameReady(object sender, AllFramesReadyEventArgs e)
		{
			using (DrawingContext dc = this.m_drawingGroup.Open())
			{
				// Draw a transparent background to set the render size
				//dc.DrawRectangle(null, new Pen(Brushes.Blue,10), new Rect(0.0, 0.0, 640, 480));
				//dc.DrawRectangle(null, null, new Rect(0.0, 0.0, 640, 480));
				dc.DrawRectangle(null, new Pen(Brushes.Blue, 1), new Rect(0.0, 0.0, 640, 480));

				if (skeletons.Length != 0)
				{
					Single distMin = Single.MaxValue;
					Skeleton player = new Skeleton();

					foreach (Skeleton skel in skeletons)
					{
						// 화면밖으로 벗어났을 때 빨간선
						//RenderClippedEdges(skel, dc);

						if (null == skel)
						{
							continue;
						}

						if (skel.TrackingState != SkeletonTrackingState.Tracked)
						{
							continue;
						}

						Single dist = skel.Position.Z;
						if (dist < distMin && dist >= 0.5)
						{
							player = skel;
							distMin = dist;
						}
					}

					if (player.TrackingState == SkeletonTrackingState.Tracked)
					{
						this.DrawBonesAndJoints(player, dc);
					}
					else if (player.TrackingState == SkeletonTrackingState.PositionOnly)
					{
						dc.DrawEllipse(
						Brushes.Blue,
						null,
						this.SkeletonPointToScreen(player.Position),
						10, 10);
					}
				}

				// prevent drawing outside of our render area
				this.m_drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, 640, 480));
			}
		}


		public Point SkeletonPointToScreen(SkeletonPoint skelpoint)
		{
			// Convert point to depth space.  
			// We are not using depth directly, but we do want the points in our 640x480 output resolution.
			DepthImagePoint depthPoint = sensorChooser.Kinect.CoordinateMapper.MapSkeletonPointToDepthPoint(
				skelpoint, sensorChooser.Kinect.DepthStream.Format);
			return new Point(depthPoint.X, depthPoint.Y);
		}

		private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext)
		{
			// Render Torso
			this.DrawBone(skeleton, drawingContext, JointType.Head, JointType.ShoulderCenter);
			this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft);
			this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight);
			this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine);
			this.DrawBone(skeleton, drawingContext, JointType.Spine, JointType.HipCenter);
			this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft);
			this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipRight);

			// Left Arm
			this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft);
			this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft);
			this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft);

			// Right Arm
			this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight);
			this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight);
			this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight);

			// Left Leg
			this.DrawBone(skeleton, drawingContext, JointType.HipLeft, JointType.KneeLeft);
			this.DrawBone(skeleton, drawingContext, JointType.KneeLeft, JointType.AnkleLeft);
			this.DrawBone(skeleton, drawingContext, JointType.AnkleLeft, JointType.FootLeft);

			// Right Leg
			this.DrawBone(skeleton, drawingContext, JointType.HipRight, JointType.KneeRight);
			this.DrawBone(skeleton, drawingContext, JointType.KneeRight, JointType.AnkleRight);
			this.DrawBone(skeleton, drawingContext, JointType.AnkleRight, JointType.FootRight);

			// Render Joints
			foreach (Joint joint in skeleton.Joints)
			{
				Brush drawBrush = null;

				if (joint.TrackingState == JointTrackingState.Tracked)
				{
					drawBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));
				}
				else if (joint.TrackingState == JointTrackingState.Inferred)
				{
					drawBrush = Brushes.Yellow;
				}

				if (drawBrush != null)
				{
					if (joint.JointType == JointType.Head)
						drawingContext.DrawEllipse(Brushes.WhiteSmoke, new Pen(drawBrush, 3), this.SkeletonPointToScreen(joint.Position), 35, 45);
					else
						drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(joint.Position), 3, 3);
				}
			}
		}

		private void DrawBone(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1)
		{
			Joint joint0 = skeleton.Joints[jointType0];
			Joint joint1 = skeleton.Joints[jointType1];

			// If we can't find either of these joints, exit
			if (joint0.TrackingState == JointTrackingState.NotTracked ||
				joint1.TrackingState == JointTrackingState.NotTracked)
			{
				return;
			}

			// Don't draw if both points are inferred
			if (joint0.TrackingState == JointTrackingState.Inferred &&
				joint1.TrackingState == JointTrackingState.Inferred)
			{
				return;
			}

			// We assume all drawn bones are inferred unless BOTH joints are tracked
			Pen drawPen;
			if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
			{
				drawPen = new Pen(Brushes.Green, 6); // bone color
			}
			else
			{
				drawPen = new Pen(Brushes.Gray, 1); // bone color (non-tracked)
			}

			drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
		}
	}
}
