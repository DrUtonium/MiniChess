
using MiniChess.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MiniChess.Model.Players
{
    public class LookaheadTimedPlayer : IPlayer
    {
        private GameState _state;
        private double _seconds;

        public LookaheadTimedPlayer(double seconds)
        {
            _seconds = seconds;
        }

        public Move move(GameState state)
        {
            _state = state;
            return StartNegaMaxIterative();
        }

        public Move StartNegaMaxIterative()
        {
            List<Move> movesLastDepth = null;
            int i = 1;
            try
            {
                DateTime end = DateTime.Now + TimeSpan.FromSeconds(_seconds);
                while (end > DateTime.Now)
                {
                    List<Move> temp = _state.GenerateAllLegalMoves();
                    NegaMax.NegamaxRevert(i, _state, end: end, iteration: 0, possibleMoves: temp);
                    movesLastDepth = temp;
                    int max2 = movesLastDepth.Max(x => x.Score);
                    i++;
                }
            }
            catch (TimeoutException)
            {

            }

            int max = movesLastDepth.Max(x => x.Score);
            var list = movesLastDepth.Where(x => x.Score == max);
            int index = Program.RANDOM.Next(list.Count());

            return list.ToList()[index];
        }

    }
}
