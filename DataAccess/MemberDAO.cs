using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;

namespace DataAccess.Repository
{
    public class MemberDAO
    {
        private static MemberDAO instance = null;

        private static readonly object instanceLock = new object();
        
        public static MemberDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new MemberDAO();
                    }
                    return instance;
                }
            }
        }

        public IEnumerable<Member> GetMemberList()
        {
            var members = new List<Member>();
            try
            {
                using var context = new FStoreDBContext();
                members = context.Members.ToList();
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return members;
        }


        public Member GetMemberByID(int memberID)
        {
            Member member = null;
            try
            {
                using var context = new FStoreDBContext();
                member = context.Members.SingleOrDefault(m => m.MemberId == memberID);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return member;
        }
        public Member GetMemberByEmailAndPassword(string email, string password)
        {
            Member member = null;
            try
            {
                using var context = new FStoreDBContext();
                member = context.Members.SingleOrDefault(m => m.Email.Equals(email) & m.Password.Equals(password));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return member;
        }
        public Member GetMemberByEmail(string email)
        {
            Member member = null;
            try
            {
                using var context = new FStoreDBContext();
                member = context.Members.SingleOrDefault(m => m.Email.Equals(email));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return member;
        }
        public void AddNew(Member member)
        {       
            try
            {
                Member _member = GetMemberByID(member.MemberId);
                Member _member2 = GetMemberByEmail(member.Email);
                
                if (_member2 != null)
                {
                    throw new Exception("Email is already exist!!!");
                }

                if (_member == null)
                {
                    using var context = new FStoreDBContext();
                    context.Members.Add(member);
                    context.SaveChanges();
                } else
                {
                    throw new Exception("Member is already exist!!!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }       
        }

        public void Update(Member member)
        {
            try { 
                Member _member = GetMemberByID(member.MemberId);
                Member _member2 = GetMemberByEmail(member.Email);

                if (_member2 != null && member.MemberId != _member2.MemberId)
                {
                    throw new Exception("Email is already exist!!!");
                }
                if (member != null) {
                    using var context = new FStoreDBContext();
                    context.Members.Update(member);
                    context.SaveChanges();
                } else {
                    throw new Exception("Member does not already exists.");
                }
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }

        }

        public void Remove(int MemberID)
        {
            try {
                Member member = GetMemberByID(MemberID);
                if (member != null) {
                    using var context = new FStoreDBContext();
                    context.Members.Remove(member);
                    context.SaveChanges();
                } else {
                    throw new Exception("Member does not already exists.");
                }
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }
    }


}
