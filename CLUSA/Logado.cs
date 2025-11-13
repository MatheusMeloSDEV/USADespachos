using MongoDB.Bson;

namespace CLUSA
{
    public class Logado
    {
        public ObjectId Id { get; set; }
        public bool admin = false;
        public bool log = false;
        public string Usuario = string.Empty;
    }
}
