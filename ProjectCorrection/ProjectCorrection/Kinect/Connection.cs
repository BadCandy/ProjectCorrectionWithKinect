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

namespace ProjectCorrection.Kinect
{
    /// <summary>
    /// 키넥트의 연결여부를 확인하기 위한 클래스입니다.
    /// </summary>
    public class Connection
    {
        private TextBlock textBlock1;   //연결 상태를 출력하기 위한 TextBlock
        private KinectSensor nui; /*키넥트를 사용하기 위해 선언한 클래스(nui를 통해 키넥트를 호출해 사용하게 된다*/

        /// <summary>
        /// 이 클래스에 대한 생성자입니다. 키넥트가 연결되어있으면 사용할 키넥트를 Load합니다.
        /// </summary>
        /// <param name="textBlock"></param>
        public Connection(TextBlock textBlock)
        {
            textBlock1 = textBlock;
            if(IsConnect())
                loadKinect();
        }

        private void Kinect_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            textBlock1.Text = nui.Status.ToString(); /*키넥트 상태가 변동되면 발생하는 이벤트로
                                                      * 화면이 TextBlock에 nui.Status의
                                                      * 내용을 출력하도록 했다. */

        }

        private void loadKinect()
        {
            nui = KinectSensor.KinectSensors[0]; //키넥트를 가져온다. 여기서 [0]은 키넥트의 순번이다.

            KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(Kinect_StatusChanged);
        }

        /// <summary>
        /// 키넥트의 연결여부를 나타내는 메소드입니다.
        /// </summary>
        /// <returns>연결되었으면 true, 아니면 false</returns>
        public bool IsConnect()
        {
            int ncount = KinectSensor.KinectSensors.Count;
            if (ncount == 0) return false;
            else return true;
        }

        /// <summary>
        /// 키넥트의 상태를 갱신하는 메소드입니다.
        /// </summary>
        public void RenewStatus()
        {
            if (!IsConnect())
            {
                textBlock1.Text = "연결된 키넥트 없음";
            }
            else
            {
                try
                {
                    String strTemp = "";
                    strTemp += KinectSensor.KinectSensors.Count.ToString("키넥트 개수 0개");
                    strTemp += "\n";
                    strTemp += nui.UniqueKinectId;      //부여된 키넥트 순서
                    strTemp += "\n";
                    strTemp += nui.DeviceConnectionId;     //해당 키넥트에 할당한 디바이스 연결 아이디
                    strTemp += "\n";
                    strTemp += nui.Status.ToString();       //키넥트의 연결상태를 알려준다.

                    textBlock1.Text = strTemp;      //이를 textBlock1에 출력하도록 함
                }
                catch (Exception e)
                {
                    MessageBox.Show("잠시후에 다시 시도해 주세요");
                }
            }
        }
    }
}
