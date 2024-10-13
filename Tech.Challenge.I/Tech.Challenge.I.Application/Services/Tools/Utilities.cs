namespace Tech.Challenge.I.Application.Services.Tools;
public static class Utilities
{
    public static (int, int) ValidatePagination(int pageNumber, int pageSize) =>
        ((pageNumber < 1) ? 1 : pageNumber, (pageSize < 1) ? 1 : pageSize);
}
