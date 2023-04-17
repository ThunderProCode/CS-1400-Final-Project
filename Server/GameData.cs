namespace GameServer
{
    class GameData
    {
        public bool ValidPreviousMovement {get; set; }
        public bool GameState {get; set; }
        public char[][] GameBoard {get; set; }

        public GameData(bool validPreviousMovement,bool gameState, char[][] gameBoard)
        {
            ValidPreviousMovement = validPreviousMovement;
            GameState = gameState;
            GameBoard = gameBoard;
        }

        public bool GetValidPreviousMovement(){
            return this.ValidPreviousMovement;
        }

        public bool GetGameState(){
            return this.GameState;
        }

        public char[][] GetGameBoard(){
            return this.GameBoard;
        }

    }
}