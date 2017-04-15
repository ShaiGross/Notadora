using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotaDAL
{
    public interface NotaDbObject<T>
    {
        int Id { get; set; }
        bool DbCompare(T other);
    }
}
