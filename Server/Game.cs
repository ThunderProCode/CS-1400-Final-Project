namespace GameServer
{
    class Game
    {
        private char[,] board = new char[3, 3]
        {
            { ' ', ' ', ' ' },
            { ' ', ' ', ' ' },
            { ' ', ' ', ' ' }
        };
        private char player1;
        private char player2;
        private int turn = 1;
        // State means - True game running - False game stopped because someone won or left
        private bool state = false;
        public Game(char player1,char player2)
        {
            this.player1 = player1;
            this.player2 = player2;
            this.state = true;
        }

        // Get the board contents 
        public char[,] GetBoard()
        {
            return this.board;
        }

        public void SetBoard(char[,] newBoard)
        {
            this.board = newBoard;
        }

        // Get current state of the game
        public bool GetState()
        {
            return this.state;
        }

        // Set current state of the game
        public void SetState(bool NewState)
        {
            this.state = NewState;
        }

        // Print the current state of the board
        public void Print()
        {
            System.Console.WriteLine();
            System.Console.WriteLine(" --TIC-TAC-TOE--");
            System.Console.WriteLine();
            System.Console.WriteLine("   0    1   2");
            Console.WriteLine("  ┌───┬───┬───┐");
            for (int row = 0; row < 3; row++)
            {

                Console.Write($"{row} │");
                for (int col = 0; col < 3; col++)
                {
                    System.Console.Write($"{this.board[row,col]}  |");
                }
                System.Console.WriteLine();
                if(row != 2)
                {
                    System.Console.WriteLine("  ├───┼───┼───┤");
                }
            }
            Console.WriteLine("  └───┴───┴───┘");
        }
    
        // Receive user movement coordinates and place on board
        public bool Move(int[] position,char player){
            if(IsMoveValid(position)){
                this.board[position[0],position[1]] = player;
                return true;
            }
            return false;
        }

        // Validate that the user movement is valid
        public bool IsMoveValid(int[] position){
            if(position[0] < 0 || position[0] > 2 || position[1] < 0 || position[1] > 2)
            {
                if((board[position[0],position[1]] == player1) || (board[position[0],position[1]] == player2))
                {
                    return false;
                }
            }
            return true;
        }

    }
}