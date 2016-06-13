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
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using ProjectCorrection.Database;
using ProjectCorrection.Main;
using ProjectCorrection.Correction;
using Microsoft.Kinect;


namespace ProjectCorrection.Select
{
    /// <summary>
    /// 자세선택화면을 나타내는 클래스입니다.
    /// </summary>
    public partial class PostureSelectionScreen : Page
    {
        private String id;                              //현재 유저 id
        private PostureSelectionProcess sProcess;       //자세선택 프로세스를 진행하기 위한 참조
        private KinectSensor _KinectDevice;
        private Skeleton[] _FrameSkeletons;

        /// <summary>
        /// 이 클래스에 대한 생성자입니다. 생성되는 즉시 자세선택 프로세스가 진행됩니다.
        /// </summary>
        /// <param name="id"></param>
        public PostureSelectionScreen(string id)
        {
            InitializeComponent();
            this.id = id;
            sProcess = new PostureSelectionProcess(dataGridPostures);
            sProcess.ShowAllPosture();
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)       //해당 버튼을 눌렀을 경우 메인화면으로 돌아가는 버튼 이벤트 처리기
        {
            MessageBox.Show("메인화면으로 돌아갑니다.", "메인화면");
            this.NavigationService.Navigate(new MainScreen(id));
        }

        private void dataGridPostures_SelectionChanged(object sender, SelectionChangedEventArgs e)      //DataGrid를 클릭했을 때 해당 자세를 선택하는 버튼 이벤트 처리기
        {
            sProcess.SelectPostureProcess(sender);
            this.NavigationService.Navigate(new PostureCorrectionScreen(sProcess.SelectImgBytes(), sProcess.SelectPName(), id, sProcess.SelectAngles()));
        }
    }
}