using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLUSA.Interfaces
{
    public interface IEntidadeBase
    {
        ObjectId Id { get; set; }
    }
}
