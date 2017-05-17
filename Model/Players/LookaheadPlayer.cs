
using MiniChess.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MiniChess.Model.Players
{
    public class LookaheadPlayer : IPlayer
    {
        private int _depth;
        private GameState _state;

        public LookaheadPlayer(int depth)
        {
            _depth = depth;
        }

        public Move move(GameState state)
        {
            _state = state;
            Move m = StartNegaMax();
            return m;
        }
        public Move StartNegaMax() {
            List<Move> _movesTop = _state.GenerateAllLegalMoves();
            NegaMax.NegamaxRevert(_depth, _state, possibleMoves: _movesTop);
            int max = _movesTop.Max(x => x.Score);
            var move = _movesTop.First(x => x.Score == max);
            return move;
        }
    }
}
