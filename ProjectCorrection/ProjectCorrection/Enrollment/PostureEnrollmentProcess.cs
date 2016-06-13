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
using ProjectCorrection.Kinect;

namespace ProjectCorrection.Enrollment
{
    /// <summary>
    /// 자세등록에 대한 처리를 진행하는 클래스입니다.
    /// </summary>
    public class PostureEnrollmentProcess
    {
        private Image image1;       //키넥트를 통해 출력되는 이미지 뼈대
        private Capture capture;    //해당 자세를 캡쳐하기 위한 참조 
        private string id;          //현재 유저 id
        private TextBox textBox1;   //PostureEnrollmentScreen에서 입력받을 자세이름의 뼈대
        
        /// <summary>
        /// 이 클래스에 대한 생성자를 나타냅니다.
        /// </summary>
        /// <param name="id">현재 유저 id</param>
        /// <param name="image">키넥트를 통해 출력되는 이미지 뼈대</param>
        /// <param name="textBox"><c>PostureEnrollmentScreen</c>에서 입력받을 자세이름의 뼈대</param>
        public PostureEnrollmentProcess(String id, Image image, TextBox textBox)
        {
            this.id = id;
            image1 = image;
            textBox1 = textBox;
            
        }

        /// <summary>
        /// 자세등록에 관한 처리를 진행하는 메소드입니다.
        /// </summary>
        public void Process()
        {
            capture = new Capture(image1);
        }

        /// <summary>
        /// 자세를 캡쳐하기 위한 메소드입니다.
        /// </summary>
        public void CaptureMyPosture()
        {
            if (textBox1.Text == "")
                textBox1.Text = "사진01";
            capture.CapturePosture(textBox1.Text, id);
        }
    }
}
