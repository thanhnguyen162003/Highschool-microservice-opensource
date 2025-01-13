namespace Domain.Constants.Guide
{
    public class GuideHelper
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class GuideAbly
    {
        public static GuideHelper RoomChannel = new GuideHelper()
        {
            Name = AblyConstant.RoomChannel,
            Description = "A communication channel representing a specific game room. This channel is used for broadcasting messages and events to all participants within the room."
        };

        public static GuideHelper PlayerJoined = new GuideHelper()
        {
            Name = AblyConstant.PlayerJoinedEvent,
            Description = "An event triggered when a new player successfully joins the game room. This event notifies all participants in the room about the new player, allowing the UI to update and display the updated list of players."
        };

        public static GuideHelper GameStarted = new GuideHelper()
        {
            Name = AblyConstant.GameStartedEvent,
            Description = "An event triggered when the host starts the game. This event initializes the game session, alerts all participants, and transitions the UI to the gameplay phase."
        };

        public static GuideHelper NewQuestion = new GuideHelper()
        {
            Name = AblyConstant.NewQuestionEvent,
            Description = "An event triggered when a new question is presented during the game. This event sends the question data to all participants in the room, enabling them to respond within a specific time frame."
        };

        public static GuideHelper GameFinished = new GuideHelper()
        {
            Name = AblyConstant.GameFinishedEvent,
            Description = "An event triggered when the game session concludes. This event displays the final results, declares the winner, and allows participants to view the leaderboard and other relevant information."
        };

    }

}
