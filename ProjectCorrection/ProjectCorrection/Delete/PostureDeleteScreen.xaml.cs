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

namespace ProjectCorrection.Delete
{
    /// <summary>
    /// 자세삭제화면을 나타내는 클래스입니다.
    /// </summary>
    public partial class PostureDeleteScreen : Page
    {
        private String id;                      //현재 유저 id
        private PostureDeleteProcess dProcess;  //자세삭제에 대한 처리를 하는 참조

        /// <summary>
        /// 이 클래스의 생성자를 나타냅니다. 생성하는 순간 자세삭제에 관한 프로세스가 진행됩니다.
        /// </summary>
        /// <param name="id">현재 유저의 id</param>
        public PostureDeleteScreen(string id)
        {
            InitializeComponent();
            this.id = id;
            dProcess = new PostureDeleteProcess(dataGridPostures, id);
            dProcess.ShowUserPosture();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)   //해당 버튼을 클릭했을 경우                                                              
        {                                                               //메인화면으로 돌아가는 버튼 이벤트 처리 메소드
            MessageBox.Show("메인화면으로 돌아갑니다.", "메인화면");
            this.NavigationService.Navigate(new MainScreen(id));
        }

        private void dataGridPostures_SelectionChanged(object sender, SelectionChangedEventArgs e)  
        {                                                               //데이터 그리드의 항목을 클릭 했을 경우 
            dProcess.DeletePostureProcess(sender);                      //해당하는 열의 자세를 삭제하는 버튼 이벤트 처리 메소드
            MessageBox.Show("해당 자세가 삭제되었습니다.", "삭제");
            dProcess.ShowUserPosture();    
        }
    }
}
