
using MiniChess.Model;
using MiniChess.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniChess.Model
{
    public class GameState
    {
        private GameBoard _board;
        public int TurnCount { get; private set; }
        public Colors Turn { get; private set; }
        public Colors Self { get; private set; }
        public Colors Won { get; private set; }

        public GameState(string s = "0 W\nkqbnr\nppppp\n.....\n.....\nPPPPP\nRNBQK", Colors self = Colors.WHITE)
        {
            Won = Colors.NONE;
            Self = self;

            int indexOfNewLine = s.IndexOf('\n');
            string firstline = s.Substring(0, indexOfNewLine);
            string[] split = firstline.Split(' ');
            TurnCount = int.Parse(split[0])*2;
            Turn = (Colors)split[1][0];
            _board = new GameBoard(s.Substring(indexOfNewLine + 1));
        }
        public GameState(GameState state)
        {
            Won = state.Won;
            TurnCount = state.TurnCount;
            Turn = state.Turn;
            Self = state.Self;
            _board = new GameBoard(state._board);
        }

        public GameState(int turnCount, Colors turn, string board)
        {
            Won = Colors.NONE;
            TurnCount = turnCount;
            Turn = turn;
            _board = new GameBoard(board);
        }

        public void Move(Move m)
        {
            char c = char.ToLower(_board.Get(m.To.Row, m.To.Column));
            _board.Move(m);

            if ((Pieces)c == Pieces.King)
            {
                Won = Turn;
                Turn = Colors.NONE;
            }
            else if (TurnCount + 1 == Program.MAXTURNS)
            {
                Won = Colors.NONE;
                Turn = Colors.NONE;
            }
            else
            {
                if (Turn == Colors.WHITE)
                    Turn = Colors.BLACK;
                else if (Turn == Colors.BLACK)
                    Turn = Colors.WHITE;
            }
            TurnCount++;
        }

        public int StateScore()
        {
            if (Turn == Colors.NONE)
            {
                if (TurnCount % 2 == 0)
                {
                    return _board.CurrentScoreFast(Colors.WHITE, Won, Won == Colors.NONE);
                }
                else if (TurnCount % 2 == 1)
                {
                    return _board.CurrentScoreFast(Colors.BLACK, Won, Won == Colors.NONE);
                }
                else
                {
                    throw new Exception("shouldn't happen");
                }
            }
            else
            {
                return _board.CurrentScoreFast(Turn, Won, false);
            }
        }

        public List<Move> GenerateAllLegalMoves()
        {
            return _board.GetMoveList(Turn);
        }


        public void Move(string s)
        {
            Move(new Move(s.ToLower()));
        }

        public override string ToString()
        {
            string s = TurnCount + " " + (char)Turn + "\n" + "  abcde\n\n";
            s += _board.ToString();
            return s;
        }

        public string ToStringClean()
        {
            string s = TurnCount + " " + (char)Turn + " ";
            s += _board.ToStringClean();
            return s;
        }

        public string ToStringReal()
        {
            string s = TurnCount + " " + (char)Turn + "\n" + "  01234\n\n";
            s += _board.ToStringReal();
            return s;
        }


        public void RevertMove(Move m)
        {
            _board.RevertMove(m);
            char c = char.ToLower(_board.Get(m.To.Row, m.To.Column));
            TurnCount--;

            if ((Pieces)c == Pieces.King)
            {
                Turn = Won;
                Won = Colors.NONE;
            }
            else if (TurnCount + 1 == Program.MAXTURNS)
            {
                Won = Colors.NONE;
                Turn = Colors.BLACK;
            }
            else
            {
                if (Turn == Colors.WHITE)
                    Turn = Colors.BLACK;
                else if (Turn == Colors.BLACK)
                    Turn = Colors.WHITE;
            }
        }
    }
}
