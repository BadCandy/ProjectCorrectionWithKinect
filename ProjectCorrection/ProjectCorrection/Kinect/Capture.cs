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
using System.IO;

namespace ProjectCorrection.Kinect
{
    ///<summary>
    /// 키넥트를 이용하여 자세를 캡쳐하기 위한 클래스입니다.
    /// </summary>
    public partial class Capture
    {
        private Image image1;       //키넥트를 통하여 출력되는 이미지
        private int[] angle = new int[14];
        /// <summary>
        /// 이 클래스에 대한 생성자입니다. 생성되는 즉시 캡쳐 프로세스가 진행됩니다.
        /// </summary>
        /// <param name="image">키넥트를 통하여 출력되는 이미지 뼈대</param>
        public Capture(Image image)
        {
            image1 = image;
            InitializeNui(); /* 설정과 작업의 편의를 위해 다음과 같이 InitializeNui라는 메소드를 호출한다. 
                              * InitializeNui는 직접 작성해야 하는 코드이다. */
        }

        private KinectSensor nui = null; // nui라는 이름으로 키넥트를 사용하기 위한 필드를 하나 생성한다.

        /// <summary>
        /// 키넥트 사용을 시작하기 위해 사용할 키넥트를 가져오고 키넥트 사용을 시작하는 메소드입니다.
        /// </summary>
        public void InitializeNui() //InitializeNUI라는 메소드를 작성한다.
        {
            nui = KinectSensor.KinectSensors[0]; /* 사용할 첫 번째 키넥트를 가져온다. 별도의 초기화
                                                  * 작업은 하지 않는다. */
            nui.ColorStream.Enable();/*컬러 스트림을 가져오기 위한 작업을 추가한다. 지정한 enable 옵셥을 통해 컬러 스트림을
                                      * 사용 가능하게 준비한다. */
            nui.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(nui_ColorFrameReady);
            //프레임이 들어올 떄 마다 nui_ColorFrameReady 메소드를 실행시킨다.

            nui.DepthStream.Enable();/*뎁스 스트림 사용 가능하게 선언*/
            nui.SkeletonStream.Enable();/*스켈리턴 스트림을 사용 가능하게 선언*/

            nui.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(nui_AllFramesReady);/*모든 프레임 정보가 들어올 때마다 실행*/
            nui.Start();
        }

        private BitmapSource src = null; /*Bitmapsouce 클래스는 비트맵 형태로 이미지를 생성해 주는 역할을 하는 클래스로
                                      * Create 메소드를 호출한다 */
        /*
         public static BitmapSource Create(
        int PixelWidth, //형태:Int32 기능:비트맵의 너비
        int PixelHeight, //형태:Int32 기능:비트맵의 높이
        double dpiX, //형태:Double 기능: 비트맵의 가로 DPI
        double dpiY, //형태:Double 기능:비트매의 세로 DPI
        PixelFormat PixelFormat, //형태:PixelFormat 기능:비트맵의 픽셀 형식
        BitmapPalette BitmapPalette, //형태:BitmapPalette 기능:비트맵의 색상표
        Array pixels, // 형태:Array 기능: 비트맵 이미지 내용이 담긴 바이트 배열
        int stride // 형태:Int32 기능:비트맵의 스트라이트

           pixelFormat에 사용하는 비트맵의 픽셀 형식은 PixelFormat으로 지정되지만 
           PixelFormats라는 것을 이용하면 간단히 지정이 가능하다. PixelFormats.Bgr32는 Bgra32픽셀 형식을 가져온다.
           Bgra32는 32BPP를 사용하는 sRGB 형식이다. 파랑, 녹색, 빨강, 알파에는 8BPP씩 할당된다. 
           BPP는 픽셀당 비트 수이다. IMAGE.Bits는 실제 영상 데이터가 들어있는 데이터에 해당한다)*/


        /// <summary>
        /// ColorFrame의 그래픽 데이터를 처리하기 위한 이벤트 처리 메소드 입니다.
        /// </summary>
        public void nui_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame ImageParam = e.OpenColorImageFrame())
            {
                if (ImageParam == null) return;

                //ColorImageFrame는 파라미터로 넘어온 그래픽 데이터를 사용하기 위한 클래스이다.
                //ImageFrame 클래스를 기반으로 다음의 내용이 추가 되어 있다.
                /* 옵션:Format           기능:컬러 데이터 형식
                 * 옵션:PixelDataLength  기능:픽셀 데이터의 길이
                 * 옵션:copyPixelDataTo  기능:픽셀 데이터를 복사*/

                byte[] ImageBits = new byte[ImageParam.PixelDataLength];
                ImageParam.CopyPixelDataTo(ImageBits);



                src = BitmapSource.Create(ImageParam.Width, ImageParam.Height, 96, 96, PixelFormats.Bgr32, null, ImageBits, ImageParam.Width * ImageParam.BytesPerPixel);
                image1.Source = src; /*image1은 앞서 MainWindow.xaml에서 작업한 Image 컨트롤이다.
                                          * 여기에 Source라는 형태로 이미지를 넣으면 화면에 표시된다.
                                          * nui_ColorFrameReady는 초당 약30회 가량 실행된다.
                                          * 그러므로 image1.Source를 통해 출력되는 이미지는 정지영상이지만 동영상과 같은 효과가 나타난다 */
            }
        }
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

        /*BitmapEncoder 클래스는 고덱에서 인코딩된 이미지에 대한 특수화된 압축루틴 또는 인터레이스를 지원할수있다.
비트맵 형식에서 프레임을 하나만 지원하는 경우에도 모든 파생된 인코더에서 다중 프레임을 지정할 수 있다. 이 경우 첫번째
프레임만 저장된다.
BitmapEncoder를 통해 사용할 수 있는 클래스는 다음의 표와 같다.
클래스               기능
BmpBitmapEncoder   비트맴 원형의 이미지 인코딩
GifBitmapEncoder   그래픽 인터체인지 포맷의 이미지 인코딩
JpegBitmapEncoder   정지영상 압축 표준 형태의 이미지 인코딩
PngBitmapEncoder   비손실 그래픽 파일 포맷의 이미지 인코딩
TiffBitmapEncoder   화상 파일 표준 포맷의 이미지 인코딩
WmpBitmapEncoder   윈도 미디어 사진 포맷의 이미지 인코딩*/
        /// <summary>
        /// 키넥트를 이용해 화면을 캡쳐하는 메소드 입니다.
        /// </summary>
        /// <param name="fileName">유저가 지정한 파일이름</param>
        /// <param name="id">유저의 id</param>
        public void CapturePosture(String fileName, String id)
        {            
            if (src != null)
            {
                if (!Directory.Exists(@"C:\PostureCapture"))
                    Directory.CreateDirectory(@"C:\PostureCapture");
                try { 
                String filePath = "C:\\PostureCapture\\" + fileName + ".png";
                PngBitmapEncoder encoder = null;
                encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(src));
                FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                encoder.Save(stream);
                stream.Close();

                BinaryReader br;
                byte[] imageData;
                stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                br = new BinaryReader(stream);
                imageData = br.ReadBytes((int)stream.Length);
                br.Close();
                stream.Close();

                PostureDB.SaveImage(fileName, id, imageData, angle);

                System.Diagnostics.Process exe = new System.Diagnostics.Process();
                exe.StartInfo.FileName = filePath;
                exe.Start();
                }
                catch (Exception e)
                {

                }
            }
        }

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
                                Point[] points = new Point[20];
                                for (int j = 0; j < nMax; j++)
                                {
                                    DepthImagePoint depthPoint;
                                    depthPoint = depthImageFrame.MapFromSkeletonPoint(joints[j].Position);
                                    points[j] = new Point((int)(image1.Width * depthPoint.X / depthImageFrame.Width), (int)(image1.Height * depthPoint.Y / depthImageFrame.Height));
                                }

                                angle[0] = (int)GetJointAngle(points[(int)JointType.ElbowRight], points[(int)JointType.ShoulderRight]);
                                angle[1] = (int)GetJointAngle(points[(int)JointType.WristRight], points[(int)JointType.ElbowRight]);
                                angle[2] = (int)GetJointAngle(points[(int)JointType.ElbowLeft], points[(int)JointType.ShoulderLeft]);
                                angle[3] = (int)GetJointAngle(points[(int)JointType.WristLeft], points[(int)JointType.ElbowLeft]);
                                angle[4] = (int)GetJointAngle(points[(int)JointType.ShoulderRight], points[(int)JointType.ShoulderCenter]);
                                angle[5] = (int)GetJointAngle(points[(int)JointType.ShoulderLeft], points[(int)JointType.ShoulderCenter]);
                                angle[6] = (int)GetJointAngle(points[(int)JointType.Head], points[(int)JointType.ShoulderCenter]);
                                angle[7] = (int)GetJointAngle(points[(int)JointType.ShoulderCenter], points[(int)JointType.Spine]);
                                angle[8] = (int)GetJointAngle(points[(int)JointType.HipCenter], points[(int)JointType.HipRight]);
                                angle[9] = (int)GetJointAngle(points[(int)JointType.HipCenter], points[(int)JointType.HipLeft]);
                                angle[10] = (int)GetJointAngle(points[(int)JointType.HipRight], points[(int)JointType.KneeRight]);
                                angle[11] = (int)GetJointAngle(points[(int)JointType.HipLeft], points[(int)JointType.KneeLeft]);
                                angle[12] = (int)GetJointAngle(points[(int)JointType.KneeRight], points[(int)JointType.AnkleRight]);
                                angle[13] = (int)GetJointAngle(points[(int)JointType.KneeLeft], points[(int)JointType.AnkleLeft]);
                            }
                        }
                    }
                }
            }
        }
    }
}
