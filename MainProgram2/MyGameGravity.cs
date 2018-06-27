using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Kinect;
using System.Threading;
using System.Windows.Threading;

namespace MainProgram2
{
	public class MyGameGravity
	{
		private MyFallingThings myFallingThings;
		private const int MaxShapes = 3; // 동시에 떨어지는 개수
		private const double MinFramerate = 15;
		private const double MaxFramerate = 70;
		private const int NumIntraFrames = 3;
		private const int TimerResolution = 2;  // ms
		private const double DefaultDropGravity = 1.0;
		private const double DefaultDropRate = 1;
		private const double DefaultDropSize = 200.0;

		private double targetFramerate = MaxFramerate;
		private double dropGravity = DefaultDropGravity;
		private double dropRate = DefaultDropRate;
		private double dropSize = DefaultDropSize;

		public bool m_runningGameThread;
		private DateTime m_predNextFrame = DateTime.MinValue;
		private double m_actualFrameTime;
		private DateTime m_lastFrameDrawn = DateTime.MinValue;
		private int m_frameCount;

		public MyKinectSensor m_myKinect = null;

		public Canvas m_canvasSkel = null;
		private Rect playerBounds;
		private readonly Dictionary<int, MySkelPlayer> players = new Dictionary<int, MySkelPlayer>();

		public int m_mode = -1;

		public MyGameGravity()
		{
			myFallingThings = new MyFallingThings(MaxShapes, targetFramerate, NumIntraFrames);
			myFallingThings.SetGravity(dropGravity);
			myFallingThings.SetDropRate(dropRate);
			myFallingThings.SetSize(dropSize);
			myFallingThings.SetPolies(PolyType.All);
			myFallingThings.SetGameMode(GameMode.Off);
			string[] a = { "PageGame1_04_깨끗이씻기.png", "PageGame1_04_부모님께반말하기.png", "PageGame1_04_선생님께인사.png",
				"PageGame1_04_손님한테인사 안하기.png", "PageGame1_04_스스로일어나기.png", "PageGame1_04_식전인사하기.png",
				"PageGame1_04_위험한장소가기.png", "PageGame1_04_인사거절하기.png","PageGame1_04_주머니에손넣고인사.png",
				"PageGame1_04_집안청소.png","PageGame1_04_하교후인사하기.png","PageGame1_04_형제와싸움.png" };
			myFallingThings.AddStrImage(a);
		}

		public void SetCanvas(Canvas _canvasSkel)
		{
			if (m_canvasSkel == null)
				m_canvasSkel = _canvasSkel;

			m_canvasSkel.ClipToBounds = true;

			UpdatePlayfieldSize();

		}

		public void SetGameMode(int mode)
		{
			m_mode = mode;
		}

		public void SetGameSpeed(int factor)
		{
			myFallingThings.SetGravity(dropGravity * factor);
		}

		public void GameStart()
		{
			// 멤버 변수 초기화
			m_predNextFrame = DateTime.MinValue;
			m_lastFrameDrawn = DateTime.MinValue;
			m_playersAlive = 0;
			
			if (m_myKinect.sensorChooser != null)
			{
				m_myKinect.evtReadySingleSkel += new EventHandler<AllFramesReadyEventArgs>(EventCheckHandOver);
			}

			var myGameThread = new Thread(GameThread);
			myGameThread.SetApartmentState(ApartmentState.STA);
			myGameThread.Start();
		}

		public int GetGameResult()
		{
			int score = 0;
			if (myFallingThings.scores.ContainsKey(m_lastID))
				score = myFallingThings.scores[m_lastID];

			return score;
		}

		public void GameEnd()
		{
			m_runningGameThread = false; // thread가 멈추면 물체와 점수 그려지지 않음
			if (m_myKinect.sensorChooser != null)
			{
				m_myKinect.evtReadySingleSkel -= new EventHandler<AllFramesReadyEventArgs>(EventCheckHandOver);
			}

			myFallingThings.Reset(); // 모든 물체 제거
		}

		private void UpdatePlayfieldSize()
		{
			//// Size of player wrt size of playfield, putting ourselves low on the screen.
			//screenRect.X = 0;
			//screenRect.Y = 0;
			//screenRect.Width = playfield.ActualWidth;
			//screenRect.Height = playfield.ActualHeight;

			//BannerText.UpdateBounds(screenRect);

			playerBounds.X = 0;
			playerBounds.Width = m_canvasSkel.ActualWidth;
			playerBounds.Y = m_canvasSkel.ActualHeight * 0.2;
			playerBounds.Height = m_canvasSkel.ActualHeight * 0.75;

			foreach (var player in players)
			{
				player.Value.SetBounds(playerBounds);
			}

			Rect fallingBounds = playerBounds;
			fallingBounds.Y = 0;
			fallingBounds.Height = m_canvasSkel.ActualHeight;
			if (myFallingThings != null)
			{
				myFallingThings.SetBoundaries(fallingBounds);
			}
		}

		private void EventCheckHandOver(object sender, AllFramesReadyEventArgs e)
		{
			Skeleton skel = (Skeleton)sender;
			int skeletonSlot = 0;

			MySkelPlayer player;
			if (players.ContainsKey(skeletonSlot))
			{
				player = players[skeletonSlot];
			}
			else
			{
				player = new MySkelPlayer(skeletonSlot);
				player.SetBounds(playerBounds);
				players.Add(skeletonSlot, player);
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


		private void GameThread()
		{
			m_runningGameThread = true;
			m_predNextFrame = DateTime.Now;
			m_actualFrameTime = 1000.0 / targetFramerate;

			// Try to dispatch at as constant of a framerate as possible by sleeping just enough since
			// the last time we dispatched.
			while (m_runningGameThread)
			{
				// Calculate average framerate.  
				DateTime now = DateTime.Now;
				if (m_lastFrameDrawn == DateTime.MinValue)
				{
					m_lastFrameDrawn = now;
				}

				double ms = now.Subtract(m_lastFrameDrawn).TotalMilliseconds;
				m_actualFrameTime = (m_actualFrameTime * 0.95) + (0.05 * ms);
				m_lastFrameDrawn = now;

				// Adjust target framerate down if we're not achieving that rate
				m_frameCount++;
				if ((m_frameCount % 100 == 0) && (1000.0 / m_actualFrameTime < targetFramerate * 0.92))
				{
					targetFramerate = Math.Max(MinFramerate, (targetFramerate + (1000.0 / m_actualFrameTime)) / 2);
				}

				if (now > m_predNextFrame)
				{
					m_predNextFrame = now;
				}
				else
				{
					double milliseconds = m_predNextFrame.Subtract(now).TotalMilliseconds;
					if (milliseconds >= TimerResolution)
					{
						Thread.Sleep((int)(milliseconds + 0.5));
					}
				}

				m_predNextFrame += TimeSpan.FromMilliseconds(1000.0 / targetFramerate);

				m_canvasSkel.Dispatcher.Invoke(DispatcherPriority.Send, new Action<int>(HandleGameTimer), 0);
			}
		}

		int m_lastID = -1;
		private void HandleGameTimer(int param)
		{
			// Every so often, notify what our actual framerate is
			if ((m_frameCount % 100) == 0)
			{
				myFallingThings.SetFramerate(1000.0 / m_actualFrameTime);
			}

			// Advance animations, and do hit testing.
			for (int i = 0; i < NumIntraFrames; ++i)
			{
				foreach (var pair in players)
				{
					m_lastID = pair.Value.GetId();
					HitType hit = myFallingThings.LookForHits(pair.Value.Segments, pair.Value.GetId(), m_mode);
					if ((hit & HitType.Squeezed) != 0)
					{
						//squeezeSound.Play();
					}
					else if ((hit & HitType.Popped) != 0)
					{
						//popSound.Play();
					}
					else if ((hit & HitType.Hand) != 0)
					{
						//hitSound.Play();
					}
				}

				myFallingThings.AdvanceFrame();
			}

			// Draw new Wpf scene by adding all objects to canvas
			m_canvasSkel.Children.Clear();
			myFallingThings.DrawFrame(m_canvasSkel.Children);

			// 점수 표시하는거 일단 주석
			//m_canvas.Children.Clear();
			//myFallingThings.DrawFrameScore(m_canvas.Children, m_canvas.ActualWidth, m_canvas.ActualHeight);

			foreach (var player in players)
			{
				player.Value.Draw(m_canvasSkel.Children, m_mode);
			}

			// 			BannerText.Draw(m_canvas.Children);
			// 			FlyingText.Draw(m_canvas.Children);

			CheckPlayers();

			if (m_runningGameThread == false)
			{
				m_canvasSkel.Children.Clear();
			}
		}

		private int m_playersAlive;
		private void CheckPlayers()
		{
			foreach (var player in players)
			{
				if (!player.Value.IsAlive)
				{
					// Player left scene since we aren't tracking it anymore, so remove from dictionary
					players.Remove(player.Value.GetId());
					break;
				}
			}

			// Count alive players
			int alive = players.Count(player => player.Value.IsAlive);

			if (alive != m_playersAlive)
			{
				if (alive == 2)
				{
					myFallingThings.SetGameMode(GameMode.TwoPlayer);
				}
				else if (alive == 1)
				{
					myFallingThings.SetGameMode(GameMode.Solo);
				}
				else if (alive == 0)
				{
					myFallingThings.SetGameMode(GameMode.Off);
				}

				// 				if ((playersAlive == 0) && (mySpeechRecognizer != null))
				// 				{
				// 					BannerText.NewBanner(
				// 						Properties.Resources.Vocabulary,
				// 						screenRect,
				// 						true,
				// 						System.Windows.Media.Color.FromArgb(200, 255, 255, 255));
				// 				}

				m_playersAlive = alive;
			}
		}
	}
}
