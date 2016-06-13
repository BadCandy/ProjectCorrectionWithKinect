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
using Microsoft.Kinect;
using ProjectCorrection.Main;

namespace ProjectCorrection.Correction
{   
    /// <summary>
    /// 자세교정화면을 나타내는 클래스입니다.
    /// </summary>
    public partial class PostureCorrectionScreen : Page
    {
        private String id;          //현재 유저 id
        private byte[] selectImage; //선택된 이미지
        private String postureName; //자세이름 
        private PostureCorrectionProcess cProcess;    //자세교정을 처리하는 참조
        private int[] angles;
        /// <summary>
        /// 이 클래스의 생성자입니다. 생성하는 순간 자세교정에 대한 프로세스가 진행됩니다.
        /// </summary>
        /// <param name="selectImg">선택된 이미지의 byte배열입니다.</param>
        /// <param name="pName">선택된 자세의 이름입니다.</param>
        /// <param name="id">현재 유저 id입니다.</param>
        public PostureCorrectionScreen(byte[] selectImg, String pName, String id, int[] angles)
        {
            InitializeComponent();    
            this.id = id;
            selectImage = selectImg;
            postureName = pName;
            this.angles = angles;
            cProcess = new PostureCorrectionProcess(image1, image2, canvas1, selectImage, postureName, angles);
            cProcess.process();
            cProcess.ShowSelectImage();
        }



        private void Button_Click_2(object sender, RoutedEventArgs e)       //해당 버튼을 누르면 메인화면으로 돌아가는 이벤트 처리
        {     
            MessageBox.Show("메인화면으로 돌아갑니다.", "메인화면");
            this.NavigationService.Navigate(new MainScreen(id));    
        }
    }
}
