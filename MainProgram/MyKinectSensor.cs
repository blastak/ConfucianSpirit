using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.BackgroundRemoval;
using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

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

		private WriteableBitmap foregroundBitmap;
		private BackgroundRemovedColorStream backgroundRemovedColorStream;

		Page page = null;
		Image imgMaskInput = null;

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

		public void BindPage(Page input_page, Image input_image)
		{
			page = input_page;
			imgMaskInput = input_image;
		}

		public void UnbindPage()
		{
			page = null;
			imgMaskInput = null;
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

				if (ReadSkeleton != null)
					ReadSkeleton(sender, e);

				this.ChooseSkeleton();
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
				this.backgroundRemovedColorStream.SetTrackedPlayer(nearestSkeleton);
				this.currentlyTrackedSkeletonId = nearestSkeleton;
			}
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
					if (imgMaskInput != null)
					{
						imgMaskInput.Source = this.foregroundBitmap;
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
	}
}
