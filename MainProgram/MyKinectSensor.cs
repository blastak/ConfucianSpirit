using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using System;
using System.Windows.Media;
using System.Windows;

namespace HrkimKinectSensor
{
	public class MyKinectSensor
	{
		private const DepthImageFormat DepthFormat = DepthImageFormat.Resolution640x480Fps30;
		private const ColorImageFormat ColorFormat = ColorImageFormat.RgbResolution640x480Fps30;

		public KinectSensorChooser sensorChooser;
		public Skeleton[] skeletons;
		public int currentlyTrackedSkeletonId;

		public event EventHandler<AllFramesReadyEventArgs> ReadSkeleton;

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

		public void DisposeBackgroundRemoveService()
		{
// 			if (null != this.backgroundRemovedColorStream)
// 			{
// 				this.backgroundRemovedColorStream.Dispose();
// 				this.backgroundRemovedColorStream = null;
// 			}
		}

		public void Closing()
		{
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
					//args.OldSensor.ColorStream.Disable();
					args.OldSensor.SkeletonStream.Disable();
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
					args.NewSensor.DepthStream.Enable(DepthFormat);
					//args.NewSensor.ColorStream.Enable(ColorFormat);
					args.NewSensor.SkeletonStream.Enable();

					// Allocate space to put the depth, color, and skeleton data we'll receive
					if (null == this.skeletons)
					{
						this.skeletons = new Skeleton[args.NewSensor.SkeletonStream.FrameSkeletonArrayLength];
					}

					// Add an event handler to be called whenever there is new depth frame data
					args.NewSensor.AllFramesReady += this.SensorAllFramesReady;
				}
				catch (InvalidOperationException)
				{
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
					}
				}

				if (ReadSkeleton != null)
					ReadSkeleton(sender, e);

				//this.ChooseSkeleton();
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

			foreach (var skel in this.skeletons)
			{
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
				this.currentlyTrackedSkeletonId = nearestSkeleton;
			}
		}
	}
}
