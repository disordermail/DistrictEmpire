using DistrictEmpire.Domain;

namespace DistrictEmpire.Application
{
    public interface IGameRepository
    {
        GameState Load();
        void Save(GameState state);
    }
}
