using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using ProjectCorrection.Database;

namespace ProjectCorrection.Login
{
    /// <summary>
    /// 로그인 처리를 하기위한 클래스입니다.
    /// </summary>
    public class LoginProcess
    {
        private string id;      //입력받은 유저 id
        private string pwd;     //입력받은 유저 패스워드

        /// <summary>
        /// 이 클래스의 생성자를 나타냅니다.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pwd"></param>
        public LoginProcess(string id, string pwd)
        {
            this.id = id;   this.pwd = pwd;
        }
        
        /// <summary>
        /// 로그인 처리하는 메소드입니다.
        /// </summary>
        /// <returns>성공하면 유저 id 반환, 실패하면 null 반환</returns>
        public String Process()
        {
            if(LoginCheck())
            {
               return id;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 해당 유저 id와 패스워드가 유효한지 검사하는 메소드입니다.
        /// </summary>
        /// <returns></returns>
        public bool LoginCheck()
        {
            return MemberDB.checkLogin(id, pwd);
        }
    }
}