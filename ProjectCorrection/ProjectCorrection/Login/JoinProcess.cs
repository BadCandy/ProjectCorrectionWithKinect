using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectCorrection.Database;

namespace ProjectCorrection.Login
{
    /// <summary>
    /// 회원가입을 위한 클래스입니다.
    /// </summary>
    public class JoinProcess
    {
        private string id;      //입력받은 유저 id
        private string pwd;     //입력받은 유저 패스워드

        /// <summary>
        /// 이 클래스에 대한 생성자입니다.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pwd"></param>
        public JoinProcess(string id, string pwd)
        {
            this.id = id; this.pwd = pwd;
        }

        /// <summary>
        /// 회원가입 할 유저 id의 중복검사를 하는 메소드입니다.
        /// </summary>
        /// <returns>중복이면 false, 중복이 아니면 true</returns>
        public bool IdCheck()
        {
            return MemberDB.idCheck(id);
        }

        /// <summary>
        /// 회원가입을 처리하는 메소드입니다.
        /// </summary>
        /// <returns>성공하면 true, 실패하면 false</returns>
        public bool Process()
        {
            if (IdCheck())
            {
                MemberDB.addUser(id, pwd);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
