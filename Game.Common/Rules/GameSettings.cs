using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Common.Rules
{
    public class GameSettings
    {
        public string Theme { get; private set; }
        
        public GameSettings(string theme)
        {
            Theme = theme;
        }
    }
}
