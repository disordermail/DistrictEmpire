using System.IO;
using DistrictEmpire.Application;
using DistrictEmpire.Domain;
using UnityEngine;

namespace DistrictEmpire.Infrastructure
{
    public sealed class JsonLocalGameRepository : IGameRepository
    {
        private readonly string path = Path.Combine(UnityEngine.Application.persistentDataPath, "district-empire-save.json");
        public GameState Load()
        {
            if (!File.Exists(path)) return null;
            try { return JsonUtility.FromJson<GameState>(File.ReadAllText(path)); }
            catch { return null; }
        }
        public void Save(GameState state) => File.WriteAllText(path, JsonUtility.ToJson(state, true));
    }
}
