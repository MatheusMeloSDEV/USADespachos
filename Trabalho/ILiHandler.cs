using CLUSA;

namespace Trabalho
{
    public interface ILiHandler
    {
        bool ContainsLi(string numeroLi);
        void AdicionarLi(LiInfo li);
        void AtualizarLi(string numeroLi, LiInfo liAtualizada);
        void RemoverLi(string numeroLi);
    }
}
