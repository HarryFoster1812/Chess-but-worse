using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chess
{
    //
    class Piece
    {
        public List<int> legalMoves;
        public int colour;
        public int value;
        public int loc;
        public bool ispinned;
        public List<int> pinnedlegal;
        public BitmapImage source;
        public virtual void genMoves(Board board) { }
        public virtual void genPseudoMoves(Board board) { }
        public List<int> genPinnedLegalMoves(List<int> psudeolegal, List<int> pinnedlegal)
        {
            List<int> templegal = new List<int>();

            foreach (int i in psudeolegal)
            {
                if (pinnedlegal.Contains(i))
                {
                    templegal.Add(i);
                }
            }

            return templegal;
        }
    }

    class Pawn : Piece
    { // need to add enpassant functionality (no check functionality)
        public bool hasMoved = false;
        public bool enpassant = false;

        public Pawn(int colour, int loc)
        {

            this.colour = colour;
            this.pinnedlegal = new List<int>();
            this.loc = loc;
            this.ispinned = false;
            this.legalMoves = new List<int>();
            switch (this.colour)
            {
                case -1:
                    this.source = new BitmapImage(new Uri(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Sprites\\" + "bP" + ".png"));
                    this.value = -1;

                    break;
                case 1:
                    this.source = new BitmapImage(new Uri(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Sprites\\" + "wP" + ".png"));
                    this.value = 1;
                    break;
            }
        }
        public override void genMoves(Board board)
        {
            legalMoves.Clear();
            if (this.hasMoved == true)
            {
                if (board.board[this.loc + 8 * this.colour] == null)
                {
                    this.legalMoves.Add(this.loc + 8 * this.colour);
                }

                if (board.board[this.loc + 7 * this.colour] != null && board.board[this.loc + 7 * this.colour].colour == (this.colour * -1) && (this.loc + 7 * this.colour) >= 0 && (this.loc + 7 * this.colour) / 8 == (this.loc / 8 + 1 * this.colour))
                {
                    this.legalMoves.Add(this.loc + 7 * this.colour);
                }

                if ((this.loc + 9 * this.colour) <= 63 && board.board[this.loc + 9 * this.colour] != null && board.board[this.loc + 9 * this.colour].colour == (this.colour * -1) && (this.loc + 9 * this.colour) >= 0 && (this.loc + 9 * this.colour) / 8 == (this.loc / 8 + 1 * this.colour))
                {
                    this.legalMoves.Add(this.loc + 9 * this.colour);
                }

                if (board.board[this.loc + 1] != null && board.board[this.loc + 1] is Pawn && board.enpassanttemp == board.board[this.loc + 1].loc && (this.loc + 1) / 8 == (this.loc / 8))
                {
                    if (this.colour == 1)
                    {
                        this.legalMoves.Add(this.loc + 9);
                    }
                    else this.legalMoves.Add(this.loc - 7);
                }

                if (board.board[this.loc - 1] != null && board.board[this.loc - 1] is Pawn && board.enpassanttemp == board.board[this.loc - 1].loc && (this.loc - 1) / 8 == (this.loc / 8))
                {
                    if (this.colour == 1)
                    {
                        this.legalMoves.Add(this.loc + 7);
                    }
                    else this.legalMoves.Add(this.loc - 9);
                }

            }

            else
            {
                if (board.board[this.loc + 8 * this.colour] == null)
                {
                    if (board.board[this.loc + 16 * this.colour] == null)
                    {
                        legalMoves.Add(this.loc + 16 * this.colour);
                    }

                    legalMoves.Add(this.loc + 8 * this.colour);


                }

                if (board.board[this.loc + 7 * this.colour] != null && board.board[this.loc + 7 * this.colour].colour == (this.colour * -1) && (this.loc + 7 * this.colour) >= 0)
                {
                    this.legalMoves.Add(this.loc + 7 * this.colour);
                }

                if (board.board[this.loc + 9 * this.colour] != null && board.board[this.loc + 9 * this.colour].colour == (this.colour * -1) && (this.loc + 9 * this.colour) >= 0)
                {
                    this.legalMoves.Add(this.loc + 9 * this.colour);
                }
            }
            if (ispinned == true)
            {
                legalMoves = genPinnedLegalMoves(legalMoves, pinnedlegal);
            }
            if (board.check == true)
            {

                legalMoves = genPinnedLegalMoves(legalMoves, board.blockcheck);
            }
        }

        public override void genPseudoMoves(Board board)
        {
            legalMoves.Clear();
            if ((this.loc + 7 * this.colour) >= 0 && (this.loc + 7 * this.colour) / 8 == (this.loc / 8 + 1 * this.colour))
            {
                this.legalMoves.Add(this.loc + 7 * this.colour);
            }

            if ((this.loc + 9 * this.colour) >= 0 && (this.loc + 9 * this.colour) / 8 == (this.loc / 8 + 1 * this.colour))
            {
                this.legalMoves.Add(this.loc + 9 * this.colour);
            }

        }
    }

    class Knight : Piece
    { // fully works

        public Knight(int colour, int loc)
        {
            this.colour = colour;
            this.ispinned = false;
            this.pinnedlegal = new List<int>();
            this.loc = loc;
            this.legalMoves = new List<int>();
            switch (this.colour)
            {
                case -1:
                    this.source = new BitmapImage(new Uri(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Sprites\\" + "bN" + ".png"));
                    this.value = -3;
                    break;
                case 1:
                    this.source = new BitmapImage(new Uri(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Sprites\\" + "wN" + ".png"));
                    this.value = 3;
                    break;
            }
        }
        public override void genMoves(Board board)
        {
            legalMoves.Clear();


            if (this.ispinned == false)
            {
                int[] possible = { 10, 6, 15, 17, -10, -17, -15, -6 };

                bool flag = false;
                int[] temp = { 0, 1, 6, 7 };
                if (temp.Contains(loc % 8) == true)
                {
                    flag = true;
                }

                for (int i = 0; i < possible.Length; i++)
                {
                    int temp1 = loc + possible[i];
                    if ((loc + possible[i]) <= 63 && (loc + possible[i]) >= 0 && (board.board[loc + possible[i]] == null || board.board[loc + possible[i]].colour == this.colour * -1))
                    {
                        if (flag == true)
                        {
                            if (temp.Contains((loc + possible[i]) % 8) == true)
                            {
                                if (loc % 8 == 0 || loc % 8 == 1)
                                {
                                    if ((loc + possible[i]) % 8 > 3) { }

                                    else
                                    {
                                        legalMoves.Add(loc + possible[i]);
                                    }
                                }

                                else if ((loc + possible[i]) % 8 < 4) { }

                                else
                                {
                                    legalMoves.Add(loc + possible[i]);
                                }

                            }
                            else
                            {
                                legalMoves.Add(loc + possible[i]);
                            }
                        }
                        else
                        {
                            legalMoves.Add(loc + possible[i]);
                        }
                    }
                }
            }
            if (board.check == true)
            {
                {
                    legalMoves = genPinnedLegalMoves(legalMoves, board.blockcheck);
                }


            }
        }

        public override void genPseudoMoves(Board board)
        {
            legalMoves.Clear();
            int[] possible = { 10, 6, 15, 17, -10, -17, -15, -6 };

            bool flag = false;
            int[] temp = { 0, 1, 6, 7 };
            if (temp.Contains(loc % 8) == true)
            {
                flag = true;
            }

            for (int i = 0; i < possible.Length; i++)
            {
                int temp1 = loc + possible[i];
                if ((loc + possible[i]) <= 63 && (loc + possible[i]) >= 0)
                {
                    if (flag == true)
                    {
                        if (temp.Contains((loc + possible[i]) % 8) == true)
                        {
                            if (loc % 8 == 0 || loc % 8 == 1)
                            {
                                if ((loc + possible[i]) % 8 > 3) { }

                                else
                                {
                                    legalMoves.Add(loc + possible[i]);
                                }
                            }

                            else if ((loc + possible[i]) % 8 < 4) { }

                            else
                            {
                                legalMoves.Add(loc + possible[i]);
                            }

                        }
                        else
                        {
                            legalMoves.Add(loc + possible[i]);
                        }
                    }
                    else
                    {
                        legalMoves.Add(loc + possible[i]);
                    }
                }
            }
        }

    }

    class Bishop : Piece
    {
        public Bishop(int colour, int loc)
        {
            this.colour = colour;
            this.pinnedlegal = new List<int>();
            this.ispinned = false;
            this.loc = loc;
            this.legalMoves = new List<int>();
            switch (this.colour)
            {
                case -1:
                    this.source = new BitmapImage(new Uri(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Sprites\\" + "bB" + ".png"));
                    this.value = -3;
                    break;
                case 1:
                    this.source = new BitmapImage(new Uri(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Sprites\\" + "wB" + ".png"));
                    this.value = 3;
                    break;
            }
        }
        public override void genMoves(Board board)
        {
            legalMoves.Clear();
            int[] presets;
            if (loc % 8 == 0)
            {
                presets = new int[] { 9, -7 };
            }
            else if (loc % 8 == 7)
            {
                presets = new int[] { -9, 7 };
            }
            else
            {
                presets = new int[] { 9, -9, 7, -7 };

            }

            foreach (int i in presets)
            {
                int possible = this.loc;
                int counter = 1;

                while ((possible + i * counter) >= 0 && (possible + i * counter) <= 63 && (board.board[(possible + i * counter)] == null || board.board[(possible + i * counter)].colour == this.colour * -1))
                {
                    if ((possible + i * counter) % 8 == 0 || (possible + i * counter) % 8 == 7 || (board.board[(possible + i * counter)] != null && board.board[(possible + i * counter)].colour == this.colour * -1 && !(board.board[(possible + i * counter)] is King)))
                    {
                        this.legalMoves.Add((possible + i * counter));
                        break;
                    }
                    this.legalMoves.Add((possible + i * counter));
                    counter++;
                }
            }
            if (ispinned == true)
            {
                legalMoves = genPinnedLegalMoves(legalMoves, pinnedlegal);
            }
            if (board.check == true)
            {

                legalMoves = genPinnedLegalMoves(legalMoves, board.blockcheck);
            }
        }

        public override void genPseudoMoves(Board board)
        {
            legalMoves.Clear();
            int[] presets;
            if (loc % 8 == 0)
            {
                presets = new int[] { 9, -7 };
            }
            else if (loc % 8 == 7)
            {
                presets = new int[] { -9, 7 };
            }
            else
            {
                presets = new int[] { 9, -9, 7, -7 };

            }

            foreach (int i in presets)
            {
                int possible = this.loc;
                int counter = 1;

                while ((possible + i * counter) >= 0 && (possible + i * counter) <= 63)
                {
                    if ((possible + i * counter) % 8 == 0 || (possible + i * counter) % 8 == 7 || (board.board[(possible + i * counter)] != null && !(board.board[(possible + i * counter)] is King)))
                    {
                        this.legalMoves.Add((possible + i * counter));
                        break;
                    }
                    this.legalMoves.Add((possible + i * counter));
                    counter++;
                }
            }
        }
    }

    class Rook : Piece
    { // fully works (no check functionality)
        public bool castle = true;
        public Rook(int colour, int loc)
        {
            this.colour = colour;
            this.pinnedlegal = new List<int>();
            this.ispinned = false;
            this.loc = loc;
            this.legalMoves = new List<int>();
            switch (this.colour)
            {
                case -1:
                    this.source = new BitmapImage(new Uri(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Sprites\\" + "bR" + ".png"));
                    this.value = -5;
                    break;
                case 1:
                    this.source = new BitmapImage(new Uri(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Sprites\\" + "wR" + ".png"));
                    this.value = 5;
                    break;
            }

        }
        public override void genMoves(Board board)
        {
            legalMoves.Clear();

            int[] presets = { 1, -1, 8, -8 };
            for (int i = 0; i < 2; i++)
            {
                int counter = 1;
                int possible = this.loc;
                int j = presets[i + 2];
                while ((possible + j * counter) >= 0 && (possible + j * counter) <= 63 && (board.board[(possible + j * counter)] == null || board.board[(possible + j * counter)].colour == this.colour * -1))
                {
                    if (board.board[(possible + j * counter)] != null && board.board[(possible + j * counter)].colour == this.colour * -1 && !(board.board[(possible + j * counter)] is King))
                    {

                        this.legalMoves.Add((possible + j * counter));
                        break;
                    }


                    this.legalMoves.Add((possible + j * counter));
                    counter++;
                }
            }


            if (this.loc % 8 == 7)
            {
                int counter = 1;
                int possible = this.loc;
                int j = presets[1];
                while ((possible + j * counter) >= 0 && (possible + j * counter) <= 63 && (board.board[(possible + j * counter)] == null || board.board[(possible + j * counter)].colour == this.colour * -1))
                {
                    if ((possible + j * counter) % 8 == 0 || (board.board[(possible + j * counter)] != null && board.board[(possible + j * counter)].colour == this.colour * -1 && !(board.board[(possible + j * counter)] is King)))
                    {
                        this.legalMoves.Add((possible + j * counter));
                        break;
                    }
                    this.legalMoves.Add((possible + j * counter));
                    counter++;
                }
            }

            else if (this.loc % 8 == 0)
            {
                int counter = 1;
                int possible = this.loc;
                int j = presets[0];
                while ((possible + j * counter) >= 0 && (possible + j * counter) <= 63 && (board.board[(possible + j * counter)] == null || board.board[(possible + j * counter)].colour == this.colour * -1))
                {
                    if ((possible + j * counter) % 8 == 7 || (board.board[(possible + j * counter)] != null && board.board[(possible + j * counter)].colour == this.colour * -1 && !(board.board[(possible + j * counter)] is King)))
                    {
                        this.legalMoves.Add((possible + j * counter));
                        break;
                    }
                    this.legalMoves.Add((possible + j * counter));
                    counter++;
                }
            }

            else
            {
                for (int i = 0; i < 2; i++)
                {
                    int counter = 1;
                    int possible = this.loc;
                    int j = presets[i];

                    while ((possible + j * counter) >= 0 && (possible + j * counter) <= 63 && (board.board[(possible + j * counter)] == null || board.board[(possible + j * counter)].colour == this.colour * -1))
                    {
                        int move = (possible + j * counter);
                        if ((possible + j * counter) % 8 == 0 || (possible + j * counter) % 8 == 7 || (board.board[(possible + j * counter)] != null && board.board[(possible + j * counter)].colour == this.colour * -1 && !(board.board[(possible + j * counter)] is King)))
                        {
                            this.legalMoves.Add((possible + j * counter));
                            break;
                        }

                        this.legalMoves.Add((possible + j * counter));
                        counter++;
                    }

                }
            }

            if (ispinned == true)
            {
                legalMoves = genPinnedLegalMoves(legalMoves, pinnedlegal);
            }

            if (board.check == true)
            {
                legalMoves = genPinnedLegalMoves(legalMoves, board.blockcheck);
            }
        }

        public override void genPseudoMoves(Board board)
        {
            legalMoves.Clear();

            int[] presets = { 1, -1, 8, -8 };
            for (int i = 0; i < 2; i++)
            {
                int counter = 1;
                int possible = this.loc;
                int j = presets[i + 2];
                while ((possible + j * counter) >= 0 && (possible + j * counter) <= 63)
                {
                    if (board.board[(possible + j * counter)] != null && !(board.board[(possible + j * counter)] is King))
                    {

                        this.legalMoves.Add((possible + j * counter));
                        break;
                    }


                    this.legalMoves.Add((possible + j * counter));
                    counter++;
                }
            }


            if (this.loc % 8 == 7)
            {
                int counter = 1;
                int possible = this.loc;
                int j = presets[1];
                while ((possible + j * counter) >= 0 && (possible + j * counter) <= 63)
                {
                    if ((possible + j * counter) % 8 == 0 || (board.board[(possible + j * counter)] != null && !(board.board[(possible + j * counter)] is King)))
                    {
                        this.legalMoves.Add((possible + j * counter));
                        break;
                    }
                    this.legalMoves.Add((possible + j * counter));
                    counter++;
                }
            }

            else if (this.loc % 8 == 0)
            {
                int counter = 1;
                int possible = this.loc;
                int j = presets[0];
                while ((possible + j * counter) >= 0 && (possible + j * counter) <= 63)
                {
                    if ((possible + j * counter) % 8 == 7 || (board.board[(possible + j * counter)] != null && !(board.board[(possible + j * counter)] is King)))
                    {
                        this.legalMoves.Add((possible + j * counter));
                        break;
                    }
                    this.legalMoves.Add((possible + j * counter));
                    counter++;
                }
            }

            else
            {
                for (int i = 0; i < 2; i++)
                {
                    int counter = 1;
                    int possible = this.loc;
                    int j = presets[i];

                    while ((possible + j * counter) >= 0 && (possible + j * counter) <= 63)
                    {
                        int move = (possible + j * counter);
                        if ((possible + j * counter) % 8 == 0 || (possible + j * counter) % 8 == 7 || (board.board[(possible + j * counter)] != null && !(board.board[(possible + j * counter)] is King)))
                        {
                            this.legalMoves.Add((possible + j * counter));
                            break;
                        }

                        this.legalMoves.Add((possible + j * counter));
                        counter++;
                    }

                }
            }
        }
    }

    class Queen : Piece // fully works (no check functionality)
    {
        public Queen(int colour, int loc)
        {
            this.colour = colour;
            this.ispinned = false;
            this.pinnedlegal = new List<int>();
            this.loc = loc;
            this.legalMoves = new List<int>();
            switch (this.colour)
            {
                case -1:
                    this.source = new BitmapImage(new Uri(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Sprites\\" + "bQ" + ".png"));
                    this.value = -9;

                    break;
                case 1:
                    this.source = new BitmapImage(new Uri(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Sprites\\" + "wQ" + ".png"));
                    this.value = 9;

                    break;
            }
        }

        public override void genMoves(Board board)
        {
            legalMoves.Clear();

            int[] presets;
            if (loc % 8 == 0)
            {
                presets = new int[] { 9, -7, -200, -200 };
            }
            else if (loc % 8 == 7)
            {
                presets = new int[] { -9, 7, -200, -200 };
            }
            else
            {
                presets = new int[] { 9, -9, 7, -7 };

            }

            foreach (int i in presets)
            {
                int possible = this.loc;
                int counter = 1;

                while ((possible + i * counter) >= 0 && (possible + i * counter) <= 63 && (board.board[(possible + i * counter)] == null || board.board[(possible + i * counter)].colour == this.colour * -1))
                {
                    if ((possible + i * counter) % 8 == 0 || (possible + i * counter) % 8 == 7 || (board.board[(possible + i * counter)] != null && board.board[(possible + i * counter)].colour == this.colour * -1 && !(board.board[(possible + i * counter)] is King)))
                    {
                        this.legalMoves.Add((possible + i * counter));
                        break;
                    }
                    this.legalMoves.Add((possible + i * counter));
                    counter++;
                }
            }

            presets[0] = 1;
            presets[1] = -1;
            presets[2] = 8;
            presets[3] = -8;
            for (int i = 0; i < 2; i++)
            {
                int counter = 1;
                int possible = this.loc;
                int j = presets[i + 2];
                while ((possible + j * counter) >= 0 && (possible + j * counter) <= 63 && (board.board[(possible + j * counter)] == null || board.board[(possible + j * counter)].colour == this.colour * -1))
                {
                    if (board.board[(possible + j * counter)] != null && board.board[(possible + j * counter)].colour == this.colour * -1 && !(board.board[(possible + j * counter)] is King))
                    {
                        this.legalMoves.Add((possible + j * counter));
                        break;
                    }
                    this.legalMoves.Add((possible + j * counter));
                    counter++;
                }

            }

            if (this.loc % 8 == 7)
            {
                int counter = 1;
                int possible = this.loc;
                int j = presets[1];
                while ((possible + j * counter) >= 0 && (possible + j * counter) <= 63 && (board.board[(possible + j * counter)] == null || board.board[(possible + j * counter)].colour == this.colour * -1))
                {
                    if ((possible + j * counter) % 8 == 0 || (board.board[(possible + j * counter)] != null && board.board[(possible + j * counter)].colour == this.colour * -1 && !(board.board[(possible + j * counter)] is King)))
                    {
                        this.legalMoves.Add((possible + j * counter));
                        break;
                    }
                    this.legalMoves.Add((possible + j * counter));
                    counter++;
                }
            }
            else if (this.loc % 8 == 0)
            {
                int counter = 1;
                int possible = this.loc;
                int j = presets[0];
                while ((possible + j * counter) >= 0 && (possible + j * counter) <= 63 && (board.board[(possible + j * counter)] == null || board.board[(possible + j * counter)].colour == this.colour * -1))
                {
                    if ((possible + j * counter) % 8 == 7 || (board.board[(possible + j * counter)] != null && board.board[(possible + j * counter)].colour == this.colour * -1 && !(board.board[(possible + j * counter)] is King)))
                    {
                        this.legalMoves.Add((possible + j * counter));
                        break;
                    }
                    this.legalMoves.Add((possible + j * counter));
                    counter++;
                }
            }

            else
            {
                for (int i = 0; i < 2; i++)
                {
                    int counter = 1;
                    int possible = this.loc;
                    int j = presets[i];
                    while ((possible + j * counter) >= 0 && (possible + j * counter) <= 63 && (board.board[(possible + j * counter)] == null || board.board[(possible + j * counter)].colour == this.colour * -1))
                    {
                        if ((possible + j * counter) % 8 == 0 || (possible + j * counter) % 8 == 7 || (board.board[(possible + j * counter)] != null && board.board[(possible + j * counter)].colour == this.colour * -1 && !(board.board[(possible + j * counter)] is King)))
                        {
                            this.legalMoves.Add((possible + j * counter));
                            break;
                        }

                        this.legalMoves.Add((possible + j * counter));
                        counter++;
                    }

                }
            }
            if (ispinned == true)
            {
                legalMoves = genPinnedLegalMoves(legalMoves, pinnedlegal);
            }

            if (board.check == true)
            {
                legalMoves = genPinnedLegalMoves(legalMoves, board.blockcheck);
            }
        }

        public override void genPseudoMoves(Board board)
        {
            legalMoves.Clear();

            int[] presets;
            if (loc % 8 == 0)
            {
                presets = new int[] { 9, -7, -200, -200 };
            }
            else if (loc % 8 == 7)
            {
                presets = new int[] { -9, 7, -200, -200 };
            }
            else
            {
                presets = new int[] { 9, -9, 7, -7 };

            }

            foreach (int i in presets)
            {
                int possible = this.loc;
                int counter = 1;

                while ((possible + i * counter) >= 0 && (possible + i * counter) <= 63)
                {
                    if ((possible + i * counter) % 8 == 0 || (possible + i * counter) % 8 == 7 || (board.board[(possible + i * counter)] != null && !(board.board[(possible + i * counter)] is King)))
                    {
                        this.legalMoves.Add((possible + i * counter));
                        break;
                    }
                    this.legalMoves.Add((possible + i * counter));
                    counter++;
                }
            }

            presets[0] = 1;
            presets[1] = -1;
            presets[2] = 8;
            presets[3] = -8;
            for (int i = 0; i < 2; i++)
            {
                int counter = 1;
                int possible = this.loc;
                int j = presets[i + 2];
                while ((possible + j * counter) >= 0 && (possible + j * counter) <= 63)
                {
                    if (board.board[(possible + j * counter)] != null && !(board.board[(possible + j * counter)] is King))
                    {
                        this.legalMoves.Add((possible + j * counter));
                        break;
                    }
                    this.legalMoves.Add((possible + j * counter));
                    counter++;
                }

            }

            if (this.loc % 8 == 7)
            {
                int counter = 1;
                int possible = this.loc;
                int j = presets[1];
                while ((possible + j * counter) >= 0 && (possible + j * counter) <= 63)
                {
                    if ((possible + j * counter) % 8 == 0 || (board.board[(possible + j * counter)] != null && !(board.board[(possible + j * counter)] is King)))
                    {
                        this.legalMoves.Add((possible + j * counter));
                        break;
                    }
                    this.legalMoves.Add((possible + j * counter));
                    counter++;
                }
            }
            else if (this.loc % 8 == 0)
            {
                int counter = 1;
                int possible = this.loc;
                int j = presets[0];
                while ((possible + j * counter) >= 0 && (possible + j * counter) <= 63)
                {
                    if ((possible + j * counter) % 8 == 7 || (board.board[(possible + j * counter)] != null && !(board.board[(possible + j * counter)] is King)))
                    {
                        this.legalMoves.Add((possible + j * counter));
                        break;
                    }
                    this.legalMoves.Add((possible + j * counter));
                    counter++;
                }
            }

            else
            {
                for (int i = 0; i < 2; i++)
                {
                    int counter = 1;
                    int possible = this.loc;
                    int j = presets[i];
                    while ((possible + j * counter) >= 0 && (possible + j * counter) <= 63)
                    {
                        if ((possible + j * counter) % 8 == 0 || (possible + j * counter) % 8 == 7 || (board.board[(possible + j * counter)] != null && !(board.board[(possible + j * counter)] is King)))
                        {
                            this.legalMoves.Add((possible + j * counter));
                            break;
                        }

                        this.legalMoves.Add((possible + j * counter));
                        counter++;
                    }

                }
            }
        }
    }

    class King : Piece
    { //  need to add check and generate legal moves
        public bool castle;
        public List<int> file1;
        public List<int> file2;
        public List<int> rank1;
        public List<int> rank2;
        public List<int> diagonal1;
        public List<int> diagonal2;
        public List<int> diagonal3;
        public List<int> diagonal4;
        public List<int> knightChecks;

        public King(int colour, int loc)
        {
            this.colour = colour;
            this.castle = true;
            this.loc = loc;
            this.file1 = new List<int>();
            this.file2 = new List<int>();
            this.rank1 = new List<int>();
            this.rank2 = new List<int>();
            this.diagonal1 = new List<int>();
            this.diagonal2 = new List<int>();
            this.diagonal3 = new List<int>();
            this.diagonal4 = new List<int>();
            this.legalMoves = new List<int>();
            this.knightChecks = new List<int>();
            switch (this.colour)
            {
                case -1:
                    this.source = new BitmapImage(new Uri(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Sprites\\" + "bK" + ".png"));
                    this.value = -10000000;

                    break;
                case 1:
                    this.source = new BitmapImage(new Uri(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Sprites\\" + "wK" + ".png"));
                    this.value = 10000000;
                    break;
            }

        }



        public override void genMoves(Board board)
        {
            legalMoves.Clear();
            bool temp = board.check;

            HashSet<int> data = new HashSet<int>();
            List<Piece> oppositepeices = new List<Piece>();
            board.check = false;
            for (int i = 0; i < 64; i++)
            {
                if (board.board[i] != null && board.board[i].colour == this.colour * -1 && !(board.board[i] is King))
                {
                    oppositepeices.Add(board.board[i]);
                }
            }


            foreach (Piece i in oppositepeices)
            {
                i.genPseudoMoves(board);

            }





            for (int i = 0; i < oppositepeices.Count; i++)
            {
                foreach (int j in oppositepeices[i].legalMoves)
                {
                    data.Add(j);
                }
            }

            if (this.loc + 1 >= 0 && this.loc + 1 <= 63 && data.Contains(this.loc + 1) == false && (board.board[this.loc + 1] == null || board.board[this.loc + 1].colour == this.colour * -1) && Math.Abs((this.loc + 1) % 8 - this.loc % 8) == 1)
            {
                legalMoves.Add(this.loc + 1);

                if (this.castle == true && data.Contains(this.loc + 2) == false && board.board[this.loc + 2] == null && board.board[this.loc + 3] != null && ((Rook)board.board[this.loc + 3]).castle == true && temp == false)
                {
                    this.legalMoves.Add(this.loc + 2);
                }
            }

            if (this.loc - 1 >= 0 && this.loc - 1 <= 63 && data.Contains(this.loc - 1) == false && (board.board[this.loc - 1] == null || board.board[this.loc - 1].colour == this.colour * -1) && Math.Abs((this.loc - 1)%8 - this.loc%8) == 1)
            {
                legalMoves.Add(this.loc + -1);

                if (this.castle == true && data.Contains(this.loc - 2) == false && board.board[this.loc - 2] == null && board.board[this.loc - 4] != null && ((Rook)board.board[this.loc - 4]).castle == true && temp == false)
                {
                    this.legalMoves.Add(this.loc - 2);
                }
            }

            int[] presets = { 8, -8, 7, -7, 9, -9 };

            foreach (int i in presets)
            {
                if (this.loc + i >= 0 && this.loc + i <= 63 && data.Contains(this.loc + i) == false && (board.board[this.loc + i] == null || board.board[this.loc + i].colour == this.colour * -1))
                {
                    if (i > 1 && Math.Abs((this.loc + i) % 8 - this.loc % 8) == 1)
                    {
                        legalMoves.Add(this.loc + i);
                    }
                    else
                    {
                        legalMoves.Add(this.loc + i);

                    }
                }
                
            }
            board.check = temp;
        }

        public void genAttack(Piece[] board)
        {
            int file_ = this.loc % 8;
            int rank_ = this.loc / 8;

            this.file1.Clear();
            this.file2.Clear();
            this.rank1.Clear();
            this.rank2.Clear();


            for (int i = 0; i < 8; i++)
            {
                if (i * 8 + file_ < this.loc)
                {
                    this.file1.Add(i * 8 + file_);
                }
                else if (i * 8 + file_ > this.loc)
                {
                    this.file2.Add(i * 8 + file_);

                }
                if (i + 8 * rank_ < this.loc)
                {
                    this.rank1.Add(i + 8 * rank_);
                }
                else if (i + 8 * rank_ > this.loc)
                {
                    this.rank2.Add(i + 8 * rank_);

                }
            }

            this.diagonal1.Clear();
            this.diagonal2.Clear();
            this.diagonal3.Clear();
            this.diagonal4.Clear();

            int[] presets;
            if (this.loc % 8 == 0)
            {
                presets = new int[] { 9, -7 };
            }
            else if (this.loc % 8 == 7)
            {
                presets = new int[] { -9, 7 };
            }
            else
            {
                presets = new int[] { 9, -9, 7, -7 };

            }

            foreach (int i in presets)
            {
                int possible = this.loc;
                int counter = 1;

                while ((possible + i * counter) >= 0 && (possible + i * counter) <= 63)
                {
                    if ((possible + i * counter) % 8 == 0 || (possible + i * counter) % 8 == 7)
                    {
                        switch (i)
                        {
                            case 9:
                                diagonal1.Add(possible + i * counter);

                                break;
                            case -9:
                                diagonal2.Add(possible + i * counter);

                                break;

                            case 7:
                                diagonal3.Add(possible + i * counter);

                                break;
                            case -7:
                                diagonal4.Add(possible + i * counter);

                                break;
                        }
                        break;
                    }


                    switch (i)
                    {
                        case 9:
                            diagonal1.Add(possible + i * counter);
                            break;
                        case -9:
                            diagonal2.Add(possible + i * counter);

                            break;

                        case 7:
                            diagonal3.Add(possible + i * counter);

                            break;
                        case -7:
                            diagonal4.Add(possible + i * counter);

                            break;
                    }
                    counter++;
                }
            }

            this.knightChecks.Clear();

            int[] possiblePresets = { 10, 6, 15, 17, -10, -17, -15, -6 };

            bool flag = false;
            int[] temp = { 0, 1, 6, 7 };
            if (temp.Contains(loc % 8) == true)
            {
                flag = true;
            }

            for (int i = 0; i < possiblePresets.Length; i++)
            {
                if ((loc + possiblePresets[i]) <= 63 && (loc + possiblePresets[i]) >= 0)
                {
                    if (flag == true)
                    {
                        if (temp.Contains((loc + possiblePresets[i]) % 8) == true)
                        {
                            if (loc % 8 == 0 || loc % 8 == 1)
                            {
                                if ((loc + possiblePresets[i]) % 8 > 1) { }
                            }

                            else if ((loc + possiblePresets[i]) % 8 < 6) { }

                            else
                            {
                                knightChecks.Add(loc + possiblePresets[i]);
                            }

                        }
                        else
                        {
                            knightChecks.Add(loc + possiblePresets[i]);
                        }
                    }
                    else
                    {
                        knightChecks.Add(loc + possiblePresets[i]);
                    }
                }
            }

        }

        public override void genPseudoMoves(Board board)
        {
            this.legalMoves.Clear();
            this.legalMoves.Add(this.loc + 8);
            this.legalMoves.Add(this.loc - 8);
            this.legalMoves.Add(this.loc - 1);
            this.legalMoves.Add(this.loc + 1);
            this.legalMoves.Add(this.loc - 7);
            this.legalMoves.Add(this.loc - 9);
            this.legalMoves.Add(this.loc + 7);
            this.legalMoves.Add(this.loc + 9);
        }
    }

    class Board
    {
        public Piece[] board;
        public int turn;
        int white = 0;
        private int wking_index = 4;
        public bool check = false;
        public int checking_index = -1;
        public List<int> pinned_pieces;
        public int topromote = -1;
        public int enpassanttemp = -1;
        public int bking_index = 60;
        public List<int> blockcheck = new List<int>();
        public int[] checkingpieces = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };
        System.Media.SoundPlayer player = new System.Media.SoundPlayer();
        int black = 0;

        public Board()
        {
            Piece[] temp = { new Rook(1, 0), new Knight(1, 1), new Bishop(1, 2), new Queen(1, 3), new King(1, 4), new Bishop(1, 5), new Knight(1, 6), new Rook(1, 7),
                new Pawn(1, 8), new Pawn(1, 9), new Pawn(1, 10), new Pawn(1, 11), new Pawn(1, 12), new Pawn(1, 13), new Pawn(1, 14), new Pawn(1, 15),
                null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null,
                new Pawn(-1, 48), new Pawn(-1, 49), new Pawn(-1, 50), new Pawn(-1,  51), new Pawn(-1, 52), new Pawn(-1, 53), new Pawn(-1, 54), new Pawn(-1, 55),
                new Rook(-1, 56), new Knight(-1, 57), new Bishop(-1, 58), new Queen(-1, 59), new King(-1, 60), new Bishop(-1, 61), new Knight(-1, 62), new Rook(-1, 63) };
            this.board = temp;
            this.pinned_pieces = new List<int>();
            ((King)board[this.wking_index]).genAttack(board);
            ((King)board[this.bking_index]).genAttack(board);
            this.turn = 1;

        }

        private int check_staus(int own, int opposite, int index, List<int> temp, List<int> ppLegal)
        {


            if (opposite > 0 && own == 0)
            { // then we know that there is a attacking piece and no own pieces to block
                return index;
            }

            else if (opposite > 0 && own == 1 && temp.Count > 0)
            { // there is still an attacking piece and there is a own piece to block

                (board[temp[0]]).ispinned = true;
                (board[temp[0]]).pinnedlegal = ppLegal;
                pinned_pieces.Add(temp[0]);

            }
            return -1;
        }

        private int check_infront(List<int> moves, King king, Type typetocheck)
        {

            int potentialcheck = 0;
            int peicesbeforecheck = 0;
            int index = -1;
            List<int> temp = new List<int>();
            foreach (int i in moves)
            {

                if (board[i] != null)
                {

                    if (board[i].colour == this.turn * -1 && ((board[i].GetType() == typetocheck) || (board[i].GetType() == typeof(Queen))))
                    {
                        potentialcheck++;
                        if (index == -1)
                        {
                            index = i;
                        }
                    }

                    else if (potentialcheck == 0)
                    {
                        if (board[i].colour == this.turn)
                        {
                            temp.Add(i);
                        }
                        peicesbeforecheck++;

                    }

                }

            }

            return check_staus(peicesbeforecheck, potentialcheck, index, temp, moves);

        }

        private int check_behind(List<int> moves, King king, Type typetocheck)
        { // rewrite to support black pieces behind  

            int opposite = 0;
            int same = 0;
            int index = 0;
            List<int> temp = new List<int>();

            foreach (int i in moves)
            {
                if (board[i] != null && board[i].colour == this.turn * -1 && (board[i] is Queen || board[i].GetType() == typetocheck))
                {
                    opposite++;
                    if (i > index)
                    {
                        index = i;
                    }
                }
                else if (board[i] != null && opposite > 0)
                {
                    temp.Add(i);
                    same++;

                }
            }

            return check_staus(same, opposite, index, temp, moves);

        }

        private int genAllLegalMoves(int colour)
        {

            List<Piece> peices = new List<Piece>();
            HashSet<int> data = new HashSet<int>();

            for (int i = 0; i < 64; i++)
            {
                if (board[i] != null && board[i].colour == colour && !(board[i] is King))
                {
                    peices.Add(board[i]);
                }
            }

            foreach (Piece i in peices)
            {
                i.genMoves(this);

            }

            for (int i = 0; i < peices.Count; i++)
            {
                foreach (int j in peices[i].legalMoves)
                {
                    data.Add(j);
                }
            }

            return data.Count;


        }

        private List<int> check_infront(List<int> moves, King king)
        {
            List<int> temp = new List<int>();

            foreach (int i in moves)
            {

                if (board[i] != null && board[i].colour == this.turn * -1 && ((board[i].GetType() == typeof(Bishop)) || (board[i].GetType() == typeof(Queen))))
                {
                    temp.Add(i);
                    break;
                }

                temp.Add(i);

            }

            return temp;

        }

        private List<int> genattack(int checkindex, int kingindex)
        {
            List<int> attacking = new List<int>();



            if (kingindex % 8 == checkindex % 8)

            {
                if (checkindex < kingindex)
                {
                    int counter = 1;
                    while (checkindex + 8 * counter < kingindex)
                    {
                        attacking.Add(checkindex + 8 * counter);
                        counter++;
                    }
                    attacking.Add(checkindex);
                    return attacking;
                }
                else
                {
                    int counter = -1;
                    while (checkindex + 8 * counter > kingindex)
                    {
                        attacking.Add(checkindex + 8 * counter);
                        counter--;
                    }
                    attacking.Add(checkindex);
                    return attacking;
                }
            }

            else if (kingindex / 8 == checkindex / 8)
            {

                if (checkindex < kingindex)
                {
                    int counter = 1;
                    while (checkindex + counter < kingindex)
                    {
                        attacking.Add(checkindex + counter);
                        counter++;
                    }
                    attacking.Add(checkindex);
                    return attacking;
                }
                else
                {
                    int counter = -1;
                    while (checkindex + counter > kingindex)
                    {
                        attacking.Add(checkindex + counter);
                        counter--;
                    }
                    attacking.Add(checkindex);
                    return attacking;
                }

            }

            if (((King)board[kingindex]).diagonal1.Contains(checkindex) == true) attacking = check_infront(((King)board[kingindex]).diagonal1, (King)board[kingindex]);

            else if (((King)board[kingindex]).diagonal2.Contains(checkindex) == true) attacking = check_infront(((King)board[kingindex]).diagonal2, (King)board[kingindex]);

            else if (((King)board[kingindex]).diagonal3.Contains(checkindex) == true) attacking = check_infront(((King)board[kingindex]).diagonal3, (King)board[kingindex]);

            else attacking = check_infront(((King)board[kingindex]).diagonal4, (King)board[kingindex]);



            attacking.Add(checkindex);

            return attacking;
        }

        public void check_for_check()
        {
            player.SoundLocation = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Sprites\\" + "Silence" + ".wav";
            foreach (int i in pinned_pieces)
            {
                if (board[i] != null) board[i].ispinned = false;
            }
            pinned_pieces.Clear();
            blockcheck.Clear();
            checkingpieces = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };
            check = false;


            King king;
            int piecesChecking = 0;
            int checkindex = 0;

            if (this.turn == -1)
            {
                king = (King)board[this.bking_index];
            }
            else
            {
                king = (King)board[this.wking_index];
            }



            checkingpieces[0] = check_behind(king.file1, king, typeof(Rook));
            checkingpieces[1] = check_infront(king.file2, king, typeof(Rook));
            checkingpieces[2] = check_behind(king.rank1, king, typeof(Rook));
            checkingpieces[3] = check_infront(king.rank2, king, typeof(Rook));
            checkingpieces[4] = check_infront(king.diagonal1, king, typeof(Bishop));
            checkingpieces[5] = check_infront(king.diagonal2, king, typeof(Bishop));
            checkingpieces[6] = check_infront(king.diagonal3, king, typeof(Bishop));
            checkingpieces[7] = check_infront(king.diagonal4, king, typeof(Bishop));

            foreach (int i in king.knightChecks)
            {
                if (board[i] != null && (board[i] is Knight) && board[i].colour == this.turn * -1)
                {
                    piecesChecking++;
                    checkindex = i;
                }
            }

            if (board[king.loc + 9 * king.colour] != null && board[king.loc + 9 * king.colour] is Pawn && board[king.loc + 9 * king.colour].colour == king.colour*-1) {
                piecesChecking++;
                checkindex = king.loc + 9 * king.colour;
            }

            if (board[king.loc + 7 * king.colour] != null && board[king.loc + 7 * king.colour] is Pawn && board[king.loc + 7 * king.colour].colour == king.colour * -1)
            {
                piecesChecking++;
                checkindex = king.loc + 7 * king.colour;
            }

            foreach (int i in checkingpieces)
            {
                if (i != -1)
                {
                    piecesChecking++;
                    checkindex = i;
                }
            }

            if (piecesChecking == 0){
                king.genMoves(this);
                if (king.legalMoves.Count == 0){
                    int legal = genAllLegalMoves(king.colour);
                    if (legal == 0)
                    {
                        player.SoundLocation = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Sprites\\" + "Checkmate" + ".wav";
                        MessageBox.Show("Stalemate!");
                    }
                }
            }

            else if (piecesChecking == 1)
            {
                check = true;
                checking_index = checkindex;
                if (board[checkindex] is Knight)
                {
                    blockcheck.Add(checkindex);
                }
                else blockcheck = genattack(checking_index, king.loc);
                player.SoundLocation = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Sprites\\" + "Check" + ".wav";

                king.genMoves(this);
                if (king.legalMoves.Count == 0)
                {
                    int legal = genAllLegalMoves(king.colour);
                    if (legal == 0)
                    {
                        player.SoundLocation = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Sprites\\" + "Checkmate" + ".wav";
                        MessageBox.Show("Checkmate");
                    }
                }
            }



            else if (piecesChecking == 2)
            {
                king.genMoves(this);
                player.SoundLocation = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Sprites\\" + "Check" + ".wav";

                if (king.legalMoves.Count == 0)
                {
                    MessageBox.Show("Checkmate");
                    player.SoundLocation = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Sprites\\" + "Checkmate" + ".wav";
                }
                checking_index = -1;
            }
            if (player.SoundLocation != System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Sprites\\" + "Silence" + ".wav") {
                player.Play();
            }
        }

        public void promote(int piece) {
            
            switch (piece) {
                case 1:
                    board[topromote] = new Queen(board[topromote].colour, topromote);
                    break;

                case 2:
                    board[topromote] = new Rook(board[topromote].colour, topromote);
                    break;

                case 3:
                    board[topromote] = new Bishop(board[topromote].colour, topromote);
                    break;

                case 4:
                    board[topromote] = new Knight(board[topromote].colour, topromote);
                    break;
            }

            topromote = -1;

        }

        public int MakeMove(int toMove, int whereTo)
        {
            this.turn *= -1;
            Piece temp = this.board[toMove];
            enpassanttemp = -1;
            if (board[whereTo] == null)
            {
                player.SoundLocation = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Sprites\\" + "Move" + ".wav";

            }
            else {
                player.SoundLocation = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Sprites\\" + "Capture" + ".wav";
            }

            if (temp is Pawn)
            {

                ((Pawn)temp).hasMoved = true;
                int temp1 = (Math.Abs(toMove - whereTo));
                bool temp2 = board[whereTo] == null;
                if (Math.Abs(toMove - whereTo) == 16)
                {
                    enpassanttemp = whereTo;
                }

                else if ((Math.Abs(toMove - whereTo) == 9 || Math.Abs(toMove - whereTo) == 7) && board[whereTo] == null)
                {
                    this.board[whereTo + 8 * this.turn] = null;
                }

                if (whereTo / 8 == 0 || whereTo / 8 == 7)
                {
                    topromote = whereTo;
                }
            }

            else if (temp is King)
            {

                if (this.turn * -1 == 1) this.wking_index = whereTo;

                else this.bking_index = whereTo;

                ((King)temp).castle = false;

                if (Math.Abs(whereTo - toMove) == 2)
                {
                    int rookloc;
                    player.SoundLocation = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Sprites\\" + "Castle" + ".wav";
                    if (whereTo - toMove == 2)
                    {

                        rookloc = (temp.loc + 3);
                        ((Rook)board[rookloc]).castle = false;
                        ((Rook)board[rookloc]).loc = temp.loc + 1;
                        board[temp.loc + 1] = board[rookloc];
                        board[rookloc] = null;

                    }
                    else
                    {
                        rookloc = (temp.loc / 8) * 8;
                        ((Rook)board[rookloc]).castle = false;
                        ((Rook)board[rookloc]).loc = temp.loc - 1;
                        board[temp.loc - 1] = board[rookloc];
                        board[rookloc] = null;
                    }
                }
                temp.loc = whereTo;

                ((King)temp).genAttack(this.board);
            }

            else if (temp is Rook) ((Rook)temp).castle = false;

            temp.loc = whereTo;
            this.board[toMove] = null;

            if (this.board[whereTo] != null)
            {
                if ((turn % 2) == 0)
                {
                    white += temp.value;
                }
                else
                {
                    black += temp.value;
                }
            }

            this.board[whereTo] = temp;
            if (topromote != -1)return 0;

            player.Play();

            check_for_check();
            return 1;
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Board board = new Board();

        Border[] squares = new Border[64];
        Canvas rootCanvas = new Canvas();
        StackPanel promotion;
        Image[] images = new Image[4];
        int currentindex = -1;
        Viewbox dynamicViewbox;
        
        public MainWindow()
        {
            InitializeComponent();
            dynamicViewbox = new Viewbox();


            // Set StretchDirection and Stretch properties  
            dynamicViewbox.StretchDirection = StretchDirection.Both;
            dynamicViewbox.Stretch = Stretch.Uniform;
            dynamicViewbox.Child = rootCanvas;
            rootCanvas.Height = Root.Height;
            rootCanvas.Width = Root.Width;
            rootCanvas.Background = Brushes.Black;
            Root.Content = dynamicViewbox;
            ResetBoard(1);
            promotion = new StackPanel();
            promotion.Background = Brushes.White;
            rootCanvas.Children.Add(promotion);

            for(int i=0; i<4; i++) {
                images[i] = new Image();
                images[i].Tag = i+1;
                images[i].MouseDown += new MouseButtonEventHandler(click);
                images[i].Height = Root.Height / 8;
                images[i].Width = Root.Height / 8;
                promotion.Children.Add(images[i]);
            }
            promotion.Visibility = Visibility.Hidden;


        }

        public void ResetBoard(int colour)
        {

            rootCanvas.Children.Clear();
            if (colour == 1)
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        int index = ((j + i * 8));
                        squares[index] = new Border();
                        squares[index].Tag = index;
                        squares[index].MouseDown += new MouseButtonEventHandler(OnMouseDown);
                        squares[index].Height = Root.Height / 8;
                        squares[index].Width = Root.Height / 8;
                        squares[index].BorderBrush = Brushes.Red;
                        squares[index].BorderThickness = new Thickness();

                        if ((index + (-1 * (i))) % 2 == 0)
                        {
                            squares[index].Background = Brushes.Green;
                        }
                        else
                        {
                            squares[index].Background = Brushes.Wheat;
                        }
                        rootCanvas.Children.Add(squares[index]);
                        Canvas.SetLeft(squares[index], j * squares[index].Height);
                        Canvas.SetBottom(squares[index], i * squares[index].Height);

                        if (board.board[index] != null)
                        {
                            Image tempimage = new Image();
                            tempimage.Source = board.board[index].source;
                            squares[index].Child = tempimage;
                        }

                    }
                }
            }

            else
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        int index = (63 - (j + i * 8));
                        squares[index] = new Border();
                        squares[index].Tag = index;
                        squares[index].MouseDown += new MouseButtonEventHandler(OnMouseDown);
                        squares[index].Height = Root.Height / 8;
                        squares[index].Width = Root.Height / 8;
                        squares[index].BorderBrush = Brushes.Red;
                        squares[index].BorderThickness = new Thickness();

                        if ((index + (-1 * (i))) % 2 == 0)
                        {
                            squares[index].Background = Brushes.Green;
                        }
                        else
                        {
                            squares[index].Background = Brushes.Wheat;
                        }
                        rootCanvas.Children.Add(squares[index]);
                        Canvas.SetLeft(squares[index], j * squares[index].Height);
                        Canvas.SetBottom(squares[index], i * squares[index].Height);

                        if (board.board[index] != null)
                        {
                            Image tempimage = new Image();
                            tempimage.Source = board.board[index].source;
                            squares[index].Child = tempimage;
                        }

                    }
                }
            }
        }

        private void changeimage(int index, BitmapImage image) {
            ((Image)squares[index].Child).Source = image;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e) // need to fix the co-ordinates maths idk rn why it isnt working please kill me
        {
            int index = (int)((Border)sender).Tag;
            Piece selectedPiece = board.board[index];

            if (currentindex != -1 && board.board[currentindex].legalMoves.Contains(index) == true)
            {

                foreach (int i in board.board[currentindex].legalMoves)
                {
                    squares[i].BorderThickness = new Thickness();

                }

                if (board.board[currentindex] is Pawn)
                {

                    if ((Math.Abs(currentindex - index) == 9 || Math.Abs(currentindex - index) == 7) && board.board[index] == null)
                    {
                        squares[index + 8 * board.turn * -1].Child = null;
                    }

                }

                else if (board.board[currentindex] is King && (Math.Abs(currentindex - index) == 2))
                {
                    int rookloc;

                    if (index - currentindex == 2)
                    {

                        rookloc = (((King)board.board[currentindex]).loc + 3);
                        Image temp1 = (Image)squares[rookloc].Child;
                        squares[rookloc].Child = null;
                        squares[currentindex + 1].Child = temp1;

                    }
                    else
                    {
                        rookloc = (((King)board.board[currentindex]).loc / 8) * 8;
                        Image temp1 = (Image)squares[rookloc].Child;
                        squares[rookloc].Child = null;
                        squares[currentindex - 1].Child = temp1;
                    }
                }

                Image temp = (Image)squares[currentindex].Child;
                squares[currentindex].Child = null;
                if (board.MakeMove(currentindex, index) == 0) {
                    ShowPromotion();
                }
                squares[index].Child = temp;
                ((Image)squares[index].Child).Source = board.board[index].source;

                currentindex = -1;
            }

            else if (selectedPiece != null && selectedPiece.colour == board.turn)
            {

                if (currentindex != -1)
                {
                    foreach (int i in board.board[currentindex].legalMoves)
                    {
                        squares[i].BorderThickness = new Thickness();
                    }
                }

                currentindex = index;
                selectedPiece.genMoves(board);



                if (selectedPiece.legalMoves.Count != 0)
                {
                    foreach (int i in selectedPiece.legalMoves)
                    {
                        squares[i].BorderThickness = new Thickness(2, 2, 2, 2);
                    }
                }
            }
        }

        public void ShowPromotion()
        {
            promotion.Visibility = Visibility.Visible;

            if (board.topromote / 8 == 7) {
                Canvas.SetTop(promotion, 0);
            }
            
            Canvas.SetLeft(promotion, (board.topromote % 8) * squares[0].Height);
            string[] imagesources = { "Q.png", "R.png", "B.png", "N.png" };

            for (int i = 0; i < 4; i++)
            {
                if (board.topromote / 8 == 7)
                {
                    images[i].Source = new BitmapImage(new Uri(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Sprites\\" + "w" + imagesources[i]));
                }
                else
                {
                    images[i].Source = new BitmapImage(new Uri(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Sprites\\" + "b" + imagesources[i]));

                }
            }
        }

        private void click(object sender, EventArgs e) {

            changeimage(board.topromote, (BitmapImage)((Image)sender).Source);
            board.promote((int)((Image)sender).Tag);
            promotion.Visibility = Visibility.Hidden;
            board.check_for_check();
        }
    }
}