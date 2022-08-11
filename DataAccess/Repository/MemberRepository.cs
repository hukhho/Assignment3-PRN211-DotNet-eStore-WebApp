using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;

namespace DataAccess.Repository
{
    public class MemberRepository : IMemberRepository
    {
        public IEnumerable<Member> GetMembers() => MemberDAO.Instance.GetMemberList();
        public Member GetMemberByEmailAndPassword(string email, string password) => MemberDAO.Instance.GetMemberByEmailAndPassword(email, password);
        public Member GetMemberByID(int memberID) => MemberDAO.Instance.GetMemberByID(memberID);
        public void InsertMember(Member member) => MemberDAO.Instance.AddNew(member);
        public void DeleteMember(int memberID) => MemberDAO.Instance.Remove(memberID);
        public void UpdateMember(Member member) => MemberDAO.Instance.Update(member);


    }
}
