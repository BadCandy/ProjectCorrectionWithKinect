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
using ProjectCorrection.Enrollment;
using ProjectCorrection.Correction;
using ProjectCorrection.Login;
using ProjectCorrection.Select;
using ProjectCorrection.Delete;
using Microsoft.Kinect;
namespace ProjectCorrection.Main
{
    /// <summary>
    /// 메인화면을 나타내는 클래스입니다.
    /// </summary>
    public partial class MainScreen : Page
    {
        private String id;                      //현재 유저 id
        private MainScreenProcess mSProcess;    //메인화면 처리를 위한 참조
        private KinectSensor _KinectDevice;
        private Skeleton[] _FrameSkeletons;
        /// <summary>
        /// 이 클래스에 대한 생성자입니다. 생성되는 순간 메인화면 프로세스가 진행됩니다.
        /// </summary>
        /// <param name="id">현재 유저 id</param>
        public MainScreen(string id)
        {
            InitializeComponent();
            this.id = id;
            this.KinectDevice = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
            mSProcess = new MainScreenProcess(this.id, textBlock1, idBlock);
            mSProcess.currentUser();
        }

        public KinectSensor KinectDevice
        {
            get { return this._KinectDevice; }
            set
            {
                if (this._KinectDevice != value)
                {
                    //Uninitialize
                    if (this._KinectDevice != null)
                    {
                        this._KinectDevice.Stop();
                        this._KinectDevice.SkeletonFrameReady -= KinectDevice_SkeletonFrameReady;
                        this._KinectDevice.SkeletonStream.Disable();
                        //              SkeletonViewerElement.KinectDevice = null;
                        this._FrameSkeletons = null;
                    }

                    this._KinectDevice = value;

                    //Initialize
                    if (this._KinectDevice != null)
                    {
                        if (this._KinectDevice.Status == KinectStatus.Connected)
                        {
                            this._KinectDevice.SkeletonStream.Enable();
                            this._FrameSkeletons = new Skeleton[this._KinectDevice.SkeletonStream.FrameSkeletonArrayLength];

                            this._KinectDevice.Start();

                            //               SkeletonViewerElement.KinectDevice = this.KinectDevice;
                            this.KinectDevice.SkeletonFrameReady += KinectDevice_SkeletonFrameReady;
                        }
                    }
                }
            }
        }
        private void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case KinectStatus.Initializing:
                case KinectStatus.Connected:
                case KinectStatus.NotPowered:
                case KinectStatus.NotReady:
                case KinectStatus.DeviceNotGenuine:
                    this.KinectDevice = e.Sensor;
                    break;
                case KinectStatus.Disconnected:
                    //TODO: Give the user feedback to plug-in a Kinect device.                    
                    this.KinectDevice = null;
                    break;
                default:
                    //TODO: Show an error state
                    break;
            }
        }


        // Listing 5-2 & 5-4
        private void KinectDevice_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)  //여기서 처리
        {
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    frame.CopySkeletonDataTo(this._FrameSkeletons);
                    Skeleton skeleton = GetPrimarySkeleton(this._FrameSkeletons);

                    if (skeleton == null)
                    {
                          LeftHandElement.Visibility = Visibility.Collapsed;
                          RightHandElement.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                          TrackHand(skeleton.Joints[JointType.HandLeft], LeftHandElement, LayoutRoot);
                          TrackHand(skeleton.Joints[JointType.HandRight], RightHandElement, LayoutRoot);
                          moveCapture(skeleton);
                          moveCorrection(skeleton);
                          moveDelection(skeleton);
                    }
                }
            }
        }


        // Listing 5-2
        private static Skeleton GetPrimarySkeleton(Skeleton[] skeletons)
        {
            Skeleton skeleton = null;

            if (skeletons != null)
            {
                //Find the closest skeleton       
                for (int i = 0; i < skeletons.Length; i++)
                {
                    if (skeletons[i].TrackingState == SkeletonTrackingState.Tracked)
                    {
                        if (skeleton == null)
                        {
                            skeleton = skeletons[i];
                        }
                        else
                        {
                            if (skeleton.Position.Z > skeletons[i].Position.Z)
                            {
                                skeleton = skeletons[i];
                            }
                        }
                    }
                }
            }

            return skeleton;
        }


        private static Point GetJointPoint(KinectSensor kinectDevice, Joint joint, Size containerSize, Point offset)
        {
            DepthImagePoint point = kinectDevice.MapSkeletonPointToDepth(joint.Position, kinectDevice.DepthStream.Format);
            point.X = (int)((point.X * containerSize.Width / kinectDevice.DepthStream.FrameWidth) - offset.X);
            point.Y = (int)((point.Y * containerSize.Height / kinectDevice.DepthStream.FrameHeight) - offset.Y);

            return new Point(point.X, point.Y);
        }


        private void TrackHand(Joint hand, FrameworkElement cursorElement, FrameworkElement container)
        {
            if (hand.TrackingState == JointTrackingState.NotTracked)
            {
                cursorElement.Visibility = Visibility.Collapsed;
            }
            else
            {
                cursorElement.Visibility = Visibility.Visible;
                Point jointPoint = GetJointPoint(this.KinectDevice, hand, container.RenderSize, new Point(cursorElement.ActualWidth / 2.0, cursorElement.ActualHeight / 2.0));
                Canvas.SetLeft(cursorElement, jointPoint.X);
                Canvas.SetTop(cursorElement, jointPoint.Y);
            }
        }


        // Listing 5-5
        private void moveCapture(Skeleton skeleton)
        {
            //Determine if the user triggers to start of a new game
            if (HitTest(skeleton.Joints[JointType.HandLeft], LeftHandStartElement) && HitTest(skeleton.Joints[JointType.HandRight], RightHandStartElement1))
            {
                if (mSProcess.IsConnect())
                {
                    this.NavigationService.Navigate(new PostureEnrollmentScreen(id));
                    LeftHandElement.Visibility = Visibility.Collapsed;
                    RightHandElement.Visibility = Visibility.Collapsed;
                }
                else MessageBox.Show("키넥트를 연결하세요.", "키넥트 연결");
            }
            
        }

        private void moveCorrection(Skeleton skeleton)
        {
            //Determine if the user triggers to start of a new game
            if (HitTest(skeleton.Joints[JointType.HandLeft], LeftHandStartElement) && HitTest(skeleton.Joints[JointType.HandRight], RightHandStartElement2))
            {
                if (mSProcess.IsConnect())
                {
                    this.NavigationService.Navigate(new PostureSelectionScreen(id));
                    LeftHandElement.Visibility = Visibility.Collapsed;
                    RightHandElement.Visibility = Visibility.Collapsed;
                }
                else MessageBox.Show("키넥트를 연결하세요.", "키넥트 연결");
            }
        }

        private void moveDelection(Skeleton skeleton)
        {
            //Determine if the user triggers to start of a new game
            if (HitTest(skeleton.Joints[JointType.HandLeft], LeftHandStartElement) && HitTest(skeleton.Joints[JointType.HandRight], RightHandStartElement3))
            {
                this.NavigationService.Navigate(new PostureDeleteScreen(id));
                LeftHandElement.Visibility = Visibility.Collapsed;
                RightHandElement.Visibility = Visibility.Collapsed;
            }
        }

        private bool HitTest(Joint joint, UIElement target)
        {
            return (GetHitTarget(joint, target) != null);
        }


        // Listing 5-5
        private IInputElement GetHitTarget(Joint joint, UIElement target)
        {
            Point targetPoint = LayoutRoot.TranslatePoint(GetJointPoint(this.KinectDevice, joint, LayoutRoot.RenderSize, new Point()), target);
            return target.InputHitTest(targetPoint);
        }


        // Listing 5-6

        private void Button_Click_1(object sender, RoutedEventArgs e)       //해당 버튼을 누르는 순간 자세등록화면으로 전환하는 버튼 이벤트 처리기
        {
            if (mSProcess.IsConnect())
                this.NavigationService.Navigate(new PostureEnrollmentScreen(id));
            else MessageBox.Show("키넥트를 연결하세요.", "키넥트 연결");
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)       //해당 버튼을 누르는 순간 자세선택화면으로 전환하는 버튼 이벤트 처리기
        {
            if (mSProcess.IsConnect())
                this.NavigationService.Navigate(new PostureSelectionScreen(id));
            else MessageBox.Show("키넥트를 연결하세요.", "키넥트 연결");
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)       //해당 버튼을 누르는 순간 자세삭제화면으로 전환하는 버튼 이벤트 처리기
        {
            this.NavigationService.Navigate(new PostureDeleteScreen(id));
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)       //해당 버튼을 누르는 순간 키넥트의 연결여부를 출력하는 버튼 이벤트 처리기
        {
            mSProcess.renewStatus();
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)       //해당 버튼을 누르는 순간 로그인 화면으로 전환하는 버튼 이벤트 처리기
        {
            MessageBox.Show("로그아웃 되었습니다.", "로그 아웃");
            this.NavigationService.Navigate(new LoginScreen());
        }
    }
}
