public static class ChatConstants
{
    public const int MAX_MESSAGE_LENGTH = 5000;
    public const int DEFAULT_PAGE_SIZE = 20;
    public const string MESSAGE_TYPE_TEXT = "Text";
    public const string MESSAGE_TYPE_IMAGE = "Image";

    public static class HubMethods
    {
        public const string RECEIVE_MESSAGE = "ReceiveMessage";
        public const string USER_TYPING = "UserTyping";
        public const string USER_ONLINE = "UserOnline";
        public const string USER_OFFLINE = "UserOffline";
    }

    public enum MessageStatus
    {
        Sent,
        Received,
        Read
    }
}
