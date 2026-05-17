namespace AcaHelpAPI.Helpers
{
    public enum ApiSuccessResponseCodes
    {
        LOGGED,
        QUESTION_CREATED,
        QUESTION_GIVEN,
        QUESTIONS_GIVEN,
        ANSWER_CREATED,
        ANSWER_GIVEN,
        ANSWERS_GIVEN,
        TAGS_GIVEN,

    }
    public enum ApiErrorResponseCodes
    {
        INVALID_CREDENTIALS,
        INTERNAL_SERVER_ERROR,
        USER_NOT_FOUND,
        QUESTION_NOT_FOUND,
        ANSWER_NOT_FOUND,
        TAG_NOT_FOUND,
        USER_ALREADY_EXISTS,
        INVALID_INPUT_DATA,
        UNAUTHORIZED_ACCESS,
        FORBIDDEN_ACCESS
    }
}
