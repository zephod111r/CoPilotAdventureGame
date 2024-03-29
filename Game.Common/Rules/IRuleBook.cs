﻿using Game.Common.Character;
using Game.RuleBook.Character;

namespace Game.Common.Rules
{
    public interface IRuleBook
    {
        Task<NameDescription[]> GetRaces();
        Task<NameDescription[]> GetClasses(string race);
        Task<PlayerCharacter> CreateCharacter(NameDescription race, NameDescription clasz, bool withAvatar);
        Task<GameMap?> CreateMap(int width, int height);
        Task<string> GetGameMasterName();
        Task<string> GetImage(string query);
    }
}
