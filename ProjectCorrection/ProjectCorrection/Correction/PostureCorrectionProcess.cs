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
using System.IO;

using Microsoft.Kinect;
using ProjectCorrection.Kinect;

namespace ProjectCorrection.Correction
{
    /// <summary>
    /// 자세 교정에 관한 처리를 담당하는 클래스입니다.
    /// </summary>
    public class PostureCorrectionProcess
    {
        private Image image1; //자세교정 화면에서 받아들이는 이미지 뼈대
        private Image selectImage;  //자세교정 화면에서 받아들이는 이미지 뼈대
        private Canvas canvas1;     //자세교정 화면에서 받아들이는 캔버스 뼈대
        private SkeletonC skeleton;  //자세교정을 위해 Skellton 처리를 하는 참조
        private byte[] selectImageBytes;    //선택된 이미지의 바이트 배열
        private String postureName;     //선택된 자세의 자세이름
        private int[] angles;
        /// <summary>
        /// 이 클래스에 대한 생성자를 나타냅니다.  
        /// </summary>
        /// <param name="image"><c>PostureCorrectionScreen</c>에서 키넥트로 출력할 이미지 뼈대입니다.</param>
        /// <param name="selectImg"><c>PostureCorrectionScreen</c>에서 선택된 이미지를 출력하기 위한 뼈대입니다.</param>
        /// <param name="canvas"><c>PostureCorrectionScreen</c>에서 처리할 canvas입니다.</param>
        /// <param name="selectImgBytes">선택된 이미지의 byte배열 입니다.</param>
        /// <param name="pName">자세교정을 위해 선택된 이미지의 자세이름입니다.</param>
        public PostureCorrectionProcess(Image image, Image selectImg, Canvas canvas, byte[] selectImgBytes, String pName, int[] angles)
        {
            image1 = image;
            canvas1 = canvas;
            selectImageBytes = selectImgBytes;
            postureName = pName;
            selectImage = selectImg;
            this.angles = angles;
        }

        /// <summary>
        /// 자세교정을 위한 프로세스를 진행합니다.
        /// </summary>
        public void process()
        {
            skeleton = new SkeletonC(image1, canvas1, angles);
        }

        /// <summary>
        /// 선택한 자세의 이미지를 보여줍니다.
        /// </summary>
        public void ShowSelectImage()
        {
            try
            {
                if (!Directory.Exists(@"C:\PostureCapture"))            //C드라이브에 해당 디렉토리가 없으면 만든다.
                    Directory.CreateDirectory(@"C:\PostureCapture");

                String filePath = "C:\\PostureCapture\\" + postureName + ".png";    //그 디렉토리에 파일을 저장한다.
                FileStream stream = new FileStream(filePath, FileMode.Create);
                stream.Write(selectImageBytes, 0, selectImageBytes.Length);
                stream.Close();

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(filePath);
                bitmap.EndInit();
                selectImage.Source = bitmap;             
            }
            catch (Exception e)
            {
                MessageBox.Show("유효하지 않은 접근입니다.", "No Access");
            }
        }
    }
}
