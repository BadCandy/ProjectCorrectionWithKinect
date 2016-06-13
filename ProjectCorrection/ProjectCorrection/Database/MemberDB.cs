using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ProjectCorrection.Database
{
    /// <summary>
    /// 데이터베이스의 유저에 관한 쿼리를 처리하는 클래스입니다. 
    /// </summary>
    public static class MemberDB
    {
        private static string strConn = "Server=168.131.152.120;Port=3306;Database=correction;Uid=root;Pwd=jsg115881;";
        private static MySqlConnection conn = new MySqlConnection(strConn);
        private static MySqlCommand com;
        private static string sql;

        /// <summary>
        /// 로그인 검사를 하는 메소드입니다.
        /// </summary>
        /// <param name="id">입력받은 유저 id</param>
        /// <param name="pwd">입력받은 비밀번호</param>
        /// <returns>유효하면 true, 유효하지 않으면 false</returns>
        public static bool checkLogin(string id, string pwd)
        {
            conn.Open();
            sql = "SELECT * FROM `member` WHERE `id` = '" + id + "' && `pwd` ='" + pwd + "'";
            com = new MySqlCommand(sql, conn);

            if (com.ExecuteScalar() == null)
            {
                conn.Close();
                return false;
            }
            else
            {
                conn.Close();
                return true;
            }
            
        }

        /// <summary>
        /// 아이디 중복검사를 하는 메소드입니다.
        /// </summary>
        /// <param name="id">입력받은 회원가입 유저 id</param>
        /// <returns>중복이 아니면 true, 중복이면 false</returns>
        public static bool idCheck(string id)
        {
            conn.Open();
            sql = "SELECT * FROM `member` WHERE `id` = '" + id + "'";
            com = new MySqlCommand(sql, conn);
            
            if (com.ExecuteScalar() == null)
            {
                conn.Close();
                return true;
            }
            else
            {
                conn.Close();
                return false;
            }
        }

        /// <summary>
        /// 회원가입을 할 때 데이터베이스에 회원을 추가하는 메소드입니다.
        /// </summary>
        /// <param name="id">입력받은 유저 id</param>
        /// <param name="pwd">입력받은 유저 패스워드</param>
        public static void addUser(string id, string pwd)
        {
            try
            {
                conn.Open();
                String sql = "INSERT INTO `member` (id, pwd) " +
                                "VALUES ( + '" + id + "', '" + pwd + "')";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                conn.Close();
            }
        }
    }
}
