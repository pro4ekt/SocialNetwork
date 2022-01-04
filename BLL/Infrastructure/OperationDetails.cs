using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Infrastructure
{
    /// <summary>
    /// Данный класс будет хранить информацию об успешности операции. Свойство Succedeed указывает,
    /// успешна ли операция, а свойства Message и Property будут хранить соответственно сообщение об
    /// ошибке и свойство, на котормо произошла ошибка.
    /// </summary>
    public class OperationDetails
    {
        public OperationDetails(bool succedeed, string message, string prop)
        {
            Succedeed = succedeed;
            Message = message;
            Property = prop;
        }
        public bool Succedeed { get; private set; }
        public string Message { get; private set; }
        public string Property { get; private set; }
    }
}
