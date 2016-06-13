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
using ProjectCorrection.Database;
using ProjectCorrection.Main;
using Microsoft.Kinect;
using System.Speech.Synthesis;

namespace ProjectCorrection.Kinect
{
    /// <summary>
    /// 키넥트가 관절을 추적하고 전달된 이들의 정보를 가져와서 연결된 선을 그리기 위한 클래스입니다.
    /// </summary>
    public class SkeletonC
    {
        private Polyline[] m_poly = new Polyline[5]; /*Polyline의 특성을 가지는 m_poly변수 생성*/
        private Image image1;           //스켈리턴을 그리기 위한 이미지
        private Canvas canvas1;         //스켈리턴을 그리기 위한 캔버스
        private StreamGeometryContext[] sgc = new StreamGeometryContext[20];
        private Path[] paths = new Path[10];
        private Point[] points;
        private TextBlock[] textBlocks = new TextBlock[15];
        private int[] angles;
        private int[] diffAngles = new int[14];
        private bool flag = true;
        private SpeechSynthesizer speechSynthesizer;

        //     private Pose[] _PoseLibrary;
        //     private Pose _StartPose;
        /// <summary>
        /// 이 클래스에 대한 생성자입니다. 생성되는 즉시 스켈리턴 프로세스를 진행합니다.
        /// </summary>
        /// <param name="img">스켈리턴을 그리기 위한 이미지</param>
        /// <param name="canvas">스켈리턴을 그리기 위한 캔버스</param>
        public SkeletonC(Image img, Canvas canvas, int[] angles)
        {
            InitializeNui();
            image1 = img;
            canvas1 = canvas;
            this.angles = angles;
            speechSynthesizer = new SpeechSynthesizer();
            speechSynthesizer.SetOutputToDefaultAudioDevice();
            speechSynthesizer.Volume = 100;

            for (int i = 0; i < 5; i++) /*m_poly의 모든 배열에 실제 메모리 사용 선언*/
            {
                m_poly[i] = new Polyline();
                m_poly[i].Stroke = new SolidColorBrush(Colors.Red);/*테두리를 빨간색으로 보이게 만들어줌*/
                m_poly[i].StrokeThickness = 2;/*테두리의 굵기를 2로 선언*/
                m_poly[i].Visibility = Visibility.Collapsed;/*레이아웃에 요소를 표시하지 않도록 함*/

                Canvas.SetTop(m_poly[i], 0);
                Canvas.SetLeft(m_poly[i], 0);

                canvas1.Children.Add(m_poly[i]);

            }

            for (int i = 0; i < 15; i++) /*textBlocks의 모든 배열에 실제 메모리 사용 선언*/
            {
                textBlocks[i] = new TextBlock();
                textBlocks[i].Height = 100;
                textBlocks[i].Width = 100;
                textBlocks[i].FontSize = 40;
                textBlocks[i].Foreground = Brushes.Black;
                canvas1.Children.Add(textBlocks[i]);
            }
        }
        private KinectSensor nui = null;/*nui라는 키넥트 센서 변수를 만듦
                         nui라는 키넥트 센서를 컬러 스트림과 뎁스 스트림 그리고
                         스켈리턴 스트림이 가능하게 만듦*/
        /// <summary>
        /// 키넥트 사용을 시작하기 위해 사용할 키넥트를 가져오고 키넥트 사용을 시작하는 메소드입니다.
        /// </summary>
        public void InitializeNui()
        {
            nui = KinectSensor.KinectSensors[0];

            nui.ColorStream.Enable();
            nui.ColorFrameReady += new /*컬러 스트림 정보가 들어올 때마다 실행*/
               EventHandler<ColorImageFrameReadyEventArgs>(nui_ColorFrameReady);
            nui.DepthStream.Enable();/*뎁스 스트림 사용 가능하게 선언*/
            nui.SkeletonStream.Enable();/*스켈리턴 스트림을 사용 가능하게 선언*/

            nui.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(nui_AllFramesReady);/*모든 프레임 정보가 들어올 때마다 실행*/
            nui.Start();
        }

        /// <summary>
        /// ColorFrame의 그래픽 데이터를 처리하기 위한 이벤트 처리 메소드 입니다.
        /// </summary>
        void nui_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e) /*프레임이 들어올 때마다 수행*/
        {
            using (ColorImageFrame ImageParam = e.OpenColorImageFrame())
            {
                if (ImageParam == null) return;

                byte[] ImageBits = new byte[ImageParam.PixelDataLength];
                ImageParam.CopyPixelDataTo(ImageBits);

                BitmapSource src = null;
                src = BitmapSource.Create(ImageParam.Width, ImageParam.Height, 96, 96, PixelFormats.Bgr32, null, ImageBits, ImageParam.Width * ImageParam.BytesPerPixel);
                image1.Source = src;
            }
        }

        private bool isCorrect()
        {
            for (int i = 0; i < 14; i++)
            {
                if (diffAngles[i] >= 20 || diffAngles[i] <= -20)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 두 관절 사이의 각도를 계산하는 수학 메소드입니다.
        /// </summary>
        /// <param name="zeroPoint"></param>
        /// <param name="anglePoint"></param>
        /// <returns></returns>
        private double GetJointAngle(Point zeroPoint, Point anglePoint)
        {
            Point x = new Point(zeroPoint.X + anglePoint.X, zeroPoint.Y);

            double a, b, c;

            a = Math.Sqrt(Math.Pow(zeroPoint.X - anglePoint.X, 2) + Math.Pow(zeroPoint.Y - anglePoint.Y, 2));
            b = anglePoint.X;
            c = Math.Sqrt(Math.Pow(anglePoint.X - x.X, 2) + Math.Pow(anglePoint.Y - x.Y, 2));

            double angleRad = Math.Acos((a * a + b * b - c * c) / (2 * a * b));
            double angleDeg = angleRad * 180 / Math.PI;

            if (zeroPoint.Y < anglePoint.Y)
            {
                angleDeg = 360 - angleDeg;
            }
            return angleDeg;
        }


        private void SetDiffAngle(int[] inputAngles)
        {
            for (int i = 0; i < 14; i++)
            {
                diffAngles[i] = angles[i] - inputAngles[i];
            }
        }
        /// <summary>
        /// 프레임이 들어올때마다 이에 대한 스켈리턴 작업을 하는 이벤트 처리 메소드입니다.
        /// </summary>
        public void nui_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            using (SkeletonFrame sf = e.OpenSkeletonFrame())
            {

                if (sf == null) return;

                Skeleton[] skeletonData = new Skeleton[sf.SkeletonArrayLength];

                sf.CopySkeletonDataTo(skeletonData);

                using (DepthImageFrame depthImageFrame = e.OpenDepthImageFrame())
                {
                    if (depthImageFrame != null)
                    {
                        foreach (Skeleton sd in skeletonData)
                        {
                            if (sd.TrackingState == SkeletonTrackingState.Tracked)
                            {
                                int nMax = 20;
                                Joint[] joints = new Joint[nMax];
                                for (int j = 0; j < nMax; j++)
                                {
                                    joints[j] = sd.Joints[(JointType)j];
                                }
                                points = new Point[20];
                                for (int j = 0; j < nMax; j++)
                                {
                                    DepthImagePoint depthPoint;
                                    depthPoint = depthImageFrame.MapFromSkeletonPoint(joints[j].Position);
                                    points[j] = new Point((int)(image1.Width * depthPoint.X / depthImageFrame.Width), (int)(image1.Height * depthPoint.Y / depthImageFrame.Height));
                                }


                                PointCollection pc0 = new PointCollection(4);/*pc()에 4개의 포인트 추가*/
                                pc0.Add(points[(int)JointType.HipCenter]);
                                pc0.Add(points[(int)JointType.Spine]);
                                pc0.Add(points[(int)JointType.ShoulderCenter]);
                                pc0.Add(points[(int)JointType.Head]);
                                m_poly[0].Points = pc0;
                                m_poly[0].Visibility = Visibility.Visible;

                                PointCollection pc1 = new PointCollection(5);
                                pc1.Add(points[(int)JointType.ShoulderCenter]);
                                pc1.Add(points[(int)JointType.ShoulderLeft]);
                                pc1.Add(points[(int)JointType.ElbowLeft]);
                                pc1.Add(points[(int)JointType.WristLeft]);
                                pc1.Add(points[(int)JointType.HandLeft]);
                                m_poly[1].Points = pc1;
                                m_poly[1].Visibility = Visibility.Visible;

                                PointCollection pc2 = new PointCollection(5);
                                pc2.Add(points[(int)JointType.ShoulderCenter]);
                                pc2.Add(points[(int)JointType.ShoulderRight]);
                                pc2.Add(points[(int)JointType.ElbowRight]);
                                pc2.Add(points[(int)JointType.WristRight]);
                                pc2.Add(points[(int)JointType.HandRight]);
                                m_poly[2].Points = pc2;
                                m_poly[2].Visibility = Visibility.Visible;

                                PointCollection pc3 = new PointCollection(5);
                                pc3.Add(points[(int)JointType.HipCenter]);
                                pc3.Add(points[(int)JointType.HipLeft]);
                                pc3.Add(points[(int)JointType.KneeLeft]);
                                pc3.Add(points[(int)JointType.AnkleLeft]);
                                pc3.Add(points[(int)JointType.FootLeft]);
                                m_poly[3].Points = pc3;
                                m_poly[3].Visibility = Visibility.Visible;

                                PointCollection pc4 = new PointCollection(5);
                                pc4.Add(points[(int)JointType.HipCenter]);
                                pc4.Add(points[(int)JointType.HipRight]);
                                pc4.Add(points[(int)JointType.KneeRight]);
                                pc4.Add(points[(int)JointType.AnkleRight]);
                                pc4.Add(points[(int)JointType.FootRight]);
                                m_poly[4].Points = pc4;
                                m_poly[4].Visibility = Visibility.Visible;

                                int[] inputAngles = new int[14];
                                inputAngles[0] = (int)GetJointAngle(points[(int)JointType.ElbowRight], points[(int)JointType.ShoulderRight]);
                                inputAngles[1] = (int)GetJointAngle(points[(int)JointType.WristRight], points[(int)JointType.ElbowRight]);
                                inputAngles[2] = (int)GetJointAngle(points[(int)JointType.ElbowLeft], points[(int)JointType.ShoulderLeft]);
                                inputAngles[3] = (int)GetJointAngle(points[(int)JointType.WristLeft], points[(int)JointType.ElbowLeft]);
                                inputAngles[4] = (int)GetJointAngle(points[(int)JointType.ShoulderRight], points[(int)JointType.ShoulderCenter]);
                                inputAngles[5] = (int)GetJointAngle(points[(int)JointType.ShoulderLeft], points[(int)JointType.ShoulderCenter]);
                                inputAngles[6] = (int)GetJointAngle(points[(int)JointType.Head], points[(int)JointType.ShoulderCenter]);
                                inputAngles[7] = (int)GetJointAngle(points[(int)JointType.ShoulderCenter], points[(int)JointType.Spine]);
                                inputAngles[8] = (int)GetJointAngle(points[(int)JointType.HipCenter], points[(int)JointType.HipRight]);
                                inputAngles[9] = (int)GetJointAngle(points[(int)JointType.HipCenter], points[(int)JointType.HipLeft]);
                                inputAngles[10] = (int)GetJointAngle(points[(int)JointType.HipRight], points[(int)JointType.KneeRight]);
                                inputAngles[11] = (int)GetJointAngle(points[(int)JointType.HipLeft], points[(int)JointType.KneeLeft]);
                                inputAngles[12] = (int)GetJointAngle(points[(int)JointType.KneeRight], points[(int)JointType.AnkleRight]);
                                inputAngles[13] = (int)GetJointAngle(points[(int)JointType.KneeLeft], points[(int)JointType.AnkleLeft]);
                               
                                SetDiffAngle(inputAngles);
                               
                                textBlocks[0].RenderTransform = new TranslateTransform(points[(int)JointType.ElbowRight].X, points[(int)JointType.ElbowRight].Y);
                                textBlocks[0].Text = diffAngles[0] + "";
                                textBlocks[1].RenderTransform = new TranslateTransform(points[(int)JointType.WristRight].X, points[(int)JointType.WristRight].Y);
                                textBlocks[1].Text = diffAngles[1] + "";
                                textBlocks[2].RenderTransform = new TranslateTransform(points[(int)JointType.ElbowLeft].X, points[(int)JointType.ElbowLeft].Y);
                                textBlocks[2].Text = diffAngles[2] + "";
                                textBlocks[3].RenderTransform = new TranslateTransform(points[(int)JointType.WristLeft].X, points[(int)JointType.WristLeft].Y);
                                textBlocks[3].Text = diffAngles[3] + "";
                                textBlocks[4].RenderTransform = new TranslateTransform(points[(int)JointType.ShoulderRight].X, points[(int)JointType.ShoulderRight].Y);
                                textBlocks[4].Text = diffAngles[4] + "";
                                textBlocks[5].RenderTransform = new TranslateTransform(points[(int)JointType.ShoulderLeft].X, points[(int)JointType.ShoulderLeft].Y);
                                textBlocks[5].Text = diffAngles[5] + "";
                                textBlocks[6].RenderTransform = new TranslateTransform(points[(int)JointType.ShoulderCenter].X, points[(int)JointType.ShoulderCenter].Y);
                                textBlocks[6].Text = diffAngles[6] + "";
                                textBlocks[7].RenderTransform = new TranslateTransform(points[(int)JointType.Spine].X, points[(int)JointType.Spine].Y);
                                textBlocks[7].Text = diffAngles[7] + "";
                                textBlocks[8].RenderTransform = new TranslateTransform(points[(int)JointType.HipRight].X, points[(int)JointType.HipRight].Y);
                                textBlocks[8].Text = diffAngles[8] + "";
                                textBlocks[9].RenderTransform = new TranslateTransform(points[(int)JointType.HipLeft].X, points[(int)JointType.HipLeft].Y);
                                textBlocks[9].Text = diffAngles[9] + "";
                                textBlocks[10].RenderTransform = new TranslateTransform(points[(int)JointType.KneeRight].X, points[(int)JointType.KneeRight].Y);
                                textBlocks[10].Text = diffAngles[10] + "";
                                textBlocks[11].RenderTransform = new TranslateTransform(points[(int)JointType.KneeLeft].X, points[(int)JointType.KneeLeft].Y);
                                textBlocks[11].Text = diffAngles[11] + "";
                                textBlocks[12].RenderTransform = new TranslateTransform(points[(int)JointType.AnkleRight].X, points[(int)JointType.AnkleRight].Y);
                                textBlocks[12].Text = diffAngles[12] + "";
                                textBlocks[13].RenderTransform = new TranslateTransform(points[(int)JointType.AnkleLeft].X, points[(int)JointType.AnkleLeft].Y);
                                textBlocks[13].Text = diffAngles[13] + "";

                                

                                textBlocks[14].RenderTransform = new TranslateTransform(10, 10);
                                textBlocks[14].Height = 100;
                                textBlocks[14].Width = 500;
                                if (isCorrect())
                                {
                                 //   if (flag == false)
                                 //       speechSynthesizer.Speak("Correct Posture");
                                    textBlocks[14].Foreground = Brushes.Green;
                                    textBlocks[14].Text = "올바른 자세입니다.";
                                    flag = true;
                                }
                                else
                                {
                                //    if (flag == true)
                                //        speechSynthesizer.Speak("Incorrect Posture");
                                    textBlocks[14].Foreground = Brushes.Red;
                                    textBlocks[14].Text = "잘못된 자세입니다.";
                                    flag = false;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}