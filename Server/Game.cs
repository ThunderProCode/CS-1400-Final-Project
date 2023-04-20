namespace GameServer
{
    class Game
    {
        private char[][] board = new char[3][]
        {
            new char[]{ ' ', ' ', ' ' },
            new char[]{ ' ', ' ', ' ' },
            new char[]{ ' ', ' ', ' ' }
        };
        private char player1;
        private char player2;
        private int turn = 1;
        // State means - True game running - False game stopped because someone won or left
        private bool state = false;
        public Game(char player1,char player2)
        {
            Random rnd = new Random();
            this.player1 = player1;
            this.player2 = player2;
            this.state = true;
            this.turn = rnd.Next(1,3);
        }
        // Get the current player turn
        public int getTurn(){
            return this.turn;
        }
        // Set the current player turn
        public void setTurn(int newTurn){
            this.turn = newTurn;
        }
        // Get the board contents 
        public char[][] GetBoard()
        {
            return this.board;
        }

        public void SetBoard(char[][] newBoard)
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

        // Returns true if all the spaces on the board are filled
        public bool isFull(){
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if(this.board[row][col] == ' '){
                        return false;
                    }
                }
            }
            return true;
        }

        // check if a player won the game
        public bool IsWin(){
            // check if there is a horizontal win
            for (int row = 0; row < 3; row++)
            {
                if(this.board[row][0] == player1 && this.board[row][1] == player1 && this.board[row][2] == player1) return true;
                if(this.board[row][0] == player2 && this.board[row][1] == player2 && this.board[row][2] == player2) return true;
            }
            // check if there is a vertical win
            for (int col = 0; col < 3; col++)
            {
                if(this.board[0][col] == player1 && this.board[1][col] == player1 && this.board[2][col] == player1) return true;
                if(this.board[0][col] == player2 && this.board[1][col] == player2 && this.board[2][col] == player2) return true;
            }
            // check if there is a diagonal right win
            if(this.board[0][0] == player1 && this.board[1][1] == player1 && this.board[2][2] == player1)return true;
            if(this.board[0][0] == player2 && this.board[1][1] == player2 && this.board[2][2] == player2)return true;
            // check if there is a diagonal left win
            if(this.board[0][2] == player1 && this.board[1][1] == player1 && this.board[2][0] == player1)return true;
            if(this.board[0][2] == player2 && this.board[1][1] == player2 && this.board[2][0] == player2)return true;

            return false;            
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
                    System.Console.Write($"{this.board[row][col]}  |");
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
        // Make sure to validate coordinates with IsMoveValid() before calling Move()
        public void Move(int[] position,char player){
            this.board[position[0]][position[1]] = player;
        }

        // Validate that the user movement is valid
        public bool IsMoveValid(int[] position){
            if(position[0] < 0 || position[0] > 2 || position[1] < 0 || position[1] > 2) return false;
            if(board[position[0]][position[1]] == player1 || board[position[0]][position[1]] == player2 ) return false;
            return true;
        }

    }
}