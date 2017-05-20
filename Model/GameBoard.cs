﻿
using MiniChess.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniChess.Model
{
    public class GameBoard
    {
        private char[,] _board = new char[Program.MAXROW, Program.MAXCOLUMN];
        public int WhiteScore { get; private set; }
        public int BlackScore { get; private set; }
        private char _lastOverride;

        public GameBoard(string s)
        {
            WhiteScore =12500;
            BlackScore =12500;
            MakeBoard(s);
        }

        public GameBoard(GameBoard gameBoard)
        {
            WhiteScore = gameBoard.WhiteScore;
            BlackScore = gameBoard.BlackScore;
            for (int a = 0; a < Program.MAXROW; a++)
            {
                for (int b = 0; b < Program.MAXCOLUMN; b++)
                {
                    _board[a, b] = gameBoard._board[a, b];
                }
            }
        }

        private void MakeBoard(string s)
        {
            int a = Program.MAXROW - 1;
            int b = Program.MAXCOLUMN - 1;

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] != '\n' && s[i] != '\r')
                {
                    _board[a, b] = s[i];
                    b--;
                    if (b == -1)
                    {
                        b = Program.MAXCOLUMN - 1;
                        a--;
                    }
                }
            }
        }

        public char Get(int row, int column)
        {
            if (row < 0 || row >= Program.MAXROW)
                throw new ArgumentOutOfRangeException("row is out of Range. must be between 0 and Program.MAXROW");
            if (column < 0 || column >= Program.MAXCOLUMN)
                throw new ArgumentOutOfRangeException("column is out of Range. must be between 0 and Program.MAXCOLUMN");
            return _board[row, column];
        }

        public void Move(Move m)
        {
            _lastOverride = _board[m.To.Row, m.To.Column];
            int score = ScoreForMove(m);
            Colors color = ColorOfPice(_lastOverride);
            if (color == Colors.WHITE)
            {
                WhiteScore -= score;
            }
            else if (color == Colors.BLACK)
            {
                BlackScore -= score;
            }
            char piece = Get(m.From.Row, m.From.Column);
            if ((m.To.Row == Program.MAXROW - 1 || m.To.Row == 0) && piece == 'p')
            {
                _board[m.To.Row, m.To.Column] = 'q';
            }
            else if ((m.To.Row == Program.MAXROW - 1 || m.To.Row == 0) && piece == 'P')
            {
                _board[m.To.Row, m.To.Column] = 'Q';
            }
            else
            {
                _board[m.To.Row, m.To.Column] = _board[m.From.Row, m.From.Column];
            }
            _board[m.From.Row, m.From.Column] = '.';
        }


        public void RevertMove(Move m)
        {
            char piece = Get(m.To.Row, m.To.Column);
            if ((m.To.Row == Program.MAXROW - 1 || m.To.Row == 0) && piece == 'q')
            {
                _board[m.From.Row, m.From.Column] = 'p';
            }
            else if ((m.From.Row == Program.MAXROW - 1 || m.From.Row == 0) && piece == 'Q')
            {
                _board[m.From.Row, m.From.Column] = 'P';
            }
            else
            {
                _board[m.From.Row, m.From.Column] = _board[m.To.Row, m.To.Column];
            }
            _board[m.To.Row, m.To.Column] = _lastOverride;
            int score = ScoreForMove(m);
            Colors color = ColorOfPice(_lastOverride);
            if (color == Colors.WHITE)
            {
                WhiteScore += score;
            }
            else if (color == Colors.BLACK)
            {
                BlackScore += score;
            }
        }

        private int ScoreForMove(Move m)
        {
            char cTo = Get(m.To.Row, m.To.Column);
            char cFrom = Get(m.From.Row, m.From.Column);
            char ch = char.ToLower(cTo);
            Pieces p = (Pieces)ch;
            int score = 0;
            switch (p)
            {
                case Pieces.King:
                    score = 10000;
                    break;
                case Pieces.Queen:
                    score = 900;
                    break;
                case Pieces.Bishop:
                case Pieces.Knight:
                    score = 300;
                    break;
                case Pieces.Rook:
                    score = 500;
                    break;
                case Pieces.Pawn:
                    score = 100;
                    break;
            }
            if ((m.To.Row == Program.MAXROW - 1 || m.To.Row == 0) && (Pieces)char.ToLower(cFrom) == Pieces.Pawn)
            {
                score += 800;
            }
            return score;
        }

        public List<Move> GetMoveList(Colors Turn)
        {
            List<Move> moves = new List<Move>();
            for (int i = 0; i < Program.MAXROW; i++)
            {
                for (int j = 0; j < Program.MAXCOLUMN; j++)
                {
                    if (Get(i, j) != '.' && ColorOfPice(Get(i, j)) == Turn)
                    {
                        moves.AddRange(MoveList(new Square(i, j)));
                    }
                }
            }

            foreach (Move m in moves)
            {
                m.Score = ScoreForMove(m);
            }
            moves = moves.OrderByDescending(x => x.Score).ToList();
            foreach (Move m in moves)
            {
                m.Score = 0;
            }

            return moves;
        }
        

        private List<Move> MoveList(Square square)
        {
            char c = Get(square.Row, square.Column);
            if (c == '.')
                throw new Exception("no piece given");

            List<Move> moves = new List<Move>();

            Pieces p = (Pieces)char.ToLower(c);
            switch (p)
            {
                case Pieces.King:
                case Pieces.Queen:
                    {
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            for (int dy = -1; dy <= 1; dy++)
                            {
                                if (dx == 0 && dy == 0)
                                    continue;
                                moves.AddRange(MoveScan(square, dx, dy, stopshort: p == Pieces.King));
                            }
                        }
                    }
                    break;
                case Pieces.Bishop:
                case Pieces.Rook:
                    {
                        int dx = 1;
                        int dy = 0;
                        for (int i = 1; i <= 4; i++)
                        {
                            moves.AddRange(MoveScan(square, dx, dy, p == Pieces.Rook, p == Pieces.Bishop));
                            SwapAndNegateSecond(ref dx, ref dy);
                        }
                        if (p == Pieces.Bishop)
                        {
                            dx = 1;
                            dy = 1;
                            for (int i = 1; i <= 4; i++)
                            {
                                moves.AddRange(MoveScan(square, dx, dy, true, false));
                                SwapAndNegateSecond(ref dx, ref dy);
                            }
                        }
                    }
                    break;
                case Pieces.Knight:
                    {
                        int dx = 1;
                        int dy = 2;
                        for (int i = 1; i <= 4; i++)
                        {
                            moves.AddRange(MoveScan(square, dx, dy, stopshort: true));
                            SwapAndNegateSecond(ref dx, ref dy);
                        }
                        dx = -1;
                        dy = 2;
                        for (int i = 1; i <= 4; i++)
                        {
                            moves.AddRange(MoveScan(square, dx, dy, stopshort: true));
                            SwapAndNegateSecond(ref dx, ref dy);
                        }
                    }
                    break;
                case Pieces.Pawn:
                    {
                        int dir = 1;
                        if (ColorOfPice(c) == Colors.BLACK)
                            dir = -1;

                        List<Move> m = MoveScan(square, -1, dir, stopshort: true);
                        if (m.Count == 1 && Get(m[0].To.Row, m[0].To.Column) != '.')
                            moves.AddRange(m);
                        m = MoveScan(square, 1, dir, stopshort: true);
                        if (m.Count == 1 && Get(m[0].To.Row, m[0].To.Column) != '.')
                            moves.AddRange(m);
                        moves.AddRange(MoveScan(square, 0, dir, false, true));
                    }
                    break;
            }
            return moves;
        }

        private List<Move> MoveScan(Square square, int dx, int dy, bool capture = true, bool stopshort = false)
        {
            int x = square.Column;
            int y = square.Row;
            Colors c = ColorOfPice(Get(y, x));
            List<Move> moves = new List<Move>();
            do
            {
                x = x + dx;
                y = y + dy;
                if (x >= Program.MAXCOLUMN || x < 0 || y >= Program.MAXROW || y < 0)
                    break;
                char current = Get(y, x);
                if (current != '.')
                {
                    if (ColorOfPice(current) == c)
                        break;
                    if (!capture)
                        break;
                    stopshort = true;
                }
                moves.Add(new Move(square.Row, square.Column, y, x));
            } while (!stopshort);
            return moves;
        }

        private Colors ColorOfPice(char c)
        {
            if (c >= 'A' && c <= 'Z')
                return Colors.WHITE;
            else if (c >= 'a' && c <= 'z')
                return Colors.BLACK;
            else
                return Colors.NONE;
        }

        private void SwapAndNegateSecond(ref int dx, ref int dy)
        {
            int temp = dx;
            dx = dy;
            dy = temp * -1;
        }

        public int CurrentScore(Colors Turn, Colors Won, bool draw)
        {
            if (draw)
            {
                return 0;
            }
            int scoreWhite = 0;
            int scoreBlack = 0;
            bool whiteKingAlive = false;
            bool blackKingAlive = false;
            for (int i = 0; i < Program.MAXROW; i++)
            {
                for (int j = 0; j < Program.MAXCOLUMN; j++)
                {
                    char c = Get(i, j);
                    Pieces piece = (Pieces)char.ToLower(c);
                    Colors color = ColorOfPice(c);
                    int temp = 0;
                    switch (piece)
                    {
                        case Pieces.King:
                            if (color == Colors.WHITE)
                            {
                                whiteKingAlive = true;
                            }
                            else if (color == Colors.BLACK)
                            {
                                blackKingAlive = true;
                            }
                            else
                            {
                                throw new Exception();
                            }
                            temp = 10000;
                            break;
                        case Pieces.Queen:
                            temp = 700;
                            break;
                        case Pieces.Bishop:
                        case Pieces.Knight:
                            temp = 300;
                            break;
                        case Pieces.Rook:
                            temp = 500;
                            break;
                        case Pieces.Pawn:
                            temp = 100;
                            break;
                    }
                    if (color == Colors.WHITE)
                        scoreWhite += temp;
                    else if (color == Colors.BLACK)
                        scoreBlack += temp;
                }
            }
            if (!blackKingAlive)
            {
                if (Turn == Colors.BLACK)
                {
                    return -10000;
                }
                else if (Turn == Colors.WHITE)
                {
                    return 10000;
                }
            }
            if (!whiteKingAlive)
            {
                if (Turn == Colors.WHITE)
                {
                    return -10000;
                }
                else if (Turn == Colors.BLACK)
                {
                    return 10000;
                }
            }
            if (Turn == Colors.BLACK)
            {
                return scoreBlack - scoreWhite;
            }
            else if (Turn == Colors.WHITE)
            {
                return scoreWhite - scoreBlack;
            }
            else
            {
                throw new Exception("shouldn't happen");
            }
        }

        public int CurrentScoreFast(Colors Turn, Colors Won, bool draw)
        {
            if (draw)
            {
                return 0;
            }
            bool whiteKingAlive = WhiteScore >= 10000;
            bool blackKingAlive = BlackScore >= 10000;
            if (!blackKingAlive)
            {
                if (Turn == Colors.BLACK)
                {
                    return -100000;
                }
                else if (Turn == Colors.WHITE)
                {
                    return 100000;
                }
            }
            if (!whiteKingAlive)
            {
                if (Turn == Colors.WHITE)
                {
                    return -100000;
                }
                else if (Turn == Colors.BLACK)
                {
                    return 100000;
                }
            }
            if (Turn == Colors.BLACK)
            {
                return BlackScore - WhiteScore;
            }
            else if (Turn == Colors.WHITE)
            {
                return WhiteScore - BlackScore;
            }
            else
            {
                throw new Exception("shouldn't happen");
            }
        }

        #region ToString Methods
        public override string ToString()
        {
            string s = "";
            int row = Program.MAXROW;
            for (int a = Program.MAXROW - 1; a >= 0; a--)
            {
                s += row-- + " ";
                for (int b = Program.MAXCOLUMN - 1; b >= 0; b--)
                {
                    s += Get(a, b);
                }
                s += "\n";
            }
            return s;
        }

        public string ToStringClean()
        {
            string s = "";
            for (int a = Program.MAXROW - 1; a >= 0; a--)
            {
                for (int b = Program.MAXCOLUMN - 1; b >= 0; b--)
                {
                    s += Get(a, b);
                }
            }
            return s;
        }

        public string ToStringReal()
        {
            string s = "";
            int row = 0;
            for (int a = 0; a < Program.MAXROW; a++)
            {
                s += row++ + " ";
                for (int b = 0; b < Program.MAXCOLUMN; b++)
                {
                    s += _board[a, b];
                }
                s += "\n";
            }
            return s;
        }
        #endregion //ToString Methods
    }
}
