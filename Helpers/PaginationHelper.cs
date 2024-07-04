using Newtonsoft.Json;

namespace SecondRoundProject.Helpers
{
    public static class PaginationHelper
    {
        public static string CreatePaginationHeader(int currentPage, int pageSize, int totalCount)
        {
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var paginationHeader = new
            {
                currentPage,
                pageSize,
                totalCount,
                totalPages
            };

            return JsonConvert.SerializeObject(paginationHeader);
        }
    }
}
