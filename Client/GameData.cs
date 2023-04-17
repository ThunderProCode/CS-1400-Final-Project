namespace GameClient
{
    class GameData
    {
        public bool ValidPreviousMovement {get; set; }
        public bool GameState {get; set; }
        public char[][] GameBoard {get; set; }
        public int Turn {get; set; }

        public GameData(bool validPreviousMovement,bool gameState, char[][] gameBoard,int turn)
        {
            ValidPreviousMovement = validPreviousMovement;
            GameState = gameState;
            GameBoard = gameBoard;
            Turn = turn;
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