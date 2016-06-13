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

namespace ProjectCorrection.Database
{
    /// <summary>
    /// 데이터베이스에서 자세에 관한 쿼리를 처리하는 클래스입니다.
    /// </summary>
    public static class PostureDB
    {
        private static string strConn = "Server=168.131.152.120;Port=3306;Database=correction;Uid=root;Pwd=jsg115881;";
        private static MySqlConnection conn = new MySqlConnection(strConn);
        private static MySqlCommand com;
        private static string sql;
        private static int[] angles;
        /// <summary>
        /// 이미지의 바이트배열을 저장하는 메소드입니다.
        /// </summary>
        /// <param name="postureName">입력받은 자세의 이름입니다.</param>
        /// <param name="id">입력받은 유저 id입니다.</param>
        /// <param name="image">입력받은 이미지 바이트 배열입니다.</param>
        public static void SaveImage(String postureName, String id, byte[] image, int[] angle)
        {
            sql = "INSERT INTO `posture` (`pname`, `user`, `image`, `angle0`, `angle1`, `angle2`, `angle3`, `angle4`, `angle5`, `angle6`"
             + ", `angle7`, `angle8`, `angle9`, `angle10`, `angle11`, `angle12`, `angle13`) VALUES(@pname, @user, @image, @angle0, @angle1, @angle2, @angle3, @angle4, @angle5, @angle6"
             + ", @angle7, @angle8, @angle9, @angle10, @angle11, @angle12, @angle13)";
            com = new MySqlCommand(sql, conn);

            com.Parameters.Add("@pname", MySqlDbType.VarChar, 11);
            com.Parameters.Add("@user", MySqlDbType.VarChar, 45);
            com.Parameters.Add("@image", MySqlDbType.Blob);
            com.Parameters.Add("@angle0", MySqlDbType.Int32, 11);
            com.Parameters.Add("@angle1", MySqlDbType.Int32, 11);
            com.Parameters.Add("@angle2", MySqlDbType.Int32, 11);
            com.Parameters.Add("@angle3", MySqlDbType.Int32, 11);
            com.Parameters.Add("@angle4", MySqlDbType.Int32, 11);
            com.Parameters.Add("@angle5", MySqlDbType.Int32, 11);
            com.Parameters.Add("@angle6", MySqlDbType.Int32, 11);
            com.Parameters.Add("@angle7", MySqlDbType.Int32, 11);
            com.Parameters.Add("@angle8", MySqlDbType.Int32, 11);
            com.Parameters.Add("@angle9", MySqlDbType.Int32, 11);
            com.Parameters.Add("@angle10", MySqlDbType.Int32, 11);
            com.Parameters.Add("@angle11", MySqlDbType.Int32, 11);
            com.Parameters.Add("@angle12", MySqlDbType.Int32, 11);
            com.Parameters.Add("@angle13", MySqlDbType.Int32, 11);


            com.Parameters["@pname"].Value = postureName;
            com.Parameters["@user"].Value = id;
            com.Parameters["@image"].Value = image;

            com.Parameters["@angle0"].Value = angle[0];
            com.Parameters["@angle1"].Value = angle[1];
            com.Parameters["@angle2"].Value = angle[2];
            com.Parameters["@angle3"].Value = angle[3];
            com.Parameters["@angle4"].Value = angle[4];
            com.Parameters["@angle5"].Value = angle[5];
            com.Parameters["@angle6"].Value = angle[6];
            com.Parameters["@angle7"].Value = angle[7];
            com.Parameters["@angle8"].Value = angle[8];
            com.Parameters["@angle9"].Value = angle[9];
            com.Parameters["@angle10"].Value = angle[10];
            com.Parameters["@angle11"].Value = angle[11];
            com.Parameters["@angle12"].Value = angle[12];
            com.Parameters["@angle13"].Value = angle[13];

            conn.Open();
            int RowsAffected = com.ExecuteNonQuery();
            conn.Close();
        }

        /// <summary>
        /// 데이터바인딩을 이용하여 자세 테이블에 대한 DataSet을 받아오는 메소드입니다.
        /// </summary>
        /// <returns>자세 테이블의 Dataset</returns>
        public static DataSet DataBindingTable()
        {
            conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT `pid`, `user`, `pname` FROM `posture`", conn);
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adp.Fill(ds, "LoadDataBinding");
                return ds;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
            finally
            {
                conn.Close();
            }
        }
        
        /// <summary>
        /// 데이터바인딩을 이용하여 자세 테이블에서 해당 유저에 대한 DataSet을 받아오는 메소드입니다.
        /// </summary>
        /// <param name="user">입력된 user id</param>
        /// <returns>자세 테이블의 해당하는 Dataset</returns>
        public static DataSet DataBindingTable(string user)
        {
            conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT `pid`, `user`, `pname` FROM `posture` WHERE `user` = '" + user + "'", conn);
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adp.Fill(ds, "LoadDataBinding");
                return ds;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 자세 테이블에서 pid에 해당하는 튜플을 삭제하는 메소드입니다. 
        /// </summary>
        /// <param name="pid">입력받은 자세 번호(pid)</param>
        public static void DeletePosture(int pid)
        {
            try
            {
                conn.Open();
                String sql = "DELETE FROM `posture` WHERE `pid` = " + pid;

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 자세 테이블에서 pid에 해당하는 이미지 바이트 배열을 가져오는 메소드입니다.
        /// </summary>
        /// <param name="pid">입력받은 자세 번호(pid)</param>
        /// <returns>해당 이미지의 byte[] 배열</returns>
        public static byte[] GetImage(int pid)
        {
            conn.Open();
            sql = "SELECT `image` FROM `posture` WHERE `pid` = " + pid;
            com = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = com.ExecuteReader();
           
            if (rdr.Read())
            {
                byte[] bytes = (byte[])rdr["image"];
                conn.Close();
//                rdr.Close();
                return bytes;
            }
            return null;
        }

        /// <summary>
        /// 자세 테이블에서 pid에 해당하는 자세의 이름을 가져오는 메소드입니다.
        /// </summary>
        /// <param name="pid">입력받은 자세 번호(pid)</param>
        /// <returns>해당 자세의 이름</returns>
        public static String GetPostureName(String pid)
        {

            conn.Open();
            sql = "SELECT `pname` FROM `posture` WHERE `pid` = " + pid;
            com = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = com.ExecuteReader();
 //           rdr.Close();
            conn.Close();
            if (rdr.Read())
            {
                return rdr.GetString(0);
            }
            return null;
        }

        public static int[] GetAngles(int pid)
        {
            conn.Open();
            sql = "SELECT `angle0`, `angle1`, `angle2`, `angle3`, `angle4`, `angle5`, `angle6`, "
                + "`angle7`, `angle8`, `angle9`,`angle10`, `angle11`,`angle12`, `angle13` FROM `posture` WHERE `pid` = " + pid;
            com = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = com.ExecuteReader();
            
            if (rdr.Read())
            {
                angles = new int[] { rdr.GetInt32(0), rdr.GetInt32(1), rdr.GetInt32(2), rdr.GetInt32(3), rdr.GetInt32(4), rdr.GetInt32(5), rdr.GetInt32(6)
                                   , rdr.GetInt32(7), rdr.GetInt32(8), rdr.GetInt32(9), rdr.GetInt32(10), rdr.GetInt32(11), rdr.GetInt32(12), rdr.GetInt32(13)};
                conn.Close();
  //              rdr.Close();
                return angles;
            }
            return null;
        }
    }
}
