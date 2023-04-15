namespace GameServer
{
    class Game
    {
        private char[,] board = new char[3,3];
        private char player1;
        private char player2;
        private int turn = 1;
        public Game(char player1,char player2){
            this.player1 = player1;
            this.player2 = player2;
        }
    
        public void move(int[,] position,char player){
            
        }

        public bool isMoveValid(int[] position){
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