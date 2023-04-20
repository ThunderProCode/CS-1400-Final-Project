namespace GameServer
{
    class GameData
    {
        public bool ValidPreviousMovement {get; set; }
        public bool GameState {get; set; }
        public bool IsFull {get; set; }
        public char[][] GameBoard {get; set; }
        public int Turn {get; set; }

        public GameData(bool validPreviousMovement,bool gameState,bool isFull, char[][] gameBoard,int turn)
        {
            ValidPreviousMovement = validPreviousMovement;
            GameState = gameState;
            GameBoard = gameBoard;
            Turn = turn;
            IsFull = isFull;
        }

        public bool GetIsFull(){
            return this.IsFull;
        }

        public void SetIsFull(bool newFullStatus){
            this.IsFull = newFullStatus;
        }

        public bool GetValidPreviousMovement()
        {
            return this.ValidPreviousMovement;
        }

        public bool GetGameState()
        {
            return this.GameState;
        }

        public char[][] GetGameBoard()
        {
            return this.GameBoard;
        }

        public int GetTurn()
        {
            return this.Turn;
        }
    }
}