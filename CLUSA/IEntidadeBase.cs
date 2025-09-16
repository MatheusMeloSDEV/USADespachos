using MongoDB.Bson;

namespace CLUSA
{
    /// <summary>
    /// Garante que a entidade tenha as propriedades essenciais para o repositório base.
    /// </summary>
    public interface IEntidadeBase
    {
        ObjectId Id { get; set; }
        string Ref_USA { get; set; }
    }
}