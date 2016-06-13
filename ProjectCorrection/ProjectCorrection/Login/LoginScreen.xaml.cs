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
using ProjectCorrection.Enrollment;
using ProjectCorrection.Correction;

namespace ProjectCorrection.Login
{
    /// <summary>
    /// 로그인화면을 나타내는 클래스입니다.
    /// </summary>
    public partial class LoginScreen : Page
    {
        private LoginProcess lProcess;  //로그인 처리를 하기위한 참조
        private JoinProcess jProcess;   //회원가입 처리를 하기위한 참조

        /// <summary>
        /// 이 클래스에 대한 생성자입니다.
        /// </summary>
        public LoginScreen()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)       //해당 버튼을 클릭하면 회원가입 프로세스를 진행하는 버튼 이벤트 처리기
        {
            string id = IDBox.Text;
            string pwd = PWBox.Password;

            //회원가입처리하기
            if (id == "" || pwd == "")
                MessageBox.Show("아이디와 비밀번호를 모두 입력해주세요.", "회원가입");
            else
            {
                jProcess = new JoinProcess(id, pwd);
                //중복체크
                if (jProcess.Process())
                {
                    MessageBox.Show("가입완료.", "회원가입");
                }
                else
                    MessageBox.Show("아이디 중복입니다.", "회원가입");
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)   //해당 버튼을 클릭하면 로그인 프로세스를 진행하는 버튼 이벤트 처리기
        {
            string id = IDBox.Text;
            string pwd = PWBox.Password;

            if (id == "" || pwd == "")
                MessageBox.Show("아이디와 비밀번호를 모두 입력해주세요.", "로그인");
            else
            {
                lProcess = new LoginProcess(id, pwd);
                string uId = lProcess.Process();
                if (uId != null)
                {
                    MessageBox.Show("로그인 되었습니다.", "로그인");
                    this.NavigationService.Navigate(new MainScreen(id));        
                }
                else
                    MessageBox.Show("아이디나 비밀번호가 맞지 않습니다.", "로그인");
            }
        }
    }
}
