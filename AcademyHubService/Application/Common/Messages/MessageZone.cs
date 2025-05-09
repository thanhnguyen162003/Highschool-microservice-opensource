namespace Application.Common.Messages
{
    public class MessageZone
    {
        public const string MemberBanned = "Member has been banned from the zone";
        public const string MemberNotBanned = "Member could not be banned from the zone";
        public const string MemberExists = "Member already exists in the zone";
        public const string MemberNotEnough = "Member is not enough";
        public const string CannotInviteAfter24h = "Cannot invite member after 24 hours";

        public const string InviteMemberSuccess = "Member has been invited to the zone";
        public const string InviteIsNotFound = "Member is not invite to this zone";
        public const string JoinZoneSuccess = "Member has joined the zone";
        public const string RejectInviteSuccess = "Member has rejected the invite";
        public const string InviteIsExpired = "Invite is expired";

        public const string GroupMemberSuccess = "Group members have been divided";
        public const string GroupMemberFailed = "Group members could not be divided";

        public const string AssignmentCreatedSuccess = "Assignment has been created success";
        public const string AssignmentCreatedFailed = "Assignment could not be created";

        public const string SubmissionCreatedSuccess = "Nộp bài thành công";
        public const string SubmissionCreatedFailed = "Nộp bài thất bại";

        public const string TestContentCreatedSuccess = "Câu hỏi tạo thành công";
        public const string TestContentcreatedFailed = "Câu hỏi tạo thất bại";
    }
}
