using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MainProgram
{
	public class MyGameGravityTouch
	{
		public event EventHandler m_evtGameManager;

		private Canvas m_canvas;
		private Canvas m_canvas2;
		private Image m_imgUserBody;
		private Image m_imgTFFace;

		public MyKinectSensor m_myKinect;
		private FallingThings myFallingThings;

		private const int NumIntraFrames = 3;
		private const int MaxShapes = 2;
        private const int TimerResolution = 2;  // ms
		private const double MinFramerate = 15;
		private const double MaxFramerate = 70;
		//private const double DefaultDropRate = 2.5;
		private const double DefaultDropRate = 1;
		//private const double DefaultDropSize = 32.0;
		private const double DefaultDropSize = 200.0;
		private const double DefaultDropGravity = 1.0;
		private double targetFramerate = MaxFramerate;
		private double dropRate = DefaultDropRate;
		private double dropSize = DefaultDropSize;
		private double dropGravity = DefaultDropGravity;
		private Rect playerBounds;
		private readonly Dictionary<int, Player> players = new Dictionary<int, Player>();

		public bool runningGameThread;
		private DateTime predNextFrame = DateTime.MinValue;
		private double actualFrameTime;
		private DateTime lastFrameDrawn = DateTime.MinValue;
		private int frameCount;
		private int playersAlive;

		string m_strbase = @"pack://application:,,/";
		private string m_strBackground;
		private string m_strQuestionSound;
		private DispatcherTimer m_timerCountdown = new DispatcherTimer();
		private MediaPlayer m_startSound = new MediaPlayer();

		private int m_timeRemain;

		public MyGameGravityTouch()
		{
			this.myFallingThings = new FallingThings(MaxShapes, this.targetFramerate, NumIntraFrames);

			this.myFallingThings.SetGravity(this.dropGravity);
			this.myFallingThings.SetDropRate(this.dropRate);
			this.myFallingThings.SetSize(this.dropSize);
			this.myFallingThings.SetPolies(PolyType.All);
			this.myFallingThings.SetGameMode(GameMode.Off);

			string[] a = { "경성_03_02.png", "경성_03_03.png", "경성_03_04.png", "경성_03_05.png", "경성_03_06.png", "경성_03_07.png" };
			this.myFallingThings.AddStrImage(a);

			m_timerCountdown.Tick += new EventHandler(TimerCountdown);

		}

		public void SetupUI(Canvas canvas, Canvas canvas2, Image userBody, Image tfFace)
		{
			m_canvas = canvas;
			m_canvas2 = canvas2;
			m_imgUserBody = userBody;
			m_imgTFFace = tfFace;
		}

		public void SetupResource(string background, string questionSound)
		{
			m_strBackground = background;
			m_strQuestionSound = questionSound;
		}

		public void Entrypoint()
		{
			m_imgUserBody.Visibility = Visibility.Hidden;
			m_imgTFFace.Visibility = Visibility.Hidden;


			m_canvas.ClipToBounds = true;
			m_canvas2.ClipToBounds = true;

			this.UpdatePlayfieldSize();

			if (m_myKinect.sensorChooser != null)
			{
				m_myKinect.evtReadySingleSkel += new EventHandler<AllFramesReadyEventArgs>(EventCheckHandOver);
			}

			m_timeRemain = 120;

			// 1. 배경 보여주기
			m_canvas.Background = new ImageBrush(new BitmapImage(new Uri(m_strbase + "Images/" + m_strBackground)));

			// 2. 사운드 재생
			m_startSound.Open(new Uri("Sounds/" + m_strQuestionSound, UriKind.Relative)); // 속성:빌드시자동복사
			m_startSound.MediaEnded += new EventHandler(MediaEnd1);
			m_startSound.Volume = 1;
			//m_startSound.Position = TimeSpan.FromSeconds(45);
			m_startSound.Play();
		}

		private void MediaEnd1(object sender, EventArgs e)
		{
			m_startSound.MediaEnded -= new EventHandler(MediaEnd1);
			m_startSound.Stop();
			m_startSound.Close();

			// 7. 제한시간 시작
			m_timerCountdown.Interval = TimeSpan.FromMilliseconds(1000);
			m_timerCountdown.Start();

			//m_imgUserBody.Visibility = Visibility.Visible;

			var myGameThread = new Thread(this.GameThread);
			myGameThread.SetApartmentState(ApartmentState.STA);
			myGameThread.Start();
		}

		private void TimerCountdown(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

			m_timeRemain -= 1;
			if (m_timeRemain <= 0)
			{
				m_timerCountdown.Stop();
				m_canvas2.Visibility = Visibility.Hidden;
				ResultGame(true);
			}
		}

		private void ResultGame(bool success)
		{
			int success2 = 0;
			if (success)
				success2 = 0;
			else
				success2 = 1;

			string[] strImgName = { "Images/중간평가1.png", "Images/중간평가2.png" };
			string[] strSoundName = { "Sounds/중간평가_1(성공).m4a", "Sounds/중간평가_2(다음기회에).m4a" };

			BitmapImage src;
			src = new BitmapImage(new Uri(m_strbase + strImgName[success2]));
			m_imgTFFace.Source = src;
			m_imgTFFace.Visibility = Visibility.Visible;

			m_startSound.Open(new Uri(strSoundName[success2], UriKind.Relative)); // 속성:빌드시자동복사
			m_startSound.MediaEnded += new EventHandler(MediaEnd2);
			m_startSound.Volume = 1;
			m_startSound.Play();
		}

		private void MediaEnd2(object sender, EventArgs e)
		{
			m_startSound.MediaEnded -= new EventHandler(MediaEnd2);
			m_startSound.Stop();
			m_startSound.Close();

			if (m_myKinect.sensorChooser != null)
			{
				m_myKinect.evtReadySingleSkel -= new EventHandler<AllFramesReadyEventArgs>(EventCheckHandOver);
			}

            this.myFallingThings.SetGameMode(GameMode.Off);
			this.runningGameThread = false;
			m_canvas.Children.Clear();
			m_canvas2.Children.Clear();

			m_evtGameManager(0, null);

			m_imgUserBody.Visibility = Visibility.Hidden;
			m_imgTFFace.Visibility = Visibility.Hidden;
		}

		private void EventCheckHandOver(object sender, AllFramesReadyEventArgs e)
		{
			Skeleton skel = (Skeleton)sender;
			int skeletonSlot = 0;

			Player player;
			if (this.players.ContainsKey(skeletonSlot))
			{
				player = this.players[skeletonSlot];
			}
			else
			{
				player = new Player(skeletonSlot);
				player.SetBounds(this.playerBounds);
				this.players.Add(skeletonSlot, player);
			}

			player.LastUpdated = DateTime.Now;

			// Update player's bone and joint positions
			if (skel.Joints.Count > 0)
			{
				player.IsAlive = true;

				// Head, hands, feet (hit testing happens in order here)
				player.UpdateJointPosition(skel.Joints, JointType.Head);
				player.UpdateJointPosition(skel.Joints, JointType.HandLeft);
				player.UpdateJointPosition(skel.Joints, JointType.HandRight);
				player.UpdateJointPosition(skel.Joints, JointType.FootLeft);
				player.UpdateJointPosition(skel.Joints, JointType.FootRight);

				// Hands and arms
				player.UpdateBonePosition(skel.Joints, JointType.HandRight, JointType.WristRight);
				player.UpdateBonePosition(skel.Joints, JointType.WristRight, JointType.ElbowRight);
				player.UpdateBonePosition(skel.Joints, JointType.ElbowRight, JointType.ShoulderRight);

				player.UpdateBonePosition(skel.Joints, JointType.HandLeft, JointType.WristLeft);
				player.UpdateBonePosition(skel.Joints, JointType.WristLeft, JointType.ElbowLeft);
				player.UpdateBonePosition(skel.Joints, JointType.ElbowLeft, JointType.ShoulderLeft);

				// Head and Shoulders
				player.UpdateBonePosition(skel.Joints, JointType.ShoulderCenter, JointType.Head);
				player.UpdateBonePosition(skel.Joints, JointType.ShoulderLeft, JointType.ShoulderCenter);
				player.UpdateBonePosition(skel.Joints, JointType.ShoulderCenter, JointType.ShoulderRight);

				// Legs
				player.UpdateBonePosition(skel.Joints, JointType.HipLeft, JointType.KneeLeft);
				player.UpdateBonePosition(skel.Joints, JointType.KneeLeft, JointType.AnkleLeft);
				player.UpdateBonePosition(skel.Joints, JointType.AnkleLeft, JointType.FootLeft);

				player.UpdateBonePosition(skel.Joints, JointType.HipRight, JointType.KneeRight);
				player.UpdateBonePosition(skel.Joints, JointType.KneeRight, JointType.AnkleRight);
				player.UpdateBonePosition(skel.Joints, JointType.AnkleRight, JointType.FootRight);

				player.UpdateBonePosition(skel.Joints, JointType.HipLeft, JointType.HipCenter);
				player.UpdateBonePosition(skel.Joints, JointType.HipCenter, JointType.HipRight);

				// Spine
				player.UpdateBonePosition(skel.Joints, JointType.HipCenter, JointType.ShoulderCenter);
			}
		}

		private void UpdatePlayfieldSize()
		{
			//// Size of player wrt size of playfield, putting ourselves low on the screen.
			//this.screenRect.X = 0;
			//this.screenRect.Y = 0;
			//this.screenRect.Width = this.playfield.ActualWidth;
			//this.screenRect.Height = this.playfield.ActualHeight;

			//BannerText.UpdateBounds(this.screenRect);

			this.playerBounds.X = 0;
			this.playerBounds.Width = this.m_canvas2.ActualWidth;
			this.playerBounds.Y = this.m_canvas2.ActualHeight * 0.2;
			this.playerBounds.Height = this.m_canvas2.ActualHeight * 0.75;

			foreach (var player in this.players)
			{
				player.Value.SetBounds(this.playerBounds);
			}

			Rect fallingBounds = this.playerBounds;
			fallingBounds.Y = 0;
			fallingBounds.Height = m_canvas2.ActualHeight;
			if (this.myFallingThings != null)
			{
				this.myFallingThings.SetBoundaries(fallingBounds);
			}
		}

		private void GameThread()
		{
			this.runningGameThread = true;
			this.predNextFrame = DateTime.Now;
			this.actualFrameTime = 1000.0 / this.targetFramerate;

			// Try to dispatch at as constant of a framerate as possible by sleeping just enough since
			// the last time we dispatched.
			while (this.runningGameThread)
			{
				// Calculate average framerate.  
				DateTime now = DateTime.Now;
				if (this.lastFrameDrawn == DateTime.MinValue)
				{
					this.lastFrameDrawn = now;
				}

				double ms = now.Subtract(this.lastFrameDrawn).TotalMilliseconds;
				this.actualFrameTime = (this.actualFrameTime * 0.95) + (0.05 * ms);
				this.lastFrameDrawn = now;

				// Adjust target framerate down if we're not achieving that rate
				this.frameCount++;
				if ((this.frameCount % 100 == 0) && (1000.0 / this.actualFrameTime < this.targetFramerate * 0.92))
				{
					this.targetFramerate = Math.Max(MinFramerate, (this.targetFramerate + (1000.0 / this.actualFrameTime)) / 2);
				}

				if (now > this.predNextFrame)
				{
					this.predNextFrame = now;
				}
				else
				{
					double milliseconds = this.predNextFrame.Subtract(now).TotalMilliseconds;
					if (milliseconds >= TimerResolution)
					{
						Thread.Sleep((int)(milliseconds + 0.5));
					}
				}

				this.predNextFrame += TimeSpan.FromMilliseconds(1000.0 / this.targetFramerate);

				m_canvas2.Dispatcher.Invoke(DispatcherPriority.Send, new Action<int>(this.HandleGameTimer), 0);
			}
		}

		private void HandleGameTimer(int param)
		{
			// Every so often, notify what our actual framerate is
			if ((this.frameCount % 100) == 0)
			{
				this.myFallingThings.SetFramerate(1000.0 / this.actualFrameTime);
			}

			// Advance animations, and do hit testing.
			for (int i = 0; i < NumIntraFrames; ++i)
			{
				foreach (var pair in this.players)
				{
					HitType hit = this.myFallingThings.LookForHits(pair.Value.Segments, pair.Value.GetId());
					if ((hit & HitType.Squeezed) != 0)
					{
						//this.squeezeSound.Play();
					}
					else if ((hit & HitType.Popped) != 0)
					{
						//this.popSound.Play();
					}
					else if ((hit & HitType.Hand) != 0)
					{
						//this.hitSound.Play();
					}
				}

				this.myFallingThings.AdvanceFrame();
			}

			// Draw new Wpf scene by adding all objects to canvas
			m_canvas2.Children.Clear();
			this.myFallingThings.DrawFrame(this.m_canvas2.Children);
			m_canvas.Children.Clear();
			this.myFallingThings.DrawFrameScore(m_canvas.Children, m_canvas.ActualWidth, m_canvas.ActualHeight);

			foreach (var player in this.players)
			{
				player.Value.Draw(m_canvas2.Children);
			}

// 			BannerText.Draw(m_canvas.Children);
// 			FlyingText.Draw(m_canvas.Children);

			this.CheckPlayers();

			if (runningGameThread == false)
			{
				m_canvas.Children.Clear();
				m_canvas2.Children.Clear();
			}
		}

		private void CheckPlayers()
		{
			foreach (var player in this.players)
			{
				if (!player.Value.IsAlive)
				{
					// Player left scene since we aren't tracking it anymore, so remove from dictionary
					this.players.Remove(player.Value.GetId());
					break;
				}
			}

			// Count alive players
			int alive = this.players.Count(player => player.Value.IsAlive);

			if (alive != this.playersAlive)
			{
				if (alive == 2)
				{
					this.myFallingThings.SetGameMode(GameMode.TwoPlayer);
				}
				else if (alive == 1)
				{
					this.myFallingThings.SetGameMode(GameMode.Solo);
				}
				else if (alive == 0)
				{
					this.myFallingThings.SetGameMode(GameMode.Off);
				}

// 				if ((this.playersAlive == 0) && (this.mySpeechRecognizer != null))
// 				{
// 					BannerText.NewBanner(
// 						Properties.Resources.Vocabulary,
// 						this.screenRect,
// 						true,
// 						System.Windows.Media.Color.FromArgb(200, 255, 255, 255));
// 				}

				this.playersAlive = alive;
			}
		}
	}
}
