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

namespace ProjectCorrection.Select
{
    /// <summary>
    /// 자세선택을 처리하는 클래스입니다.
    /// </summary>
    public class PostureSelectionProcess
    {
        private DataGrid dataGrid1;     //모든 자세에 대한 DataSet을 넣을 DataGrid
        private byte[] imgBytes;        //선택된 이미지에 대한 byte 배열
        private string pName;           //선택된 자세이름
        private int pid;                //선택된 자세번호
        private int[] angles;
        /// <summary>
        /// 이 클래스에 대한 생성자입니다.
        /// </summary>
        /// <param name="dataGrid">모든 자세에 대한 DataSet을 넣을 DataGrid</param>
        public PostureSelectionProcess(DataGrid dataGrid)
        {
            dataGrid1 = dataGrid;
        }

        /// <summary>
        /// 자세 DataSet을 DataGrid에 넣는 메소드입니다.
        /// </summary>
        public void ShowAllPosture()
        {
            dataGrid1.DataContext = PostureDB.DataBindingTable();
        }

        /// <summary>
        /// 자세선택 프로세스를 진행하는 메소드입니다.
        /// </summary>
        /// <param name="sender"></param>
        public void SelectPostureProcess(object sender)
        {
            var grid = sender as DataGrid;
            if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
            {
                var rowView = grid.SelectedItem as DataRowView;
                if (rowView != null)
                {
                    DataRow row = rowView.Row;
                    pid = (int)row["pid"];
                    pName = (String)row["pname"];
                    imgBytes = PostureDB.GetImage(pid);
                }
            }
            angles = PostureDB.GetAngles(pid);
        }
        /// <summary>
        /// 선택된 이미지의 byte 배열을 반환하는 메소드입니다.
        /// </summary>
        /// <returns>해당 이미지의 byte 배열</returns>
        public byte[] SelectImgBytes()
        {
            return imgBytes;
        }

        /// <summary>
        /// 선택된 자세의 이름을 반환하는 메소드입니다.
        /// </summary>
        /// <returns>해당 자세의 이름</returns>
        public String SelectPName()
        {
            return pName;
        }

        public int[] SelectAngles()
        {
            return angles;
        }
    }
}
