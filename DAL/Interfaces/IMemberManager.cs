using System;
using System.Threading.Tasks;
using DAL.Entities;

namespace DAL.Interfaces
{
    public interface IMemberManager : IDisposable
    {
        void Create(MemberProfile item);
    }
}
