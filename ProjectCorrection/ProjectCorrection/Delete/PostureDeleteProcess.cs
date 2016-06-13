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

namespace ProjectCorrection.Delete
{
    /// <summary>
    /// 해당 유저에 대한 자세삭제에 대한 처리를 하는 클래스입니다.
    /// </summary>
    public class PostureDeleteProcess
    {
        private DataGrid dataGrid1;     //PostureDeleteScreen의 dataGrid
        private byte[] imgBytes;        //삭제할 이미지의 byte 배열
        private string pName;           //삭제할 자세의 이름
        private int pid;                //삭제할 자세의 자세번호
        private String id;              //현재 유저 id

        /// <summary>
        /// 이 클래스에 대한 생성자를 나타냅니다.
        /// </summary>
        /// <param name="dataGrid">PostureDeleteScreen의 dataGrid</param>
        /// <param name="id">현재 유저 id</param>
        public PostureDeleteProcess(DataGrid dataGrid, String id)
        {
            dataGrid1 = dataGrid;
            this.id = id;
        }

        /// <summary>
        /// 현재 유저에 해당하는 자세만을 보여주는 메소드입니다.
        /// </summary>
        public void ShowUserPosture()
        {
            dataGrid1.DataContext = PostureDB.DataBindingTable(id);
        }

        /// <summary>
        /// 자세삭제에 대한 프로세스를 진행하는 메소드입니다.
        /// </summary>
        /// <param name="sender">object타입의 이벤트 발생에 해당하는 객체</param>
        public void DeletePostureProcess(object sender)
        {
            var grid = sender as DataGrid;
            if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
            {
                var rowView = grid.SelectedItem as DataRowView;
                if (rowView != null)
                {
                    DataRow row = rowView.Row;
                    pid = (int)row["pid"];
                }
            }
            PostureDB.DeletePosture(pid);
        }

        /// <summary>
        /// 선택된 자세에 대한 이미지 byte 배열을 반환하는 메소드입니다.
        /// </summary>
        /// <returns>해당 이미지 byte 배열</returns>
        public byte[] SelectImgBytes()
        {
            return imgBytes;
        }

        /// <summary>
        /// 선택된 자세에 대한 자세이름을 반환하는 메소드입니다.
        /// </summary>
        /// <returns>해당 이미지의 자세 이름</returns>
        public String SelectPName()
        {
            return pName;
        }
    }
}
