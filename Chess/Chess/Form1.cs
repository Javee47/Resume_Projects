

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;




namespace Chess
{


    public partial class Form1 : Form
    {
        Button[,] boardArray = new Button[8, 8];
        Boolean[,] attackArray = new Boolean[8, 8];
        Button Concede = new Button();
        Boolean WhichPieceIsChecking = false;
        int[,] pieces = new int[8, 8];
        int movementmemoryx, movementmemoryy;
        int incrementor = 0;
        int enemy, pawnadd;
        int turns = 1;
        Boolean checkMate = true;
        int directionCX, directionCY;
        Boolean attacks = false;
        Boolean pieceSelected = false;
        Boolean mistake = false;
        Boolean IsThisMoveRepeaterChecking = false;


        int CheckerX = 0, CheckerY = 0;
        int BKingX = 0, BKingY = 0;
        int WKingX = 0, WKingY = 0;
       
        public Form1()
        {
            InitializeComponent();
            int h = 20;
            int v = 20;
            Setboard(boardArray, pieces, h, v);
            Paintboard(boardArray);
            disabler();
            Enabler();

            Concede.Location = new Point(900, 85);
            Concede.Size = new Size(97, 85);
            Concede.Click += new EventHandler(concession);
            Concede.Text = "Concede";
            this.Controls.Add(Concede);



        }
        void concession(Object sender, EventArgs e)
        {
            if (turns == 1)
            {
                MessageBox.Show("White wins, Checkmate!");
            }
            else
            {
                MessageBox.Show("Black wins, Checkmate!");
            }
            reset();
        }
        void Paintboard(Button[,] boardArray)
        {
            int ColorAlt = 1;
            for (int f = 0; f < 8; f++)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (ColorAlt == 1)
                    {
                        if (i % 2 == 0)
                        {
                            boardArray[i, f].BackColor = Color.White;
                        }
                        else
                        {
                            boardArray[i, f].BackColor = Color.LightGray;
                        }
                    }
                    else
                    {


                        if (i % 2 == 0)
                        {
                            boardArray[i, f].BackColor = Color.LightGray;
                        }
                        else
                        {
                            boardArray[i, f].BackColor = Color.White;
                        }
                    }
                    if ((i + 1) % 8 == 0)
                    {
                        ColorAlt = ColorAlt * -1;
                    }


                }
            }
        }
        void reset()
        {

            Paintboard(boardArray);
            turns = 1;
            disabler();
            for (int f = 0; f < 8; f++)
            {
                for (int i = 0; i < 8; i++)
                {

                    boardArray[i, f].Text = "";
                    pieces[i, f] = 0;
                    boardArray[i, f].Image = null;
                    attackArray[i, f] = false;

                    if (f == 1)
                    {
                        pieces[i, f] = 1;
                        boardArray[i, f].Text = "WP";
                        //boardArray[i, f].Image = Image.FromFile(@"C:\Users\Joshua\Pictures\WP.PNG");
                    }
                    if (f == 6)
                    {
                        pieces[i, f] = 2;
                        boardArray[i, f].Text = "BP";
                        //boardArray[i, f].Image = Image.FromFile(@"C:\Users\Joshua\Pictures\BP.PNG");
                    }
                    if ((f == 0) && (i == 0) || (f == 0) && (i == 7))
                    {
                        pieces[i, f] = 1;
                        boardArray[i, f].Text = "WR";
                        //boardArray[i, f].Image = Image.FromFile(@"C:\Users\Joshua\Pictures\WR.PNG");
                    }
                    if ((f == 7) && (i == 0) || (f == 7) && (i == 7))
                    {
                        pieces[i, f] = 2;
                        boardArray[i, f].Text = "BR";
                        //boardArray[i, f].Image = Image.FromFile(@"C:\Users\Joshua\Pictures\BR.PNG");
                    }
                    if ((f == 0) && (i == 1) || (f == 0) && (i == 6))
                    {
                        pieces[i, f] = 1;
                        boardArray[i, f].Text = "WK";
                        //boardArray[i, f].Image = Image.FromFile(@"C:\Users\Joshua\Pictures\WK.PNG");
                    }
                    if ((f == 7) && (i == 1) || (f == 7) && (i == 6))
                    {
                        pieces[i, f] = 2;
                        boardArray[i, f].Text = "BK";
                        //boardArray[i, f].Image = Image.FromFile(@"C:\Users\Joshua\Pictures\BK.PNG");
                    }
                    if ((f == 0) && (i == 2) || (f == 0) && (i == 5))
                    {
                        pieces[i, f] = 1;
                        boardArray[i, f].Text = "WB";
                        //boardArray[i, f].Image = Image.FromFile(@"C:\Users\Joshua\Pictures\WB.PNG");
                    }
                    if ((f == 7) && (i == 2) || (f == 7) && (i == 5))
                    {
                        pieces[i, f] = 2;
                        boardArray[i, f].Text = "BB";
                        //boardArray[i, f].Image = Image.FromFile(@"C:\Users\Joshua\Pictures\bb.PNG");
                    }
                    if ((f == 0) && (i == 3))
                    {
                        pieces[i, f] = 1;
                        boardArray[i, f].Text = "WKing";
                        //boardArray[i, f].Image = Image.FromFile(@"C:\Users\Joshua\Pictures\WKing.PNG");
                    }
                    if ((f == 0) && (i == 4))
                    {
                        pieces[i, f] = 1;
                        boardArray[i, f].Text = "WQ";
                        //boardArray[i, f].Image = Image.FromFile(@"C:\Users\Joshua\Pictures\WQueen.PNG");
                    }
                    if ((f == 7) && (i == 3))
                    {
                        pieces[i, f] = 2;
                        boardArray[i, f].Text = "BKing";
                        //boardArray[i, f].Image = Image.FromFile(@"C:\Users\Joshua\Pictures\BKing.PNG");
                    }
                    if ((f == 7) && (i == 4))
                    {
                        pieces[i, f] = 2;
                        boardArray[i, f].Text = "BQ";
                        //boardArray[i, f].Image = Image.FromFile(@"C:\Users\Joshua\Pictures\BQueen.PNG");
                    }
                    if (pieces[i, f] == 1)
                    {
                        boardArray[i, f].Enabled = true;
                    }
                }
            }
            
        }
        void yellowEnabler()
        {
            for (int f = 0; f < 8; f++)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (boardArray[i, f].BackColor == Color.Yellow)
                    {
                        boardArray[i, f].Enabled = true;
                        pieceSelected = true;
                    }
                    if (boardArray[i, f].BackColor == Color.Green)
                    {
                        pieceSelected = true;
                    }
                }
            }
        }
        void disabler()
        {
            for (int f = 0; f < 8; f++)
            {
                for (int i = 0; i < 8; i++)
                {
                    boardArray[i, f].Enabled = false;
                }
            }
        }
        void Enabler()
        {
            for (int f = 0; f < 8; f++)
            {
                for (int i = 0; i < 8; i++)
                {

                    if ((turns == 1) & pieces[i, f] == 1)
                    {
                        boardArray[i, f].Enabled = true;
                    }
                    else if ((turns != 1) & (pieces[i, f] == 2))
                    {
                        boardArray[i, f].Enabled = true;
                    }

                }
            }

            turns = turns * -1;
        }
        void Setboard(Button[,] boardArray, int[,] pieces, int horizontal, int vertical)
        {
            for (int f = 0; f < 8; f++)
            {
                for (int i = 0; i < 8; i++)
                {
                    boardArray[i, f] = new Button();
                    boardArray[i, f].Size = new Size(97, 85);
                    boardArray[i, f].Location = new Point(horizontal, vertical);
                    boardArray[i, f].Click += new EventHandler(FirstStepMove);
                    attackArray[i, f] = false;

                    pieces[i, f] = 0;
                    if ((i + 1) % 8 == 0)
                    {
                        vertical += 85;
                        horizontal = 20;
                    }
                    else
                    {
                        horizontal += 85;
                    }
                    if (f == 1)
                    {
                        pieces[i, f] = 1;
                        boardArray[i, f].Text = "WP";
                        //boardArray[i, f].Image = Image.FromFile(@"C:\Users\Joshua\Pictures\WP.PNG");
                    }
                    if (f == 6)
                    {
                        pieces[i, f] = 2;
                        boardArray[i, f].Text = "BP";
                        //boardArray[i, f].Image = Image.FromFile(@"C:\Users\Joshua\Pictures\BP.PNG");
                    }
                    if ((f == 0) && (i == 0) || (f == 0) && (i == 7))
                    {
                        pieces[i, f] = 1;
                        boardArray[i, f].Text = "WR";
                        //boardArray[i, f].Image = Image.FromFile(@"C:\Users\Joshua\Pictures\WR.PNG");
                    }
                    if ((f == 7) && (i == 0) || (f == 7) && (i == 7))
                    {
                        pieces[i, f] = 2;
                        boardArray[i, f].Text = "BR";
                        //boardArray[i, f].Image = Image.FromFile(@"C:\Users\Joshua\Pictures\BR.PNG");
                    }
                    if ((f == 0) && (i == 1) || (f == 0) && (i == 6))
                    {
                        pieces[i, f] = 1;
                        boardArray[i, f].Text = "WK";
                        //boardArray[i, f].Image = Image.FromFile(@"C:\Users\Joshua\Pictures\WK.PNG");
                    }
                    if ((f == 7) && (i == 1) || (f == 7) && (i == 6))
                    {
                        pieces[i, f] = 2;
                        boardArray[i, f].Text = "BK";
                        //boardArray[i, f].Image = Image.FromFile(@"C:\Users\Joshua\Pictures\BK.PNG");
                    }
                    if ((f == 0) && (i == 2) || (f == 0) && (i == 5))
                    {
                        pieces[i, f] = 1;
                        boardArray[i, f].Text = "WB";
                        //boardArray[i, f].Image = Image.FromFile(@"C:\Users\Joshua\Pictures\WB.PNG");
                    }
                    if ((f == 7) && (i == 2) || (f == 7) && (i == 5))
                    {
                        pieces[i, f] = 2;
                        boardArray[i, f].Text = "BB";
                        //boardArray[i, f].Image = Image.FromFile(@"C:\Users\Joshua\Pictures\bb.PNG");
                    }
                    if ((f == 0) && (i == 3))
                    {
                        pieces[i, f] = 1;
                        boardArray[i, f].Text = "WKing";
                        //boardArray[i, f].Image = Image.FromFile(@"C:\Users\Joshua\Pictures\WKing.PNG");
                    }
                    if ((f == 0) && (i == 4))
                    {
                        pieces[i, f] = 1;
                        boardArray[i, f].Text = "WQ";
                        //boardArray[i, f].Image = Image.FromFile(@"C:\Users\Joshua\Pictures\WQueen.PNG");
                    }
                    if ((f == 7) && (i == 3))
                    {
                        pieces[i, f] = 2;
                        boardArray[i, f].Text = "BKing";
                        //boardArray[i, f].Image = Image.FromFile(@"C:\Users\Joshua\Pictures\BKing.PNG");
                    }
                    if ((f == 7) && (i == 4))
                    {
                        pieces[i, f] = 2;
                        boardArray[i, f].Text = "BQ";
                        //boardArray[i, f].Image = Image.FromFile(@"C:\Users\Joshua\Pictures\BQueen.PNG");
                    }


                    this.Controls.Add(boardArray[i, f]);
                }
            }


        }
        void FirstStepMove(Object sender, EventArgs e)
        {

            Button clickedButton = (Button)sender;


            for (int f = 0; f < 8; f++)
            {
                for (int i = 0; i < 8; i++)
                {

                    if (pieces[i, f] == 1)
                    {
                        enemy = 2;
                        pawnadd = -1;
                    }
                    else
                    {
                        enemy = 1;
                        pawnadd = 1;
                    }

                    if (boardArray[i, f] == clickedButton)
                    {
                        if (boardArray[i, f].BackColor == Color.Yellow)
                        {
                            secondStepMove(boardArray, pieces, i, f, enemy, pawnadd);
                        }
                        else
                        {
                            disabler();




                            if (boardArray[i, f].Text == "BP" || boardArray[i, f].Text == "WP")
                            {
                                pawns(boardArray, pieces, i, f, enemy, pawnadd);
                            }
                            if (boardArray[i, f].Text == "WB" || boardArray[i, f].Text == "BB")
                            {

                                MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 1, 1);
                                MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, -1, 1);
                                MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 1, -1);
                                MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, -1, -1);

                            }
                            if (boardArray[i, f].Text == "WR" || boardArray[i, f].Text == "BR")
                            {
                                MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, -1, 0);
                                MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 1, 0);
                                MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 0, -1);
                                MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 0, 1);
                            }
                            if (boardArray[i, f].Text == "BQ" || boardArray[i, f].Text == "WQ")
                            {
                                MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 1, 1);
                                MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, -1, 1);
                                MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 1, -1);
                                MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, -1, -1);
                                MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, -1, 0);
                                MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 1, 0);
                                MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 0, -1);
                                MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 0, 1);
                            }
                            if (boardArray[i, f].Text == "BK" || boardArray[i, f].Text == "WK")
                            {
                                KnightMovement(boardArray, pieces, i, f, enemy, 1, 1);
                                KnightMovement(boardArray, pieces, i, f, enemy, 1, -1);
                                KnightMovement(boardArray, pieces, i, f, enemy, -1, 1);
                                KnightMovement(boardArray, pieces, i, f, enemy, -1, -1);
                            }
                            if (boardArray[i, f].Text == "WKing" || boardArray[i, f].Text == "BKing")
                            {
                                KingMove(boardArray, pieces, i, f, enemy, 1, 1);
                                KingMove(boardArray, pieces, i, f, enemy, -1, 1);
                                KingMove(boardArray, pieces, i, f, enemy, 1, -1);
                                KingMove(boardArray, pieces, i, f, enemy, -1, -1);
                                KingMove(boardArray, pieces, i, f, enemy, -1, 0);
                                KingMove(boardArray, pieces, i, f, enemy, 1, 0);
                                KingMove(boardArray, pieces, i, f, enemy, 0, -1);
                                KingMove(boardArray, pieces, i, f, enemy, 0, 1);

                            }
                            if (pieces[i, f] == 2 || pieces[i, f] == 1)
                            {
                                clickedButton.BackColor = Color.Green;
                            }
         
                        }


                    }


                }
            }
            yellowEnabler();

        }


        void KingMove(Button[,] boardArray, int[,] pieces, int RealI, int RealF, int targetColor, int multipleX, int multipleY)
        {
            int i = RealI;
            int f = RealF;
            if ((i - (1 * multipleX) >= 0) & (i - (1 * multipleX) < 8) & (f - (1 * multipleY) < 8) & (f - (1 * multipleY) >= 0))
            {
                if (pieces[i - (1 * multipleX), f - (1 * multipleY)] == 0 || pieces[i - (1 * multipleX), f - (1 * multipleY)] == targetColor)
                {
                    boardArray[i - (1 * multipleX), f - (1 * multipleY)].BackColor = Color.Yellow;
                }


            }
            movementmemoryx = i;
            movementmemoryy = f;
        }
        void KingCheck(Button[,] boardArray, int[,] pieces, int RealI, int RealF, int targetColor, int multipleX, int multipleY)
        {
            int i = RealI;
            int f = RealF;
            if ((i - (1 * multipleX) >= 0) & (i - (1 * multipleX) < 8) & (f - (1 * multipleY) < 8) & (f - (1 * multipleY) >= 0))
            {
                if (pieces[i - (1 * multipleX), f - (1 * multipleY)] == 0 || pieces[i - (1 * multipleX), f - (1 * multipleY)] == targetColor)
                {

                    if (boardArray[i - (1 * multipleX), f - (1 * multipleY)].BackColor != Color.Yellow)
                    {
                        
                        checkMate = false;
                        boardArray[i - (1 * multipleX), f - (1 * multipleY)].BackColor = Color.Purple;
                        
                    }

                }
            }
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Escape) && pieceSelected == true)
            {
                Paintboard(boardArray);
                Enabler();
                disabler();
                Enabler();
                return true;
            }
            else if (keyData == (Keys.Escape))
            {
                Close();
                return true;
            }
            if (keyData == (Keys.C))
            {
                reset();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        void CheckKing(Button[,] boardArray, int[,] pieces, int ri, int rf, int targetColor, int pawnadd)
        {

            checkMate = true;

            for (int f = 0; f < 8; f++)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (boardArray[i, f].Text == "WKing")
                    {
                        WKingX = i;
                        WKingY = f;

                    }
                    else if (boardArray[i, f].Text == "BKing")
                    {
                        BKingX = i;
                        BKingY = f;
                    }
                }
            }

            allPieces();


            if (boardArray[WKingX, WKingY].BackColor == Color.Yellow)
            {

                MessageBox.Show("White King is in check!");
                if (turns == -1)
                {
                    MessageBox.Show("That's an illegal move because your king is in check!");
                    mistake = true;

                }
                else
                {
                    Paintboard(boardArray);
                    allWhite();
                   

                    if (boardArray[CheckerX, CheckerY].BackColor == Color.Yellow)
                    {
                        checkMate = false;
                    }

                    Paintboard(boardArray);
                    attacks = true;
                    allBlack();
                    attacks = false;
                    KingCheck(boardArray, pieces, WKingX, WKingY, 2, 1, 1);
                    KingCheck(boardArray, pieces, WKingX, WKingY, 2, -1, 1);
                    KingCheck(boardArray, pieces, WKingX, WKingY, 2, 1, -1);
                    KingCheck(boardArray, pieces, WKingX, WKingY, 2, -1, -1);
                    KingCheck(boardArray, pieces, WKingX, WKingY, 2, -1, 0);
                    KingCheck(boardArray, pieces, WKingX, WKingY, 2, 1, 0);
                    KingCheck(boardArray, pieces, WKingX, WKingY, 2, 0, -1);
                    KingCheck(boardArray, pieces, WKingX, WKingY, 2, 0, 1);
            
                    if (boardArray[CheckerX, CheckerY].Text == "BQ" || boardArray[CheckerX, CheckerY].Text == "BB" || boardArray[CheckerX, CheckerY].Text == "BR")
                    {
                        Paintboard(boardArray);
                        IsThisMoveRepeaterChecking = true;
                        allBlack();
                        Paintboard(boardArray);
                        MoveRepeater(boardArray, pieces, incrementor, CheckerX, CheckerY, 1, directionCX, directionCY);
        
                        for (int f = 0; f < 8; f++)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                if (boardArray[i, f].BackColor == Color.Yellow)
                                {
                                    attackArray[i, f] = true;
                                }


                            }
                        }
                        Paintboard(boardArray);
                        allWhite();
                        for (int f = 0; f < 8; f++)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                if (boardArray[i, f].BackColor == Color.Yellow && attackArray[i, f] == true)
                                {
                                    checkMate = false;
                                }
                            }
                        }
                    }
                    if (checkMate == true)
                    {

                        MessageBox.Show("Black wins, Checkmate!");
                        reset();
                    }
                }
            }
            else if (boardArray[BKingX, BKingY].BackColor == Color.Yellow)
            {

                MessageBox.Show("Black King is in check!");
                if (turns == 1)
                {
                    MessageBox.Show("That's an illegal move because your king is in check!");
                    mistake = true;
                }
                else
                {

                    Paintboard(boardArray);
                    allBlack();

                    if (boardArray[CheckerX, CheckerY].BackColor == Color.Yellow)
                    {
                        checkMate = false;
                    }
                    Paintboard(boardArray);
                    attacks = true;
                    allWhite();
                    attacks = false;
                    KingCheck(boardArray, pieces, BKingX, BKingY, 1, 1, 1);
                    KingCheck(boardArray, pieces, BKingX, BKingY, 1, -1, 1);
                    KingCheck(boardArray, pieces, BKingX, BKingY, 1, 1, -1);
                    KingCheck(boardArray, pieces, BKingX, BKingY, 1, -1, -1);
                    KingCheck(boardArray, pieces, BKingX, BKingY, 1, -1, 0);
                    KingCheck(boardArray, pieces, BKingX, BKingY, 1, 1, 0);
                    KingCheck(boardArray, pieces, BKingX, BKingY, 1, 0, -1);
                    KingCheck(boardArray, pieces, BKingX, BKingY, 1, 0, 1);


                    if (boardArray[CheckerX, CheckerY].Text == "WQ" || boardArray[CheckerX, CheckerY].Text == "WB" || boardArray[CheckerX, CheckerY].Text == "WR")
                    {
                        Paintboard(boardArray);
                        IsThisMoveRepeaterChecking = true;
                        allWhite();
                        Paintboard(boardArray);
                        MoveRepeater(boardArray, pieces, incrementor, CheckerX, CheckerY, 2, directionCX, directionCY);

                        for (int f = 0; f < 8; f++)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                if (boardArray[i, f].BackColor == Color.Yellow)
                                {
                                    attackArray[i, f] = true;
                                }


                            }
                        }
                        Paintboard(boardArray);
                        allWhite();
                        for (int f = 0; f < 8; f++)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                if (boardArray[i, f].BackColor == Color.Yellow && attackArray[i, f] == true)
                                {
                                    checkMate = false;
                                }
                            }
                        }
                    }
                    if (checkMate == true)
                    {

                        MessageBox.Show("White wins, Checkmate!");
                        reset();
                    }

                }
            }
            Paintboard(boardArray);

            WhichPieceIsChecking = false;
        }

        void pawns(Button[,] boardArray, int[,] pieces, int RealI, int RealF, int targetColor, int pawnadd)
        {
            int i = RealI;
            int f = RealF;
            if ((f - (1 * pawnadd) < 8) & (f - (1 * pawnadd) >= 0))
            {
                if (f - (1 * pawnadd) >= 0 & pieces[i, f - (1 * pawnadd)] == 0)
                {

                    boardArray[i, f - (1 * pawnadd)].BackColor = Color.Yellow;

                }
                if ((f - (2 * pawnadd) < 8) & (f - (2 * pawnadd) >= 0))
                {
                    if (RealF == 1 && targetColor == 2)
                    {
                        if (pieces[i, f - (2 * pawnadd)] == 0)
                        {

                            boardArray[i, f - (2 * pawnadd)].BackColor = Color.Yellow;
                        }

                    }
                    else if (RealF == 6 && targetColor == 1)
                    {
                        if (pieces[i, f - (2 * pawnadd)] == 0)
                        {

                            boardArray[i, f - (2 * pawnadd)].BackColor = Color.Yellow;
                        }

                    }
                }

                pawnsAttacks(boardArray, pieces, i, f, targetColor, pawnadd, false);


            }

            movementmemoryx = i;
            movementmemoryy = f;

        }
        void pawnsAttacks(Button[,] boardArray, int[,] pieces, int RealI, int RealF, int targetColor, int pawnadd, Boolean CheckOrMove)
        {
            int i = RealI;
            int f = RealF;

            if (i + (1) < 8 && i + (1) >= 0 && f - (1 * pawnadd) >= 0 && f - (1 * pawnadd) < 8)
            {
                if (pieces[i + (1), f - (1 * pawnadd)] == targetColor)
                {
                    boardArray[RealI + (1), RealF - (1 * pawnadd)].BackColor = Color.Yellow;
                }



            }
            if (i - (1) >= 0 && i - (1) < 8 && f - (1 * pawnadd) >= 0 && f - (1 * pawnadd) < 8)
            {

                if (pieces[i - (1), f - (1 * pawnadd)] == targetColor)
                {
                    boardArray[RealI - (1), RealF - (1 * pawnadd)].BackColor = Color.Yellow;
                }

            }
        }
        void MoveRepeater(Button[,] boardArray, int[,] pieces, int incrementor, int i, int f, int enemy, int angleX, int angleY)
        {
            while (true)
            {
                incrementor++;
                if ((f + (incrementor * angleY) < 8) & (f + (incrementor * angleY) >= 0) & (i + (incrementor * angleX) < 8) & (i + (incrementor * angleX) >= 0))
                {
                    if ((pieces[i + (incrementor * angleX), f + (incrementor * angleY)] == 0) || (pieces[i + (incrementor * angleX), f + (incrementor * angleY)] == enemy))
                    {
                        boardArray[i + (incrementor * angleX), f + (incrementor * angleY)].BackColor = Color.Yellow;
                        if (pieces[i + (incrementor * angleX), f + (incrementor * angleY)] == enemy)
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                else
                {
                    break;
                }

            }
            if (IsThisMoveRepeaterChecking == true)
            {
                if (boardArray[WKingX, WKingY].BackColor == Color.Yellow || boardArray[BKingX, BKingY].BackColor == Color.Yellow)
                {
                    directionCX = angleX;
                    directionCY = angleY;
                    IsThisMoveRepeaterChecking = false;
                }

            }




            movementmemoryx = i;
            movementmemoryy = f;
            incrementor = 0;

        }
        void KnightMovement(Button[,] boardArray, int[,] pieces, int i, int f, int enemy, int x, int y)
        {


            if ((f + (2 * y) < 8) & (f + (2 * y) >= 0) & (i + (1 * x) < 8) & (i + (1 * x) >= 0))
            {
                if ((pieces[i + (1 * x), f + (2 * y)] == 0) || (pieces[i + (1 * x), f + (2 * y)] == enemy))
                {
                    boardArray[i + (1 * x), f + (2 * y)].BackColor = Color.Yellow;
                }
            }

            if ((f + (1 * y) < 8) & (f + (1 * y) >= 0) & (i + (2 * x) < 8) & (i + (2 * x) >= 0))
            {
                if ((pieces[i + (2 * x), f + (1 * y)] == 0) || (pieces[i + (2 * x), f + (1 * y)] == enemy))
                {
                    boardArray[i + (2 * x), f + (1 * y)].BackColor = Color.Yellow;

                }
            }

            movementmemoryx = i;
            movementmemoryy = f;

        }
        void secondStepMove(Button[,] boardArray, int[,] pieces, int i, int f, int targetColor, int pawnadd)
        {
            int oldX = movementmemoryx;
            int oldY = movementmemoryy;
            Image takenPic = boardArray[i, f].Image;
            String takenPiece = boardArray[i, f].Text;
            int takenType = pieces[i, f];
            boardArray[i, f].Text = boardArray[movementmemoryx, movementmemoryy].Text;
            boardArray[i, f].Enabled = false;
            pieces[i, f] = pieces[movementmemoryx, movementmemoryy];
            pieces[movementmemoryx, movementmemoryy] = 0;
            boardArray[movementmemoryx, movementmemoryy].Text = "";
            boardArray[i, f].Image = boardArray[movementmemoryx, movementmemoryy].Image;
            boardArray[movementmemoryx, movementmemoryy].Image = null;
            Paintboard(boardArray);
            if (boardArray[i, f].Text == "WP" && f == 7)
            {
                boardArray[i, f].Text = "WQ";
                //boardArray[i, f].Image = Image.FromFile(@"C:\Users\Joshua\Pictures\WQueen.PNG");
            }
            if (boardArray[i, f].Text == "BP" && f == 0)
            {
                boardArray[i, f].Text = "BQ";
                //boardArray[i, f].Image = Image.FromFile(@"C:\Users\Joshua\Pictures\BQueen.PNG");
            }
            CheckKing(boardArray, pieces, i, f, targetColor, pawnadd);

            if (mistake == true)
            {
                pieces[oldX, oldY] = pieces[i, f];
                boardArray[oldX, oldY].Text = boardArray[i, f].Text;
                boardArray[oldX, oldY].Image = boardArray[i, f].Image;
                boardArray[i, f].Text = takenPiece;
                boardArray[i, f].Image = null;
                boardArray[i, f].Image = takenPic;
                pieces[i, f] = takenType;

                Enabler();
                disabler();
            }


            movementmemoryx = 0;
            movementmemoryy = 0;
            pieceSelected = false;
            mistake = false;
            Enabler();
        }
        void allWhite()
        {

            for (int f = 0; f < 8; f++)
            {
                for (int i = 0; i < 8; i++)
                {


                    enemy = 2;
                    pawnadd = -1;

                    if (boardArray[i, f].Text == "WP")
                    {
                        if (attacks == true)
                        {
                            pawnsAttacks(boardArray, pieces, i, f, enemy, pawnadd, true);
                        }
                        else
                        {
                            pawns(boardArray, pieces, i, f, enemy, pawnadd);
                        }
                    }
                    if (boardArray[i, f].Text == "WB")
                    {

                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 1, 1);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, -1, 1);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 1, -1);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, -1, -1);

                    }
                    if (boardArray[i, f].Text == "WR")
                    {
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, -1, 0);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 1, 0);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 0, -1);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 0, 1);
                    }
                    if (boardArray[i, f].Text == "WQ")
                    {
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 1, 1);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, -1, 1);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 1, -1);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, -1, -1);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, -1, 0);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 1, 0);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 0, -1);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 0, 1);
                    }
                    if (boardArray[i, f].Text == "WK")
                    {
                        KnightMovement(boardArray, pieces, i, f, enemy, 1, 1);
                        KnightMovement(boardArray, pieces, i, f, enemy, 1, -1);
                        KnightMovement(boardArray, pieces, i, f, enemy, -1, 1);
                        KnightMovement(boardArray, pieces, i, f, enemy, -1, -1);
                    }
                

                }






            }
        }
        void allBlack()
        {

            for (int f = 0; f < 8; f++)
            {
                for (int i = 0; i < 8; i++)
                {


                    enemy = 1;
                    pawnadd = 1;

                    if (boardArray[i, f].Text == "BP")
                    {
                        if (attacks == true)
                        {
                            pawnsAttacks(boardArray, pieces, i, f, enemy, pawnadd, true);
                        }
                        else
                        {
                            pawns(boardArray, pieces, i, f, enemy, pawnadd);
                        }


                    }
                    if (boardArray[i, f].Text == "BB")
                    {

                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 1, 1);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, -1, 1);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 1, -1);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, -1, -1);

                    }
                    if (boardArray[i, f].Text == "BR")
                    {
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, -1, 0);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 1, 0);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 0, -1);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 0, 1);
                    }
                    if (boardArray[i, f].Text == "BQ")
                    {
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 1, 1);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, -1, 1);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 1, -1);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, -1, -1);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, -1, 0);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 1, 0);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 0, -1);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 0, 1);
                    }
                    if (boardArray[i, f].Text == "BK")
                    {
                        KnightMovement(boardArray, pieces, i, f, enemy, 1, 1);
                        KnightMovement(boardArray, pieces, i, f, enemy, 1, -1);
                        KnightMovement(boardArray, pieces, i, f, enemy, -1, 1);
                        KnightMovement(boardArray, pieces, i, f, enemy, -1, -1);
                    }
                   

                }






            }
        }

        void allPieces()
        {

            for (int f = 0; f < 8; f++)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (pieces[i, f] == 1)
                    {
                        enemy = 2;
                        pawnadd = -1;
                    }
                    else
                    {
                        enemy = 1;
                        pawnadd = 1;
                    }
                    if (boardArray[i, f].Text == "BP" || boardArray[i, f].Text == "WP")
                    {
                        pawns(boardArray, pieces, i, f, enemy, pawnadd);

                    }
                    if (boardArray[i, f].Text == "WB" || boardArray[i, f].Text == "BB")
                    {

                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 1, 1);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, -1, 1);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 1, -1);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, -1, -1);

                    }
                    if (boardArray[i, f].Text == "WR" || boardArray[i, f].Text == "BR")
                    {
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, -1, 0);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 1, 0);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 0, -1);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 0, 1);
                    }
                    if (boardArray[i, f].Text == "BQ" || boardArray[i, f].Text == "WQ")
                    {
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 1, 1);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, -1, 1);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 1, -1);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, -1, -1);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, -1, 0);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 1, 0);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 0, -1);
                        MoveRepeater(boardArray, pieces, incrementor, i, f, enemy, 0, 1);
                    }
                    if (boardArray[i, f].Text == "BK" || boardArray[i, f].Text == "WK")
                    {
                        KnightMovement(boardArray, pieces, i, f, enemy, 1, 1);
                        KnightMovement(boardArray, pieces, i, f, enemy, 1, -1);
                        KnightMovement(boardArray, pieces, i, f, enemy, -1, 1);
                        KnightMovement(boardArray, pieces, i, f, enemy, -1, -1);
                    }
                    if (boardArray[i, f].Text == "WKing" || boardArray[i, f].Text == "BKing")
                    {
                        KingMove(boardArray, pieces, i, f, enemy, 1, 1);
                        KingMove(boardArray, pieces, i, f, enemy, -1, 1);
                        KingMove(boardArray, pieces, i, f, enemy, 1, -1);
                        KingMove(boardArray, pieces, i, f, enemy, -1, -1);
                        KingMove(boardArray, pieces, i, f, enemy, -1, 0);
                        KingMove(boardArray, pieces, i, f, enemy, 1, 0);
                        KingMove(boardArray, pieces, i, f, enemy, 0, -1);
                        KingMove(boardArray, pieces, i, f, enemy, 0, 1);

                    }
                    if (boardArray[WKingX, WKingY].BackColor == Color.Yellow || boardArray[BKingX, BKingY].BackColor == Color.Yellow)
                    {
                        if (WhichPieceIsChecking == false)
                        {
                            CheckerX = i;
                            CheckerY = f;
                            WhichPieceIsChecking = true;
                        }
                    }

                }





            }
        }
    }
}




