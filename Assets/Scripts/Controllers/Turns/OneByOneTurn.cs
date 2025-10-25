using System.Collections.Generic;

namespace Unity3D
{
    public class OneByOneTurn : ITurn
    {
        private int _index;
        private readonly IReadOnlyList<Team> _teams;
        private readonly Team _white;

        public Team Current => _teams[_index];
        public Team White => _white;
        public Team Black => _white == Team.Player1 ? Team.Player2 : Team.Player1;

        public void Next()
        {
            _index = (_index + 1) % _teams.Count;
        }

        public OneByOneTurn(IReadOnlyList<Team> teams, UnitGameSettings settings)
        {
            _white = UnityEngine.Random.value < 0.5f ? Team.Player1 : Team.Player2;
            _teams = new List<Team> { _white, Black };
            _index = 0;

            Unit[] allUnits = UnityEngine.Object.FindObjectsOfType<Unit>();

            foreach (Unit unit in allUnits)
            {
                Team unitTeam = unit.IsWhite ? _white : Black;
                unit.SetTeam(unitTeam);
            }
        }
    }
}
