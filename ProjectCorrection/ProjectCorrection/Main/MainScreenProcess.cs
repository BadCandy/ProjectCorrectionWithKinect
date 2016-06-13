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
using ProjectCorrection.Kinect;
using Microsoft.Kinect;

namespace ProjectCorrection.Main
{
    /// <summary>
    /// 메인화면을 처리하는 클래스입니다.
    /// </summary>
    public class MainScreenProcess
    {

        private String id;              //현재 유저 id
        private TextBlock textBlock1;   //키넥트 연결 여부를 출력하기 위한 TextBlock 뼈대
        private TextBlock idBlock1;     //현재 유저 id를 출력하기 위한 TextBlock 뼈대
        private Connection kConn;       //키넥트의 연결여부에 대한 처리를 하기위한 참조

        /// <summary>
        /// 이 클래스에 대한 생성자입니다. 생성하는 순간 Connection의 객체가 생성됩니다.
        /// </summary>
        /// <param name="id">현재 유저 id</param>
        /// <param name="textBlock"><c>MainScreen</c>에서 받아온 키넥트 연결 여부를 출력하기 위한 TextBlock 뼈대</param>
        /// <param name="idBlock"><c>MainScreen</c>에서 현재 유저 id를 출력하기 위한 TextBlock 뼈대</param>
        public MainScreenProcess(string id, TextBlock textBlock, TextBlock idBlock)
        {
            this.id = id;
            textBlock1 = textBlock;
            idBlock1 = idBlock;
            kConn = new Connection(textBlock1);
        }
        
        /// <summary>
        /// 키넥트가 현재 연결상태를 반환하는 메소드입니다.
        /// </summary>
        /// <returns>키넥트가 연결되었으면 true, 아니면 false</returns>
        public bool IsConnect()
        {
            return kConn.IsConnect();
        }

        /// <summary>
        /// 현재 유저의 id를 TextBlock에 쓰는 메소드
        /// </summary>
        public void currentUser()
        {
            idBlock1.Text = id;
        }

        /// <summary>
        /// 키넥트의 상태를 갱신하는 메소드
        /// </summary>
        public void renewStatus()
        {
            kConn.RenewStatus();
        }
    }
}
