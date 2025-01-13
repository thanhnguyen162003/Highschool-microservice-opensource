namespace Application.Messages
{
    public class MessageGame
    {
        public const string CreaetRoomSuccess = "Create room success";
        public const string CreateRoomFail = "Create room fail";

        public const string JoinRoomSuccess = "Join room success";
        public const string JoinRoomFail = "Join room fail";

        public const string StartGameSuccess = "Start game success";
        public const string StartGameFail = "Start game fail";

        public const string ErrorWhileProcessJoinGame = "Error while process join game. Please, try again!";
        public const string NetworkError = "Network error. Please, try again!";

        public const string QuestionNotFound = "Question not found. Maybe you are not init question or error while processing! Please check and try again.";
        public const string KetContentEmpty = "Ket content is empty. Please, add some ket content to start the game!";

        public const string RoomNotFound = "Room not found. Maybe you are not init room or error while processing! Please check and try again.";
        public const string RoomFound = "Room found. Enter you name before play game!";

        public const string FinishGame = "Finish game!";

        public const string DeleteGameSuccess = "Delete game success";

        public const string KickPlayerSuccess = "Kick player success";
        public const string KickPlayerFail = "Kick player fail";
        public const string YouAreKicked = "You are kicked from the game!";

        public const string UpdatePlayerInfoSuccess = "Update player info success";
        public const string UpdatePlayerInfoFail = "Update player info fail. Please, check network and try again!";

        public const string SelectAnswerSuccess = "Select answer success";
        public const string SelectAnswerFail = "Select answer fail. Please, check network and try again!";
    }
}
