using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;

namespace DAL.Interfaces
{
    public interface IMessageManager : IDisposable
    {
        void Create(Messages item);
        void Remove(Messages item);
        Task<Messages> Find(string id);
    }
}
