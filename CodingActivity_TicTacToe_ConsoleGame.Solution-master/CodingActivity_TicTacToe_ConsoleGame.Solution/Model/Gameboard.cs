using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

//using TicTacToe.ConsoleApp.Model;

namespace CodingActivity_TicTacToe_ConsoleGame
{
    public class Gameboard
    {
        #region ENUMS

        public enum PlayerPiece
        {
            X,
            O,
            None
        }

        public enum GameboardState
        {
            NewRound,
            PlayerXTurn,
            PlayerOTurn,
            PlayerXWin,
            PlayerOWin,
            CatsGame
        }

        #endregion

        #region FIELDS

        private const int MAX_NUM_OF_ROWS_COLUMNS = 3;

        private PlayerPiece[,] _firstBoard;
        private PlayerPiece[,] _secondBoard;
        private PlayerPiece[,] _thirdBoard;

        private List<PlayerPiece[,]> _boards;

        private GameboardState _currentRoundState;

        #endregion

        #region PROPERTIES

        public int MaxNumOfRowsColumns
        {
            get { return MAX_NUM_OF_ROWS_COLUMNS; }
        }

        public List<PlayerPiece[,]> Boards
        {
            get { return _boards; }
        }

        public PlayerPiece[,] FirstBoard
        {
            get { return _firstBoard; }
            set { _firstBoard = value; }
        }

        public PlayerPiece[,] SecondBoard
        {
            get { return _secondBoard; }
            set { _secondBoard = value; }
        }

        public PlayerPiece[,] ThirdBoard
        {
            get { return _thirdBoard; }
            set { _thirdBoard = value; }
        }

        public GameboardState CurrentRoundState
        {
            get { return _currentRoundState; }
            set { _currentRoundState = value; }
        }
        #endregion

        #region CONSTRUCTORS

        public Gameboard()
        {
            _firstBoard = new PlayerPiece[MAX_NUM_OF_ROWS_COLUMNS, MAX_NUM_OF_ROWS_COLUMNS];
            _secondBoard = new PlayerPiece[MAX_NUM_OF_ROWS_COLUMNS, MAX_NUM_OF_ROWS_COLUMNS];
            _thirdBoard = new PlayerPiece[MAX_NUM_OF_ROWS_COLUMNS, MAX_NUM_OF_ROWS_COLUMNS];

            _boards = new List<PlayerPiece[,]>();

            _boards.Add(_firstBoard);
            _boards.Add(_secondBoard);
            _boards.Add(_thirdBoard);

            InitializeGameboard();
        }

        #endregion

        #region METHODS

        /// <summary>
        /// fill the game board array with "None" enum values
        /// </summary>
        public void InitializeGameboard()
        {
            _currentRoundState = GameboardState.NewRound;

            //
            // Set all PlayerPiece array values to "None"
            //
            foreach (PlayerPiece[,] board in _boards)
            {
                for (int row = 0; row < MAX_NUM_OF_ROWS_COLUMNS; row++)
                {
                    for (int column = 0; column < MAX_NUM_OF_ROWS_COLUMNS; column++)
                    {
                        board[row, column] = PlayerPiece.None;
                    }
                }
            }
        }


        /// <summary>
        /// Determine if the game board position is taken
        /// </summary>
        /// <param name="gameboardPosition"></param>
        /// <returns>true if position is open</returns>
        public bool GameboardPositionAvailable(GameboardPosition gameboardPosition)
        {
            //
            // Confirm that the board position is empty
            // Note: gameboardPosition converted to array index by subtracting 1
            //

            Boolean isAvailable = false;

            switch (gameboardPosition.Layer)
            {
                case 1:
                    if (_firstBoard[gameboardPosition.Row - 1, gameboardPosition.Column - 1] == PlayerPiece.None)
                    {
                        isAvailable = true;
                    }
                    break;
                case 2:
                    if (_secondBoard[gameboardPosition.Row - 1, gameboardPosition.Column - 1] == PlayerPiece.None)
                    {
                        isAvailable = true;
                    }
                    break;
                case 3:
                    if (_thirdBoard[gameboardPosition.Row - 1, gameboardPosition.Column - 1] == PlayerPiece.None)
                    {
                        isAvailable = true;
                    }
                    break;
            }

            return isAvailable;
        }

        /// <summary>
        /// Update the game board state if a player wins or a cat's game happens.
        /// </summary>
        public void UpdateGameboardState()
        {
            if (ThreeInARow(PlayerPiece.X))
            {
                _currentRoundState = GameboardState.PlayerXWin;
            }
            //
            // A player O has won
            //
            else if (ThreeInARow(PlayerPiece.O))
            {
                _currentRoundState = GameboardState.PlayerOWin;
            }
            //
            // All positions filled
            //
            else if (IsCatsGame())
            {
                _currentRoundState = GameboardState.CatsGame;
            }
        }

        public bool IsCatsGame()
        {
            //
            // All positions on board are filled and no winner
            //

            Boolean isCatsGame = false;

            foreach (PlayerPiece[,] board in _boards)
            {
                for (int row = 0; row < 3; row++)
                {
                    for (int column = 0; column < 3; column++)
                    {
                        if (board[row, column] != PlayerPiece.None)
                        {
                            isCatsGame = true;
                        }
                        else
                        {
                            isCatsGame = false;
                        }
                    }
                }
            }

            return isCatsGame;
        }

        /// <summary>
        /// Check for any three in a row.
        /// </summary>
        /// <param name="playerPieceToCheck">Player's game piece to check</param>
        /// <returns>true if a player has won</returns>
        private bool ThreeInARow(PlayerPiece playerPieceToCheck)
        {
            Boolean isWon = false;

            isWon = CheckLayerWin(playerPieceToCheck);

            isWon = Check3DWin(playerPieceToCheck);

            //
            // No Player Has Won
            //

            return isWon;
        }

        private bool Check3DWin(PlayerPiece playerPieceToCheck)
        {
            Boolean isWon = false;
            //
            // Check rows for player win
            //
            for (int row = 0; row < 3; row++)
            {
                if (_boards[0][row, row] == playerPieceToCheck &&
                    _boards[1][row, row] == playerPieceToCheck &&
                    _boards[2][row, row] == playerPieceToCheck)
                {
                    isWon = true;
                    break;
                }
            }
            //
            // Check columns for player win
            //
            for (int column = 0; column < 3; column++)
            {
                if (_boards[0][column, column] == playerPieceToCheck &&
                    _boards[1][column, column] == playerPieceToCheck &&
                    _boards[2][column, column] == playerPieceToCheck)
                {
                    isWon = true;
                    break;
                }
            }

            //
            // Check diagonals for player win (8 ways)
            //
            if (
                (_boards[0][0, 0] == playerPieceToCheck &&
                 _boards[1][1, 1] == playerPieceToCheck &&
                 _boards[2][2, 2] == playerPieceToCheck)
                ||
                (_boards[0][0, 2] == playerPieceToCheck &&
                 _boards[1][1, 1] == playerPieceToCheck &&
                 _boards[2][2, 0] == playerPieceToCheck)
                 ||
                 (_boards[0][0, 0] == playerPieceToCheck &&
                 _boards[1][0, 1] == playerPieceToCheck &&
                 _boards[2][0, 2] == playerPieceToCheck)
                 ||
                 (_boards[0][1, 0] == playerPieceToCheck &&
                 _boards[1][1, 1] == playerPieceToCheck &&
                 _boards[2][1, 2] == playerPieceToCheck)
                 ||
                 (_boards[0][2, 0] == playerPieceToCheck &&
                 _boards[1][2, 1] == playerPieceToCheck &&
                 _boards[2][2, 2] == playerPieceToCheck)
                 ||
                 (_boards[0][0, 2] == playerPieceToCheck &&
                 _boards[1][0, 1] == playerPieceToCheck &&
                 _boards[2][0, 0] == playerPieceToCheck)
                 ||
                 (_boards[0][1, 2] == playerPieceToCheck &&
                 _boards[1][1, 1] == playerPieceToCheck &&
                 _boards[2][1, 0] == playerPieceToCheck)
                 ||
                 (_boards[0][2, 2] == playerPieceToCheck &&
                 _boards[1][2, 1] == playerPieceToCheck &&
                 _boards[2][2, 0] == playerPieceToCheck)
                )
            {
                isWon = true;
            }

            return isWon;
        }

        private bool CheckLayerWin(PlayerPiece playerPieceToCheck)
        {
            Boolean isWon = false;

            foreach (PlayerPiece[,] board in _boards)
            {
                //
                // Check rows for player win
                //
                for (int row = 0; row < 3; row++)
                {
                    if (board[row, 0] == playerPieceToCheck &&
                        board[row, 1] == playerPieceToCheck &&
                        board[row, 2] == playerPieceToCheck)
                    {
                        isWon = true;
                    }
                }
                //
                // Check columns for player win
                //
                for (int column = 0; column < 3; column++)
                {
                    if (board[0, column] == playerPieceToCheck &&
                        board[1, column] == playerPieceToCheck &&
                        board[2, column] == playerPieceToCheck)
                    {
                        isWon = true;
                    }
                }
                //
                // Check diagonals for player win
                //
                if (
                    (board[0, 0] == playerPieceToCheck &&
                     board[1, 1] == playerPieceToCheck &&
                     board[2, 2] == playerPieceToCheck)
                    ||
                    (board[0, 2] == playerPieceToCheck &&
                     board[1, 1] == playerPieceToCheck &&
                     board[2, 0] == playerPieceToCheck)
                    )
                {
                    isWon = true;
                }
            }

            return isWon;
        }

        /// <summary>
        /// Add player's move to the game board.
        /// </summary>
        /// <param name="gameboardPosition"></param>
        /// <param name="PlayerPiece"></param>
        public void SetPlayerPiece(GameboardPosition gameboardPosition, PlayerPiece PlayerPiece)
        {
            //
            // Row and column value adjusted to match array structure
            // Note: gameboardPosition converted to array index by subtracting 1
            //
            switch (gameboardPosition.Layer)
            {
                case 1:
                    _firstBoard[gameboardPosition.Row - 1, gameboardPosition.Column - 1] = PlayerPiece;
                    break;
                case 2:
                    _secondBoard[gameboardPosition.Row - 1, gameboardPosition.Column - 1] = PlayerPiece;
                    break;
                case 3:
                    _thirdBoard[gameboardPosition.Row - 1, gameboardPosition.Column - 1] = PlayerPiece;
                    break;
            }

            //
            // Change game board state to next player
            //
            SetNextPlayer();
        }

        /// <summary>
        /// Switch the game board state to the next player.
        /// </summary>
        private void SetNextPlayer()
        {
            if (_currentRoundState == GameboardState.PlayerXTurn)
            {
                _currentRoundState = GameboardState.PlayerOTurn;
            }
            else
            {
                _currentRoundState = GameboardState.PlayerXTurn;
            }
        }

        #endregion
    }
}

