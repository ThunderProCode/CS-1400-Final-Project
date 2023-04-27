namespace GameServer
{
    class GameData
    {
        public bool ValidPreviousMovement { get; set; }
        public bool GameState { get; set; }
        public bool IsFull { get; set; }
        public char[][] GameBoard { get; set; }
        public bool YourTurn { get; set; }
        public int MyScore { get; set; }
        public int Player2Score {get; set; }
        public bool PlayersConnected {get; set; }
        public bool BothPlayingAgain { get; set; }

        public GameData(bool validPreviousMovement,bool gameState,bool isFull, char[][] gameBoard,bool yourTurn,int player2Score, int myScore, bool playersConnected, bool bothPlayingAgain)
        {
            ValidPreviousMovement = validPreviousMovement;
            GameState = gameState;
            GameBoard = gameBoard;
            YourTurn = yourTurn;
            IsFull = isFull;
            Player2Score = player2Score;
            MyScore = myScore;
            PlayersConnected = playersConnected;
            BothPlayingAgain = bothPlayingAgain;
        }

        public int GetMyScore()
        {
            return this.MyScore;
        }

        public int GetPlayer2Score()
        {
            return this.Player2Score;
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

        public void SetGameState(bool NewGameState)
        {
            this.GameState = NewGameState;
        }

        public char[][] GetGameBoard()
        {
            return this.GameBoard;
        }

        public bool GetYourTurn()
        {
            return this.YourTurn;
        }
        public void SetYourTurn(bool NewYourTurn)
        {
            this.YourTurn = NewYourTurn;
        }
    }
}